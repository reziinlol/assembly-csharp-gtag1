using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000E13 RID: 3603
public class ZoneDef : MonoBehaviour
{
	// Token: 0x17000845 RID: 2117
	// (get) Token: 0x060057BD RID: 22461 RVA: 0x001C6CB4 File Offset: 0x001C4EB4
	public GroupJoinZoneAB groupZoneAB
	{
		get
		{
			return new GroupJoinZoneAB
			{
				a = this.groupZone,
				b = this.groupZoneB
			};
		}
	}

	// Token: 0x060057BE RID: 22462 RVA: 0x001C6CE4 File Offset: 0x001C4EE4
	public bool IsSameZone(ZoneDef other)
	{
		return !(other == null) && this.zoneId == other.zoneId && this.subZoneId == other.subZoneId;
	}

	// Token: 0x0400688A RID: 26762
	public GTZone zoneId;

	// Token: 0x0400688B RID: 26763
	[FormerlySerializedAs("subZoneType")]
	[FormerlySerializedAs("subZone")]
	public GTSubZone subZoneId;

	// Token: 0x0400688C RID: 26764
	public GroupJoinZoneA groupZone;

	// Token: 0x0400688D RID: 26765
	public GroupJoinZoneB groupZoneB;

	// Token: 0x0400688E RID: 26766
	public int trackStayIntervalSec = 30;

	// Token: 0x0400688F RID: 26767
	[Space]
	public bool trackEnter = true;

	// Token: 0x04006890 RID: 26768
	public bool trackExit;

	// Token: 0x04006891 RID: 26769
	public bool trackStay = true;
}
