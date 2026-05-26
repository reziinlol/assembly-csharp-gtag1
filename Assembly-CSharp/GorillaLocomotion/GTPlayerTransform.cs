using System;
using GorillaTag.Gravity;
using UnityEngine;

namespace GorillaLocomotion
{
	// Token: 0x020010E6 RID: 4326
	public class GTPlayerTransform : MonkeGravityController
	{
		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06006CD6 RID: 27862 RVA: 0x00237AAE File Offset: 0x00235CAE
		// (set) Token: 0x06006CD7 RID: 27863 RVA: 0x00237AB5 File Offset: 0x00235CB5
		public static Vector3 Up { get; private set; } = Vector3.up;

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x06006CD8 RID: 27864 RVA: 0x00237ABD File Offset: 0x00235CBD
		// (set) Token: 0x06006CD9 RID: 27865 RVA: 0x00237AC4 File Offset: 0x00235CC4
		public static Vector3 PhysicsUp { get; private set; } = Vector3.up;

		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06006CDA RID: 27866 RVA: 0x00237ACC File Offset: 0x00235CCC
		// (set) Token: 0x06006CDB RID: 27867 RVA: 0x00237AD3 File Offset: 0x00235CD3
		public static Vector3 Down { get; private set; } = Vector3.down;

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06006CDC RID: 27868 RVA: 0x00237ADB File Offset: 0x00235CDB
		// (set) Token: 0x06006CDD RID: 27869 RVA: 0x00237AE2 File Offset: 0x00235CE2
		public static Vector3 PhysicsDown { get; private set; } = Vector3.down;

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06006CDE RID: 27870 RVA: 0x00237AEA File Offset: 0x00235CEA
		// (set) Token: 0x06006CDF RID: 27871 RVA: 0x00237AF1 File Offset: 0x00235CF1
		public static Vector3 Forward { get; private set; } = Vector3.forward;

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x06006CE0 RID: 27872 RVA: 0x00237AF9 File Offset: 0x00235CF9
		// (set) Token: 0x06006CE1 RID: 27873 RVA: 0x00237B00 File Offset: 0x00235D00
		public static Vector3 Right { get; private set; } = Vector3.right;

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06006CE2 RID: 27874 RVA: 0x00237B08 File Offset: 0x00235D08
		public static Quaternion BodyRotation
		{
			get
			{
				return GTPlayerTransform.k_bodyTransform.rotation;
			}
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06006CE3 RID: 27875 RVA: 0x00023994 File Offset: 0x00021B94
		public static bool UseNetRotation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06006CE4 RID: 27876 RVA: 0x00237B14 File Offset: 0x00235D14
		// (set) Token: 0x06006CE5 RID: 27877 RVA: 0x00237B1B File Offset: 0x00235D1B
		public static bool IgnoreGravityRotation { get; set; } = false;

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06006CE6 RID: 27878 RVA: 0x00237B23 File Offset: 0x00235D23
		// (set) Token: 0x06006CE7 RID: 27879 RVA: 0x00237B2A File Offset: 0x00235D2A
		public static bool IgnoreGravityForce { get; set; } = false;

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06006CE8 RID: 27880 RVA: 0x00237B32 File Offset: 0x00235D32
		public static Vector3 RotationPosOffsetChange
		{
			get
			{
				return GTPlayerTransform.k_rotationPosOffsetChange;
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x06006CE9 RID: 27881 RVA: 0x00237B39 File Offset: 0x00235D39
		// (set) Token: 0x06006CEA RID: 27882 RVA: 0x00237B40 File Offset: 0x00235D40
		public static GTPlayerTransform Instance { get; private set; }

		// Token: 0x06006CEB RID: 27883 RVA: 0x00237B48 File Offset: 0x00235D48
		public static void RotateToUp(in Vector3 targetUp)
		{
			if (targetUp == GTPlayerTransform.Up)
			{
				GTPlayerTransform.Instance.ClearRotationRecovery();
				return;
			}
			Vector3 up = GTPlayerTransform.Up;
			GTPlayerTransform.RotateFromToDirection(up, targetUp);
		}

		// Token: 0x06006CEC RID: 27884 RVA: 0x00237B80 File Offset: 0x00235D80
		public static void RotateToForward(in Vector3 targetForward)
		{
			if (targetForward == GTPlayerTransform.Forward)
			{
				return;
			}
			Vector3 forward = GTPlayerTransform.Forward;
			GTPlayerTransform.RotateFromToDirection(forward, targetForward);
		}

		// Token: 0x06006CED RID: 27885 RVA: 0x00237BB0 File Offset: 0x00235DB0
		public static void RotateFromToDirection(in Vector3 currentDir, in Vector3 targetDir)
		{
			Quaternion rotation = GTPlayerTransform.k_transform.rotation;
			Quaternion quaternion = Quaternion.FromToRotation(currentDir, targetDir) * rotation;
			GTPlayerTransform.SetRotation(quaternion, rotation);
		}

		// Token: 0x06006CEE RID: 27886 RVA: 0x00237BEC File Offset: 0x00235DEC
		public static void RotateBy(in Quaternion rotation)
		{
			Quaternion rotation2 = GTPlayerTransform.k_transform.rotation;
			Quaternion quaternion = rotation2 * rotation;
			GTPlayerTransform.SetRotation(quaternion, rotation2);
		}

		// Token: 0x06006CEF RID: 27887 RVA: 0x00237C1C File Offset: 0x00235E1C
		public static void SetRotation(in Quaternion targetRotation)
		{
			Quaternion rotation = GTPlayerTransform.k_transform.rotation;
			GTPlayerTransform.SetRotation(targetRotation, rotation);
		}

		// Token: 0x06006CF0 RID: 27888 RVA: 0x00237C3C File Offset: 0x00235E3C
		private static void SetRotation(in Quaternion newRotation, in Quaternion currentRotation)
		{
			ref readonly GTPlayer.HandState leftHandRef = ref GTPlayerTransform.k_playerInstance.LeftHandRef;
			ref readonly GTPlayer.HandState rightHandRef = ref GTPlayerTransform.k_playerInstance.RightHandRef;
			Vector3 position = GTPlayerTransform.k_transform.position;
			Quaternion quaternion = newRotation * Quaternion.Inverse(currentRotation);
			Vector3 position2 = GTPlayerTransform.k_bodyTransform.position;
			Vector3 vector = GTPlayerTransform.GetRotatedDifference(position, position2, quaternion);
			if (leftHandRef.wasColliding || leftHandRef.wasSliding)
			{
				Vector3 rotatedDifference = GTPlayerTransform.GetRotatedDifference(position, leftHandRef.lastPosition, quaternion);
				Vector3 lhs = Vector3.Normalize(rotatedDifference);
				RaycastHit lastHitInfo = leftHandRef.lastHitInfo;
				if (Vector3.Dot(lhs, lastHitInfo.normal) <= 0f)
				{
					vector -= rotatedDifference;
				}
			}
			if (rightHandRef.wasColliding || rightHandRef.wasSliding)
			{
				Vector3 rotatedDifference2 = GTPlayerTransform.GetRotatedDifference(position, rightHandRef.lastPosition, quaternion);
				Vector3 lhs2 = Vector3.Normalize(rotatedDifference2);
				RaycastHit lastHitInfo = rightHandRef.lastHitInfo;
				if (Vector3.Dot(lhs2, lastHitInfo.normal) <= 0f)
				{
					vector -= rotatedDifference2;
				}
			}
			GTPlayerTransform.k_rotationPosOffsetChange -= vector;
			GTPlayerTransform.k_rigidBody.position = position - vector;
			GTPlayerTransform.k_rigidBody.rotation = newRotation;
			GTPlayerTransform.Up = newRotation * Vector3.up;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			GTPlayerTransform.Forward = newRotation * Vector3.forward;
			GTPlayerTransform.Right = newRotation * Vector3.right;
		}

		// Token: 0x06006CF1 RID: 27889 RVA: 0x00237DBC File Offset: 0x00235FBC
		private static Vector3 GetRotatedDifference(in Vector3 pivotPoint, in Vector3 worldPoint, in Quaternion rotation)
		{
			Vector3 vector = worldPoint - pivotPoint;
			return rotation * vector - vector;
		}

		// Token: 0x06006CF2 RID: 27890 RVA: 0x00237DED File Offset: 0x00235FED
		public static void ApplyRotationOverride(in Quaternion rotation, int frameTime)
		{
			GTPlayerTransform.SetRotation(rotation);
			GTPlayerTransform.k_rotationOverrideFrameTime = frameTime;
		}

		// Token: 0x06006CF3 RID: 27891 RVA: 0x00237DFB File Offset: 0x00235FFB
		public static void ResetRotationPositionOffset()
		{
			GTPlayerTransform.k_rotationPosOffsetChange = Vector3.zero;
		}

		// Token: 0x06006CF4 RID: 27892 RVA: 0x000028C5 File Offset: 0x00000AC5
		public static void EnableNetworkRotations()
		{
		}

		// Token: 0x06006CF5 RID: 27893 RVA: 0x000028C5 File Offset: 0x00000AC5
		public static void DisableNetworkRotations()
		{
		}

		// Token: 0x06006CF6 RID: 27894 RVA: 0x00237E08 File Offset: 0x00236008
		protected override void Awake()
		{
			base.Awake();
			if (!base.Register)
			{
				Debug.LogError("GTPlayerTransform: failed to load required references", base.gameObject);
			}
			GTPlayerTransform.Instance = this;
			GTPlayerTransform.k_transform = this.m_targetTransform;
			GTPlayerTransform.k_rigidBody = this.m_targetRigidBody;
			GTPlayerTransform.k_bodyTransform = this.m_gtPlayerBodyTransform;
			GTPlayerTransform.k_playerInstance = this.m_gtPlayerInstance;
			GTPlayerTransform.Up = GTPlayerTransform.k_transform.up;
			GTPlayerTransform.Forward = GTPlayerTransform.k_transform.forward;
			GTPlayerTransform.Right = GTPlayerTransform.k_transform.right;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			this.m_globalGravityIntent = false;
		}

		// Token: 0x06006CF7 RID: 27895 RVA: 0x00237EB0 File Offset: 0x002360B0
		public override void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			if (GTPlayerTransform.IgnoreGravityRotation || GTPlayerTransform.k_rotationOverrideFrameTime >= Time.frameCount - 1)
			{
				return;
			}
			if (base.InstantRotation)
			{
				GTPlayerTransform.RotateToUp(upDir);
				return;
			}
			float num = Vector3.Angle(GTPlayerTransform.Up, upDir);
			Vector3 vector;
			if (num * 0.017453292f <= speed)
			{
				vector = upDir;
			}
			else
			{
				Vector3 target = upDir;
				if (Mathf.Approximately(num, 180f))
				{
					switch (this.m_preferredRotationDirection)
					{
					case RotationDirection.Forward:
						target = GTPlayerTransform.k_bodyTransform.forward;
						break;
					case RotationDirection.Backward:
						target = GTPlayerTransform.k_bodyTransform.forward * -1f;
						break;
					case RotationDirection.Left:
						target = GTPlayerTransform.k_bodyTransform.right * -1f;
						break;
					case RotationDirection.Right:
						target = GTPlayerTransform.k_bodyTransform.right;
						break;
					}
				}
				vector = Vector3.RotateTowards(GTPlayerTransform.Up, target, speed, 0f);
			}
			GTPlayerTransform.RotateToUp(vector);
		}

		// Token: 0x06006CF8 RID: 27896 RVA: 0x00237FA0 File Offset: 0x002361A0
		public override void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			if (GTPlayerTransform.IgnoreGravityForce || GTPlayerTransform.k_playerInstance.isClimbing || GTPlayerTransform.k_playerInstance.GravityOverrideCount > 0)
			{
				return;
			}
			Vector3 vector = force * GTPlayerTransform.k_playerInstance.scale;
			base.ApplyGravityForce(vector, forceType);
		}

		// Token: 0x06006CF9 RID: 27897 RVA: 0x00237FED File Offset: 0x002361ED
		public override Vector3 GetWorldPoint()
		{
			return GTPlayerTransform.k_bodyTransform.position;
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06006CFA RID: 27898 RVA: 0x00237FF9 File Offset: 0x002361F9
		public override float Scale
		{
			get
			{
				return VRRig.LocalRig.scaleFactor;
			}
		}

		// Token: 0x06006CFB RID: 27899 RVA: 0x00238008 File Offset: 0x00236208
		public override void CallBack()
		{
			base.CallBack();
			GTPlayerTransform.PhysicsUp = base.GravityUp;
			GTPlayerTransform.PhysicsDown = base.GravityDown;
			if (base.GravityZonesCount > 0)
			{
				return;
			}
			if (GTPlayerTransform.Up != GTPlayerTransform.PhysicsUp)
			{
				Vector3 physicsUp = GTPlayerTransform.PhysicsUp;
				this.ApplyGravityUpRotation(physicsUp, MonkeGravityManager.DefaultGravityInfo.rotationSpeed * Time.fixedDeltaTime);
			}
		}

		// Token: 0x04007D97 RID: 32151
		private static Vector3 k_rotationPosOffsetChange = Vector3.zero;

		// Token: 0x04007D99 RID: 32153
		private static Transform k_transform;

		// Token: 0x04007D9A RID: 32154
		private static Rigidbody k_rigidBody;

		// Token: 0x04007D9B RID: 32155
		private static Transform k_bodyTransform;

		// Token: 0x04007D9C RID: 32156
		private static GTPlayer k_playerInstance;

		// Token: 0x04007D9D RID: 32157
		private static int k_rotationOverrideFrameTime;

		// Token: 0x04007D9E RID: 32158
		[SerializeField]
		private Transform m_gtPlayerBodyTransform;

		// Token: 0x04007D9F RID: 32159
		[SerializeField]
		private GTPlayer m_gtPlayerInstance;
	}
}
