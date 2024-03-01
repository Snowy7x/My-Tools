using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Systems.Advanced_Character.Movement;
using Systems.IK.Base;
using UnityEditor.Animations;
using UnityEngine;

public enum ActivationRequirement
{
    VelocityXZ,
    VelocityY,
    Velocity,
    Custom
}

public enum AnimationLayer
{
    Base,
    Override
}

[Serializable] public class AnimationState
{
    public string stateName;
    public AnimationClip animationClip;
    public float animationSpeed;
    public AnimationLayer animationLayer;
    public float animationTransitionTime;
    public ActivationRequirement activationRequirement;
    public float requirementValue;
}

public class AnimationManager : MonoBehaviour
{
    public BaseIK baseIK;
    [Header("Animations Settings")]
    public AvatarMask defaultAvatar;
    public AvatarMask overrideAvatar;
    public List<AnimationState> animationStates = new List<AnimationState>();
    public Transform animatedTarget;
    public string defaultAnimationStateName;
    
    [Header("IK Settings")]
    public Transform rightHandTarget;
    public Transform rightHandPole;
    public Transform leftHandTarget;
    public Transform leftHandPole;
    
    private AnimationState defaultAnimationState;
    private AnimationState currentAnimationState;
    private Animator animator;
    private AnimatorController animatorController;
    
    // Velocity Magnitude Calculation Variables
    Vector3 velocity;
    private Vector3 previousPosition;
    private float velocityMagnitude;
    private float velocityY;
    private float velocityXZ;
    private Vector3 defaultLeftHandPosition;
    private Vector3 defaultRightHandPosition;
    private MovementBrain movementBrain;
    
    private void Awake()
    {
        Init();
        movementBrain = GetComponentInParent<MovementBrain>();
    }
    
    [ContextMenu("Init")]
    private void Init()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!baseIK) baseIK = GetComponent<BaseIK>();
        animator.runtimeAnimatorController = null;
        animatorController = AnimatorController.CreateAnimatorControllerAtPath($"Assets/Systems/Advanced Character/AnimationControllers/{gameObject.name}.controller");
        for (int i = 0; i < animatorController.layers.Length; i++)
        {
            if (i == 0) continue;
            animatorController.RemoveLayer(i);
        }
        
        animatorController.AddLayer(new AnimatorControllerLayer
        {
            avatarMask = defaultAvatar,
            iKPass = true,
            name = "C Base Layer",
            defaultWeight = 1,
            stateMachine = new AnimatorStateMachine()
        });
        
        animatorController.AddLayer(new AnimatorControllerLayer 
        {
            avatarMask = overrideAvatar,
            iKPass = true,
            name = "Override Layer",
            defaultWeight = 1,
            blendingMode = AnimatorLayerBlendingMode.Override,
            stateMachine = new AnimatorStateMachine()
        });
        
        // Remove other layers
        for (int i = 0; i < animatorController.layers.Length; i++)
        {
            if (i == animatorController.layers.Length - 1 || i == animatorController.layers.Length - 2) continue;
            animatorController.RemoveLayer(i);
        }
        
        animator.runtimeAnimatorController = animatorController;
        
        // Add the default animation state to animator controller
        defaultAnimationState = animationStates.Find(x => x.stateName == defaultAnimationStateName);
        if (defaultAnimationState != null)
        {
            AnimatorState animatorState = animatorController.AddMotion(defaultAnimationState.animationClip, 0);
            animatorState.name = defaultAnimationState.stateName;
            animatorState.speed = defaultAnimationState.animationSpeed;
            animatorState.motion = defaultAnimationState.animationClip;
            
            // Add it to the additive layer
            animatorState = animatorController.AddMotion(defaultAnimationState.animationClip, 1);
            animatorState.name = defaultAnimationState.stateName;
            animatorState.speed = defaultAnimationState.animationSpeed;
            animatorState.motion = defaultAnimationState.animationClip;
        }

        AnimationState[] animatorStates = animationStates.Where(x => x != defaultAnimationState).ToArray();
        
        // Add new states to animator controller
        foreach (AnimationState state in animatorStates)
        {
            AnimatorState animatorState = animatorController.AddMotion(state.animationClip, state.animationLayer == AnimationLayer.Base ? 0 : 1);
            animatorState.name = state.stateName;
            animatorState.speed = state.animationSpeed;
            animatorState.motion = state.animationClip;
        }
        
        // Set default state
        animator.Play(defaultAnimationStateName);
    }

    private void Start()
    {
         defaultAnimationState = animationStates.Find(x => x.stateName == defaultAnimationStateName);
        currentAnimationState = defaultAnimationState;
        
        // Set IK Targets
        ChainIK rightHandChain = new ChainIK();
        rightHandChain.transform = animator.GetBoneTransform(HumanBodyBones.RightHand);
        rightHandChain.Target = rightHandTarget;
        rightHandChain.Pole = rightHandPole;
        rightHandChain.Iterations = 3;
        rightHandChain.Delta = 0.001f;
        
        ChainIK leftHandChain = new ChainIK();
        leftHandChain.transform = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        leftHandChain.Target = leftHandTarget;
        leftHandChain.Pole = leftHandPole;
        leftHandChain.Iterations = 3;
        leftHandChain.Delta = 0.001f;
        
        rightHandChain.Init();
        leftHandChain.Init();
        
        baseIK.chains.Add(rightHandChain);
        baseIK.chains.Add(leftHandChain);
        
        defaultLeftHandPosition = leftHandTarget.localPosition;
        defaultRightHandPosition = rightHandTarget.localPosition;
    }

    private void Update()
    {
        CalculateVelocityMagnitude();
        UpdateAnimationState();
    }

    private void CalculateVelocityMagnitude()
    {
        Vector3 pos = transform.position;
        Vector2 pos2D = new Vector2(pos.x, pos.z);
        Vector2 previousPos2D = new Vector2(previousPosition.x, previousPosition.z);
        velocity = (transform.position - previousPosition);
        velocityXZ = (pos2D - previousPos2D).magnitude / Time.deltaTime;
        velocityY = velocity.y / Time.deltaTime;
        velocityMagnitude = velocity.magnitude / Time.deltaTime;
        previousPosition = transform.position;
    }

    private void UpdateAnimationState()
    {
        AnimationState animationState = defaultAnimationState;

        foreach (AnimationState state in animationStates)
        {
            switch (state.activationRequirement)
            {
                case ActivationRequirement.VelocityXZ:
                    if (velocityXZ >= state.requirementValue)
                    {
                        animationState = state;
                    }

                    break;
                
                case ActivationRequirement.VelocityY:
                    if (velocityY >= state.requirementValue)
                    {
                        animationState = state;
                    }
                    break;
                case ActivationRequirement.Velocity:
                    if (velocityMagnitude >= state.requirementValue)
                    {
                        animationState = state;
                    }
                    break;
                case ActivationRequirement.Custom:
                    break;
            }
        }

        if (currentAnimationState != animationState)
        {
            animator.CrossFade(animationState.stateName, animationState.animationTransitionTime);
            currentAnimationState = animationState;
        }
    }
    
    public void ResetIKHandTargets()
    {
        rightHandTarget.localPosition = defaultRightHandPosition;
        leftHandTarget.localPosition = defaultLeftHandPosition;
    }
    
    public void PlayOverrideAnimation(string animationStateName, int layer = 1, float transitionTime = 0.15f)
    {
        // play ovveride animation
        AnimationState animationState = animationStates.Find(x => x.stateName == animationStateName);
        if (animationState != null)
        {
            animator.CrossFade(animationStateName, transitionTime, layer);
        }
    }
    
    public void PlayIdle(int layer = 1, float transitionTime = 0.15f)
    {
        // play override animation
        animator.CrossFade(defaultAnimationStateName, transitionTime, layer);
    }
    
    public void PlayBaseAnimation(string animationStateName, int layer = 0, float transitionTime = 0.15f)
    {
        // play base animation
        AnimationState animationState = animationStates.Find(x => x.stateName == animationStateName);
        if (animationState != null)
        {
            animator.CrossFade(animationStateName, transitionTime, layer);
        }
    }
    
    [CanBeNull] public AnimationState GetAnimationState(string stateName = null)
    {
        return animationStates.Find(x => x.stateName == stateName);
    }
}
