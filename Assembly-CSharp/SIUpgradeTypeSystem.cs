using System;

// Token: 0x02000120 RID: 288
public static class SIUpgradeTypeSystem
{
	// Token: 0x0600071D RID: 1821 RVA: 0x00028AD2 File Offset: 0x00026CD2
	public static int GetPageId(this SIUpgradeType self)
	{
		return (int)(self / SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00028AD8 File Offset: 0x00026CD8
	public static int GetNodeId(this SIUpgradeType self)
	{
		return (int)(self % SIUpgradeType.Stilt_Unlock);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00028ADE File Offset: 0x00026CDE
	public static SIUpgradeType GetUpgradeType(int pageId, int nodeId)
	{
		return (SIUpgradeType)(pageId * 100 + nodeId);
	}
}
