using System;
using UniLabs.Time;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class RealWorldDateTimeWindow : ScriptableObject
{
	// Token: 0x0600038C RID: 908 RVA: 0x00014A1E File Offset: 0x00012C1E
	public bool MatchesDate(DateTime utcDate)
	{
		return this.startTime <= utcDate && this.endTime >= utcDate;
	}

	// Token: 0x04000411 RID: 1041
	[SerializeField]
	private UDateTime startTime;

	// Token: 0x04000412 RID: 1042
	[SerializeField]
	private UDateTime endTime;
}
