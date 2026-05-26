using System;
using UnityEngine;

// Token: 0x020005C8 RID: 1480
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x06002521 RID: 9505 RVA: 0x000C5C22 File Offset: 0x000C3E22
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000C5C30 File Offset: 0x000C3E30
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000C5C5C File Offset: 0x000C3E5C
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x0400307A RID: 12410
	public int sceneTriggerIndex;

	// Token: 0x0400307B RID: 12411
	public GorillaCameraSceneTrigger parentTrigger;
}
