using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E1 RID: 4577
	public class CosmeticExclusionSource : MonoBehaviour
	{
		// Token: 0x06007307 RID: 29447 RVA: 0x00256A2A File Offset: 0x00254C2A
		public bool IsRestricted()
		{
			return CosmeticExclusionZoneRegistryUtility.IsPositionRestricted(base.transform.position);
		}
	}
}
