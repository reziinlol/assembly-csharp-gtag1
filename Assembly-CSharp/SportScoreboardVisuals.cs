using System;
using GorillaTag.Sports;
using UnityEngine;

// Token: 0x020009D6 RID: 2518
public class SportScoreboardVisuals : MonoBehaviour
{
	// Token: 0x06004076 RID: 16502 RVA: 0x00158630 File Offset: 0x00156830
	private void Awake()
	{
		SportScoreboard.Instance.RegisterTeamVisual(this.TeamIndex, this);
	}

	// Token: 0x0400510D RID: 20749
	[SerializeField]
	public MaterialUVOffsetListSetter score1s;

	// Token: 0x0400510E RID: 20750
	[SerializeField]
	public MaterialUVOffsetListSetter score10s;

	// Token: 0x0400510F RID: 20751
	[SerializeField]
	private int TeamIndex;
}
