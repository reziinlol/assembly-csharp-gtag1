using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000790 RID: 1936
public class GREnemyPhantom : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x0600315E RID: 12638 RVA: 0x0010DDCC File Offset: 0x0010BFCC
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
	}

	// Token: 0x0600315F RID: 12639 RVA: 0x0010DE78 File Offset: 0x0010C078
	public void OnEntityInit()
	{
		this.abilityMine.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityRage.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAlert.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityReturn.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		int num = (int)this.entity.createData;
		this.Setup((long)num);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x06003160 RID: 12640 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003161 RID: 12641 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x0010E140 File Offset: 0x0010C340
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x0010E170 File Offset: 0x0010C370
	private void Setup(long createData)
	{
		this.SetPatrolPath(createData);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[0];
			this.idleLocation = this.target;
			this.SetBehavior(GREnemyPhantom.Behavior.Return, true);
		}
		else
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Mine, true);
		}
		this.SetBodyState(GREnemyPhantom.BodyState.Bones, true);
		if (this.attackLight != null)
		{
			this.attackLight.gameObject.SetActive(false);
		}
		if (this.negativeLight != null)
		{
			this.negativeLight.gameObject.SetActive(false);
		}
		GREnemy.HideRenderers(this.bones, false);
		GREnemy.HideRenderers(this.always, false);
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x0010E23F File Offset: 0x0010C43F
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPhantom.Behavior.Jump, false);
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x0010E259 File Offset: 0x0010C459
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 9)
		{
			return;
		}
		this.SetBehavior((GREnemyPhantom.Behavior)newState, false);
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x0010E26D File Offset: 0x0010C46D
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 2)
		{
			return;
		}
		this.SetBodyState((GREnemyPhantom.BodyState)newState, false);
	}

	// Token: 0x06003167 RID: 12647 RVA: 0x0010E280 File Offset: 0x0010C480
	public void SetPatrolPath(long createData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(createData);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x0010E2AB File Offset: 0x0010C4AB
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x0010E2B4 File Offset: 0x0010C4B4
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x0010E2C0 File Offset: 0x0010C4C0
	public void SetBehavior(GREnemyPhantom.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Stop();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Stop();
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Stop();
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Stop();
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Stop();
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Stop();
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Start();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Start();
			this.soundAlert.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Start();
			this.soundReturn.Play(this.audioSource);
			this.abilityReturn.SetTarget(this.idleLocation);
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Start();
			this.soundRage.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Start();
			this.soundChase.Play(this.audioSource);
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = null;
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = null;
			this.soundAttack.Play(this.audioSource);
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x0010E5A8 File Offset: 0x0010C7A8
	public void SetBodyState(GREnemyPhantom.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x0010E624 File Offset: 0x0010C824
	private void RefreshBody()
	{
		GREnemyPhantom.BodyState bodyState = this.currBodyState;
		if (bodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			this.armor.SetHp(0);
			return;
		}
		if (bodyState != GREnemyPhantom.BodyState.Bones)
		{
			return;
		}
		this.armor.SetHp(0);
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x0010E659 File Offset: 0x0010C859
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x0010E668 File Offset: 0x0010C868
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = null;
			this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPhantom.Behavior.Investigate, false);
			return;
		}
		if (this.currBehavior == GREnemyPhantom.Behavior.Investigate)
		{
			if (this.idleLocation != null)
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			this.SetBehavior(GREnemyPhantom.Behavior.Idle, false);
		}
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x0010E714 File Offset: 0x0010C914
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPhantom.tempRigs.Clear();
		GREnemyPhantom.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPhantom.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPhantom.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Alert:
		case GREnemyPhantom.Behavior.Rage:
		case GREnemyPhantom.Behavior.Attack:
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.SetTarget(this.idleLocation);
			this.abilityReturn.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			break;
		default:
			return;
		}
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x0010E83A File Offset: 0x0010CA3A
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x0010E858 File Offset: 0x0010CA58
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.UpdateAuthority(dt);
			if (this.idleLocation != null)
			{
				GameAgent.UpdateFacingDir(base.transform, this.agent.navAgent, this.idleLocation.forward, 180f);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyPhantom.Behavior.Alert:
			this.UpdateAlert(dt);
			return;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.UpdateAuthority(dt);
			if (this.abilityReturn.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Mine, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.UpdateAuthority(dt);
			if (this.abilityRage.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPhantom.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.UpdateAuthority(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003172 RID: 12658 RVA: 0x0010EA08 File Offset: 0x0010CC08
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Rage:
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x0010EA80 File Offset: 0x0010CC80
	public void UpdateAlert(float dt)
	{
		this.abilityAlert.SetTargetPlayer(this.agent.targetPlayer);
		this.abilityAlert.UpdateAuthority(dt);
		double timeAsDouble = Time.timeAsDouble;
		if (!this.senseNearby.IsAnyoneNearby())
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
			return;
		}
		float num;
		if (this.abilityAlert.IsDone() && this.senseNearby.PickClosest(out num) != null)
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Rage, false);
		}
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x0010EAF8 File Offset: 0x0010CCF8
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyPhantom.Behavior.Attack)
		{
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position,
					hittablePoint = component3.FindHittablePoint(collider)
				};
				component3.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x0010EC11 File Offset: 0x0010CE11
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x0010EC48 File Offset: 0x0010CE48
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.nextPatrolNode;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x0010EC90 File Offset: 0x0010CE90
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPhantom.Behavior newBehavior = (GREnemyPhantom.Behavior)reader.ReadByte();
		GREnemyPhantom.BodyState newBodyState = (GREnemyPhantom.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x04003FB7 RID: 16311
	public GameEntity entity;

	// Token: 0x04003FB8 RID: 16312
	public GameAgent agent;

	// Token: 0x04003FB9 RID: 16313
	public GRArmorEnemy armor;

	// Token: 0x04003FBA RID: 16314
	public GRAttributes attributes;

	// Token: 0x04003FBB RID: 16315
	public Animation anim;

	// Token: 0x04003FBC RID: 16316
	public GRSenseNearby senseNearby;

	// Token: 0x04003FBD RID: 16317
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003FBE RID: 16318
	public GRAbilityIdle abilityMine;

	// Token: 0x04003FBF RID: 16319
	public AbilitySound soundMine;

	// Token: 0x04003FC0 RID: 16320
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003FC1 RID: 16321
	public GRAbilityWatch abilityRage;

	// Token: 0x04003FC2 RID: 16322
	public AbilitySound soundRage;

	// Token: 0x04003FC3 RID: 16323
	public GRAbilityWatch abilityAlert;

	// Token: 0x04003FC4 RID: 16324
	public AbilitySound soundAlert;

	// Token: 0x04003FC5 RID: 16325
	public GRAbilityChase abilityChase;

	// Token: 0x04003FC6 RID: 16326
	public AbilitySound soundChase;

	// Token: 0x04003FC7 RID: 16327
	public GRAbilityMoveToTarget abilityReturn;

	// Token: 0x04003FC8 RID: 16328
	public AbilitySound soundReturn;

	// Token: 0x04003FC9 RID: 16329
	public GRAbilityAttackLatchOn abilityAttack;

	// Token: 0x04003FCA RID: 16330
	public AbilitySound soundAttack;

	// Token: 0x04003FCB RID: 16331
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04003FCC RID: 16332
	public GRAbilityJump abilityJump;

	// Token: 0x04003FCD RID: 16333
	public List<Renderer> bones;

	// Token: 0x04003FCE RID: 16334
	public List<Renderer> always;

	// Token: 0x04003FCF RID: 16335
	public Transform coreMarker;

	// Token: 0x04003FD0 RID: 16336
	public GRCollectible corePrefab;

	// Token: 0x04003FD1 RID: 16337
	public Transform headTransform;

	// Token: 0x04003FD2 RID: 16338
	public float attackRange = 2f;

	// Token: 0x04003FD3 RID: 16339
	public float hearingRadius = 7f;

	// Token: 0x04003FD4 RID: 16340
	public List<VRRig> rigsNearby;

	// Token: 0x04003FD5 RID: 16341
	public GameLight attackLight;

	// Token: 0x04003FD6 RID: 16342
	public GameLight negativeLight;

	// Token: 0x04003FD7 RID: 16343
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003FD8 RID: 16344
	private Transform idleLocation;

	// Token: 0x04003FD9 RID: 16345
	public NavMeshAgent navAgent;

	// Token: 0x04003FDA RID: 16346
	public AudioSource audioSource;

	// Token: 0x04003FDB RID: 16347
	public double lastStateChange;

	// Token: 0x04003FDC RID: 16348
	private Vector3? investigateLocation;

	// Token: 0x04003FDD RID: 16349
	private Transform target;

	// Token: 0x04003FDE RID: 16350
	[ReadOnly]
	public int hp;

	// Token: 0x04003FDF RID: 16351
	[ReadOnly]
	public GREnemyPhantom.Behavior currBehavior;

	// Token: 0x04003FE0 RID: 16352
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003FE1 RID: 16353
	[ReadOnly]
	public GREnemyPhantom.BodyState currBodyState;

	// Token: 0x04003FE2 RID: 16354
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003FE3 RID: 16355
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003FE4 RID: 16356
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04003FE5 RID: 16357
	private Rigidbody rigidBody;

	// Token: 0x04003FE6 RID: 16358
	private List<Collider> colliders;

	// Token: 0x04003FE7 RID: 16359
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x02000791 RID: 1937
	public enum Behavior
	{
		// Token: 0x04003FE9 RID: 16361
		Mine,
		// Token: 0x04003FEA RID: 16362
		Idle,
		// Token: 0x04003FEB RID: 16363
		Alert,
		// Token: 0x04003FEC RID: 16364
		Return,
		// Token: 0x04003FED RID: 16365
		Rage,
		// Token: 0x04003FEE RID: 16366
		Chase,
		// Token: 0x04003FEF RID: 16367
		Attack,
		// Token: 0x04003FF0 RID: 16368
		Investigate,
		// Token: 0x04003FF1 RID: 16369
		Jump,
		// Token: 0x04003FF2 RID: 16370
		Count
	}

	// Token: 0x02000792 RID: 1938
	public enum BodyState
	{
		// Token: 0x04003FF4 RID: 16372
		Destroyed,
		// Token: 0x04003FF5 RID: 16373
		Bones,
		// Token: 0x04003FF6 RID: 16374
		Count
	}
}
