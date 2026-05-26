using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F19 RID: 3865
	[NetworkBehaviourWeaved(6)]
	public class LurkerGhost : NetworkComponent
	{
		// Token: 0x06006071 RID: 24689 RVA: 0x001F0EB6 File Offset: 0x001EF0B6
		protected override void Awake()
		{
			base.Awake();
			this.possibleTargets = new List<NetPlayer>();
			this.targetPlayer = null;
			this.targetTransform = null;
			this.targetVRRig = null;
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x001F0EDE File Offset: 0x001EF0DE
		protected override void Start()
		{
			base.Start();
			this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
			this.PickNextWaypoint();
			this.ChangeState(LurkerGhost.ghostState.patrol);
		}

		// Token: 0x06006073 RID: 24691 RVA: 0x001F0F04 File Offset: 0x001EF104
		private void LateUpdate()
		{
			this.UpdateState();
			this.UpdateGhostVisibility();
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x001F0F14 File Offset: 0x001EF114
		private void PickNextWaypoint()
		{
			if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
			{
				ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, "");
				if (zoneBasedObject == null)
				{
					zoneBasedObject = this.lastWaypointRegion;
				}
				if (zoneBasedObject == null)
				{
					return;
				}
				this.lastWaypointRegion = zoneBasedObject;
				this.waypoints.Clear();
				foreach (object obj in zoneBasedObject.transform)
				{
					Transform item = (Transform)obj;
					this.waypoints.Add(item);
				}
			}
			int index = Random.Range(0, this.waypoints.Count);
			this.currentWaypoint = this.waypoints[index];
			this.targetRotation = Quaternion.LookRotation(this.currentWaypoint.position - base.transform.position);
			this.waypoints.RemoveAt(index);
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x001F1034 File Offset: 0x001EF234
		private void Patrol()
		{
			Transform transform = this.currentWaypoint;
			if (transform != null)
			{
				base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 360f * Time.deltaTime);
			}
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001F10AC File Offset: 0x001EF2AC
		private void PlaySound(AudioClip clip, bool loop)
		{
			if (this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.audioSource && clip != null)
			{
				this.audioSource.clip = clip;
				this.audioSource.loop = loop;
				this.audioSource.GTPlay();
			}
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x001F1118 File Offset: 0x001EF318
		private bool PickPlayer(float maxDistance)
		{
			if (base.IsMine)
			{
				this.possibleTargets.Clear();
				for (int i = 0; i < VRRigCache.ActiveRigContainers.Count; i++)
				{
					if ((VRRigCache.ActiveRigContainers[i].transform.position - base.transform.position).magnitude < maxDistance && VRRigCache.ActiveRigContainers[i].Creator != this.targetPlayer)
					{
						this.possibleTargets.Add(VRRigCache.ActiveRigContainers[i].Creator);
					}
				}
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
				if (this.possibleTargets.Count > 0)
				{
					int index = Random.Range(0, this.possibleTargets.Count);
					this.PickPlayer(this.possibleTargets[index]);
				}
			}
			else
			{
				this.targetPlayer = null;
				this.targetTransform = null;
				this.targetVRRig = null;
			}
			return this.targetPlayer != null && this.targetTransform != null;
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001F1228 File Offset: 0x001EF428
		private void PickPlayer(NetPlayer player)
		{
			int num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Creator != null && x.Creator == player);
			if (num > -1 && num < VRRigCache.ActiveRigContainers.Count)
			{
				VRRig rig = VRRigCache.ActiveRigContainers[num].Rig;
				this.targetPlayer = rig.creator;
				this.targetTransform = rig.head.rigTarget;
				this.targetVRRig = rig;
			}
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x001F12A0 File Offset: 0x001EF4A0
		private void SeekPlayer()
		{
			if (this.targetTransform.IsNull())
			{
				this.ChangeState(LurkerGhost.ghostState.patrol);
				return;
			}
			this.targetPosition = this.targetTransform.position + this.targetTransform.forward.x0z() * this.seekAheadDistance;
			this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.seekSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x001F1374 File Offset: 0x001EF574
		private void ChargeAtPlayer()
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.targetPosition, this.chargeSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, 720f * Time.deltaTime);
		}

		// Token: 0x0600607B RID: 24699 RVA: 0x001F13DC File Offset: 0x001EF5DC
		private void UpdateGhostVisibility()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.seek:
			case LurkerGhost.ghostState.charge:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer || this.passingPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.meshRenderer.sharedMaterial = this.visibleMaterial;
					this.bonesMeshRenderer.sharedMaterial = this.visibleMaterialBones;
					return;
				}
				this.meshRenderer.sharedMaterial = this.scryableMaterial;
				this.bonesMeshRenderer.sharedMaterial = this.scryableMaterialBones;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x001F1500 File Offset: 0x001EF700
		private void HauntObjects()
		{
			Collider[] array = new Collider[20];
			int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
			for (int i = 0; i < num; i++)
			{
				if (array[i].CompareTag("HauntedObject"))
				{
					UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
					if (triggerHauntedObjects != null)
					{
						triggerHauntedObjects(array[i].gameObject);
					}
				}
			}
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x001F1564 File Offset: 0x001EF764
		private void ChangeState(LurkerGhost.ghostState newState)
		{
			this.currentState = newState;
			VRRig vrrig = null;
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.PlaySound(this.patrolAudio, true);
				this.passingPlayer = null;
				this.cooldownTimeRemaining = Random.Range(this.cooldownDuration, this.maxCooldownDuration);
				this.currentRepeatHuntTimes = 0;
				break;
			case LurkerGhost.ghostState.charge:
				this.PlaySound(this.huntAudio, false);
				this.targetPosition = this.targetTransform.position;
				this.targetRotation = Quaternion.LookRotation(this.targetTransform.position - base.transform.position);
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.PlaySound(this.possessedAudio, true);
					GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
					GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
				}
				vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				break;
			}
			Shader.SetGlobalFloat(this._BlackAndWhite, (float)((newState == LurkerGhost.ghostState.possess && this.targetPlayer == NetworkSystem.Instance.LocalPlayer) ? 1 : 0));
			if (vrrig != this.lastHauntedVRRig && this.lastHauntedVRRig != null)
			{
				this.lastHauntedVRRig.IsHaunted = false;
			}
			if (vrrig != null)
			{
				vrrig.IsHaunted = true;
			}
			this.lastHauntedVRRig = vrrig;
			this.UpdateGhostVisibility();
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x001F16E2 File Offset: 0x001EF8E2
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x001F1700 File Offset: 0x001EF900
		private void UpdateState()
		{
			switch (this.currentState)
			{
			case LurkerGhost.ghostState.patrol:
				this.Patrol();
				if (base.IsMine)
				{
					if (this.currentWaypoint == null || Vector3.Distance(base.transform.position, this.currentWaypoint.position) < 0.2f)
					{
						this.PickNextWaypoint();
					}
					this.cooldownTimeRemaining -= Time.deltaTime;
					if (this.cooldownTimeRemaining <= 0f)
					{
						this.cooldownTimeRemaining = 0f;
						if (this.PickPlayer(this.maxHuntDistance))
						{
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
					}
				}
				break;
			case LurkerGhost.ghostState.seek:
				this.SeekPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < this.seekCloseEnoughDistance * this.seekCloseEnoughDistance)
				{
					this.ChangeState(LurkerGhost.ghostState.charge);
					return;
				}
				break;
			case LurkerGhost.ghostState.charge:
				this.ChargeAtPlayer();
				if (base.IsMine && (this.targetPosition - base.transform.position).sqrMagnitude < 0.25f)
				{
					if ((this.targetTransform.position - this.targetPosition).magnitude < this.minCatchDistance)
					{
						this.ChangeState(LurkerGhost.ghostState.possess);
						return;
					}
					this.huntedPassedTime = 0f;
					this.ChangeState(LurkerGhost.ghostState.patrol);
					return;
				}
				break;
			case LurkerGhost.ghostState.possess:
				if (this.targetTransform != null)
				{
					float num = this.SpookyMagicNumbers.x + MathF.Abs(MathF.Sin(Time.time * this.SpookyMagicNumbers.y));
					float num2 = this.HauntedMagicNumbers.x * MathF.Sin(Time.time * this.HauntedMagicNumbers.y) + this.HauntedMagicNumbers.z * MathF.Sin(Time.time * this.HauntedMagicNumbers.w);
					float y = 0.5f + 0.5f * MathF.Sin(Time.time * this.SpookyMagicNumbers.z);
					Vector3 target = this.targetTransform.position + new Vector3(num * (float)Math.Sin((double)num2), y, num * (float)Math.Cos((double)num2));
					base.transform.position = Vector3.MoveTowards(base.transform.position, target, this.chargeSpeed);
					base.transform.rotation = Quaternion.LookRotation(base.transform.position - this.targetTransform.position);
				}
				if (base.IsMine)
				{
					this.huntedPassedTime += Time.deltaTime;
					if (this.huntedPassedTime >= this.PossessionDuration)
					{
						this.huntedPassedTime = 0f;
						if (this.hauntNeighbors && this.currentRepeatHuntTimes < this.maxRepeatHuntTimes && this.PickPlayer(this.maxRepeatHuntDistance))
						{
							this.currentRepeatHuntTimes++;
							this.ChangeState(LurkerGhost.ghostState.seek);
							return;
						}
						this.ChangeState(LurkerGhost.ghostState.patrol);
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06006080 RID: 24704 RVA: 0x001F1A0D File Offset: 0x001EFC0D
		// (set) Token: 0x06006081 RID: 24705 RVA: 0x001F1A37 File Offset: 0x001EFC37
		[Networked]
		[NetworkedWeaved(0, 6)]
		private unsafe LurkerGhost.LurkerGhostData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(LurkerGhost.LurkerGhostData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing LurkerGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(LurkerGhost.LurkerGhostData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06006082 RID: 24706 RVA: 0x001F1A62 File Offset: 0x001EFC62
		public override void WriteDataFusion()
		{
			this.Data = new LurkerGhost.LurkerGhostData(this.currentState, this.currentIndex, this.targetPlayer.ActorNumber, this.targetPosition);
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x001F1A8C File Offset: 0x001EFC8C
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentIndex, this.Data.TargetActor, this.Data.TargetPos);
		}

		// Token: 0x06006084 RID: 24708 RVA: 0x001F1AD8 File Offset: 0x001EFCD8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.currentState);
			stream.SendNext(this.currentIndex);
			if (this.targetPlayer != null)
			{
				stream.SendNext(this.targetPlayer.ActorNumber);
			}
			else
			{
				stream.SendNext(-1);
			}
			stream.SendNext(this.targetPosition);
		}

		// Token: 0x06006085 RID: 24709 RVA: 0x001F1B54 File Offset: 0x001EFD54
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			LurkerGhost.ghostState state = (LurkerGhost.ghostState)stream.ReceiveNext();
			int index = (int)stream.ReceiveNext();
			int targetActorNumber = (int)stream.ReceiveNext();
			Vector3 targetPos = (Vector3)stream.ReceiveNext();
			this.ReadDataShared(state, index, targetActorNumber, targetPos);
		}

		// Token: 0x06006086 RID: 24710 RVA: 0x001F1BAC File Offset: 0x001EFDAC
		private void ReadDataShared(LurkerGhost.ghostState state, int index, int targetActorNumber, Vector3 targetPos)
		{
			LurkerGhost.ghostState ghostState = this.currentState;
			this.currentState = state;
			this.currentIndex = index;
			NetPlayer netPlayer = this.targetPlayer;
			this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
			this.targetPosition = targetPos;
			float num = 10000f;
			if (!this.targetPosition.IsValid(num))
			{
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(this.targetPlayer, out rigContainer))
				{
					this.targetPosition = (this.targetPlayer.IsLocal ? rigContainer.Rig.transform.position : rigContainer.Rig.syncPos);
				}
				else
				{
					this.targetPosition = base.transform.position;
				}
			}
			if (this.targetPlayer != netPlayer)
			{
				this.PickPlayer(this.targetPlayer);
			}
			if (ghostState != this.currentState || this.targetPlayer != netPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06006087 RID: 24711 RVA: 0x001F1C8D File Offset: 0x001EFE8D
		public override void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			base.OnOwnerChange(newOwner, previousOwner);
			if (newOwner == PhotonNetwork.LocalPlayer)
			{
				this.ChangeState(this.currentState);
			}
		}

		// Token: 0x06006089 RID: 24713 RVA: 0x001F1DB0 File Offset: 0x001EFFB0
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600608A RID: 24714 RVA: 0x001F1DC8 File Offset: 0x001EFFC8
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04006EF1 RID: 28401
		public float patrolSpeed = 3f;

		// Token: 0x04006EF2 RID: 28402
		public float seekSpeed = 6f;

		// Token: 0x04006EF3 RID: 28403
		public float chargeSpeed = 6f;

		// Token: 0x04006EF4 RID: 28404
		[Tooltip("Cooldown until the next time the ghost needs to hunt a new player")]
		public float cooldownDuration = 10f;

		// Token: 0x04006EF5 RID: 28405
		[Tooltip("Max Cooldown (randomized)")]
		public float maxCooldownDuration = 10f;

		// Token: 0x04006EF6 RID: 28406
		[Tooltip("How long the possession effects should last")]
		public float PossessionDuration = 15f;

		// Token: 0x04006EF7 RID: 28407
		[Tooltip("Hunted objects within this radius will get triggered ")]
		public float sphereColliderRadius = 2f;

		// Token: 0x04006EF8 RID: 28408
		[Tooltip("Maximum distance to the possible player to get hunted")]
		public float maxHuntDistance = 20f;

		// Token: 0x04006EF9 RID: 28409
		[Tooltip("Minimum distance from the player to start the possession effects")]
		public float minCatchDistance = 2f;

		// Token: 0x04006EFA RID: 28410
		[Tooltip("Maximum distance to the possible player to get repeat hunted")]
		public float maxRepeatHuntDistance = 5f;

		// Token: 0x04006EFB RID: 28411
		[Tooltip("Maximum times the lurker can haunt a nearby player before going back on cooldown")]
		public int maxRepeatHuntTimes = 3;

		// Token: 0x04006EFC RID: 28412
		[Tooltip("Time in seconds before a haunted player can pass the lurker to another player by tagging")]
		public float tagCoolDown = 2f;

		// Token: 0x04006EFD RID: 28413
		[Tooltip("UP & DOWN, IN & OUT")]
		public Vector3 SpookyMagicNumbers = new Vector3(1f, 1f, 1f);

		// Token: 0x04006EFE RID: 28414
		[Tooltip("SPIN, SPIN, SPIN, SPIN")]
		public Vector4 HauntedMagicNumbers = new Vector4(1f, 2f, 3f, 1f);

		// Token: 0x04006EFF RID: 28415
		[Tooltip("Haptic vibration when haunted by the ghost")]
		public float hapticStrength = 1f;

		// Token: 0x04006F00 RID: 28416
		public float hapticDuration = 1.5f;

		// Token: 0x04006F01 RID: 28417
		public GameObject waypointsContainer;

		// Token: 0x04006F02 RID: 28418
		private ZoneBasedObject[] waypointRegions;

		// Token: 0x04006F03 RID: 28419
		private ZoneBasedObject lastWaypointRegion;

		// Token: 0x04006F04 RID: 28420
		private List<Transform> waypoints = new List<Transform>();

		// Token: 0x04006F05 RID: 28421
		private Transform currentWaypoint;

		// Token: 0x04006F06 RID: 28422
		public Material visibleMaterial;

		// Token: 0x04006F07 RID: 28423
		public Material scryableMaterial;

		// Token: 0x04006F08 RID: 28424
		public Material visibleMaterialBones;

		// Token: 0x04006F09 RID: 28425
		public Material scryableMaterialBones;

		// Token: 0x04006F0A RID: 28426
		public MeshRenderer meshRenderer;

		// Token: 0x04006F0B RID: 28427
		public MeshRenderer bonesMeshRenderer;

		// Token: 0x04006F0C RID: 28428
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006F0D RID: 28429
		public AudioClip patrolAudio;

		// Token: 0x04006F0E RID: 28430
		public AudioClip huntAudio;

		// Token: 0x04006F0F RID: 28431
		public AudioClip possessedAudio;

		// Token: 0x04006F10 RID: 28432
		public ThrowableSetDressing scryingGlass;

		// Token: 0x04006F11 RID: 28433
		public float scryingAngerAngle;

		// Token: 0x04006F12 RID: 28434
		public float scryingAngerDelay;

		// Token: 0x04006F13 RID: 28435
		public float seekAheadDistance;

		// Token: 0x04006F14 RID: 28436
		public float seekCloseEnoughDistance;

		// Token: 0x04006F15 RID: 28437
		private float scryingAngerAfterTimestamp;

		// Token: 0x04006F16 RID: 28438
		private int currentRepeatHuntTimes;

		// Token: 0x04006F17 RID: 28439
		public UnityAction<GameObject> TriggerHauntedObjects;

		// Token: 0x04006F18 RID: 28440
		private int currentIndex;

		// Token: 0x04006F19 RID: 28441
		private LurkerGhost.ghostState currentState;

		// Token: 0x04006F1A RID: 28442
		private float cooldownTimeRemaining;

		// Token: 0x04006F1B RID: 28443
		private List<NetPlayer> possibleTargets;

		// Token: 0x04006F1C RID: 28444
		private NetPlayer targetPlayer;

		// Token: 0x04006F1D RID: 28445
		private Transform targetTransform;

		// Token: 0x04006F1E RID: 28446
		private float huntedPassedTime;

		// Token: 0x04006F1F RID: 28447
		private Vector3 targetPosition;

		// Token: 0x04006F20 RID: 28448
		private Quaternion targetRotation;

		// Token: 0x04006F21 RID: 28449
		private VRRig targetVRRig;

		// Token: 0x04006F22 RID: 28450
		private ShaderHashId _BlackAndWhite = "_BlackAndWhite";

		// Token: 0x04006F23 RID: 28451
		private VRRig lastHauntedVRRig;

		// Token: 0x04006F24 RID: 28452
		private float nextTagTime;

		// Token: 0x04006F25 RID: 28453
		private NetPlayer passingPlayer;

		// Token: 0x04006F26 RID: 28454
		[SerializeField]
		private bool hauntNeighbors = true;

		// Token: 0x04006F27 RID: 28455
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 6)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private LurkerGhost.LurkerGhostData _Data;

		// Token: 0x02000F1A RID: 3866
		private enum ghostState
		{
			// Token: 0x04006F29 RID: 28457
			patrol,
			// Token: 0x04006F2A RID: 28458
			seek,
			// Token: 0x04006F2B RID: 28459
			charge,
			// Token: 0x04006F2C RID: 28460
			possess
		}

		// Token: 0x02000F1B RID: 3867
		[NetworkStructWeaved(6)]
		[StructLayout(LayoutKind.Explicit, Size = 24)]
		private struct LurkerGhostData : INetworkStruct
		{
			// Token: 0x17000924 RID: 2340
			// (get) Token: 0x0600608B RID: 24715 RVA: 0x001F1DDC File Offset: 0x001EFFDC
			// (set) Token: 0x0600608C RID: 24716 RVA: 0x001F1DE4 File Offset: 0x001EFFE4
			public LurkerGhost.ghostState CurrentState { readonly get; set; }

			// Token: 0x17000925 RID: 2341
			// (get) Token: 0x0600608D RID: 24717 RVA: 0x001F1DED File Offset: 0x001EFFED
			// (set) Token: 0x0600608E RID: 24718 RVA: 0x001F1DF5 File Offset: 0x001EFFF5
			public int CurrentIndex { readonly get; set; }

			// Token: 0x17000926 RID: 2342
			// (get) Token: 0x0600608F RID: 24719 RVA: 0x001F1DFE File Offset: 0x001EFFFE
			// (set) Token: 0x06006090 RID: 24720 RVA: 0x001F1E06 File Offset: 0x001F0006
			public int TargetActor { readonly get; set; }

			// Token: 0x17000927 RID: 2343
			// (get) Token: 0x06006091 RID: 24721 RVA: 0x001F1E0F File Offset: 0x001F000F
			// (set) Token: 0x06006092 RID: 24722 RVA: 0x001F1E21 File Offset: 0x001F0021
			[Networked]
			[NetworkedWeaved(3, 3)]
			public unsafe Vector3 TargetPos
			{
				readonly get
				{
					return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos);
				}
				set
				{
					*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._TargetPos) = value;
				}
			}

			// Token: 0x06006093 RID: 24723 RVA: 0x001F1E34 File Offset: 0x001F0034
			public LurkerGhostData(LurkerGhost.ghostState state, int index, int actor, Vector3 pos)
			{
				this.CurrentState = state;
				this.CurrentIndex = index;
				this.TargetActor = actor;
				this.TargetPos = pos;
			}

			// Token: 0x04006F30 RID: 28464
			[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(12)]
			private FixedStorage@3 _TargetPos;
		}
	}
}
