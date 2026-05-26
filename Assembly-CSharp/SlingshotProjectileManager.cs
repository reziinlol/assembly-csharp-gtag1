using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004C1 RID: 1217
public class SlingshotProjectileManager : MonoBehaviourTick
{
	// Token: 0x06001DB8 RID: 7608 RVA: 0x000A056D File Offset: 0x0009E76D
	protected void Awake()
	{
		if (SlingshotProjectileManager.hasInstance && SlingshotProjectileManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SlingshotProjectileManager.SetInstance(this);
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x000A0590 File Offset: 0x0009E790
	public static void CreateManager()
	{
		SlingshotProjectileManager.SetInstance(new GameObject("SlingshotProjectileManager").AddComponent<SlingshotProjectileManager>());
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x000A05A6 File Offset: 0x0009E7A6
	private static void SetInstance(SlingshotProjectileManager manager)
	{
		SlingshotProjectileManager.instance = manager;
		SlingshotProjectileManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x000A05C1 File Offset: 0x0009E7C1
	public static void RegisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (!SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Add(sP);
		}
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x000A05E7 File Offset: 0x0009E7E7
	public static void UnregisterSP(SlingshotProjectile sP)
	{
		if (!SlingshotProjectileManager.hasInstance)
		{
			SlingshotProjectileManager.CreateManager();
		}
		if (SlingshotProjectileManager.allsP.Contains(sP))
		{
			SlingshotProjectileManager.allsP.Remove(sP);
		}
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x000A0610 File Offset: 0x0009E810
	public override void Tick()
	{
		for (int i = 0; i < SlingshotProjectileManager.allsP.Count; i++)
		{
			SlingshotProjectileManager.allsP[i].InvokeUpdate();
		}
	}

	// Token: 0x04002803 RID: 10243
	public static SlingshotProjectileManager instance;

	// Token: 0x04002804 RID: 10244
	public static bool hasInstance = false;

	// Token: 0x04002805 RID: 10245
	public static List<SlingshotProjectile> allsP = new List<SlingshotProjectile>();
}
