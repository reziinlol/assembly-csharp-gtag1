using System;
using GorillaNetworking;

// Token: 0x02000A28 RID: 2600
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x0600427F RID: 17023 RVA: 0x0016367F File Offset: 0x0016187F
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x00163688 File Offset: 0x00161888
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x06004281 RID: 17025 RVA: 0x001636E0 File Offset: 0x001618E0
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x04005473 RID: 21619
	public string gameMode;

	// Token: 0x04005474 RID: 21620
	private bool initialized;
}
