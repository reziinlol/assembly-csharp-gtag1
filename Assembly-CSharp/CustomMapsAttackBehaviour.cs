using System;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A4C RID: 2636
public class CustomMapsAttackBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x0600437C RID: 17276 RVA: 0x0016A338 File Offset: 0x00168538
	public CustomMapsAttackBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.attackType = agentSettings.attackType;
		this.attackDist = agentSettings.attackDist;
		this.attackDistSq = this.attackDist * this.attackDist;
		this.stopMovingToAttack = agentSettings.stopMovingToAttack;
		this.useColliders = agentSettings.useColliders;
		this.damageDelayAfterPlayingAnimation = agentSettings.damageDelayAfterPlayingAnim;
		this.damageAmount = agentSettings.damageAmount;
		this.attackAnimName = agentSettings.attackAnimName;
		this.sightOffset = agentSettings.sightOffset;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIController;
		this.animBlendTime = agentSettings.animBlendTime;
		this.turnSpeed = agentSettings.turnSpeed * 10f;
		this.timeBetweenAttacks = agentSettings.timeBetweenAttacks;
		this.controller.attributes.AddAttribute(GRAttributeType.PlayerDamage, this.damageAmount);
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x0600437D RID: 17277 RVA: 0x0016A437 File Offset: 0x00168637
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull() && this.IsTargetInAttackRange(null) && this.IsTargetVisible();
	}

	// Token: 0x0600437E RID: 17278 RVA: 0x0016A470 File Offset: 0x00168670
	private bool IsTargetVisible()
	{
		Vector3 startPos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(startPos, this.controller.TargetPlayer, this.attackDist);
	}

	// Token: 0x0600437F RID: 17279 RVA: 0x0016A4C8 File Offset: 0x001686C8
	private bool IsTargetInAttackRange(GRPlayer target = null)
	{
		if (target.IsNull() && this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (target.IsNotNull())
		{
			Vector3 vector;
			return this.controller.IsTargetInRange(this.controller.transform.position, target, this.attackDistSq, out vector);
		}
		Vector3 vector2;
		return this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.attackDistSq, out vector2);
	}

	// Token: 0x06004380 RID: 17280 RVA: 0x0016A54C File Offset: 0x0016874C
	public override bool CanContinueExecuting()
	{
		if (this.state != CustomMapsAttackBehaviour.State.Idle && this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return true;
		}
		if (this.controller.IsNull() || this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.ClearTarget();
			return false;
		}
		return this.CanExecute();
	}

	// Token: 0x06004381 RID: 17281 RVA: 0x0016A5C2 File Offset: 0x001687C2
	public override void Execute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.stopMovingToAttack)
		{
			this.controller.StopMoving();
		}
		this.FaceTarget();
		this.controller.agent.RequestBehaviorChange(2);
	}

	// Token: 0x06004382 RID: 17282 RVA: 0x0016A5FC File Offset: 0x001687FC
	public override void NetExecute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.state == CustomMapsAttackBehaviour.State.Attacking && !this.useColliders && this.startTime > this.lastAttackTime && Time.time > this.startTime + this.damageDelayAfterPlayingAnimation)
		{
			this.TriggerAttack(null);
		}
		if (this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return;
		}
		CustomMapsAttackBehaviour.State state = this.state;
		if (state != CustomMapsAttackBehaviour.State.Idle)
		{
			if (state != CustomMapsAttackBehaviour.State.Attacking)
			{
				return;
			}
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				this.state = CustomMapsAttackBehaviour.State.Idle;
				return;
			}
			this.startTime = Time.time;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
		else
		{
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				return;
			}
			this.startTime = Time.time;
			this.state = CustomMapsAttackBehaviour.State.Attacking;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
	}

	// Token: 0x06004383 RID: 17283 RVA: 0x0016A6F1 File Offset: 0x001688F1
	public override void ResetBehavior()
	{
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x06004384 RID: 17284 RVA: 0x0016A6FC File Offset: 0x001688FC
	private void FaceTarget()
	{
		if (this.controller.TargetPlayer.IsNull())
		{
			return;
		}
		GameAgent.UpdateFacingTarget(this.controller.transform, this.controller.agent.navAgent, this.controller.TargetPlayer.transform, this.turnSpeed);
	}

	// Token: 0x06004385 RID: 17285 RVA: 0x0016A754 File Offset: 0x00168954
	public override void OnTriggerEnter(Collider otherCollider)
	{
		if (!this.useColliders)
		{
			return;
		}
		if (Time.time < this.lastAttackTime + this.timeBetweenAttacks || this.state != CustomMapsAttackBehaviour.State.Attacking)
		{
			return;
		}
		GRPlayer componentInParent = otherCollider.GetComponentInParent<GRPlayer>();
		if (componentInParent.IsNull())
		{
			return;
		}
		if (componentInParent.MyRig.IsNotNull() && !componentInParent.MyRig.isLocal)
		{
			return;
		}
		if (componentInParent.State == GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		this.TriggerAttack(componentInParent);
	}

	// Token: 0x06004386 RID: 17286 RVA: 0x0016A7C4 File Offset: 0x001689C4
	private void TriggerAttack(GRPlayer targetPlayer = null)
	{
		this.lastAttackTime = Time.time;
		GRPlayer grplayer = (targetPlayer != null) ? targetPlayer : (this.controller.entity.IsAuthority() ? this.controller.TargetPlayer : null);
		if (!this.controller.entity.IsAuthority() && grplayer == null)
		{
			Vector3 sourcePos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
			grplayer = this.controller.FindBestTarget(sourcePos, this.attackDist, this.attackDistSq, this.sightMinDot);
		}
		if (grplayer == null)
		{
			return;
		}
		if (!grplayer.MyRig.isLocal)
		{
			return;
		}
		if (this.controller.entity.IsAuthority() && !this.IsTargetInAttackRange(grplayer))
		{
			return;
		}
		switch (this.attackType)
		{
		case AttackType.Tag:
			if (GameMode.ActiveGameMode.GameType() != GameModeType.Custom)
			{
				GameMode.ReportHit();
				return;
			}
			CustomGameMode.TaggedByAI(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber);
			return;
		case AttackType.UseGT:
			CustomMapsGameManager.instance.OnPlayerHit(this.controller.entity.id, grplayer, this.controller.transform.position);
			return;
		case AttackType.UseLuau:
			CustomGameMode.OnPlayerHit(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber, this.damageAmount);
			return;
		default:
			return;
		}
	}

	// Token: 0x04005583 RID: 21891
	private CustomMapsAIBehaviourController controller;

	// Token: 0x04005584 RID: 21892
	private CustomMapsAttackBehaviour.State state;

	// Token: 0x04005585 RID: 21893
	private AttackType attackType;

	// Token: 0x04005586 RID: 21894
	private float attackDist;

	// Token: 0x04005587 RID: 21895
	private float attackDistSq;

	// Token: 0x04005588 RID: 21896
	private bool stopMovingToAttack;

	// Token: 0x04005589 RID: 21897
	private bool useColliders;

	// Token: 0x0400558A RID: 21898
	private float damageAmount;

	// Token: 0x0400558B RID: 21899
	private Vector3 sightOffset;

	// Token: 0x0400558C RID: 21900
	private float sightFOV;

	// Token: 0x0400558D RID: 21901
	private float sightMinDot;

	// Token: 0x0400558E RID: 21902
	private string attackAnimName;

	// Token: 0x0400558F RID: 21903
	private float timeBetweenAttacks;

	// Token: 0x04005590 RID: 21904
	private float damageDelayAfterPlayingAnimation;

	// Token: 0x04005591 RID: 21905
	private float animBlendTime;

	// Token: 0x04005592 RID: 21906
	private float startTime;

	// Token: 0x04005593 RID: 21907
	private float turnSpeed;

	// Token: 0x04005594 RID: 21908
	private float lastAttackTime;

	// Token: 0x02000A4D RID: 2637
	private enum State
	{
		// Token: 0x04005596 RID: 21910
		Idle,
		// Token: 0x04005597 RID: 21911
		Attacking
	}
}
