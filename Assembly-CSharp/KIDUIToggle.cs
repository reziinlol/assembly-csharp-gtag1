using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000B84 RID: 2948
public class KIDUIToggle : Slider
{
	// Token: 0x17000700 RID: 1792
	// (get) Token: 0x06004A3C RID: 19004 RVA: 0x0018D291 File Offset: 0x0018B491
	// (set) Token: 0x06004A3D RID: 19005 RVA: 0x0018D299 File Offset: 0x0018B499
	public bool CurrentValue { get; private set; }

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x06004A3E RID: 19006 RVA: 0x0018D2A2 File Offset: 0x0018B4A2
	public bool IsOn
	{
		get
		{
			return this.CurrentValue;
		}
	}

	// Token: 0x06004A3F RID: 19007 RVA: 0x0018D2AA File Offset: 0x0018B4AA
	protected override void Awake()
	{
		base.Awake();
		this.SetupToggleComponent();
	}

	// Token: 0x06004A40 RID: 19008 RVA: 0x0018D2B8 File Offset: 0x0018B4B8
	protected override void Start()
	{
		base.Start();
		base.interactable = false;
	}

	// Token: 0x06004A41 RID: 19009 RVA: 0x0018D2C7 File Offset: 0x0018B4C7
	protected override void OnEnable()
	{
		base.OnEnable();
		base.interactable = false;
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004A42 RID: 19010 RVA: 0x0018D2F8 File Offset: 0x0018B4F8
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.Toggle();
	}

	// Token: 0x06004A43 RID: 19011 RVA: 0x0018D307 File Offset: 0x0018B507
	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.SetHighlighted();
		this.inside = true;
	}

	// Token: 0x06004A44 RID: 19012 RVA: 0x0018D316 File Offset: 0x0018B516
	public override void OnPointerExit(PointerEventData pointerEventData)
	{
		this.SetNormal();
		this.inside = false;
	}

	// Token: 0x06004A45 RID: 19013 RVA: 0x0018D328 File Offset: 0x0018B528
	protected virtual void SetupToggleComponent()
	{
		this.SetupSliderComponent();
		base.handleRect.anchorMin = new Vector2(0f, 0.5f);
		base.handleRect.anchorMax = new Vector3(0f, 0.5f);
		base.handleRect.pivot = new Vector2(0f, 0.5f);
		base.handleRect.sizeDelta = new Vector2(base.handleRect.sizeDelta.x, base.handleRect.sizeDelta.x);
	}

	// Token: 0x06004A46 RID: 19014 RVA: 0x0018D3C0 File Offset: 0x0018B5C0
	protected virtual void SetupSliderComponent()
	{
		base.interactable = false;
		base.colors.disabledColor = Color.white;
		this.SetColors();
		base.transition = Selectable.Transition.None;
	}

	// Token: 0x06004A47 RID: 19015 RVA: 0x0018D3F4 File Offset: 0x0018B5F4
	public void RegisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.AddListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x06004A48 RID: 19016 RVA: 0x0018D428 File Offset: 0x0018B628
	public void UnregisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.RemoveListener(delegate()
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x06004A49 RID: 19017 RVA: 0x0018D45C File Offset: 0x0018B65C
	public void RegisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004A4A RID: 19018 RVA: 0x0018D490 File Offset: 0x0018B690
	public void UnregisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004A4B RID: 19019 RVA: 0x0018D4C4 File Offset: 0x0018B6C4
	public void RegisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.AddListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004A4C RID: 19020 RVA: 0x0018D4F8 File Offset: 0x0018B6F8
	public void UnregisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.RemoveListener(delegate()
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x06004A4D RID: 19021 RVA: 0x0018D529 File Offset: 0x0018B729
	private void SetColors()
	{
		base.colors = this._fillColors;
	}

	// Token: 0x06004A4E RID: 19022 RVA: 0x0018D537 File Offset: 0x0018B737
	private void Toggle()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetStateAndStartAnimation(!this.CurrentValue, false);
	}

	// Token: 0x06004A4F RID: 19023 RVA: 0x0018D552 File Offset: 0x0018B752
	public void SetValue(bool newValue)
	{
		if (newValue == this.CurrentValue)
		{
			return;
		}
		this.SetStateAndStartAnimation(newValue, false);
	}

	// Token: 0x06004A50 RID: 19024 RVA: 0x0018D568 File Offset: 0x0018B768
	private void SetStateAndStartAnimation(bool state, bool skipAnim = false)
	{
		if (this.CurrentValue == state)
		{
			Debug.Log("IS SAME STATE, WILL NOT CHANGE");
			return;
		}
		this.CurrentValue = state;
		UnityEvent onToggleChanged = this._onToggleChanged;
		if (onToggleChanged != null)
		{
			onToggleChanged.Invoke();
		}
		if (this.CurrentValue)
		{
			UnityEvent onToggleOn = this._onToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.Success);
		}
		else
		{
			UnityEvent onToggleOff = this._onToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
			KIDAudioManager.Instance.PlaySound(KIDAudioManager.KIDSoundType.TurnOffPermission);
		}
		if (this._animationCoroutine != null)
		{
			base.StopCoroutine(this._animationCoroutine);
		}
		this._handleUnlockIcon.gameObject.SetActive(this.CurrentValue);
		this._handleLockIcon.gameObject.SetActive(!this.CurrentValue);
		if (this._animationDuration == 0f || skipAnim)
		{
			Debug.Log("[KID::UI::SetStateAndStartAnimation] Skipping animation. Setting value to " + (this.CurrentValue ? "1f" : "0f"));
			this.value = (this.CurrentValue ? 1f : 0f);
			return;
		}
		this._animationCoroutine = base.StartCoroutine(this.AnimateSlider());
	}

	// Token: 0x06004A51 RID: 19025 RVA: 0x0018D687 File Offset: 0x0018B887
	private IEnumerator AnimateSlider()
	{
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] is {1}", base.name, this.CurrentValue));
		float startValue = this.CurrentValue ? 0f : 1f;
		float endValue = this.CurrentValue ? 1f : 0f;
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] Start: {1}, End: {2}, Value: {3}", new object[]
		{
			base.name,
			startValue,
			endValue,
			this.value
		}));
		float time = 0f;
		while (time < this._animationDuration)
		{
			time += Time.deltaTime;
			float t = this._toggleEase.Evaluate(time / this._animationDuration);
			this.value = Mathf.Lerp(startValue, endValue, t);
			yield return null;
		}
		this.value = endValue;
		yield break;
	}

	// Token: 0x06004A52 RID: 19026 RVA: 0x0018D698 File Offset: 0x0018B898
	private void PostUpdate()
	{
		if (!this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance)
		{
			if (ControllerBehaviour.Instance.TriggerDown && KIDUIToggle._canTrigger)
			{
				string text = string.Concat(new string[]
				{
					"[",
					base.transform.parent.parent.parent.name,
					".",
					base.transform.parent.parent.name,
					".",
					base.transform.parent.name,
					".",
					base.transform.name,
					"]"
				});
				Debug.Log(string.Concat(new string[]
				{
					"[KID::UIBUTTON::DEBUG] ",
					text,
					" - STEAM - OnClick is pressed. Time: [",
					Time.time.ToString(),
					"]"
				}), this);
				this.Toggle();
				KIDUIToggle._triggeredThisFrame = true;
				KIDUIToggle._canTrigger = false;
				return;
			}
			if (!ControllerBehaviour.Instance.TriggerDown)
			{
				KIDUIToggle._canTrigger = true;
			}
		}
	}

	// Token: 0x06004A53 RID: 19027 RVA: 0x0018D7C4 File Offset: 0x0018B9C4
	private void LateUpdate()
	{
		if (KIDUIToggle._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIToggle._triggeredThisFrame = false;
	}

	// Token: 0x06004A54 RID: 19028 RVA: 0x0018D8A9 File Offset: 0x0018BAA9
	protected new void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x06004A55 RID: 19029 RVA: 0x0018D8D4 File Offset: 0x0018BAD4
	private void SetDisabled(bool isLockedButEnabled)
	{
		this.SetSwitchColors(this._borderColors.disabledColor, this._handleColors.disabledColor, this._fillColors.disabledColor);
		this.SetBorderSize(this._disabledBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06004A56 RID: 19030 RVA: 0x0018D910 File Offset: 0x0018BB10
	private void SetNormal()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.normalColor, this._handleColors.normalColor, this._fillColors.normalColor);
		this.SetBorderSize(this._normalBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06004A57 RID: 19031 RVA: 0x0018D960 File Offset: 0x0018BB60
	private void SetSelected()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.selectedColor, this._handleColors.selectedColor, this._fillColors.selectedColor);
		this.SetBorderSize(this._selectedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004A58 RID: 19032 RVA: 0x0018D9B0 File Offset: 0x0018BBB0
	private void SetHighlighted()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.highlightedColor, this._handleColors.highlightedColor, this._fillColors.highlightedColor);
		this.SetBorderSize(this._highlightedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004A59 RID: 19033 RVA: 0x0018DA00 File Offset: 0x0018BC00
	private void SetPressed()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.pressedColor, this._handleColors.pressedColor, this._fillColors.pressedColor);
		this.SetBorderSize(this._pressedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06004A5A RID: 19034 RVA: 0x0018DA50 File Offset: 0x0018BC50
	private void SetSwitchColors(Color borderColor, Color handleColor, Color fillColor)
	{
		this._borderImg.color = borderColor;
		this._handleImg.color = handleColor;
	}

	// Token: 0x06004A5B RID: 19035 RVA: 0x0018DA6A File Offset: 0x0018BC6A
	private void SetBorderSize(float borderScale)
	{
		this._borderImgRef.offsetMin = new Vector2(-borderScale, -borderScale * this._borderHeightRatio);
		this._borderImgRef.offsetMax = new Vector2(borderScale, borderScale * this._borderHeightRatio);
	}

	// Token: 0x06004A5C RID: 19036 RVA: 0x0018DAA0 File Offset: 0x0018BCA0
	private void SetBackgroundActive(bool isActive)
	{
		this._fillImg.gameObject.SetActive(isActive);
		this._fillInactiveImg.gameObject.SetActive(!isActive);
		this.SetBackgroundLocksActive(isActive);
	}

	// Token: 0x06004A5D RID: 19037 RVA: 0x0018DAD0 File Offset: 0x0018BCD0
	private void SetBackgroundLocksActive(bool isActive)
	{
		Color color = isActive ? this._lockActiveColor : this._lockInactiveColor;
		this._lockIcon.color = color;
		this._unlockIcon.color = color;
	}

	// Token: 0x04005D02 RID: 23810
	[Header("Toggle Setup")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _initValue;

	// Token: 0x04005D03 RID: 23811
	[SerializeField]
	private Image _borderImg;

	// Token: 0x04005D04 RID: 23812
	[SerializeField]
	private float _borderHeightRatio = 2f;

	// Token: 0x04005D05 RID: 23813
	[SerializeField]
	private Image _fillImg;

	// Token: 0x04005D06 RID: 23814
	[SerializeField]
	private Image _fillInactiveImg;

	// Token: 0x04005D07 RID: 23815
	[SerializeField]
	private Image _handleImg;

	// Token: 0x04005D08 RID: 23816
	[SerializeField]
	private Image _lockIcon;

	// Token: 0x04005D09 RID: 23817
	[SerializeField]
	private Image _unlockIcon;

	// Token: 0x04005D0A RID: 23818
	[SerializeField]
	private Image _handleLockIcon;

	// Token: 0x04005D0B RID: 23819
	[SerializeField]
	private Image _handleUnlockIcon;

	// Token: 0x04005D0C RID: 23820
	[SerializeField]
	private Color _lockActiveColor;

	// Token: 0x04005D0D RID: 23821
	[SerializeField]
	private Color _lockInactiveColor;

	// Token: 0x04005D0E RID: 23822
	[SerializeField]
	private RectTransform _borderImgRef;

	// Token: 0x04005D0F RID: 23823
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005D10 RID: 23824
	[Header("Animation")]
	[SerializeField]
	private float _animationDuration = 0.15f;

	// Token: 0x04005D11 RID: 23825
	[SerializeField]
	private AnimationCurve _toggleEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005D12 RID: 23826
	[Header("Fill Colors")]
	[SerializeField]
	private ColorBlock _fillColors;

	// Token: 0x04005D13 RID: 23827
	[Header("Border Colors")]
	[SerializeField]
	private ColorBlock _borderColors;

	// Token: 0x04005D14 RID: 23828
	[Header("Borders")]
	[SerializeField]
	private float _normalBorderSize = 1f;

	// Token: 0x04005D15 RID: 23829
	[SerializeField]
	private float _disabledBorderSize = 1f;

	// Token: 0x04005D16 RID: 23830
	[SerializeField]
	private float _highlightedBorderSize = 1f;

	// Token: 0x04005D17 RID: 23831
	[SerializeField]
	private float _pressedBorderSize = 1f;

	// Token: 0x04005D18 RID: 23832
	[SerializeField]
	private float _selectedBorderSize = 1f;

	// Token: 0x04005D19 RID: 23833
	[Header("Handle Colors")]
	[SerializeField]
	private ColorBlock _handleColors;

	// Token: 0x04005D1A RID: 23834
	[Header("Events")]
	[SerializeField]
	private UnityEvent _onToggleOn;

	// Token: 0x04005D1B RID: 23835
	[SerializeField]
	private UnityEvent _onToggleOff;

	// Token: 0x04005D1C RID: 23836
	[SerializeField]
	private UnityEvent _onToggleChanged;

	// Token: 0x04005D1D RID: 23837
	private bool _previousValue;

	// Token: 0x04005D1E RID: 23838
	private bool _isDisabled;

	// Token: 0x04005D1F RID: 23839
	private Coroutine _animationCoroutine;

	// Token: 0x04005D21 RID: 23841
	private bool inside;

	// Token: 0x04005D22 RID: 23842
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005D23 RID: 23843
	private static bool _canTrigger = true;
}
