using System;
using TMPro;
using UnityEngine;

// Token: 0x02000BAF RID: 2991
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x06004B26 RID: 19238 RVA: 0x00191EB6 File Offset: 0x001900B6
	public void Show(string errorMessage)
	{
		base.gameObject.SetActive(true);
		if (errorMessage != null && errorMessage.Length > 0)
		{
			this._errorTxt.text = errorMessage;
		}
	}

	// Token: 0x06004B27 RID: 19239 RVA: 0x00191EDC File Offset: 0x001900DC
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06004B28 RID: 19240 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005E1E RID: 24094
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005E1F RID: 24095
	[SerializeField]
	private TMP_Text _errorTxt;
}
