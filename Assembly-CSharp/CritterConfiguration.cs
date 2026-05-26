using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
[Serializable]
public class CritterConfiguration
{
	// Token: 0x06000114 RID: 276 RVA: 0x0000695C File Offset: 0x00004B5C
	public CritterConfiguration()
	{
		this.animalType = CritterConfiguration.AnimalType.UNKNOWN;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00006988 File Offset: 0x00004B88
	public int GetIndex()
	{
		return CrittersManager.instance.creatureIndex.critterTypes.IndexOf(this);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000069A1 File Offset: 0x00004BA1
	private bool RegionMatches(CrittersRegion region)
	{
		return !region || (region.Biome & this.biome) > (CrittersBiome)0;
	}

	// Token: 0x06000117 RID: 279 RVA: 0x000069BD File Offset: 0x00004BBD
	private bool SpawnCriteriaMatches()
	{
		return !this.spawnCriteria || this.spawnCriteria.CanSpawn();
	}

	// Token: 0x06000118 RID: 280 RVA: 0x000069D9 File Offset: 0x00004BD9
	public bool CanSpawn()
	{
		return this.SpawnCriteriaMatches();
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000069E1 File Offset: 0x00004BE1
	public bool CanSpawn(CrittersRegion region)
	{
		return this.RegionMatches(region) && this.SpawnCriteriaMatches();
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000069F4 File Offset: 0x00004BF4
	public bool DateConditionsMet(DateTime utcDate)
	{
		return !this.dateLimit || this.dateLimit.MatchesDate(utcDate);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00006A11 File Offset: 0x00004C11
	public bool ShouldDespawn()
	{
		return !this.SpawnCriteriaMatches();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00006A1C File Offset: 0x00004C1C
	public void ApplyToCreature(CrittersPawn crittersPawn)
	{
		this.behaviour.ApplyToCritter(crittersPawn);
		if (CrittersManager.instance.LocalAuthority())
		{
			this.ApplyVisualsTo(crittersPawn, true);
			return;
		}
		this.ApplyVisualsTo(crittersPawn, false);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00006A49 File Offset: 0x00004C49
	private void ApplyVisualsTo(CrittersPawn critter, bool generateAppearance = true)
	{
		this.ApplyVisualsTo(critter.visuals, generateAppearance);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00006A58 File Offset: 0x00004C58
	public void ApplyVisualsTo(CritterVisuals visuals, bool generateAppearance = true)
	{
		visuals.critterType = this.GetIndex();
		visuals.ApplyMesh(CritterIndex.GetMesh(this.animalType));
		visuals.ApplyMaterial(this.critterMat);
		if (generateAppearance)
		{
			visuals.SetAppearance(this.GenerateAppearance());
		}
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00006A94 File Offset: 0x00004C94
	public CritterAppearance GenerateAppearance()
	{
		string hatName = "";
		if (Random.value <= this.behaviour.GetTemplateValue<float>("hatChance"))
		{
			GameObject[] templateValue = this.behaviour.GetTemplateValue<GameObject[]>("hats");
			if (!templateValue.IsNullOrEmpty<GameObject>())
			{
				hatName = templateValue[Random.Range(0, templateValue.Length)].name;
			}
		}
		float templateValue2 = this.behaviour.GetTemplateValue<float>("minSize");
		float templateValue3 = this.behaviour.GetTemplateValue<float>("maxSize");
		float size = Random.Range(templateValue2, templateValue3);
		return new CritterAppearance(hatName, size);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00006B1C File Offset: 0x00004D1C
	public override string ToString()
	{
		return string.Format("{0} B:{1} C:{2}", this.critterName, this.behaviour, this.spawnCriteria);
	}

	// Token: 0x04000117 RID: 279
	[Tooltip("Basic internal description of critter.  Could be role, purpose, player experience, etc.")]
	public string internalDescription;

	// Token: 0x04000118 RID: 280
	public string critterName = "UNNAMED CRITTER";

	// Token: 0x04000119 RID: 281
	public CritterConfiguration.AnimalType animalType;

	// Token: 0x0400011A RID: 282
	public CritterTemplate behaviour;

	// Token: 0x0400011B RID: 283
	public CritterSpawnCriteria spawnCriteria;

	// Token: 0x0400011C RID: 284
	public RealWorldDateTimeWindow dateLimit;

	// Token: 0x0400011D RID: 285
	public CrittersBiome biome = CrittersBiome.Any;

	// Token: 0x0400011E RID: 286
	public float spawnWeight = 1f;

	// Token: 0x0400011F RID: 287
	public Material critterMat;

	// Token: 0x02000044 RID: 68
	public enum AnimalType
	{
		// Token: 0x04000121 RID: 289
		Raccoon,
		// Token: 0x04000122 RID: 290
		Cat,
		// Token: 0x04000123 RID: 291
		Bird,
		// Token: 0x04000124 RID: 292
		Goblin,
		// Token: 0x04000125 RID: 293
		Egg,
		// Token: 0x04000126 RID: 294
		UNKNOWN = -1
	}
}
