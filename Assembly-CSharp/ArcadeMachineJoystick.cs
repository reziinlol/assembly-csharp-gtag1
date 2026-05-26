using System;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200000E RID: 14
public class ArcadeMachineJoystick : HandHold, ISnapTurnOverride, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000026 RID: 38 RVA: 0x00002782 File Offset: 0x00000982
	// (set) Token: 0x06000027 RID: 39 RVA: 0x0000278A File Offset: 0x0000098A
	public bool heldByLocalPlayer { get; private set; }

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000028 RID: 40 RVA: 0x00002793 File Offset: 0x00000993
	public bool IsHeldLeftHanded
	{
		get
		{
			return this.heldByLocalPlayer && this.xrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000029 RID: 41 RVA: 0x000027A8 File Offset: 0x000009A8
	// (set) Token: 0x0600002A RID: 42 RVA: 0x000027B0 File Offset: 0x000009B0
	public ArcadeButtons currentButtonState { get; private set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600002B RID: 43 RVA: 0x000027B9 File Offset: 0x000009B9
	// (set) Token: 0x0600002C RID: 44 RVA: 0x000027C1 File Offset: 0x000009C1
	public int player { get; private set; }

	// Token: 0x0600002D RID: 45 RVA: 0x000027CA File Offset: 0x000009CA
	public void Init(ArcadeMachine machine, int player)
	{
		this.machine = machine;
		this.player = player;
		this.guard = base.GetComponent<RequestableOwnershipGuard>();
		this.guard.AddCallbackTarget(this);
	}

	// Token: 0x0600002E RID: 46 RVA: 0x000027F4 File Offset: 0x000009F4
	public void BindController(bool leftHand)
	{
		this.xrNode = (leftHand ? XRNode.LeftHand : XRNode.RightHand);
		this.heldByLocalPlayer = true;
		if (!leftHand)
		{
			if (!this.snapTurn)
			{
				this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			}
			if (this.snapTurn != null)
			{
				this.snapTurnOverride = true;
				this.snapTurn.SetTurningOverride(this);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.guard.TransferOwnership(PhotonNetwork.LocalPlayer, "");
		}
		else if (!this.guard.isMine)
		{
			this.guard.RequestOwnership(new Action(this.OnOwnershipSuccess), new Action(this.OnOwnershipFail));
		}
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnInputUpdate));
		PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
	}

	// Token: 0x0600002F RID: 47 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnOwnershipSuccess()
	{
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000028C7 File Offset: 0x00000AC7
	private void OnOwnershipFail()
	{
		this.ForceRelease();
	}

	// Token: 0x06000031 RID: 49 RVA: 0x000028CF File Offset: 0x00000ACF
	public void UnbindController()
	{
		this.heldByLocalPlayer = false;
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
		this.OnInputUpdate();
		ControllerInputPoller.RemoveUpdateCallback(new Action(this.OnInputUpdate));
	}

	// Token: 0x06000032 RID: 50 RVA: 0x0000290C File Offset: 0x00000B0C
	private void OnInputUpdate()
	{
		ArcadeButtons arcadeButtons = (ArcadeButtons)0;
		if (this.heldByLocalPlayer)
		{
			arcadeButtons |= ArcadeButtons.GRAB;
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.UP;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).y < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.DOWN;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x < -0.5f)
			{
				arcadeButtons |= ArcadeButtons.LEFT;
			}
			if (ControllerInputPoller.Primary2DAxis(this.xrNode).x > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.RIGHT;
			}
			if (ControllerInputPoller.PrimaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B0;
			}
			if (ControllerInputPoller.SecondaryButtonPress(this.xrNode))
			{
				arcadeButtons |= ArcadeButtons.B1;
			}
			if (ControllerInputPoller.TriggerFloat(this.xrNode) > 0.5f)
			{
				arcadeButtons |= ArcadeButtons.TRIGGER;
			}
		}
		if (arcadeButtons != this.currentButtonState)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x000029F8 File Offset: 0x00000BF8
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner)
		{
			return;
		}
		ArcadeButtons arcadeButtons = (ArcadeButtons)((int)stream.ReceiveNext());
		if (arcadeButtons != this.currentButtonState && this.machine != null)
		{
			this.machine.OnJoystickStateChange(this.player, arcadeButtons);
		}
		this.currentButtonState = arcadeButtons;
		this.machine.ReadPlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002A68 File Offset: 0x00000C68
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext((int)this.currentButtonState);
		this.machine.WritePlayerDataPUN(this.player, stream, info);
	}

	// Token: 0x06000035 RID: 53 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ReceiveRemoteState(ArcadeButtons newState)
	{
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002A8E File Offset: 0x00000C8E
	public bool TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002A96 File Offset: 0x00000C96
	public override bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return !this.machine.IsControllerInUse(this.player);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002AAC File Offset: 0x00000CAC
	public void ForceRelease()
	{
		this.heldByLocalPlayer = false;
		this.currentButtonState = (ArcadeButtons)0;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002ABC File Offset: 0x00000CBC
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		if (this.heldByLocalPlayer && (toPlayer == null || !toPlayer.IsLocal))
		{
			this.ForceRelease();
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002AD7 File Offset: 0x00000CD7
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002AD7 File Offset: 0x00000CD7
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		return !this.heldByLocalPlayer;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyOwnerLeft()
	{
	}

	// Token: 0x0600003D RID: 61 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnMyCreatorLeft()
	{
	}

	// Token: 0x0400001F RID: 31
	private XRNode xrNode;

	// Token: 0x04000023 RID: 35
	private ArcadeMachine machine;

	// Token: 0x04000024 RID: 36
	private RequestableOwnershipGuard guard;

	// Token: 0x04000025 RID: 37
	private GorillaSnapTurn snapTurn;

	// Token: 0x04000026 RID: 38
	private bool snapTurnOverride;
}
