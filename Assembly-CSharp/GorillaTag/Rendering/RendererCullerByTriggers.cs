using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020011FD RID: 4605
	public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
	{
		// Token: 0x06007386 RID: 29574 RVA: 0x00258AA8 File Offset: 0x00256CA8
		protected void OnEnable()
		{
			this.camWasTouching = false;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
		}

		// Token: 0x06007387 RID: 29575 RVA: 0x00258B04 File Offset: 0x00256D04
		protected void LateUpdate()
		{
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
			Vector3 position = this.mainCameraTransform.position;
			bool flag = false;
			foreach (Collider collider in this.colliders)
			{
				if (!(collider == null) && (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = flag;
				}
			}
		}

		// Token: 0x06007388 RID: 29576 RVA: 0x00023994 File Offset: 0x00021B94
		public bool BuildValidationCheck()
		{
			return true;
		}

		// Token: 0x040083DF RID: 33759
		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		// Token: 0x040083E0 RID: 33760
		public Collider[] colliders;

		// Token: 0x040083E1 RID: 33761
		private bool camWasTouching;

		// Token: 0x040083E2 RID: 33762
		private const float cameraRadiusSq = 0.010000001f;

		// Token: 0x040083E3 RID: 33763
		private Transform mainCameraTransform;
	}
}
