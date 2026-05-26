using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001176 RID: 4470
	public class SimpleAABB : MonoBehaviour
	{
		// Token: 0x06007131 RID: 28977 RVA: 0x0024FB92 File Offset: 0x0024DD92
		private void Awake()
		{
			this.m_bounds = new Bounds(this.m_center, this.m_size);
		}

		// Token: 0x06007132 RID: 28978 RVA: 0x0024FBAC File Offset: 0x0024DDAC
		public bool IsInBounds(Vector3 point)
		{
			Vector3 point2 = base.transform.InverseTransformPoint(point);
			return this.m_bounds.Contains(point2);
		}

		// Token: 0x04008135 RID: 33077
		[SerializeField]
		private Vector3 m_center;

		// Token: 0x04008136 RID: 33078
		[SerializeField]
		private Vector3 m_size;

		// Token: 0x04008137 RID: 33079
		private Bounds m_bounds;
	}
}
