using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A4F RID: 2639
public class CustomMapsSearchBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06004390 RID: 17296 RVA: 0x0016AB90 File Offset: 0x00168D90
	public CustomMapsSearchBehaviour(CustomMapsAIBehaviourController AIcontroller, AIAgent agentSettings)
	{
		this.sightOffset = agentSettings.sightOffset;
		this.sightDist = agentSettings.sightDist;
		this.sightDistSq = this.sightDist * this.sightDist;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIcontroller;
	}

	// Token: 0x06004391 RID: 17297 RVA: 0x0016ABFE File Offset: 0x00168DFE
	public override bool CanExecute()
	{
		return !this.controller.IsNull();
	}

	// Token: 0x06004392 RID: 17298 RVA: 0x0016AC10 File Offset: 0x00168E10
	public override bool CanContinueExecuting()
	{
		return this.CanExecute() && this.controller.TargetPlayer == null;
	}

	// Token: 0x06004393 RID: 17299 RVA: 0x0016AC30 File Offset: 0x00168E30
	public override void Execute()
	{
		if (Time.time < this.lastSearchTime + 0.1f)
		{
			return;
		}
		this.lastSearchTime = Time.time;
		Vector3 sourcePos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		this.controller.SetTarget(this.controller.FindBestTarget(sourcePos, this.sightDist, this.sightDistSq, this.sightMinDot));
	}

	// Token: 0x06004394 RID: 17300 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void NetExecute()
	{
	}

	// Token: 0x06004395 RID: 17301 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void ResetBehavior()
	{
	}

	// Token: 0x06004396 RID: 17302 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnTriggerEnter(Collider otherCollider)
	{
	}

	// Token: 0x040055A0 RID: 21920
	private const float SEARCH_COOLDOWN = 0.1f;

	// Token: 0x040055A1 RID: 21921
	private CustomMapsAIBehaviourController controller;

	// Token: 0x040055A2 RID: 21922
	private float sightDist;

	// Token: 0x040055A3 RID: 21923
	private float sightDistSq;

	// Token: 0x040055A4 RID: 21924
	private Vector3 sightOffset;

	// Token: 0x040055A5 RID: 21925
	private float sightFOV;

	// Token: 0x040055A6 RID: 21926
	private float sightMinDot;

	// Token: 0x040055A7 RID: 21927
	private float lastSearchTime;
}
