using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;

// Token: 0x02000BE5 RID: 3045
[DisallowMultipleComponent]
public class LocalizedText : LocalizeStringEvent
{
	// Token: 0x06004BFA RID: 19450 RVA: 0x00196159 File Offset: 0x00194359
	public bool HasFontOverrides()
	{
		return this._localisationFontsOverrides.Count > 0;
	}

	// Token: 0x17000727 RID: 1831
	// (get) Token: 0x06004BFB RID: 19451 RVA: 0x00196169 File Offset: 0x00194369
	private TextComponentLegacySupportStore TextComponent
	{
		get
		{
			if (!this._textComponent.IsValid)
			{
				this._textComponent = new TextComponentLegacySupportStore(base.transform);
			}
			return this._textComponent;
		}
	}

	// Token: 0x06004BFC RID: 19452 RVA: 0x00196190 File Offset: 0x00194390
	private void Awake()
	{
		this._textComponent = new TextComponentLegacySupportStore(base.transform);
		base.OnUpdateString = new UnityEventString();
		base.OnUpdateString.AddListener(delegate(string val)
		{
			this.OnLocaleChanged(val);
		});
		if (!this.TextComponent.IsValid)
		{
			base.gameObject.AddComponent<TMP_Text>();
			this._textComponent = new TextComponentLegacySupportStore(base.transform);
		}
	}

	// Token: 0x06004BFD RID: 19453 RVA: 0x00196200 File Offset: 0x00194400
	protected override void UpdateString(string value)
	{
		LocalizedText.<UpdateString>d__11 <UpdateString>d__;
		<UpdateString>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateString>d__.<>4__this = this;
		<UpdateString>d__.value = value;
		<UpdateString>d__.<>1__state = -1;
		<UpdateString>d__.<>t__builder.Start<LocalizedText.<UpdateString>d__11>(ref <UpdateString>d__);
	}

	// Token: 0x06004BFE RID: 19454 RVA: 0x00196240 File Offset: 0x00194440
	private void OnLocaleChanged(string newText)
	{
		LocalizedText.<OnLocaleChanged>d__12 <OnLocaleChanged>d__;
		<OnLocaleChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnLocaleChanged>d__.<>4__this = this;
		<OnLocaleChanged>d__.newText = newText;
		<OnLocaleChanged>d__.<>1__state = -1;
		<OnLocaleChanged>d__.<>t__builder.Start<LocalizedText.<OnLocaleChanged>d__12>(ref <OnLocaleChanged>d__);
	}

	// Token: 0x06004BFF RID: 19455 RVA: 0x00196280 File Offset: 0x00194480
	private bool GetLocalizedFonts(out LocalisationFontPair fontData)
	{
		fontData = default(LocalisationFontPair);
		if (!this.HasFontOverrides())
		{
			return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
		}
		for (int i = 0; i < this._localisationFontsOverrides.Count; i++)
		{
			if (this._localisationFontsOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontData = new LocalisationFontPair
				{
					fontAsset = this._localisationFontsOverrides[i].fontAsset,
					legacyFontAsset = this._localisationFontsOverrides[i].legacyFontAsset,
					charSpacing = this._localisationFontsOverrides[i].charSpacing
				};
				return true;
			}
		}
		return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
	}

	// Token: 0x04005F3D RID: 24381
	[SerializeField]
	private bool _isLocalized;

	// Token: 0x04005F3E RID: 24382
	[SerializeField]
	private bool _isNewKey;

	// Token: 0x04005F3F RID: 24383
	[SerializeField]
	private string _newKeyName;

	// Token: 0x04005F40 RID: 24384
	[SerializeField]
	private ELocale _previewLocale;

	// Token: 0x04005F41 RID: 24385
	[SerializeField]
	private List<LocalisationFontPair> _localisationFontsOverrides = new List<LocalisationFontPair>();

	// Token: 0x04005F42 RID: 24386
	private static List<ELocale> _cachedELocalesList = new List<ELocale>();

	// Token: 0x04005F43 RID: 24387
	private TextComponentLegacySupportStore _textComponent;
}
