using System;
using System.Collections.Generic;
using GorillaNetworking;
using GorillaTagScripts.GhostReactor.SoakTasks;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000710 RID: 1808
public class GhostReactorSoak
{
	// Token: 0x06002E0C RID: 11788 RVA: 0x000FC71C File Offset: 0x000FA91C
	public void Setup(GRPlayer grPlayer)
	{
		this.grPlayer = grPlayer;
		GhostReactorSoak.instance = this;
		if (this.IsSoaking())
		{
			Debug.LogFormat("Soak Setup {0} InRoom {1} Auth {2}", new object[]
			{
				this.state,
				this.grManager != null && this.grManager.IsAuthority(),
				PhotonNetwork.InRoom
			});
		}
		this._soakTasks.Add(new SoakTaskGrabThrow(grPlayer));
		this._soakTasks.Add(new SoakTaskDepositCollectibles(grPlayer));
		this._soakTasks.Add(new SoakTaskBreakable(grPlayer));
		this._soakTasks.Add(new SoakTaskHitEnemy(grPlayer));
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x00002076 File Offset: 0x00000276
	public bool IsSoaking()
	{
		return false;
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000FC7D4 File Offset: 0x000FA9D4
	public void OnUpdate()
	{
		if (!this.IsSoaking())
		{
			return;
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.grPlayer.gamePlayer.rig.zoneEntity.currentZone);
		if (managerForZone == null)
		{
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			if (!PhotonNetwork.InRoom && timeAsDouble > this.reconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Connecting);
				return;
			}
			break;
		case GhostReactorSoak.State.Connecting:
			if (this.grManager.IsZoneActive())
			{
				this.SetState(GhostReactorSoak.State.Active);
				return;
			}
			if (timeAsDouble > this.stateStartTime + 15.0)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			break;
		case GhostReactorSoak.State.Active:
			this.UpdateActive();
			if (timeAsDouble > this.disconnectTime)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
				return;
			}
			if (!PhotonNetwork.InRoom)
			{
				this.SetState(GhostReactorSoak.State.Disconnected);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000FC8BC File Offset: 0x000FAABC
	private int GetActorNumber()
	{
		if (this.grPlayer.gamePlayer.rig.OwningNetPlayer == null)
		{
			return -1;
		}
		return this.grPlayer.gamePlayer.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000FC8F4 File Offset: 0x000FAAF4
	public void SetState(GhostReactorSoak.State newState)
	{
		this.state = newState;
		this.stateStartTime = Time.timeAsDouble;
		Debug.LogFormat("Soak Set State {0} Player {1} InRoom {2} Auth {3}", new object[]
		{
			this.state,
			this.GetActorNumber(),
			this.grManager != null && this.grManager.IsAuthority(),
			PhotonNetwork.InRoom
		});
		switch (this.state)
		{
		case GhostReactorSoak.State.Disconnected:
			this.LeaveRoom();
			this.reconnectTime = this.stateStartTime + (double)Random.Range(3f, 6f);
			return;
		case GhostReactorSoak.State.Connecting:
			this.JoinRoom();
			return;
		case GhostReactorSoak.State.Active:
			this.disconnectTime = this.stateStartTime + (double)Random.Range(5f, 60f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000FC9D2 File Offset: 0x000FABD2
	public void JoinRoom()
	{
		Debug.LogFormat("Soak Join Room {0}", new object[]
		{
			"AKJSOAK"
		});
		PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("AKJSOAK", JoinType.Solo);
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000FC9FE File Offset: 0x000FABFE
	public void LeaveRoom()
	{
		Debug.LogFormat("Soak Leave Room", Array.Empty<object>());
		NetworkSystem.Instance.ReturnToSinglePlayer();
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000FCA1C File Offset: 0x000FAC1C
	private void UpdateActive()
	{
		if (this._activeTask != null)
		{
			bool flag = false;
			if (!this._activeTask.Update())
			{
				Debug.LogError(string.Format("Failed to execute soak task of type {0}", this._activeTask.GetType()));
				flag = true;
			}
			if (flag || this._activeTask.Complete)
			{
				this._activeTask.Reset();
				this._activeTask = null;
				return;
			}
		}
		else if (Random.value <= 0.005f)
		{
			int index = Random.Range(0, this._soakTasks.Count);
			this._activeTask = this._soakTasks[index];
		}
	}

	// Token: 0x04003AB9 RID: 15033
	public static GhostReactorSoak instance;

	// Token: 0x04003ABA RID: 15034
	private const string SOAK_ROOM = "AKJSOAK";

	// Token: 0x04003ABB RID: 15035
	private const float MIN_CONNECTED_TIME = 5f;

	// Token: 0x04003ABC RID: 15036
	private const float MAX_CONNECTED_TIME = 60f;

	// Token: 0x04003ABD RID: 15037
	private const float MIN_DISCONNECTED_TIME = 3f;

	// Token: 0x04003ABE RID: 15038
	private const float MAX_DISCONNECTED_TIME = 6f;

	// Token: 0x04003ABF RID: 15039
	public GRPlayer grPlayer;

	// Token: 0x04003AC0 RID: 15040
	public GhostReactorManager grManager;

	// Token: 0x04003AC1 RID: 15041
	public GhostReactorSoak.State state;

	// Token: 0x04003AC2 RID: 15042
	public double stateStartTime;

	// Token: 0x04003AC3 RID: 15043
	public double reconnectTime;

	// Token: 0x04003AC4 RID: 15044
	public double disconnectTime;

	// Token: 0x04003AC5 RID: 15045
	public const float START_NEW_TASK_ODDS = 0.005f;

	// Token: 0x04003AC6 RID: 15046
	private IGhostReactorSoakTask _activeTask;

	// Token: 0x04003AC7 RID: 15047
	private readonly List<IGhostReactorSoakTask> _soakTasks = new List<IGhostReactorSoakTask>();

	// Token: 0x02000711 RID: 1809
	public enum State
	{
		// Token: 0x04003AC9 RID: 15049
		Disconnected,
		// Token: 0x04003ACA RID: 15050
		Connecting,
		// Token: 0x04003ACB RID: 15051
		Active
	}
}
