using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200072E RID: 1838
[Serializable]
public class GRAbilityFlashed : GRAbilityBase
{
	// Token: 0x06002EC0 RID: 11968 RVA: 0x000FF2B0 File Offset: 0x000FD4B0
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000FF2C1 File Offset: 0x000FD4C1
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000FF2CC File Offset: 0x000FD4CC
	protected override void OnStart()
	{
		if (this.flashAnimations.Count > 0)
		{
			this.flashAnimationIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.flashAnimations.Count, this.flashAnimationIndex);
			this.PlayAnim(this.flashAnimations[this.flashAnimationIndex].animName, 0.1f, this.flashAnimations[this.flashAnimationIndex].speed);
			this.behaviorEndTime = Time.timeAsDouble + (double)this.flashAnimations[this.flashAnimationIndex].duration + (double)this.stunTime;
		}
		else
		{
			this.PlayAnim("GREnemyFlashReaction01", 0.1f, 1f);
			this.behaviorEndTime = Time.timeAsDouble + 0.5 + (double)this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000FDF7D File Offset: 0x000FC17D
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000FF3B6 File Offset: 0x000FD5B6
	public override bool IsDone()
	{
		return Time.timeAsDouble >= this.behaviorEndTime;
	}

	// Token: 0x04003BC6 RID: 15302
	public List<AnimationData> flashAnimations;

	// Token: 0x04003BC7 RID: 15303
	private int flashAnimationIndex;

	// Token: 0x04003BC8 RID: 15304
	private double behaviorEndTime;

	// Token: 0x04003BC9 RID: 15305
	private float stunTime;
}
