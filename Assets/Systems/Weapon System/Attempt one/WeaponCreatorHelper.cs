using System;
using System.Collections.Generic;
using Snowy.CustomAttributes;
using Systems.IK.Base;
using Systems.Weapon_System.Creator;
using UnityEngine;

namespace Systems.Weapon_System.Creator.Attempt_01
{

    [Serializable]
    public class FingerSettings
    {
        [MinMax(-0.05f, 0.4f)] public float power;
        [MinMax(-0.1f, 0.1f)] public float openAmount;
        [MinMax(-0.1f, 0.1f)] public float distance;
        public Vector3 tipRotation;
    }

    [Serializable]
    public class HandTargetGroup
    {
        public Transform mainTarget;

        // Finger targets
        public Transform thumbTarget;
        public Transform indexTarget;
        public Transform middleTarget;
        public Transform ringTarget;
        public Transform pinkyTarget;

        public Transform thumbParent;
        public Transform indexParent;
        public Transform middleParent;
        public Transform ringParent;
        public Transform pinkyParent;

        Vector3 thumbDefaultPosition;
        Vector3 indexDefaultPosition;
        Vector3 middleDefaultPosition;
        Vector3 ringDefaultPosition;
        Vector3 pinkyDefaultPosition;

        Quaternion thumbDefaultRotation;
        Quaternion indexDefaultRotation;
        Quaternion middleDefaultRotation;
        Quaternion ringDefaultRotation;
        Quaternion pinkyDefaultRotation;

        // Finger open amounts
        [SerializeField] FingerSettings thumbOpen;
        [SerializeField] FingerSettings indexOpen;
        [SerializeField] FingerSettings middleOpen;
        [SerializeField] FingerSettings ringOpen;
        [SerializeField] FingerSettings pinkyOpen;

        public void Init()
        {
            ResetFingers();

            thumbDefaultPosition = thumbTarget.localPosition;
            indexDefaultPosition = indexTarget.localPosition;
            middleDefaultPosition = middleTarget.localPosition;
            ringDefaultPosition = ringTarget.localPosition;
            pinkyDefaultPosition = pinkyTarget.localPosition;

            thumbDefaultRotation = thumbTarget.localRotation;
            indexDefaultRotation = indexTarget.localRotation;
            middleDefaultRotation = middleTarget.localRotation;
            ringDefaultRotation = ringTarget.localRotation;
            pinkyDefaultRotation = pinkyTarget.localRotation;
        }

        public void SetFingersOpen(FingerSettings thumb, FingerSettings index, FingerSettings middle,
            FingerSettings ring, FingerSettings pinky)
        {
            thumbOpen = thumb;
            indexOpen = index;
            middleOpen = middle;
            ringOpen = ring;
            pinkyOpen = pinky;

            UpdateThumbOpen(thumbOpen, thumbTarget, thumbDefaultPosition);
            UpdateFingerOpen(indexOpen, indexTarget, indexDefaultPosition);
            UpdateFingerOpen(middleOpen, middleTarget, middleDefaultPosition);
            UpdateFingerOpen(ringOpen, ringTarget, ringDefaultPosition);
            UpdateFingerOpen(pinkyOpen, pinkyTarget, pinkyDefaultPosition);
        }

        public void StraightenFinger(Transform fParent, Transform target)
        {
            // Set the finger target to the front of the hand at the max length
            // Index
            // Calculate the max length of the finger
            float length = 0f;
            Transform current = fParent.GetChild(0);
            while (current != fParent.parent)
            {
                length += Vector3.Distance(current.position, current.parent.position);
                current = current.parent;
            }

            // Set the target to be at the max length from the parent position
            fParent.parent.localRotation = Quaternion.identity;
            target.position = fParent.parent.position + fParent.parent.forward * length;
            target.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // Set the tip rotation to be in the direction of the target
            target.rotation = Quaternion.LookRotation(target.position - fParent.position);
        }

        public void ResetFingers()
        {
            StraightenFinger(thumbParent, thumbTarget);
            StraightenFinger(indexParent, indexTarget);
            StraightenFinger(middleParent, middleTarget);
            StraightenFinger(ringParent, ringTarget);
            StraightenFinger(pinkyParent, pinkyTarget);

        }

        public void UpdateThumbOpen(FingerSettings settings, Transform target, Vector3 defaultPos)
        {
            // Index.
            // use the amount to lerp between the default position and the open position using sin and cos with the amount
            try
            {
                float yShift = Mathf.Sin(settings.openAmount) * settings.power;
                float zShift = Mathf.Sin(settings.openAmount < 0 ? 0 : -settings.openAmount) * settings.power;
                target.localPosition = new Vector3(defaultPos.x + yShift, defaultPos.y + settings.distance,
                    defaultPos.z + zShift);
                target.localRotation = Quaternion.Euler(settings.tipRotation);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void UpdateFingerOpen(FingerSettings settings, Transform target, Vector3 defaultPos)
        {
            // Index.
            // use the amount to lerp between the default position and the open position using sin and cos with the amount
            try
            {
                float yShift = Mathf.Sin(settings.openAmount) * 0.2f;
                float zShift = Mathf.Sin(settings.openAmount > 0 ? 0 : settings.openAmount) * settings.power;
                target.localPosition = new Vector3(defaultPos.x + settings.distance, defaultPos.y + yShift,
                    defaultPos.z + zShift);
                target.localRotation = Quaternion.Euler(settings.tipRotation);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void UpdateHand()
        {
            return;
            try
            {
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    public class HandBones
    {
        public HumanBodyBones main;
        public HumanBodyBones thumb;
        public HumanBodyBones index;
        public HumanBodyBones middle;
        public HumanBodyBones ring;
        public HumanBodyBones pinky;
    }

    [ExecuteInEditMode]
    public class WeaponCreatorHelper : MonoBehaviour
    {
        [SerializeField] private Transform camPivot;
        [SerializeField] Indicator rightIndicator;
        [SerializeField] Indicator leftIndicator;
        [SerializeField] Indicator shootPointIndicator;
        [SerializeField] BaseIK ik;
        [SerializeField] private Animator anim;

        public GameObject currentWeapon;
        public GameObject currentWeaponPrefab;

        private GameObject shootPointTarget;

        [SerializeField] HandTargetGroup leftHandTargetGroup;
        [SerializeField] HandTargetGroup rightHandTargetGroup;

        private void Awake()
        {
            // Clear all children
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        public void CreateWeaponSetup(GameObject prefab)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }

            currentWeaponPrefab = prefab;

            currentWeapon = Instantiate(prefab, transform);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;

            // Create 3 childs for the weapon (left, right, and shootPoint)
            shootPointTarget = new GameObject("ShootPoint");
            shootPointTarget.transform.SetParent(currentWeapon.transform);

            ik.chains = new List<ChainIK>();
            rightHandTargetGroup = new HandTargetGroup();
            leftHandTargetGroup = new HandTargetGroup();
            SetUpHand(new HandBones
            {
                main = HumanBodyBones.RightHand,
                thumb = HumanBodyBones.RightThumbDistal,
                index = HumanBodyBones.RightIndexDistal,
                middle = HumanBodyBones.RightMiddleDistal,
                ring = HumanBodyBones.RightRingDistal,
                pinky = HumanBodyBones.RightLittleDistal
            }, rightIndicator, rightHandTargetGroup);

            SetUpHand(new HandBones
            {
                main = HumanBodyBones.LeftHand,
                thumb = HumanBodyBones.LeftThumbDistal,
                index = HumanBodyBones.LeftIndexDistal,
                middle = HumanBodyBones.LeftMiddleDistal,
                ring = HumanBodyBones.LeftRingDistal,
                pinky = HumanBodyBones.LeftLittleDistal
            }, leftIndicator, leftHandTargetGroup);
        }

        public void UpdateLeftHandSettings(HandSettings settings)
        {
            // Update the left hand settings
            if (!rightHandTargetGroup?.mainTarget) return;


            leftHandTargetGroup.mainTarget.transform.localPosition = settings.GetPosition();
            leftHandTargetGroup.mainTarget.transform.localRotation = settings.GetRotation();
            leftHandTargetGroup.SetFingersOpen(settings.thumb, settings.index, settings.middle, settings.ring,
                settings.pinky);
            leftHandTargetGroup.UpdateHand();
        }

        public HandSettings GetLeftHandSettings()
        {
            Debug.Log("Getting left hand settings");
            if (leftHandTargetGroup == null)
            {
                Debug.Log("Left hand target is null");
                return new HandSettings();
            }

            return new HandSettings
            {
                positionX = leftHandTargetGroup.mainTarget.transform.localPosition.x,
                positionY = leftHandTargetGroup.mainTarget.transform.localPosition.y,
                positionZ = leftHandTargetGroup.mainTarget.transform.localPosition.z,
                rotationX = leftHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.x,
                rotationY = leftHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.y,
                rotationZ = leftHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.z
            };
        }

        public HandSettings GetRightHandSettings()
        {
            if (rightHandTargetGroup == null) return new HandSettings();
            return new HandSettings
            {
                positionX = rightHandTargetGroup.mainTarget.transform.localPosition.x,
                positionY = rightHandTargetGroup.mainTarget.transform.localPosition.y,
                positionZ = rightHandTargetGroup.mainTarget.transform.localPosition.z,
                rotationX = rightHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.x,
                rotationY = rightHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.y,
                rotationZ = rightHandTargetGroup.mainTarget.transform.localRotation.eulerAngles.z
            };
        }

        public void UpdateRightHandSettings(HandSettings settings)
        {
            // Update the right hand settings
            if (!rightHandTargetGroup?.mainTarget) return;

            rightHandTargetGroup.mainTarget.transform.localPosition = settings.GetPosition();
            rightHandTargetGroup.mainTarget.transform.localRotation = settings.GetRotation();
            rightHandTargetGroup.SetFingersOpen(settings.thumb, settings.index, settings.middle, settings.ring,
                settings.pinky);
            rightHandTargetGroup.UpdateHand();
        }

        public void UpdateCamera(CameraSettings camSettings)
        {
            foreach (var cam in Camera.allCameras)
            {
                cam.fieldOfView = camSettings.fov;
            }

            camPivot.localRotation = camSettings.GetRotation();
        }

        public CameraSettings GetCameraSettings()
        {
            return new CameraSettings
            {
                fov = Camera.main.fieldOfView,
                rotationX = camPivot.localRotation.eulerAngles.x,
                rotationY = camPivot.localRotation.eulerAngles.y,
                rotationZ = camPivot.localRotation.eulerAngles.z
            };
        }


        public void SetUpHand(HandBones bone, Indicator indicator, HandTargetGroup group)
        {
            if (!anim) anim = ik.GetComponent<Animator>();
            if (!anim)
            {
                Debug.LogError("No animator found");
                return;
            }

            // Create the main target
            Transform mainTarget = new GameObject(bone.main.ToString()).transform;
            mainTarget.SetParent(currentWeapon.transform);
            mainTarget.localPosition = Vector3.zero;
            mainTarget.localRotation = Quaternion.identity;
            group.mainTarget = mainTarget;
            SetUpIKTarget(mainTarget, bone.main);
            Transform mainBone = anim.GetBoneTransform(bone.main);
            Debug.Log(mainBone.gameObject.name);

            // Create the finger targets
            Transform thumbTarget = new GameObject("Thumb").transform;
            Transform indexTarget = new GameObject("Index").transform;
            Transform middleTarget = new GameObject("Middle").transform;
            Transform ringTarget = new GameObject("Ring").transform;
            Transform pinkyTarget = new GameObject("Pinky").transform;

            Transform thumbBone = anim.GetBoneTransform(bone.thumb);
            Transform indexBone = anim.GetBoneTransform(bone.index);
            Transform middleBone = anim.GetBoneTransform(bone.middle);
            Transform ringBone = anim.GetBoneTransform(bone.ring);
            Transform pinkyBone = anim.GetBoneTransform(bone.pinky);

            thumbTarget.SetParent(mainBone);
            indexTarget.SetParent(mainBone);
            middleTarget.SetParent(mainBone);
            ringTarget.SetParent(mainBone);
            pinkyTarget.SetParent(mainBone);

            Debug.Log(anim.GetBoneTransform(bone.thumb).gameObject.name + " - " +
                      anim.GetBoneTransform(bone.thumb).localPosition.ToString());
            Debug.Log(thumbTarget.name);
            Debug.Log(anim.GetBoneTransform(bone.thumb));
            thumbTarget.position = thumbBone.position;
            indexTarget.position = indexBone.position;
            middleTarget.position = middleBone.position;
            ringTarget.position = ringBone.position;
            pinkyTarget.position = pinkyBone.position;

            thumbTarget.rotation = thumbBone.rotation;
            indexTarget.rotation = indexBone.rotation;
            middleTarget.rotation = middleBone.rotation;
            ringTarget.rotation = ringBone.rotation;
            pinkyTarget.rotation = pinkyBone.rotation;

            // Set the group
            group.thumbParent = thumbBone.parent;
            group.indexParent = indexBone.parent;
            group.middleParent = middleBone.parent;
            group.ringParent = ringBone.parent;
            group.pinkyParent = pinkyBone.parent;
            group.thumbTarget = thumbTarget;
            group.indexTarget = indexTarget;
            group.middleTarget = middleTarget;
            group.ringTarget = ringTarget;
            group.pinkyTarget = pinkyTarget;
            group.Init();

            // Set up ik
            SetUpIKTarget(thumbTarget, bone.thumb, true);
            SetUpIKTarget(indexTarget, bone.index);
            SetUpIKTarget(middleTarget, bone.middle);
            SetUpIKTarget(ringTarget, bone.ring);
            SetUpIKTarget(pinkyTarget, bone.pinky);

            // Set up indicators
            indicator.target = mainTarget;
        }

        public void SetUpIKTarget(Transform target, HumanBodyBones bone, bool isThumb = false)
        {
            ChainIK chain = new ChainIK();
            chain.transform = anim.GetBoneTransform(bone);

            // Create the pole
            Transform pole = new GameObject("Pole " + bone.ToString()).transform;
            // Remove all duplicates under the target.parent
            foreach (Transform child in target.parent)
            {
                if (child.name.Contains("Pole " + bone.ToString()))
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            pole.SetParent(target.parent);
            // Set the pole to be in the middle of the chain
            if (!isThumb)
            {
                pole.position = (chain.transform.position + target.position) / 2f;
                pole.position += pole.up * 0.05f;
                pole.rotation = chain.transform.rotation;
            }
            else
            {
                pole.position = (chain.transform.position + target.position) / 2f;
                pole.position -= pole.right * 0.05f;
                pole.rotation = chain.transform.rotation;
            }

            //pole.rotation = chain.transform.rotation;
            //pole.localScale = Vector3.one;
            //pole.position += pole.forward;
            chain.Pole = pole;

            chain.Target = target;
            chain.ChainLength = 2;
            chain.Iterations = 10;
            chain.Delta = 0.001f;
            chain.SnapBackStrength = 1f;
            chain.Init();

            ik.chains.Add(chain);
        }

        public void ResetFingers()
        {
            if (leftHandTargetGroup == null || rightHandTargetGroup == null) return;
            leftHandTargetGroup.ResetFingers();
            rightHandTargetGroup.ResetFingers();
        }

    }
}