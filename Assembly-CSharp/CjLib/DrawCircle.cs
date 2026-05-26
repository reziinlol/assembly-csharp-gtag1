using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001338 RID: 4920
	[ExecuteInEditMode]
	public class DrawCircle : DrawBase
	{
		// Token: 0x06007BC7 RID: 31687 RVA: 0x00285CB0 File Offset: 0x00283EB0
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007BC8 RID: 31688 RVA: 0x00285CDA File Offset: 0x00283EDA
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawCircle(base.transform.position, base.transform.rotation * Vector3.back, this.Radius, this.NumSegments, color, depthTest, style);
		}

		// Token: 0x04008D1C RID: 36124
		public float Radius = 1f;

		// Token: 0x04008D1D RID: 36125
		public int NumSegments = 64;
	}
}
