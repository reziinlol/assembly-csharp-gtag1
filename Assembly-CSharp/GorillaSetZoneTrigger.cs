using System;
using UnityEngine;

// Token: 0x020003B1 RID: 945
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x060016CA RID: 5834 RVA: 0x000848AB File Offset: 0x00082AAB
	public override void OnBoxTriggered()
	{
		Debug.Log("Triggered set zone box on gameobject " + base.gameObject.name);
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x040021D1 RID: 8657
	[SerializeField]
	private GTZone[] zones;
}
