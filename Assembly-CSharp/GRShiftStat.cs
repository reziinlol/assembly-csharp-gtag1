using System;
using System.Collections.Generic;
using System.IO;
using GorillaTagScripts.GhostReactor;

// Token: 0x020007DF RID: 2015
public class GRShiftStat
{
	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x0600336B RID: 13163 RVA: 0x0011B53C File Offset: 0x0011973C
	public IReadOnlyDictionary<GREnemyType, int> EnemyKills
	{
		get
		{
			return this.enemyKills;
		}
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x0011B544 File Offset: 0x00119744
	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.GetShiftStat(GRShiftStatType.EnemyDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.PlayerDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.CoresCollected));
		writer.Write(this.GetShiftStat(GRShiftStatType.SentientCoresCollected));
		writer.Write(this.enemyKills.Count);
		foreach (KeyValuePair<GREnemyType, int> keyValuePair in this.enemyKills)
		{
			writer.Write((int)keyValuePair.Key);
			writer.Write(keyValuePair.Value);
		}
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x0011B5F0 File Offset: 0x001197F0
	public void Deserialize(BinaryReader reader)
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.PlayerDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.CoresCollected] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GREnemyType key = (GREnemyType)reader.ReadInt32();
			this.enemyKills[key] = reader.ReadInt32();
		}
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x0011B671 File Offset: 0x00119871
	public void SetShiftStat(GRShiftStatType stat, int newValue)
	{
		this.shiftStats[stat] = newValue;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x0011B690 File Offset: 0x00119890
	public void IncrementShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			Dictionary<GRShiftStatType, int> dictionary = this.shiftStats;
			int num = dictionary[stat];
			dictionary[stat] = num + 1;
			return;
		}
		this.shiftStats[stat] = 1;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x0011B6E4 File Offset: 0x001198E4
	public void IncrementEnemyKills(GREnemyType type)
	{
		if (type == GREnemyType.None)
		{
			return;
		}
		if (!this.enemyKills.TryAdd(type, 1))
		{
			Dictionary<GREnemyType, int> dictionary = this.enemyKills;
			int num = dictionary[type];
			dictionary[type] = num + 1;
		}
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x0011B72C File Offset: 0x0011992C
	public void ResetShiftStats()
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = 0;
		this.shiftStats[GRShiftStatType.PlayerDeaths] = 0;
		this.shiftStats[GRShiftStatType.CoresCollected] = 0;
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = 0;
		this.enemyKills.Clear();
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x0011B787 File Offset: 0x00119987
	public int GetShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			return this.shiftStats[stat];
		}
		return 0;
	}

	// Token: 0x04004326 RID: 17190
	public Dictionary<GRShiftStatType, int> shiftStats = new Dictionary<GRShiftStatType, int>();

	// Token: 0x04004327 RID: 17191
	private Dictionary<GREnemyType, int> enemyKills = new Dictionary<GREnemyType, int>();
}
