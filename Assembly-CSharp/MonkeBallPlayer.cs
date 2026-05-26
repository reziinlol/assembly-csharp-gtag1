using System;
using UnityEngine;

// Token: 0x020005FD RID: 1533
public class MonkeBallPlayer : MonoBehaviour
{
	// Token: 0x0600264D RID: 9805 RVA: 0x000CB1EF File Offset: 0x000C93EF
	private void Awake()
	{
		if (this.gamePlayer == null)
		{
			this.gamePlayer = base.GetComponent<GameBallPlayer>();
		}
	}

	// Token: 0x040031A4 RID: 12708
	public GameBallPlayer gamePlayer;

	// Token: 0x040031A5 RID: 12709
	public MonkeBallGoalZone currGoalZone;
}
