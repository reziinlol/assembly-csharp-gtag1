using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005F7 RID: 1527
public class MonkeBallBallResetTrigger : MonoBehaviour
{
	// Token: 0x06002603 RID: 9731 RVA: 0x000C9540 File Offset: 0x000C7740
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			GameBallPlayer gameBallPlayer = (component.heldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.heldByActorNumber);
			if (gameBallPlayer == null)
			{
				gameBallPlayer = ((component.lastHeldByActorNumber < 0) ? null : GameBallPlayer.GetGamePlayer(component.lastHeldByActorNumber));
				if (gameBallPlayer == null)
				{
					return;
				}
			}
			this._lastBall = component;
			int num = gameBallPlayer.teamId;
			if (num == -1)
			{
				num = component.lastHeldByTeamId;
			}
			if (num >= 0 && num < this.teamMaterials.Length)
			{
				this.trigger.sharedMaterial = this.teamMaterials[num];
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(true, num);
			}
		}
	}

	// Token: 0x06002604 RID: 9732 RVA: 0x000C95F8 File Offset: 0x000C77F8
	private void OnTriggerExit(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (component == this._lastBall)
			{
				this.trigger.sharedMaterial = this.neutralMaterial;
				this._lastBall = null;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.ToggleResetButton(false, -1);
			}
		}
	}

	// Token: 0x04003168 RID: 12648
	public Renderer trigger;

	// Token: 0x04003169 RID: 12649
	public Material[] teamMaterials;

	// Token: 0x0400316A RID: 12650
	public Material neutralMaterial;

	// Token: 0x0400316B RID: 12651
	private GameBall _lastBall;
}
