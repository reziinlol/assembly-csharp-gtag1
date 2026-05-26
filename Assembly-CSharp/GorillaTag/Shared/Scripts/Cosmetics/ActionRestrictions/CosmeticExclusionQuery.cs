using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E0 RID: 4576
	public static class CosmeticExclusionQuery
	{
		// Token: 0x06007306 RID: 29446 RVA: 0x002569EC File Offset: 0x00254BEC
		public static bool IsRestricted(VRRig ownerRig = null, GameObject effectSource = null)
		{
			CosmeticExclusionSource cosmeticExclusionSource;
			return (ownerRig != null && CosmeticExclusionZoneRegistry.IsRestricted(ownerRig)) || (effectSource != null && effectSource.TryGetComponent<CosmeticExclusionSource>(out cosmeticExclusionSource) && cosmeticExclusionSource.IsRestricted());
		}
	}
}
