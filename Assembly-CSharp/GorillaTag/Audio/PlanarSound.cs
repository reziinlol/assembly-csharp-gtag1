using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011EE RID: 4590
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x0600732F RID: 29487 RVA: 0x00256F75 File Offset: 0x00255175
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x06007330 RID: 29488 RVA: 0x00256F9C File Offset: 0x0025519C
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 localPosition = transform.parent.InverseTransformPoint(this.cameraXform.position);
			localPosition.y = 0f;
			if (this.limitDistance && localPosition.sqrMagnitude > this.maxDistance * this.maxDistance)
			{
				localPosition = localPosition.normalized * this.maxDistance;
			}
			transform.localPosition = localPosition;
		}

		// Token: 0x04008396 RID: 33686
		private Transform cameraXform;

		// Token: 0x04008397 RID: 33687
		private bool hasCamera;

		// Token: 0x04008398 RID: 33688
		[SerializeField]
		private bool limitDistance;

		// Token: 0x04008399 RID: 33689
		[SerializeField]
		private float maxDistance = 1f;
	}
}
