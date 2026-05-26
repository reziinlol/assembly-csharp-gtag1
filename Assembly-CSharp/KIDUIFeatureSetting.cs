using System;
using KID.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000B7E RID: 2942
public class KIDUIFeatureSetting : MonoBehaviour
{
	// Token: 0x170006FD RID: 1789
	// (get) Token: 0x06004A0C RID: 18956 RVA: 0x0018C7F3 File Offset: 0x0018A9F3
	// (set) Token: 0x06004A0D RID: 18957 RVA: 0x0018C7FB File Offset: 0x0018A9FB
	public bool AlwaysCheckFeatureSetting { get; private set; }

	// Token: 0x06004A0E RID: 18958 RVA: 0x0018C804 File Offset: 0x0018AA04
	public void CreateNewFeatureSettingGuardianManaged(KIDUI_MainScreen.FeatureToggleSetup feature, bool isEnabled)
	{
		this.CreateNewFeatureSettingWithoutToggle(feature, false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
	}

	// Token: 0x06004A0F RID: 18959 RVA: 0x0018C829 File Offset: 0x0018AA29
	public KIDUIToggle CreateNewFeatureSettingWithToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool initialState = false, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, true);
		this._featureToggle.SetValue(initialState);
		KIDUIToggle featureToggle = this._featureToggle;
		if (featureToggle != null)
		{
			featureToggle.RegisterOnChangeEvent(new Action(this.SetFeatureName));
		}
		return this._featureToggle;
	}

	// Token: 0x06004A10 RID: 18960 RVA: 0x0018C863 File Offset: 0x0018AA63
	public void CreateNewFeatureSettingWithoutToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, false);
	}

	// Token: 0x06004A11 RID: 18961 RVA: 0x0018C870 File Offset: 0x0018AA70
	private void SetFeatureData(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting, bool featureToggleEnabled)
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.enabledText), this);
		}
		this._enabledTextStr = text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FEATURE_SETTING] Failed to get key for  k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._disabledTextStr = text;
		this._hasToggle = featureToggleEnabled;
		this._featureType = feature.linkedFeature;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(feature.featureName, out text, feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for k-ID Feature [{0}]\n[{1}]", feature.featureName, feature.disabledText), this);
		}
		this._featureName = text;
		this.SetFeatureName();
		GameObject gameObject = base.gameObject;
		string name = gameObject.name;
		string str = "_";
		LocalizedString featureName = feature.featureName;
		gameObject.name = name + str + ((featureName != null) ? featureName.ToString() : null);
		this._permissionName = feature.permissionName;
		this._featureToggle.gameObject.SetActive(featureToggleEnabled);
		this.AlwaysCheckFeatureSetting = alwaysCheckFeatureSetting;
		this._feature = feature;
	}

	// Token: 0x06004A12 RID: 18962 RVA: 0x0018C998 File Offset: 0x0018AB98
	public void RefreshTextOnLanguageChanged()
	{
		string text;
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.enabledText, out text, "ON", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.enabledText));
		}
		this._enabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed enabled text: " + this._enabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.disabledText, out text, "OFF", null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._disabledTextStr = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed disabled text: " + this._disabledTextStr);
		if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._feature.featureName, out text, this._feature.permissionName, null))
		{
			Debug.LogError(string.Format("[LOCALIZATION::FeatureSetting] Failed to get key for Game Mode [{0}]", this._feature.disabledText));
		}
		this._featureName = text;
		Debug.Log("[KIDUIFeatureSetting::Language] Refreshed feature name text: " + this._featureName);
		this.SetFeatureName();
	}

	// Token: 0x06004A13 RID: 18963 RVA: 0x0018CAA1 File Offset: 0x0018ACA1
	public void UnregisterOnToggleChangeEvent(Action action)
	{
		this._featureToggle.UnregisterOnChangeEvent(action);
	}

	// Token: 0x06004A14 RID: 18964 RVA: 0x0018CAAF File Offset: 0x0018ACAF
	public void RegisterToggleOnEvent(Action action)
	{
		this._featureToggle.RegisterToggleOnEvent(action);
	}

	// Token: 0x06004A15 RID: 18965 RVA: 0x0018CABD File Offset: 0x0018ACBD
	public void UnregisterToggleOnEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOnEvent(action);
	}

	// Token: 0x06004A16 RID: 18966 RVA: 0x0018CACB File Offset: 0x0018ACCB
	public void RegisterToggleOffEvent(Action action)
	{
		this._featureToggle.RegisterToggleOffEvent(action);
	}

	// Token: 0x06004A17 RID: 18967 RVA: 0x0018CAD9 File Offset: 0x0018ACD9
	public void UnregisterToggleOffEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOffEvent(action);
	}

	// Token: 0x06004A18 RID: 18968 RVA: 0x0018CAE7 File Offset: 0x0018ACE7
	public bool GetFeatureToggleState()
	{
		if (this._hasToggle)
		{
			return this._featureToggle.IsOn;
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._featureType);
		if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogError("[KID::FeatureSetting] GetToggleState: feature has no toggle AND is not managed by Guardian");
		}
		return permissionDataByFeature.Enabled;
	}

	// Token: 0x06004A19 RID: 18969 RVA: 0x0018CB20 File Offset: 0x0018AD20
	public bool GetHasToggle()
	{
		return this._hasToggle;
	}

	// Token: 0x06004A1A RID: 18970 RVA: 0x0018CB28 File Offset: 0x0018AD28
	public void SetFeatureSettingVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06004A1B RID: 18971 RVA: 0x0018CB36 File Offset: 0x0018AD36
	public void SetFeatureToggle(bool enableToggle)
	{
		this._featureToggle.interactable = enableToggle;
	}

	// Token: 0x06004A1C RID: 18972 RVA: 0x0018CB44 File Offset: 0x0018AD44
	public void SetGuardianManagedState(bool isEnabled)
	{
		this._featureToggle.gameObject.SetActive(false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
		this.SetupGuardianManagedClickHandlers();
		this.SetFeatureName();
	}

	// Token: 0x06004A1D RID: 18973 RVA: 0x0018CB80 File Offset: 0x0018AD80
	public void SetPlayerManagedState(bool isInteractable, bool isOptedIn)
	{
		this._featureToggle.gameObject.SetActive(true);
		this._guardianManagedEnabled.SetActive(false);
		this._guardianManagedLocked.SetActive(false);
		this._featureToggle.interactable = isInteractable;
		this._featureToggle.SetValue(isOptedIn);
	}

	// Token: 0x06004A1E RID: 18974 RVA: 0x0018CBD0 File Offset: 0x0018ADD0
	private void SetFeatureName()
	{
		string text = this.GetFeatureToggleState() ? ("<b>(" + this._enabledTextStr + ")</b>") : ("<b>(" + this._disabledTextStr + ")</b>");
		this._featureNameTxt.text = "<b>" + this._featureName + "</b>";
		this._featureStatusTxt.text = (text ?? "");
	}

	// Token: 0x06004A1F RID: 18975 RVA: 0x0018CC47 File Offset: 0x0018AE47
	private void SetupGuardianManagedClickHandlers()
	{
		this.AddDeniedSoundHandler(this._guardianManagedEnabled);
		this.AddDeniedSoundHandler(this._guardianManagedLocked);
	}

	// Token: 0x06004A20 RID: 18976 RVA: 0x0018CC64 File Offset: 0x0018AE64
	private void AddDeniedSoundHandler(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		EventTrigger component = obj.GetComponent<EventTrigger>();
		if (component != null)
		{
			Object.DestroyImmediate(component);
		}
		EventTrigger eventTrigger = obj.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener(delegate(BaseEventData data)
		{
			Debug.Log("[KIDUIFeatureSetting] Guardian-managed feature clicked - playing denied sound");
			KIDAudioManager instance = KIDAudioManager.Instance;
			if (instance == null)
			{
				return;
			}
			instance.PlaySound(KIDAudioManager.KIDSoundType.Denied);
		});
		eventTrigger.triggers.Add(entry);
		this.EnsureRaycastTarget(obj);
	}

	// Token: 0x06004A21 RID: 18977 RVA: 0x0018CCE0 File Offset: 0x0018AEE0
	private void EnsureRaycastTarget(GameObject obj)
	{
		Graphic component = obj.GetComponent<Graphic>();
		if (component != null)
		{
			component.raycastTarget = true;
			return;
		}
		Image image = obj.GetComponent<Image>();
		if (image == null)
		{
			image = obj.AddComponent<Image>();
		}
		image.color = new Color(0f, 0f, 0f, 0f);
		image.raycastTarget = true;
	}

	// Token: 0x04005CE3 RID: 23779
	[SerializeField]
	private TMP_Text _featureNameTxt;

	// Token: 0x04005CE4 RID: 23780
	[SerializeField]
	private TMP_Text _featureStatusTxt;

	// Token: 0x04005CE5 RID: 23781
	[SerializeField]
	private KIDUIToggle _featureToggle;

	// Token: 0x04005CE6 RID: 23782
	[SerializeField]
	private GameObject _tickIcon;

	// Token: 0x04005CE7 RID: 23783
	[SerializeField]
	private GameObject _crossIcon;

	// Token: 0x04005CE8 RID: 23784
	[SerializeField]
	private GameObject _guardianManagedLocked;

	// Token: 0x04005CE9 RID: 23785
	[SerializeField]
	private GameObject _guardianManagedEnabled;

	// Token: 0x04005CEA RID: 23786
	private bool _hasToggle;

	// Token: 0x04005CEB RID: 23787
	private string _featureName;

	// Token: 0x04005CEC RID: 23788
	private string _permissionName;

	// Token: 0x04005CED RID: 23789
	private string _enabledTextStr;

	// Token: 0x04005CEE RID: 23790
	private string _disabledTextStr;

	// Token: 0x04005CEF RID: 23791
	private EKIDFeatures _featureType;

	// Token: 0x04005CF0 RID: 23792
	private Action<EKIDFeatures> _onChangeCallback;

	// Token: 0x04005CF1 RID: 23793
	private KIDUI_MainScreen.FeatureToggleSetup _feature;
}
