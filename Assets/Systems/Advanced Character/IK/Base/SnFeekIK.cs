using System;
using Snowy.CustomAttributes;
using UnityEngine;

namespace Systems.IK.Base
{
	public class SnFeekIK : MonoBehaviour
	{
		/*public BaseIK baseIK;
		
		private void Start()
		{ 
			base.Invoke("StartFootPlacement", 0.1f);
			this.GetFootPlacementDependencies();
		}

		private void OnAnimatorIK(int layerIndex)
		{
			UpdateIK(0);
		}

		private void LateUpdate()
		{
			bool started = this.Started;
		}

		public void StartFootPlacement()
		{
			this.Started = true;
			this.LeftFootPlaceBase.position = this.LeftFoot.position;
			this.RightFootPlaceBase.position = this.RightFoot.position;
		}

		private void GetFootPlacementDependencies()
		{
			if (this.GroundLayers.value == 0)
			{
				this.GroundLayers = LayerMask.GetMask(new string[]
				{
					"Default"
				});
			}
			if (this.LeftFootPlaceBase == null && this.RightFootPlaceBase == null)
			{
				this.baseIK = GetComponent<BaseIK>();
				ChainIK leftFootChain = new ChainIK();
				leftFootChain.transform = LeftFoot;
				leftFootChain.Target = leftFootIkTarget;
				leftFootChain.Pole = leftFootIkPole;
				leftFootChain.ChainLength = 2;
				leftFootChain.Iterations = 2;
				leftFootChain.Delta = 0.01f;
				leftFootChain.SnapBackStrength = 0.1f;
				
				ChainIK rightFootChain = new ChainIK();
				rightFootChain.transform = RightFoot;
				rightFootChain.Target = rightFootIkTarget;
				rightFootChain.Pole = rightFootIkPole;
				rightFootChain.ChainLength = 2;
				rightFootChain.Iterations = 2;
				rightFootChain.Delta = 0.01f;
				leftFootChain.SnapBackStrength = 0.1f;

				leftFootChain.Init();
				rightFootChain.Init();
				this.baseIK.chains.Add(leftFootChain);
				this.baseIK.chains.Add(rightFootChain);
				
				this.SmothedLeftFootPosition = this.LeftFoot.position - base.transform.forward * 0.1f;
				this.SmothedRightFootPosition = this.RightFoot.position - base.transform.forward * 0.1f;
				this.SmothedLeftFootRotation = this.LeftFoot.rotation;
				this.SmothedRightFootRotation = this.RightFoot.rotation;
				this.LeftFootPlaceBase = new GameObject("Left Foot Position").transform;
				this.RightFootPlaceBase = new GameObject("Right Foot Position").transform;
				this.LeftFootPlaceBase.position = this.LeftFoot.position;
				this.RightFootPlaceBase.position = this.RightFoot.position;
				this.LeftFootPlaceBase.gameObject.hideFlags = HideFlags.HideAndDontSave;
				this.RightFootPlaceBase.gameObject.hideFlags = HideFlags.HideAndDontSave;
				this.LeftFootBase_UP = new GameObject("Left Foot BASE UP").transform;
				this.RightFootBase_UP = new GameObject("Right Foot BASE UP").transform;
				this.LeftFootBase_UP.position = this.LeftFoot.position;
				this.RightFootBase_UP.position = this.RightFoot.position;
				this.LeftFootBase_UP.transform.SetParent(this.LeftFoot);
				this.RightFootBase_UP.transform.SetParent(this.RightFoot);
				this.LeftFootBase_UP.gameObject.hideFlags = HideFlags.HideAndDontSave;
				this.RightFootBase_UP.gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		private void FootPlacementPositions()
		{
			if (this.UseDynamicFootPlacing)
			{
				this.LeftFootHeightFromGround = this.FootHeightMultiplier * this.AnimationLeftFootPositionY;
				this.RightFootHeightFromGround = this.FootHeightMultiplier * this.AnimationRightFootPositionY;
			}
			else
			{
				this.LeftFootHeightFromGround = Mathf.Lerp(this.LeftFootHeightFromGround, this.leftFootHeightCurve.Evaluate(Time.time) / 2f, 20f * Time.deltaTime);
				this.RightFootHeightFromGround = Mathf.Lerp(this.RightFootHeightFromGround, this.rightFootHeightCurve.Evaluate(Time.time) / 2f, 20f * Time.deltaTime);
			}
			Physics.SphereCast(this.LeftFoot.position + base.transform.up * this.RaycastHeight + this.LeftFootBase_UP.forward * 0.12f, this.radius, -base.transform.up, out this.LeftHitPlaceBase, this.RaycastMaxDistance, this.GroundLayers);
			Physics.SphereCast(this.RightFoot.position + base.transform.up * this.RaycastHeight + this.RightFootBase_UP.forward * 0.12f, this.radius, -base.transform.up, out this.RightHitPlaceBase, this.RaycastMaxDistance, this.GroundLayers);
			if (this.LeftHitPlaceBase.point != Vector3.zero)
			{
				this.LeftFootPlaceBase.position = this.LeftHitPlaceBase.point;
				this.LeftFootPlaceBase.rotation = Quaternion.FromToRotation(base.transform.up, this.LeftHitPlaceBase.normal) * base.transform.rotation;
				this.LeftHit = true;
			}
			else
			{
				this.LeftFootPlaceBase.position = this.LeftFoot.position;
				this.LeftHit = false;
			}
			if (this.RightHitPlaceBase.point != Vector3.zero)
			{
				this.RightFootPlaceBase.position = this.RightHitPlaceBase.point;
				this.RightFootPlaceBase.rotation = Quaternion.FromToRotation(base.transform.up, this.RightHitPlaceBase.normal) * base.transform.rotation;
				this.RightHit = true;
			}
			else
			{
				this.RightFootPlaceBase.position = this.RightFoot.position;
				this.RightHit = false;
			}
			this.LeftFootHeight = this.FootHeight - Vector3.SignedAngle(this.LeftFootBase_UP.up, base.transform.up, base.transform.right) / 500f;
			this.RightFootHeight = this.FootHeight - Vector3.SignedAngle(this.RightFootBase_UP.up, base.transform.up, base.transform.right) / 500f;
			this.LeftFootHeight = Mathf.Clamp(this.LeftFootHeight, -0.2f, 0.2f);
			this.RightFootHeight = Mathf.Clamp(this.RightFootHeight, -0.2f, 0.2f);
			if (this.LeftHit)
			{
				if (this.LeftHitPlaceBase.point.y < base.transform.position.y + this.MaxStepHeight)
				{
					this.SmothedLeftFootPosition = Vector3.Lerp(this.SmothedLeftFootPosition, this.LeftFootPlaceBase.position + this.LeftHitPlaceBase.normal * this.LeftFootHeight + base.transform.up * this.LeftFootHeightFromGround, 15f * Time.deltaTime);
				}
				else
				{
					this.SmothedLeftFootPosition = Vector3.Lerp(this.SmothedLeftFootPosition, base.transform.position + base.transform.up * this.FootHeight + base.transform.up * this.LeftFootHeightFromGround, 15f * Time.deltaTime);
				}
			}
			else
			{
				this.SmothedLeftFootPosition = this.LeftFoot.position;
			}
			if (this.RightHit)
			{
				if (this.RightHitPlaceBase.point.y < base.transform.position.y + this.MaxStepHeight)
				{
					this.SmothedRightFootPosition = Vector3.Lerp(this.SmothedRightFootPosition, this.RightFootPlaceBase.position + this.RightHitPlaceBase.normal * this.RightFootHeight + base.transform.up * this.RightFootHeightFromGround, 20f * Time.deltaTime);
				}
				else
				{
					this.SmothedRightFootPosition = Vector3.Lerp(this.SmothedRightFootPosition, base.transform.position + base.transform.up * this.FootHeight + base.transform.up * this.RightFootHeightFromGround, 20f * Time.deltaTime);
				}
			}
			else
			{
				this.SmothedRightFootPosition = this.RightFoot.position;
			}
			Vector3 axis = Vector3.Cross(Vector3.up, this.LeftHitPlaceBase.normal);
			Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, this.LeftHitPlaceBase.normal) * this.GlobalWeight, axis);
			this.LeftFootPlaceBase.rotation = rotation;
			this.SmothedLeftFootRotation = Quaternion.Lerp(this.SmothedLeftFootRotation, this.LeftFootPlaceBase.rotation, 20f * Time.deltaTime);
			Vector3 axis2 = Vector3.Cross(Vector3.up, this.RightHitPlaceBase.normal);
			Quaternion rotation2 = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, this.RightHitPlaceBase.normal) * this.GlobalWeight, axis2);
			this.RightFootPlaceBase.rotation = rotation2;
			this.SmothedRightFootRotation = Quaternion.Lerp(this.SmothedRightFootRotation, this.RightFootPlaceBase.rotation, 20f * Time.deltaTime);
			if (this.LeftFootHeightFromGround < 0.3f)
			{
				this.LeftFootRotationWeight = Mathf.Lerp(this.LeftFootRotationWeight, 1f, 8f * Time.deltaTime);
			}
			else
			{
				this.LeftFootRotationWeight = Mathf.Lerp(this.LeftFootRotationWeight, 0f, 1f * Time.deltaTime);
			}
			if (this.RightFootHeightFromGround < 0.3f)
			{
				this.RightFootRotationWeight = Mathf.Lerp(this.RightFootRotationWeight, 1f, 8f * Time.deltaTime);
			}
			else
			{
				this.RightFootRotationWeight = Mathf.Lerp(this.RightFootRotationWeight, 0f, 1f * Time.deltaTime);
			}
			if (this.SmoothIKTransition)
			{
				this.TransitionIKtoFKWeight = Mathf.Lerp(this.TransitionIKtoFKWeight, 1f, 5f * Time.deltaTime);
				return;
			}
			this.TransitionIKtoFKWeight = Mathf.Lerp(this.TransitionIKtoFKWeight, 0f, 5f * Time.deltaTime);
		}

		private void BodyPlacement()
		{
			Physics.SphereCast(base.transform.position + base.transform.up * this.RaycastDistanceToGround, this.GroundCheckRadius, -base.transform.up, out this.HitGroundBodyPlacement, this.RaycastDistanceToGround + 0.2f, this.GroundLayers);
			if (this.HitGroundBodyPlacement.point != Vector3.zero)
			{
				this.TheresGroundBelow = true;
			}
			else
			{
				this.TheresGroundBelow = false;
			}
			this.GroundAngle = Vector3.Angle(Vector3.up, this.HitGroundBodyPlacement.normal);
			if (this.KeepCharacterOnGround)
			{
				this.BodyHeightPosition = Mathf.Clamp(this.BodyHeightPosition, this.MinBodyHeightPosition, this.MaxBodyPositionHeight);
				if (this.TheresGroundBelow)
				{
					float b = this.HitGroundBodyPlacement.point.y - this.BodyHeightPosition;
					float y = Mathf.Lerp(base.transform.position.y, b, this.Force * Time.fixedDeltaTime);
					Vector3 position = new Vector3(base.transform.position.x, y, base.transform.position.z);
					base.transform.position = position;
				}
			}
			if (this.TheresGroundBelow && !base.IsInvoking("DisableBlock") && this.BlockBodyPositioning)
			{
				base.Invoke("DisableBlock", 0.5f);
			}
			if (this.EnableDynamicBodyPlacing && !this.BlockBodyPositioning)
			{
				if (this.LeftHitPlaceBase.point == Vector3.zero || this.RightHitPlaceBase.point == Vector3.zero || this.LastBodyPositionY == 0f)
				{
					this.LastBodyPositionY = this.Animation_Y_BodyPosition;
					this.BodyPositionOffset = 0f;
					this.NewAnimationBodyPosition = this.hips.position;
					return;
				}
				float num = this.LeftHitPlaceBase.point.y - base.transform.position.y - this.RightFootHeightFromGround / 2f;
				float num2 = this.RightHitPlaceBase.point.y - base.transform.position.y - this.LeftFootHeightFromGround / 2f;
				this.BodyPositionOffset = ((num < num2) ? num : num2);
				this.BodyPositionOffset = Mathf.Clamp(this.BodyPositionOffset, -this.MaxBodyCrouchHeight, 0f);
				float num3 = this.UpAndDownForce + this.GroundAngle / 20f;
				this.NewAnimationBodyPosition = this.hips.position + base.transform.up * this.BodyPositionOffset;
				this.NewAnimationBodyPosition.y = Mathf.Lerp(this.LastBodyPositionY, this.NewAnimationBodyPosition.y, num3 * Time.deltaTime);
				float num4 = Mathf.Abs(this.Animation_Y_BodyPosition - this.LastBodyPositionY);
				if (!this.JustCalculateBodyPosition && num4 < 1f)
				{
					this.hips.position = this.NewAnimationBodyPosition;
				}
				this.LastBodyPositionY = this.hips.position.y;
				return;
			}
			else
			{
				if (!this.TheresGroundBelow || this.BlockBodyPositioning)
				{
					return;
				}
				this.NewAnimationBodyPosition = this.hips.position + base.transform.up * this.BodyPositionOffset;
				this.NewAnimationBodyPosition.y = Mathf.Lerp(this.LastBodyPositionY, this.Animation_Y_BodyPosition, this.UpAndDownForce * Time.deltaTime);
				this.hips.position = this.NewAnimationBodyPosition;
				this.LastBodyPositionY = this.hips.position.y;
				return;
			}
		}

		private void DisableBlock()
		{
			this.BlockBodyPositioning = false;
			this.LastBodyPositionY = this.Animation_Y_BodyPosition;
		}

		public Vector3 GetCalculatedAnimatorCenterOfMass()
		{
			return this.NewAnimationBodyPosition;
		}

		private void UpdateIK(int layerIndex)
		{
			if (Vector3.Angle(base.transform.up, Vector3.up) > 30f && this.EnableFootPlacement)
			{
				this.SmoothIKTransition = false;
			}
			if (layerIndex == 0)
			{
				this.FootPlacementPositions();
				this.Animation_Y_BodyPosition = this.hips.position.y;
				if (this.TransitionIKtoFKWeight < 0.1f || this.GlobalWeight < 0.01f || this.RightHitPlaceBase.point == Vector3.zero || this.RightHitPlaceBase.point == Vector3.zero)
				{
					return;
				}
				if (this.EnableFootPlacement)
				{
					this.AnimationLeftFootPositionY = base.transform.position.y - (this.LeftFoot.position.y - this.FootHeight);
					this.AnimationRightFootPositionY = base.transform.position.y - (this.RightFoot.position.y - this.FootHeight);
					this.AnimationLeftFootPositionY = Mathf.Abs(this.AnimationLeftFootPositionY);
					this.AnimationRightFootPositionY = Mathf.Abs(this.AnimationRightFootPositionY);
					this.AnimationLeftFootPositionY = Mathf.Clamp(this.AnimationLeftFootPositionY, 0f, 1f);
					this.AnimationRightFootPositionY = Mathf.Clamp(this.AnimationRightFootPositionY, 0f, 1f);
					if (Vector3.Angle(base.transform.up, Vector3.up) < 40f)
					{
						this.BodyPlacement();
					}
					if (this.LeftHit && this.LeftHitPlaceBase.point.y < base.transform.position.y + this.RaycastHeight)
					{
						Vector3 goalPosition = new Vector3(this.LeftFoot.position.x, this.SmothedLeftFootPosition.y, this.LeftFoot.position.z);
						this.leftFootIkTarget.position = goalPosition;
						// adjust the rotation to look forward
						this.leftFootIkTarget.rotation = this.SmothedLeftFootRotation * this.LeftFootBase_UP.rotation;
					
					}
					if (this.RightHit && this.RightHitPlaceBase.point.y < base.transform.position.y + this.RaycastHeight)
					{
						Vector3 goalPosition2 = new Vector3(this.RightFoot.position.x, this.SmothedRightFootPosition.y, this.RightFoot.position.z);
						this.rightFootIkTarget.position = goalPosition2;
						// adjust the rotation to look forward
						this.rightFootIkTarget.rotation = this.SmothedRightFootRotation * this.RightFootBase_UP.rotation;
					}
				}
			}
		}

		private void OnDrawGizmos()
		{
			// Draw a vector at the transform's position pointing forward with a length of 2
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(this.LeftFoot.position, this.LeftFoot.forward * 0.1f);
			if (!this.LeftFootBase_UP || !this.RightFootBase_UP) return;
			Gizmos.color = Color.green;
			Gizmos.DrawRay(this.LeftFootPlaceBase.position, this.LeftFootPlaceBase.forward * 0.1f);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(this.LeftFootBase_UP.position, this.LeftFootBase_UP.forward * 0.1f);
		}

		private bool Started;

		[HideInInspector]
		public bool BlockBodyPositioning;
		

		private RaycastHit LeftHitPlaceBase;

		private RaycastHit RightHitPlaceBase;

		private Transform RightFootPlaceBase;

		private Transform LeftFootPlaceBase;

		private Vector3 SmothedLeftFootPosition;

		private Vector3 SmothedRightFootPosition;

		private Quaternion SmothedLeftFootRotation;

		private Quaternion SmothedRightFootRotation;

		[Header("FOOT PLACEMENT")]
		public bool EnableFootPlacement = true;

		public bool AdvancedMode;

		[Header("Raycasts Settings")]
		[Space]
		public LayerMask GroundLayers;

		[SerializeField] private Transform hips;
		
		[SerializeField] private Transform LeftFoot;

		private Transform LeftFootBase_UP;

		[SerializeField] private Transform RightFoot;

		private Transform RightFootBase_UP;

		[SerializeField] Transform leftFootIkTarget;
		[SerializeField] Transform rightFootIkTarget;
		[SerializeField] Transform leftFootIkPole;
		[SerializeField] Transform rightFootIkPole;
		
		[SnReadOnly("AdvancedMode", false, true)]
		public float RaycastMaxDistance = 2f;

		[SnReadOnly("AdvancedMode", false, true)]
		public float RaycastHeight = 1f;

		[Range(0f, 1f)]
		[Header("Foot Placing System")]
		[Space]
		public float FootHeight = 0.1f;

		private float LeftFootHeight;

		private float RightFootHeight;

		[SnReadOnly("AdvancedMode", false, true)]
		public float MaxStepHeight = 0.6f;

		public bool UseDynamicFootPlacing = true;

		private float AnimationLeftFootPositionY;

		private float AnimationRightFootPositionY;

		[SnReadOnly("AdvancedMode", false, true)]
		public bool SmoothIKTransition = true;

		[SnReadOnly("AdvancedMode", false, true)]
		public float FootHeightMultiplier = 0.6f;

		[Range(0f, 1f)]
		public float GlobalWeight = 1f;

		private float TransitionIKtoFKWeight;

		[HideInInspector]
		public float LeftFootHeightFromGround;

		[HideInInspector]
		public float RightFootHeightFromGround;

		[HideInInspector]
		public float LeftFootRotationWeight;

		[HideInInspector]
		public float RightFootRotationWeight;

		private bool LeftHit;

		private bool RightHit;

		[SnReadOnly("AdvancedMode", false, true)]
		public float radius = 0.1f;

		[Header("DYNAMIC BODY PLACEMENT")]
		[Space]
		public AnimationCurve rightFootHeightCurve;
		public AnimationCurve leftFootHeightCurve;
		[Tooltip("When enabled, it will change your character's position according to the terrain.")]
		public bool EnableDynamicBodyPlacing = true;

		[SnReadOnly("EnableDynamicBodyPlacing", false, true)]
		public float UpAndDownForce = 10f;

		[SnReadOnly("AdvancedMode", false, true)]
		public float MaxBodyCrouchHeight = 0.65f;

		[Tooltip("If true, it will only calculate the ideal body position, but it will not affect the body position of the character, useful if you want to make a custom Body Placement.  Use ' GetCalculatedAnimatorCenterOfMass(); ' to have the calculated position of the body. ")]
		[SnReadOnly("AdvancedMode", false, true)]
		public bool JustCalculateBodyPosition;

		[Space]
		[Tooltip("This will keep your character grounded.")]
		public bool KeepCharacterOnGround;

		[SnReadOnly("KeepCharacterOnGround", false, true)]
		public float RaycastDistanceToGround = 1.2f;

		[SnReadOnly("KeepCharacterOnGround", false, true)]
		public float BodyHeightPosition = 0.01f;

		[SnReadOnly("KeepCharacterOnGround", false, true)]
		public float Force = 10f;

		private float MinBodyHeightPosition = 0.005f;

		private float MaxBodyPositionHeight = 1f;

		[Header("Ground Check")]
		[SnReadOnly("", false, true)]
		[Space]
		public bool TheresGroundBelow;

		[SnReadOnly("AdvancedMode", false, true)]
		public float GroundCheckRadius = 0.1f;

		private RaycastHit HitGroundBodyPlacement;

		[HideInInspector]
		public float LastBodyPositionY;
		[HideInInspector]
		public Vector3 NewAnimationBodyPosition;

		private float BodyPositionOffset;

		[HideInInspector]
		public float Animation_Y_BodyPosition;

		private float GroundAngle;*/
	}
}