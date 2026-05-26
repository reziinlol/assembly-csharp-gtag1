using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D62 RID: 3426
public class ObjectPools : MonoBehaviour, IBuildValidation
{
	// Token: 0x170007F0 RID: 2032
	// (get) Token: 0x06005439 RID: 21561 RVA: 0x001B8425 File Offset: 0x001B6625
	// (set) Token: 0x0600543A RID: 21562 RVA: 0x001B842D File Offset: 0x001B662D
	public bool initialized { get; private set; }

	// Token: 0x0600543B RID: 21563 RVA: 0x001B8436 File Offset: 0x001B6636
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x0600543C RID: 21564 RVA: 0x001B843E File Offset: 0x001B663E
	protected void Start()
	{
		this.InitializePools();
	}

	// Token: 0x0600543D RID: 21565 RVA: 0x001B8448 File Offset: 0x001B6648
	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x0600543E RID: 21566 RVA: 0x001B857C File Offset: 0x001B677C
	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	// Token: 0x0600543F RID: 21567 RVA: 0x001B858A File Offset: 0x001B678A
	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	// Token: 0x06005440 RID: 21568 RVA: 0x001B8598 File Offset: 0x001B6798
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x06005441 RID: 21569 RVA: 0x001B85A8 File Offset: 0x001B67A8
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	// Token: 0x06005442 RID: 21570 RVA: 0x001B85C3 File Offset: 0x001B67C3
	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	// Token: 0x06005443 RID: 21571 RVA: 0x001B85D2 File Offset: 0x001B67D2
	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	// Token: 0x06005444 RID: 21572 RVA: 0x001B85E1 File Offset: 0x001B67E1
	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06005445 RID: 21573 RVA: 0x001B85F7 File Offset: 0x001B67F7
	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06005446 RID: 21574 RVA: 0x001B860F File Offset: 0x001B680F
	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06005447 RID: 21575 RVA: 0x001B8625 File Offset: 0x001B6825
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06005448 RID: 21576 RVA: 0x001B863D File Offset: 0x001B683D
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06005449 RID: 21577 RVA: 0x001B866C File Offset: 0x001B686C
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x0600544A RID: 21578 RVA: 0x001B867C File Offset: 0x001B687C
	public bool BuildValidationCheck()
	{
		bool result = true;
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SinglePool pool = enumerator.Current;
				if (pool.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					result = false;
				}
				else
				{
					DelayedDestroyPooledObj[] componentsInChildren = pool.objectToPool.GetComponentsInChildren<DelayedDestroyPooledObj>(true);
					if (componentsInChildren.Length > 1)
					{
						Debug.LogError(string.Format("Pooled prefab '{0}' has {1} ", pool.objectToPool.name, componentsInChildren.Length) + "DelayedDestroyPooledObj components in its hierarchy. Only the root should have one. Children with their own will try to pool-destroy themselves and spam 'not contained in the activePool' errors. Extra components on:" + string.Concat(Array.ConvertAll<DelayedDestroyPooledObj, string>(componentsInChildren, delegate(DelayedDestroyPooledObj c)
						{
							if (!(c.gameObject == pool.objectToPool))
							{
								return "\n  - " + c.gameObject.name;
							}
							return "";
						})), pool.objectToPool);
						result = false;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600544B RID: 21579 RVA: 0x001B876C File Offset: 0x001B696C
	public static int InstantiateDelayed(GameObject prefab, Vector3 pos, float delay)
	{
		return ObjectPools.InstantiateDelayed(prefab, null, pos, delay);
	}

	// Token: 0x0600544C RID: 21580 RVA: 0x001B8778 File Offset: 0x001B6978
	public static int InstantiateDelayed(GameObject prefab, Transform xform, Vector3 localPos, float delay)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return -1;
		}
		int num;
		if (ObjectPools._delayedFreeHead >= 0)
		{
			num = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = ObjectPools._delayedFreeNext[num];
		}
		else
		{
			if (ObjectPools._delayedHighWater >= ObjectPools._delayedData.Length)
			{
				int newSize = ObjectPools._delayedData.Length * 2;
				Array.Resize<ObjectPools.DelayedSpawnData>(ref ObjectPools._delayedData, newSize);
				Array.Resize<int>(ref ObjectPools._delayedFreeNext, newSize);
			}
			num = ObjectPools._delayedHighWater++;
		}
		ObjectPools._delayedData[num] = new ObjectPools.DelayedSpawnData
		{
			prefabHash = PoolUtils.GameObjHashCode(prefab),
			xform = xform,
			pos = localPos
		};
		GTDelayedExec.Add(ObjectPools._delayedListener, delay, num);
		return num;
	}

	// Token: 0x0600544D RID: 21581 RVA: 0x001B8824 File Offset: 0x001B6A24
	public static void UpdateDelayedInstantiate(int idx, Transform xform)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].xform = xform;
	}

	// Token: 0x0600544E RID: 21582 RVA: 0x001B8840 File Offset: 0x001B6A40
	public static void UpdateDelayedInstantiate(int idx, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].pos = localPos;
	}

	// Token: 0x0600544F RID: 21583 RVA: 0x001B885C File Offset: 0x001B6A5C
	public static void CancelDelayedInstantiate(int idx)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].prefabHash = 0;
	}

	// Token: 0x06005450 RID: 21584 RVA: 0x001B8878 File Offset: 0x001B6A78
	public static void UpdateDelayedInstantiate(int idx, Transform xform, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools.DelayedSpawnData[] delayedData = ObjectPools._delayedData;
		delayedData[idx].xform = xform;
		delayedData[idx].pos = localPos;
	}

	// Token: 0x0400650E RID: 25870
	public static ObjectPools instance = null;

	// Token: 0x04006510 RID: 25872
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04006511 RID: 25873
	private Dictionary<int, SinglePool> lookUp;

	// Token: 0x04006512 RID: 25874
	private const int k_delayedInitialCount = 16;

	// Token: 0x04006513 RID: 25875
	[OnEnterPlay_Set(0)]
	private static int _delayedHighWater;

	// Token: 0x04006514 RID: 25876
	[OnEnterPlay_Set(-1)]
	private static int _delayedFreeHead = -1;

	// Token: 0x04006515 RID: 25877
	[OnEnterPlay_SetNew]
	private static ObjectPools.DelayedSpawnData[] _delayedData = new ObjectPools.DelayedSpawnData[16];

	// Token: 0x04006516 RID: 25878
	[OnEnterPlay_SetNew]
	private static int[] _delayedFreeNext = new int[16];

	// Token: 0x04006517 RID: 25879
	[OnEnterPlay_SetNew]
	private static readonly ObjectPools.DelayedSpawnListener _delayedListener = new ObjectPools.DelayedSpawnListener();

	// Token: 0x02000D63 RID: 3427
	private struct DelayedSpawnData
	{
		// Token: 0x04006518 RID: 25880
		public int prefabHash;

		// Token: 0x04006519 RID: 25881
		public Transform xform;

		// Token: 0x0400651A RID: 25882
		public Vector3 pos;
	}

	// Token: 0x02000D64 RID: 3428
	private class DelayedSpawnListener : IDelayedExecListener
	{
		// Token: 0x06005453 RID: 21587 RVA: 0x001B88CC File Offset: 0x001B6ACC
		public void OnDelayedAction(int contextId)
		{
			if (contextId >= ObjectPools._delayedHighWater)
			{
				return;
			}
			ref ObjectPools.DelayedSpawnData ptr = ref ObjectPools._delayedData[contextId];
			if (ptr.prefabHash != 0 && ObjectPools.instance != null)
			{
				Vector3 position = (ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos;
				ObjectPools.instance.Instantiate(ptr.prefabHash, position, true);
			}
			ptr = default(ObjectPools.DelayedSpawnData);
			ObjectPools._delayedFreeNext[contextId] = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = contextId;
		}
	}
}
