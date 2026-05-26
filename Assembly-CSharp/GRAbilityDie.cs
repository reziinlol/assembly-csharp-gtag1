using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x02000728 RID: 1832
[Serializable]
public class GRAbilityDie : GRAbilityBase
{
	// Token: 0x06002E81 RID: 11905 RVA: 0x000FDFCC File Offset: 0x000FC1CC
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.disableAllCollidersWhenDead)
		{
			agent.GetComponentsInChildren<Collider>(this.disableCollidersWhenDead);
		}
		if (this.disableAllRenderersWhenDead)
		{
			agent.GetComponentsInChildren<Renderer>(this.hideWhenDead);
		}
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.staggerMovement.Setup(root);
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000FE02C File Offset: 0x000FC22C
	protected override void OnStart()
	{
		this.totalDeathDelay = this.delayDeath;
		if (this.animData.Count > 0)
		{
			int index = Random.Range(0, this.animData.Count);
			this.totalDeathDelay += this.animData[index].duration;
			this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.totalDeathDelay);
			this.PlayAnim(this.animData[index].animName, 0.1f, this.animData[index].speed);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.isDead = false;
		if (this.doKnockback)
		{
			this.staggerMovement.Start();
		}
		this.soundDeath.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundOnHide.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundDeath.Play(null);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000FE170 File Offset: 0x000FC370
	protected override void OnStop()
	{
		this.staggerMovement.Stop();
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		GRAbilityDie.Hide(this.hideWhenDead, false);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	// Token: 0x06002E84 RID: 11908 RVA: 0x000FE1D8 File Offset: 0x000FC3D8
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 a = vel / magnitude;
			a.y = 0f;
			vel = a * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.totalDeathDelay);
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x000FE224 File Offset: 0x000FC424
	public void SetInstigatingPlayerIndex(int actorNumber)
	{
		Debug.Log(string.Format("SetInstigatingPlayerIndex {0}", actorNumber));
		this.instigatingActorNumber = actorNumber;
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x000FE244 File Offset: 0x000FC444
	private void Die()
	{
		this.soundOnHide.Play(null);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
			this.fxDeath.SetActive(true);
		}
		GRAbilityDie.Hide(this.hideWhenDead, true);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		GameEntity entity = this.agent.entity;
		GameEntity gameEntity;
		if (this.lootTable != null && entity.IsAuthority() && this.lootTable.TryForRandomItem(entity, out gameEntity, 0))
		{
			Transform transform = this.lootSpawnMarker;
			if (transform == null)
			{
				transform = this.agent.transform;
			}
			Vector3 vector = transform.position;
			if (transform == null)
			{
				vector.y += 0.33f;
			}
			RaycastHit raycastHit;
			if (this.spawnOnGround && Physics.Raycast(new Ray(vector + Vector3.up * 0.5f, -Vector3.up), out raycastHit, 5f, this.groundLayerMask.value, QueryTriggerInteraction.Ignore))
			{
				vector = raycastHit.point;
			}
			entity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), vector, transform.rotation, 0L);
		}
		GREnemy component = entity.GetComponent<GREnemy>();
		if (component != null && component.damageFlash != null)
		{
			component.damageFlash.Play();
		}
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000FE3B0 File Offset: 0x000FC5B0
	public void DestroySelf()
	{
		Debug.Log("DESTROY SELF");
		this.ReportDeathStat();
		if (this.agent.entity.IsAuthority())
		{
			this.agent.entity.manager.RequestDestroyItem(this.agent.entity.id);
		}
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x000FE404 File Offset: 0x000FC604
	public void ReportDeathStat()
	{
		if (this.reported)
		{
			return;
		}
		this.reported = true;
		GameEntity entity = this.agent.entity;
		GRPlayer grplayer = GRPlayer.Get(this.instigatingActorNumber);
		if (grplayer != null)
		{
			grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Kills, 1f);
		}
		GhostReactor.instance.shiftManager.shiftStats.IncrementEnemyKills(entity.GetEnemyType());
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x000FE468 File Offset: 0x000FC668
	protected override void OnUpdateShared(float dt)
	{
		if (this.startTime >= 0.0)
		{
			if (this.doKnockback)
			{
				this.staggerMovement.Update(dt);
			}
			double num = Time.timeAsDouble - this.startTime;
			if (!this.isDead && num > (double)this.totalDeathDelay)
			{
				this.isDead = true;
				this.Die();
			}
			else if (this.isDead && num > (double)(this.totalDeathDelay + this.destroyDelay))
			{
				GhostReactorManager.Get(this.entity).OnAbilityDie(this.entity, this.delayRespawn);
				this.DestroySelf();
				this.startTime = -1.0;
			}
			this.events.TryPlay((float)num, this.audioSource);
		}
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x000FE528 File Offset: 0x000FC728
	public static void Hide(List<Renderer> renderers, bool hide)
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

	// Token: 0x06002E8C RID: 11916 RVA: 0x000FE56C File Offset: 0x000FC76C
	public static void Disable(List<Collider> colliders, bool disable)
	{
		if (colliders == null)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].enabled = !disable;
			}
		}
	}

	// Token: 0x04003B95 RID: 15253
	public float delayDeath;

	// Token: 0x04003B96 RID: 15254
	public float delayRespawn = -1f;

	// Token: 0x04003B97 RID: 15255
	public List<Renderer> hideWhenDead;

	// Token: 0x04003B98 RID: 15256
	public List<Collider> disableCollidersWhenDead;

	// Token: 0x04003B99 RID: 15257
	public bool disableAllCollidersWhenDead;

	// Token: 0x04003B9A RID: 15258
	public bool disableAllRenderersWhenDead;

	// Token: 0x04003B9B RID: 15259
	public GameObject fxDeath;

	// Token: 0x04003B9C RID: 15260
	public AbilitySound soundDeath;

	// Token: 0x04003B9D RID: 15261
	public AbilitySound soundOnHide;

	// Token: 0x04003B9E RID: 15262
	public float destroyDelay = 3f;

	// Token: 0x04003B9F RID: 15263
	public bool doKnockback = true;

	// Token: 0x04003BA0 RID: 15264
	public GRBreakableItemSpawnConfig lootTable;

	// Token: 0x04003BA1 RID: 15265
	public bool spawnOnGround;

	// Token: 0x04003BA2 RID: 15266
	public LayerMask groundLayerMask;

	// Token: 0x04003BA3 RID: 15267
	public Transform lootSpawnMarker;

	// Token: 0x04003BA4 RID: 15268
	public List<AnimationData> animData;

	// Token: 0x04003BA5 RID: 15269
	private int instigatingActorNumber;

	// Token: 0x04003BA6 RID: 15270
	private bool isDead;

	// Token: 0x04003BA7 RID: 15271
	private float totalDeathDelay;

	// Token: 0x04003BA8 RID: 15272
	public GRAbilityInterpolatedMovement staggerMovement;

	// Token: 0x04003BA9 RID: 15273
	public GameAbilityEvents events;

	// Token: 0x04003BAA RID: 15274
	private bool reported;
}
