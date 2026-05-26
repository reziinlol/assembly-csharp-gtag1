using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001337 RID: 4919
	[ExecuteInEditMode]
	public class DrawBox : DrawBase
	{
		// Token: 0x06007BC4 RID: 31684 RVA: 0x00285BE0 File Offset: 0x00283DE0
		private void OnValidate()
		{
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007BC5 RID: 31685 RVA: 0x00285C0C File Offset: 0x00283E0C
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04008D18 RID: 36120
		public float Radius = 1f;

		// Token: 0x04008D19 RID: 36121
		public int NumSegments = 64;

		// Token: 0x04008D1A RID: 36122
		public float StartAngle;

		// Token: 0x04008D1B RID: 36123
		public float ArcAngle = 60f;
	}
}
