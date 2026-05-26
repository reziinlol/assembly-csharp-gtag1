using System;

// Token: 0x02000920 RID: 2336
public abstract class RankedMultiplayerStatistic
{
	// Token: 0x06003D0A RID: 15626 RVA: 0x0005A99D File Offset: 0x00058B9D
	public override string ToString()
	{
		return string.Empty;
	}

	// Token: 0x06003D0B RID: 15627
	public abstract void Load();

	// Token: 0x06003D0C RID: 15628
	protected abstract void Save();

	// Token: 0x06003D0D RID: 15629
	public abstract bool TrySetValue(string valAsString);

	// Token: 0x06003D0E RID: 15630 RVA: 0x0014C387 File Offset: 0x0014A587
	public virtual string WriteToJson()
	{
		return string.Format("{{{0}:\"{1}\"}}", this.name, this.ToString());
	}

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x06003D0F RID: 15631 RVA: 0x0014C39F File Offset: 0x0014A59F
	// (set) Token: 0x06003D10 RID: 15632 RVA: 0x0014C3A7 File Offset: 0x0014A5A7
	public bool IsValid { get; protected set; }

	// Token: 0x06003D11 RID: 15633 RVA: 0x0014C3B0 File Offset: 0x0014A5B0
	public RankedMultiplayerStatistic(string n, RankedMultiplayerStatistic.SerializationType sType = RankedMultiplayerStatistic.SerializationType.Mothership)
	{
		this.serializationType = sType;
		this.name = n;
		this.IsValid = (this.serializationType != RankedMultiplayerStatistic.SerializationType.Mothership);
		RankedMultiplayerStatistic.SerializationType serializationType = this.serializationType;
	}

	// Token: 0x06003D12 RID: 15634 RVA: 0x0014C3E8 File Offset: 0x0014A5E8
	protected virtual void HandleUserDataSetSuccess(string keyName)
	{
		if (keyName == this.name)
		{
			this.IsValid = true;
		}
	}

	// Token: 0x06003D13 RID: 15635 RVA: 0x0014C3FF File Offset: 0x0014A5FF
	protected virtual void HandleUserDataGetSuccess(string keyName, string value)
	{
		if (keyName == this.name)
		{
			if (this.TrySetValue(value))
			{
				this.IsValid = true;
				return;
			}
			this.Save();
		}
	}

	// Token: 0x06003D14 RID: 15636 RVA: 0x0014C426 File Offset: 0x0014A626
	protected void HandleUserDataGetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
			this.IsValid = true;
		}
	}

	// Token: 0x06003D15 RID: 15637 RVA: 0x0014C443 File Offset: 0x0014A643
	protected void HandleUserDataSetFailure(string keyName)
	{
		if (keyName == this.name)
		{
			this.Save();
		}
	}

	// Token: 0x04004DA4 RID: 19876
	protected RankedMultiplayerStatistic.SerializationType serializationType = RankedMultiplayerStatistic.SerializationType.Mothership;

	// Token: 0x04004DA5 RID: 19877
	public string name;

	// Token: 0x02000921 RID: 2337
	public enum SerializationType
	{
		// Token: 0x04004DA8 RID: 19880
		None,
		// Token: 0x04004DA9 RID: 19881
		Mothership,
		// Token: 0x04004DAA RID: 19882
		PlayerPrefs
	}
}
