using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000911 RID: 2321
public class SizeManagerManager : MonoBehaviour
{
	// Token: 0x06003CAA RID: 15530 RVA: 0x0014A618 File Offset: 0x00148818
	protected void Awake()
	{
		if (SizeManagerManager.hasInstance && SizeManagerManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		SizeManagerManager.SetInstance(this);
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x0014A63B File Offset: 0x0014883B
	public static void CreateManager()
	{
		SizeManagerManager.SetInstance(new GameObject("SizeManagerManager").AddComponent<SizeManagerManager>());
	}

	// Token: 0x06003CAC RID: 15532 RVA: 0x0014A651 File Offset: 0x00148851
	private static void SetInstance(SizeManagerManager manager)
	{
		SizeManagerManager.instance = manager;
		SizeManagerManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x0014A66C File Offset: 0x0014886C
	public static void RegisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (!SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Add(sM);
		}
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x0014A692 File Offset: 0x00148892
	public static void UnregisterSM(SizeManager sM)
	{
		if (!SizeManagerManager.hasInstance)
		{
			SizeManagerManager.CreateManager();
		}
		if (SizeManagerManager.allSM.Contains(sM))
		{
			SizeManagerManager.allSM.Remove(sM);
		}
	}

	// Token: 0x06003CAF RID: 15535 RVA: 0x0014A6BC File Offset: 0x001488BC
	public void FixedUpdate()
	{
		for (int i = 0; i < SizeManagerManager.allSM.Count; i++)
		{
			SizeManagerManager.allSM[i].InvokeFixedUpdate();
		}
	}

	// Token: 0x04004D59 RID: 19801
	[OnEnterPlay_SetNull]
	public static SizeManagerManager instance;

	// Token: 0x04004D5A RID: 19802
	[OnEnterPlay_Set(false)]
	public static bool hasInstance = false;

	// Token: 0x04004D5B RID: 19803
	[OnEnterPlay_Clear]
	public static List<SizeManager> allSM = new List<SizeManager>();
}
