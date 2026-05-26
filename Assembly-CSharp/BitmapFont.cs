using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class BitmapFont : ScriptableObject
{
	// Token: 0x06001E6E RID: 7790 RVA: 0x000A28C4 File Offset: 0x000A0AC4
	private void OnEnable()
	{
		this._charToSymbol = this.symbols.ToDictionary((BitmapFont.SymbolData s) => s.character, (BitmapFont.SymbolData s) => s);
	}

	// Token: 0x06001E6F RID: 7791 RVA: 0x000A2920 File Offset: 0x000A0B20
	public void RenderToTexture(Texture2D target, string text)
	{
		if (text == null)
		{
			text = string.Empty;
		}
		int num = target.width * target.height;
		if (this._empty.Length != num)
		{
			this._empty = new Color[num];
			for (int i = 0; i < this._empty.Length; i++)
			{
				this._empty[i] = Color.black;
			}
		}
		target.SetPixels(this._empty);
		int length = text.Length;
		int num2 = 1;
		int width = this.fontImage.width;
		int height = this.fontImage.height;
		for (int j = 0; j < length; j++)
		{
			char key = text[j];
			BitmapFont.SymbolData symbolData = this._charToSymbol[key];
			int width2 = symbolData.width;
			int height2 = symbolData.height;
			int x = symbolData.x;
			int y = symbolData.y;
			Graphics.CopyTexture(this.fontImage, 0, 0, x, height - (y + height2), width2, height2, target, 0, 0, num2, 2 + symbolData.yoffset);
			num2 += width2 + 1;
		}
		target.Apply(false);
	}

	// Token: 0x04002899 RID: 10393
	public Texture2D fontImage;

	// Token: 0x0400289A RID: 10394
	public TextAsset fontJson;

	// Token: 0x0400289B RID: 10395
	public int symbolPixelsPerUnit = 1;

	// Token: 0x0400289C RID: 10396
	public string characterMap;

	// Token: 0x0400289D RID: 10397
	[Space]
	public BitmapFont.SymbolData[] symbols = new BitmapFont.SymbolData[0];

	// Token: 0x0400289E RID: 10398
	private Dictionary<char, BitmapFont.SymbolData> _charToSymbol;

	// Token: 0x0400289F RID: 10399
	private Color[] _empty = new Color[0];

	// Token: 0x020004E4 RID: 1252
	[Serializable]
	public struct SymbolData
	{
		// Token: 0x040028A0 RID: 10400
		public char character;

		// Token: 0x040028A1 RID: 10401
		[Space]
		public int id;

		// Token: 0x040028A2 RID: 10402
		public int width;

		// Token: 0x040028A3 RID: 10403
		public int height;

		// Token: 0x040028A4 RID: 10404
		public int x;

		// Token: 0x040028A5 RID: 10405
		public int y;

		// Token: 0x040028A6 RID: 10406
		public int xadvance;

		// Token: 0x040028A7 RID: 10407
		public int yoffset;
	}
}
