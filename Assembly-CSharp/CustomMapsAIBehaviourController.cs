using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A50 RID: 2640
public class CustomMapsAIBehaviourController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000653 RID: 1619
	// (get) Token: 0x06004398 RID: 17304 RVA: 0x0016ACBA File Offset: 0x00168EBA
	// (set) Token: 0x06004397 RID: 17303 RVA: 0x0016ACB1 File Offset: 0x00168EB1
	public GRPlayer TargetPlayer { get; private set; }

	// Token: 0x06004399 RID: 17305 RVA: 0x0016ACC4 File Offset: 0x00168EC4
	private void Awake()
	{
		this.TargetPlayer = null;
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default",
			"Gorilla Object"
		});
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x0016AD15 File Offset: 0x00168F15
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x0600439B RID: 17307 RVA: 0x0016AD2E File Offset: 0x00168F2E
	public void SetTarget(GRPlayer newTarget)
	{
		if (newTarget.IsNull())
		{
			this.ClearTarget();
			return;
		}
		this.TargetPlayer = newTarget;
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x0016AD46 File Offset: 0x00168F46
	public void ClearTarget()
	{
		this.TargetPlayer = null;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x0016AD4F File Offset: 0x00168F4F
	private void Update()
	{
		this.OnThink();
		this.UpdateAnimators();
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x0016AD60 File Offset: 0x00168F60
	private void OnTriggerEnter(Collider collider)
	{
		CustomMapsBehaviourBase customMapsBehaviourBase = this.behaviourDict[this.currentBehaviour];
		if (customMapsBehaviourBase != null)
		{
			customMapsBehaviourBase.OnTriggerEnter(collider);
		}
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x0016AD89 File Offset: 0x00168F89
	private void InitAnimators()
	{
		this.animators = base.gameObject.GetComponentsInChildren<Animator>();
	}

	// Token: 0x060043A0 RID: 17312 RVA: 0x0016AD9C File Offset: 0x00168F9C
	private void UpdateAnimators()
	{
		if (this.animators.IsNullOrEmpty<Animator>())
		{
			return;
		}
		float magnitude = this.agent.navAgent.velocity.magnitude;
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].SetFloat(CustomMapsAIBehaviourController.movementSpeedParamIndex, magnitude);
		}
	}

	// Token: 0x060043A1 RID: 17313 RVA: 0x0016ADF8 File Offset: 0x00168FF8
	public void PlayAnimation(string stateName, float blendTime = 0f)
	{
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].CrossFadeInFixedTime(stateName, blendTime);
		}
	}

	// Token: 0x060043A2 RID: 17314 RVA: 0x0016AE28 File Offset: 0x00169028
	public bool IsAnimationPlaying(string stateName)
	{
		int num = 0;
		if (num >= this.animators.Length)
		{
			return false;
		}
		Animator animator = this.animators[num];
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		return (currentAnimatorStateInfo.IsName(stateName) && currentAnimatorStateInfo.normalizedTime < 1f) || animator.GetNextAnimatorStateInfo(0).IsName(stateName);
	}

	// Token: 0x060043A3 RID: 17315 RVA: 0x0016AE88 File Offset: 0x00169088
	public void SetupBehaviours(AIAgent aiAgent)
	{
		this.allowTargetingTaggedPlayers = aiAgent.allowTargetingTaggedPlayers;
		for (int i = 0; i < aiAgent.agentBehaviours.Count; i++)
		{
			if (!this.usedBehaviours.Contains(aiAgent.agentBehaviours[i]))
			{
				switch (aiAgent.agentBehaviours[i])
				{
				case AgentBehaviours.Search:
					this.behaviourDict[AgentBehaviours.Search] = new CustomMapsSearchBehaviour(this, aiAgent);
					break;
				case AgentBehaviours.Chase:
					this.behaviourDict[AgentBehaviours.Chase] = new CustomMapsChaseBehaviour(this, aiAgent);
					break;
				case AgentBehaviours.Attack:
					this.behaviourDict[AgentBehaviours.Attack] = new CustomMapsAttackBehaviour(this, aiAgent);
					break;
				default:
					goto IL_A1;
				}
				this.usedBehaviours.Add(aiAgent.agentBehaviours[i]);
			}
			IL_A1:;
		}
	}

	// Token: 0x060043A4 RID: 17316 RVA: 0x0016AF4B File Offset: 0x0016914B
	public void StopMoving()
	{
		this.RequestDestination(base.transform.position);
	}

	// Token: 0x060043A5 RID: 17317 RVA: 0x0016AF5E File Offset: 0x0016915E
	public void RequestDestination(Vector3 destination)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.agent.RequestDestination(destination);
	}

	// Token: 0x060043A6 RID: 17318 RVA: 0x0016AF7C File Offset: 0x0016917C
	private void OnThink()
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (this.behaviourDict == null || this.behaviourDict.Count == 0)
		{
			return;
		}
		int num = -1;
		if (this.currentBehaviourIndex != -1 && this.behaviourDict[this.usedBehaviours[this.currentBehaviourIndex]].CanContinueExecuting())
		{
			num = this.currentBehaviourIndex;
		}
		else
		{
			for (int i = 0; i < this.usedBehaviours.Count; i++)
			{
				if (i != this.currentBehaviourIndex && this.behaviourDict[this.usedBehaviours[i]].CanExecute())
				{
					num = i;
					break;
				}
			}
		}
		if (num == -1)
		{
			return;
		}
		if (this.currentBehaviourIndex != num)
		{
			this.currentBehaviourIndex = num;
			this.currentBehaviour = this.usedBehaviours[num];
			this.agent.RequestBehaviorChange((byte)this.currentBehaviour);
		}
		this.behaviourDict[this.currentBehaviour].Execute();
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x0016B074 File Offset: 0x00169274
	private void OnNetworkBehaviourStateChanged(byte newstate)
	{
		if (newstate < 0 || newstate >= 3)
		{
			return;
		}
		if (!this.behaviourDict.ContainsKey((AgentBehaviours)newstate))
		{
			return;
		}
		if (this.currentBehaviour != (AgentBehaviours)newstate && this.behaviourDict.ContainsKey(this.currentBehaviour))
		{
			this.behaviourDict[this.currentBehaviour].ResetBehavior();
		}
		this.currentBehaviour = (AgentBehaviours)newstate;
		this.behaviourDict[this.currentBehaviour].NetExecute();
	}

	// Token: 0x060043A8 RID: 17320 RVA: 0x0016B0EC File Offset: 0x001692EC
	public void OnEntityInit()
	{
		bool flag = AISpawnManager.HasInstance && AISpawnManager.instance != null;
		if (!flag && MapSpawnManager.instance == null)
		{
			return;
		}
		this.entity.transform.parent = (flag ? AISpawnManager.instance.transform : MapSpawnManager.instance.transform);
		byte enemyTypeIndex;
		AIAgent.UnpackCreateData(this.entity.createData, out enemyTypeIndex, out this.luaAgentID);
		AIAgent newEnemy;
		if (flag && AISpawnManager.instance.SpawnEnemy((int)enemyTypeIndex, out newEnemy))
		{
			this.SetupNewEnemy(newEnemy);
			return;
		}
		MapEntity mapEntity;
		if (!flag && MapSpawnManager.instance.SpawnEntity((int)enemyTypeIndex, out mapEntity))
		{
			this.SetupNewEnemy((AIAgent)mapEntity);
			return;
		}
		GTDev.LogError<string>("CustomMapsAIBehaviourController::OnEntityInit could not spawn enemy", null);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060043A9 RID: 17321 RVA: 0x0016B1B0 File Offset: 0x001693B0
	private void SetupNewEnemy(AIAgent newEnemy)
	{
		newEnemy.gameObject.SetActive(true);
		newEnemy.transform.parent = this.entity.transform;
		newEnemy.transform.localPosition = Vector3.zero;
		newEnemy.transform.localRotation = Quaternion.identity;
		this.InitAnimators();
		NavMeshAgent component = this.entity.gameObject.GetComponent<NavMeshAgent>();
		if (component.IsNull())
		{
			GTDev.LogError<string>("nav mesh agent is null", null);
			Object.Destroy(base.gameObject);
			return;
		}
		component.agentTypeID = this.GetNavAgentType(newEnemy.navAgentType);
		component.speed = newEnemy.movementSpeed;
		component.angularSpeed = newEnemy.turnSpeed;
		component.acceleration = newEnemy.acceleration;
		this.SetupBehaviours(newEnemy);
	}

	// Token: 0x060043AA RID: 17322 RVA: 0x0016B274 File Offset: 0x00169474
	private int GetNavAgentType(NavAgentType navType)
	{
		int settingsCount = NavMesh.GetSettingsCount();
		int agentTypeID = NavMesh.GetSettingsByIndex(0).agentTypeID;
		for (int i = 0; i < settingsCount; i++)
		{
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(i);
			if (NavMesh.GetSettingsNameFromID(settingsByIndex.agentTypeID) == navType.ToString())
			{
				agentTypeID = settingsByIndex.agentTypeID;
				break;
			}
		}
		return agentTypeID;
	}

	// Token: 0x060043AB RID: 17323 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060043AC RID: 17324 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x060043AD RID: 17325 RVA: 0x0016B2D8 File Offset: 0x001694D8
	public GRPlayer FindBestTarget(Vector3 sourcePos, float maxRange, float maxRangeSq, float minDotVal)
	{
		float num = 0f;
		GRPlayer result = null;
		this.tempRigs.Clear();
		this.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.tempRigs);
		Vector3 rhs = base.transform.rotation * Vector3.forward;
		for (int i = 0; i < this.tempRigs.Count; i++)
		{
			GRPlayer component = this.tempRigs[i].GetComponent<GRPlayer>();
			Vector3 vector;
			if (this.IsTargetInRange(sourcePos, component, maxRangeSq, out vector))
			{
				float num2 = 0f;
				if (vector.sqrMagnitude > 0f)
				{
					num2 = Mathf.Sqrt(vector.magnitude);
				}
				float num3 = Vector3.Dot(vector.normalized, rhs);
				if (num3 >= minDotVal)
				{
					float num4 = Mathf.Lerp(0f, 0.5f, 1f - num2 / maxRange);
					float num5 = Mathf.Lerp(0f, 0.5f, (1f - minDotVal - (1f - num3)) / (1f - minDotVal));
					if (num4 + num5 > num && this.IsTargetVisible(sourcePos, component, maxRange))
					{
						num = num4 + num5;
						result = component;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060043AE RID: 17326 RVA: 0x0016B40C File Offset: 0x0016960C
	public bool IsTargetVisible(Vector3 startPos, GRPlayer target, float maxDist)
	{
		if (!this.IsTargetable(target))
		{
			return false;
		}
		int num = Physics.RaycastNonAlloc(new Ray(startPos, target.transform.position - startPos), CustomMapsAIBehaviourController.visibilityHits, Mathf.Min(Vector3.Distance(target.transform.position, startPos), maxDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			if (CustomMapsAIBehaviourController.visibilityHits[i].transform != base.transform && !CustomMapsAIBehaviourController.visibilityHits[i].transform.IsChildOf(base.transform))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060043AF RID: 17327 RVA: 0x0016B4B4 File Offset: 0x001696B4
	public bool IsTargetInRange(Vector3 startPos, GRPlayer target, float maxRangeSq, out Vector3 toTarget)
	{
		toTarget = Vector3.zero;
		if (!this.IsTargetable(target))
		{
			return false;
		}
		Vector3 position = target.transform.position;
		toTarget = position - startPos;
		return toTarget.sqrMagnitude <= maxRangeSq;
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x0016B500 File Offset: 0x00169700
	public bool IsTargetable(GRPlayer potentialTarget)
	{
		if (potentialTarget.IsNull())
		{
			return false;
		}
		if (potentialTarget.State == GRPlayer.GRPlayerState.Ghost)
		{
			return false;
		}
		if (potentialTarget.MyRig.isLocal)
		{
			if (CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				return false;
			}
		}
		else if (CustomMapManager.IsRemotePlayerInVirtualStump(potentialTarget.MyRig.OwningNetPlayer.UserId))
		{
			return false;
		}
		return this.allowTargetingTaggedPlayers || GameMode.ActiveGameMode.GameType() == GameModeType.Custom || !GameMode.LocalIsTagged(potentialTarget.MyRig.OwningNetPlayer);
	}

	// Token: 0x040055A8 RID: 21928
	private static readonly int movementSpeedParamIndex = Animator.StringToHash("MovementSpeed");

	// Token: 0x040055A9 RID: 21929
	public GameEntity entity;

	// Token: 0x040055AA RID: 21930
	public GameAgent agent;

	// Token: 0x040055AB RID: 21931
	public GRAttributes attributes;

	// Token: 0x040055AC RID: 21932
	private Animator[] animators;

	// Token: 0x040055AD RID: 21933
	public short luaAgentID;

	// Token: 0x040055AE RID: 21934
	private List<VRRig> tempRigs = new List<VRRig>(20);

	// Token: 0x040055AF RID: 21935
	private static RaycastHit[] visibilityHits = new RaycastHit[10];

	// Token: 0x040055B0 RID: 21936
	private LayerMask visibilityLayerMask;

	// Token: 0x040055B1 RID: 21937
	private bool allowTargetingTaggedPlayers;

	// Token: 0x040055B3 RID: 21939
	private Dictionary<AgentBehaviours, CustomMapsBehaviourBase> behaviourDict = new Dictionary<AgentBehaviours, CustomMapsBehaviourBase>(8);

	// Token: 0x040055B4 RID: 21940
	private List<AgentBehaviours> usedBehaviours = new List<AgentBehaviours>(8);

	// Token: 0x040055B5 RID: 21941
	private AgentBehaviours currentBehaviour;

	// Token: 0x040055B6 RID: 21942
	private int currentBehaviourIndex;

	// Token: 0x040055B7 RID: 21943
	private const int BEHAVIOUR_COUNT = 3;

	// Token: 0x02000A51 RID: 2641
	public enum CustomMapsAIBehaviour
	{
		// Token: 0x040055B9 RID: 21945
		Search,
		// Token: 0x040055BA RID: 21946
		Chase,
		// Token: 0x040055BB RID: 21947
		Attack
	}
}
