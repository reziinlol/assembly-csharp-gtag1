using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006D3 RID: 1747
public class GameHitter : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002BE8 RID: 11240 RVA: 0x000ED74E File Offset: 0x000EB94E
	private void Awake()
	{
		this.components = new List<IGameHitter>(1);
		base.GetComponentsInChildren<IGameHitter>(this.components);
		this.attributes = base.GetComponent<GRAttributes>();
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000ED774 File Offset: 0x000EB974
	public void OnEntityInit()
	{
		GRTool component = base.GetComponent<GRTool>();
		if (component != null)
		{
			component.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(component);
		}
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x000ED7AA File Offset: 0x000EB9AA
	private void OnToolUpgraded(GRTool tool)
	{
		if (this.attributes.HasValueForAttribute(GRAttributeType.KnockbackMultiplier))
		{
			this.knockbackMultiplier = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.KnockbackMultiplier);
		}
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x000ED7D0 File Offset: 0x000EB9D0
	public void ApplyHit(GameHitData hitData)
	{
		if (this.hitFx.hitSound != null)
		{
			this.hitFx.hitSound.Play(null);
		}
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Stop();
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHit(hitData);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, 0.2f);
			GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
			if (gamePlayer != null)
			{
				int num = gamePlayer.FindHandIndex(this.gameEntity.id);
				if (num != -1)
				{
					GTPlayer.Instance.TempFreezeHand(GamePlayer.IsLeftHand(num), 0.15f);
				}
			}
		}
		if (GRNoiseEventManager.instance != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(hitData.hitPosition, 1f, 1f);
		}
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x000ED8E4 File Offset: 0x000EBAE4
	public void ApplyHitToPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.hitFx.hitSound.Play(null);
		if (this.hitFx.hitEffect != null)
		{
			this.hitFx.hitEffect.Play();
		}
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnSuccessfulHitPlayer(player, hitPosition);
		}
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x000ED950 File Offset: 0x000EBB50
	private void PlayVibration(float strength, float duration)
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000ED9B0 File Offset: 0x000EBBB0
	private T GetParentEnemy<T>(Collider collider) where T : MonoBehaviour
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return default(T);
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000ED9F8 File Offset: 0x000EBBF8
	public int CalcHitAmount(GameHitType hitType, GameHittable hittable, GameEntity hitByEntity)
	{
		int result = 0;
		if (hitByEntity != null)
		{
			GRAttributes component = hitByEntity.GetComponent<GRAttributes>();
			if (component != null)
			{
				switch (hitType)
				{
				case GameHitType.Club:
					result = component.CalculateFinalValueForAttribute(this.damageAttribute);
					break;
				case GameHitType.Flash:
					result = component.CalculateFinalValueForAttribute(this.flashDamageAttribute);
					break;
				case GameHitType.Shield:
					result = component.CalculateFinalValueForAttribute(this.shieldDamageAttribute);
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000EDA60 File Offset: 0x000EBC60
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.hitOnCollision)
		{
			return;
		}
		float num = this.gameEntity.GetVelocity().sqrMagnitude;
		if (this.gameEntity.lastHeldByActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			return;
		}
		bool flag = false;
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer != null)
		{
			float handSpeed = GamePlayerLocal.instance.GetHandSpeed(gamePlayer.FindHandIndex(this.gameEntity.id));
			num = handSpeed * handSpeed;
		}
		if (num < this.minSwingSpeed * this.minSwingSpeed)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble < this.hitCooldownEnd)
		{
			return;
		}
		Collider collider = collision.collider;
		GameHittable parentEnemy = this.GetParentEnemy<GameHittable>(collider);
		if (parentEnemy != null && parentEnemy.IsColliderValid(collision.collider))
		{
			Vector3 a = parentEnemy.transform.position - base.transform.position;
			a.Normalize();
			if (!flag && gamePlayer != null)
			{
				a = GamePlayerLocal.instance.GetHandVelocity(gamePlayer.FindHandIndex(this.gameEntity.id)).normalized;
			}
			float num2 = Mathf.Sqrt(num);
			num2 = Mathf.Min(num2, this.maxImpulseSpeed);
			a *= num2;
			Vector3 position = parentEnemy.transform.position;
			GameHitData hitData = new GameHitData
			{
				hitTypeId = (int)this.hitType,
				hitEntityId = parentEnemy.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = position,
				hitImpulse = a * this.knockbackMultiplier,
				hitPosition = collision.GetContact(0).point,
				hitAmount = this.CalcHitAmount(this.hitType, parentEnemy, this.gameEntity),
				hittablePoint = parentEnemy.FindHittablePoint(collider)
			};
			if (parentEnemy.IsHitValid(hitData))
			{
				parentEnemy.RequestHit(hitData);
				this.hitCooldownEnd = timeAsDouble + 0.25;
			}
		}
	}

	// Token: 0x04003851 RID: 14417
	public GameEntity gameEntity;

	// Token: 0x04003852 RID: 14418
	public GameHitType hitType;

	// Token: 0x04003853 RID: 14419
	public GRAttributeType damageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x04003854 RID: 14420
	public GRAttributeType flashDamageAttribute = GRAttributeType.FlashDamage;

	// Token: 0x04003855 RID: 14421
	public GRAttributeType shieldDamageAttribute = GRAttributeType.BatonDamage;

	// Token: 0x04003856 RID: 14422
	public float minSwingSpeed = 1.5f;

	// Token: 0x04003857 RID: 14423
	public GameHitFx hitFx;

	// Token: 0x04003858 RID: 14424
	private GRAttributes attributes;

	// Token: 0x04003859 RID: 14425
	public float knockbackMultiplier = 1f;

	// Token: 0x0400385A RID: 14426
	public float maxImpulseSpeed = 4.5f;

	// Token: 0x0400385B RID: 14427
	private List<IGameHitter> components;

	// Token: 0x0400385C RID: 14428
	private double hitCooldownEnd;

	// Token: 0x0400385D RID: 14429
	public bool hitOnCollision = true;
}
