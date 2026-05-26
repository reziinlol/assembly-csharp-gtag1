using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class CritterIndex : ScriptableObject
{
	// Token: 0x1700001B RID: 27
	public CritterConfiguration this[int index]
	{
		get
		{
			if (index < 0 || index >= this.critterTypes.Count)
			{
				return null;
			}
			return this.critterTypes[index];
		}
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00006B5C File Offset: 0x00004D5C
	private void OnEnable()
	{
		CritterIndex._instance = this;
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00006B64 File Offset: 0x00004D64
	public static Mesh GetMesh(CritterConfiguration.AnimalType animalType)
	{
		if (animalType < CritterConfiguration.AnimalType.Raccoon || animalType >= (CritterConfiguration.AnimalType)CritterIndex._instance.animalMeshes.Count)
		{
			return null;
		}
		return CritterIndex._instance.animalMeshes[(int)animalType].mesh;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00006B93 File Offset: 0x00004D93
	public int GetRandomCritterType(CrittersRegion region = null)
	{
		return this.critterTypes.IndexOf(this.GetRandomConfiguration(region));
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00006BA8 File Offset: 0x00004DA8
	public CritterConfiguration GetRandomConfiguration(CrittersRegion region = null)
	{
		WeightedList<CritterConfiguration> validCritterTypes = this.GetValidCritterTypes(region);
		if (validCritterTypes.Count == 0)
		{
			return null;
		}
		return validCritterTypes.GetRandomItem();
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00006BCD File Offset: 0x00004DCD
	public static DateTime GetCritterDateTime()
	{
		if (!GorillaComputer.instance)
		{
			return DateTime.UtcNow;
		}
		return GorillaComputer.instance.GetServerTime();
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00006BF0 File Offset: 0x00004DF0
	private WeightedList<CritterConfiguration> GetValidCritterTypes(CrittersRegion region = null)
	{
		this._currentConfigs.Clear();
		DateTime critterDateTime = CritterIndex.GetCritterDateTime();
		foreach (CritterConfiguration critterConfiguration in this.critterTypes)
		{
			if (critterConfiguration.DateConditionsMet(critterDateTime) && critterConfiguration.CanSpawn(region))
			{
				this._currentConfigs.Add(critterConfiguration, critterConfiguration.spawnWeight);
			}
		}
		return this._currentConfigs;
	}

	// Token: 0x04000127 RID: 295
	public List<CritterIndex.AnimalTypeMeshEntry> animalMeshes = new List<CritterIndex.AnimalTypeMeshEntry>();

	// Token: 0x04000128 RID: 296
	public List<CritterConfiguration> critterTypes;

	// Token: 0x04000129 RID: 297
	private WeightedList<CritterConfiguration> _currentConfigs = new WeightedList<CritterConfiguration>();

	// Token: 0x0400012A RID: 298
	private static CritterIndex _instance;

	// Token: 0x02000046 RID: 70
	[Serializable]
	public class AnimalTypeMeshEntry
	{
		// Token: 0x0400012B RID: 299
		public CritterConfiguration.AnimalType animalType;

		// Token: 0x0400012C RID: 300
		public Mesh mesh;
	}
}
