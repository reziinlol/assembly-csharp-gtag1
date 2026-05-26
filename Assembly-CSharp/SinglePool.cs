using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D61 RID: 3425
[Serializable]
public class SinglePool : IGorillaSimpleBackgroundWorker
{
	// Token: 0x0600542F RID: 21551 RVA: 0x001B8230 File Offset: 0x001B6430
	public void SimpleWork()
	{
		int count = this.inactivePool.Count;
		if (count >= this.initAmountToPool)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
		gameObject.name = this.objectToPool.name + "(PoolIndex=" + count.ToString() + ")";
		gameObject.SetActive(false);
		this.inactivePool.Push(gameObject);
		this.amountAllocatedToPool++;
		int instanceID = gameObject.GetInstanceID();
		this.pooledObjects.Add(instanceID);
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x06005430 RID: 21552 RVA: 0x001B82CD File Offset: 0x001B64CD
	private void PrivAllocPooledObjects()
	{
		if (this.inactivePool.Count == 0)
		{
			this.SimpleWork();
			return;
		}
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x06005431 RID: 21553 RVA: 0x001B82E9 File Offset: 0x001B64E9
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x06005432 RID: 21554 RVA: 0x001B8328 File Offset: 0x001B6528
	public GameObject Instantiate(bool setActive = true)
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(setActive);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x06005433 RID: 21555 RVA: 0x001B8390 File Offset: 0x001B6590
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x06005434 RID: 21556 RVA: 0x001B83E2 File Offset: 0x001B65E2
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x06005435 RID: 21557 RVA: 0x001B83EF File Offset: 0x001B65EF
	public int GetTotalCount()
	{
		return this.pooledObjects.Count;
	}

	// Token: 0x06005436 RID: 21558 RVA: 0x001B83FC File Offset: 0x001B65FC
	public int GetActiveCount()
	{
		return this.activePool.Count;
	}

	// Token: 0x06005437 RID: 21559 RVA: 0x001B8409 File Offset: 0x001B6609
	public int GetInactiveCount()
	{
		return this.inactivePool.Count;
	}

	// Token: 0x04006507 RID: 25863
	public GameObject objectToPool;

	// Token: 0x04006508 RID: 25864
	public int initAmountToPool = 8;

	// Token: 0x04006509 RID: 25865
	private HashSet<int> pooledObjects;

	// Token: 0x0400650A RID: 25866
	private Stack<GameObject> inactivePool;

	// Token: 0x0400650B RID: 25867
	private Dictionary<int, GameObject> activePool;

	// Token: 0x0400650C RID: 25868
	private GameObject gameObject;

	// Token: 0x0400650D RID: 25869
	private int amountAllocatedToPool;
}
