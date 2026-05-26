using System;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class GameModeSelectorJoinSubsButton : MonoBehaviour
{
	// Token: 0x06000409 RID: 1033 RVA: 0x0001815C File Offset: 0x0001635C
	private void OnEnable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent += new Action(this.OnLeaveRoom);
		this.CheckSubscribed();
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x000181C8 File Offset: 0x000163C8
	private void OnDisable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.CheckSubscribed));
		RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinRoom);
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeaveRoom);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x0001822B File Offset: 0x0001642B
	[ContextMenu("Check Subscribed")]
	private void CheckSubscribed()
	{
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			this.DisableButtonSubscribers();
			return;
		}
		if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.MaxPlayers <= 10)
		{
			this.ShowButton();
			return;
		}
		this.DisableButtonInPublicRoom();
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0001825D File Offset: 0x0001645D
	private void OnJoinRoom()
	{
		if (!RoomSystem.WasRoomPrivate)
		{
			this.CheckSubscribed();
			return;
		}
		this.DisableButtonPrivate();
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00018273 File Offset: 0x00016473
	private void OnLeaveRoom()
	{
		this.CheckSubscribed();
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0001827B File Offset: 0x0001647B
	private void ShowButton()
	{
		this.subsPublicButton.enabled = true;
		this.subsPublicButton.SetUnpressedMaterial();
		this.disabledObject.SetActive(false);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x000182A0 File Offset: 0x000164A0
	private void DisableButtonSubscribers()
	{
		this.DisableButton("ONLY FOR\nSUBSCRIBERS");
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x000182AD File Offset: 0x000164AD
	private void DisableButtonPrivate()
	{
		this.DisableButton("IN PRIVATE ROOM");
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x000182BA File Offset: 0x000164BA
	private void DisableButtonInPublicRoom()
	{
		this.DisableButton("ALREADY IN\nPUBLIC ROOM");
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x000182C7 File Offset: 0x000164C7
	private void DisableButton(string disabled)
	{
		this.subsPublicButton.enabled = false;
		this.subsPublicButton.SetRendererMaterial(this.DisabledButtonMaterial);
		this.disabledObject.SetActive(true);
		this.disabledText.text = disabled;
	}

	// Token: 0x04000477 RID: 1143
	public Material DisabledButtonMaterial;

	// Token: 0x04000478 RID: 1144
	[SerializeField]
	private GorillaPressableButton subsPublicButton;

	// Token: 0x04000479 RID: 1145
	[SerializeField]
	private GameObject disabledObject;

	// Token: 0x0400047A RID: 1146
	[SerializeField]
	private TextMeshPro disabledText;
}
