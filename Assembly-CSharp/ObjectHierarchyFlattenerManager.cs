using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000359 RID: 857
public class ObjectHierarchyFlattenerManager : MonoBehaviourPostTick
{
	// Token: 0x060014F2 RID: 5362 RVA: 0x0006F829 File Offset: 0x0006DA29
	protected void Awake()
	{
		if (ObjectHierarchyFlattenerManager.hasInstance && ObjectHierarchyFlattenerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ObjectHierarchyFlattenerManager.SetInstance(this);
	}

	// Token: 0x060014F3 RID: 5363 RVA: 0x0006F84C File Offset: 0x0006DA4C
	public static void CreateManager()
	{
		ObjectHierarchyFlattenerManager.SetInstance(new GameObject("ObjectHierarchyFlattenerManager").AddComponent<ObjectHierarchyFlattenerManager>());
	}

	// Token: 0x060014F4 RID: 5364 RVA: 0x0006F862 File Offset: 0x0006DA62
	private static void SetInstance(ObjectHierarchyFlattenerManager manager)
	{
		ObjectHierarchyFlattenerManager.instance = manager;
		ObjectHierarchyFlattenerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060014F5 RID: 5365 RVA: 0x0006F87D File Offset: 0x0006DA7D
	public static void RegisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (!ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Add(rbWI);
		}
	}

	// Token: 0x060014F6 RID: 5366 RVA: 0x0006F8A3 File Offset: 0x0006DAA3
	public static void UnregisterOHF(ObjectHierarchyFlattener rbWI)
	{
		if (!ObjectHierarchyFlattenerManager.hasInstance)
		{
			ObjectHierarchyFlattenerManager.CreateManager();
		}
		if (ObjectHierarchyFlattenerManager.alloHF.Contains(rbWI))
		{
			ObjectHierarchyFlattenerManager.alloHF.Remove(rbWI);
		}
	}

	// Token: 0x060014F7 RID: 5367 RVA: 0x0006F8CC File Offset: 0x0006DACC
	public override void PostTick()
	{
		for (int i = 0; i < ObjectHierarchyFlattenerManager.alloHF.Count; i++)
		{
			ObjectHierarchyFlattenerManager.alloHF[i].InvokeLateUpdate();
		}
	}

	// Token: 0x040019D7 RID: 6615
	public static ObjectHierarchyFlattenerManager instance;

	// Token: 0x040019D8 RID: 6616
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x040019D9 RID: 6617
	public static List<ObjectHierarchyFlattener> alloHF = new List<ObjectHierarchyFlattener>();
}
