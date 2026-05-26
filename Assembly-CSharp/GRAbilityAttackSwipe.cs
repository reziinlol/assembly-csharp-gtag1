using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000730 RID: 1840
[Serializable]
public class GRAbilityAttackSwipe : GRAbilityBase
{
	// Token: 0x06002EC7 RID: 11975 RVA: 0x000FF3C8 File Offset: 0x000FD5C8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x000FF3FC File Offset: 0x000FD5FC
	protected override void OnStart()
	{
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f;
		}
		this.soundAttack.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundAttack.Play(null);
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.state = GRAbilityAttackSwipe.State.Tell;
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000FF505 File Offset: 0x000FD705
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x000FF53A File Offset: 0x000FD73A
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSwipe.State.Done;
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x000FF548 File Offset: 0x000FD748
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSwipe.State.Tell:
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackSwipe.State.Attack;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			break;
		case GRAbilityAttackSwipe.State.Attack:
		{
			float d = num - this.tellDuration;
			Vector3 vector = this.initialPos + this.initialVel * d;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, this.walkableArea))
			{
				vector = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector, out navMeshHit, this.walkableArea))
				{
					vector = navMeshHit.position;
				}
				this.root.position = vector;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				this.state = GRAbilityAttackSwipe.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackSwipe.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackSwipe.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x000FF700 File Offset: 0x000FD900
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
			}
		}
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000FF740 File Offset: 0x000FD940
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000FF748 File Offset: 0x000FD948
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x04003BCA RID: 15306
	public float duration;

	// Token: 0x04003BCB RID: 15307
	public float tellDuration;

	// Token: 0x04003BCC RID: 15308
	public float attackDuration;

	// Token: 0x04003BCD RID: 15309
	public float coolDown;

	// Token: 0x04003BCE RID: 15310
	public float attackMoveSpeed;

	// Token: 0x04003BCF RID: 15311
	public List<AnimationData> animData;

	// Token: 0x04003BD0 RID: 15312
	public AbilitySound soundAttack;

	// Token: 0x04003BD1 RID: 15313
	private GRAbilityAttackSwipe.State state;

	// Token: 0x04003BD2 RID: 15314
	public float maxTurnSpeed;

	// Token: 0x04003BD3 RID: 15315
	public GameObject damageTrigger;

	// Token: 0x04003BD4 RID: 15316
	private Transform target;

	// Token: 0x04003BD5 RID: 15317
	private string animNameString;

	// Token: 0x04003BD6 RID: 15318
	private int lastAnimIndex = -1;

	// Token: 0x04003BD7 RID: 15319
	public Vector3 targetPos;

	// Token: 0x04003BD8 RID: 15320
	public Vector3 initialPos;

	// Token: 0x04003BD9 RID: 15321
	public Vector3 initialVel;

	// Token: 0x02000731 RID: 1841
	private enum State
	{
		// Token: 0x04003BDB RID: 15323
		Tell,
		// Token: 0x04003BDC RID: 15324
		Attack,
		// Token: 0x04003BDD RID: 15325
		FollowThrough,
		// Token: 0x04003BDE RID: 15326
		Done
	}
}
