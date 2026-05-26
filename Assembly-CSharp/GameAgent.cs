using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200069E RID: 1694
public class GameAgent : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06002A21 RID: 10785 RVA: 0x000E30AC File Offset: 0x000E12AC
	// (remove) Token: 0x06002A22 RID: 10786 RVA: 0x000E30E4 File Offset: 0x000E12E4
	public event GameAgent.StateChangedEvent onBodyStateChanged;

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06002A23 RID: 10787 RVA: 0x000E311C File Offset: 0x000E131C
	// (remove) Token: 0x06002A24 RID: 10788 RVA: 0x000E3154 File Offset: 0x000E1354
	public event GameAgent.StateChangedEvent onBehaviorStateChanged;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06002A25 RID: 10789 RVA: 0x000E318C File Offset: 0x000E138C
	// (remove) Token: 0x06002A26 RID: 10790 RVA: 0x000E31C4 File Offset: 0x000E13C4
	public event GameAgent.NavigationLinkReachedEvent onReachedNavigationLink;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06002A27 RID: 10791 RVA: 0x000E31FC File Offset: 0x000E13FC
	// (remove) Token: 0x06002A28 RID: 10792 RVA: 0x000E3234 File Offset: 0x000E1434
	public event GameAgent.JumpRequestedEvent onJumpRequested;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06002A29 RID: 10793 RVA: 0x000E326C File Offset: 0x000E146C
	// (remove) Token: 0x06002A2A RID: 10794 RVA: 0x000E32A4 File Offset: 0x000E14A4
	public event GameAgent.NavigationFailedEvent onNavigationFailed;

	// Token: 0x06002A2B RID: 10795 RVA: 0x000E32D9 File Offset: 0x000E14D9
	public GameAgentManager GetGameAgentManager()
	{
		return this.entity.manager.gameAgentManager;
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x000E32EB File Offset: 0x000E14EB
	private void Awake()
	{
		this.agentComponents = new List<IGameAgentComponent>(1);
		base.GetComponentsInChildren<IGameAgentComponent>(this.agentComponents);
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x000E3305 File Offset: 0x000E1505
	public void OnEntityInit()
	{
		this.GetGameAgentManager().AddGameAgent(this);
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x000E3313 File Offset: 0x000E1513
	public void OnEntityDestroy()
	{
		this.GetGameAgentManager().RemoveGameAgent(this);
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x000E3321 File Offset: 0x000E1521
	public void OnBehaviorStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBehaviorStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000E3334 File Offset: 0x000E1534
	public void OnBodyStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBodyStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000E3348 File Offset: 0x000E1548
	public void OnThink(float deltaTime)
	{
		if (!this.pauseEntityThink)
		{
			for (int i = 0; i < this.agentComponents.Count; i++)
			{
				this.agentComponents[i].OnEntityThink(deltaTime);
			}
		}
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000E3388 File Offset: 0x000E1588
	public void OnUpdate()
	{
		if (this.navAgent == null)
		{
			return;
		}
		if (this.navAgent.isOnNavMesh)
		{
			this.lastPosOnNavMesh = this.navAgent.transform.position;
		}
		if (!this.navAgent.autoTraverseOffMeshLink && !this.wasOnOffMeshNavLink && this.navAgent.isOnOffMeshLink)
		{
			if (this.entity.IsAuthority())
			{
				if ((this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.startPos).sqrMagnitude < (this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.endPos).sqrMagnitude)
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.endPos, 1f, 1f);
				}
				else
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.startPos, 1f, 1f);
				}
			}
			GameAgent.NavigationLinkReachedEvent navigationLinkReachedEvent = this.onReachedNavigationLink;
			if (navigationLinkReachedEvent != null)
			{
				navigationLinkReachedEvent(this.navAgent.currentOffMeshLinkData);
			}
		}
		this.wasOnOffMeshNavLink = this.navAgent.isOnOffMeshLink;
		if (!this.hasNotifiedNavigationFailure && !this.navAgent.pathPending && (this.navAgent.pathStatus == NavMeshPathStatus.PathPartial || this.navAgent.pathStatus == NavMeshPathStatus.PathInvalid))
		{
			GameAgent.NavigationFailedEvent navigationFailedEvent = this.onNavigationFailed;
			if (navigationFailedEvent != null)
			{
				navigationFailedEvent(this.navAgent.pathStatus, this.navAgent.destination, this.navAgent.remainingDistance);
			}
			this.hasNotifiedNavigationFailure = true;
		}
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000E3571 File Offset: 0x000E1771
	public void OnJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		GameAgent.JumpRequestedEvent jumpRequestedEvent = this.onJumpRequested;
		if (jumpRequestedEvent == null)
		{
			return;
		}
		jumpRequestedEvent(start, end, heightScale, speedScale);
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000E3588 File Offset: 0x000E1788
	public bool IsOnNavMesh()
	{
		return this.navAgent != null && this.navAgent.isOnNavMesh;
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000E35A5 File Offset: 0x000E17A5
	public Vector3 GetLastPosOnNavMesh()
	{
		return this.lastPosOnNavMesh;
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x000E35B0 File Offset: 0x000E17B0
	public void RequestDestination(Vector3 dest)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!this.IsOnNavMesh())
		{
			dest = this.lastPosOnNavMesh;
		}
		if (Vector3.Distance(this.lastRequestedDest, dest) < 0.5f)
		{
			return;
		}
		this.lastRequestedDest = dest;
		if (this.entity.IsAuthority())
		{
			this.GetGameAgentManager().RequestDestination(this, dest);
		}
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000E3610 File Offset: 0x000E1810
	public void RequestBehaviorChange(byte behavior)
	{
		this.GetGameAgentManager().RequestBehavior(this, behavior);
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x000E361F File Offset: 0x000E181F
	public void RequestStateChange(byte state)
	{
		this.GetGameAgentManager().RequestState(this, state);
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x000E362E File Offset: 0x000E182E
	public void RequestTarget(NetPlayer targetPlayer)
	{
		this.GetGameAgentManager().RequestTarget(this, targetPlayer);
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000E3640 File Offset: 0x000E1840
	public void ApplyDestination(Vector3 dest)
	{
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(dest, out navMeshHit, 1.5f, -1))
		{
			return;
		}
		dest = navMeshHit.position;
		this.lastReceivedDest = dest;
		this.hasNotifiedNavigationFailure = false;
		if (this.navAgent != null && this.navAgent.isOnNavMesh)
		{
			this.navAgent.destination = dest;
		}
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000E369C File Offset: 0x000E189C
	public void SetDisableNetworkSync(bool disable)
	{
		this.disableNetworkSync = disable;
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000E36A5 File Offset: 0x000E18A5
	public void SetIsPathing(bool isPathing, bool ignoreRigiBody = false)
	{
		if (this.navAgent != null)
		{
			this.navAgent.enabled = isPathing;
		}
		if (!ignoreRigiBody && this.rigidBody != null)
		{
			this.rigidBody.isKinematic = isPathing;
		}
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000E36DE File Offset: 0x000E18DE
	public void SetStopped(bool stopMovement)
	{
		if (this.navAgent != null)
		{
			this.navAgent.isStopped = stopMovement;
		}
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x000E36FA File Offset: 0x000E18FA
	public void SetSpeed(float speed)
	{
		if (this.navAgent != null)
		{
			this.navAgent.speed = speed;
		}
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x000E3716 File Offset: 0x000E1916
	public void SetVelocity(Vector3 vel)
	{
		if (this.navAgent != null)
		{
			this.navAgent.velocity = vel;
		}
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x000E3732 File Offset: 0x000E1932
	public void ClearLastRequestedDestination()
	{
		this.lastRequestedDest = Vector3.one * 10000f;
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x000E374C File Offset: 0x000E194C
	public void ApplyNetworkUpdate(Vector3 position, Quaternion rotation)
	{
		if (this.disableNetworkSync)
		{
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > this.networkPositionCorrectionDist * this.networkPositionCorrectionDist && this.navAgent != null)
		{
			this.navAgent.Warp(position);
			this.navAgent.destination = this.lastReceivedDest;
		}
		base.transform.rotation = rotation;
		if (this.rigidBody != null)
		{
			this.rigidBody.rotation = rotation;
		}
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000E37DC File Offset: 0x000E19DC
	public static void UpdateFacing(Transform transform, NavMeshAgent navAgent, NetPlayer targetPlayer, float turnspeed = 3600f)
	{
		Transform target = null;
		Vector3 forward = transform.forward;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				target = grplayer.transform;
			}
		}
		GameAgent.UpdateFacingTarget(transform, navAgent, target, turnspeed);
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000E3824 File Offset: 0x000E1A24
	public static void UpdateFacingTarget(Transform transform, NavMeshAgent navAgent, Transform target, float turnspeed = 3600f)
	{
		Vector3 forward = transform.forward;
		if (target != null)
		{
			Vector3 position = target.position;
			Vector3 position2 = transform.position;
			Vector3 a = position - position2;
			a.y = 0f;
			float magnitude = a.magnitude;
			if (magnitude > 0f)
			{
				forward = a / magnitude;
			}
		}
		else
		{
			Vector3 a2 = (navAgent == null) ? Vector3.zero : navAgent.desiredVelocity;
			a2.y = 0f;
			float magnitude2 = a2.magnitude;
			if (magnitude2 > 0f)
			{
				forward = a2 / magnitude2;
			}
		}
		Quaternion b = Quaternion.LookRotation(forward);
		if (navAgent != null && navAgent.speed > 0f)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, b) * Time.deltaTime, 0f, 1f));
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Clamp(turnspeed / Quaternion.Angle(transform.rotation, b) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000E3950 File Offset: 0x000E1B50
	public static void UpdateFacingForward(Transform transform, NavMeshAgent navAgent, float turnspeed = 3600f)
	{
		Vector3 a = (navAgent == null) ? Vector3.zero : navAgent.desiredVelocity;
		a.y = 0f;
		float magnitude = a.magnitude;
		if (magnitude <= 0f)
		{
			return;
		}
		Vector3 facingDir = a / magnitude;
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000E39A4 File Offset: 0x000E1BA4
	public static void UpdateFacingPos(Transform transform, NavMeshAgent navAgent, Vector3 facingPos, float turnspeed = 3600f)
	{
		Vector3 facingDir = facingPos - transform.position;
		facingDir.y = 0f;
		facingDir.Normalize();
		GameAgent.UpdateFacingDir(transform, navAgent, facingDir, turnspeed);
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x000E39DC File Offset: 0x000E1BDC
	public static void UpdateFacingDir(Transform transform, NavMeshAgent navAgent, Vector3 facingDir, float turnspeed = 3600f)
	{
		float num = (navAgent == null) ? 0f : navAgent.speed;
		Quaternion b = Quaternion.LookRotation(facingDir);
		transform.rotation = Quaternion.Lerp(transform.rotation, b, Mathf.Clamp(turnspeed * num / Quaternion.Angle(transform.rotation, b) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x04003703 RID: 14083
	public GameEntity entity;

	// Token: 0x04003704 RID: 14084
	public NavMeshAgent navAgent;

	// Token: 0x04003705 RID: 14085
	public Rigidbody rigidBody;

	// Token: 0x04003706 RID: 14086
	public float networkPositionCorrectionDist = 2.5f;

	// Token: 0x04003707 RID: 14087
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003708 RID: 14088
	private bool disableNetworkSync;

	// Token: 0x04003709 RID: 14089
	private Vector3 lastPosOnNavMesh;

	// Token: 0x0400370A RID: 14090
	private Vector3 lastRequestedDest;

	// Token: 0x0400370B RID: 14091
	private Vector3 lastReceivedDest;

	// Token: 0x04003711 RID: 14097
	private bool hasNotifiedNavigationFailure;

	// Token: 0x04003712 RID: 14098
	private List<IGameAgentComponent> agentComponents;

	// Token: 0x04003713 RID: 14099
	private bool wasOnOffMeshNavLink;

	// Token: 0x04003714 RID: 14100
	public bool navAgentless;

	// Token: 0x04003715 RID: 14101
	[ReadOnly]
	public bool pauseEntityThink;

	// Token: 0x0200069F RID: 1695
	// (Invoke) Token: 0x06002A4A RID: 10826
	public delegate void StateChangedEvent(byte newState);

	// Token: 0x020006A0 RID: 1696
	// (Invoke) Token: 0x06002A4E RID: 10830
	public delegate void NavigationLinkReachedEvent(OffMeshLinkData linkData);

	// Token: 0x020006A1 RID: 1697
	// (Invoke) Token: 0x06002A52 RID: 10834
	public delegate void JumpRequestedEvent(Vector3 start, Vector3 end, float heightScale, float speedScale);

	// Token: 0x020006A2 RID: 1698
	// (Invoke) Token: 0x06002A56 RID: 10838
	public delegate void NavigationFailedEvent(NavMeshPathStatus status, Vector3 destination, float remainingDistance);
}
