using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020008D7 RID: 2263
[NetworkBehaviourWeaved(5)]
public class HalloweenGhostChaser : NetworkComponent
{
	// Token: 0x06003B30 RID: 15152 RVA: 0x0014428F File Offset: 0x0014248F
	protected override void Awake()
	{
		base.Awake();
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<NetPlayer>();
	}

	// Token: 0x06003B31 RID: 15153 RVA: 0x001442C4 File Offset: 0x001424C4
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06003B32 RID: 15154 RVA: 0x001442F4 File Offset: 0x001424F4
	private void InitializeGhost()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06003B33 RID: 15155 RVA: 0x00144394 File Offset: 0x00142594
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < VRRigCache.ActiveRigContainers.Count; j++)
						{
							if ((VRRigCache.ActiveRigContainers[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(VRRigCache.ActiveRigContainers[j].Creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
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

	// Token: 0x06003B34 RID: 15156 RVA: 0x00144614 File Offset: 0x00142814
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
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
					this.MoveHead();
				}
			}
			else if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x06003B35 RID: 15157 RVA: 0x001446F8 File Offset: 0x001428F8
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlay();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.GTPlayOneShot(this.gong, 1f);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlay();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x001449F0 File Offset: 0x00142BF0
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x00144A60 File Offset: 0x00142C60
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Creator != null && x.Creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, VRRigCache.ActiveRigContainers.Count);
		}
		this.possibleTarget.Clear();
		if (num < VRRigCache.ActiveRigContainers.Count)
		{
			VRRig rig = VRRigCache.ActiveRigContainers[num].Rig;
			this.targetPlayer = rig.creator;
			this.followTarget = rig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x00144B50 File Offset: 0x00142D50
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x00144BCC File Offset: 0x00142DCC
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x00144C38 File Offset: 0x00142E38
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x00144CD4 File Offset: 0x00142ED4
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x00144D84 File Offset: 0x00142F84
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x06003B3D RID: 15165 RVA: 0x00144E94 File Offset: 0x00143094
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 a in this.path.corners)
		{
			this.points.Add(a + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06003B3E RID: 15166 RVA: 0x00144F68 File Offset: 0x00143168
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06003B3F RID: 15167 RVA: 0x00144F74 File Offset: 0x00143174
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x00145048 File Offset: 0x00143248
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x001450E6 File Offset: 0x001432E6
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x06003B42 RID: 15170 RVA: 0x00145122 File Offset: 0x00143322
	// (set) Token: 0x06003B43 RID: 15171 RVA: 0x0014514C File Offset: 0x0014334C
	[Networked]
	[NetworkedWeaved(0, 5)]
	public unsafe HalloweenGhostChaser.GhostData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HalloweenGhostChaser.GhostData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HalloweenGhostChaser.GhostData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x00145178 File Offset: 0x00143378
	public override void WriteDataFusion()
	{
		HalloweenGhostChaser.GhostData data = default(HalloweenGhostChaser.GhostData);
		NetPlayer netPlayer = this.targetPlayer;
		data.TargetActorNumber = ((netPlayer != null) ? netPlayer.ActorNumber : -1);
		data.CurrentState = (int)this.currentState;
		data.SpawnIndex = this.spawnIndex;
		data.CurrentSpeed = this.currentSpeed;
		data.IsSummoned = this.isSummoned;
		this.Data = data;
	}

	// Token: 0x06003B45 RID: 15173 RVA: 0x001451E8 File Offset: 0x001433E8
	public override void ReadDataFusion()
	{
		int targetActorNumber = this.Data.TargetActorNumber;
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		this.currentState = (HalloweenGhostChaser.ChaseState)this.Data.CurrentState;
		this.spawnIndex = this.Data.SpawnIndex;
		float f = this.Data.CurrentSpeed;
		this.isSummoned = this.Data.IsSummoned;
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x06003B46 RID: 15174 RVA: 0x00145268 File Offset: 0x00143468
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (this.targetPlayer == null)
		{
			stream.SendNext(-1);
		}
		else
		{
			stream.SendNext(this.targetPlayer.ActorNumber);
		}
		stream.SendNext(this.currentState);
		stream.SendNext(this.spawnIndex);
		stream.SendNext(this.currentSpeed);
		stream.SendNext(this.isSummoned);
	}

	// Token: 0x06003B47 RID: 15175 RVA: 0x00145304 File Offset: 0x00143504
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		int playerID = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		float f = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
		if (float.IsFinite(f))
		{
			this.currentSpeed = f;
		}
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x00145399 File Offset: 0x00143599
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x001453B7 File Offset: 0x001435B7
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x0014547F File Offset: 0x0014367F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003B4C RID: 15180 RVA: 0x00145497 File Offset: 0x00143697
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004B81 RID: 19329
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04004B82 RID: 19330
	public Transform followTarget;

	// Token: 0x04004B83 RID: 19331
	public Transform childGhost;

	// Token: 0x04004B84 RID: 19332
	public float velocityStep = 1f;

	// Token: 0x04004B85 RID: 19333
	public float currentSpeed;

	// Token: 0x04004B86 RID: 19334
	public float velocityIncreaseTime = 20f;

	// Token: 0x04004B87 RID: 19335
	public float riseDistance = 2f;

	// Token: 0x04004B88 RID: 19336
	public float summonDistance = 5f;

	// Token: 0x04004B89 RID: 19337
	public float timeEncircled;

	// Token: 0x04004B8A RID: 19338
	public float lastSummonCheck;

	// Token: 0x04004B8B RID: 19339
	public float timeGongStarted;

	// Token: 0x04004B8C RID: 19340
	public float summoningDuration = 30f;

	// Token: 0x04004B8D RID: 19341
	public float summoningCheckCountdown = 5f;

	// Token: 0x04004B8E RID: 19342
	public float gongDuration = 5f;

	// Token: 0x04004B8F RID: 19343
	public int summonCount = 5;

	// Token: 0x04004B90 RID: 19344
	public bool wasSurroundedLastCheck;

	// Token: 0x04004B91 RID: 19345
	public AudioSource laugh;

	// Token: 0x04004B92 RID: 19346
	public List<NetPlayer> possibleTarget;

	// Token: 0x04004B93 RID: 19347
	public AudioClip defaultLaugh;

	// Token: 0x04004B94 RID: 19348
	public AudioClip deepLaugh;

	// Token: 0x04004B95 RID: 19349
	public AudioClip gong;

	// Token: 0x04004B96 RID: 19350
	public Vector3 noisyOffset;

	// Token: 0x04004B97 RID: 19351
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x04004B98 RID: 19352
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x04004B99 RID: 19353
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x04004B9A RID: 19354
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x04004B9B RID: 19355
	public Vector3 leftHandStartingLocal;

	// Token: 0x04004B9C RID: 19356
	public Vector3 rightHandStartingLocal;

	// Token: 0x04004B9D RID: 19357
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04004B9E RID: 19358
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x04004B9F RID: 19359
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x04004BA0 RID: 19360
	public float maxTimeToNextHeadAngle;

	// Token: 0x04004BA1 RID: 19361
	public float lastHeadAngleTime;

	// Token: 0x04004BA2 RID: 19362
	public float nextHeadAngleTime;

	// Token: 0x04004BA3 RID: 19363
	public float nextTimeToChasePlayer;

	// Token: 0x04004BA4 RID: 19364
	public float maxNextTimeToChasePlayer;

	// Token: 0x04004BA5 RID: 19365
	public float timeRiseStarted;

	// Token: 0x04004BA6 RID: 19366
	public float totalTimeToRise;

	// Token: 0x04004BA7 RID: 19367
	public float catchDistance;

	// Token: 0x04004BA8 RID: 19368
	public float grabTime;

	// Token: 0x04004BA9 RID: 19369
	public float grabDuration;

	// Token: 0x04004BAA RID: 19370
	public float grabSpeed = 1f;

	// Token: 0x04004BAB RID: 19371
	public float minGrabCooldown;

	// Token: 0x04004BAC RID: 19372
	public float lastSpeedIncreased;

	// Token: 0x04004BAD RID: 19373
	public Vector3[] headEulerAngles;

	// Token: 0x04004BAE RID: 19374
	public Transform skullTransform;

	// Token: 0x04004BAF RID: 19375
	public Transform leftArm;

	// Token: 0x04004BB0 RID: 19376
	public Transform rightArm;

	// Token: 0x04004BB1 RID: 19377
	public Transform leftHand;

	// Token: 0x04004BB2 RID: 19378
	public Transform rightHand;

	// Token: 0x04004BB3 RID: 19379
	public Transform[] spawnTransforms;

	// Token: 0x04004BB4 RID: 19380
	public Transform[] spawnTransformOffsets;

	// Token: 0x04004BB5 RID: 19381
	public NetPlayer targetPlayer;

	// Token: 0x04004BB6 RID: 19382
	public GameObject ghostBody;

	// Token: 0x04004BB7 RID: 19383
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x04004BB8 RID: 19384
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x04004BB9 RID: 19385
	public int spawnIndex;

	// Token: 0x04004BBA RID: 19386
	public NetPlayer grabbedPlayer;

	// Token: 0x04004BBB RID: 19387
	public Material ghostMaterial;

	// Token: 0x04004BBC RID: 19388
	public Color defaultColor;

	// Token: 0x04004BBD RID: 19389
	public Color summonedColor;

	// Token: 0x04004BBE RID: 19390
	public bool isSummoned;

	// Token: 0x04004BBF RID: 19391
	private bool targetIsOnNavMesh;

	// Token: 0x04004BC0 RID: 19392
	private const float navMeshSampleRange = 5f;

	// Token: 0x04004BC1 RID: 19393
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04004BC2 RID: 19394
	public float hapticDuration = 1.5f;

	// Token: 0x04004BC3 RID: 19395
	private NavMeshPath path;

	// Token: 0x04004BC4 RID: 19396
	public List<Vector3> points;

	// Token: 0x04004BC5 RID: 19397
	public int currentTargetIdx;

	// Token: 0x04004BC6 RID: 19398
	private float nextPathTimestamp;

	// Token: 0x04004BC7 RID: 19399
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 5)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HalloweenGhostChaser.GhostData _Data;

	// Token: 0x020008D8 RID: 2264
	public enum ChaseState
	{
		// Token: 0x04004BC9 RID: 19401
		Dormant = 1,
		// Token: 0x04004BCA RID: 19402
		InitialRise,
		// Token: 0x04004BCB RID: 19403
		Gong = 4,
		// Token: 0x04004BCC RID: 19404
		Chasing = 8,
		// Token: 0x04004BCD RID: 19405
		Grabbing = 16
	}

	// Token: 0x020008D9 RID: 2265
	[NetworkStructWeaved(5)]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GhostData : INetworkStruct
	{
		// Token: 0x17000547 RID: 1351
		// (get) Token: 0x06003B4D RID: 15181 RVA: 0x001454AB File Offset: 0x001436AB
		// (set) Token: 0x06003B4E RID: 15182 RVA: 0x001454B9 File Offset: 0x001436B9
		[Networked]
		[NetworkedWeaved(3, 1)]
		public unsafe float CurrentSpeed
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed) = value;
			}
		}

		// Token: 0x04004BCE RID: 19406
		[FieldOffset(0)]
		public int TargetActorNumber;

		// Token: 0x04004BCF RID: 19407
		[FieldOffset(4)]
		public int CurrentState;

		// Token: 0x04004BD0 RID: 19408
		[FieldOffset(8)]
		public int SpawnIndex;

		// Token: 0x04004BD1 RID: 19409
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(12)]
		private FixedStorage@1 _CurrentSpeed;

		// Token: 0x04004BD2 RID: 19410
		[FieldOffset(16)]
		public NetworkBool IsSummoned;
	}
}
