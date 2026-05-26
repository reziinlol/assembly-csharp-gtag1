using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001336 RID: 4918
	public abstract class DrawBase : MonoBehaviour
	{
		// Token: 0x06007BC1 RID: 31681 RVA: 0x00285B64 File Offset: 0x00283D64
		private void Update()
		{
			if (this.Style != DebugUtil.Style.Wireframe)
			{
				this.Draw(this.ShadededColor, this.Style, this.DepthTest);
			}
			if (this.Style == DebugUtil.Style.Wireframe || this.Wireframe)
			{
				this.Draw(this.WireframeColor, DebugUtil.Style.Wireframe, this.DepthTest);
			}
		}

		// Token: 0x06007BC2 RID: 31682
		protected abstract void Draw(Color color, DebugUtil.Style style, bool depthTest);

		// Token: 0x04008D13 RID: 36115
		public Color WireframeColor = Color.white;

		// Token: 0x04008D14 RID: 36116
		public Color ShadededColor = Color.gray;

		// Token: 0x04008D15 RID: 36117
		public bool Wireframe;

		// Token: 0x04008D16 RID: 36118
		public DebugUtil.Style Style = DebugUtil.Style.FlatShaded;

		// Token: 0x04008D17 RID: 36119
		public bool DepthTest = true;
	}
}
