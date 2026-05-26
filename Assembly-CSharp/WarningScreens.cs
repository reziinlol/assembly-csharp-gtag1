using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000BB8 RID: 3000
public class WarningScreens : MonoBehaviour
{
	// Token: 0x06004B44 RID: 19268 RVA: 0x001924D1 File Offset: 0x001906D1
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	// Token: 0x06004B45 RID: 19269 RVA: 0x001924F8 File Offset: 0x001906F8
	private Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreenInternal>d__14 <StartWarningScreenInternal>d__;
		<StartWarningScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreenInternal>d__.<>4__this = this;
		<StartWarningScreenInternal>d__.cancellationToken = cancellationToken;
		<StartWarningScreenInternal>d__.<>1__state = -1;
		<StartWarningScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartWarningScreenInternal>d__14>(ref <StartWarningScreenInternal>d__);
		return <StartWarningScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06004B46 RID: 19270 RVA: 0x00192544 File Offset: 0x00190744
	private Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreenInternal>d__15 <StartOptInFollowUpScreenInternal>d__;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreenInternal>d__.<>4__this = this;
		<StartOptInFollowUpScreenInternal>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreenInternal>d__.<>1__state = -1;
		<StartOptInFollowUpScreenInternal>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreenInternal>d__15>(ref <StartOptInFollowUpScreenInternal>d__);
		return <StartOptInFollowUpScreenInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06004B47 RID: 19271 RVA: 0x00192590 File Offset: 0x00190790
	public static Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartWarningScreen>d__16 <StartWarningScreen>d__;
		<StartWarningScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartWarningScreen>d__.cancellationToken = cancellationToken;
		<StartWarningScreen>d__.<>1__state = -1;
		<StartWarningScreen>d__.<>t__builder.Start<WarningScreens.<StartWarningScreen>d__16>(ref <StartWarningScreen>d__);
		return <StartWarningScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06004B48 RID: 19272 RVA: 0x001925D4 File Offset: 0x001907D4
	public static Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		WarningScreens.<StartOptInFollowUpScreen>d__17 <StartOptInFollowUpScreen>d__;
		<StartOptInFollowUpScreen>d__.<>t__builder = AsyncTaskMethodBuilder<WarningButtonResult>.Create();
		<StartOptInFollowUpScreen>d__.cancellationToken = cancellationToken;
		<StartOptInFollowUpScreen>d__.<>1__state = -1;
		<StartOptInFollowUpScreen>d__.<>t__builder.Start<WarningScreens.<StartOptInFollowUpScreen>d__17>(ref <StartOptInFollowUpScreen>d__);
		return <StartOptInFollowUpScreen>d__.<>t__builder.Task;
	}

	// Token: 0x06004B49 RID: 19273 RVA: 0x00192618 File Offset: 0x00190818
	private static Task WaitForResponse(CancellationToken cancellationToken)
	{
		WarningScreens.<WaitForResponse>d__18 <WaitForResponse>d__;
		<WaitForResponse>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForResponse>d__.cancellationToken = cancellationToken;
		<WaitForResponse>d__.<>1__state = -1;
		<WaitForResponse>d__.<>t__builder.Start<WarningScreens.<WaitForResponse>d__18>(ref <WaitForResponse>d__);
		return <WaitForResponse>d__.<>t__builder.Task;
	}

	// Token: 0x06004B4A RID: 19274 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004B4B RID: 19275 RVA: 0x0019265B File Offset: 0x0019085B
	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction();
	}

	// Token: 0x06004B4C RID: 19276 RVA: 0x00192686 File Offset: 0x00190886
	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction();
	}

	// Token: 0x04005E3A RID: 24122
	private static WarningScreens _activeReference;

	// Token: 0x04005E3B RID: 24123
	[SerializeField]
	private MessageBox _messageBox;

	// Token: 0x04005E3C RID: 24124
	[SerializeField]
	private GameObject _imageContainerAfter;

	// Token: 0x04005E3D RID: 24125
	[SerializeField]
	private GameObject _imageContainerBefore;

	// Token: 0x04005E3E RID: 24126
	[SerializeField]
	private TMP_Text _withImageTextBefore;

	// Token: 0x04005E3F RID: 24127
	[SerializeField]
	private TMP_Text _withImageTextAfter;

	// Token: 0x04005E40 RID: 24128
	[SerializeField]
	private TMP_Text _noImageText;

	// Token: 0x04005E41 RID: 24129
	private Action _onLeftButtonPressedAction;

	// Token: 0x04005E42 RID: 24130
	private Action _onRightButtonPressedAction;

	// Token: 0x04005E43 RID: 24131
	private static WarningButtonResult _result;

	// Token: 0x04005E44 RID: 24132
	private static WarningButtonResult _leftButtonResult;

	// Token: 0x04005E45 RID: 24133
	private static WarningButtonResult _rightButtonResult;

	// Token: 0x04005E46 RID: 24134
	private static bool _closedMessageBox;
}
