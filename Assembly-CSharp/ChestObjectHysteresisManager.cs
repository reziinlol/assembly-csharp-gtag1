using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000519 RID: 1305
[DefaultExecutionOrder(2000)]
public class ChestObjectHysteresisManager : MonoBehaviourTick
{
	// Token: 0x060020AA RID: 8362 RVA: 0x000AF292 File Offset: 0x000AD492
	protected void Awake()
	{
		if (ChestObjectHysteresisManager.hasInstance && ChestObjectHysteresisManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ChestObjectHysteresisManager.SetInstance(this);
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000AF2B5 File Offset: 0x000AD4B5
	public static void CreateManager()
	{
		ChestObjectHysteresisManager.SetInstance(new GameObject("ChestObjectHysteresisManager").AddComponent<ChestObjectHysteresisManager>());
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000AF2CB File Offset: 0x000AD4CB
	private static void SetInstance(ChestObjectHysteresisManager manager)
	{
		ChestObjectHysteresisManager.instance = manager;
		ChestObjectHysteresisManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000AF2E6 File Offset: 0x000AD4E6
	public static void RegisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (!ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Add(cOH);
		}
	}

	// Token: 0x060020AE RID: 8366 RVA: 0x000AF30C File Offset: 0x000AD50C
	public static void UnregisterCH(ChestObjectHysteresis cOH)
	{
		if (!ChestObjectHysteresisManager.hasInstance)
		{
			ChestObjectHysteresisManager.CreateManager();
		}
		if (ChestObjectHysteresisManager.allChests.Contains(cOH))
		{
			ChestObjectHysteresisManager.allChests.Remove(cOH);
		}
	}

	// Token: 0x060020AF RID: 8367 RVA: 0x000AF334 File Offset: 0x000AD534
	public override void Tick()
	{
		for (int i = 0; i < ChestObjectHysteresisManager.allChests.Count; i++)
		{
			ChestObjectHysteresisManager.allChests[i].InvokeUpdate();
		}
	}

	// Token: 0x04002B5F RID: 11103
	public static ChestObjectHysteresisManager instance;

	// Token: 0x04002B60 RID: 11104
	public static bool hasInstance = false;

	// Token: 0x04002B61 RID: 11105
	public static List<ChestObjectHysteresis> allChests = new List<ChestObjectHysteresis>();
}
