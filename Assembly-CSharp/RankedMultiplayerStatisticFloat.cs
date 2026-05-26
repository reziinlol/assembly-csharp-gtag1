using System;
using UnityEngine;

// Token: 0x02000923 RID: 2339
[Serializable]
public class RankedMultiplayerStatisticFloat : RankedMultiplayerStatistic
{
	// Token: 0x06003D20 RID: 15648 RVA: 0x0014C5BB File Offset: 0x0014A7BB
	public RankedMultiplayerStatisticFloat(string n, float val, float min = 0f, float max = 3.4028235E+38f, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.floatValue = val;
		this.minValue = min;
		this.maxValue = max;
	}

	// Token: 0x06003D21 RID: 15649 RVA: 0x0014C5DC File Offset: 0x0014A7DC
	public static implicit operator float(RankedMultiplayerStatisticFloat stat)
	{
		if (stat.IsValid)
		{
			return stat.floatValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return 0f;
	}

	// Token: 0x06003D22 RID: 15650 RVA: 0x0014C607 File Offset: 0x0014A807
	public void Set(float val)
	{
		this.floatValue = Mathf.Clamp(val, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003D23 RID: 15651 RVA: 0x0014C627 File Offset: 0x0014A827
	public float Get()
	{
		return this.floatValue;
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x0014C630 File Offset: 0x0014A830
	public override bool TrySetValue(string valAsString)
	{
		float value;
		bool flag = float.TryParse(valAsString, out value);
		if (flag)
		{
			this.floatValue = Mathf.Clamp(value, this.minValue, this.maxValue);
		}
		return flag;
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x0014C660 File Offset: 0x0014A860
	public void Increment()
	{
		this.AddTo(1f);
	}

	// Token: 0x06003D26 RID: 15654 RVA: 0x0014C66D File Offset: 0x0014A86D
	public void AddTo(float amount)
	{
		this.floatValue += amount;
		this.floatValue = Mathf.Clamp(this.floatValue, this.minValue, this.maxValue);
		this.Save();
	}

	// Token: 0x06003D27 RID: 15655 RVA: 0x0014C6A0 File Offset: 0x0014A8A0
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetFloat(this.name, this.floatValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003D28 RID: 15656 RVA: 0x0014C6D4 File Offset: 0x0014A8D4
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.floatValue = PlayerPrefs.GetFloat(this.name, this.floatValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003D29 RID: 15657 RVA: 0x0014C716 File Offset: 0x0014A916
	public override string ToString()
	{
		return this.floatValue.ToString();
	}

	// Token: 0x04004DAE RID: 19886
	private float floatValue;

	// Token: 0x04004DAF RID: 19887
	private float minValue;

	// Token: 0x04004DB0 RID: 19888
	private float maxValue;
}
