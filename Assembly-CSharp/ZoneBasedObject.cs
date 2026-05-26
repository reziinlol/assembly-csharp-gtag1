using System;
using UnityEngine;

// Token: 0x02000AB7 RID: 2743
public class ZoneBasedObject : MonoBehaviour
{
	// Token: 0x06004626 RID: 17958 RVA: 0x0017B624 File Offset: 0x00179824
	public bool IsLocalPlayerInZone()
	{
		GTZone[] array = this.zones;
		for (int i = 0; i < array.Length; i++)
		{
			if (ZoneManagement.IsInZone(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004627 RID: 17959 RVA: 0x0017B654 File Offset: 0x00179854
	public static ZoneBasedObject SelectRandomEligible(ZoneBasedObject[] objects, string overrideChoice = "")
	{
		if (overrideChoice != "")
		{
			foreach (ZoneBasedObject zoneBasedObject in objects)
			{
				if (zoneBasedObject.gameObject.name == overrideChoice)
				{
					return zoneBasedObject;
				}
			}
		}
		ZoneBasedObject result = null;
		int num = 0;
		foreach (ZoneBasedObject zoneBasedObject2 in objects)
		{
			if (zoneBasedObject2.gameObject.activeInHierarchy)
			{
				GTZone[] array = zoneBasedObject2.zones;
				for (int j = 0; j < array.Length; j++)
				{
					if (ZoneManagement.IsInZone(array[j]))
					{
						if (Random.Range(0, num) == 0)
						{
							result = zoneBasedObject2;
						}
						num++;
						break;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0400589C RID: 22684
	public GTZone[] zones;
}
