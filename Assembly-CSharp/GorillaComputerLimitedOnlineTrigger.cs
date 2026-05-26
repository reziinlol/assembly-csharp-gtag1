using System;
using GorillaNetworking;

// Token: 0x020009FF RID: 2559
public class GorillaComputerLimitedOnlineTrigger : GorillaTriggerBox
{
	// Token: 0x0600416D RID: 16749 RVA: 0x0015E49F File Offset: 0x0015C69F
	public override void OnBoxTriggered()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(true);
	}

	// Token: 0x0600416E RID: 16750 RVA: 0x0015E4AE File Offset: 0x0015C6AE
	public override void OnBoxExited()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(false);
	}
}
