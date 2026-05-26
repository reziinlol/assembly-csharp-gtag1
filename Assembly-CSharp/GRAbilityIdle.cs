using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200071F RID: 1823
[Serializable]
public class GRAbilityIdle : GRAbilityBase
{
	// Token: 0x06002E42 RID: 11842 RVA: 0x000FD19D File Offset: 0x000FB39D
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.animLoops = 0;
		this.cachedDuration = this.duration;
		this.cachedAnimSpeed = this.animSpeed;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000FD1D0 File Offset: 0x000FB3D0
	protected override void OnStart()
	{
		this.agent.SetStopped(true);
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.animLoops = 0;
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x000FD22E File Offset: 0x000FB42E
	protected override void OnStop()
	{
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
		this.agent.SetStopped(false);
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000FD258 File Offset: 0x000FB458
	protected override void OnUpdateShared(float dt)
	{
		float abilityTime = (float)(Time.timeAsDouble - this.startTime);
		if (this.anim != null && this.anim[this.animName] != null)
		{
			if ((int)this.anim[this.animName].normalizedTime > this.animLoops)
			{
				this.events.Reset();
				this.animLoops = (int)this.anim[this.animName].normalizedTime;
			}
			abilityTime = this.anim[this.animName].time - this.anim[this.animName].length * (float)this.animLoops;
		}
		this.events.TryPlay(abilityTime, this.audioSource);
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x000FD32C File Offset: 0x000FB52C
	public override bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	// Token: 0x06002E47 RID: 11847 RVA: 0x000FD35A File Offset: 0x000FB55A
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x000FD368 File Offset: 0x000FB568
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x000FD370 File Offset: 0x000FB570
	public void SpeedUp(float mult)
	{
		this.duration = this.cachedDuration / mult;
		this.animSpeed = this.cachedAnimSpeed * mult;
	}

	// Token: 0x04003B56 RID: 15190
	public float duration;

	// Token: 0x04003B57 RID: 15191
	public string animName;

	// Token: 0x04003B58 RID: 15192
	public float animSpeed;

	// Token: 0x04003B59 RID: 15193
	public float coolDown;

	// Token: 0x04003B5A RID: 15194
	public float range;

	// Token: 0x04003B5B RID: 15195
	private float cachedDuration;

	// Token: 0x04003B5C RID: 15196
	private float cachedAnimSpeed;

	// Token: 0x04003B5D RID: 15197
	public GameAbilityEvents events;

	// Token: 0x04003B5E RID: 15198
	[ReadOnly]
	public int animLoops;
}
