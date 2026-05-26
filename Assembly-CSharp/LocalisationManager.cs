using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000BD9 RID: 3033
public class LocalisationManager : MonoBehaviour
{
	// Token: 0x1700071D RID: 1821
	// (get) Token: 0x06004BB8 RID: 19384 RVA: 0x00194E78 File Offset: 0x00193078
	public static LocalisationManager Instance
	{
		get
		{
			return LocalisationManager._instance;
		}
	}

	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06004BB9 RID: 19385 RVA: 0x00194E7F File Offset: 0x0019307F
	public static bool IsReady
	{
		get
		{
			return LocalisationManager.Instance != null && LocalisationManager._localeTablePairs.Count != 0;
		}
	}

	// Token: 0x1700071F RID: 1823
	// (get) Token: 0x06004BBA RID: 19386 RVA: 0x00194E9D File Offset: 0x0019309D
	public static bool LanguageSet
	{
		get
		{
			return PlayerPrefs.GetInt("has-set-language", 0) == 1;
		}
	}

	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06004BBB RID: 19387 RVA: 0x00194EAD File Offset: 0x001930AD
	public static Locale CurrentLanguage
	{
		get
		{
			return LocalizationSettings.SelectedLocale;
		}
	}

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06004BBC RID: 19388 RVA: 0x00194EB4 File Offset: 0x001930B4
	private static string LanugageSetPlayerPrefKey
	{
		get
		{
			return "selected-locale";
		}
	}

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06004BBD RID: 19389 RVA: 0x00194EBB File Offset: 0x001930BB
	public static bool ApplicationRunning
	{
		get
		{
			return Application.isPlaying && !ApplicationQuittingState.IsQuitting;
		}
	}

	// Token: 0x06004BBE RID: 19390 RVA: 0x00194ED0 File Offset: 0x001930D0
	private void Awake()
	{
		if (LocalisationManager._instance != null)
		{
			Object.DestroyImmediate(this);
			return;
		}
		LocalisationManager._instance = this;
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
		LocalisationManager._localisationFontDict.Clear();
		for (int i = 0; i < this._localisationFonts.Count; i++)
		{
			for (int j = 0; j < this._localisationFonts[i].locales.Count; j++)
			{
				if (!(this._localisationFonts[i].locales[j] == null) && !LocalisationManager._localisationFontDict.ContainsKey(this._localisationFonts[i].locales[j].Identifier.Code) && !(this._localisationFonts[i].fontAsset == null))
				{
					LocalisationManager._localisationFontDict.Add(this._localisationFonts[i].locales[j].Identifier.Code, this._localisationFonts[i]);
					Debug.Log("[LOCALIZATION::MANAGER] Added new Locale-Font pair to Dictionary: [" + this._localisationFonts[i].locales[j].LocaleName + "]");
				}
			}
		}
		LocalisationManager._requestCancellationSource = new CancellationTokenSource();
	}

	// Token: 0x06004BBF RID: 19391 RVA: 0x00195040 File Offset: 0x00193240
	private void Start()
	{
		LocalisationManager.<Start>d__32 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<LocalisationManager.<Start>d__32>(ref <Start>d__);
	}

	// Token: 0x06004BC0 RID: 19392 RVA: 0x00195077 File Offset: 0x00193277
	private void OnDestroy()
	{
		LocalisationManager._requestCancellationSource.Cancel();
		LocalisationManager._onLanguageChanged = null;
	}

	// Token: 0x06004BC1 RID: 19393 RVA: 0x00195089 File Offset: 0x00193289
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void InitialiseLocTables()
	{
		CultureInfo.CurrentCulture = new CultureInfo("en");
		LocalisationManager.CacheLocTables();
	}

	// Token: 0x06004BC2 RID: 19394 RVA: 0x001950A0 File Offset: 0x001932A0
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitialiseLanguage()
	{
		LocalisationManager._hasInitialised = false;
		string @string = PlayerPrefs.GetString(LocalisationManager.LanugageSetPlayerPrefKey, "");
		Locale locale = null;
		if (!string.IsNullOrEmpty(@string) && LocalisationManager.LanguageSet)
		{
			LocalisationManager.LoadPreviousLanguage(@string, out locale);
		}
		else
		{
			LocalisationManager.DefaultLocaleFallback(out locale);
		}
		MothershipClientApiUnity.SetLanguage(locale.Identifier.Code);
		LocalisationManager._initLocale = locale;
		LocalisationManager._hasInitialised = true;
	}

	// Token: 0x06004BC3 RID: 19395 RVA: 0x00195104 File Offset: 0x00193304
	private static void CacheLocTables()
	{
		LocalisationManager._localeTablePairs.Clear();
		float time = Time.time;
		foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
		{
			AsyncOperationHandle<IList<StringTable>> allTables = LocalizationSettings.StringDatabase.GetAllTables(locale);
			allTables.WaitForCompletion();
			IList<StringTable> result = allTables.Result;
			if (result.Count != 0)
			{
				int count = result.Count;
				LocalisationManager._localeTablePairs.Add(locale.Identifier.Code, result[0]);
			}
		}
	}

	// Token: 0x06004BC4 RID: 19396 RVA: 0x001951B4 File Offset: 0x001933B4
	public void OnLanguageButtonPressed(string langCode, bool saveLanguage)
	{
		Locale newLocale;
		if (!LocalisationManager.TryGetLocaleFromCode(langCode, out newLocale))
		{
			return;
		}
		this.TryUpdateLanguage(newLocale, saveLanguage);
	}

	// Token: 0x06004BC5 RID: 19397 RVA: 0x001951D4 File Offset: 0x001933D4
	private void ReconstructBindings()
	{
		int num = 1;
		LocalisationManager._localeDisplayBinding.Clear();
		foreach (Locale value in LocalizationSettings.AvailableLocales.Locales)
		{
			LocalisationManager._localeDisplayBinding.Add(num, value);
			num++;
		}
	}

	// Token: 0x06004BC6 RID: 19398 RVA: 0x00195240 File Offset: 0x00193440
	private static void LoadPreviousLanguage(string languageCode, out Locale result)
	{
		if (!LocalisationManager.TryGetLocaleFromCode(languageCode, out result))
		{
			LocalisationManager.DefaultLocaleFallback(out result);
			return;
		}
		PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, result.Identifier.Code);
		PlayerPrefs.SetInt("has-set-language", 1);
		PlayerPrefs.Save();
	}

	// Token: 0x06004BC7 RID: 19399 RVA: 0x00195288 File Offset: 0x00193488
	private static void DefaultLocaleFallback(out Locale result)
	{
		if (LocalisationManager.SysLangToLoc(Application.systemLanguage, out result))
		{
			PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, result.Identifier.Code);
			PlayerPrefs.SetInt("has-set-language", 1);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06004BC8 RID: 19400 RVA: 0x001952CC File Offset: 0x001934CC
	private static bool SysLangToLoc(SystemLanguage sysLanguage, out Locale language)
	{
		if (sysLanguage <= SystemLanguage.French)
		{
			if (sysLanguage == SystemLanguage.English)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en");
				return language != null;
			}
			if (sysLanguage == SystemLanguage.French)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("fr");
				return language != null;
			}
		}
		else
		{
			if (sysLanguage == SystemLanguage.German)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("de");
				return language != null;
			}
			if (sysLanguage == SystemLanguage.Spanish)
			{
				language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("es");
				return language != null;
			}
		}
		language = LocalizationSettings.Instance.GetAvailableLocales().GetLocale("en");
		language == null;
		return false;
	}

	// Token: 0x06004BC9 RID: 19401 RVA: 0x001953AC File Offset: 0x001935AC
	private void TryUpdateLanguage(Locale newLocale, bool saveLanguage = true)
	{
		if (this._updateLangCoroutine != null)
		{
			base.StopCoroutine(this._updateLangCoroutine);
		}
		this._updateLangCoroutine = base.StartCoroutine(this.UpdateLanguage(newLocale, saveLanguage));
	}

	// Token: 0x06004BCA RID: 19402 RVA: 0x001953D6 File Offset: 0x001935D6
	private IEnumerator UpdateLanguage(Locale newLocale, bool saveLanguage)
	{
		if (!this._cachedHasInitialised)
		{
			yield return LocalizationSettings.InitializationOperation;
		}
		this._cachedHasInitialised = true;
		if (LocalisationManager.CurrentLanguage.Identifier.Code == newLocale.Identifier.Code)
		{
			yield break;
		}
		TelemetryData telemetryData = new TelemetryData
		{
			EventName = "language_changed",
			CustomTags = new string[]
			{
				LocalizationTelemetry.GameVersionCustomTag
			},
			BodyData = new Dictionary<string, string>
			{
				{
					"starting_language",
					LocalisationManager.CurrentLanguage.Identifier.Code
				},
				{
					"new_language",
					newLocale.Identifier.Code
				}
			}
		};
		MothershipClientApiUnity.SetLanguage(newLocale.Identifier.Code);
		GorillaTelemetry.EnqueueTelemetryEvent(telemetryData.EventName, telemetryData.BodyData, telemetryData.CustomTags);
		LocalizationSettings.SelectedLocale = newLocale;
		UnityEvent languageEvent = GameEvents.LanguageEvent;
		if (languageEvent != null)
		{
			languageEvent.Invoke();
		}
		Action onLanguageChanged = LocalisationManager._onLanguageChanged;
		if (onLanguageChanged != null)
		{
			onLanguageChanged();
		}
		if (!saveLanguage)
		{
			yield break;
		}
		LocalisationManager.OnSaveLanguage();
		yield break;
	}

	// Token: 0x06004BCB RID: 19403 RVA: 0x001953F3 File Offset: 0x001935F3
	public static bool TryGetLocaleFromCode(string code, out Locale result)
	{
		result = LocalizationSettings.AvailableLocales.GetLocale(code);
		return result != null;
	}

	// Token: 0x06004BCC RID: 19404 RVA: 0x0019540F File Offset: 0x0019360F
	public static void RegisterOnLanguageChanged(Action callback)
	{
		LocalisationManager._onLanguageChanged = (Action)Delegate.Combine(LocalisationManager._onLanguageChanged, callback);
	}

	// Token: 0x06004BCD RID: 19405 RVA: 0x00195426 File Offset: 0x00193626
	public static void UnregisterOnLanguageChanged(Action callback)
	{
		LocalisationManager._onLanguageChanged = (Action)Delegate.Remove(LocalisationManager._onLanguageChanged, callback);
	}

	// Token: 0x06004BCE RID: 19406 RVA: 0x00195440 File Offset: 0x00193640
	public static bool GetFontAssetForCurrentLocale(out LocalisationFontPair result)
	{
		result = default(LocalisationFontPair);
		if (LocalisationManager.Instance == null)
		{
			bool applicationRunning = LocalisationManager.ApplicationRunning;
			return false;
		}
		if (!LocalisationManager._localisationFontDict.ContainsKey(LocalisationManager.CurrentLanguage.Identifier.Code))
		{
			float time = Time.time;
			return false;
		}
		result = LocalisationManager._localisationFontDict[LocalisationManager.CurrentLanguage.Identifier.Code];
		return true;
	}

	// Token: 0x06004BCF RID: 19407 RVA: 0x001954B8 File Offset: 0x001936B8
	public static void OnSaveLanguage()
	{
		PlayerPrefs.SetString(LocalisationManager.LanugageSetPlayerPrefKey, LocalisationManager.CurrentLanguage.Identifier.Code);
		PlayerPrefs.SetInt("has-set-language", 1);
		PlayerPrefs.Save();
	}

	// Token: 0x06004BD0 RID: 19408 RVA: 0x001954F4 File Offset: 0x001936F4
	public static bool TryGetLocaleBinding(int binding, out Locale loc)
	{
		loc = null;
		if (LocalisationManager.Instance == null)
		{
			return false;
		}
		if (LocalisationManager._localeDisplayBinding.Count != LocalizationSettings.AvailableLocales.Locales.Count)
		{
			LocalisationManager.Instance.ReconstructBindings();
		}
		return LocalisationManager._localeDisplayBinding.TryGetValue(binding, out loc);
	}

	// Token: 0x06004BD1 RID: 19409 RVA: 0x00195544 File Offset: 0x00193744
	public static Dictionary<int, Locale> GetAllBindings()
	{
		if (LocalisationManager._localeDisplayBinding.Count != LocalizationSettings.AvailableLocales.Locales.Count)
		{
			LocalisationManager.Instance.ReconstructBindings();
		}
		return LocalisationManager._localeDisplayBinding;
	}

	// Token: 0x06004BD2 RID: 19410 RVA: 0x00195570 File Offset: 0x00193770
	public static bool TryGetKeyForCurrentLocale(string key, out string result, string defaultResult = "")
	{
		result = defaultResult;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (LocalisationManager._localeTablePairs.Count == 0)
		{
			return false;
		}
		StringTable stringTable;
		if (!LocalisationManager._localeTablePairs.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, out stringTable))
		{
			return false;
		}
		TableEntry entry = stringTable.GetEntry(key);
		if (entry == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(entry.LocalizedValue))
		{
			result = defaultResult;
			return true;
		}
		result = entry.LocalizedValue;
		return true;
	}

	// Token: 0x06004BD3 RID: 19411 RVA: 0x001955E4 File Offset: 0x001937E4
	public static bool TryGetKeyForEnglishString(string englishString, out string result)
	{
		result = "";
		if (LocalisationManager._localeTablePairs.Count == 0)
		{
			return false;
		}
		StringTable stringTable;
		if (!LocalisationManager._localeTablePairs.TryGetValue("en", out stringTable))
		{
			return false;
		}
		foreach (StringTableEntry stringTableEntry in stringTable.Values)
		{
			if (!englishString.Contains(stringTableEntry.LocalizedValue))
			{
				result = stringTableEntry.LocalizedValue;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004BD4 RID: 19412 RVA: 0x00195674 File Offset: 0x00193874
	public static bool TryGetTranslationForCurrentLocaleWithLocString(LocalizedString key, out string result, string defaultResult = "", Object context = null)
	{
		result = defaultResult;
		key.TableReference;
		StringTable table = LocalizationSettings.StringDatabase.GetTable(key.TableReference, null);
		if (table == null)
		{
			return false;
		}
		TableEntry entryFromReference = table.GetEntryFromReference(key.TableEntryReference);
		if (entryFromReference == null)
		{
			return false;
		}
		result = entryFromReference.LocalizedValue;
		return true;
	}

	// Token: 0x06004BD5 RID: 19413 RVA: 0x001956C8 File Offset: 0x001938C8
	public static string LocaleToFriendlyString(Locale locale = null, bool forceEnglishChars = false)
	{
		if (locale == null)
		{
			locale = LocalisationManager.CurrentLanguage;
		}
		string code = locale.Identifier.Code;
		if (code == "en")
		{
			return "English";
		}
		if (code == "fr")
		{
			return "Français";
		}
		if (code == "de")
		{
			return "Deutsch";
		}
		if (code == "es")
		{
			return "Español";
		}
		if (!(code == "ja"))
		{
			return "English";
		}
		if (forceEnglishChars)
		{
			return "Nihongo";
		}
		return "日本語";
	}

	// Token: 0x06004BD6 RID: 19414 RVA: 0x00195764 File Offset: 0x00193964
	public static string LocaleDisplayNameToFriendlyString(string locTextName, bool forceEnglishChar = false)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(locTextName);
		if (num > 2645429922U)
		{
			if (num <= 3560075306U)
			{
				if (num != 3159852254U)
				{
					if (num != 3560075306U)
					{
						goto IL_103;
					}
					if (!(locTextName == "JAPANESE"))
					{
						goto IL_103;
					}
					goto IL_121;
				}
				else if (!(locTextName == "ESPAÑOL"))
				{
					goto IL_103;
				}
			}
			else if (num != 3567715190U)
			{
				if (num != 3825731007U)
				{
					if (num != 4169853379U)
					{
						goto IL_103;
					}
					if (!(locTextName == "ESPANOL"))
					{
						goto IL_103;
					}
				}
				else
				{
					if (!(locTextName == "NIHONGO"))
					{
						goto IL_103;
					}
					goto IL_121;
				}
			}
			else
			{
				if (!(locTextName == "FRANÇAIS"))
				{
					goto IL_103;
				}
				goto IL_10F;
			}
			return "Español";
		}
		if (num <= 1409693518U)
		{
			if (num != 1157811451U)
			{
				if (num == 1409693518U)
				{
					if (locTextName == "日本語")
					{
						goto IL_121;
					}
				}
			}
			else if (locTextName == "ENGLISH")
			{
				return "English";
			}
		}
		else if (num != 2572742563U)
		{
			if (num == 2645429922U)
			{
				if (locTextName == "FRANCAIS")
				{
					goto IL_10F;
				}
			}
		}
		else if (locTextName == "DEUTSCH")
		{
			return "Deutsch";
		}
		IL_103:
		return "English";
		IL_10F:
		return "Français";
		IL_121:
		if (forceEnglishChar)
		{
			return "Nihongo";
		}
		return "日本語";
	}

	// Token: 0x04005F04 RID: 24324
	public const string ENGLISH_IDENTIFIER = "en";

	// Token: 0x04005F05 RID: 24325
	public const string FRENCH_IDENTIFIER = "fr";

	// Token: 0x04005F06 RID: 24326
	public const string GERMAN_IDENTIFIER = "de";

	// Token: 0x04005F07 RID: 24327
	public const string ITALIAN_IDENTIFIER = "it";

	// Token: 0x04005F08 RID: 24328
	public const string SPANISH_IDENTIFIER = "es";

	// Token: 0x04005F09 RID: 24329
	public const string JAPENESE_IDENTIFIER = "ja";

	// Token: 0x04005F0A RID: 24330
	private static LocalisationManager _instance;

	// Token: 0x04005F0B RID: 24331
	[SerializeField]
	private List<LocalisationFontPair> _localisationFonts = new List<LocalisationFontPair>();

	// Token: 0x04005F0C RID: 24332
	private bool _cachedHasInitialised;

	// Token: 0x04005F0D RID: 24333
	private static bool _hasInitialised = false;

	// Token: 0x04005F0E RID: 24334
	private const string LANGUAGE_SET_PLAYER_PREF = "has-set-language";

	// Token: 0x04005F0F RID: 24335
	private const string LOC_SYSTEM_PLAYER_PREF = "selected-locale";

	// Token: 0x04005F10 RID: 24336
	private static Locale _initLocale;

	// Token: 0x04005F11 RID: 24337
	private static Action _onLanguageChanged;

	// Token: 0x04005F12 RID: 24338
	private Coroutine _updateLangCoroutine;

	// Token: 0x04005F13 RID: 24339
	private static CancellationTokenSource _requestCancellationSource;

	// Token: 0x04005F14 RID: 24340
	private static Dictionary<int, Locale> _localeDisplayBinding = new Dictionary<int, Locale>();

	// Token: 0x04005F15 RID: 24341
	private static Dictionary<string, StringTable> _localeTablePairs = new Dictionary<string, StringTable>();

	// Token: 0x04005F16 RID: 24342
	private static Dictionary<string, LocalisationFontPair> _localisationFontDict = new Dictionary<string, LocalisationFontPair>();
}
