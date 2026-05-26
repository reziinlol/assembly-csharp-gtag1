using System;
using UnityEngine;

// Token: 0x0200073C RID: 1852
[Serializable]
public class GRAbilityAttackSimpleWander : GRAbilityBase
{
	// Token: 0x06002F06 RID: 12038 RVA: 0x00100AFB File Offset: 0x000FECFB
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.wander.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.attack.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x00100B34 File Offset: 0x000FED34
	protected override void OnStart()
	{
		this.wander.Start();
		this.attack.Start();
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x00100B4C File Offset: 0x000FED4C
	protected override void OnStop()
	{
		this.wander.Stop();
		this.attack.Stop();
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x00100B64 File Offset: 0x000FED64
	protected override void OnThink(float dt)
	{
		this.wander.Think(dt);
		this.attack.Think(dt);
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x00100B7E File Offset: 0x000FED7E
	protected override void OnUpdateAuthority(float dt)
	{
		this.wander.UpdateAuthority(dt);
		this.attack.UpdateAuthority(dt);
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x00100B98 File Offset: 0x000FED98
	protected override void OnUpdateRemote(float dt)
	{
		this.wander.UpdateRemote(dt);
		this.attack.UpdateRemote(dt);
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x00100BB2 File Offset: 0x000FEDB2
	public override bool IsDone()
	{
		return this.attack.IsDone();
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x00100BBF File Offset: 0x000FEDBF
	public override bool IsCoolDownOver()
	{
		return this.attack.IsCoolDownOver();
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x00100BCC File Offset: 0x000FEDCC
	public override float GetRange()
	{
		return this.attack.GetRange();
	}

	// Token: 0x04003C4C RID: 15436
	public GRAbilityWander wander;

	// Token: 0x04003C4D RID: 15437
	public GRAbilityAttackSimple attack;
}
