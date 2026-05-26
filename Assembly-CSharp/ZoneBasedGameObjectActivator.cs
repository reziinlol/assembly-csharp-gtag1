using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E10 RID: 3600
public class ZoneBasedGameObjectActivator : MonoBehaviour
{
	// Token: 0x060057B1 RID: 22449 RVA: 0x001C6846 File Offset: 0x001C4A46
	private void OnEnable()
	{
		ZoneManagement.OnZoneChange += this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x060057B2 RID: 22450 RVA: 0x001C6859 File Offset: 0x001C4A59
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x060057B3 RID: 22451 RVA: 0x001C686C File Offset: 0x001C4A6C
	private void ZoneManagement_OnZoneChange(ZoneData[] zoneData)
	{
		HashSet<GTZone> hashSet = new HashSet<GTZone>(this.zones);
		bool flag = false;
		for (int i = 0; i < zoneData.Length; i++)
		{
			flag |= (zoneData[i].active && hashSet.Contains(zoneData[i].zone));
		}
		for (int j = 0; j < this.gameObjects.Length; j++)
		{
			this.gameObjects[j].SetActive(flag);
		}
	}

	// Token: 0x04006879 RID: 26745
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x0400687A RID: 26746
	[SerializeField]
	private GameObject[] gameObjects;
}
