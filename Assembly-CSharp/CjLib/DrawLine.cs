using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001339 RID: 4921
	[ExecuteInEditMode]
	public class DrawLine : DrawBase
	{
		// Token: 0x06007BCA RID: 31690 RVA: 0x00285D2B File Offset: 0x00283F2B
		private void OnValidate()
		{
			this.Wireframe = true;
			this.Style = DebugUtil.Style.Wireframe;
		}

		// Token: 0x06007BCB RID: 31691 RVA: 0x00285D3B File Offset: 0x00283F3B
		protected override void Draw(Color color, DebugUtil.Style style, bool depthTest)
		{
			DebugUtil.DrawLine(base.transform.position, base.transform.position + base.transform.TransformVector(this.LocalEndVector), color, depthTest);
		}

		// Token: 0x04008D1E RID: 36126
		public Vector3 LocalEndVector = Vector3.right;
	}
}
