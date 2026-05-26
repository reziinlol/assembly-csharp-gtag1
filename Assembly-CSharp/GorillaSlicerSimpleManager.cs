using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x0200088D RID: 2189
public class GorillaSlicerSimpleManager : MonoBehaviour
{
	// Token: 0x06003929 RID: 14633 RVA: 0x0013823D File Offset: 0x0013643D
	protected void Awake()
	{
		if (GorillaSlicerSimpleManager.hasInstance && GorillaSlicerSimpleManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaSlicerSimpleManager.SetInstance(this);
	}

	// Token: 0x0600392A RID: 14634 RVA: 0x00138260 File Offset: 0x00136460
	public static void CreateManager()
	{
		GorillaSlicerSimpleManager gorillaSlicerSimpleManager = new GameObject("GorillaSlicerSimpleManager").AddComponent<GorillaSlicerSimpleManager>();
		gorillaSlicerSimpleManager.fixedUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.updateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.lateUpdateSlice = new List<IGorillaSliceableSimple>();
		gorillaSlicerSimpleManager.sW = new Stopwatch();
		GorillaSlicerSimpleManager.SetInstance(gorillaSlicerSimpleManager);
	}

	// Token: 0x0600392B RID: 14635 RVA: 0x001382AD File Offset: 0x001364AD
	private static void SetInstance(GorillaSlicerSimpleManager manager)
	{
		GorillaSlicerSimpleManager.instance = manager;
		GorillaSlicerSimpleManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x0600392C RID: 14636 RVA: 0x00018E08 File Offset: 0x00017008
	public static void RegisterSliceable(IGorillaSliceableSimple gSS)
	{
		GorillaSlicerSimpleManager.RegisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x001382C8 File Offset: 0x001364C8
	public static void RegisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		GorillaSlicerSimpleManager.instance.lastRunTicks.TryAdd(gSS, 0L);
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (!GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (!GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Add(gSS);
				return;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (!GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Add(gSS);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x0013836F File Offset: 0x0013656F
	public static bool UnregisterSliceable(IGorillaSliceableSimple gSS)
	{
		return GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.Update) || GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.LateUpdate) || GorillaSlicerSimpleManager.UnregisterSliceable(gSS, GorillaSlicerSimpleManager.UpdateStep.FixedUpdate);
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x00138390 File Offset: 0x00136590
	public static bool UnregisterSliceable(IGorillaSliceableSimple gSS, GorillaSlicerSimpleManager.UpdateStep step)
	{
		if (!GorillaSlicerSimpleManager.hasInstance)
		{
			GorillaSlicerSimpleManager.CreateManager();
		}
		switch (step)
		{
		case GorillaSlicerSimpleManager.UpdateStep.FixedUpdate:
			if (GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.fixedUpdateSlice.Remove(gSS);
				return true;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.Update:
			if (GorillaSlicerSimpleManager.instance.updateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.updateSlice.Remove(gSS);
				return true;
			}
			break;
		case GorillaSlicerSimpleManager.UpdateStep.LateUpdate:
			if (GorillaSlicerSimpleManager.instance.lateUpdateSlice.Contains(gSS))
			{
				GorillaSlicerSimpleManager.instance.lateUpdateSlice.Remove(gSS);
				return true;
			}
			break;
		}
		return false;
	}

	// Token: 0x06003930 RID: 14640 RVA: 0x00138430 File Offset: 0x00136630
	public void FixedUpdate()
	{
		this.startingIndex = this.updateIndex;
		if (this.updateIndex < 0 || this.updateIndex >= this.fixedUpdateSlice.Count + this.updateSlice.Count + this.lateUpdateSlice.Count)
		{
			this.updateIndex = 0;
		}
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && this.updateIndex < this.fixedUpdateSlice.Count)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.fixedUpdateSlice[this.updateIndex];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex && this.updateIndex < this.fixedUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x001385C0 File Offset: 0x001367C0
	public void Update()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int num = count + count2;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && count <= this.updateIndex && this.updateIndex < num)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.updateSlice[this.updateIndex - count];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex - count && this.updateIndex - count < this.updateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.ticksThisFrame += this.sW.ElapsedTicks;
		this.sW.Stop();
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x00138730 File Offset: 0x00136930
	public void LateUpdate()
	{
		int count = this.fixedUpdateSlice.Count;
		int count2 = this.updateSlice.Count;
		int count3 = this.lateUpdateSlice.Count;
		int num = count + count2;
		int num2 = num + count3;
		this.sW.Restart();
		while (this.ticksThisFrame + this.sW.ElapsedTicks < this.ticksPerFrame && num <= this.updateIndex && this.updateIndex < num2)
		{
			IGorillaSliceableSimple gorillaSliceableSimple = this.lateUpdateSlice[this.updateIndex - num];
			if (this.startingIndex != this.updateIndex && this.ticksThisFrame + this.sW.ElapsedTicks + this.lastRunTicks[gorillaSliceableSimple] >= this.ticksPerFrame)
			{
				this.ticksThisFrame = this.ticksPerFrame;
				break;
			}
			long elapsedTicks = this.sW.ElapsedTicks;
			if (0 <= this.updateIndex - num && this.updateIndex - num < this.lateUpdateSlice.Count)
			{
				MonoBehaviour monoBehaviour = gorillaSliceableSimple as MonoBehaviour;
				if (monoBehaviour == null || monoBehaviour.isActiveAndEnabled)
				{
					try
					{
						gorillaSliceableSimple.SliceUpdate();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			this.lastRunTicks[gorillaSliceableSimple] = this.sW.ElapsedTicks - elapsedTicks;
			this.updateIndex++;
		}
		this.sW.Stop();
		if (this.updateIndex >= num2)
		{
			this.updateIndex = -1;
		}
		this.ticksThisFrame = 0L;
	}

	// Token: 0x0400492F RID: 18735
	public static GorillaSlicerSimpleManager instance;

	// Token: 0x04004930 RID: 18736
	public static bool hasInstance;

	// Token: 0x04004931 RID: 18737
	public List<IGorillaSliceableSimple> fixedUpdateSlice;

	// Token: 0x04004932 RID: 18738
	public List<IGorillaSliceableSimple> updateSlice;

	// Token: 0x04004933 RID: 18739
	public List<IGorillaSliceableSimple> lateUpdateSlice;

	// Token: 0x04004934 RID: 18740
	public long ticksPerFrame = 1000L;

	// Token: 0x04004935 RID: 18741
	public long ticksThisFrame;

	// Token: 0x04004936 RID: 18742
	public int updateIndex = -1;

	// Token: 0x04004937 RID: 18743
	public int startingIndex = -1;

	// Token: 0x04004938 RID: 18744
	public Stopwatch sW;

	// Token: 0x04004939 RID: 18745
	public Dictionary<IGorillaSliceableSimple, long> lastRunTicks = new Dictionary<IGorillaSliceableSimple, long>();

	// Token: 0x0200088E RID: 2190
	public enum UpdateStep
	{
		// Token: 0x0400493B RID: 18747
		FixedUpdate,
		// Token: 0x0400493C RID: 18748
		Update,
		// Token: 0x0400493D RID: 18749
		LateUpdate
	}
}
