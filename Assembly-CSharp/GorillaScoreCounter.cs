using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200067B RID: 1659
public class GorillaScoreCounter : MonoBehaviour
{
	// Token: 0x06002973 RID: 10611 RVA: 0x000DFBD0 File Offset: 0x000DDDD0
	private void Awake()
	{
		this.text = base.gameObject.GetComponent<Text>();
		if (this.isRedTeam)
		{
			this.attribute = "redScore";
			return;
		}
		this.attribute = "blueScore";
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000DFC04 File Offset: 0x000DDE04
	private void Update()
	{
		if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties[this.attribute] != null)
		{
			this.text.text = ((int)PhotonNetwork.CurrentRoom.CustomProperties[this.attribute]).ToString();
		}
	}

	// Token: 0x040035EB RID: 13803
	public bool isRedTeam;

	// Token: 0x040035EC RID: 13804
	public Text text;

	// Token: 0x040035ED RID: 13805
	public string attribute;
}
