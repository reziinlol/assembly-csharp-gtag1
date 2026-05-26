using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public class PUNCallbackNotifier : MonoBehaviourPunCallbacks, IOnEventCallback
{
	// Token: 0x06001C08 RID: 7176 RVA: 0x00098966 File Offset: 0x00096B66
	private void Start()
	{
		this.parentSystem = base.GetComponent<NetworkSystemPUN>();
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x00098974 File Offset: 0x00096B74
	public override void OnConnectedToMaster()
	{
		this.parentSystem.OnConnectedtoMaster();
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x00098981 File Offset: 0x00096B81
	public override void OnJoinedRoom()
	{
		this.parentSystem.OnJoinedRoom();
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x0009898E File Offset: 0x00096B8E
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001C0D RID: 7181 RVA: 0x0009898E File Offset: 0x00096B8E
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		this.parentSystem.OnJoinRoomFailed(returnCode, message);
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x0009899D File Offset: 0x00096B9D
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		this.parentSystem.OnCreateRoomFailed(returnCode, message);
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x000989AC File Offset: 0x00096BAC
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.parentSystem.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x000989BA File Offset: 0x00096BBA
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.parentSystem.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x000989C8 File Offset: 0x00096BC8
	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("Disconnect callback, cause:" + cause.ToString());
		this.parentSystem.OnDisconnected(cause);
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x000989F2 File Offset: 0x00096BF2
	public void OnEvent(EventData photonEvent)
	{
		this.parentSystem.RaiseEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x00098A11 File Offset: 0x00096C11
	public override void OnPreLeavingRoom()
	{
		this.parentSystem.PreLeavingRoom();
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x00098A1E File Offset: 0x00096C1E
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		this.parentSystem.OnMasterClientSwitched(newMasterClient);
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x00098A2C File Offset: 0x00096C2C
	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		base.OnCustomAuthenticationResponse(data);
		NetworkSystem.Instance.CustomAuthenticationResponse(data);
	}

	// Token: 0x04002636 RID: 9782
	private NetworkSystemPUN parentSystem;
}
