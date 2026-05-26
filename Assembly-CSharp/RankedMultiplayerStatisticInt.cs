using System;
using UnityEngine;

// Token: 0x02000922 RID: 2338
[Serializable]
public class RankedMultiplayerStatisticInt : RankedMultiplayerStatistic
{
	// Token: 0x06003D16 RID: 15638 RVA: 0x0014C459 File Offset: 0x0014A659
	public RankedMultiplayerStatisticInt(string n, int val, int min = 0, int max = 2147483647, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.intValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003D17 RID: 15639 RVA: 0x0014C47A File Offset: 0x0014A67A
	public static implicit operator int(RankedMultiplayerStatisticInt stat)
	{
		if (stat.IsValid)
		{
			return stat.intValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0;
	}

	// Token: 0x06003D18 RID: 15640 RVA: 0x0014C4A1 File Offset: 0x0014A6A1
	public void Set(int val)
	{
		this.intValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003D19 RID: 15641 RVA: 0x0014C4C1 File Offset: 0x0014A6C1
	public int Get()
	{
		return this.intValue;
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x0014C4CC File Offset: 0x0014A6CC
	public override bool TrySetValue(string valAsString)
	{
		int value;
		bool flag = int.TryParse(valAsString, out value);
		if (flag)
		{
			this.intValue = Mathf.Clamp(value, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x0014C4FC File Offset: 0x0014A6FC
	public void Increment()
	{
		this.AddTo(1);
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x0014C505 File Offset: 0x0014A705
	public void AddTo(int amount)
	{
		this.intValue += amount;
		this.intValue = Mathf.Clamp(this.intValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x0014C538 File Offset: 0x0014A738
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetInt(this.name, this.intValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003D1E RID: 15646 RVA: 0x0014C56C File Offset: 0x0014A76C
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.intValue = PlayerPrefs.GetInt(this.name, this.intValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003D1F RID: 15647 RVA: 0x0014C5AE File Offset: 0x0014A7AE
	public override string ToString()
	{
		return this.intValue.ToString();
	}

	// Token: 0x04004DAB RID: 19883
	private int intValue;

	// Token: 0x04004DAC RID: 19884
	private int minValue;

	// Token: 0x04004DAD RID: 19885
	private int maxValue;
}
