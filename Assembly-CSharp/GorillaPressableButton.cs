using System;
using GorillaExtensions;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x02000A12 RID: 2578
public class GorillaPressableButton : MonoBehaviour, IClickable
{
	// Token: 0x14000084 RID: 132
	// (add) Token: 0x060041E0 RID: 16864 RVA: 0x001605E0 File Offset: 0x0015E7E0
	// (remove) Token: 0x060041E1 RID: 16865 RVA: 0x00160618 File Offset: 0x0015E818
	public event Action<GorillaPressableButton, bool> onPressed;

	// Token: 0x060041E2 RID: 16866 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void Start()
	{
	}

	// Token: 0x060041E3 RID: 16867 RVA: 0x00160650 File Offset: 0x0015E850
	protected virtual void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.RefreshText));
		if (this.isSubscriberOnlyButton)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
			this.CheckSubscription();
		}
		this.RefreshText();
	}

	// Token: 0x060041E4 RID: 16868 RVA: 0x001606A3 File Offset: 0x0015E8A3
	protected virtual void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RefreshText));
		if (this.isSubscriberOnlyButton)
		{
			SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscription));
		}
	}

	// Token: 0x060041E5 RID: 16869 RVA: 0x001606E0 File Offset: 0x0015E8E0
	private void CheckSubscription()
	{
		bool flag = SubscriptionManager.IsLocalSubscribed();
		if (!this._subscriptionChecked || flag != this._localPlayerSubscribed)
		{
			this.UpdateSubscriptionState(flag);
		}
	}

	// Token: 0x060041E6 RID: 16870 RVA: 0x0016070B File Offset: 0x0015E90B
	private void UpdateSubscriptionState(bool subscribed)
	{
		this._localPlayerSubscribed = subscribed;
		this.UpdateColor();
		this._subscriptionChecked = true;
	}

	// Token: 0x060041E7 RID: 16871 RVA: 0x00160724 File Offset: 0x0015E924
	protected virtual void RefreshText()
	{
		if (this._offLocalizedText == null || this._offLocalizedText.IsEmpty || this._onLocalizedText == null || this._onLocalizedText.IsEmpty)
		{
			return;
		}
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString;
		if (!this.isOn)
		{
			localizedString = this.offText;
			localizedString = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				localizedString = this.offText;
			}
		}
		else
		{
			localizedString = this.onText;
			localizedString = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				localizedString = this.onText;
			}
		}
		if (this._myTxtSet || this.myText.IsNotNull())
		{
			this.myText.text = localizedString;
		}
		if (this._myTmpTxtSet || this.myTmpText.IsNotNull())
		{
			this.myTmpText.text = localizedString;
		}
		if (this._myTmpTxt2Set || this.myTmpText2.IsNotNull())
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x060041E8 RID: 16872 RVA: 0x00160834 File Offset: 0x0015EA34
	protected virtual void SetOffText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.offText;
		if (this._offLocalizedText != null && !this._offLocalizedText.IsEmpty)
		{
			localizedString = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				localizedString = this.offText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x060041E9 RID: 16873 RVA: 0x001608D0 File Offset: 0x0015EAD0
	protected virtual void SetOnText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.onText;
		if (this._onLocalizedText != null && !this._onLocalizedText.IsEmpty)
		{
			localizedString = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				localizedString = this.onText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x060041EA RID: 16874 RVA: 0x0016096C File Offset: 0x0015EB6C
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x060041EB RID: 16875 RVA: 0x001609B8 File Offset: 0x0015EBB8
	private void PressButton(bool isLeftHand)
	{
		if (this.isSubscriberOnlyButton && !this._localPlayerSubscribed)
		{
			return;
		}
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		Action<GorillaPressableButton, bool> action = this.onPressed;
		if (action != null)
		{
			action(this, isLeftHand);
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[]
			{
				67,
				isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x060041EC RID: 16876 RVA: 0x00160AAB File Offset: 0x0015ECAB
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x060041ED RID: 16877 RVA: 0x00160AB4 File Offset: 0x0015ECB4
	public virtual void UpdateColor()
	{
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x060041EE RID: 16878 RVA: 0x00160AC4 File Offset: 0x0015ECC4
	protected void UpdateColorWithState(bool state)
	{
		if (this.isSubscriberOnlyButton && !this._localPlayerSubscribed)
		{
			this.SetUnsubscribedMaterial();
			this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		if (state)
		{
			this.SetPressedMaterial();
			this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		this.SetUnpressedMaterial();
		this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
	}

	// Token: 0x060041EF RID: 16879 RVA: 0x00160B6D File Offset: 0x0015ED6D
	public void SetRendererMaterial(Material mat)
	{
		if (this.buttonRenderer)
		{
			this.buttonRenderer.material = mat;
		}
	}

	// Token: 0x060041F0 RID: 16880 RVA: 0x00160B88 File Offset: 0x0015ED88
	public void SetPressedMaterial()
	{
		this.SetRendererMaterial(this.pressedMaterial);
	}

	// Token: 0x060041F1 RID: 16881 RVA: 0x00160B96 File Offset: 0x0015ED96
	public void SetUnpressedMaterial()
	{
		this.SetRendererMaterial(this.unpressedMaterial);
	}

	// Token: 0x060041F2 RID: 16882 RVA: 0x00160BA4 File Offset: 0x0015EDA4
	public void SetUnsubscribedMaterial()
	{
		this.SetRendererMaterial(this.nonSubscriberMaterial ? this.nonSubscriberMaterial : this.unpressedMaterial);
	}

	// Token: 0x060041F3 RID: 16883 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x060041F4 RID: 16884 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x060041F5 RID: 16885 RVA: 0x00160BC7 File Offset: 0x0015EDC7
	public virtual void ResetState()
	{
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x060041F6 RID: 16886 RVA: 0x00160BD8 File Offset: 0x0015EDD8
	public void SetText(string newText)
	{
		if (this.myTmpText != null)
		{
			this.myTmpText.text = newText;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.text = newText;
		}
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
	}

	// Token: 0x040053BA RID: 21434
	public Material pressedMaterial;

	// Token: 0x040053BB RID: 21435
	public Material unpressedMaterial;

	// Token: 0x040053BC RID: 21436
	public MeshRenderer buttonRenderer;

	// Token: 0x040053BD RID: 21437
	public int pressButtonSoundIndex = 67;

	// Token: 0x040053BE RID: 21438
	public bool isOn;

	// Token: 0x040053BF RID: 21439
	public float debounceTime = 0.25f;

	// Token: 0x040053C0 RID: 21440
	public float touchTime;

	// Token: 0x040053C1 RID: 21441
	public bool testPress;

	// Token: 0x040053C2 RID: 21442
	public bool testHandLeft;

	// Token: 0x040053C3 RID: 21443
	[SerializeField]
	private bool _useOnOffText = true;

	// Token: 0x040053C4 RID: 21444
	[TextArea]
	public string offText;

	// Token: 0x040053C5 RID: 21445
	[SerializeField]
	private LocalizedString _offLocalizedText;

	// Token: 0x040053C6 RID: 21446
	[TextArea]
	public string onText;

	// Token: 0x040053C7 RID: 21447
	[SerializeField]
	private LocalizedString _onLocalizedText;

	// Token: 0x040053C8 RID: 21448
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText;

	// Token: 0x040053C9 RID: 21449
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText2;

	// Token: 0x040053CA RID: 21450
	public Text myText;

	// Token: 0x040053CB RID: 21451
	public bool isSubscriberOnlyButton;

	// Token: 0x040053CC RID: 21452
	public Material nonSubscriberMaterial;

	// Token: 0x040053CD RID: 21453
	private bool _localPlayerSubscribed;

	// Token: 0x040053CE RID: 21454
	private bool _subscriptionChecked;

	// Token: 0x040053CF RID: 21455
	[Space]
	public UnityEvent onPressButton;

	// Token: 0x040053D0 RID: 21456
	protected bool _myTxtSet;

	// Token: 0x040053D1 RID: 21457
	protected bool _myTmpTxtSet;

	// Token: 0x040053D2 RID: 21458
	protected bool _myTmpTxt2Set;
}
