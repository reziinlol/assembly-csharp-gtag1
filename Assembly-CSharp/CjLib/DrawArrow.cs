using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001335 RID: 4917
	[ExecuteInEditMode]
	public class DrawArrow : DrawBase
	{
		// Token: 0x06007BBE RID: 31678 RVA: 0x00285A6C File Offset: 0x00283C6C
		private void OnValidate()
		{
			this.ConeRadius = Mathf.Max(0f, this.ConeRadius);
			this.ConeHeight = Mathf.Max(0f, this.ConeHeight);
			this.StemThickness = Mathf.Max(0f, this.StemThickness);
			this.NumSegments = Mathf.Max(4, this.NumSegments);
		}

		// Token: 0x06007BBF RID: 31679 RVA: 0x00285AD0 File Offset: 0x00283CD0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawArrow(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), this.ConeRadius, this.ConeHeight, this.NumSegments, this.StemThickness, color, depthTest, style);
		}

		// Token: 0x04008D0E RID: 36110
		public Vector3 LocalEndVector = Vector3.right;

		// Token: 0x04008D0F RID: 36111
		public float ConeRadius = 0.05f;

		// Token: 0x04008D10 RID: 36112
		public float ConeHeight = 0.1f;

		// Token: 0x04008D11 RID: 36113
		public float StemThickness = 0.05f;

		// Token: 0x04008D12 RID: 36114
		public int NumSegments = 8;
	}
}
