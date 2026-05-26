using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000741 RID: 1857
public class GRAttributes : MonoBehaviour
{
	// Token: 0x06002F1D RID: 12061 RVA: 0x00100F5C File Offset: 0x000FF15C
	private void Awake()
	{
		foreach (GRAttributes.GRAttributePair grattributePair in this.startingAttributes)
		{
			this.defaultAttributes[grattributePair.type] = (int)(grattributePair.value * 100f);
		}
		this.bonusSystem.Init(this);
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x00100FD4 File Offset: 0x000FF1D4
	public bool HasBeenInitialized()
	{
		return this.bonusSystem.GetDefaultAttributes() != null;
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x00100FE7 File Offset: 0x000FF1E7
	public void AddAttribute(GRAttributeType type, float value)
	{
		this.defaultAttributes[type] = (int)(value * 100f);
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x00100FFD File Offset: 0x000FF1FD
	public void AddBonus(GRBonusEntry entry)
	{
		this.bonusSystem.AddBonus(entry);
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x0010100B File Offset: 0x000FF20B
	public void RemoveBonus(GRBonusEntry entry)
	{
		this.bonusSystem.RemoveBonus(entry);
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x0010101C File Offset: 0x000FF21C
	public float CalculateFinalFloatValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		float result = 0f;
		if (num > 0)
		{
			result = (float)num / 100f;
		}
		return result;
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x0010104C File Offset: 0x000FF24C
	public int CalculateFinalValueForAttribute(GRAttributeType attributeType)
	{
		int num = this.bonusSystem.CalculateFinalValueForAttribute(attributeType);
		if (num > 0)
		{
			num /= 100;
		}
		return num;
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x00101070 File Offset: 0x000FF270
	public bool HasValueForAttribute(GRAttributeType attributeType)
	{
		return this.bonusSystem.HasValueForAttribute(attributeType);
	}

	// Token: 0x04003C7C RID: 15484
	[SerializeField]
	private List<GRAttributes.GRAttributePair> startingAttributes;

	// Token: 0x04003C7D RID: 15485
	[NonSerialized]
	private GRBonusSystem bonusSystem = new GRBonusSystem();

	// Token: 0x04003C7E RID: 15486
	public Dictionary<GRAttributeType, int> defaultAttributes = new Dictionary<GRAttributeType, int>();

	// Token: 0x02000742 RID: 1858
	[Serializable]
	public struct GRAttributePair
	{
		// Token: 0x04003C7F RID: 15487
		public GRAttributeType type;

		// Token: 0x04003C80 RID: 15488
		public float value;
	}
}
