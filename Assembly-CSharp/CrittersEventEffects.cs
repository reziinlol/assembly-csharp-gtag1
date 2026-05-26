using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class CrittersEventEffects : MonoBehaviour
{
	// Token: 0x060001D3 RID: 467 RVA: 0x0000AED8 File Offset: 0x000090D8
	private void Awake()
	{
		if (this.manager == null)
		{
			GTDev.LogError<string>("CrittersEventEffects missing reference to CrittersManager", null);
			return;
		}
		this.effectResponse = new Dictionary<CrittersManager.CritterEvent, GameObject>();
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			if (this.eventEffects[i].effect != null)
			{
				this.effectResponse.Add(this.eventEffects[i].eventType, this.eventEffects[i].effect);
			}
		}
		this.manager.OnCritterEventReceived += this.HandleReceivedEvent;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000AF70 File Offset: 0x00009170
	private void HandleReceivedEvent(CrittersManager.CritterEvent eventType, int sourceActor, Vector3 position, Quaternion rotation)
	{
		GameObject prefab;
		if (this.effectResponse.TryGetValue(eventType, out prefab))
		{
			GameObject pooled = CrittersPool.GetPooled(prefab);
			if (pooled.IsNotNull())
			{
				pooled.transform.position = position;
				pooled.transform.rotation = rotation;
			}
		}
	}

	// Token: 0x04000215 RID: 533
	public CrittersManager manager;

	// Token: 0x04000216 RID: 534
	public CrittersEventEffects.CrittersEventResponse[] eventEffects;

	// Token: 0x04000217 RID: 535
	private Dictionary<CrittersManager.CritterEvent, GameObject> effectResponse;

	// Token: 0x0200005E RID: 94
	[Serializable]
	public class CrittersEventResponse
	{
		// Token: 0x04000218 RID: 536
		public CrittersManager.CritterEvent eventType;

		// Token: 0x04000219 RID: 537
		public GameObject effect;
	}
}
