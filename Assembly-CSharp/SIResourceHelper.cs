using System;
using System.Collections.Generic;

// Token: 0x02000155 RID: 341
public static class SIResourceHelper
{
	// Token: 0x060008FE RID: 2302 RVA: 0x000309F8 File Offset: 0x0002EBF8
	public static bool IsInOrder(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null)
		{
			return true;
		}
		SIResource.ResourceType resourceType = (SIResource.ResourceType)(-1);
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			if (resourceCost.type <= resourceType)
			{
				return false;
			}
			resourceType = resourceCost.type;
		}
		return true;
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00030A58 File Offset: 0x0002EC58
	public static bool IsValid(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null || cost.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount <= 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00030AD4 File Offset: 0x0002ECD4
	public static bool IsValid_AllowZero(this IList<SIResource.ResourceCost> cost)
	{
		if (cost == null || cost.Count == 0)
		{
			return false;
		}
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in cost)
		{
			int num2 = 1 << (int)resourceCost.type;
			if ((num & num2) != 0)
			{
				return false;
			}
			if (resourceCost.amount < 0)
			{
				return false;
			}
			num |= num2;
		}
		return true;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00030B50 File Offset: 0x0002ED50
	public static SIResource.ResourceCategoryCost GetCategoryCosts(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		int num2 = 0;
		if (costs != null)
		{
			foreach (SIResource.ResourceCost resourceCost in costs)
			{
				if (resourceCost.type == SIResource.ResourceType.TechPoint)
				{
					num += resourceCost.amount;
				}
				else
				{
					num2 += resourceCost.amount;
				}
			}
		}
		return new SIResource.ResourceCategoryCost(num, num2);
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00030BBC File Offset: 0x0002EDBC
	public static List<SIResource.ResourceCost> GetTotalResourceCost(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost item in additiveCosts)
		{
			list.Add(item);
		}
		return list;
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00030C0C File Offset: 0x0002EE0C
	public static List<SIResource.ResourceCost> GetMax(this IList<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCosts)
	{
		List<SIResource.ResourceCost> list = new List<SIResource.ResourceCost>(baseCost);
		foreach (SIResource.ResourceCost item in additiveCosts)
		{
			list.Add(item);
		}
		list.Sort();
		return list;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00030C64 File Offset: 0x0002EE64
	public static int GetAmount(this IList<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType)
	{
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == resourceType)
			{
				return resourceCost.amount;
			}
		}
		return 0;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00030CBC File Offset: 0x0002EEBC
	public static void SetAmount(this List<SIResource.ResourceCost> costs, SIResource.ResourceType resourceType, int amount)
	{
		for (int i = 0; i < costs.Count; i++)
		{
			SIResource.ResourceCost resourceCost = costs[i];
			if (resourceCost.type == resourceType)
			{
				resourceCost.amount = amount;
				costs[i] = resourceCost;
				return;
			}
		}
		costs.Add(new SIResource.ResourceCost(resourceType, amount));
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00030D0C File Offset: 0x0002EF0C
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, SIResource.ResourceCost additiveCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == additiveCost.type)
			{
				resourceCost.amount += additiveCost.amount;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(additiveCost);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00030D64 File Offset: 0x0002EF64
	public static void AddResourceCost(this List<SIResource.ResourceCost> baseCost, IList<SIResource.ResourceCost> additiveCost)
	{
		foreach (SIResource.ResourceCost additiveCost2 in additiveCost)
		{
			baseCost.AddResourceCost(additiveCost2);
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00030DAC File Offset: 0x0002EFAC
	public static int GetTechPointCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00030E04 File Offset: 0x0002F004
	public static int GetMiscCost(this IList<SIResource.ResourceCost> costs)
	{
		int num = 0;
		foreach (SIResource.ResourceCost resourceCost in costs)
		{
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				num += resourceCost.amount;
			}
		}
		return num;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00030E5C File Offset: 0x0002F05C
	public static void SetResourceCost(this IList<SIResource.ResourceCost> costs, SIResource.ResourceCategoryCost desiredCosts)
	{
		costs.SetTechPointCost(desiredCosts.techPoints);
		costs.SetMiscCost(desiredCosts.misc);
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00030E76 File Offset: 0x0002F076
	public static void AddResourceCost(this IList<SIResource.ResourceCost> baseCost, SIResource.ResourceCategoryCost additiveCost)
	{
		baseCost.SetTechPointCost(baseCost.GetTechPointCost() + additiveCost.techPoints);
		baseCost.SetMiscCost(baseCost.GetMiscCost() + additiveCost.misc);
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00030EA0 File Offset: 0x0002F0A0
	public static void SetTechPointCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type == SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount = desiredCost;
				baseCost[i] = resourceCost;
				return;
			}
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.TechPoint, desiredCost));
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00030EEC File Offset: 0x0002F0EC
	public static void SetMiscCost(this IList<SIResource.ResourceCost> baseCost, int desiredCost)
	{
		int miscCost = baseCost.GetMiscCost();
		if (miscCost == desiredCost)
		{
			return;
		}
		for (int i = 0; i < baseCost.Count; i++)
		{
			SIResource.ResourceCost resourceCost = baseCost[i];
			if (resourceCost.type != SIResource.ResourceType.TechPoint)
			{
				resourceCost.amount += desiredCost - miscCost;
				if (resourceCost.amount >= 1)
				{
					baseCost[i] = resourceCost;
					return;
				}
				baseCost.RemoveAt(i--);
				miscCost = baseCost.GetMiscCost();
				if (miscCost == desiredCost)
				{
					return;
				}
			}
		}
		if (desiredCost == miscCost)
		{
			return;
		}
		baseCost.Add(new SIResource.ResourceCost(SIResource.ResourceType.StrangeWood, desiredCost - miscCost));
	}
}
