using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x020010BA RID: 4282
	public class EnviromentMovement : MonoBehaviour
	{
		// Token: 0x06006B77 RID: 27511 RVA: 0x0022BDF3 File Offset: 0x00229FF3
		private void Start()
		{
			this.thisTransform = base.gameObject.GetComponent<Transform>();
			this.nextPosition = this.pos1;
		}

		// Token: 0x06006B78 RID: 27512 RVA: 0x0022BE14 File Offset: 0x0022A014
		private void Update()
		{
			if (Vector3.Distance(this.thisTransform.position, this.nextPosition) > 0.5f)
			{
				base.transform.position = Vector3.Lerp(this.thisTransform.position, this.nextPosition, 2f * Time.deltaTime);
				return;
			}
			if (this.nextPosition == this.pos1)
			{
				this.nextPosition = this.pos2;
				return;
			}
			if (this.nextPosition == this.pos2)
			{
				this.nextPosition = this.pos1;
				return;
			}
		}

		// Token: 0x04007B79 RID: 31609
		private Vector3 nextPosition = Vector3.zero;

		// Token: 0x04007B7A RID: 31610
		private Transform thisTransform;

		// Token: 0x04007B7B RID: 31611
		public Vector3 pos1;

		// Token: 0x04007B7C RID: 31612
		public Vector3 pos2;
	}
}
