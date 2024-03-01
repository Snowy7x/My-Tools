using Systems.Advanced_Character.Animation_System.Cycles;
using Systems.Advanced_Character.Movement;
using UnityEditor.Animations;
using UnityEngine;

namespace Systems.Advanced_Character.Animation_System
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimator : MonoBehaviour
    {
        private Animator _animator;
        [Header("Animation Cycles")]
        [SerializeField] private LocomotionCycle _locomotionCycle;

        [Header("Avatar Masks")]
        [SerializeField] private AvatarMask defaultAvatar;
        [SerializeField] private AvatarMask overrideAvatar;
        
        private MovementBrain _movementBrain;
        private AnimatorController _controller;
        private AnimatorOverrideController _overrideController;
        
        private State _previousState;
        
        [ContextMenu("Set Up Animator")]
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null) _animator = gameObject.AddComponent<Animator>();
            _movementBrain  = GetComponentInParent<MovementBrain>();
            if (_movementBrain == null) Debug.LogError("No MovementBrain found in parent!", this);
            if (_locomotionCycle == null) Debug.LogError("No LocomotionCycle found!", this);
            
            SetUpAnimator();
        }
        
        private void Update()
        {
            UpdateLocomotion();
        }
        
        private void SetUpAnimator()
        {
            _animator.runtimeAnimatorController = null;
            _controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Systems/Advanced Character/Animation System/Controllers/CharacterController.controller");
            
            // Set up layers
            for (int i = 0; i < _controller.layers.Length; i++)
            {
                if (i == 0) continue;
                _controller.RemoveLayer(i);
            }
        
            _controller.AddLayer(new AnimatorControllerLayer{
                avatarMask = overrideAvatar,
                iKPass = true,
                name = "Override Layer",
                defaultWeight = 1,
                blendingMode = AnimatorLayerBlendingMode.Override,
                stateMachine = new AnimatorStateMachine()
            });
            
            // Set up states
            foreach (var state in _locomotionCycle.GetType().GetFields())
            {
                if (state.FieldType != typeof(AnimationState)) continue;
                AnimationState animationState = (AnimationState) state.GetValue(_locomotionCycle);
                if (animationState.animationClip == null) continue;
                AnimatorState animatorState = _controller.AddMotion(animationState.animationClip, 0);
                animatorState.name = animationState.stateName;
                animatorState.speed = animationState.animationSpeed;
                animatorState.motion = animationState.animationClip;
                
                // Add it to the override layer
                animatorState = _controller.AddMotion(animationState.animationClip, 1);
                animatorState.name = animationState.stateName;
                animatorState.speed = animationState.animationSpeed;
                animatorState.motion = animationState.animationClip;
            }
            
            _overrideController = new AnimatorOverrideController(_controller);
            _animator.runtimeAnimatorController = _overrideController;
            
        }

        private void UpdateLocomotion()
        {
            if (_animator == null) return;
            if (_locomotionCycle == null) return;
            
            // Update the current running animation according to the current state
            if (_previousState != _movementBrain.State)
            {
                switch (_movementBrain.State)
                {
                    case State.Idle:
                        PlayAnimation(_locomotionCycle.idle);
                        break;
                    case State.Walking:
                        PlayAnimation(_locomotionCycle.walk);
                        break;
                    case State.Running:
                        PlayAnimation(_locomotionCycle.run);
                        break;
                    case State.Falling:
                        PlayAnimation(_locomotionCycle.fall);
                        break;
                }
            }
            
            // Update the previous state
            _previousState = _movementBrain.State;
        }
        
        public void PlayAnimationInstantly(string stateName, int layer = 0)
        {
            // Check if the animation is already playing
            if (_animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName)) return;
            _animator.Play(stateName, layer, 0);
        }
        
        public void PlayAnimationInstantly(AnimationState state)
        {
            if (state.animationClip == null) return;
            PlayAnimationInstantly(state.stateName, state.animationLayerIndex);
        }
        
        public void PlayAnimation(string stateName, int layer = 0, float transitionTime = 0.15f)
        {
            // Check if the animation is already playing
            if (_animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName)) return;
            _animator.CrossFade(stateName, transitionTime, layer);
        }
        
        public void PlayAnimation(AnimationState state)
        {
            if (state.animationClip == null) return;
            PlayAnimation(state.stateName, state.animationLayerIndex, state.animationTransitionTime);
        }
        
        [ContextMenu("Test")]
        private void Test()
        {
            PlayAnimation(_locomotionCycle.walk);
        }
    }
}