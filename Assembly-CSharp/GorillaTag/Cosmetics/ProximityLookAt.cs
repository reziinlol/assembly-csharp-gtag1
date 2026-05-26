using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001238 RID: 4664
	public class ProximityLookAt : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060074B3 RID: 29875 RVA: 0x00263928 File Offset: 0x00261B28
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.transferableParent != null)
			{
				this.ownerRig = this.transferableParent.ownerRig;
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = base.GetComponentInParent<VRRig>();
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.CacheSettings();
		}

		// Token: 0x060074B4 RID: 29876 RVA: 0x00263999 File Offset: 0x00261B99
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lookTarget = null;
			this.lastTargetSwitchTime = float.NegativeInfinity;
		}

		// Token: 0x060074B5 RID: 29877 RVA: 0x002639B5 File Offset: 0x00261BB5
		private void OnValidate()
		{
			this.CacheSettings();
		}

		// Token: 0x060074B6 RID: 29878 RVA: 0x002639BD File Offset: 0x00261BBD
		private void CacheSettings()
		{
			this.normalizedLocalForward = ProximityLookAt.LocalAxisToVector(this.localForward);
			this.cosAngle = Mathf.Cos(this.targetSearchAngleDegrees * 0.017453292f);
			this.sqrRadius = this.lookRadius * this.lookRadius;
		}

		// Token: 0x060074B7 RID: 29879 RVA: 0x002639FC File Offset: 0x00261BFC
		private static Vector3 LocalAxisToVector(ProximityLookAt.LocalAxis axis)
		{
			switch (axis)
			{
			case ProximityLookAt.LocalAxis.Forward:
				return Vector3.forward;
			case ProximityLookAt.LocalAxis.Back:
				return Vector3.back;
			case ProximityLookAt.LocalAxis.Right:
				return Vector3.right;
			case ProximityLookAt.LocalAxis.Left:
				return Vector3.left;
			case ProximityLookAt.LocalAxis.Up:
				return Vector3.up;
			case ProximityLookAt.LocalAxis.Down:
				return Vector3.down;
			default:
				return Vector3.forward;
			}
		}

		// Token: 0x060074B8 RID: 29880 RVA: 0x00263A54 File Offset: 0x00261C54
		public void SliceUpdate()
		{
			Transform x = this.FindTarget();
			if (x == this.lookTarget)
			{
				return;
			}
			if (Time.time - this.lastTargetSwitchTime < this.targetSwitchCooldown)
			{
				return;
			}
			this.lookTarget = x;
			this.lastTargetSwitchTime = Time.time;
		}

		// Token: 0x060074B9 RID: 29881 RVA: 0x00263AA0 File Offset: 0x00261CA0
		private void LateUpdate()
		{
			if (this.lookTransforms == null)
			{
				return;
			}
			Vector3 vector = base.transform.TransformDirection(this.normalizedLocalForward);
			for (int i = 0; i < this.lookTransforms.Length; i++)
			{
				Transform transform = this.lookTransforms[i];
				if (!(transform == null))
				{
					Vector3 vector2 = (this.lookTarget != null) ? (this.lookTarget.position - transform.position).normalized : vector;
					vector2 = Vector3.RotateTowards(vector, vector2, this.lookAtAngleDegreeMax * 0.017453292f, 0f);
					if (this.pivotConstraint != null)
					{
						Vector3 vector3 = this.pivotConstraint.InverseTransformDirection(vector2);
						vector3.y = Mathf.Clamp(vector3.y, this.minPivotY, this.maxPivotY);
						vector2 = this.pivotConstraint.TransformDirection(vector3.normalized);
					}
					Vector3 forward = Vector3.RotateTowards(transform.rotation * Vector3.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
					transform.rotation = ((this.pivotConstraint != null) ? Quaternion.LookRotation(forward, this.pivotConstraint.up) : Quaternion.LookRotation(forward));
				}
			}
		}

		// Token: 0x060074BA RID: 29882 RVA: 0x00263BEC File Offset: 0x00261DEC
		private Transform FindTarget()
		{
			if (!PhotonNetwork.InRoom)
			{
				return GorillaTagger.Instance.offlineVRRig.tagSound.transform;
			}
			Vector3 lhs = base.transform.TransformDirection(this.normalizedLocalForward);
			float num = float.NegativeInfinity;
			Transform result = null;
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (this.includeOwner || !(vrrig == this.ownerRig))
				{
					Vector3 vector = vrrig.tagSound.transform.position - base.transform.position;
					if (vector.sqrMagnitude <= this.sqrRadius)
					{
						Vector3 normalized = vector.normalized;
						float num2 = Vector3.Dot(lhs, normalized);
						if (num2 >= this.cosAngle && num2 > num)
						{
							num = num2;
							result = vrrig.tagSound.transform;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04008616 RID: 34326
		[Header("Settings")]
		[SerializeField]
		private Transform[] lookTransforms;

		// Token: 0x04008617 RID: 34327
		[Tooltip("The local axis that points 'forward' on this transform.")]
		[SerializeField]
		private ProximityLookAt.LocalAxis localForward = ProximityLookAt.LocalAxis.Down;

		// Token: 0x04008618 RID: 34328
		[SerializeField]
		private float lookRadius = 0.5f;

		// Token: 0x04008619 RID: 34329
		[Tooltip("The cone angle in degrees used to detect nearby players.Only players within this angle of the forward direction are considered as targets.")]
		[SerializeField]
		private float targetSearchAngleDegrees = 60f;

		// Token: 0x0400861A RID: 34330
		[Tooltip("How far in degrees the transform can physically rotate from its rest position.Should be less than or equal to targetSearchAngleDegrees")]
		[SerializeField]
		private float lookAtAngleDegreeMax = 45f;

		// Token: 0x0400861B RID: 34331
		[SerializeField]
		private float rotSpeed = 180f;

		// Token: 0x0400861C RID: 34332
		[Tooltip("Seconds to hold the current target before switching to a new one")]
		[SerializeField]
		private float targetSwitchCooldown = 0.5f;

		// Token: 0x0400861D RID: 34333
		[Tooltip("Whether the cosmetic owner can be considered as a look target.")]
		[SerializeField]
		private bool includeOwner;

		// Token: 0x0400861E RID: 34334
		[Header("Pivot Clamping (Optional)")]
		[Tooltip("Assign a pivot transform to constrain rotation relative to it. Leave empty to skip clamping.")]
		[SerializeField]
		private Transform pivotConstraint;

		// Token: 0x0400861F RID: 34335
		[SerializeField]
		private float minPivotY = -1f;

		// Token: 0x04008620 RID: 34336
		[SerializeField]
		private float maxPivotY = 1f;

		// Token: 0x04008621 RID: 34337
		private TransferrableObject transferableParent;

		// Token: 0x04008622 RID: 34338
		private VRRig ownerRig;

		// Token: 0x04008623 RID: 34339
		private Transform lookTarget;

		// Token: 0x04008624 RID: 34340
		private Vector3 normalizedLocalForward;

		// Token: 0x04008625 RID: 34341
		private float cosAngle;

		// Token: 0x04008626 RID: 34342
		private float sqrRadius;

		// Token: 0x04008627 RID: 34343
		private float lastTargetSwitchTime = float.NegativeInfinity;

		// Token: 0x02001239 RID: 4665
		public enum LocalAxis
		{
			// Token: 0x04008629 RID: 34345
			Forward,
			// Token: 0x0400862A RID: 34346
			Back,
			// Token: 0x0400862B RID: 34347
			Right,
			// Token: 0x0400862C RID: 34348
			Left,
			// Token: 0x0400862D RID: 34349
			Up,
			// Token: 0x0400862E RID: 34350
			Down
		}
	}
}
