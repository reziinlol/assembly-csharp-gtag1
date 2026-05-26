using System;
using UnityEngine;

// Token: 0x02000BE9 RID: 3049
[Serializable]
public class TitleDataLocalization
{
	// Token: 0x06004C14 RID: 19476 RVA: 0x001968EC File Offset: 0x00194AEC
	public string GetLocalizedText()
	{
		Debug.Log("TODO: JH - Review localization method");
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		if (!(code == "en"))
		{
			if (code == "fr")
			{
				return this.French;
			}
			if (code == "es")
			{
				return this.Spanish;
			}
			if (code == "it")
			{
				return this.Italian;
			}
			if (code == "de")
			{
				return this.German;
			}
			if (code == "ja")
			{
				return this.Japanese;
			}
		}
		return this.English;
	}

	// Token: 0x04005F52 RID: 24402
	public string English;

	// Token: 0x04005F53 RID: 24403
	public string French;

	// Token: 0x04005F54 RID: 24404
	public string German;

	// Token: 0x04005F55 RID: 24405
	public string Spanish;

	// Token: 0x04005F56 RID: 24406
	public string Italian;

	// Token: 0x04005F57 RID: 24407
	public string Japanese;
}
