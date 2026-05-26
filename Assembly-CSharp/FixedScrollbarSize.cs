using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000A0 RID: 160
public class FixedScrollbarSize : MonoBehaviour
{
	// Token: 0x060003F8 RID: 1016 RVA: 0x000179B0 File Offset: 0x00015BB0
	private void OnEnable()
	{
		this.EnforceScrollbarSize();
		CanvasUpdateRegistry.instance.Equals(null);
		Canvas.willRenderCanvases += this.EnforceScrollbarSize;
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x000179D5 File Offset: 0x00015BD5
	private void OnDisable()
	{
		Canvas.willRenderCanvases -= this.EnforceScrollbarSize;
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x000179E8 File Offset: 0x00015BE8
	private void EnforceScrollbarSize()
	{
		if (this.ScrollRect.horizontalScrollbar && this.ScrollRect.horizontalScrollbar.size != this.HorizontalBarSize)
		{
			this.ScrollRect.horizontalScrollbar.size = this.HorizontalBarSize;
		}
		if (this.ScrollRect.verticalScrollbar && this.ScrollRect.verticalScrollbar.size != this.VerticalBarSize)
		{
			this.ScrollRect.verticalScrollbar.size = this.VerticalBarSize;
		}
	}

	// Token: 0x04000466 RID: 1126
	public ScrollRect ScrollRect;

	// Token: 0x04000467 RID: 1127
	public float HorizontalBarSize = 0.2f;

	// Token: 0x04000468 RID: 1128
	public float VerticalBarSize = 0.2f;
}
