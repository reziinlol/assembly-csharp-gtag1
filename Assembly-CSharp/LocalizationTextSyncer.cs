using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000BE1 RID: 3041
public class LocalizationTextSyncer : MonoBehaviour
{
	// Token: 0x06004BF0 RID: 19440 RVA: 0x00195ECB File Offset: 0x001940CB
	private void Start()
	{
		this.OnLanguageChanged();
	}

	// Token: 0x06004BF1 RID: 19441 RVA: 0x00195ED3 File Offset: 0x001940D3
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		this.OnLanguageChanged();
	}

	// Token: 0x06004BF2 RID: 19442 RVA: 0x00195EFA File Offset: 0x001940FA
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004BF3 RID: 19443 RVA: 0x00195EFA File Offset: 0x001940FA
	private void OnDestroy()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004BF4 RID: 19444 RVA: 0x00195F10 File Offset: 0x00194110
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair);
		LocalisationFontPair localisationFontPair2;
		bool flag = this.TryGetFontDataOverride(out localisationFontPair2);
		if (!flag && !LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair2))
		{
			return;
		}
		foreach (LocalizationTextSyncer.TextCompSyncData textCompSyncData in this._textComponentsToSync)
		{
			if (!(textCompSyncData.textComponent == null))
			{
				LocalisationFontPair localisationFontPair3;
				if (textCompSyncData.overrideLanguageSettings && textCompSyncData.GetOverrideForLanguage(out localisationFontPair3))
				{
					localisationFontPair2 = localisationFontPair3;
				}
				if (localisationFontPair2.fontAsset != null)
				{
					textCompSyncData.textComponent.font = localisationFontPair2.fontAsset;
				}
				else
				{
					textCompSyncData.textComponent.font = localisationFontPair.fontAsset;
				}
				if (flag)
				{
					textCompSyncData.textComponent.characterSpacing = localisationFontPair2.charSpacing;
					textCompSyncData.textComponent.lineSpacing = localisationFontPair2.lineSpacing;
					if (localisationFontPair2.fontSize != 0f)
					{
						textCompSyncData.textComponent.fontSize = (textCompSyncData.textComponent.fontSizeMax = localisationFontPair2.fontSize);
					}
				}
			}
		}
	}

	// Token: 0x06004BF5 RID: 19445 RVA: 0x00196038 File Offset: 0x00194238
	private bool TryGetFontDataOverride(out LocalisationFontPair fontDataOverride)
	{
		fontDataOverride = default(LocalisationFontPair);
		for (int i = 0; i < this._universalFontOverrides.Count; i++)
		{
			if (this._universalFontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontDataOverride = this._universalFontOverrides[i];
				return true;
			}
		}
		return false;
	}

	// Token: 0x04005F36 RID: 24374
	[SerializeField]
	[Tooltip("List of all the Text Components - and optional overrides - that will be updated when langauge changes")]
	private List<LocalizationTextSyncer.TextCompSyncData> _textComponentsToSync = new List<LocalizationTextSyncer.TextCompSyncData>();

	// Token: 0x04005F37 RID: 24375
	[SerializeField]
	[Tooltip("List of optional overrides that will be applied to ALL Text Components on this object")]
	private List<LocalisationFontPair> _universalFontOverrides = new List<LocalisationFontPair>();

	// Token: 0x02000BE2 RID: 3042
	[Serializable]
	public struct TextCompSyncData
	{
		// Token: 0x06004BF7 RID: 19447 RVA: 0x001960B0 File Offset: 0x001942B0
		public bool GetOverrideForLanguage(out LocalisationFontPair fontData)
		{
			fontData = default(LocalisationFontPair);
			for (int i = 0; i < this._fontOverrides.Count; i++)
			{
				if (this._fontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
				{
					fontData = this._fontOverrides[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x04005F38 RID: 24376
		public TMP_Text textComponent;

		// Token: 0x04005F39 RID: 24377
		public bool overrideLanguageSettings;

		// Token: 0x04005F3A RID: 24378
		public List<LocalisationFontPair> _fontOverrides;
	}
}
