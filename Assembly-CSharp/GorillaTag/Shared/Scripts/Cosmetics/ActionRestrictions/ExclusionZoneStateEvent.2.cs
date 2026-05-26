using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E7 RID: 4583
	[Serializable]
	public class ExclusionZoneStateEvent<T> : ZoneStateEventBase
	{
		// Token: 0x0600731B RID: 29467 RVA: 0x00256BD2 File Offset: 0x00254DD2
		public void Invoke(VRRig vrRig, T arg)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg);
				return;
			}
		}

		// Token: 0x04008384 RID: 33668
		public ExclusionZoneStateEvent<T>.TypedEvent onNormal;

		// Token: 0x04008385 RID: 33669
		public ExclusionZoneStateEvent<T>.TypedEvent onRestricted;

		// Token: 0x020011E8 RID: 4584
		[Serializable]
		public class TypedEvent : UnityEvent<T>
		{
		}
	}
}
