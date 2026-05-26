using System;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	// Token: 0x020011E9 RID: 4585
	[Serializable]
	public class ExclusionZoneStateEvent<T0, T1> : ZoneStateEventBase
	{
		// Token: 0x0600731E RID: 29470 RVA: 0x00256C10 File Offset: 0x00254E10
		public void Invoke(VRRig vrRig, T0 arg0, T1 arg1)
		{
			if (base.IsRestricted(vrRig))
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent = this.onRestricted;
				if (typedEvent == null)
				{
					return;
				}
				typedEvent.Invoke(arg0, arg1);
				return;
			}
			else
			{
				ExclusionZoneStateEvent<T0, T1>.TypedEvent typedEvent2 = this.onNormal;
				if (typedEvent2 == null)
				{
					return;
				}
				typedEvent2.Invoke(arg0, arg1);
				return;
			}
		}

		// Token: 0x04008386 RID: 33670
		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onNormal;

		// Token: 0x04008387 RID: 33671
		public ExclusionZoneStateEvent<T0, T1>.TypedEvent onRestricted;

		// Token: 0x020011EA RID: 4586
		[Serializable]
		public class TypedEvent : UnityEvent<T0, T1>
		{
		}
	}
}
