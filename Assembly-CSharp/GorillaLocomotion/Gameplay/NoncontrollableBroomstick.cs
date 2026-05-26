using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001102 RID: 4354
	public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
	{
		// Token: 0x06006DA4 RID: 28068 RVA: 0x0023D478 File Offset: 0x0023B678
		private void Start()
		{
			this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
			this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
			this.progress = this.SplineProgressOffet;
			this.secondsToCycles = 1.0 / (double)this.duration;
			if (this.unitySpline != null)
			{
				this.nativeSpline = new NativeSpline(this.unitySpline.Spline, this.unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			}
		}

		// Token: 0x06006DA5 RID: 28069 RVA: 0x0023D508 File Offset: 0x0023B708
		protected virtual void FixedUpdate()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this.secondsToCycles + (double)this.SplineProgressOffet;
				this.progress = (float)(num % 1.0);
			}
			else
			{
				this.progress = (this.progress + this.progressPerFixedUpdate) % 1f;
			}
			Quaternion a = Quaternion.identity;
			if (this.unitySpline != null)
			{
				float3 v;
				float3 @float;
				float3 float2;
				this.nativeSpline.Evaluate(this.progress, out v, out @float, out float2);
				base.transform.position = v;
				if (this.lookForward)
				{
					a = Quaternion.LookRotation(new Vector3(@float.x, @float.y, @float.z));
				}
			}
			else if (this.spline != null)
			{
				Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
				base.transform.position = point;
				if (this.lookForward)
				{
					a = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
				}
			}
			if (this.lookForward)
			{
				base.transform.rotation = Quaternion.Slerp(a, base.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
			}
		}

		// Token: 0x06006DA6 RID: 28070 RVA: 0x00023994 File Offset: 0x00021B94
		bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber)
		{
			return true;
		}

		// Token: 0x06006DA7 RID: 28071 RVA: 0x0023D651 File Offset: 0x0023B851
		void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedObject, out Vector3 grabbedLocalPosition)
		{
			grabbedObject = base.transform;
			grabbedLocalPosition = base.transform.InverseTransformPoint(g.transform.position);
		}

		// Token: 0x06006DA8 RID: 28072 RVA: 0x000028C5 File Offset: 0x00000AC5
		void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
		{
		}

		// Token: 0x06006DA9 RID: 28073 RVA: 0x0023D677 File Offset: 0x0023B877
		private void OnDestroy()
		{
			this.nativeSpline.Dispose();
		}

		// Token: 0x06006DAA RID: 28074 RVA: 0x0023D684 File Offset: 0x0023B884
		public bool MomentaryGrabOnly()
		{
			return this.momentaryGrabOnly;
		}

		// Token: 0x06006DAC RID: 28076 RVA: 0x00014807 File Offset: 0x00012A07
		string IGorillaGrabable.get_name()
		{
			return base.name;
		}

		// Token: 0x04007EAB RID: 32427
		public SplineContainer unitySpline;

		// Token: 0x04007EAC RID: 32428
		public BezierSpline spline;

		// Token: 0x04007EAD RID: 32429
		public float duration = 30f;

		// Token: 0x04007EAE RID: 32430
		public float smoothRotationTrackingRate = 0.5f;

		// Token: 0x04007EAF RID: 32431
		public bool lookForward = true;

		// Token: 0x04007EB0 RID: 32432
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x04007EB1 RID: 32433
		private float progress;

		// Token: 0x04007EB2 RID: 32434
		private float smoothRotationTrackingRateExp;

		// Token: 0x04007EB3 RID: 32435
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04007EB4 RID: 32436
		private float progressPerFixedUpdate;

		// Token: 0x04007EB5 RID: 32437
		private double secondsToCycles;

		// Token: 0x04007EB6 RID: 32438
		private NativeSpline nativeSpline;

		// Token: 0x04007EB7 RID: 32439
		[SerializeField]
		private bool momentaryGrabOnly = true;
	}
}
