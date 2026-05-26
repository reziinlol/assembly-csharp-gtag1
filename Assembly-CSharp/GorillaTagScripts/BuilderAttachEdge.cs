using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED0 RID: 3792
	public class BuilderAttachEdge : MonoBehaviour
	{
		// Token: 0x06005D65 RID: 23909 RVA: 0x001D981E File Offset: 0x001D7A1E
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06005D66 RID: 23910 RVA: 0x001D983C File Offset: 0x001D7A3C
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Transform transform = this.center;
			if (transform == null)
			{
				transform = base.transform;
			}
			Vector3 a = transform.rotation * Vector3.right;
			Gizmos.DrawLine(transform.position - a * this.length * 0.5f, transform.position + a * this.length * 0.5f);
		}

		// Token: 0x04006BF2 RID: 27634
		public Transform center;

		// Token: 0x04006BF3 RID: 27635
		public float length;
	}
}
