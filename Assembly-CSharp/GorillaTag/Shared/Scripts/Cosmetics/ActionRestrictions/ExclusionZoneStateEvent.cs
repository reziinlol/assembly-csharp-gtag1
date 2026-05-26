using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E6 RID: 4582
	[Serializable]
	public class ExclusionZoneStateEvent
	{
		// Token: 0x06007319 RID: 29465 RVA: 0x00256BA7 File Offset: 0x00254DA7
		public void Invoke(VRRig vrRig)
		{
			if (CosmeticExclusionZoneRegistry.IsRestricted(vrRig))
			{
				UnityEvent onRestricted = this.OnRestricted;
				if (onRestricted == null)
				{
					return;
				}
				onRestricted.Invoke();
				return;
			}
			else
			{
				UnityEvent onNormal = this.OnNormal;
				if (onNormal == null)
				{
					return;
				}
				onNormal.Invoke();
				return;
			}
		}

		// Token: 0x04008382 RID: 33666
		public UnityEvent OnNormal;

		// Token: 0x04008383 RID: 33667
		public UnityEvent OnRestricted;
	}
}
