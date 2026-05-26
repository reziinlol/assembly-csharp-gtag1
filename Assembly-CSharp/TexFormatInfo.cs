using System;
using UnityEngine;

// Token: 0x0200038B RID: 907
public struct TexFormatInfo
{
	// Token: 0x060015E9 RID: 5609 RVA: 0x0007FE80 File Offset: 0x0007E080
	public TexFormatInfo(Texture2D tex2d)
	{
		this.width = tex2d.width;
		this.height = tex2d.height;
		this.format = tex2d.format;
		this.filterMode = tex2d.filterMode;
		this.isLinearColor = !tex2d.isDataSRGB;
		this.mipmapCount = tex2d.mipmapCount;
		this.isValid = true;
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x0007FEE0 File Offset: 0x0007E0E0
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"TexFormatInfo(isValid: ",
			this.isValid.ToString(),
			", width: ",
			this.width.ToString(),
			", height: ",
			this.height.ToString(),
			", format: ",
			this.format.ToString(),
			", filterMode: ",
			this.filterMode.ToString(),
			", isLinearColor: ",
			this.isLinearColor.ToString(),
			", mipmapCount: ",
			this.mipmapCount.ToString(),
			")"
		});
	}

	// Token: 0x04002036 RID: 8246
	public bool isValid;

	// Token: 0x04002037 RID: 8247
	public int width;

	// Token: 0x04002038 RID: 8248
	public int height;

	// Token: 0x04002039 RID: 8249
	public TextureFormat format;

	// Token: 0x0400203A RID: 8250
	public FilterMode filterMode;

	// Token: 0x0400203B RID: 8251
	public int mipmapCount;

	// Token: 0x0400203C RID: 8252
	public bool isLinearColor;
}
