using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068C RID: 1676
public class FlockingUpdateManager : MonoBehaviour
{
	// Token: 0x060029C6 RID: 10694 RVA: 0x000E1B88 File Offset: 0x000DFD88
	protected void Awake()
	{
		if (FlockingUpdateManager.hasInstance && FlockingUpdateManager.instance != null && FlockingUpdateManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		FlockingUpdateManager.SetInstance(this);
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000E1BB8 File Offset: 0x000DFDB8
	public static void CreateManager()
	{
		FlockingUpdateManager.SetInstance(new GameObject("FlockingUpdateManager").AddComponent<FlockingUpdateManager>());
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000E1BCE File Offset: 0x000DFDCE
	private static void SetInstance(FlockingUpdateManager manager)
	{
		FlockingUpdateManager.instance = manager;
		FlockingUpdateManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x000E1BE9 File Offset: 0x000DFDE9
	public static void RegisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (!FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Add(flocking);
		}
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x000E1C0F File Offset: 0x000DFE0F
	public static void UnregisterFlocking(Flocking flocking)
	{
		if (!FlockingUpdateManager.hasInstance)
		{
			FlockingUpdateManager.CreateManager();
		}
		if (FlockingUpdateManager.allFlockings.Contains(flocking))
		{
			FlockingUpdateManager.allFlockings.Remove(flocking);
		}
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000E1C38 File Offset: 0x000DFE38
	public void Update()
	{
		for (int i = 0; i < FlockingUpdateManager.allFlockings.Count; i++)
		{
			FlockingUpdateManager.allFlockings[i].InvokeUpdate();
		}
	}

	// Token: 0x04003686 RID: 13958
	public static FlockingUpdateManager instance;

	// Token: 0x04003687 RID: 13959
	public static bool hasInstance = false;

	// Token: 0x04003688 RID: 13960
	public static List<Flocking> allFlockings = new List<Flocking>();
}
