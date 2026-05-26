using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000729 RID: 1833
[Serializable]
public class GRAbilityWander : GRAbilityBase
{
	// Token: 0x06002E8E RID: 11918 RVA: 0x000FE5D2 File Offset: 0x000FC7D2
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x000FE5F8 File Offset: 0x000FC7F8
	protected override void OnStart()
	{
		this.moveAbility.Start();
		Vector3 targetPos = this.PickRandomDestination();
		this.moveAbility.SetTargetPos(targetPos);
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x000FE623 File Offset: 0x000FC823
	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002E92 RID: 11922 RVA: 0x000FE630 File Offset: 0x000FC830
	protected override void OnThink(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 targetPos = this.PickRandomDestination();
			this.moveAbility.SetTargetPos(targetPos);
		}
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x000FE660 File Offset: 0x000FC860
	private Vector3 PickRandomDestination()
	{
		Vector3 position = this.agent.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 forward = this.agent.transform.forward;
			float num = 0f;
			for (int i = 0; i < GRAbilityWander.rotations.Length; i++)
			{
				Vector3 a = GRAbilityWander.rotations[i] * forward;
				float num2 = 8f;
				if (NavMesh.Raycast(position2, position2 + a * num2, out navMeshHit, this.walkableArea))
				{
					num2 = navMeshHit.distance * 0.95f;
				}
				float num3 = num2 * GRAbilityWander.rotationWeight[i];
				if (num3 > num && NavMesh.SamplePosition(position2 + a * num2, out navMeshHit, 1f, this.walkableArea))
				{
					num = num3;
					position = navMeshHit.position;
				}
			}
		}
		return position;
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x000FE75E File Offset: 0x000FC95E
	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000FE76C File Offset: 0x000FC96C
	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	// Token: 0x04003BAB RID: 15275
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x04003BAC RID: 15276
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, -45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};

	// Token: 0x04003BAD RID: 15277
	private static float[] rotationWeight = new float[]
	{
		1f,
		0.75f,
		0.75f,
		0.5f,
		0.5f,
		0.2f,
		0.2f,
		0.2f
	};
}
