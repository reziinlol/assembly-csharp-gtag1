using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001218 RID: 4632
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060073E6 RID: 29670 RVA: 0x0025C3B6 File Offset: 0x0025A5B6
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060073E7 RID: 29671 RVA: 0x0025C3D4 File Offset: 0x0025A5D4
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float angle = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, angle, Space.World);
		}

		// Token: 0x04008482 RID: 33922
		private const float smoothTime = 0.005f;

		// Token: 0x04008483 RID: 33923
		private float currentVelocity;
	}
}
