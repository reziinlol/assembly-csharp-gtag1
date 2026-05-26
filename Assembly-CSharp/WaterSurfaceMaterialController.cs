using System;
using UnityEngine;

// Token: 0x020001A2 RID: 418
[ExecuteAlways]
public class WaterSurfaceMaterialController : MonoBehaviour
{
	// Token: 0x06000B40 RID: 2880 RVA: 0x0003C14D File Offset: 0x0003A34D
	protected void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.matPropBlock = new MaterialPropertyBlock();
		this.ApplyProperties();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x0003C16C File Offset: 0x0003A36C
	private void ApplyProperties()
	{
		this.matPropBlock.SetVector(ShaderProps._ScrollSpeedAndScale, new Vector4(this.ScrollX, this.ScrollY, this.Scale, 0f));
		if (this.renderer)
		{
			this.renderer.SetPropertyBlock(this.matPropBlock);
		}
	}

	// Token: 0x04000D86 RID: 3462
	public float ScrollX = 0.6f;

	// Token: 0x04000D87 RID: 3463
	public float ScrollY = 0.6f;

	// Token: 0x04000D88 RID: 3464
	public float Scale = 1f;

	// Token: 0x04000D89 RID: 3465
	private Renderer renderer;

	// Token: 0x04000D8A RID: 3466
	private MaterialPropertyBlock matPropBlock;
}
