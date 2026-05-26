using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x02000774 RID: 1908
public class GREnemy : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x06003047 RID: 12359 RVA: 0x00106687 File Offset: 0x00104887
	private void Awake()
	{
		this.damageFlash.Setup();
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x00106694 File Offset: 0x00104894
	public void OnEntityInit()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x00106694 File Offset: 0x00104894
	public void OnEntityDestroy()
	{
		if (this.gameEntity != null)
		{
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnTick = (Action)Delegate.Combine(gameEntity.OnTick, new Action(this.OnUpdate));
		}
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x001066CC File Offset: 0x001048CC
	public static void HideRenderers(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x00106710 File Offset: 0x00104910
	public static void HideObjects(List<GameObject> objects, bool hide)
	{
		if (objects == null)
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null)
			{
				objects[i].SetActive(!hide);
			}
		}
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x00106751 File Offset: 0x00104951
	public void OnUpdate()
	{
		this.damageFlash.Update();
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x0010675E File Offset: 0x0010495E
	public void SetMaxHP(int maxHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.Setup(maxHp);
		}
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x0010677A File Offset: 0x0010497A
	public void SetHP(int newHp)
	{
		if (this.healthMeter != null)
		{
			this.healthMeter.SetHP(newHp);
		}
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x00106796 File Offset: 0x00104996
	public void OnHit(GameHitData hit)
	{
		if (hit.hitAmount > 0)
		{
			this.damageFlash.Play();
		}
	}

	// Token: 0x04003DD9 RID: 15833
	public GRHealthMeter healthMeter;

	// Token: 0x04003DDA RID: 15834
	public GREnemyType enemyType;

	// Token: 0x04003DDB RID: 15835
	public GameEntity gameEntity;

	// Token: 0x04003DDC RID: 15836
	public GRDamageFlash damageFlash;
}
