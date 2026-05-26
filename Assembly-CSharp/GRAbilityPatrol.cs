using System;
using CjLib;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200072D RID: 1837
[Serializable]
public class GRAbilityPatrol : GRAbilityBase
{
	// Token: 0x06002EB2 RID: 11954 RVA: 0x000FEF7C File Offset: 0x000FD17C
	public bool HasValidPatrolPath()
	{
		return this.patrolPath != null && this.patrolPath.patrolNodes.Count > 1;
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x000FEFA4 File Offset: 0x000FD1A4
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.InitializeRandoms();
		this.nextPatrolNode = 0;
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000FF024 File Offset: 0x000FD224
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000FF054 File Offset: 0x000FD254
	protected override void OnStart()
	{
		this.moveAbility.Start();
		this.agent.SetIsPathing(true, true);
		if (this.patrolPath != null)
		{
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		else
		{
			Debug.LogError("Starting patrol ability with no patrol path");
		}
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000FF0BA File Offset: 0x000FD2BA
	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000FF0C7 File Offset: 0x000FD2C7
	public void SetPatrolPath(GRPatrolPath patrolPath)
	{
		this.patrolPath = patrolPath;
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000FF0D0 File Offset: 0x000FD2D0
	public GRPatrolPath GetPatrolPath()
	{
		return this.patrolPath;
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000FF0D8 File Offset: 0x000FD2D8
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x000FF0E1 File Offset: 0x000FD2E1
	public void CalculateNextPatrolGroan()
	{
		this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x000FF108 File Offset: 0x000FD308
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000FF160 File Offset: 0x000FD360
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (this.moveAbility.IsDone())
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x000FF204 File Offset: 0x000FD404
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.SetTarget(null);
		this.moveAbility.SetTargetPos(this.agent.navAgent.destination);
		this.moveAbility.UpdateRemote(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x04003BB9 RID: 15289
	private NavMeshAgent navMeshAgent;

	// Token: 0x04003BBA RID: 15290
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003BBB RID: 15291
	private GRPatrolPath patrolPath;

	// Token: 0x04003BBC RID: 15292
	public double lastStateChange;

	// Token: 0x04003BBD RID: 15293
	public float ambientSoundVolume = 0.5f;

	// Token: 0x04003BBE RID: 15294
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x04003BBF RID: 15295
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x04003BC0 RID: 15296
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x04003BC1 RID: 15297
	private double lastPartrolAmbientSoundTime;

	// Token: 0x04003BC2 RID: 15298
	private double nextPatrolGroanTime;

	// Token: 0x04003BC3 RID: 15299
	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	// Token: 0x04003BC4 RID: 15300
	private Unity.Mathematics.Random patrolGroanSoundRandom;

	// Token: 0x04003BC5 RID: 15301
	[ReadOnly]
	public int nextPatrolNode;
}
