using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000732 RID: 1842
[Serializable]
public class GRAbilityAttackSimple : GRAbilityBase
{
	// Token: 0x06002ED0 RID: 11984 RVA: 0x000FF765 File Offset: 0x000FD965
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.EnableList(this.damageTrigger, false);
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x000FF784 File Offset: 0x000FD984
	protected override void OnStart()
	{
		if ((double)(this.tellDuration * this.timeMult) > 0.0)
		{
			this.PlayState(GRAbilityAttackSimple.State.Tell, this.tellAnimData, this.soundTell, false);
		}
		else
		{
			this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
		}
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(false, true);
			this.agent.SetDisableNetworkSync(true);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x000FF81C File Offset: 0x000FDA1C
	protected override void OnStop()
	{
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
		}
		this.EnableList(this.damageTrigger, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000FF874 File Offset: 0x000FDA74
	private void PlayState(GRAbilityAttackSimple.State newState, AnimationData animData, AbilitySound sound, bool damageEnabled)
	{
		if (!string.IsNullOrEmpty(animData.animName))
		{
			this.PlayAnim(animData.animName, 0.1f, animData.speed);
			this.animNameString = animData.animName;
			this.timeMult = ((this.adjustByAnimationSpeed && !Mathf.Approximately(animData.speed, 0f)) ? (1f / animData.speed) : 1f);
		}
		sound.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		sound.Play(null);
		this.EnableList(this.damageTrigger, damageEnabled);
		this.state = newState;
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x000FF907 File Offset: 0x000FDB07
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSimple.State.Done;
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x000FF914 File Offset: 0x000FDB14
	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSimple.State.Tell:
			if (num > this.tellDuration * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
			}
			break;
		case GRAbilityAttackSimple.State.Attack:
			if (num > (this.tellDuration + this.attackDuration) * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.FollowThrough, this.outroAnimData, this.soundOutro, false);
			}
			break;
		case GRAbilityAttackSimple.State.FollowThrough:
			if (num >= this.duration * this.timeMult)
			{
				this.state = GRAbilityAttackSimple.State.Done;
			}
			break;
		}
		this.events.TryPlay(num / this.timeMult, this.audioSource);
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x000FF9CD File Offset: 0x000FDBCD
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x000FF9D8 File Offset: 0x000FDBD8
	public void EnableList(List<GameObject> objs, bool enable)
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i] != null)
			{
				objs[i].SetActive(enable);
			}
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x000FFA12 File Offset: 0x000FDC12
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x000FFA20 File Offset: 0x000FDC20
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003BDF RID: 15327
	public float duration;

	// Token: 0x04003BE0 RID: 15328
	public float tellDuration;

	// Token: 0x04003BE1 RID: 15329
	public float attackDuration;

	// Token: 0x04003BE2 RID: 15330
	public float coolDown;

	// Token: 0x04003BE3 RID: 15331
	public float range;

	// Token: 0x04003BE4 RID: 15332
	public bool allowMovement;

	// Token: 0x04003BE5 RID: 15333
	public AnimationData tellAnimData;

	// Token: 0x04003BE6 RID: 15334
	public AnimationData attackAnimData;

	// Token: 0x04003BE7 RID: 15335
	public AnimationData outroAnimData;

	// Token: 0x04003BE8 RID: 15336
	public AbilitySound soundTell;

	// Token: 0x04003BE9 RID: 15337
	public AbilitySound soundAttack;

	// Token: 0x04003BEA RID: 15338
	public AbilitySound soundOutro;

	// Token: 0x04003BEB RID: 15339
	private float timeMult = 1f;

	// Token: 0x04003BEC RID: 15340
	private GRAbilityAttackSimple.State state;

	// Token: 0x04003BED RID: 15341
	public float maxTurnSpeed;

	// Token: 0x04003BEE RID: 15342
	public List<GameObject> damageTrigger;

	// Token: 0x04003BEF RID: 15343
	private string animNameString;

	// Token: 0x04003BF0 RID: 15344
	public GameAbilityEvents events;

	// Token: 0x04003BF1 RID: 15345
	public bool adjustByAnimationSpeed;

	// Token: 0x02000733 RID: 1843
	private enum State
	{
		// Token: 0x04003BF3 RID: 15347
		Tell,
		// Token: 0x04003BF4 RID: 15348
		Attack,
		// Token: 0x04003BF5 RID: 15349
		FollowThrough,
		// Token: 0x04003BF6 RID: 15350
		Done
	}
}
