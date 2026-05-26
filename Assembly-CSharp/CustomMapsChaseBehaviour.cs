using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A4E RID: 2638
public class CustomMapsChaseBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06004387 RID: 17287 RVA: 0x0016A944 File Offset: 0x00168B44
	public CustomMapsChaseBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.rememberLoseSightPos = agentSettings.rememberLoseSightPosition;
		this.loseSightDist = agentSettings.loseSightDist;
		this.loseSightDistSq = this.loseSightDist * this.loseSightDist;
		this.stopDistSq = agentSettings.stopDist * agentSettings.stopDist;
		this.controller = AIController;
	}

	// Token: 0x06004388 RID: 17288 RVA: 0x0016A9A8 File Offset: 0x00168BA8
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull();
	}

	// Token: 0x06004389 RID: 17289 RVA: 0x0016A9D0 File Offset: 0x00168BD0
	public override bool CanContinueExecuting()
	{
		if (!this.CanExecute())
		{
			return false;
		}
		bool flag;
		if (this.IsTargetInChaseRange(out flag))
		{
			return !flag;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.StopMoving();
		}
		this.controller.ClearTarget();
		return false;
	}

	// Token: 0x0600438A RID: 17290 RVA: 0x0016AA28 File Offset: 0x00168C28
	public override void Execute()
	{
		bool flag;
		if (!this.IsTargetInChaseRange(out flag))
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (!this.IsTargetVisible())
		{
			this.controller.ClearTarget();
			this.isChasing = false;
			if (!this.rememberLoseSightPos)
			{
				this.controller.StopMoving();
			}
			return;
		}
		if (flag && this.isChasing)
		{
			this.isChasing = false;
			this.controller.StopMoving();
			return;
		}
		this.isChasing = true;
		this.controller.RequestDestination(this.controller.TargetPlayer.transform.position);
	}

	// Token: 0x0600438B RID: 17291 RVA: 0x0016AAD8 File Offset: 0x00168CD8
	private bool IsTargetVisible()
	{
		Vector3 startPos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(startPos, this.controller.TargetPlayer, this.loseSightDist);
	}

	// Token: 0x0600438C RID: 17292 RVA: 0x0016AB30 File Offset: 0x00168D30
	private bool IsTargetInChaseRange(out bool withinStopDist)
	{
		withinStopDist = false;
		Vector3 vector;
		if (!this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.loseSightDistSq, out vector))
		{
			return false;
		}
		if (vector.sqrMagnitude < this.stopDistSq)
		{
			withinStopDist = true;
		}
		return true;
	}

	// Token: 0x0600438D RID: 17293 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void NetExecute()
	{
	}

	// Token: 0x0600438E RID: 17294 RVA: 0x0016AB85 File Offset: 0x00168D85
	public override void ResetBehavior()
	{
		this.isChasing = false;
	}

	// Token: 0x0600438F RID: 17295 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x04005598 RID: 21912
	private NavMeshAgent navMeshAgent;

	// Token: 0x04005599 RID: 21913
	private CustomMapsAIBehaviourController controller;

	// Token: 0x0400559A RID: 21914
	private float loseSightDist;

	// Token: 0x0400559B RID: 21915
	private float loseSightDistSq;

	// Token: 0x0400559C RID: 21916
	private Vector3 sightOffset;

	// Token: 0x0400559D RID: 21917
	private bool rememberLoseSightPos;

	// Token: 0x0400559E RID: 21918
	private float stopDistSq;

	// Token: 0x0400559F RID: 21919
	private bool isChasing;
}
