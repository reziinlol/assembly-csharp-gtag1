using System;

// Token: 0x0200011C RID: 284
[Serializable]
internal struct SIUpgradeBasedGeneric<T>
{
	// Token: 0x0600071B RID: 1819 RVA: 0x00028A0C File Offset: 0x00026C0C
	public bool TryGetActiveValue(SIUpgradeSet withUpgrades, out T out_value)
	{
		out_value = default(T);
		bool result = false;
		for (int i = 0; i < this.entries.Length; i++)
		{
			if (this.entries[i].IsActive(withUpgrades))
			{
				result = true;
				out_value = this.entries[i].value;
			}
		}
		return result;
	}

	// Token: 0x04000903 RID: 2307
	public SIUpgradeBasedGenericEntry<T>[] entries;
}
