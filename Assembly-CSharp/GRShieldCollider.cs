using System;
using UnityEngine;

// Token: 0x020007DC RID: 2012
public class GRShieldCollider : MonoBehaviour
{
	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06003363 RID: 13155 RVA: 0x0011B33A File Offset: 0x0011953A
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06003364 RID: 13156 RVA: 0x0011B342 File Offset: 0x00119542
	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	// Token: 0x06003365 RID: 13157 RVA: 0x0011B34A File Offset: 0x0011954A
	private void Awake()
	{
		this.lastBlockHittableEntityId = GameEntityId.Invalid;
		this.lastBlockHittableTime = 0.0;
	}

	// Token: 0x06003366 RID: 13158 RVA: 0x0011B366 File Offset: 0x00119566
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	// Token: 0x06003367 RID: 13159 RVA: 0x0011B384 File Offset: 0x00119584
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable)
	{
		if (this.shieldTool != null)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble - this.lastBlockHittableTime >= 1.0 || !(hittable.gameEntity.id == this.lastBlockHittableEntityId))
			{
				this.lastBlockHittableEntityId = hittable.gameEntity.id;
				this.lastBlockHittableTime = timeAsDouble;
				this.shieldTool.BlockHittable(enemyPosition, enemyAttackDirection, hittable, this);
			}
		}
	}

	// Token: 0x04004313 RID: 17171
	[SerializeField]
	private float knockbackVelocity = 3f;

	// Token: 0x04004314 RID: 17172
	[SerializeField]
	private GRToolDirectionalShield shieldTool;

	// Token: 0x04004315 RID: 17173
	private const float BLOCK_SAME_HITTABLE_COOLDOWN = 1f;

	// Token: 0x04004316 RID: 17174
	private GameEntityId lastBlockHittableEntityId;

	// Token: 0x04004317 RID: 17175
	private double lastBlockHittableTime;
}
