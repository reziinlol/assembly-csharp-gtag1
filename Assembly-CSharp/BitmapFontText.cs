using System;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class BitmapFontText : MonoBehaviour
{
	// Token: 0x06001E75 RID: 7797 RVA: 0x000A2A6F File Offset: 0x000A0C6F
	private void Awake()
	{
		this.Init();
		this.Render();
	}

	// Token: 0x06001E76 RID: 7798 RVA: 0x000A2A7D File Offset: 0x000A0C7D
	public void Render()
	{
		this.font.RenderToTexture(this.texture, this.uppercaseOnly ? this.text.ToUpperInvariant() : this.text);
	}

	// Token: 0x06001E77 RID: 7799 RVA: 0x000A2AAC File Offset: 0x000A0CAC
	public void Init()
	{
		this.texture = new Texture2D(this.textArea.x, this.textArea.y, this.font.fontImage.format, false);
		this.texture.filterMode = FilterMode.Point;
		this.material = new Material(this.renderer.sharedMaterial);
		this.material.mainTexture = this.texture;
		this.renderer.sharedMaterial = this.material;
	}

	// Token: 0x040028AB RID: 10411
	public string text;

	// Token: 0x040028AC RID: 10412
	public bool uppercaseOnly;

	// Token: 0x040028AD RID: 10413
	public Vector2Int textArea;

	// Token: 0x040028AE RID: 10414
	[Space]
	public Renderer renderer;

	// Token: 0x040028AF RID: 10415
	public Texture2D texture;

	// Token: 0x040028B0 RID: 10416
	public Material material;

	// Token: 0x040028B1 RID: 10417
	public BitmapFont font;
}
