using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054E RID: 1358
[DefaultExecutionOrder(1549)]
public class TransferrableObjectManager : MonoBehaviour
{
	// Token: 0x0600228C RID: 8844 RVA: 0x000B9A4E File Offset: 0x000B7C4E
	protected void Awake()
	{
		if (TransferrableObjectManager.hasInstance && TransferrableObjectManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		TransferrableObjectManager.SetInstance(this);
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000B9A71 File Offset: 0x000B7C71
	protected void OnDestroy()
	{
		if (TransferrableObjectManager.instance == this)
		{
			TransferrableObjectManager.hasInstance = false;
			TransferrableObjectManager.instance = null;
		}
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000B9A8C File Offset: 0x000B7C8C
	protected void LateUpdate()
	{
		for (int i = 0; i < TransferrableObjectManager.transObs.Count; i++)
		{
			TransferrableObjectManager.transObs[i].TriggeredLateUpdate();
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000B9ABE File Offset: 0x000B7CBE
	private static void CreateManager()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TransferrableObjectManager.SetInstance(new GameObject("TransferrableObjectManager").AddComponent<TransferrableObjectManager>());
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x000B9ADC File Offset: 0x000B7CDC
	private static void SetInstance(TransferrableObjectManager manager)
	{
		TransferrableObjectManager.instance = manager;
		TransferrableObjectManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000B9AF7 File Offset: 0x000B7CF7
	public static void Register(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (!TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Add(transOb);
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000B9B1D File Offset: 0x000B7D1D
	public static void Unregister(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Remove(transOb);
		}
	}

	// Token: 0x04002D9F RID: 11679
	public static TransferrableObjectManager instance;

	// Token: 0x04002DA0 RID: 11680
	public static bool hasInstance = false;

	// Token: 0x04002DA1 RID: 11681
	public static readonly List<TransferrableObject> transObs = new List<TransferrableObject>(1024);
}
