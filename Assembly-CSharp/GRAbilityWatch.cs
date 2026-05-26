using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000723 RID: 1827
[Serializable]
public class GRAbilityWatch : GRAbilityBase
{
	// Token: 0x06002E5C RID: 11868 RVA: 0x000FD888 File Offset: 0x000FBA88
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x000FD8A0 File Offset: 0x000FBAA0
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.endTime = -1.0;
		if (this.duration > 0f)
		{
			this.endTime = Time.timeAsDouble + (double)this.duration;
		}
		this.agent.SetStopped(true);
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x000FD8FF File Offset: 0x000FBAFF
	protected override void OnStop()
	{
		this.agent.SetStopped(false);
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000FD90D File Offset: 0x000FBB0D
	public override bool IsDone()
	{
		return this.endTime > 0.0 && Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000FD932 File Offset: 0x000FBB32
	protected override void OnUpdateShared(float dt)
	{
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x000FD958 File Offset: 0x000FBB58
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

	// Token: 0x04003B75 RID: 15221
	public float duration;

	// Token: 0x04003B76 RID: 15222
	public string animName;

	// Token: 0x04003B77 RID: 15223
	public float animSpeed;

	// Token: 0x04003B78 RID: 15224
	public float maxTurnSpeed;

	// Token: 0x04003B79 RID: 15225
	private Transform target;

	// Token: 0x04003B7A RID: 15226
	[ReadOnly]
	public double endTime;
}
