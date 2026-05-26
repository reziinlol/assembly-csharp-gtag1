using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000737 RID: 1847
[Serializable]
public class GRAbilityAttackLaser : GRAbilityBase
{
	// Token: 0x06002EEC RID: 12012 RVA: 0x001000D6 File Offset: 0x000FE2D6
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002EED RID: 12013 RVA: 0x00100108 File Offset: 0x000FE308
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
		this.state = GRAbilityAttackLaser.State.Tell;
	}

	// Token: 0x06002EEE RID: 12014 RVA: 0x00100214 File Offset: 0x000FE414
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		if (this.laserFx != null)
		{
			this.laserFx.DisableLazer();
		}
		if (this.tellLaserFx != null)
		{
			this.tellLaserFx.DisableLazer();
		}
	}

	// Token: 0x06002EEF RID: 12015 RVA: 0x00100286 File Offset: 0x000FE486
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackLaser.State.Done;
	}

	// Token: 0x06002EF0 RID: 12016 RVA: 0x00100294 File Offset: 0x000FE494
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackLaser.State.Tell:
		{
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			Vector3 position = this.head.position;
			Vector3 a = this.targetPos - position;
			float num2 = a.magnitude;
			if (num2 > 0f)
			{
				Vector3 a2 = a / num2;
				num2 = Mathf.Min(this.maxLaserRange, num2);
				this.targetPos = position + a2 * num2;
			}
			if (!this.doNotFaceTarget)
			{
				GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			}
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackLaser.State.Attack;
				if (this.damageCollider != null && this.laserOrigins.Length != 0)
				{
					this.damageCollider.transform.position = (position + this.targetPos) / 2f;
					this.damageCollider.height = num2;
					this.damageCollider.direction = 2;
					if (num2 > 0f)
					{
						this.damageCollider.transform.rotation = Quaternion.LookRotation(a / num2);
					}
				}
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				if (this.tellLaserFx != null)
				{
					this.tellLaserFx.DisableLazer();
				}
				if (this.laserFx != null && this.target != null)
				{
					GamePlayer component = this.target.GetComponent<GamePlayer>();
					if (component != null && component.rig != null)
					{
						this.laserFx.EnableLazer(this.laserOrigins, this.targetPos);
					}
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			if (this.tellLaserFx != null)
			{
				this.tellLaserFx.EnableLazer(this.laserOrigins, this.targetPos);
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.Attack:
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
				if (this.laserFx != null)
				{
					this.laserFx.DisableLazer();
				}
				this.state = GRAbilityAttackLaser.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackLaser.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002EF1 RID: 12017 RVA: 0x001005E0 File Offset: 0x000FE7E0
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

	// Token: 0x06002EF2 RID: 12018 RVA: 0x00100620 File Offset: 0x000FE820
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002EF3 RID: 12019 RVA: 0x00100628 File Offset: 0x000FE828
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002EF4 RID: 12020 RVA: 0x00100636 File Offset: 0x000FE836
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003C16 RID: 15382
	public float duration;

	// Token: 0x04003C17 RID: 15383
	public float tellDuration;

	// Token: 0x04003C18 RID: 15384
	public float attackDuration;

	// Token: 0x04003C19 RID: 15385
	public float coolDown;

	// Token: 0x04003C1A RID: 15386
	public float range;

	// Token: 0x04003C1B RID: 15387
	public float attackMoveSpeed;

	// Token: 0x04003C1C RID: 15388
	public bool doNotFaceTarget;

	// Token: 0x04003C1D RID: 15389
	public List<AnimationData> animData;

	// Token: 0x04003C1E RID: 15390
	public AbilitySound soundAttack;

	// Token: 0x04003C1F RID: 15391
	public float maxLaserRange;

	// Token: 0x04003C20 RID: 15392
	public Transform[] laserOrigins;

	// Token: 0x04003C21 RID: 15393
	public Monkeye_LazerFX tellLaserFx;

	// Token: 0x04003C22 RID: 15394
	public Monkeye_LazerFX laserFx;

	// Token: 0x04003C23 RID: 15395
	private GRAbilityAttackLaser.State state;

	// Token: 0x04003C24 RID: 15396
	public float maxTurnSpeed;

	// Token: 0x04003C25 RID: 15397
	public GameObject damageTrigger;

	// Token: 0x04003C26 RID: 15398
	public CapsuleCollider damageCollider;

	// Token: 0x04003C27 RID: 15399
	private Transform target;

	// Token: 0x04003C28 RID: 15400
	private string animNameString;

	// Token: 0x04003C29 RID: 15401
	private int lastAnimIndex = -1;

	// Token: 0x04003C2A RID: 15402
	public Vector3 targetPos;

	// Token: 0x04003C2B RID: 15403
	public Vector3 initialPos;

	// Token: 0x04003C2C RID: 15404
	public Vector3 initialVel;

	// Token: 0x02000738 RID: 1848
	private enum State
	{
		// Token: 0x04003C2E RID: 15406
		Tell,
		// Token: 0x04003C2F RID: 15407
		Attack,
		// Token: 0x04003C30 RID: 15408
		FollowThrough,
		// Token: 0x04003C31 RID: 15409
		Done
	}
}
