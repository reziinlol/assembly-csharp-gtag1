using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200088B RID: 2187
public class GorillaSimpleBackgroundWorkerManager : MonoBehaviour
{
	// Token: 0x06003920 RID: 14624 RVA: 0x00138104 File Offset: 0x00136304
	protected void Awake()
	{
		if (GorillaSimpleBackgroundWorkerManager.hasInstance && GorillaSimpleBackgroundWorkerManager._instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSimpleBackgroundWorkerManager.SetInstance(this);
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x00138127 File Offset: 0x00136327
	private static void SetInstance(GorillaSimpleBackgroundWorkerManager manager)
	{
		GorillaSimpleBackgroundWorkerManager._instance = manager;
		GorillaSimpleBackgroundWorkerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x00138144 File Offset: 0x00136344
	public static void CreateManager()
	{
		GameObject gameObject = new GameObject("GorillaSimpleBackgroundWorkerManager");
		GorillaSimpleBackgroundWorkerManager instance = gameObject.AddComponent<GorillaSimpleBackgroundWorkerManager>();
		Object.DontDestroyOnLoad(gameObject);
		GorillaSimpleBackgroundWorkerManager.SetInstance(instance);
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x0013816D File Offset: 0x0013636D
	public static long DoWork(long ticksOfWork)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		return GorillaSimpleBackgroundWorkerManager._instance._DoWork(ticksOfWork);
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x00138188 File Offset: 0x00136388
	public long _DoWork(long ticksOfWork)
	{
		this.stopwatch.Restart();
		if (ticksOfWork < GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK)
		{
			ticksOfWork = GorillaSimpleBackgroundWorkerManager.MINIMUM_TICKS_OF_WORK;
		}
		while (this.stopwatch.ElapsedTicks < ticksOfWork && this.workerSignups.Count > 0)
		{
			IGorillaSimpleBackgroundWorker gorillaSimpleBackgroundWorker = this.workerSignups.Dequeue();
			if (gorillaSimpleBackgroundWorker != null)
			{
				gorillaSimpleBackgroundWorker.SimpleWork();
			}
		}
		return this.stopwatch.ElapsedTicks;
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x001381EE File Offset: 0x001363EE
	public static void WorkerSignup(IGorillaSimpleBackgroundWorker worker)
	{
		if (!GorillaSimpleBackgroundWorkerManager.hasInstance)
		{
			GorillaSimpleBackgroundWorkerManager.CreateManager();
		}
		GorillaSimpleBackgroundWorkerManager._instance.workerSignups.Enqueue(worker);
	}

	// Token: 0x0400492A RID: 18730
	private static GorillaSimpleBackgroundWorkerManager _instance;

	// Token: 0x0400492B RID: 18731
	private static bool hasInstance = false;

	// Token: 0x0400492C RID: 18732
	private static long MINIMUM_TICKS_OF_WORK = 10000L;

	// Token: 0x0400492D RID: 18733
	public Queue<IGorillaSimpleBackgroundWorker> workerSignups = new Queue<IGorillaSimpleBackgroundWorker>();

	// Token: 0x0400492E RID: 18734
	private Stopwatch stopwatch = new Stopwatch();
}
