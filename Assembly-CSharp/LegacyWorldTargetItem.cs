using System;
using Photon.Realtime;

// Token: 0x020004B2 RID: 1202
public class LegacyWorldTargetItem
{
	// Token: 0x06001D55 RID: 7509 RVA: 0x0009E768 File Offset: 0x0009C968
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x0009E77E File Offset: 0x0009C97E
	public void Invalidate()
	{
		this.itemIdx = -1;
		this.owner = null;
	}

	// Token: 0x04002783 RID: 10115
	public Player owner;

	// Token: 0x04002784 RID: 10116
	public int itemIdx;
}
