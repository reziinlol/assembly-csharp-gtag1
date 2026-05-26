using System;
using UnityEngine;

// Token: 0x0200072A RID: 1834
[Serializable]
public class GRAbilityGrabbed : GRAbilityBase
{
	// Token: 0x06002E98 RID: 11928 RVA: 0x000FE882 File Offset: 0x000FCA82
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000FE8A7 File Offset: 0x000FCAA7
	protected override void OnStart()
	{
		this.agent.SetIsPathing(false, true);
		this.idleAbility.Start();
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000FE8C1 File Offset: 0x000FCAC1
	protected override void OnStop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, true);
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000FE8DB File Offset: 0x000FCADB
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000FE8E8 File Offset: 0x000FCAE8
	protected override void OnUpdateAuthority(float dt)
	{
		this.idleAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000FE8F6 File Offset: 0x000FCAF6
	protected override void OnUpdateRemote(float dt)
	{
		this.idleAbility.UpdateRemote(dt);
	}

	// Token: 0x04003BAE RID: 15278
	public GRAbilityIdle idleAbility;
}
