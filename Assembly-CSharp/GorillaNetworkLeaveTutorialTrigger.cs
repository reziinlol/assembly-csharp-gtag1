using System;

// Token: 0x02000C81 RID: 3201
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x06004F7C RID: 20348 RVA: 0x001A5451 File Offset: 0x001A3651
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
