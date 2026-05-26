using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x02000B7D RID: 2941
public class KIDUIButton : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x170006FC RID: 1788
	// (get) Token: 0x060049FA RID: 18938 RVA: 0x00087432 File Offset: 0x00085632
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x060049FB RID: 18939 RVA: 0x0018C2A6 File Offset: 0x0018A4A6
	protected override void OnEnable()
	{
		base.OnEnable();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x060049FC RID: 18940 RVA: 0x0018C2D0 File Offset: 0x0018A4D0
	private void PostUpdate()
	{
		if (!KIDUIButton._canTrigger)
		{
			KIDUIButton._canTrigger = !ControllerBehaviour.Instance.TriggerDown;
		}
		if (!base.interactable || !this.inside || !KIDUIButton._canTrigger)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown && !KIDUIButton._triggeredThisFrame)
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
			Button.ButtonClickedEvent onClick = base.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
			KIDUIButton._triggeredThisFrame = true;
			KIDUIButton._canTrigger = false;
		}
	}

	// Token: 0x060049FD RID: 18941 RVA: 0x0018C41C File Offset: 0x0018A61C
	private void LateUpdate()
	{
		if (KIDUIButton._triggeredThisFrame)
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
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x060049FE RID: 18942 RVA: 0x0018C501 File Offset: 0x0018A701
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.inside = false;
	}

	// Token: 0x060049FF RID: 18943 RVA: 0x0018C511 File Offset: 0x0018A711
	public void ResetButton()
	{
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06004A00 RID: 18944 RVA: 0x0018C520 File Offset: 0x0018A720
	protected override void OnDisable()
	{
		this.FixStuckPressedState();
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004A01 RID: 18945 RVA: 0x0018C54A File Offset: 0x0018A74A
	private void FixStuckPressedState()
	{
		this.InstantClearState();
		this._buttonText.color = (base.interactable ? this._normalTextColor : this._disabledTextColor);
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06004A02 RID: 18946 RVA: 0x0018C580 File Offset: 0x0018A780
	protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		switch (state)
		{
		default:
			this._buttonText.color = this._normalTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Highlighted:
			this._buttonText.color = this._highlightedTextColor;
			this.SetIcons(false, true);
			return;
		case Selectable.SelectionState.Pressed:
			this._buttonText.color = this._pressedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Selected:
			this._buttonText.color = this._selectedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Disabled:
			this._buttonText.color = this._disabledTextColor;
			this.SetIcons(true, false);
			return;
		}
	}

	// Token: 0x06004A03 RID: 18947 RVA: 0x0018C630 File Offset: 0x0018A830
	private void SetIcons(bool normalEnabled, bool highlightedEnabled)
	{
		if (this._normalIcon == null || this._highlightedIcon == null)
		{
			return;
		}
		GameObject normalIcon = this._normalIcon;
		if (normalIcon != null)
		{
			normalIcon.SetActive(normalEnabled);
		}
		GameObject highlightedIcon = this._highlightedIcon;
		if (highlightedIcon == null)
		{
			return;
		}
		highlightedIcon.SetActive(highlightedEnabled);
	}

	// Token: 0x06004A04 RID: 18948 RVA: 0x0018C680 File Offset: 0x0018A880
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.inside = true;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(KIDAudioManager.KIDSoundType.Hover);
		}
		Debug.Log("[KID::UIBUTTON::KIDAudioManager] Hover played");
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06004A05 RID: 18949 RVA: 0x0018C700 File Offset: 0x0018A900
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.inside = false;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySound(this.onClickSound);
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._pressedVibrationStrength, this._pressedVibrationDuration);
	}

	// Token: 0x06004A06 RID: 18950 RVA: 0x0018C77A File Offset: 0x0018A97A
	public void SetText(string text)
	{
		this._buttonText.SetText(text);
	}

	// Token: 0x06004A07 RID: 18951 RVA: 0x0018C788 File Offset: 0x0018A988
	public void SetFont(TMP_FontAsset font)
	{
		this._buttonText.font = font;
	}

	// Token: 0x06004A08 RID: 18952 RVA: 0x0018C796 File Offset: 0x0018A996
	public string GetText()
	{
		return this._buttonText.text;
	}

	// Token: 0x06004A09 RID: 18953 RVA: 0x0018C7A3 File Offset: 0x0018A9A3
	public void SetBorderImage(Sprite newImg)
	{
		this._borderImage.sprite = newImg;
	}

	// Token: 0x04005CC6 RID: 23750
	[SerializeField]
	private Image _borderImage;

	// Token: 0x04005CC7 RID: 23751
	[SerializeField]
	private RectTransform _fillImageRef;

	// Token: 0x04005CC8 RID: 23752
	[SerializeField]
	private TMP_Text _buttonText;

	// Token: 0x04005CC9 RID: 23753
	[Header("Transition States")]
	[Header("Normal")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalBorderColor;

	// Token: 0x04005CCA RID: 23754
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalTextColor;

	// Token: 0x04005CCB RID: 23755
	[SerializeField]
	private float _normalBorderSize;

	// Token: 0x04005CCC RID: 23756
	[Header("Highlighted")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedBorderColor;

	// Token: 0x04005CCD RID: 23757
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedTextColor;

	// Token: 0x04005CCE RID: 23758
	[SerializeField]
	private float _highlightedBorderSize;

	// Token: 0x04005CCF RID: 23759
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04005CD0 RID: 23760
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04005CD1 RID: 23761
	[Header("Pressed")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedBorderColor;

	// Token: 0x04005CD2 RID: 23762
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedTextColor;

	// Token: 0x04005CD3 RID: 23763
	[SerializeField]
	private float _pressedBorderSize;

	// Token: 0x04005CD4 RID: 23764
	[SerializeField]
	private float _pressedVibrationStrength = 0.5f;

	// Token: 0x04005CD5 RID: 23765
	[SerializeField]
	private float _pressedVibrationDuration = 0.1f;

	// Token: 0x04005CD6 RID: 23766
	[Header("Selected")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedBorderColor;

	// Token: 0x04005CD7 RID: 23767
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedTextColor;

	// Token: 0x04005CD8 RID: 23768
	[SerializeField]
	private float _selectedBorderSize;

	// Token: 0x04005CD9 RID: 23769
	[Header("Disabled")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledBorderColor;

	// Token: 0x04005CDA RID: 23770
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledTextColor;

	// Token: 0x04005CDB RID: 23771
	[SerializeField]
	private float _disabledBorderSize;

	// Token: 0x04005CDC RID: 23772
	[Header("Audio")]
	[SerializeField]
	private KIDAudioManager.KIDSoundType onClickSound;

	// Token: 0x04005CDD RID: 23773
	[Header("Icon Swap Settings")]
	[SerializeField]
	private GameObject _normalIcon;

	// Token: 0x04005CDE RID: 23774
	[SerializeField]
	private GameObject _highlightedIcon;

	// Token: 0x04005CDF RID: 23775
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005CE0 RID: 23776
	private bool inside;

	// Token: 0x04005CE1 RID: 23777
	private static bool _triggeredThisFrame = false;

	// Token: 0x04005CE2 RID: 23778
	private static bool _canTrigger = true;
}
