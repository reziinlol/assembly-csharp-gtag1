using System;
using System.Collections.Generic;

// Token: 0x020003B9 RID: 953
public static class ZoneExtensions
{
	// Token: 0x060016F2 RID: 5874 RVA: 0x00085268 File Offset: 0x00083468
	public static bool IsAnyPlayerInZone(this GTZone zone)
	{
		using (IEnumerator<VRRig> enumerator = VRRigCache.ActiveRigs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.zoneEntity.currentZone == zone)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060016F3 RID: 5875 RVA: 0x000852C0 File Offset: 0x000834C0
	public static bool IsAnyPlayerInZones(this IList<GTZone> zones)
	{
		if (zones == null)
		{
			return false;
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			if (zones.Contains(vrrig.zoneEntity.currentZone))
			{
				return true;
			}
		}
		return false;
	}
}
