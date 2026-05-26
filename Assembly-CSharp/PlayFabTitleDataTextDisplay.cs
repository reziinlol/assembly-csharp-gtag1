using System;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

// Token: 0x02000A20 RID: 2592
public class PlayFabTitleDataTextDisplay : MonoBehaviour, IBuildValidation
{
	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x0600424B RID: 16971 RVA: 0x0016235A File Offset: 0x0016055A
	public string playFabKeyValue
	{
		get
		{
			return this.playfabKey;
		}
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x00162364 File Offset: 0x00160564
	private void Start()
	{
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
		if (!this._hasRegisteredCallback)
		{
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x001623F9 File Offset: 0x001605F9
	private void OnEnable()
	{
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = true;
	}

	// Token: 0x0600424E RID: 16974 RVA: 0x00162421 File Offset: 0x00160621
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = false;
	}

	// Token: 0x0600424F RID: 16975 RVA: 0x0016243C File Offset: 0x0016063C
	private void OnPlayFabError(PlayFabError error)
	{
		if (this.textBox != null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"PlayFabTitleDataTextDisplay: PlayFab error retrieving title data for key '",
				this.playfabKey,
				"' displayed '",
				this.fallbackText,
				"': ",
				error.GenerateErrorReport()
			}));
			if (this._fallbackLocalizedText == null || this._fallbackLocalizedText.IsEmpty)
			{
				this.textBox.text = this.fallbackText;
				return;
			}
			string text;
			if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._fallbackLocalizedText, out text, this.fallbackText, null))
			{
				Debug.LogError("[LOCALIZATION::PLAYFAB_TITLEDATA_TEXT_DISPLAY] Failed to get key for PlayFab Title Data Text [_fallbackLocalizedText]");
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x06004250 RID: 16976 RVA: 0x001624F0 File Offset: 0x001606F0
	private void OnLanguageChanged()
	{
		if (string.IsNullOrEmpty(this._cachedText))
		{
			Debug.LogError("[LOCALIZATION::PLAY_FAB_TITLE_DATA_TEXT_DISPLAY] [_cachedText] is not set yet, is this being called before title data has been obtained?");
			return;
		}
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x06004251 RID: 16977 RVA: 0x00162540 File Offset: 0x00160740
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		if (this.textBox != null)
		{
			this._cachedText = titleDataResult;
			string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
			if (text[0] == '"' && text[text.Length - 1] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x06004252 RID: 16978 RVA: 0x001625BB File Offset: 0x001607BB
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey && this.textBox != null)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x06004253 RID: 16979 RVA: 0x001625EA File Offset: 0x001607EA
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x06004254 RID: 16980 RVA: 0x00162607 File Offset: 0x00160807
	public bool BuildValidationCheck()
	{
		if (this.textBox == null)
		{
			Debug.LogError("text reference is null! sign text will be broken");
			return false;
		}
		return true;
	}

	// Token: 0x06004255 RID: 16981 RVA: 0x00162624 File Offset: 0x00160824
	public void ChangeTitleDataAtRuntime(string newTitleDataKey)
	{
		this.playfabKey = newTitleDataKey;
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x04005427 RID: 21543
	[SerializeField]
	private TextMeshPro textBox;

	// Token: 0x04005428 RID: 21544
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x04005429 RID: 21545
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x0400542A RID: 21546
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x0400542B RID: 21547
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;

	// Token: 0x0400542C RID: 21548
	[SerializeField]
	private LocalizedString _fallbackLocalizedText;

	// Token: 0x0400542D RID: 21549
	private bool _hasRegisteredCallback;

	// Token: 0x0400542E RID: 21550
	private string _cachedText = string.Empty;
}
