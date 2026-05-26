using System;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000C77 RID: 3191
public class FriendingStation : MonoBehaviour
{
	// Token: 0x1700076E RID: 1902
	// (get) Token: 0x06004F44 RID: 20292 RVA: 0x001A42A2 File Offset: 0x001A24A2
	public TextMeshProUGUI Player1Text
	{
		get
		{
			return this.player1Text;
		}
	}

	// Token: 0x1700076F RID: 1903
	// (get) Token: 0x06004F45 RID: 20293 RVA: 0x001A42AA File Offset: 0x001A24AA
	public TextMeshProUGUI Player2Text
	{
		get
		{
			return this.player2Text;
		}
	}

	// Token: 0x17000770 RID: 1904
	// (get) Token: 0x06004F46 RID: 20294 RVA: 0x001A42B2 File Offset: 0x001A24B2
	public TextMeshProUGUI StatusText
	{
		get
		{
			return this.statusText;
		}
	}

	// Token: 0x17000771 RID: 1905
	// (get) Token: 0x06004F47 RID: 20295 RVA: 0x001A42BA File Offset: 0x001A24BA
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x06004F48 RID: 20296 RVA: 0x001A42C2 File Offset: 0x001A24C2
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
	}

	// Token: 0x06004F49 RID: 20297 RVA: 0x001A42F4 File Offset: 0x001A24F4
	private void OnEnable()
	{
		FriendingManager.Instance.RegisterFriendingStation(this);
		if (PhotonNetwork.InRoom)
		{
			this.displayedData.actorNumberA = -1;
			this.displayedData.actorNumberB = -1;
			this.displayedData.state = FriendingManager.FriendStationState.WaitingForPlayers;
		}
		else
		{
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
		}
		this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
		this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
		this.UpdateDisplayedState(this.displayedData.state);
	}

	// Token: 0x06004F4A RID: 20298 RVA: 0x001A43A0 File Offset: 0x001A25A0
	private void OnDisable()
	{
		FriendingManager.Instance.UnregisterFriendingStation(this);
	}

	// Token: 0x06004F4B RID: 20299 RVA: 0x001A43B0 File Offset: 0x001A25B0
	private void UpdatePlayerText(TextMeshProUGUI playerText, int playerId)
	{
		if (playerId == -2)
		{
			playerText.text = "";
			return;
		}
		if (playerId == -1)
		{
			playerText.text = "PLAYER:\nNONE";
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(playerId);
		if (netPlayerByID != null)
		{
			playerText.text = "PLAYER:\n" + netPlayerByID.SanitizedNickName;
			return;
		}
		playerText.text = "PLAYER:\nNONE";
	}

	// Token: 0x06004F4C RID: 20300 RVA: 0x001A4410 File Offset: 0x001A2610
	private void UpdateDisplayedState(FriendingManager.FriendStationState state)
	{
		switch (state)
		{
		case FriendingManager.FriendStationState.NotInRoom:
			this.statusText.text = "JOIN A ROOM TO USE";
			return;
		case FriendingManager.FriendStationState.WaitingForPlayers:
			this.statusText.text = "";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusBoth:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerA:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnFriendStatusPlayerB:
			this.statusText.text = "LOADING";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonBoth:
			this.statusText.text = "PRESS [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerA:
			this.statusText.text = "PRESS [       ] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnButtonPlayerB:
			this.statusText.text = "READY [       ] PRESS";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer0:
			this.statusText.text = "READY [       ] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer1:
			this.statusText.text = "READY [-     -] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer2:
			this.statusText.text = "READY [--   --] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer3:
			this.statusText.text = "READY [--- ---] READY";
			return;
		case FriendingManager.FriendStationState.ButtonConfirmationTimer4:
			this.statusText.text = "READY [-------] READY";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestBoth:
			this.statusText.text = " SENT [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerA:
			this.statusText.text = " SENT [-------] DONE ";
			return;
		case FriendingManager.FriendStationState.WaitingOnRequestPlayerB:
			this.statusText.text = " DONE [-------] SENT ";
			return;
		case FriendingManager.FriendStationState.RequestFailed:
			this.statusText.text = "FRIEND REQUEST FAILED";
			return;
		case FriendingManager.FriendStationState.Friends:
			this.statusText.text = "\\O/ FRIENDS \\O/";
			return;
		case FriendingManager.FriendStationState.AlreadyFriends:
			this.statusText.text = "ALREADY FRIENDS";
			return;
		default:
			return;
		}
	}

	// Token: 0x06004F4D RID: 20301 RVA: 0x001A45B4 File Offset: 0x001A27B4
	private void UpdateAddFriendButton()
	{
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
		{
			this.addFriendButton.isOn = true;
		}
		else
		{
			this.addFriendButton.isOn = false;
		}
		this.addFriendButton.UpdateColor();
	}

	// Token: 0x06004F4E RID: 20302 RVA: 0x001A4654 File Offset: 0x001A2854
	private void UpdateDisplay(ref FriendingManager.FriendStationData data)
	{
		if (this.displayedData.actorNumberA != data.actorNumberA)
		{
			this.UpdatePlayerText(this.player1Text, data.actorNumberA);
		}
		if (this.displayedData.actorNumberB != data.actorNumberB)
		{
			this.UpdatePlayerText(this.player2Text, data.actorNumberB);
		}
		if (this.displayedData.state != data.state)
		{
			this.UpdateDisplayedState(data.state);
		}
		this.displayedData = data;
		this.UpdateAddFriendButton();
	}

	// Token: 0x06004F4F RID: 20303 RVA: 0x001A46DC File Offset: 0x001A28DC
	public void UpdateState(FriendingManager.FriendStationData data)
	{
		this.UpdateDisplay(ref data);
	}

	// Token: 0x06004F50 RID: 20304 RVA: 0x001A46E8 File Offset: 0x001A28E8
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null && component.OwningNetPlayer != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerEnteredStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x06004F51 RID: 20305 RVA: 0x001A47C0 File Offset: 0x001A29C0
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (PhotonNetwork.InRoom)
		{
			VRRig component = other.GetComponent<VRRig>();
			if (component != null)
			{
				this.addFriendButton.ResetState();
				FriendingManager.Instance.PlayerExitedStation(this.zone, component.OwningNetPlayer);
				return;
			}
		}
		else if (other == GTPlayer.Instance.headCollider)
		{
			this.displayedData.state = FriendingManager.FriendStationState.NotInRoom;
			this.displayedData.actorNumberA = -2;
			this.displayedData.actorNumberB = -2;
			this.UpdateDisplayedState(this.displayedData.state);
			this.UpdatePlayerText(this.player1Text, this.displayedData.actorNumberA);
			this.UpdatePlayerText(this.player2Text, this.displayedData.actorNumberB);
			this.addFriendButton.ResetState();
		}
	}

	// Token: 0x06004F52 RID: 20306 RVA: 0x001A4890 File Offset: 0x001A2A90
	public void FriendButtonPressed()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		string text;
		if (this.LocalPlayerIsAtCapacity(out text))
		{
			this.statusText.text = text;
			return;
		}
		if (!this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonPressedRPC", RpcTarget.MasterClient, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if (this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonBoth || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB))
			{
				this.addFriendButton.isOn = true;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x06004F53 RID: 20307 RVA: 0x001A497C File Offset: 0x001A2B7C
	private bool LocalPlayerIsAtCapacity(out string fullMessage)
	{
		fullMessage = null;
		FriendBackendController instance = FriendBackendController.Instance;
		if (instance == null || instance.FriendsList == null)
		{
			return false;
		}
		int configuredFreeExtraPageCount = FriendDisplay.ConfiguredFreeExtraPageCount;
		int configuredVimPageCount = FriendDisplay.ConfiguredVimPageCount;
		int num = 9 * configuredVimPageCount;
		int num2 = 9 + 9 * configuredFreeExtraPageCount;
		int num3 = num2 + num;
		int count = instance.FriendsList.Count;
		if (count < num2)
		{
			return false;
		}
		if (!SubscriptionManager.IsLocalSubscribed() && configuredVimPageCount != 0)
		{
			int num4 = Mathf.Clamp(count - num2, 0, num);
			if (num4 <= 0)
			{
				fullMessage = ZString.Format<int>("FRIEND SLOTS ARE FULL! SUBSCRIBE FOR {0} ADDITIONAL SLOTS!", num);
			}
			else
			{
				int arg = num - num4;
				fullMessage = ZString.Format<int>("RENEW GTFC SUBSCRIPTION TO UNLOCK YOUR REMAINING {0} SLOTS.", arg);
			}
			return true;
		}
		if (count >= num3)
		{
			fullMessage = "CANNOT ADD FRIEND! ALL FRIEND SLOTS FILLED!";
			return true;
		}
		return false;
	}

	// Token: 0x06004F54 RID: 20308 RVA: 0x001A4A30 File Offset: 0x001A2C30
	public void FriendButtonReleased()
	{
		if (this.displayedData.state == FriendingManager.FriendStationState.WaitingForPlayers || this.displayedData.state == FriendingManager.FriendStationState.Friends)
		{
			return;
		}
		if (this.addFriendButton.isOn)
		{
			FriendingManager.Instance.photonView.RPC("FriendButtonUnpressedRPC", RpcTarget.MasterClient, new object[]
			{
				this.zone
			});
			int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
			if ((this.displayedData.state >= FriendingManager.FriendStationState.ButtonConfirmationTimer0 && this.displayedData.state <= FriendingManager.FriendStationState.ButtonConfirmationTimer4) || (this.displayedData.actorNumberA == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerB) || (this.displayedData.actorNumberB == actorNumber && this.displayedData.state == FriendingManager.FriendStationState.WaitingOnButtonPlayerA))
			{
				this.addFriendButton.isOn = false;
				this.addFriendButton.UpdateColor();
			}
		}
	}

	// Token: 0x04006107 RID: 24839
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x04006108 RID: 24840
	[SerializeField]
	private TextMeshProUGUI player1Text;

	// Token: 0x04006109 RID: 24841
	[SerializeField]
	private TextMeshProUGUI player2Text;

	// Token: 0x0400610A RID: 24842
	[SerializeField]
	private TextMeshProUGUI statusText;

	// Token: 0x0400610B RID: 24843
	[SerializeField]
	private GTZone zone;

	// Token: 0x0400610C RID: 24844
	[SerializeField]
	private GorillaPressableButton addFriendButton;

	// Token: 0x0400610D RID: 24845
	private FriendingManager.FriendStationData displayedData;
}
