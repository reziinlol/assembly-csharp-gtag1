using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class SpawnRegion<TItem, TRegion> : MonoBehaviour where TItem : Object where TRegion : SpawnRegion<TItem, TRegion>
{
	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060009EA RID: 2538 RVA: 0x00035720 File Offset: 0x00033920
	public static List<TRegion> Regions
	{
		get
		{
			return SpawnRegion<TItem, TRegion>._regions;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060009EB RID: 2539 RVA: 0x00035727 File Offset: 0x00033927
	// (set) Token: 0x060009EC RID: 2540 RVA: 0x0003572F File Offset: 0x0003392F
	public int MaxItems { get; private set; } = 10;

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060009ED RID: 2541 RVA: 0x00035738 File Offset: 0x00033938
	private bool HasSpawnOrigins
	{
		get
		{
			Transform[] array = this.spawnOrigins;
			return array != null && array.Length != 0;
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060009EE RID: 2542 RVA: 0x0003574A File Offset: 0x0003394A
	public List<TItem> Items
	{
		get
		{
			return this._items;
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060009EF RID: 2543 RVA: 0x00035752 File Offset: 0x00033952
	public int ItemCount
	{
		get
		{
			return this._items.Count;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x060009F0 RID: 2544 RVA: 0x0003575F File Offset: 0x0003395F
	// (set) Token: 0x060009F1 RID: 2545 RVA: 0x00035767 File Offset: 0x00033967
	public int ID { get; private set; }

	// Token: 0x060009F2 RID: 2546 RVA: 0x00035770 File Offset: 0x00033970
	private void OnEnable()
	{
		Transform[] array = this.spawnOrigins;
		this._useSpawnOrigins = (array != null && array.Length != 0);
		this._testAgainstGeo = (!this._useSpawnOrigins && this.geoTestPoint);
		if (this._testAgainstGeo && this._hitTestBuffer == null)
		{
			this._hitTestBuffer = new RaycastHit[20];
		}
		SpawnRegion<TItem, TRegion>.RegisterRegion((TRegion)((object)this));
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x000357D8 File Offset: 0x000339D8
	private void OnDisable()
	{
		SpawnRegion<TItem, TRegion>.UnregisterRegion((TRegion)((object)this));
		foreach (TItem titem in this._items)
		{
			if (titem)
			{
				SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(titem);
			}
		}
		this._items.Clear();
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00035854 File Offset: 0x00033A54
	private static void RegisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup[region.ID] = region;
		SpawnRegion<TItem, TRegion>._regions.Add(region);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00035877 File Offset: 0x00033A77
	private static void UnregisterRegion(TRegion region)
	{
		SpawnRegion<TItem, TRegion>._regionLookup.Remove(region.ID);
		SpawnRegion<TItem, TRegion>._regions.Remove(region);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x0003589C File Offset: 0x00033A9C
	public static void AddItemToRegion(TItem item, int regionId)
	{
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(regionId, out tregion))
		{
			tregion.AddItem(item);
		}
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x000358C4 File Offset: 0x00033AC4
	public static void RemoveItemFromRegion(TItem item)
	{
		int key;
		TRegion tregion;
		if (SpawnRegion<TItem, TRegion>._itemRegionLookup.TryGetValue(item, out key) && SpawnRegion<TItem, TRegion>._regionLookup.TryGetValue(key, out tregion))
		{
			tregion.RemoveItem(item);
		}
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x000358FB File Offset: 0x00033AFB
	public void AddItem(TItem item)
	{
		this._items.Add(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup[item] = this.ID;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0003591A File Offset: 0x00033B1A
	public void RemoveItem(TItem item)
	{
		this._items.Remove(item);
		SpawnRegion<TItem, TRegion>._itemRegionLookup.Remove(item);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00035938 File Offset: 0x00033B38
	[return: TupleElementNames(new string[]
	{
		"isOnGround",
		"position",
		"normal"
	})]
	public ValueTuple<bool, Vector3, Vector3> GetSpawnPointWithNormal(int maxTries = 5)
	{
		for (int i = 0; i < maxTries; i++)
		{
			RaycastHit raycastHit;
			if (this.TryGetSpawnPoint(out raycastHit))
			{
				return new ValueTuple<bool, Vector3, Vector3>(true, raycastHit.point, raycastHit.normal);
			}
		}
		float num = this._scale / 2f;
		Vector3 item = base.transform.TransformPoint(new Vector3(Random.Range(-num, num), num, Random.Range(-num, num)));
		return new ValueTuple<bool, Vector3, Vector3>(false, item, Vector3.up);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x000359AC File Offset: 0x00033BAC
	private bool TryGetSpawnPoint(out RaycastHit spawnPoint)
	{
		float num = base.transform.lossyScale.y * this._scale;
		if (this._useSpawnOrigins)
		{
			Vector3 vector = this.spawnOrigins[Random.Range(0, this.spawnOrigins.Length)].position;
			if (this.TryGetSpawnPoint(vector, Random.onUnitSphere, Mathf.Max(num, 100f), out spawnPoint))
			{
				return spawnPoint.normal.y > 0f || this.TryGetSpawnPoint(spawnPoint.point, Vector3.down, num, out spawnPoint);
			}
			spawnPoint = default(RaycastHit);
			return false;
		}
		else
		{
			float num2 = this._scale / 2f;
			Vector3 vector = base.transform.TransformPoint(new Vector3(Random.Range(-num2, num2), num2, Random.Range(-num2, num2)));
			if (this._testAgainstGeo && this.IsInsideGeo(vector))
			{
				spawnPoint = default(RaycastHit);
				return false;
			}
			return this.TryGetSpawnPoint(vector, Vector3.down, num, out spawnPoint);
		}
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00035A98 File Offset: 0x00033C98
	private bool TryGetSpawnPoint(Vector3 origin, Vector3 direction, float distance, out RaycastHit spawnPoint)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, direction, out raycastHit, distance, -1, QueryTriggerInteraction.Ignore))
		{
			Debug.DrawLine(origin, raycastHit.point, Color.green, 5f);
			spawnPoint = raycastHit;
			return true;
		}
		Debug.DrawLine(origin, origin + direction * distance, Color.red, 5f);
		spawnPoint = default(RaycastHit);
		return false;
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00035AFC File Offset: 0x00033CFC
	private bool IsInsideGeo(Vector3 point)
	{
		Vector3 position = this.geoTestPoint.position;
		Vector3 vector = position - point;
		int num;
		int num2;
		for (;;)
		{
			num = Physics.RaycastNonAlloc(point, vector, this._hitTestBuffer, vector.magnitude, -1, QueryTriggerInteraction.Ignore);
			num2 = Physics.RaycastNonAlloc(position, -vector, this._hitTestBuffer, vector.magnitude, -1, QueryTriggerInteraction.Ignore);
			if (num < this._hitTestBuffer.Length && num2 < this._hitTestBuffer.Length)
			{
				break;
			}
			this._hitTestBuffer = new RaycastHit[this._hitTestBuffer.Length * 2];
		}
		bool flag = (num + num2) % 2 != 0;
		Debug.DrawLine(point, position, flag ? Color.red : Color.green, 5f);
		return flag;
	}

	// Token: 0x04000C40 RID: 3136
	private static List<TRegion> _regions = new List<TRegion>();

	// Token: 0x04000C41 RID: 3137
	private static Dictionary<int, TRegion> _regionLookup = new Dictionary<int, TRegion>();

	// Token: 0x04000C42 RID: 3138
	private static Dictionary<TItem, int> _itemRegionLookup = new Dictionary<TItem, int>();

	// Token: 0x04000C43 RID: 3139
	[SerializeField]
	private float _scale = 10f;

	// Token: 0x04000C45 RID: 3141
	[SerializeField]
	[Tooltip("If set, spawn points will be created via raycasts from one of these points.")]
	private Transform[] spawnOrigins;

	// Token: 0x04000C46 RID: 3142
	[SerializeField]
	[Tooltip("If set, all spawn points will be tested against this transform to see if they're inside geo.  Ignored if spawn origins are configured.")]
	private Transform geoTestPoint;

	// Token: 0x04000C47 RID: 3143
	private List<TItem> _items = new List<TItem>();

	// Token: 0x04000C49 RID: 3145
	private bool _useSpawnOrigins;

	// Token: 0x04000C4A RID: 3146
	private bool _testAgainstGeo;

	// Token: 0x04000C4B RID: 3147
	private RaycastHit[] _hitTestBuffer;
}
