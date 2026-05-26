using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020007F7 RID: 2039
public class GRToolDirectionalShield : MonoBehaviour, IGameHitter
{
	// Token: 0x0600341A RID: 13338 RVA: 0x0011EBC0 File Offset: 0x0011CDC0
	private void Awake()
	{
		this.hitter = base.GetComponent<GameHitter>();
		this.attributes = base.GetComponent<GRAttributes>();
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x0600341B RID: 13339 RVA: 0x0011EC18 File Offset: 0x0011CE18
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize1))
		{
			this.deflectAudio = this.upgrade1DeflectAudio;
			this.shieldDeflectVFX = this.upgrade1ShieldDeflectVFX;
			this.reflectsProjectiles = true;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize2))
		{
			this.deflectAudio = this.upgrade2DeflectAudio;
			this.shieldDeflectVFX = this.upgrade2ShieldDeflectVFX;
			this.reflectsProjectiles = false;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize3))
		{
			this.deflectAudio = this.upgrade3DeflectAudio;
			this.shieldDeflectVFX = this.upgrade3ShieldDeflectVFX;
			this.reflectsProjectiles = true;
			return;
		}
		this.reflectsProjectiles = false;
	}

	// Token: 0x0600341C RID: 13340 RVA: 0x0011ECAA File Offset: 0x0011CEAA
	public void OnEnable()
	{
		this.SetState(GRToolDirectionalShield.State.Closed);
	}

	// Token: 0x0600341D RID: 13341 RVA: 0x0011ECB3 File Offset: 0x0011CEB3
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x0600341E RID: 13342 RVA: 0x0011ECCC File Offset: 0x0011CECC
	private bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x0011ECE0 File Offset: 0x0011CEE0
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable, GRShieldCollider shieldCollider)
	{
		if (this.IsHeldLocal())
		{
			float d = 1f;
			if (this.attributes != null && this.attributes.HasValueForAttribute(GRAttributeType.KnockbackMultiplier))
			{
				d = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.KnockbackMultiplier);
			}
			Vector3 hitImpulse = -enemyAttackDirection * shieldCollider.KnockbackVelocity * d;
			if (this.reflectsProjectiles)
			{
				GRRangedEnemyProjectile component = hittable.GetComponent<GRRangedEnemyProjectile>();
				Vector3 a;
				if (component != null && component.owningEntity != null && GREnemyRanged.CalculateLaunchDirection(enemyPosition, component.owningEntity.transform.position + new Vector3(0f, 0.5f, 0f), component.projectileSpeed, out a))
				{
					hitImpulse = a * component.projectileSpeed;
				}
			}
			GameHitData hitData = new GameHitData
			{
				hitTypeId = 2,
				hitEntityId = hittable.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = enemyPosition,
				hitImpulse = hitImpulse,
				hitPosition = enemyPosition,
				hitAmount = this.hitter.CalcHitAmount(GameHitType.Shield, hittable, this.gameEntity)
			};
			if (hittable.IsHitValid(hitData))
			{
				hittable.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x0011EE28 File Offset: 0x0011D028
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(enemyPosition);
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x0011EE3C File Offset: 0x0011D03C
	private void PlayBlockEffects(Vector3 enemyPosition)
	{
		this.audioSource.PlayOneShot(this.deflectAudio, this.deflectVolume);
		this.shieldDeflectVFX.Play();
		Vector3 b = Vector3.ClampMagnitude(enemyPosition - this.shieldArcCenterReferencePoint.position, this.shieldArcCenterRadius);
		Vector3 position = this.shieldArcCenterReferencePoint.position + b;
		this.shieldDeflectImpactPointVFX.transform.position = position;
		this.shieldDeflectImpactPointVFX.Play();
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x0011EEB6 File Offset: 0x0011D0B6
	public void OnSuccessfulHit(GameHitData hitData)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(hitData.hitEntityPosition);
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0011EED0 File Offset: 0x0011D0D0
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (!this.IsHeld())
		{
			this.SetState(GRToolDirectionalShield.State.Closed);
			return;
		}
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x0011EF0C File Offset: 0x0011D10C
	private void OnUpdateAuthority(float dt)
	{
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
			if (state != GRToolDirectionalShield.State.Open)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetStateAuthority(GRToolDirectionalShield.State.Closed);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetStateAuthority(GRToolDirectionalShield.State.Open);
			return;
		}
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x0011EF6C File Offset: 0x0011D16C
	private void OnUpdateRemote(float dt)
	{
		GRToolDirectionalShield.State state = (GRToolDirectionalShield.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06003426 RID: 13350 RVA: 0x0011EF96 File Offset: 0x0011D196
	private void SetStateAuthority(GRToolDirectionalShield.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06003427 RID: 13351 RVA: 0x0011EFB8 File Offset: 0x0011D1B8
	private void SetState(GRToolDirectionalShield.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
		}
		this.state = newState;
		state = this.state;
		if (state == GRToolDirectionalShield.State.Closed)
		{
			this.openCollidersParent.gameObject.SetActive(false);
			for (int i = 0; i < this.shieldAnimators.Count; i++)
			{
				this.shieldAnimators[i].SetBool("Activated", false);
			}
			this.audioSource.PlayOneShot(this.closeAudio, this.closeVolume);
			this.closeHaptic.PlayIfHeldLocal(this.gameEntity);
			this.hitter != null;
			return;
		}
		if (state != GRToolDirectionalShield.State.Open)
		{
			return;
		}
		this.openCollidersParent.gameObject.SetActive(true);
		for (int j = 0; j < this.shieldAnimators.Count; j++)
		{
			this.shieldAnimators[j].SetBool("Activated", true);
		}
		this.audioSource.PlayOneShot(this.openAudio, this.openVolume);
		this.openHaptic.PlayIfHeldLocal(this.gameEntity);
		this.hitter != null;
	}

	// Token: 0x06003428 RID: 13352 RVA: 0x0011F0DC File Offset: 0x0011D2DC
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x040043ED RID: 17389
	[Header("References")]
	public GameEntity gameEntity;

	// Token: 0x040043EE RID: 17390
	public GRTool tool;

	// Token: 0x040043EF RID: 17391
	public Rigidbody rigidBody;

	// Token: 0x040043F0 RID: 17392
	public AudioSource audioSource;

	// Token: 0x040043F1 RID: 17393
	public List<Animator> shieldAnimators;

	// Token: 0x040043F2 RID: 17394
	public Transform openCollidersParent;

	// Token: 0x040043F3 RID: 17395
	private GameHitter hitter;

	// Token: 0x040043F4 RID: 17396
	private GRAttributes attributes;

	// Token: 0x040043F5 RID: 17397
	[Header("Audio")]
	public AudioClip openAudio;

	// Token: 0x040043F6 RID: 17398
	public float openVolume = 0.5f;

	// Token: 0x040043F7 RID: 17399
	public AudioClip closeAudio;

	// Token: 0x040043F8 RID: 17400
	public float closeVolume = 0.5f;

	// Token: 0x040043F9 RID: 17401
	public AudioClip deflectAudio;

	// Token: 0x040043FA RID: 17402
	public AudioClip upgrade1DeflectAudio;

	// Token: 0x040043FB RID: 17403
	public AudioClip upgrade2DeflectAudio;

	// Token: 0x040043FC RID: 17404
	public AudioClip upgrade3DeflectAudio;

	// Token: 0x040043FD RID: 17405
	public float deflectVolume = 0.5f;

	// Token: 0x040043FE RID: 17406
	[Header("VFX")]
	public ParticleSystem shieldDeflectVFX;

	// Token: 0x040043FF RID: 17407
	public ParticleSystem upgrade1ShieldDeflectVFX;

	// Token: 0x04004400 RID: 17408
	public ParticleSystem upgrade2ShieldDeflectVFX;

	// Token: 0x04004401 RID: 17409
	public ParticleSystem upgrade3ShieldDeflectVFX;

	// Token: 0x04004402 RID: 17410
	public ParticleSystem shieldDeflectImpactPointVFX;

	// Token: 0x04004403 RID: 17411
	public Transform shieldArcCenterReferencePoint;

	// Token: 0x04004404 RID: 17412
	public float shieldArcCenterRadius = 1f;

	// Token: 0x04004405 RID: 17413
	[Header("Haptic")]
	public AbilityHaptic openHaptic;

	// Token: 0x04004406 RID: 17414
	public AbilityHaptic closeHaptic;

	// Token: 0x04004407 RID: 17415
	public bool reflectsProjectiles;

	// Token: 0x04004408 RID: 17416
	private GRToolDirectionalShield.State state;

	// Token: 0x020007F8 RID: 2040
	private enum State
	{
		// Token: 0x0400440A RID: 17418
		Closed,
		// Token: 0x0400440B RID: 17419
		Open
	}
}
