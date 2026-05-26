using System;

// Token: 0x02000121 RID: 289
public struct SIUpgradeSet
{
	// Token: 0x06000720 RID: 1824 RVA: 0x00028AE6 File Offset: 0x00026CE6
	public void Clear()
	{
		this.backingBits = 0;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x00028AEF File Offset: 0x00026CEF
	public SIUpgradeSet(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x00028AF8 File Offset: 0x00026CF8
	public int GetBits()
	{
		return this.backingBits;
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x00028AEF File Offset: 0x00026CEF
	public void SetBits(int bits)
	{
		this.backingBits = bits;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x00028B00 File Offset: 0x00026D00
	public long GetCreateData(SIPlayer player)
	{
		return (long)this.backingBits << 32 | (long)player.ActorNr;
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00028B14 File Offset: 0x00026D14
	public void Add(SIUpgradeType upgrade)
	{
		this.backingBits |= 1 << upgrade.GetNodeId();
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00028B2E File Offset: 0x00026D2E
	public void Add(int nodeId)
	{
		this.backingBits |= 1 << nodeId;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x00028B43 File Offset: 0x00026D43
	public void Remove(SIUpgradeType upgrade)
	{
		this.backingBits &= ~(1 << upgrade.GetNodeId());
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00028B5E File Offset: 0x00026D5E
	public bool Contains(SIUpgradeType upgrade)
	{
		return (this.backingBits & 1 << upgrade.GetNodeId()) != 0;
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x00028B78 File Offset: 0x00026D78
	public bool ContainsAny(params SIUpgradeType[] upgrades)
	{
		int num = 0;
		foreach (SIUpgradeType self in upgrades)
		{
			num |= 1 << self.GetNodeId();
		}
		return (this.backingBits & num) != 0;
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x00028BB4 File Offset: 0x00026DB4
	public string GetString(SITechTreePageId pageId)
	{
		string text = "";
		int i = this.backingBits;
		int num = 0;
		bool flag = true;
		while (i > 0)
		{
			if ((i & 1) != 0)
			{
				if (!flag)
				{
					text += "|";
				}
				text += SIUpgradeTypeSystem.GetUpgradeType((int)pageId, num).ToString();
				flag = false;
			}
			i >>= 1;
			num++;
		}
		return text;
	}

	// Token: 0x0400096F RID: 2415
	private int backingBits;
}
