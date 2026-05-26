using System;

// Token: 0x02000136 RID: 310
public struct ResettableUseCounter
{
	// Token: 0x060007BC RID: 1980 RVA: 0x0002A5D0 File Offset: 0x000287D0
	public ResettableUseCounter(int maxRegularUses, int maxSuperchargeUses, Action<bool> onReadyChanged = null)
	{
		this.maxRegularUses = maxRegularUses;
		this.maxSuperchargeUses = maxSuperchargeUses;
		this.usesRemaining = maxRegularUses;
		this.onReadyChanged = onReadyChanged;
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060007BD RID: 1981 RVA: 0x0002A5EE File Offset: 0x000287EE
	public bool IsReady
	{
		get
		{
			return this.usesRemaining > 0;
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002A5FC File Offset: 0x000287FC
	public bool TryUse()
	{
		if (!this.IsReady)
		{
			return false;
		}
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		bool flag = activeSuperInfectionManager != null && activeSuperInfectionManager.IsSupercharged;
		if (this.usesRemaining > this.maxRegularUses && !flag)
		{
			this.usesRemaining = this.maxRegularUses;
		}
		this.usesRemaining--;
		if (!this.IsReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action != null)
			{
				action(false);
			}
		}
		return true;
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0002A66B File Offset: 0x0002886B
	public void Reset()
	{
		bool isReady = this.IsReady;
		this.usesRemaining = this.maxSuperchargeUses;
		if (!isReady)
		{
			Action<bool> action = this.onReadyChanged;
			if (action == null)
			{
				return;
			}
			action(true);
		}
	}

	// Token: 0x040009CA RID: 2506
	private int usesRemaining;

	// Token: 0x040009CB RID: 2507
	private int maxRegularUses;

	// Token: 0x040009CC RID: 2508
	private int maxSuperchargeUses;

	// Token: 0x040009CD RID: 2509
	private Action<bool> onReadyChanged;
}
