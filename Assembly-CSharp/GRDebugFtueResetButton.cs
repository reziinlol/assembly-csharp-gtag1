using System;
using UnityEngine;

// Token: 0x02000756 RID: 1878
public class GRDebugFtueResetButton : GorillaPressableReleaseButton
{
	// Token: 0x06002F82 RID: 12162 RVA: 0x00102C6D File Offset: 0x00100E6D
	private void Awake()
	{
		if (!this.availableOnLive)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x00102C83 File Offset: 0x00100E83
	public void OnPressedButton()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002F84 RID: 12164 RVA: 0x00102C99 File Offset: 0x00100E99
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.isOn = true;
		this.UpdateColor();
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00102CAE File Offset: 0x00100EAE
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x04003D07 RID: 15623
	public bool availableOnLive;
}
