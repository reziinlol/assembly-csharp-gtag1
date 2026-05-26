using System;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class FortuneResults : ScriptableObject
{
	// Token: 0x060029EA RID: 10730 RVA: 0x000E24D0 File Offset: 0x000E06D0
	private void OnValidate()
	{
		this.totalChance = 0f;
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			this.totalChance += this.fortuneResults[i].weightedChance;
		}
	}

	// Token: 0x060029EB RID: 10731 RVA: 0x000E251C File Offset: 0x000E071C
	public FortuneResults.FortuneResult GetResult()
	{
		float num = Random.Range(0f, this.totalChance);
		int i = 0;
		while (i < this.fortuneResults.Length)
		{
			FortuneResults.FortuneCategory fortuneCategory = this.fortuneResults[i];
			if (num <= fortuneCategory.weightedChance)
			{
				if (fortuneCategory.textResults.Length == 0)
				{
					return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
				}
				int resultIndex = Random.Range(0, fortuneCategory.textResults.Length);
				return new FortuneResults.FortuneResult(fortuneCategory.fortuneType, resultIndex);
			}
			else
			{
				num -= fortuneCategory.weightedChance;
				i++;
			}
		}
		return new FortuneResults.FortuneResult(FortuneResults.FortuneCategoryType.Invalid, -1);
	}

	// Token: 0x060029EC RID: 10732 RVA: 0x000E25A0 File Offset: 0x000E07A0
	public string GetResultText(FortuneResults.FortuneResult result)
	{
		for (int i = 0; i < this.fortuneResults.Length; i++)
		{
			if (this.fortuneResults[i].fortuneType == result.fortuneType && result.resultIndex >= 0 && result.resultIndex < this.fortuneResults[i].textResults.Length)
			{
				return this.fortuneResults[i].textResults[result.resultIndex];
			}
		}
		return "!! Invalid Fortune !!";
	}

	// Token: 0x0400369F RID: 13983
	[SerializeField]
	private FortuneResults.FortuneCategory[] fortuneResults;

	// Token: 0x040036A0 RID: 13984
	[SerializeField]
	private float totalChance;

	// Token: 0x02000692 RID: 1682
	public enum FortuneCategoryType
	{
		// Token: 0x040036A2 RID: 13986
		Invalid,
		// Token: 0x040036A3 RID: 13987
		Positive,
		// Token: 0x040036A4 RID: 13988
		Neutral,
		// Token: 0x040036A5 RID: 13989
		Negative,
		// Token: 0x040036A6 RID: 13990
		Seasonal
	}

	// Token: 0x02000693 RID: 1683
	[Serializable]
	public struct FortuneCategory
	{
		// Token: 0x040036A7 RID: 13991
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x040036A8 RID: 13992
		public float weightedChance;

		// Token: 0x040036A9 RID: 13993
		public string[] textResults;
	}

	// Token: 0x02000694 RID: 1684
	public struct FortuneResult
	{
		// Token: 0x060029EE RID: 10734 RVA: 0x000E261B File Offset: 0x000E081B
		public FortuneResult(FortuneResults.FortuneCategoryType fortuneType, int resultIndex)
		{
			this.fortuneType = fortuneType;
			this.resultIndex = resultIndex;
		}

		// Token: 0x040036AA RID: 13994
		public FortuneResults.FortuneCategoryType fortuneType;

		// Token: 0x040036AB RID: 13995
		public int resultIndex;
	}
}
