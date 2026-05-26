using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class CrittersPool : MonoBehaviour
{
	// Token: 0x060002D1 RID: 721 RVA: 0x000111A8 File Offset: 0x0000F3A8
	public static GameObject GetPooled(GameObject prefab)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return null;
		}
		return crittersPool.GetInstance(prefab);
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x000111BB File Offset: 0x0000F3BB
	public static void Return(GameObject pooledGO)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return;
		}
		crittersPool.ReturnInstance(pooledGO);
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x000111CD File Offset: 0x0000F3CD
	private void Awake()
	{
		if (CrittersPool.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		CrittersPool.instance = this;
		this.SetupPools();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000111F0 File Offset: 0x0000F3F0
	private void SetupPools()
	{
		this.pools = new Dictionary<GameObject, List<GameObject>>();
		this.poolParent = new GameObject("CrittersPool")
		{
			transform = 
			{
				parent = base.transform
			}
		}.transform;
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			CrittersPool.CrittersPoolSettings crittersPoolSettings = this.eventEffects[i];
			if (crittersPoolSettings.poolObject == null || crittersPoolSettings.poolSize <= 0)
			{
				GTDev.Log<string>("CrittersPool.SetupPools Failed. Pool has no poolObject or has size 0.", null);
			}
			else
			{
				List<GameObject> list = new List<GameObject>();
				for (int j = 0; j < crittersPoolSettings.poolSize; j++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(crittersPoolSettings.poolObject);
					gameObject.transform.SetParent(this.poolParent);
					GameObject gameObject2 = gameObject;
					gameObject2.name += j.ToString();
					gameObject.SetActive(false);
					list.Add(gameObject);
				}
				this.pools.Add(crittersPoolSettings.poolObject, list);
			}
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x000112EC File Offset: 0x0000F4EC
	private GameObject GetInstance(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pools.TryGetValue(prefab, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && !list[i].activeSelf)
				{
					list[i].SetActive(true);
					return list[i];
				}
			}
			GTDev.Log<string>("CrittersPool.GetInstance Failed. No available instance.", null);
			return null;
		}
		GTDev.LogError<string>("CrittersPool.GetInstance Failed. Prefab doesn't have a valid pool setup.", null);
		return null;
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x00011365 File Offset: 0x0000F565
	private void ReturnInstance(GameObject instance)
	{
		instance.transform.SetParent(this.poolParent);
		instance.SetActive(false);
	}

	// Token: 0x04000343 RID: 835
	private static CrittersPool instance;

	// Token: 0x04000344 RID: 836
	public CrittersPool.CrittersPoolSettings[] eventEffects;

	// Token: 0x04000345 RID: 837
	private Dictionary<GameObject, List<GameObject>> pools;

	// Token: 0x04000346 RID: 838
	public Transform poolParent;

	// Token: 0x02000074 RID: 116
	[Serializable]
	public class CrittersPoolSettings
	{
		// Token: 0x04000347 RID: 839
		public GameObject poolObject;

		// Token: 0x04000348 RID: 840
		public int poolSize = 20;
	}
}
