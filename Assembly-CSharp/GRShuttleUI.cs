using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020007E2 RID: 2018
[Serializable]
public class GRShuttleUI
{
	// Token: 0x06003377 RID: 13175 RVA: 0x0011B845 File Offset: 0x00119A45
	public void Setup(GhostReactor reactor, NetPlayer player)
	{
		this.reactor = reactor;
		this.player = player;
		this.RefreshUI();
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x0011B85C File Offset: 0x00119A5C
	public void RefreshUI()
	{
		if (this.playerName != null)
		{
			this.playerName.text = ((this.player == null) ? null : this.player.SanitizedNickName);
		}
		if (this.playerTitle != null)
		{
			GRPlayer grplayer = (this.player == null) ? null : GRPlayer.Get(this.player.ActorNumber);
			if (grplayer != null)
			{
				this.playerTitle.text = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			}
			else
			{
				this.playerTitle.text = null;
			}
		}
		if (this.shuttle != null)
		{
			int targetFloor = this.shuttle.GetTargetFloor();
			if (this.destFloorText != null)
			{
				if (targetFloor == -1)
				{
					this.destFloorText.text = "HQ";
				}
				else
				{
					this.destFloorText.text = (targetFloor + 1).ToString();
				}
			}
			bool flag = targetFloor <= this.shuttle.GetMaxDropFloor();
			this.validScreen.SetActive(flag);
			this.invalidScreen.SetActive(!flag);
			if (flag)
			{
				this.infoText.text = "READY!\n\nDROP TO LEVEL";
				return;
			}
			this.infoText.text = "UNSAFE!\n\nUPGRADE DROP CHASSIS";
		}
	}

	// Token: 0x04004331 RID: 17201
	public TMP_Text playerName;

	// Token: 0x04004332 RID: 17202
	public TMP_Text playerTitle;

	// Token: 0x04004333 RID: 17203
	public TMP_Text destFloorText;

	// Token: 0x04004334 RID: 17204
	public TMP_Text infoText;

	// Token: 0x04004335 RID: 17205
	public GameObject validScreen;

	// Token: 0x04004336 RID: 17206
	public GameObject invalidScreen;

	// Token: 0x04004337 RID: 17207
	public GRShuttle shuttle;

	// Token: 0x04004338 RID: 17208
	private NetPlayer player;

	// Token: 0x04004339 RID: 17209
	private GhostReactor reactor;
}
