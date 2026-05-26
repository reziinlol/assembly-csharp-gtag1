using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E3 RID: 4579
	public static class CosmeticExclusionZoneRegistry
	{
		// Token: 0x0600730E RID: 29454 RVA: 0x00256AB7 File Offset: 0x00254CB7
		public static void Enter(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Add(rig);
			}
		}

		// Token: 0x0600730F RID: 29455 RVA: 0x00256ACE File Offset: 0x00254CCE
		public static void Exit(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Remove(rig);
			}
		}

		// Token: 0x06007310 RID: 29456 RVA: 0x00256AE5 File Offset: 0x00254CE5
		public static bool IsRestricted(VRRig rig)
		{
			return rig != null && CosmeticExclusionZoneRegistry.restrictedRigs.Contains(rig);
		}

		// Token: 0x06007311 RID: 29457 RVA: 0x00256AFD File Offset: 0x00254CFD
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			CosmeticExclusionZoneRegistry.restrictedRigs.Clear();
		}

		// Token: 0x04008380 RID: 33664
		private static readonly HashSet<VRRig> restrictedRigs = new HashSet<VRRig>();
	}
}
