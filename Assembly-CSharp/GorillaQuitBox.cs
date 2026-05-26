using System;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x06002535 RID: 9525 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000C5E71 File Offset: 0x000C4071
	public override void OnBoxTriggered()
	{
		Debug.Log("quitbox hit! hopefully you expected this to happen!");
		Application.Quit();
	}
}
