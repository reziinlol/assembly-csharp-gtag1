using System;
using UnityEngine;

// Token: 0x02000924 RID: 2340
[Serializable]
public class RankedMultiplayerStatisticString : RankedMultiplayerStatistic
{
	// Token: 0x06003D2A RID: 15658 RVA: 0x0014C723 File Offset: 0x0014A923
	public RankedMultiplayerStatisticString(string n, string val, RankedMultiplayerStatistic.SerializationType s = RankedMultiplayerStatistic.SerializationType.None) : base(n, s)
	{
		this.stringValue = val;
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x0014C734 File Offset: 0x0014A934
	public static implicit operator string(RankedMultiplayerStatisticString stat)
	{
		if (stat.IsValid)
		{
			return stat.stringValue;
		}
		Debug.LogError("Attempting to retrieve value for user data that does not yet have a valid key: " + stat.name);
		return string.Empty;
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x0014C75F File Offset: 0x0014A95F
	public void Set(string val)
	{
		this.stringValue = val;
		this.Save();
	}

	// Token: 0x06003D2D RID: 15661 RVA: 0x0014C76E File Offset: 0x0014A96E
	public string Get()
	{
		return this.stringValue;
	}

	// Token: 0x06003D2E RID: 15662 RVA: 0x0014C776 File Offset: 0x0014A976
	public override bool TrySetValue(string valAsString)
	{
		this.stringValue = valAsString;
		return true;
	}

	// Token: 0x06003D2F RID: 15663 RVA: 0x0014C780 File Offset: 0x0014A980
	protected override void Save()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership && serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
		{
			PlayerPrefs.SetString(this.name, this.stringValue);
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x0014C7B4 File Offset: 0x0014A9B4
	public override void Load()
	{
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
		if (serializationType != RankedMultiplayerStatistic.SerializationType.Mothership)
		{
			if (serializationType == RankedMultiplayerStatistic.SerializationType.PlayerPrefs)
			{
				base.IsValid = true;
				this.stringValue = PlayerPrefs.GetString(this.name, this.stringValue);
				return;
			}
		}
		else
		{
			base.IsValid = false;
		}
	}

	// Token: 0x06003D31 RID: 15665 RVA: 0x0014C76E File Offset: 0x0014A96E
	public override string ToString()
	{
		return this.stringValue;
	}

	// Token: 0x04004DB1 RID: 19889
	private string stringValue;
}
