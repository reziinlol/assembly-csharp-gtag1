using System;
using UnityEngine;

// Token: 0x02000734 RID: 1844
[Serializable]
public class GRAbilityAttackLatchOn : GRAbilityBase
{
	// Token: 0x06002EDC RID: 11996 RVA: 0x000FFA3B File Offset: 0x000FDC3B
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000FFA70 File Offset: 0x000FDC70
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetSpeed(this.tellMoveSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000FFACA File Offset: 0x000FDCCA
	protected override void OnStop()
	{
		this.agent.transform.SetParent(null);
		this.agent.SetIsPathing(true, true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EDF RID: 11999 RVA: 0x000FFB04 File Offset: 0x000FDD04
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002EE0 RID: 12000 RVA: 0x000FFB1E File Offset: 0x000FDD1E
	protected override void OnUpdateAuthority(float dt)
	{
		this.UpdateNavSpeed();
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002EE1 RID: 12001 RVA: 0x000FFB48 File Offset: 0x000FDD48
	protected override void OnUpdateRemote(float dt)
	{
		this.UpdateNavSpeed();
	}

	// Token: 0x06002EE2 RID: 12002 RVA: 0x000FFB50 File Offset: 0x000FDD50
	private void UpdateNavSpeed()
	{
		if (Time.timeAsDouble - this.startTime > (double)this.tellDuration)
		{
			this.agent.SetSpeed(this.attackMoveSpeed);
			this.agent.SetVelocity(this.agent.navAgent.velocity.normalized * this.attackMoveSpeed);
			if (this.damageTrigger != null)
			{
				this.damageTrigger.SetActive(true);
			}
		}
	}

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000FFBCC File Offset: 0x000FDDCC
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.agent.transform.SetParent(grplayer.attachEnemy);
				this.agent.transform.localPosition = Vector3.zero;
				this.agent.transform.localRotation = Quaternion.identity;
				this.agent.SetIsPathing(false, true);
			}
		}
	}

	// Token: 0x04003BF7 RID: 15351
	public float duration;

	// Token: 0x04003BF8 RID: 15352
	public float attackMoveSpeed;

	// Token: 0x04003BF9 RID: 15353
	public float tellDuration;

	// Token: 0x04003BFA RID: 15354
	public float tellMoveSpeed;

	// Token: 0x04003BFB RID: 15355
	public string animName;

	// Token: 0x04003BFC RID: 15356
	public float animSpeed;

	// Token: 0x04003BFD RID: 15357
	public float maxTurnSpeed;

	// Token: 0x04003BFE RID: 15358
	public Transform target;

	// Token: 0x04003BFF RID: 15359
	public GameObject damageTrigger;
}
