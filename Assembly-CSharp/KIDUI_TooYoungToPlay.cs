using System;
using UnityEngine;

// Token: 0x02000BB4 RID: 2996
public class KIDUI_TooYoungToPlay : MonoBehaviour
{
	// Token: 0x06004B3E RID: 19262 RVA: 0x0018BA56 File Offset: 0x00189C56
	public void ShowTooYoungToPlayScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004B3F RID: 19263 RVA: 0x0018DF22 File Offset: 0x0018C122
	public void OnQuitPressed()
	{
		Application.Quit();
	}
}
