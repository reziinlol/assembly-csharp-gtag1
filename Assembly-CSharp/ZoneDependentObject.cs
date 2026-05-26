using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001F6 RID: 502
public class ZoneDependentObject : MonoBehaviour
{
	// Token: 0x06000D32 RID: 3378 RVA: 0x0004837A File Offset: 0x0004657A
	private void Awake()
	{
		ZoneEntityBSP.onPlayerZoneChange += this.OnPlayerZoneChange;
		this.UpdateObjectState();
	}

	// Token: 0x06000D33 RID: 3379 RVA: 0x00048393 File Offset: 0x00046593
	private void OnDestroy()
	{
		ZoneEntityBSP.onPlayerZoneChange -= this.OnPlayerZoneChange;
	}

	// Token: 0x06000D34 RID: 3380 RVA: 0x000483A8 File Offset: 0x000465A8
	private void OnPlayerZoneChange(VRRig rig, GTZone fromZone, GTZone toZone)
	{
		string format = "PlayerZoneChange: Player[{0}] {1}->{2}";
		NetPlayer creator = rig.Creator;
		Debug.Log(string.Format(format, (creator != null) ? new int?(creator.ActorNumber) : null, fromZone, toZone));
		this.UpdateObjectState();
	}

	// Token: 0x06000D35 RID: 3381 RVA: 0x000483FA File Offset: 0x000465FA
	private void UpdateObjectState()
	{
		if (this.zones.IsAnyPlayerInZones() != base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(this.zones.IsAnyPlayerInZones());
		}
	}

	// Token: 0x04000FC9 RID: 4041
	public List<GTZone> zones = new List<GTZone>
	{
		GTZone.forest
	};
}
