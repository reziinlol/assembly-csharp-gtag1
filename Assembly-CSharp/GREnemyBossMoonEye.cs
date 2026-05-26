using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200077F RID: 1919
public class GREnemyBossMoonEye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x060030B5 RID: 12469 RVA: 0x00109180 File Offset: 0x00107380
	private void Awake()
	{
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		if (this.navAgent != null)
		{
			this.navAgent.updateRotation = false;
		}
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[8];
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x001091FC File Offset: 0x001073FC
	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoonEye.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.AttackLaser, this.abilityAttackLaser, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Closed, this.abilityClosed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityStart, this.abilityGravityStart, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityEnd, this.abilityGravityEnd, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityIdle, this.abilityGravityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		int maxHP = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(maxHP);
		}
		this.SetHP(maxHP);
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0010944C File Offset: 0x0010764C
	private void SetupAbility(GREnemyBossMoonEye.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x060030B8 RID: 12472 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060030B9 RID: 12473 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x00109468 File Offset: 0x00107668
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x00109481 File Offset: 0x00107681
	public void Setup(long entityCreateData)
	{
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x0010948B File Offset: 0x0010768B
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoonEye.Behavior)newState, false);
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x0010949E File Offset: 0x0010769E
	public void ResetEye()
	{
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, false);
		}
	}

	// Token: 0x060030BE RID: 12478 RVA: 0x001094B5 File Offset: 0x001076B5
	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x001094D8 File Offset: 0x001076D8
	public bool TrySetBehavior(GREnemyBossMoonEye.Behavior newBehavior)
	{
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x001094E4 File Offset: 0x001076E4
	private void SetBehavior(GREnemyBossMoonEye.Behavior newBehavior, bool force = false)
	{
		if (this.abilities == null)
		{
			Debug.LogError("Abilities have not been initialized", this);
			return;
		}
		if (newBehavior < GREnemyBossMoonEye.Behavior.Idle || newBehavior >= (GREnemyBossMoonEye.Behavior)this.abilities.Length)
		{
			Debug.LogErrorFormat("New Behavior Index is invalid {0} {1} {2}", new object[]
			{
				(int)newBehavior,
				newBehavior,
				base.gameObject.name
			});
			return;
		}
		GRAbilityBase grabilityBase = this.abilities[(int)newBehavior];
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		Debug.LogFormat("Boss Eye SetBehavior {0} -> {1}", new object[]
		{
			this.currBehavior,
			newBehavior
		});
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			this.SetHP(this.CalcMaxHP());
		}
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.AttackLaser)
		{
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x00109614 File Offset: 0x00107814
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	// Token: 0x060030C2 RID: 12482 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void RefreshBody()
	{
	}

	// Token: 0x060030C3 RID: 12483 RVA: 0x0010964D File Offset: 0x0010784D
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060030C4 RID: 12484 RVA: 0x0010965C File Offset: 0x0010785C
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyBossMoonEye.tempRigs.Clear();
		GREnemyBossMoonEye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoonEye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoonEye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Idle)
		{
			this.ChooseNewBehavior();
		}
	}

	// Token: 0x060030C5 RID: 12485 RVA: 0x00109704 File Offset: 0x00107904
	private bool TryChooseAttackBehavior()
	{
		if (Time.timeAsDouble > this.lastHitTime + (double)this.counterAttackWindow)
		{
			return false;
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			return false;
		}
		GREnemyBossMoonEye.tempPotentialAttacks.Clear();
		if (this.allowLaserAttack)
		{
			GREnemyBossMoonEye.tempPotentialAttacks.Add(GREnemyBossMoonEye.Behavior.AttackLaser);
		}
		for (int i = GREnemyBossMoonEye.tempPotentialAttacks.Count - 1; i >= 0; i--)
		{
			GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoonEye.tempPotentialAttacks[i]];
			if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
			{
				GREnemyBossMoonEye.tempPotentialAttacks.RemoveAt(i);
			}
		}
		if (GREnemyBossMoonEye.tempPotentialAttacks.Count <= 0)
		{
			return false;
		}
		int index = Random.Range(0, GREnemyBossMoonEye.tempPotentialAttacks.Count);
		this.SetBehavior(GREnemyBossMoonEye.tempPotentialAttacks[index], false);
		return true;
	}

	// Token: 0x060030C6 RID: 12486 RVA: 0x001097D7 File Offset: 0x001079D7
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.TryChooseAttackBehavior())
		{
			return;
		}
		this.TrySetBehavior(GREnemyBossMoonEye.Behavior.Idle);
	}

	// Token: 0x060030C7 RID: 12487 RVA: 0x001097F1 File Offset: 0x001079F1
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060030C8 RID: 12488 RVA: 0x0010980F File Offset: 0x00107A0F
	private void OnUpdateAuthority(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			if (this.currAbility.IsDone())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.None, false);
				this.ChooseNewBehavior();
			}
		}
	}

	// Token: 0x060030C9 RID: 12489 RVA: 0x00109840 File Offset: 0x00107A40
	private void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	// Token: 0x060030CA RID: 12490 RVA: 0x00109856 File Offset: 0x00107A56
	public void InstantKill()
	{
		if (this.hp <= 0)
		{
			return;
		}
		this.SetHP(0);
		this.lastHitTime = Time.timeAsDouble;
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
		}
	}

	// Token: 0x060030CB RID: 12491 RVA: 0x0010988C File Offset: 0x00107A8C
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Dying)
		{
			return;
		}
		this.SetHP(this.hp - hit.hitAmount);
		this.lastHitTime = Time.timeAsDouble;
		if (this.hp <= 0)
		{
			this.hp = 0;
			if (this.entity.IsAuthority())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
				return;
			}
		}
		else
		{
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
		}
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x00109903 File Offset: 0x00107B03
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x00109910 File Offset: 0x00107B10
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyBossMoonEye.Behavior.AttackLaser)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position,
					hittablePoint = component5.FindHittablePoint(collider)
				};
				component5.RequestHit(hitData);
			}
		}
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x00109A74 File Offset: 0x00107C74
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 hitImpulse = player.transform.position - base.transform.position;
			hitImpulse.y = 0f;
			hitImpulse = hitImpulse.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, hitImpulse);
		}
		yield break;
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x00109A8A File Offset: 0x00107C8A
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x00109AC0 File Offset: 0x00107CC0
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		int value2 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(this.hp);
		writer.Write(value2);
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x00109B08 File Offset: 0x00107D08
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoonEye.Behavior newBehavior = (GREnemyBossMoonEye.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		int playerID = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x00109B4C File Offset: 0x00107D4C
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			if (hitTypeId == GameHitType.Club)
			{
				this.OnHitByClub(gameComponent, hit);
				return;
			}
			if (hitTypeId != GameHitType.Shield)
			{
				return;
			}
			this.OnHitByShield(gameComponent, hit);
		}
	}

	// Token: 0x04003E93 RID: 16019
	public GameEntity entity;

	// Token: 0x04003E94 RID: 16020
	public GameAgent agent;

	// Token: 0x04003E95 RID: 16021
	public GREnemy enemy;

	// Token: 0x04003E96 RID: 16022
	public GRArmorEnemy armor;

	// Token: 0x04003E97 RID: 16023
	public GameHittable hittable;

	// Token: 0x04003E98 RID: 16024
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003E99 RID: 16025
	public GRSenseNearby senseNearby;

	// Token: 0x04003E9A RID: 16026
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003E9B RID: 16027
	public Animation anim;

	// Token: 0x04003E9C RID: 16028
	private GRAbilityBase[] abilities;

	// Token: 0x04003E9D RID: 16029
	private GRAbilityBase currAbility;

	// Token: 0x04003E9E RID: 16030
	public GRAbilityAgent abilityAgent;

	// Token: 0x04003E9F RID: 16031
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003EA0 RID: 16032
	public GRAbilityIdle abilityClosed;

	// Token: 0x04003EA1 RID: 16033
	public GRAbilityAttackLaser abilityAttackLaser;

	// Token: 0x04003EA2 RID: 16034
	public GRAbilityDie abilityDie;

	// Token: 0x04003EA3 RID: 16035
	public GRAbilityIdle abilityGravityStart;

	// Token: 0x04003EA4 RID: 16036
	public GRAbilityIdle abilityGravityEnd;

	// Token: 0x04003EA5 RID: 16037
	public GRAbilityIdle abilityGravityIdle;

	// Token: 0x04003EA6 RID: 16038
	public Transform headTransform;

	// Token: 0x04003EA7 RID: 16039
	public NavMeshAgent navAgent;

	// Token: 0x04003EA8 RID: 16040
	public AudioSource audioSource;

	// Token: 0x04003EA9 RID: 16041
	public float counterAttackWindow = 3f;

	// Token: 0x04003EAA RID: 16042
	private Transform target;

	// Token: 0x04003EAB RID: 16043
	[ReadOnly]
	public int hp;

	// Token: 0x04003EAC RID: 16044
	[ReadOnly]
	public GREnemyBossMoonEye.Behavior currBehavior;

	// Token: 0x04003EAD RID: 16045
	[ReadOnly]
	public GREnemyBossMoonEye.BodyState currBodyState;

	// Token: 0x04003EAE RID: 16046
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003EAF RID: 16047
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003EB0 RID: 16048
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003EB1 RID: 16049
	public bool allowLaserAttack;

	// Token: 0x04003EB2 RID: 16050
	public bool canChaseJump = true;

	// Token: 0x04003EB3 RID: 16051
	public float chaseJumpDistance = 5f;

	// Token: 0x04003EB4 RID: 16052
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003EB5 RID: 16053
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003EB6 RID: 16054
	private double lastHitTime;

	// Token: 0x04003EB7 RID: 16055
	private List<Collider> colliders;

	// Token: 0x04003EB8 RID: 16056
	private float lastHitPlayerTime;

	// Token: 0x04003EB9 RID: 16057
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003EBA RID: 16058
	public float hearingRadius = 5f;

	// Token: 0x04003EBB RID: 16059
	public int maxSimultaneousSummonedEntities = 6;

	// Token: 0x04003EBC RID: 16060
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003EBD RID: 16061
	private static List<GREnemyBossMoonEye.Behavior> tempPotentialAttacks = new List<GREnemyBossMoonEye.Behavior>(16);

	// Token: 0x04003EBE RID: 16062
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x02000780 RID: 1920
	public enum Behavior
	{
		// Token: 0x04003EC0 RID: 16064
		Idle,
		// Token: 0x04003EC1 RID: 16065
		AttackLaser,
		// Token: 0x04003EC2 RID: 16066
		Closed,
		// Token: 0x04003EC3 RID: 16067
		GravityStart,
		// Token: 0x04003EC4 RID: 16068
		GravityEnd,
		// Token: 0x04003EC5 RID: 16069
		GravityIdle,
		// Token: 0x04003EC6 RID: 16070
		Dying,
		// Token: 0x04003EC7 RID: 16071
		None,
		// Token: 0x04003EC8 RID: 16072
		Count
	}

	// Token: 0x02000781 RID: 1921
	public enum BodyState
	{
		// Token: 0x04003ECA RID: 16074
		Destroyed,
		// Token: 0x04003ECB RID: 16075
		Bones,
		// Token: 0x04003ECC RID: 16076
		Shell,
		// Token: 0x04003ECD RID: 16077
		Count
	}
}
