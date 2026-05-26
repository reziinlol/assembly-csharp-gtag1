using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class CrittersGrabber : CrittersActor
{
	// Token: 0x060001E5 RID: 485 RVA: 0x0000B3C5 File Offset: 0x000095C5
	public override void ProcessRemote()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000B3DF File Offset: 0x000095DF
	public override bool ProcessLocal()
	{
		if (this.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			this.UpdateAverageSpeed();
		}
		return base.ProcessLocal();
	}

	// Token: 0x04000227 RID: 551
	public Transform grabPosition;

	// Token: 0x04000228 RID: 552
	public bool grabbing;

	// Token: 0x04000229 RID: 553
	public float grabDistance;

	// Token: 0x0400022A RID: 554
	public List<CrittersActor> grabbedActors = new List<CrittersActor>();

	// Token: 0x0400022B RID: 555
	public bool isLeft;
}
