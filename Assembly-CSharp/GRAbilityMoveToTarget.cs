using System;
using UnityEngine;

// Token: 0x02000724 RID: 1828
[Serializable]
public class GRAbilityMoveToTarget : GRAbilityBase
{
	// Token: 0x06002E63 RID: 11875 RVA: 0x000FD998 File Offset: 0x000FBB98
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		this.targetPos = agent.transform.position;
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000FD9C4 File Offset: 0x000FBBC4
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		if (this.attributes && this.moveSpeed == 0f)
		{
			this.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.agent.navAgent.speed = this.moveSpeed;
		this.targetPos = this.agent.transform.position;
		this.movementSound.Play(null);
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000FDA4D File Offset: 0x000FBC4D
	protected override void OnStop()
	{
		this.movementSound.Stop();
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x000FDA5C File Offset: 0x000FBC5C
	public override bool IsDone()
	{
		return (this.targetPos - this.root.position).sqrMagnitude < 0.25f;
	}

	// Token: 0x06002E67 RID: 11879 RVA: 0x000FDA90 File Offset: 0x000FBC90
	protected override void OnUpdateShared(float dt)
	{
		if (this.target != null)
		{
			this.targetPos = this.target.position;
			this.agent.RequestDestination(this.targetPos);
		}
		Transform transform = (this.lookAtTarget != null) ? this.lookAtTarget : this.target;
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, transform, this.maxTurnSpeed);
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x000FDB07 File Offset: 0x000FBD07
	public void SetTarget(Transform transform)
	{
		this.target = transform;
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x000FDB10 File Offset: 0x000FBD10
	public void SetTargetPos(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		this.agent.RequestDestination(targetPos);
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000FDB25 File Offset: 0x000FBD25
	public Vector3 GetTargetPos()
	{
		return this.targetPos;
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x000FDB2D File Offset: 0x000FBD2D
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x04003B7B RID: 15227
	public float moveSpeed;

	// Token: 0x04003B7C RID: 15228
	public string animName;

	// Token: 0x04003B7D RID: 15229
	public float animSpeed = 1f;

	// Token: 0x04003B7E RID: 15230
	public float maxTurnSpeed = 360f;

	// Token: 0x04003B7F RID: 15231
	public AbilitySound movementSound;

	// Token: 0x04003B80 RID: 15232
	private Vector3 targetPos;

	// Token: 0x04003B81 RID: 15233
	private Transform target;

	// Token: 0x04003B82 RID: 15234
	private Transform lookAtTarget;
}
