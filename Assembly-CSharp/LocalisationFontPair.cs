using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000BD8 RID: 3032
[Serializable]
public struct LocalisationFontPair
{
	// Token: 0x06004BB7 RID: 19383 RVA: 0x00194E04 File Offset: 0x00193004
	public bool ContainsLocale(Locale locale)
	{
		int count = this.locales.Count;
		for (int i = 0; i < this.locales.Count; i++)
		{
			if (!(this.locales[i] == null) && this.locales[i].Identifier.Code == locale.Identifier.Code)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04005EFE RID: 24318
	public List<Locale> locales;

	// Token: 0x04005EFF RID: 24319
	public TMP_FontAsset fontAsset;

	// Token: 0x04005F00 RID: 24320
	public Font legacyFontAsset;

	// Token: 0x04005F01 RID: 24321
	public float charSpacing;

	// Token: 0x04005F02 RID: 24322
	public float lineSpacing;

	// Token: 0x04005F03 RID: 24323
	public float fontSize;
}
