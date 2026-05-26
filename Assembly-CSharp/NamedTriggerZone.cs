using System;
using UnityEngine;

// Token: 0x020001E6 RID: 486
public class NamedTriggerZone : MonoBehaviour
{
	// Token: 0x06000CBA RID: 3258 RVA: 0x00046112 File Offset: 0x00044312
	private void Reset()
	{
		this.ConfigureCollider();
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x0004611C File Offset: 0x0004431C
	private void ConfigureCollider()
	{
		Collider collider = base.GetComponent<Collider>();
		if (!collider)
		{
			collider = base.gameObject.AddComponent<BoxCollider>();
		}
		collider.isTrigger = true;
		base.gameObject.layer = LayerMask.NameToLayer("Gorilla Trigger");
	}

	// Token: 0x04000F6C RID: 3948
	public string TriggerName = "Trigger";
}
