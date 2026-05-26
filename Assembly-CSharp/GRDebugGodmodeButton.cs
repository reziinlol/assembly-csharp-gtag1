using System;

// Token: 0x02000757 RID: 1879
public class GRDebugGodmodeButton : GorillaPressableReleaseButton
{
	// Token: 0x06002F87 RID: 12167 RVA: 0x000440BC File Offset: 0x000422BC
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPressedButton()
	{
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x00102CCB File Offset: 0x00100ECB
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.UpdateColor();
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x00102CD9 File Offset: 0x00100ED9
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.UpdateColor();
	}
}
