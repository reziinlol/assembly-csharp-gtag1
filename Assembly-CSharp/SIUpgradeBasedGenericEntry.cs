using System;
using UnityEngine;

// Token: 0x0200011D RID: 285
[Serializable]
internal struct SIUpgradeBasedGenericEntry<T>
{
	// Token: 0x0600071C RID: 1820 RVA: 0x00028A64 File Offset: 0x00026C64
	public bool IsActive(SIUpgradeSet withUpgrades)
	{
		bool flag = true;
		if (this.activeRequirements.Length != 0)
		{
			flag = false;
			foreach (SIUpgradeType upgrade in this.activeRequirements)
			{
				if (withUpgrades.Contains(upgrade))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			foreach (SIUpgradeType upgrade2 in this.inactiveRequirements)
			{
				if (withUpgrades.Contains(upgrade2))
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	// Token: 0x04000904 RID: 2308
	public T value;

	// Token: 0x04000905 RID: 2309
	[Tooltip("For the objects to become activated, you must match AT LEAST ONE appearRequirement (if there are any), and not match any disappearRequirements.")]
	public SIUpgradeType[] activeRequirements;

	// Token: 0x04000906 RID: 2310
	[Tooltip("For the objects to become deactivated, you must match AT LEAST ONE disappearRequirement (if there are any).")]
	public SIUpgradeType[] inactiveRequirements;
}
