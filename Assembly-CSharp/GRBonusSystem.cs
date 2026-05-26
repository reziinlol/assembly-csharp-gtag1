using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074C RID: 1868
public class GRBonusSystem
{
	// Token: 0x06002F52 RID: 12114 RVA: 0x00101BE4 File Offset: 0x000FFDE4
	public void Init(GRAttributes attributes)
	{
		this.defaultAttributes = attributes;
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x00101BED File Offset: 0x000FFDED
	public GRAttributes GetDefaultAttributes()
	{
		return this.defaultAttributes;
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x00101BF8 File Offset: 0x000FFDF8
	public void AddBonus(GRBonusEntry entry)
	{
		if (entry.bonusType == GRBonusEntry.GRBonusType.None)
		{
			return;
		}
		if (!this.currentAdditiveBonuses.ContainsKey(entry.attributeType))
		{
			this.currentAdditiveBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (!this.currentMultiplicativeBonuses.ContainsKey(entry.attributeType))
		{
			this.currentMultiplicativeBonuses[entry.attributeType] = new List<GRBonusEntry>();
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Additive)
		{
			this.currentAdditiveBonuses[entry.attributeType].Add(entry);
			return;
		}
		if (entry.bonusType == GRBonusEntry.GRBonusType.Multiplicative)
		{
			this.currentMultiplicativeBonuses[entry.attributeType].Add(entry);
		}
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x00101CA4 File Offset: 0x000FFEA4
	public void RemoveBonus(GRBonusEntry entry)
	{
		foreach (List<GRBonusEntry> list in this.currentAdditiveBonuses.Values)
		{
			list.Remove(entry);
		}
		foreach (List<GRBonusEntry> list2 in this.currentMultiplicativeBonuses.Values)
		{
			list2.Remove(entry);
		}
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x00101D44 File Offset: 0x000FFF44
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.defaultAttributes != null && this.defaultAttributes.defaultAttributes.ContainsKey(attributeType);
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x00101D68 File Offset: 0x000FFF68
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		if (this.defaultAttributes == null)
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes null.  Please fix configuration.", Array.Empty<object>());
			return 0;
		}
		if (!this.defaultAttributes.defaultAttributes.ContainsKey(attributeType))
		{
			Debug.LogErrorFormat("CalculateFinalValueForAttribute DefaultAttributes Does not have entry for {0}.  Please fix configuration.", new object[]
			{
				attributeType
			});
			return 0;
		}
		int num = this.defaultAttributes.defaultAttributes[attributeType];
		if (this.currentAdditiveBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry in this.currentAdditiveBonuses[attributeType])
			{
				if (grbonusEntry.customBonus != null)
				{
					num = grbonusEntry.customBonus(num, grbonusEntry);
				}
				else
				{
					num += grbonusEntry.GetBonusValue();
				}
			}
		}
		if (this.currentMultiplicativeBonuses.ContainsKey(attributeType))
		{
			foreach (GRBonusEntry grbonusEntry2 in this.currentMultiplicativeBonuses[attributeType])
			{
				if (grbonusEntry2.customBonus != null)
				{
					num = grbonusEntry2.customBonus(num, grbonusEntry2);
				}
				else
				{
					float num2 = (float)grbonusEntry2.GetBonusValue() / 100f;
					num = (int)((float)num * num2);
				}
			}
		}
		return num;
	}

	// Token: 0x04003CC3 RID: 15555
	private GRAttributes defaultAttributes;

	// Token: 0x04003CC4 RID: 15556
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentAdditiveBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();

	// Token: 0x04003CC5 RID: 15557
	private Dictionary<GRAttributeType, List<GRBonusEntry>> currentMultiplicativeBonuses = new Dictionary<GRAttributeType, List<GRBonusEntry>>();
}
