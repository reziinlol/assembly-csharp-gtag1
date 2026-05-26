using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001334 RID: 4916
	[ExecuteInEditMode]
	public class DrawArc : DrawBase
	{
		// Token: 0x06007BBB RID: 31675 RVA: 0x0028598F File Offset: 0x00283B8F
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
			this.Radius = Mathf.Max(0f, this.Radius);
			this.NumSegments = Mathf.Max(0, this.NumSegments);
		}

		// Token: 0x06007BBC RID: 31676 RVA: 0x002859C8 File Offset: 0x00283BC8
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			Quaternion rhs = QuaternionUtil.AxisAngle(Vector3.forward, this.StartAngle * MathUtil.Deg2Rad);
			DebugUtil.DrawArc(base.transform.position, base.transform.rotation * rhs * Vector3.right, base.transform.rotation * Vector3.forward, this.ArcAngle * MathUtil.Deg2Rad, this.Radius, this.NumSegments, color, depthTest);
		}

		// Token: 0x04008D0A RID: 36106
		public float Radius = 1f;

		// Token: 0x04008D0B RID: 36107
		public int NumSegments = 64;

		// Token: 0x04008D0C RID: 36108
		public float StartAngle;

		// Token: 0x04008D0D RID: 36109
		public float ArcAngle = 60f;
	}
}
