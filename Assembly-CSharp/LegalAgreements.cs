using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000BC7 RID: 3015
[DefaultExecutionOrder(1)]
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x1700071A RID: 1818
	// (get) Token: 0x06004B7E RID: 19326 RVA: 0x00193892 File Offset: 0x00191A92
	// (set) Token: 0x06004B7F RID: 19327 RVA: 0x00193899 File Offset: 0x00191A99
	public static LegalAgreements instance { get; private set; }

	// Token: 0x06004B80 RID: 19328 RVA: 0x001938A4 File Offset: 0x00191AA4
	protected virtual void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	// Token: 0x06004B81 RID: 19329 RVA: 0x001938FC File Offset: 0x00191AFC
	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (ControllerBehaviour.Instance.IsUpStick || ControllerBehaviour.Instance.IsDownStick)
		{
			if (ControllerBehaviour.Instance.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(ControllerBehaviour.Instance.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004B82 RID: 19330 RVA: 0x00193AB0 File Offset: 0x00191CB0
	public virtual Task StartLegalAgreements()
	{
		LegalAgreements.<StartLegalAgreements>d__24 <StartLegalAgreements>d__;
		<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartLegalAgreements>d__.<>4__this = this;
		<StartLegalAgreements>d__.<>1__state = -1;
		<StartLegalAgreements>d__.<>t__builder.Start<LegalAgreements.<StartLegalAgreements>d__24>(ref <StartLegalAgreements>d__);
		return <StartLegalAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06004B83 RID: 19331 RVA: 0x00193AF3 File Offset: 0x00191CF3
	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	// Token: 0x06004B84 RID: 19332 RVA: 0x00193AFC File Offset: 0x00191CFC
	protected Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__27 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__27>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x06004B85 RID: 19333 RVA: 0x00193B40 File Offset: 0x00191D40
	private Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		LegalAgreements.<UpdateText>d__28 <UpdateText>d__;
		<UpdateText>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateText>d__.<>4__this = this;
		<UpdateText>d__.asset = asset;
		<UpdateText>d__.version = version;
		<UpdateText>d__.<>1__state = -1;
		<UpdateText>d__.<>t__builder.Start<LegalAgreements.<UpdateText>d__28>(ref <UpdateText>d__);
		return <UpdateText>d__.<>t__builder.Task;
	}

	// Token: 0x06004B86 RID: 19334 RVA: 0x00193B94 File Offset: 0x00191D94
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.target = target;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06004B87 RID: 19335 RVA: 0x00193BEF File Offset: 0x00191DEF
	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	// Token: 0x06004B88 RID: 19336 RVA: 0x00193BF8 File Offset: 0x00191DF8
	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	// Token: 0x06004B89 RID: 19337 RVA: 0x00193C08 File Offset: 0x00191E08
	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__36 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__36>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06004B8A RID: 19338 RVA: 0x00193C4C File Offset: 0x00191E4C
	private Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__37 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__37>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06004B8B RID: 19339 RVA: 0x00193C90 File Offset: 0x00191E90
	private Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__38 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__38>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06004B8C RID: 19340 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005E92 RID: 24210
	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	// Token: 0x04005E93 RID: 24211
	[Header("Scroll Behavior")]
	[SerializeField]
	protected float _minScrollSpeed = 0.02f;

	// Token: 0x04005E94 RID: 24212
	[SerializeField]
	private float _maxScrollSpeed = 3f;

	// Token: 0x04005E95 RID: 24213
	[SerializeField]
	private float _scrollInterpTime = 3f;

	// Token: 0x04005E96 RID: 24214
	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005E98 RID: 24216
	[SerializeField]
	protected Transform uiParent;

	// Token: 0x04005E99 RID: 24217
	[SerializeField]
	protected TMP_Text tmpBody;

	// Token: 0x04005E9A RID: 24218
	[SerializeField]
	protected TMP_Text tmpTitle;

	// Token: 0x04005E9B RID: 24219
	[SerializeField]
	protected Scrollbar scrollBar;

	// Token: 0x04005E9C RID: 24220
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x04005E9D RID: 24221
	[SerializeField]
	protected KIDUIButton _pressAndHoldToConfirmButton;

	// Token: 0x04005E9E RID: 24222
	[SerializeField]
	private TMP_Text _scrollToBottomText;

	// Token: 0x04005E9F RID: 24223
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x04005EA0 RID: 24224
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x04005EA1 RID: 24225
	protected float stickHeldDuration;

	// Token: 0x04005EA2 RID: 24226
	protected float scrollSpeed;

	// Token: 0x04005EA3 RID: 24227
	private float scrollTime;

	// Token: 0x04005EA4 RID: 24228
	protected bool legalAgreementsStarted;

	// Token: 0x04005EA5 RID: 24229
	protected bool _accepted;

	// Token: 0x04005EA6 RID: 24230
	private string cachedText;

	// Token: 0x04005EA7 RID: 24231
	private int state;

	// Token: 0x04005EA8 RID: 24232
	private bool optIn;

	// Token: 0x04005EA9 RID: 24233
	private bool optional;
}
