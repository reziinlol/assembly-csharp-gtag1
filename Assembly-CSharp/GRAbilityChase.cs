using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000725 RID: 1829
[Serializable]
public class GRAbilityChase : GRAbilityBase
{
	// Token: 0x06002E6D RID: 11885 RVA: 0x000FDB54 File Offset: 0x000FBD54
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.targetPlayer = null;
		this.lastSeenTargetTime = 0.0;
		this.lastSeenTargetPosition = Vector3.zero;
		if (GRAbilityChase.targetOffsets == null)
		{
			int num = 8;
			GRAbilityChase.targetOffsets = new List<Vector3>(num);
			float x2 = 1f;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = new Vector3(x2, 0f, 0f);
				vector = Quaternion.Euler(0f, (float)i / (float)num * 360f, 0f) * vector;
				GRAbilityChase.targetOffsets.Add(vector);
			}
			Random random = new Random();
			List<Vector3> collection = (from x in GRAbilityChase.targetOffsets
			orderby random.Next()
			select x).ToList<Vector3>();
			GRAbilityChase.targetOffsets.Clear();
			GRAbilityChase.targetOffsets.AddRange(collection);
		}
		if (this.attributes && this.chaseSpeed == 0f)
		{
			this.chaseSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
		}
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x000FDC70 File Offset: 0x000FBE70
	protected override void OnStart()
	{
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetSpeed(this.chaseSpeed);
		this.lastSeenTargetTime = Time.timeAsDouble;
		this.movementSound.Play(null);
		this.agent.ClearLastRequestedDestination();
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnStop()
	{
	}

	// Token: 0x06002E70 RID: 11888 RVA: 0x000FDCC7 File Offset: 0x000FBEC7
	public override bool IsDone()
	{
		return this.targetPlayer == null || Time.timeAsDouble - this.lastSeenTargetTime >= (double)this.giveUpDelay;
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x000FDCEC File Offset: 0x000FBEEC
	protected override void OnThink(float dt)
	{
		GRPlayer grplayer = GRPlayer.Get(this.targetPlayer);
		if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
		{
			Vector3 vector = grplayer.transform.position;
			vector += GRAbilityChase.GetMoveTargetOffset(vector, this.entity);
			if (this.lineOfSight.HasLineOfSight(this.head.position, vector))
			{
				this.lastSeenTargetTime = Time.timeAsDouble;
			}
			if ((float)(Time.timeAsDouble - this.lastSeenTargetTime) <= this.loseVisibilityDelay)
			{
				this.lastSeenTargetPosition = vector;
			}
		}
		this.agent.RequestDestination(this.lastSeenTargetPosition);
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x000FDD88 File Offset: 0x000FBF88
	protected override void OnUpdateShared(float dt)
	{
		GameAgent.UpdateFacing(this.root, this.agent.navAgent, this.targetPlayer, this.maxTurnSpeed);
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x000FDDAC File Offset: 0x000FBFAC
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.targetPlayer = targetPlayer;
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000FDDB8 File Offset: 0x000FBFB8
	public static Vector3 GetMoveTargetOffset(Vector3 targetPos, GameEntity attackingEntity)
	{
		int index = attackingEntity.id.index % GRAbilityChase.targetOffsets.Count;
		return GRAbilityChase.targetOffsets[index];
	}

	// Token: 0x04003B83 RID: 15235
	public float chaseSpeed;

	// Token: 0x04003B84 RID: 15236
	public string animName;

	// Token: 0x04003B85 RID: 15237
	public float animSpeed;

	// Token: 0x04003B86 RID: 15238
	public float maxTurnSpeed;

	// Token: 0x04003B87 RID: 15239
	public float loseVisibilityDelay;

	// Token: 0x04003B88 RID: 15240
	public float giveUpDelay;

	// Token: 0x04003B89 RID: 15241
	public AbilitySound movementSound;

	// Token: 0x04003B8A RID: 15242
	private NetPlayer targetPlayer;

	// Token: 0x04003B8B RID: 15243
	private double lastSeenTargetTime;

	// Token: 0x04003B8C RID: 15244
	private Vector3 lastSeenTargetPosition;

	// Token: 0x04003B8D RID: 15245
	private static List<Vector3> targetOffsets;
}
