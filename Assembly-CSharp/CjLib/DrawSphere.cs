using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200133A RID: 4922
	[ExecuteInEditMode]
	public class DrawSphere : DrawBase
	{
		// Token: 0x06007BCD RID: 31693 RVA: 0x00285D83 File Offset: 0x00283F83
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.LatSegments = Mathf.Max(0, this.LatSegments);
		}

		// Token: 0x06007BCE RID: 31694 RVA: 0x00285DB0 File Offset: 0x00283FB0
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawSphere(base.transform.position, base.transform.rotation, this.Radius * base.transform.lossyScale.x, this.LatSegments, this.LongSegments, color, depthTest, style);
		}

		// Token: 0x04008D1F RID: 36127
		public float Radius = 1f;

		// Token: 0x04008D20 RID: 36128
		public int LatSegments = 12;

		// Token: 0x04008D21 RID: 36129
		public int LongSegments = 12;
	}
}
