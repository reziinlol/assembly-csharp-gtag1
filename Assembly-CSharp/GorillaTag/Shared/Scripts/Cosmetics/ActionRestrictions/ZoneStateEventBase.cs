using System;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E5 RID: 4581
	public class ZoneStateEventBase
	{
		// Token: 0x06007317 RID: 29463 RVA: 0x00256B9F File Offset: 0x00254D9F
		protected bool IsRestricted(VRRig vrRig)
		{
			return CosmeticExclusionZoneRegistry.IsRestricted(vrRig);
		}
	}
}
