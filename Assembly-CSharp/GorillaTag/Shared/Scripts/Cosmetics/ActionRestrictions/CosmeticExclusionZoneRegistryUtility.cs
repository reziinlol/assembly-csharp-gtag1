using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E4 RID: 4580
	public static class CosmeticExclusionZoneRegistryUtility
	{
		// Token: 0x06007313 RID: 29459 RVA: 0x00256B15 File Offset: 0x00254D15
		public static void RegisterZone(Collider zone)
		{
			if (zone != null && !CosmeticExclusionZoneRegistryUtility.exclusionZones.Contains(zone))
			{
				CosmeticExclusionZoneRegistryUtility.exclusionZones.Add(zone);
			}
		}

		// Token: 0x06007314 RID: 29460 RVA: 0x00256B38 File Offset: 0x00254D38
		public static void UnregisterZone(Collider zone)
		{
			CosmeticExclusionZoneRegistryUtility.exclusionZones.Remove(zone);
		}

		// Token: 0x06007315 RID: 29461 RVA: 0x00256B48 File Offset: 0x00254D48
		public static bool IsPositionRestricted(Vector3 worldPos)
		{
			for (int i = 0; i < CosmeticExclusionZoneRegistryUtility.exclusionZones.Count; i++)
			{
				Collider collider = CosmeticExclusionZoneRegistryUtility.exclusionZones[i];
				if (collider != null && collider.bounds.Contains(worldPos))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04008381 RID: 33665
		private static readonly List<Collider> exclusionZones = new List<Collider>();
	}
}
