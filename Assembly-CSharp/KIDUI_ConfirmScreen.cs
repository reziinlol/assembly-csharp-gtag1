using System;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

// Token: 0x02000B9D RID: 2973
public class KIDUI_ConfirmScreen : MonoBehaviour
{
	// Token: 0x06004AB9 RID: 19129 RVA: 0x0018F33C File Offset: 0x0018D53C
	private void Awake()
	{
		if (this._emailToConfirmTxt == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Email To Confirm Field is NULL", Array.Empty<object>());
			return;
		}
		if (this._setupScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Setup K-ID Screen is NULL", Array.Empty<object>());
			return;
		}
		if (this._mainScreen == null)
		{
			Debug.LogErrorFormat("[KID::UI::Setup] Main Screen is NULL", Array.Empty<object>());
			return;
		}
		this._cancellationTokenSource = new CancellationTokenSource();
	}

	// Token: 0x06004ABA RID: 19130 RVA: 0x0018F3AE File Offset: 0x0018D5AE
	private void OnEnable()
	{
		this._confirmButton.interactable = true;
		this._backButton.interactable = true;
	}

	// Token: 0x06004ABB RID: 19131 RVA: 0x0018F3C8 File Offset: 0x0018D5C8
	public void OnEmailSubmitted(string emailAddress)
	{
		this._submittedEmailAddress = emailAddress;
		this._emailToConfirmTxt.text = this._submittedEmailAddress;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004ABC RID: 19132 RVA: 0x0018F3F0 File Offset: 0x0018D5F0
	public void OnConfirmPressed()
	{
		KIDUI_ConfirmScreen.<OnConfirmPressed>d__16 <OnConfirmPressed>d__;
		<OnConfirmPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnConfirmPressed>d__.<>4__this = this;
		<OnConfirmPressed>d__.<>1__state = -1;
		<OnConfirmPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnConfirmPressed>d__16>(ref <OnConfirmPressed>d__);
	}

	// Token: 0x06004ABD RID: 19133 RVA: 0x0018F428 File Offset: 0x0018D628
	public void OnBackPressed()
	{
		KIDUI_ConfirmScreen.<OnBackPressed>d__17 <OnBackPressed>d__;
		<OnBackPressed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnBackPressed>d__.<>4__this = this;
		<OnBackPressed>d__.<>1__state = -1;
		<OnBackPressed>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<OnBackPressed>d__17>(ref <OnBackPressed>d__);
	}

	// Token: 0x06004ABE RID: 19134 RVA: 0x0018F45F File Offset: 0x0018D65F
	public void NotifyOfResult(bool success)
	{
		this._hasCompletedSendEmailRequest = true;
		this._emailRequestResult = success;
	}

	// Token: 0x06004ABF RID: 19135 RVA: 0x0018F470 File Offset: 0x0018D670
	private void ShowErrorScreen(string errorMessage)
	{
		KIDUI_ConfirmScreen.<ShowErrorScreen>d__19 <ShowErrorScreen>d__;
		<ShowErrorScreen>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ShowErrorScreen>d__.<>4__this = this;
		<ShowErrorScreen>d__.errorMessage = errorMessage;
		<ShowErrorScreen>d__.<>1__state = -1;
		<ShowErrorScreen>d__.<>t__builder.Start<KIDUI_ConfirmScreen.<ShowErrorScreen>d__19>(ref <ShowErrorScreen>d__);
	}

	// Token: 0x06004AC0 RID: 19136 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005D90 RID: 23952
	[SerializeField]
	private TMP_Text _emailToConfirmTxt;

	// Token: 0x04005D91 RID: 23953
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005D92 RID: 23954
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;

	// Token: 0x04005D93 RID: 23955
	[SerializeField]
	private KIDUI_ErrorScreen _errorScreen;

	// Token: 0x04005D94 RID: 23956
	[SerializeField]
	private KIDUI_EmailSuccess _successScreen;

	// Token: 0x04005D95 RID: 23957
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005D96 RID: 23958
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04005D97 RID: 23959
	[SerializeField]
	private KIDUIButton _backButton;

	// Token: 0x04005D98 RID: 23960
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x04005D99 RID: 23961
	private string _submittedEmailAddress;

	// Token: 0x04005D9A RID: 23962
	private CancellationTokenSource _cancellationTokenSource;

	// Token: 0x04005D9B RID: 23963
	private bool _hasCompletedSendEmailRequest;

	// Token: 0x04005D9C RID: 23964
	private bool _emailRequestResult;
}
