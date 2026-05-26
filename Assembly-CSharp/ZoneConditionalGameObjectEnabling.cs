using System;
using UnityEngine;

// Token: 0x020003C2 RID: 962
public class ZoneConditionalGameObjectEnabling : MonoBehaviour
{
	// Token: 0x0600171B RID: 5915 RVA: 0x00085B56 File Offset: 0x00083D56
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600171C RID: 5916 RVA: 0x00085B84 File Offset: 0x00083D84
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600171D RID: 5917 RVA: 0x00085BAC File Offset: 0x00083DAC
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.gameObjects != null)
			{
				for (int i = 0; i < this.gameObjects.Length; i++)
				{
					this.gameObjects[i].SetActive(!ZoneManagement.IsInZone(this.zone));
				}
				return;
			}
		}
		else if (this.gameObjects != null)
		{
			for (int j = 0; j < this.gameObjects.Length; j++)
			{
				this.gameObjects[j].SetActive(ZoneManagement.IsInZone(this.zone));
			}
		}
	}

	// Token: 0x0400224D RID: 8781
	[SerializeField]
	private GTZone zone;

	// Token: 0x0400224E RID: 8782
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x0400224F RID: 8783
	[SerializeField]
	private GameObject[] gameObjects;
}
