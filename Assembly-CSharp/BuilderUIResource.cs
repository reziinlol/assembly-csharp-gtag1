using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x0200065C RID: 1628
public class BuilderUIResource : MonoBehaviour
{
	// Token: 0x0600288B RID: 10379 RVA: 0x000DC490 File Offset: 0x000DA690
	public void SetResourceCost(BuilderResourceQuantity resourceCost, BuilderTable table)
	{
		BuilderResourceType type = resourceCost.type;
		int count = resourceCost.count;
		int availableResources = table.GetAvailableResources(type);
		if (this.resourceNameLabel != null)
		{
			this.resourceNameLabel.text = this.GetResourceName(type);
		}
		if (this.costLabel != null)
		{
			this.costLabel.text = count.ToString();
		}
		if (this.availableLabel != null)
		{
			this.availableLabel.text = availableResources.ToString();
		}
	}

	// Token: 0x0600288C RID: 10380 RVA: 0x000DC513 File Offset: 0x000DA713
	private string GetResourceName(BuilderResourceType type)
	{
		switch (type)
		{
		case BuilderResourceType.Basic:
			return "Basic";
		case BuilderResourceType.Decorative:
			return "Decorative";
		case BuilderResourceType.Functional:
			return "Functional";
		default:
			return "Resource Needs Name";
		}
	}

	// Token: 0x040034DF RID: 13535
	public TextMeshPro resourceNameLabel;

	// Token: 0x040034E0 RID: 13536
	public TextMeshPro costLabel;

	// Token: 0x040034E1 RID: 13537
	public TextMeshPro availableLabel;
}
