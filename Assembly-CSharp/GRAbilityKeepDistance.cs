using System;
using CjLib;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200072C RID: 1836
[Serializable]
public class GRAbilityKeepDistance : GRAbilityBase
{
	// Token: 0x06002EA6 RID: 11942 RVA: 0x000FE988 File Offset: 0x000FCB88
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.BackupSpeed);
		}
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x000FE9FC File Offset: 0x000FCBFC
	protected override void OnStart()
	{
		if (this.target != null)
		{
			Vector3 vector = this.agent.transform.position - this.target.position;
			if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
			{
				this.agent.SetStopped(true);
				this.PlayAnim(this.idleAnimName, 0.5f, 1f);
				this.idleSound.Play(null);
			}
			else
			{
				this.moveAbility.Start();
			}
		}
		else
		{
			this.moveAbility.Start();
		}
		this.agent.SetIsPathing(true, true);
		Vector3 targetPos = this.PickBackupDestination();
		this.moveAbility.SetTargetPos(targetPos);
		if (this.navMeshAgent != null)
		{
			this.defaultUpdateRotation = this.navMeshAgent.updateRotation;
			this.navMeshAgent.updateRotation = false;
		}
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x000FEAE8 File Offset: 0x000FCCE8
	protected override void OnStop()
	{
		this.moveAbility.Stop();
		this.idleSound.Stop();
		if (this.navMeshAgent != null)
		{
			this.navMeshAgent.updateRotation = this.defaultUpdateRotation;
		}
		this.agent.SetStopped(false);
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x000FEB38 File Offset: 0x000FCD38
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.moveAbility.SetLookAtTarget(this.target);
			}
		}
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000FEB8C File Offset: 0x000FCD8C
	protected override void OnThink(float dt)
	{
		Vector3 vector = this.agent.transform.position - this.target.position;
		if (this.moveAbility.IsDone())
		{
			if (this.maxDistanceFromTarget < 0f || vector.magnitude < this.maxDistanceFromTarget)
			{
				if (this.navMeshAgent != null && this.navMeshAgent.isOnNavMesh && this.navMeshAgent.isStopped)
				{
					this.idleSound.Stop();
					this.moveAbility.Start();
				}
				Vector3 targetPos = this.PickBackupDestination();
				this.moveAbility.SetTargetPos(targetPos);
				return;
			}
		}
		else if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
		{
			this.moveAbility.SetTargetPos(this.root.position);
			this.moveAbility.Stop();
			this.agent.SetStopped(true);
			this.PlayAnim(this.idleAnimName, 0.5f, 1f);
			this.idleSound.Play(null);
		}
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x000FECA4 File Offset: 0x000FCEA4
	private Vector3 PickBackupDestination()
	{
		Vector3 position = this.agent.transform.position;
		if (this.target == null)
		{
			return position;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 vector = this.agent.transform.position - this.target.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			int i = 0;
			while (i < GRAbilityKeepDistance.rotations.Length)
			{
				Vector3 a = GRAbilityKeepDistance.rotations[i] * normalized;
				float d = 2f;
				Vector3 vector2 = position2 + a * d;
				NavMeshHit navMeshHit2;
				if (!NavMesh.Raycast(position2, vector2, out navMeshHit2, this.walkableArea))
				{
					goto IL_D6;
				}
				if (navMeshHit2.distance >= this.minBackupSpaceRequired)
				{
					vector2 = navMeshHit2.position;
					goto IL_D6;
				}
				IL_128:
				i++;
				continue;
				IL_D6:
				NavMeshHit navMeshHit3;
				if (!NavMesh.SamplePosition(vector2, out navMeshHit3, 1f, this.walkableArea))
				{
					goto IL_128;
				}
				Vector3 position3 = navMeshHit3.position;
				Vector3 vector3 = position3 - this.target.position;
				vector3.y = 0f;
				if (vector3.sqrMagnitude > vector.sqrMagnitude)
				{
					return position3;
				}
				goto IL_128;
			}
		}
		return position;
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x000FEDEE File Offset: 0x000FCFEE
	protected override void OnUpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.magenta, true);
		}
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x000FEE18 File Offset: 0x000FD018
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x000FEE26 File Offset: 0x000FD026
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	// Token: 0x04003BB0 RID: 15280
	private NavMeshAgent navMeshAgent;

	// Token: 0x04003BB1 RID: 15281
	private Transform target;

	// Token: 0x04003BB2 RID: 15282
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003BB3 RID: 15283
	public string idleAnimName;

	// Token: 0x04003BB4 RID: 15284
	public AbilitySound idleSound;

	// Token: 0x04003BB5 RID: 15285
	public float minBackupSpaceRequired = 0.5f;

	// Token: 0x04003BB6 RID: 15286
	public float maxDistanceFromTarget = -1f;

	// Token: 0x04003BB7 RID: 15287
	private bool defaultUpdateRotation;

	// Token: 0x04003BB8 RID: 15288
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 30f, 0f),
		Quaternion.Euler(0f, -30f, 0f),
		Quaternion.Euler(0f, 60f, 0f),
		Quaternion.Euler(0f, -60f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};
}
