using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaTag.Rendering;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Token: 0x02000218 RID: 536
[NetworkBehaviourWeaved(3)]
public class AngryBeeSwarm : NetworkComponent
{
	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000E09 RID: 3593 RVA: 0x0004CB00 File Offset: 0x0004AD00
	public bool isDormant
	{
		get
		{
			return this.currentState == AngryBeeSwarm.ChaseState.Dormant;
		}
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x0004CB0C File Offset: 0x0004AD0C
	protected override void Awake()
	{
		base.Awake();
		AngryBeeSwarm.instance = this;
		this.targetPlayer = null;
		this.currentState = AngryBeeSwarm.ChaseState.Dormant;
		this.grabTimestamp = -this.minGrabCooldown;
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0004CB5C File Offset: 0x0004AD5C
	private void InitializeSwarm()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.beeAnimator.transform.localPosition = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x0004CBA8 File Offset: 0x0004ADA8
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = AngryBeeSwarm.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			AngryBeeSwarm.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case AngryBeeSwarm.ChaseState.Dormant:
				if (Application.isEditor && Keyboard.current[Key.Space].wasPressedThisFrame)
				{
					this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
				}
				break;
			case AngryBeeSwarm.ChaseState.InitialEmerge:
				if (Time.time > this.emergeStartedTimestamp + this.totalTimeToEmerge)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Chasing;
				}
				break;
			case (AngryBeeSwarm.ChaseState)3:
				break;
			case AngryBeeSwarm.ChaseState.Chasing:
				if (this.followTarget == null || this.targetPlayer == null || Time.time > this.NextRefreshClosestPlayerTimestamp)
				{
					this.ChooseClosestTarget();
					if (this.followTarget != null)
					{
						this.BoredToDeathAtTimestamp = -1f;
					}
					else if (this.BoredToDeathAtTimestamp < 0f)
					{
						this.BoredToDeathAtTimestamp = Time.time + this.boredAfterDuration;
					}
				}
				if (this.BoredToDeathAtTimestamp >= 0f && Time.time > this.BoredToDeathAtTimestamp)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Dormant;
				}
				else if (!(this.followTarget == null) && (this.followTarget.position - this.beeAnimator.transform.position).magnitude < this.catchDistance)
				{
					float num = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
					if (this.followTarget.position.y > num)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Grabbing;
					}
				}
				break;
			default:
				if (chaseState == AngryBeeSwarm.ChaseState.Grabbing)
				{
					if (Time.time > this.grabTimestamp + this.grabDuration)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Dormant;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0004CD8C File Offset: 0x0004AF8C
	public void UpdateState()
	{
		AngryBeeSwarm.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			if (NetworkSystem.Instance.InRoom)
			{
				this.SwarmEmergeUpdateShared();
				return;
			}
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				return;
			}
			break;
		default:
			if (chaseState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.RiseGrabbedLocalPlayer();
				}
				this.GrabBodyShared();
			}
			break;
		}
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0004CE1B File Offset: 0x0004B01B
	public void Emerge(Vector3 fromPosition, Vector3 toPosition)
	{
		base.transform.position = fromPosition;
		this.emergeFromPosition = fromPosition;
		this.emergeToPosition = toPosition;
		this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
		this.emergeStartedTimestamp = Time.time;
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0004CE4C File Offset: 0x0004B04C
	private void OnChangeState(AngryBeeSwarm.ChaseState newState)
	{
		switch (newState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
			if (this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				base.transform.position = new Vector3(0f, -9999f, 0f);
				this.InitializeSwarm();
			}
			this.SetInitialRotations();
			return;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			this.emergeStartedTimestamp = Time.time;
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(0f);
			if (base.IsMine)
			{
				this.currentSpeed = 0f;
				this.ChooseClosestTarget();
			}
			this.SetInitialRotations();
			return;
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(1f);
			this.ResetPath();
			this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
			this.BoredToDeathAtTimestamp = -1f;
			return;
		default:
		{
			if (newState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.grabTimestamp = Time.time;
			this.beeAnimator.transform.localPosition = this.ghostOffsetGrabbingLocal;
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
			if (vrrig != null)
			{
				this.followTarget = vrrig.transform;
			}
			break;
		}
		}
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0004CFF4 File Offset: 0x0004B1F4
	private void ChooseClosestTarget()
	{
		float num = Mathf.Lerp(this.initialRangeLimit, this.finalRangeLimit, (Time.time + this.totalTimeToEmerge - this.emergeStartedTimestamp) / this.rangeLimitBlendDuration);
		float num2 = num * num;
		VRRig vrrig = null;
		float num3 = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			VRRig rig = rigContainer.Rig;
			if (rig.head != null && !(rig.head.rigTarget == null) && rig.head.rigTarget.position.y > num3)
			{
				float sqrMagnitude = (base.transform.position - rig.head.rigTarget.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					vrrig = rig;
				}
			}
		}
		if (vrrig.IsNotNull())
		{
			this.targetPlayer = vrrig.creator;
			this.followTarget = vrrig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
		}
		else
		{
			this.targetPlayer = null;
			this.followTarget = null;
		}
		this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0004D158 File Offset: 0x0004B358
	private void SetInitialRotations()
	{
		this.beeAnimator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0004D170 File Offset: 0x0004B370
	private void SwarmEmergeUpdateShared()
	{
		if (Time.time < this.emergeStartedTimestamp + this.totalTimeToEmerge)
		{
			float emergeFraction = (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge;
			if (base.IsMine)
			{
				base.transform.position = Vector3.Lerp(this.emergeFromPosition, this.emergeToPosition, (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge);
			}
			this.beeAnimator.SetEmergeFraction(emergeFraction);
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0004D1E8 File Offset: 0x0004B3E8
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTimestamp + this.minGrabCooldown)
		{
			this.grabTimestamp = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTimestamp + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0004D298 File Offset: 0x0004B498
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.pathPoints[this.pathPoints.Count - 1] = destination;
		Vector3 vector = this.pathPoints[this.currentPathPointIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentPathPointIdx + 1 < this.pathPoints.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentPathPointIdx++;
		}
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0004D3A8 File Offset: 0x0004B5A8
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.pathPoints = new List<Vector3>();
		foreach (Vector3 a in this.path.corners)
		{
			this.pathPoints.Add(a + Vector3.up * this.heightAboveNavmesh);
		}
		this.pathPoints.Add(destination);
		this.currentPathPointIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0004D47C File Offset: 0x0004B67C
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x0004D488 File Offset: 0x0004B688
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseInterval)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			float num = ZoneShaderSettings.GetWaterY() + this.MinHeightAboveWater;
			Vector3 position = this.followTarget.position;
			if (position.y < num)
			{
				position.y = num;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, this.currentSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0004D540 File Offset: 0x0004B740
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.beeAnimator.transform.localPosition = this.noisyOffset;
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0004D5AD File Offset: 0x0004B7AD
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000E1A RID: 3610 RVA: 0x0004D5E9 File Offset: 0x0004B7E9
	// (set) Token: 0x06000E1B RID: 3611 RVA: 0x0004D613 File Offset: 0x0004B813
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe BeeSwarmData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BeeSwarmData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BeeSwarmData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0004D63E File Offset: 0x0004B83E
	public override void WriteDataFusion()
	{
		this.Data = new BeeSwarmData(this.targetPlayer.ActorNumber, (int)this.currentState, this.currentSpeed);
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0004D664 File Offset: 0x0004B864
	public override void ReadDataFusion()
	{
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(this.Data.TargetActorNumber);
		this.currentState = (AngryBeeSwarm.ChaseState)this.Data.CurrentState;
		if (float.IsFinite(this.Data.CurrentSpeed))
		{
			this.currentSpeed = this.Data.CurrentSpeed;
		}
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0004D6CC File Offset: 0x0004B8CC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.Equals(PhotonNetwork.MasterClient))
		{
			return;
		}
		NetPlayer netPlayer = this.targetPlayer;
		stream.SendNext((netPlayer != null) ? netPlayer.ActorNumber : -1);
		stream.SendNext(this.currentState);
		stream.SendNext(this.currentSpeed);
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0004D734 File Offset: 0x0004B934
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (AngryBeeSwarm.ChaseState)stream.ReceiveNext();
		float f = (float)stream.ReceiveNext();
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0004D798 File Offset: 0x0004B998
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0004D7B6 File Offset: 0x0004B9B6
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeSwarm();
		}
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0004D7CA File Offset: 0x0004B9CA
	private void TestEmerge()
	{
		this.Emerge(this.testEmergeFrom.transform.position, this.testEmergeTo.transform.position);
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0004D880 File Offset: 0x0004BA80
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0004D898 File Offset: 0x0004BA98
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040010E0 RID: 4320
	public static AngryBeeSwarm instance;

	// Token: 0x040010E1 RID: 4321
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x040010E2 RID: 4322
	public Transform followTarget;

	// Token: 0x040010E3 RID: 4323
	[SerializeField]
	private float velocityStep = 1f;

	// Token: 0x040010E4 RID: 4324
	private float currentSpeed;

	// Token: 0x040010E5 RID: 4325
	[SerializeField]
	private float velocityIncreaseInterval = 20f;

	// Token: 0x040010E6 RID: 4326
	public Vector3 noisyOffset;

	// Token: 0x040010E7 RID: 4327
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x040010E8 RID: 4328
	private float emergeStartedTimestamp;

	// Token: 0x040010E9 RID: 4329
	private float grabTimestamp;

	// Token: 0x040010EA RID: 4330
	private float lastSpeedIncreased;

	// Token: 0x040010EB RID: 4331
	[SerializeField]
	private float totalTimeToEmerge;

	// Token: 0x040010EC RID: 4332
	[SerializeField]
	private float catchDistance;

	// Token: 0x040010ED RID: 4333
	[SerializeField]
	private float grabDuration;

	// Token: 0x040010EE RID: 4334
	[SerializeField]
	private float grabSpeed = 1f;

	// Token: 0x040010EF RID: 4335
	[SerializeField]
	private float minGrabCooldown;

	// Token: 0x040010F0 RID: 4336
	[SerializeField]
	private float initialRangeLimit;

	// Token: 0x040010F1 RID: 4337
	[SerializeField]
	private float finalRangeLimit;

	// Token: 0x040010F2 RID: 4338
	[SerializeField]
	private float rangeLimitBlendDuration;

	// Token: 0x040010F3 RID: 4339
	[SerializeField]
	private float boredAfterDuration;

	// Token: 0x040010F4 RID: 4340
	public NetPlayer targetPlayer;

	// Token: 0x040010F5 RID: 4341
	public AngryBeeAnimator beeAnimator;

	// Token: 0x040010F6 RID: 4342
	public AngryBeeSwarm.ChaseState currentState;

	// Token: 0x040010F7 RID: 4343
	public AngryBeeSwarm.ChaseState lastState;

	// Token: 0x040010F8 RID: 4344
	public NetPlayer grabbedPlayer;

	// Token: 0x040010F9 RID: 4345
	private bool targetIsOnNavMesh;

	// Token: 0x040010FA RID: 4346
	private const float navMeshSampleRange = 5f;

	// Token: 0x040010FB RID: 4347
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x040010FC RID: 4348
	public float hapticDuration = 1.5f;

	// Token: 0x040010FD RID: 4349
	public float MinHeightAboveWater = 0.5f;

	// Token: 0x040010FE RID: 4350
	public float PlayerMinHeightAboveWater = 0.5f;

	// Token: 0x040010FF RID: 4351
	public float RefreshClosestPlayerInterval = 1f;

	// Token: 0x04001100 RID: 4352
	private float NextRefreshClosestPlayerTimestamp = 1f;

	// Token: 0x04001101 RID: 4353
	private float BoredToDeathAtTimestamp = -1f;

	// Token: 0x04001102 RID: 4354
	[SerializeField]
	private Transform testEmergeFrom;

	// Token: 0x04001103 RID: 4355
	[SerializeField]
	private Transform testEmergeTo;

	// Token: 0x04001104 RID: 4356
	private Vector3 emergeFromPosition;

	// Token: 0x04001105 RID: 4357
	private Vector3 emergeToPosition;

	// Token: 0x04001106 RID: 4358
	private NavMeshPath path;

	// Token: 0x04001107 RID: 4359
	public List<Vector3> pathPoints;

	// Token: 0x04001108 RID: 4360
	public int currentPathPointIdx;

	// Token: 0x04001109 RID: 4361
	private float nextPathTimestamp;

	// Token: 0x0400110A RID: 4362
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BeeSwarmData _Data;

	// Token: 0x02000219 RID: 537
	public enum ChaseState
	{
		// Token: 0x0400110C RID: 4364
		Dormant = 1,
		// Token: 0x0400110D RID: 4365
		InitialEmerge,
		// Token: 0x0400110E RID: 4366
		Chasing = 4,
		// Token: 0x0400110F RID: 4367
		Grabbing = 8
	}
}
