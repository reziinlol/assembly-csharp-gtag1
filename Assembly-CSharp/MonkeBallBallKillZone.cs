using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005F6 RID: 1526
public class MonkeBallBallKillZone : MonoBehaviour
{
	// Token: 0x06002601 RID: 9729 RVA: 0x000C94F0 File Offset: 0x000C76F0
	private void OnTriggerEnter(Collider other)
	{
		GameBall component = other.transform.GetComponent<GameBall>();
		if (component != null)
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				MonkeBallGame.Instance.RequestResetBall(component.id, -1);
				return;
			}
			GameBallManager.Instance.RequestSetBallPosition(component.id);
		}
	}
}
