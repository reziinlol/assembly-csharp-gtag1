using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class CrittersRegion : MonoBehaviour
{
	// Token: 0x17000033 RID: 51
	// (get) Token: 0x060002D9 RID: 729 RVA: 0x0001138F File Offset: 0x0000F58F
	public static List<CrittersRegion> Regions
	{
		get
		{
			return CrittersRegion._regions;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x060002DA RID: 730 RVA: 0x00011396 File Offset: 0x0000F596
	public int CritterCount
	{
		get
		{
			return this._critters.Count;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x060002DB RID: 731 RVA: 0x000113A3 File Offset: 0x0000F5A3
	// (set) Token: 0x060002DC RID: 732 RVA: 0x000113AB File Offset: 0x0000F5AB
	public int ID { get; private set; }

	// Token: 0x060002DD RID: 733 RVA: 0x000113B4 File Offset: 0x0000F5B4
	private void OnEnable()
	{
		CrittersRegion.RegisterRegion(this);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x000113BC File Offset: 0x0000F5BC
	private void OnDisable()
	{
		CrittersRegion.UnregisterRegion(this);
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000113C4 File Offset: 0x0000F5C4
	private static void RegisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup[region.ID] = region;
		CrittersRegion._regions.Add(region);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x000113E2 File Offset: 0x0000F5E2
	private static void UnregisterRegion(CrittersRegion region)
	{
		CrittersRegion._regionLookup.Remove(region.ID);
		CrittersRegion._regions.Remove(region);
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00011404 File Offset: 0x0000F604
	public static void AddCritterToRegion(CrittersPawn critter, int regionId)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(regionId, out crittersRegion))
		{
			crittersRegion.AddCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Attempted to add critter to non-existing region {0}.", regionId), null);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00011440 File Offset: 0x0000F640
	public static void RemoveCritterFromRegion(CrittersPawn critter)
	{
		CrittersRegion crittersRegion;
		if (CrittersRegion._regionLookup.TryGetValue(critter.regionId, out crittersRegion))
		{
			crittersRegion.RemoveCritter(critter);
			return;
		}
		GTDev.LogError<string>(string.Format("Couldn't find region with id {0}", critter.regionId), null);
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00011484 File Offset: 0x0000F684
	public void AddCritter(CrittersPawn pawn)
	{
		this._critters.Add(pawn);
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x00011492 File Offset: 0x0000F692
	public void RemoveCritter(CrittersPawn pawn)
	{
		this._critters.Remove(pawn);
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x000114A4 File Offset: 0x0000F6A4
	public Vector3 GetSpawnPoint()
	{
		float num = this.scale / 2f;
		float num2 = base.transform.lossyScale.y * this.scale;
		Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, -base.transform.up, out raycastHit, num2, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(vector, raycastHit.point, Color.green, 5f);
			return raycastHit.point;
		}
		Debug.DrawLine(vector, vector - base.transform.up * num2, Color.red, 5f);
		return vector;
	}

	// Token: 0x04000349 RID: 841
	private static List<CrittersRegion> _regions = new List<CrittersRegion>();

	// Token: 0x0400034A RID: 842
	private static Dictionary<int, CrittersRegion> _regionLookup = new Dictionary<int, CrittersRegion>();

	// Token: 0x0400034B RID: 843
	public CrittersBiome Biome = CrittersBiome.Any;

	// Token: 0x0400034C RID: 844
	public int maxCritters = 10;

	// Token: 0x0400034D RID: 845
	public float scale = 10f;

	// Token: 0x0400034E RID: 846
	public List<CrittersPawn> _critters = new List<CrittersPawn>();
}
