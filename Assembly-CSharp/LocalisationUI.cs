using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000BDC RID: 3036
public class LocalisationUI : MonoBehaviour
{
	// Token: 0x17000725 RID: 1829
	// (get) Token: 0x06004BE1 RID: 19425 RVA: 0x00195AE5 File Offset: 0x00193CE5
	public static LocalisationUI Instance
	{
		get
		{
			return LocalisationUI._instance;
		}
	}

	// Token: 0x06004BE2 RID: 19426 RVA: 0x00195AEC File Offset: 0x00193CEC
	private void Awake()
	{
		if (LocalisationUI._instance != null)
		{
			Object.DestroyImmediate(this);
			return;
		}
		LocalisationUI._instance = this;
	}

	// Token: 0x06004BE3 RID: 19427 RVA: 0x00195B08 File Offset: 0x00193D08
	private void Start()
	{
		this.ConstructLocalisationUI();
		this.CheckSelectedLanguage();
	}

	// Token: 0x06004BE4 RID: 19428 RVA: 0x00195B16 File Offset: 0x00193D16
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (this._hasConstructedUI)
		{
			this.CheckSelectedLanguage();
		}
	}

	// Token: 0x06004BE5 RID: 19429 RVA: 0x00195B37 File Offset: 0x00193D37
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004BE6 RID: 19430 RVA: 0x00195B4C File Offset: 0x00193D4C
	public void OnLanguageButtonPressed(KIDUIButton objRef, int languageIndex)
	{
		if (objRef != this._activeButton)
		{
			KIDUIButton activeButton = this._activeButton;
			if (activeButton != null)
			{
				activeButton.SetBorderImage(this._inactiveSprite);
			}
			objRef.SetBorderImage(this._activeSprite);
			this._activeButton = objRef;
		}
		Locale locale;
		if (!LocalisationManager.TryGetLocaleBinding(languageIndex, out locale))
		{
			return;
		}
		LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, false);
	}

	// Token: 0x06004BE7 RID: 19431 RVA: 0x00195BB5 File Offset: 0x00193DB5
	public void OnContinueButtonPressed()
	{
		HandRayController.Instance.DisableHandRays();
		PrivateUIRoom.RemoveUI(LocalisationUI.GetUITransform());
		LocalisationManager.OnSaveLanguage();
	}

	// Token: 0x06004BE8 RID: 19432 RVA: 0x00195BD0 File Offset: 0x00193DD0
	private void ConstructLocalisationUI()
	{
		using (Dictionary<int, Locale>.Enumerator enumerator = LocalisationManager.GetAllBindings().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Locale> item = enumerator.Current;
				KIDUIButton newButton = Object.Instantiate<KIDUIButton>(this._languageButtonPrefab, this._languageButtonGridTransform);
				bool forceEnglishChars = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
				newButton.SetText(LocalisationManager.LocaleToFriendlyString(item.Value, forceEnglishChars).ToUpper());
				newButton.onClick.AddListener(delegate()
				{
					this.OnLanguageButtonPressed(newButton, item.Key);
				});
				this._languageButtons.Add(newButton);
			}
		}
		this._hasConstructedUI = true;
	}

	// Token: 0x06004BE9 RID: 19433 RVA: 0x00195CC4 File Offset: 0x00193EC4
	private void CheckSelectedLanguage()
	{
		KIDUIButton kiduibutton = null;
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool forceEnglishChars = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			if (!(this._languageButtons[i].GetText() != LocalisationManager.LocaleToFriendlyString(LocalisationManager.CurrentLanguage, forceEnglishChars).ToUpper()))
			{
				kiduibutton = this._languageButtons[i];
				break;
			}
		}
		if (kiduibutton == null)
		{
			return;
		}
		if (this._activeButton != null)
		{
			this._activeButton.SetBorderImage(this._inactiveSprite);
		}
		kiduibutton.SetBorderImage(this._activeSprite);
		this._activeButton = kiduibutton;
	}

	// Token: 0x06004BEA RID: 19434 RVA: 0x00195D80 File Offset: 0x00193F80
	private void OnLanguageChanged()
	{
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool forceEnglishChar = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			this._languageButtons[i].SetText(LocalisationManager.LocaleDisplayNameToFriendlyString(this._languageButtons[i].GetText(), forceEnglishChar).ToUpper());
			if (!(LocalisationManager.CurrentLanguage.Identifier.Code == "ja"))
			{
				this._languageButtons[i].SetFont(this._defaultFont);
			}
			else
			{
				this._languageButtons[i].SetFont(this._japaneseFont);
			}
		}
	}

	// Token: 0x06004BEB RID: 19435 RVA: 0x00195E48 File Offset: 0x00194048
	public static Transform GetUITransform()
	{
		if (LocalisationUI.Instance == null)
		{
			return null;
		}
		if (LocalisationUI.Instance._uiTransform == null)
		{
			LocalisationUI.Instance._uiTransform = LocalisationUI.Instance.transform.GetChild(0);
		}
		return LocalisationUI.Instance._uiTransform;
	}

	// Token: 0x04005F1F RID: 24351
	private static LocalisationUI _instance;

	// Token: 0x04005F20 RID: 24352
	[Header("Text Components")]
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x04005F21 RID: 24353
	[SerializeField]
	private TMP_Text _confirmBtnTxt;

	// Token: 0x04005F22 RID: 24354
	[Header("UI Setup")]
	[SerializeField]
	private KIDUIButton _languageButtonPrefab;

	// Token: 0x04005F23 RID: 24355
	[SerializeField]
	private Transform _languageButtonGridTransform;

	// Token: 0x04005F24 RID: 24356
	[SerializeField]
	private Sprite _activeSprite;

	// Token: 0x04005F25 RID: 24357
	[SerializeField]
	private Sprite _inactiveSprite;

	// Token: 0x04005F26 RID: 24358
	[SerializeField]
	private TMP_FontAsset _defaultFont;

	// Token: 0x04005F27 RID: 24359
	[SerializeField]
	private TMP_FontAsset _japaneseFont;

	// Token: 0x04005F28 RID: 24360
	private Transform _uiTransform;

	// Token: 0x04005F29 RID: 24361
	private KIDUIButton _activeButton;

	// Token: 0x04005F2A RID: 24362
	private List<KIDUIButton> _languageButtons = new List<KIDUIButton>();

	// Token: 0x04005F2B RID: 24363
	private bool _hasConstructedUI;
}
