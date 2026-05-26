using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067A RID: 1658
public class GorillaPlayerCounter : MonoBehaviour
{
	// Token: 0x06002970 RID: 10608 RVA: 0x000DFB28 File Offset: 0x000DDD28
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000DFB3C File Offset: 0x000DDD3C
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null)
		{
			int num = 0;
			foreach (KeyValuePair<int, Player> keyValuePair in PhotonNetwork.CurrentRoom.Players)
			{
				if ((bool)keyValuePair.Value.CustomProperties["isRedTeam"] == this.isRedTeam)
				{
					num++;
				}
			}
			this.text.text = num.ToString();
		}
	}

	// Token: 0x040035E8 RID: 13800
	public bool isRedTeam;

	// Token: 0x040035E9 RID: 13801
	public Text text;

	// Token: 0x040035EA RID: 13802
	public string attribute;
}
