using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000BB1 RID: 2993
public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	// Token: 0x06004B2E RID: 19246 RVA: 0x00191F9C File Offset: 0x0019019C
	public Task SendUpgradeEmail(List<string> requestedPermissions)
	{
		KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4 <SendUpgradeEmail>d__;
		<SendUpgradeEmail>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SendUpgradeEmail>d__.<>4__this = this;
		<SendUpgradeEmail>d__.requestedPermissions = requestedPermissions;
		<SendUpgradeEmail>d__.<>1__state = -1;
		<SendUpgradeEmail>d__.<>t__builder.Start<KIDUI_SendUpgradeEmailScreen.<SendUpgradeEmail>d__4>(ref <SendUpgradeEmail>d__);
		return <SendUpgradeEmail>d__.<>t__builder.Task;
	}

	// Token: 0x06004B2F RID: 19247 RVA: 0x00191FE7 File Offset: 0x001901E7
	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06004B30 RID: 19248 RVA: 0x00192001 File Offset: 0x00190201
	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show(null);
	}

	// Token: 0x06004B31 RID: 19249 RVA: 0x0019201B File Offset: 0x0019021B
	private void OnFailure(string errorMessage)
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show(errorMessage);
	}

	// Token: 0x04005E23 RID: 24099
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005E24 RID: 24100
	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	// Token: 0x04005E25 RID: 24101
	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	// Token: 0x04005E26 RID: 24102
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
