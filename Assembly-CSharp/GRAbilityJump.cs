using System;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000722 RID: 1826
[Serializable]
public class GRAbilityJump : GRAbilityBase
{
	// Token: 0x06002E52 RID: 11858 RVA: 0x000FD56A File Offset: 0x000FB76A
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.isActive = false;
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x000FD584 File Offset: 0x000FB784
	public void SetupJump(Vector3 start, Vector3 end, float heightScale = 1f, float speedScale = 1f)
	{
		this.elapsedTime = 0f;
		this.startPos = start;
		this.endPos = end;
		float magnitude = (this.endPos - this.startPos).magnitude;
		this.controlPoint = (this.startPos + this.endPos) / 2f + new Vector3(0f, magnitude * heightScale, 0f);
		this.jumpTime = magnitude / (this.jumpSpeed * speedScale);
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x000FD610 File Offset: 0x000FB810
	public void SetupJumpFromLinkData(OffMeshLinkData linkData)
	{
		if ((this.root.position - linkData.startPos).sqrMagnitude < (this.root.position - linkData.endPos).sqrMagnitude)
		{
			this.SetupJump(linkData.startPos, linkData.endPos, 1f, 1f);
			return;
		}
		this.SetupJump(linkData.endPos, linkData.startPos, 1f, 1f);
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x000FD69C File Offset: 0x000FB89C
	protected override void OnStart()
	{
		this.elapsedTime = 0f;
		this.isActive = true;
		this.PlayAnim(this.animationData.animName, 0.05f, this.animationData.speed);
		this.agent.SetStopped(true);
		this.agent.SetDisableNetworkSync(true);
		this.agent.pauseEntityThink = true;
		this.soundJump.Play(this.audioSource);
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x000FD714 File Offset: 0x000FB914
	protected override void OnStop()
	{
		this.agent.navAgent.Warp(this.endPos);
		this.agent.navAgent.CompleteOffMeshLink();
		this.agent.SetStopped(false);
		this.isActive = false;
		this.agent.SetDisableNetworkSync(false);
		this.agent.pauseEntityThink = false;
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x000FD773 File Offset: 0x000FB973
	public override bool IsDone()
	{
		return this.elapsedTime >= this.jumpTime;
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000FD786 File Offset: 0x000FB986
	public bool IsActive()
	{
		return this.isActive;
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000FD790 File Offset: 0x000FB990
	protected override void OnUpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.startPos, this.controlPoint, Color.green, true);
			DebugUtil.DrawLine(this.endPos, this.controlPoint, Color.green, true);
		}
		float t = (this.jumpTime > 0f) ? Math.Clamp(this.elapsedTime / this.jumpTime, 0f, 1f) : 1f;
		Vector3 position = GRAbilityJump.EvaluateQuadratic(this.startPos, this.controlPoint, this.endPos, t);
		this.root.position = position;
		if (this.rb != null)
		{
			this.rb.position = position;
		}
		this.elapsedTime += dt;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000FD850 File Offset: 0x000FBA50
	public static Vector3 EvaluateQuadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		Vector3 a = Vector3.Lerp(p0, p1, t);
		Vector3 b = Vector3.Lerp(p1, p2, t);
		return Vector3.Lerp(a, b, t);
	}

	// Token: 0x04003B6C RID: 15212
	private Vector3 startPos;

	// Token: 0x04003B6D RID: 15213
	private Vector3 endPos;

	// Token: 0x04003B6E RID: 15214
	private Vector3 controlPoint;

	// Token: 0x04003B6F RID: 15215
	[ReadOnly]
	public float jumpTime;

	// Token: 0x04003B70 RID: 15216
	[ReadOnly]
	public float elapsedTime;

	// Token: 0x04003B71 RID: 15217
	private bool isActive;

	// Token: 0x04003B72 RID: 15218
	public AnimationData animationData;

	// Token: 0x04003B73 RID: 15219
	public float jumpSpeed = 3f;

	// Token: 0x04003B74 RID: 15220
	public AbilitySound soundJump;
}
