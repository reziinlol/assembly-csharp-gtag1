using System;
using UnityEngine;

// Token: 0x02000735 RID: 1845
[Serializable]
public class GRAbilityAttackJump : GRAbilityBase
{
	// Token: 0x06002EE5 RID: 12005 RVA: 0x000FFC59 File Offset: 0x000FDE59
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000FFC8C File Offset: 0x000FDE8C
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.state = GRAbilityAttackJump.State.Tell;
	}

	// Token: 0x06002EE7 RID: 12007 RVA: 0x000FFCF5 File Offset: 0x000FDEF5
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EE8 RID: 12008 RVA: 0x000FFD2A File Offset: 0x000FDF2A
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002EE9 RID: 12009 RVA: 0x000FFD44 File Offset: 0x000FDF44
	protected override void OnUpdateShared(float dt)
	{
		double num = (double)((float)Time.timeAsDouble) - this.startTime;
		switch (this.state)
		{
		case GRAbilityAttackJump.State.Tell:
			if (num > (double)this.jumpTime)
			{
				this.targetPos = this.agent.transform.position + this.agent.transform.forward * 0.5f;
				if (this.target != null)
				{
					Vector3 a = this.target.transform.position - this.agent.transform.position;
					this.targetPos = this.agent.transform.position + a * this.jumpLengthScale;
					this.targetPos.y = this.target.transform.position.y;
				}
				float num2 = this.attackLandTime - this.jumpTime;
				num2 = Mathf.Max(0.1f, num2);
				this.initialPos = this.agent.transform.position;
				Vector3 vector = this.targetPos - this.initialPos;
				float y = vector.y;
				vector.y = 0f;
				float num3 = num2;
				float y2 = 0f;
				if (num3 > 0f)
				{
					Vector3 gravity = Physics.gravity;
					y2 = (y - 0.5f * gravity.y * num3 * num3) / num3;
				}
				this.initialVel = vector / num2;
				this.initialVel.y = y2;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.PlayAnim(this.jumpAnimName, 0.1f, this.animSpeed);
				this.jumpSound.Play(null);
				this.state = GRAbilityAttackJump.State.Jump;
			}
			break;
		case GRAbilityAttackJump.State.Jump:
		{
			float d = (float)(num - (double)this.jumpTime);
			Vector3 position = this.initialPos + this.initialVel * d + 0.5f * Physics.gravity * d * d;
			this.root.position = position;
			if (num > (double)this.attackLandTime)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.doReturnPhase)
				{
					float num4 = this.attackReturnTime - this.attackLandTime;
					num4 = Mathf.Max(0.1f, num4);
					Vector3 a2 = this.initialPos;
					this.initialPos = this.agent.transform.position;
					this.initialVel = (a2 - this.initialPos) / num4;
					this.state = GRAbilityAttackJump.State.Return;
				}
				else
				{
					this.state = GRAbilityAttackJump.State.Done;
				}
			}
			break;
		}
		case GRAbilityAttackJump.State.Return:
		{
			float d2 = (float)(num - (double)this.attackLandTime);
			Vector3 position2 = this.initialPos + this.initialVel * d2;
			this.root.position = position2;
			if (num > (double)this.attackReturnTime)
			{
				this.state = GRAbilityAttackJump.State.Done;
			}
			break;
		}
		}
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x0010007C File Offset: 0x000FE27C
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

	// Token: 0x04003C00 RID: 15360
	public float duration;

	// Token: 0x04003C01 RID: 15361
	public float jumpTime;

	// Token: 0x04003C02 RID: 15362
	public float attackLandTime;

	// Token: 0x04003C03 RID: 15363
	public float attackReturnTime;

	// Token: 0x04003C04 RID: 15364
	public bool doReturnPhase = true;

	// Token: 0x04003C05 RID: 15365
	public float jumpLengthScale = 1f;

	// Token: 0x04003C06 RID: 15366
	public string animName;

	// Token: 0x04003C07 RID: 15367
	public float animSpeed;

	// Token: 0x04003C08 RID: 15368
	public float maxTurnSpeed;

	// Token: 0x04003C09 RID: 15369
	public string jumpAnimName;

	// Token: 0x04003C0A RID: 15370
	public AbilitySound jumpSound;

	// Token: 0x04003C0B RID: 15371
	public GameObject damageTrigger;

	// Token: 0x04003C0C RID: 15372
	private Transform target;

	// Token: 0x04003C0D RID: 15373
	private GRAbilityAttackJump.State state;

	// Token: 0x04003C0E RID: 15374
	public Vector3 targetPos;

	// Token: 0x04003C0F RID: 15375
	public Vector3 initialPos;

	// Token: 0x04003C10 RID: 15376
	public Vector3 initialVel;

	// Token: 0x02000736 RID: 1846
	private enum State
	{
		// Token: 0x04003C12 RID: 15378
		Tell,
		// Token: 0x04003C13 RID: 15379
		Jump,
		// Token: 0x04003C14 RID: 15380
		Return,
		// Token: 0x04003C15 RID: 15381
		Done
	}
}
