using System;
using UnityEngine;

// Token: 0x0200074A RID: 1866
[Serializable]
public class GRBonusEntry
{
	// Token: 0x06002F4D RID: 12109 RVA: 0x00101B3A File Offset: 0x000FFD3A
	private GRBonusEntry()
	{
		GRBonusEntry.idCounter++;
		this.id = GRBonusEntry.idCounter;
	}

	// Token: 0x1700046D RID: 1133
	// (get) Token: 0x06002F4E RID: 12110 RVA: 0x00101B59 File Offset: 0x000FFD59
	// (set) Token: 0x06002F4F RID: 12111 RVA: 0x00101B61 File Offset: 0x000FFD61
	public int id { get; private set; }

	// Token: 0x06002F50 RID: 12112 RVA: 0x00101B6A File Offset: 0x000FFD6A
	public int GetBonusValue()
	{
		return (int)(this.bonusValue * 100f);
	}

	// Token: 0x06002F51 RID: 12113 RVA: 0x00101B7C File Offset: 0x000FFD7C
	public override string ToString()
	{
		bool flag = this.customBonus != null;
		return string.Format("GRBonusEntry BonusType {0} AttributeType {1} BonusValue {2} Id {3} CustomBonusSet {4}", new object[]
		{
			this.bonusType,
			this.attributeType,
			this.bonusValue,
			this.id,
			flag
		});
	}

	// Token: 0x04003CB9 RID: 15545
	private static int idCounter;

	// Token: 0x04003CBA RID: 15546
	public GRBonusEntry.GRBonusType bonusType;

	// Token: 0x04003CBB RID: 15547
	public GRAttributeType attributeType;

	// Token: 0x04003CBC RID: 15548
	[SerializeField]
	private float bonusValue;

	// Token: 0x04003CBE RID: 15550
	public Func<int, GRBonusEntry, int> customBonus;

	// Token: 0x0200074B RID: 1867
	public enum GRBonusType
	{
		// Token: 0x04003CC0 RID: 15552
		None,
		// Token: 0x04003CC1 RID: 15553
		Additive,
		// Token: 0x04003CC2 RID: 15554
		Multiplicative
	}
}
