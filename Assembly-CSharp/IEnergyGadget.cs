using System;

// Token: 0x020000FD RID: 253
public interface IEnergyGadget
{
	// Token: 0x17000063 RID: 99
	// (get) Token: 0x060005E9 RID: 1513
	bool UsesEnergy { get; }

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x060005EA RID: 1514
	bool IsFull { get; }

	// Token: 0x060005EB RID: 1515
	void UpdateRecharge(float dt);
}
