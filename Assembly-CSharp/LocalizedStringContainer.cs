using System;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000BE4 RID: 3044
[Serializable]
public struct LocalizedStringContainer
{
	// Token: 0x06004BF9 RID: 19449 RVA: 0x0019610C File Offset: 0x0019430C
	public string GetName()
	{
		string localizedString = this.StringReference.GetLocalizedString();
		string text = (localizedString != null) ? localizedString.ToUpper() : null;
		if (string.IsNullOrEmpty(text) || text.ToLower().Contains("no translation found"))
		{
			return this.FallbackName;
		}
		return text;
	}

	// Token: 0x04005F3B RID: 24379
	[SerializeField]
	private LocalizedString StringReference;

	// Token: 0x04005F3C RID: 24380
	[SerializeField]
	private string FallbackName;
}
