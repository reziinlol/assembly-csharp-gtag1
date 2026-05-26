using System;

// Token: 0x0200060D RID: 1549
public class BuilderRendererPreRender : MonoBehaviourPostTick
{
	// Token: 0x0600269C RID: 9884 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000CC5FD File Offset: 0x000CA7FD
	public override void PostTick()
	{
		if (this.builderRenderer != null)
		{
			this.builderRenderer.PreRenderIndirect();
		}
	}

	// Token: 0x0400320B RID: 12811
	public BuilderRenderer builderRenderer;
}
