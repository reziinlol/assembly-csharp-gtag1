using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class GameHittable : MonoBehaviour
{
	// Token: 0x06002BDC RID: 11228 RVA: 0x000ED478 File Offset: 0x000EB678
	private void Awake()
	{
		this.components = new List<IGameHittable>(1);
		base.GetComponentsInChildren<IGameHittable>(this.components);
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Setup();
		}
	}

	// Token: 0x06002BDD RID: 11229 RVA: 0x000ED4C9 File Offset: 0x000EB6C9
	private void OnEnable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x000ED500 File Offset: 0x000EB700
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Remove(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x000ED538 File Offset: 0x000EB738
	public void OnUpdate()
	{
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			this.hittablePoints[i].damageFlash.Update();
		}
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x000ED571 File Offset: 0x000EB771
	public void RequestHit(GameHitData hitData)
	{
		hitData.hitEntityId = this.gameEntity.id;
		this.gameEntity.manager.RequestHit(hitData);
	}

	// Token: 0x06002BE1 RID: 11233 RVA: 0x000ED598 File Offset: 0x000EB798
	public void ApplyHit(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnHit(hitData);
		}
		GameHitter component = this.gameEntity.manager.GetGameEntity(hitData.hitByEntityId).GetComponent<GameHitter>();
		if (component != null)
		{
			component.ApplyHit(hitData);
		}
		GameHittable.HittablePoint hittablePoint = this.GetHittablePoint(hitData.hittablePoint);
		if (hittablePoint != null)
		{
			hittablePoint.damageFlash.Play();
		}
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000ED614 File Offset: 0x000EB814
	private GameHittable.HittablePoint GetHittablePoint(int hittablePoint)
	{
		if (hittablePoint < 0 || hittablePoint >= this.hittablePoints.Count)
		{
			return null;
		}
		return this.hittablePoints[hittablePoint];
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x000ED638 File Offset: 0x000EB838
	public bool IsHitValid(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			if (!this.components[i].IsHitValid(hitData))
			{
				return false;
			}
		}
		return this.hittablePoints.Count <= 0 || (hitData.hittablePoint >= 0 && hitData.hittablePoint < this.hittablePoints.Count);
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x000ED6A0 File Offset: 0x000EB8A0
	public int FindHittablePoint(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return 0;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x000ED6F8 File Offset: 0x000EB8F8
	public bool IsColliderValid(Collider collider)
	{
		if (this.hittablePoints == null || this.hittablePoints.Count == 0)
		{
			return true;
		}
		for (int i = 0; i < this.hittablePoints.Count; i++)
		{
			if (this.hittablePoints[i].colliders.Contains(collider))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400384A RID: 14410
	public GameEntity gameEntity;

	// Token: 0x0400384B RID: 14411
	public List<GameHittable.HittablePoint> hittablePoints;

	// Token: 0x0400384C RID: 14412
	private List<IGameHittable> components;

	// Token: 0x020006D1 RID: 1745
	[Serializable]
	public class HittablePoint
	{
		// Token: 0x0400384D RID: 14413
		public List<Collider> colliders;

		// Token: 0x0400384E RID: 14414
		public GRDamageFlash damageFlash;
	}
}
