using System;
using TMPro;
using UnityEngine;

// Token: 0x02000BA7 RID: 2983
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x06004AE7 RID: 19175 RVA: 0x00190375 File Offset: 0x0018E575
	public void ShowErrorScreen(string title, string email, string errorMessage)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		this._errorTxt.text = errorMessage;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004AE8 RID: 19176 RVA: 0x001903A7 File Offset: 0x0018E5A7
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06004AE9 RID: 19177 RVA: 0x0018DF22 File Offset: 0x0018C122
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06004AEA RID: 19178 RVA: 0x001903C1 File Offset: 0x0018E5C1
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x04005DCD RID: 24013
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x04005DCE RID: 24014
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x04005DCF RID: 24015
	[SerializeField]
	private TMP_Text _errorTxt;

	// Token: 0x04005DD0 RID: 24016
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005DD1 RID: 24017
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
