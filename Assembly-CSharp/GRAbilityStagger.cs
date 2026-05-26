using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000727 RID: 1831
[Serializable]
public class GRAbilityStagger : GRAbilityBase
{
	// Token: 0x06002E78 RID: 11896 RVA: 0x000FDDF4 File Offset: 0x000FBFF4
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x000FDE00 File Offset: 0x000FC000
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 a = vel / magnitude;
			a.y = 0f;
			vel = a * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.duration);
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x000FDE4C File Offset: 0x000FC04C
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.staggerMovement.Setup(root);
		this.staggerMovement.interpolationType = GRAbilityInterpolatedMovement.InterpType.EaseOut;
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000FDE78 File Offset: 0x000FC078
	protected override void OnStart()
	{
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration + this.stunTime;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f + this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.duration);
		this.staggerMovement.Start();
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000FDF7D File Offset: 0x000FC17D
	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000FDF98 File Offset: 0x000FC198
	public override bool IsDone()
	{
		return this.staggerMovement.IsDone();
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000FDFA5 File Offset: 0x000FC1A5
	protected override void OnUpdateShared(float dt)
	{
		this.staggerMovement.Update(dt);
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000FDFB3 File Offset: 0x000FC1B3
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x04003B8F RID: 15247
	private float duration;

	// Token: 0x04003B90 RID: 15248
	public List<AnimationData> animData;

	// Token: 0x04003B91 RID: 15249
	private int lastAnimIndex = -1;

	// Token: 0x04003B92 RID: 15250
	private string animNameString;

	// Token: 0x04003B93 RID: 15251
	public GRAbilityInterpolatedMovement staggerMovement;

	// Token: 0x04003B94 RID: 15252
	private float stunTime;
}
