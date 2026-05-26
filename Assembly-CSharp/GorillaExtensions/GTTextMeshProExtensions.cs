using System;
using Cysharp.Text;
using TMPro;

namespace GorillaExtensions
{
	// Token: 0x0200111A RID: 4378
	public static class GTTextMeshProExtensions
	{
		// Token: 0x06006E3D RID: 28221 RVA: 0x00240F1C File Offset: 0x0023F11C
		public static void SetTextToZString(this TMP_Text textMono, Utf16ValueStringBuilder zStringBuilder)
		{
			ArraySegment<char> arraySegment = zStringBuilder.AsArraySegment();
			textMono.SetCharArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
	}
}
