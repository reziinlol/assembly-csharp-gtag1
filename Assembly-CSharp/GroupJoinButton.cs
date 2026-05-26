using System;
using GorillaNetworking;
using Photon.Pun;

// Token: 0x020008D0 RID: 2256
public class GroupJoinButton : GorillaPressableButton
{
	// Token: 0x06003AF8 RID: 15096 RVA: 0x00143971 File Offset: 0x00141B71
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (this.inPrivate)
		{
			GorillaComputer.instance.OnGroupJoinButtonPress(this.gameModeIndex, this.friendCollider);
		}
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x00143999 File Offset: 0x00141B99
	public void Update()
	{
		this.inPrivate = (PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible);
		if (!this.inPrivate)
		{
			this.isOn = true;
		}
	}

	// Token: 0x04004B63 RID: 19299
	public int gameModeIndex;

	// Token: 0x04004B64 RID: 19300
	public GorillaFriendCollider friendCollider;

	// Token: 0x04004B65 RID: 19301
	public bool inPrivate;
}
