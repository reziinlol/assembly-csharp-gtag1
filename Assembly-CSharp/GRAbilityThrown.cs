using System;
using UnityEngine;

// Token: 0x0200072B RID: 1835
[Serializable]
public class GRAbilityThrown : GRAbilityBase
{
	// Token: 0x06002E9F RID: 11935 RVA: 0x000FE904 File Offset: 0x000FCB04
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000FE929 File Offset: 0x000FCB29
	protected override void OnStart()
	{
		this.agent.SetIsPathing(false, false);
		this.idleAbility.Start();
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000FE943 File Offset: 0x000FCB43
	protected override void OnStop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, false);
	}

	// Token: 0x06002EA2 RID: 11938 RVA: 0x000FE95D File Offset: 0x000FCB5D
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002EA3 RID: 11939 RVA: 0x000FE96A File Offset: 0x000FCB6A
	protected override void OnUpdateAuthority(float dt)
	{
		this.idleAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002EA4 RID: 11940 RVA: 0x000FE978 File Offset: 0x000FCB78
	protected override void OnUpdateRemote(float dt)
	{
		this.idleAbility.UpdateRemote(dt);
	}

	// Token: 0x04003BAF RID: 15279
	public GRAbilityIdle idleAbility;
}
