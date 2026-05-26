using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020007F3 RID: 2035
public class GRToolClub : MonoBehaviourTick, IGameHitter, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x060033F2 RID: 13298 RVA: 0x0011DBD1 File Offset: 0x0011BDD1
	private void Awake()
	{
		this.retractableSection.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0011DBF2 File Offset: 0x0011BDF2
	public new void OnEnable()
	{
		base.OnEnable();
		this.SetExtendedAmount(0f);
		this.gameHitter.hitFx = this.noPowerFx;
		this.gameHitter.damageAttribute = this.noPowerAttribute;
		this.SetState(GRToolClub.State.Idle);
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x0011DC2E File Offset: 0x0011BE2E
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060033F7 RID: 13303 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnToolUpgraded(GRTool tool)
	{
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x0011DC64 File Offset: 0x0011BE64
	private void EnableImpactVFXForCurrentUpgradeLevel()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage1))
		{
			this.gameHitter.hitFx = this.upgrade1ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage2))
		{
			this.gameHitter.hitFx = this.upgrade2ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage3))
		{
			this.gameHitter.hitFx = this.upgrade3ImpactVFX;
			return;
		}
		this.gameHitter.hitFx = this.poweredImpactFx;
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x0011DCE4 File Offset: 0x0011BEE4
	public override void Tick()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeld())
		{
			if (this.gameEntity.IsHeldByLocalPlayer())
			{
				this.OnUpdateAuthority(deltaTime);
			}
			else
			{
				this.OnUpdateRemote(deltaTime);
			}
		}
		else
		{
			this.SetState(GRToolClub.State.Idle);
		}
		this.OnUpdateShared(deltaTime);
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x0011DD34 File Offset: 0x0011BF34
	private void OnUpdateAuthority(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolClub.State.Idle);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetState(GRToolClub.State.Extended);
			return;
		}
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x0011DD94 File Offset: 0x0011BF94
	private void OnUpdateRemote(float dt)
	{
		GRToolClub.State state = (GRToolClub.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x0011DDC0 File Offset: 0x0011BFC0
	private void OnUpdateShared(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (this.extendedAmount < 1f)
			{
				float num = Mathf.MoveTowards(this.extendedAmount, 1f, 1f / this.extensionTime * Time.deltaTime);
				this.SetExtendedAmount(num);
			}
		}
		else if (this.extendedAmount > 0f)
		{
			float num2 = Mathf.MoveTowards(this.extendedAmount, 0f, 1f / this.extensionTime * Time.deltaTime);
			this.SetExtendedAmount(num2);
			return;
		}
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x0011DE4C File Offset: 0x0011C04C
	private void SetExtendedAmount(float newExtendedAmount)
	{
		this.extendedAmount = newExtendedAmount;
		float y = Mathf.Lerp(this.retractableSectionMin, this.retractableSectionMax, this.extendedAmount);
		this.retractableSection.localPosition = new Vector3(0f, y, 0f);
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x0011DE94 File Offset: 0x0011C094
	private void SetState(GRToolClub.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
		}
		this.state = newState;
		state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state == GRToolClub.State.Extended)
			{
				this.idleCollider.enabled = false;
				this.extendedCollider.enabled = true;
				for (int i = 0; i < this.meshAndMaterials.Count; i++)
				{
					MaterialUtils.SwapMaterial(this.meshAndMaterials[i], false);
				}
				this.humAudioSource.Play();
				this.dullLight.SetActive(true);
				this.audioSource.PlayOneShot(this.extendAudio, this.extendVolume);
				for (int j = 0; j < this.humParticleEffects.Count; j++)
				{
					this.humParticleEffects[j].gameObject.SetActive(true);
				}
				this.EnableImpactVFXForCurrentUpgradeLevel();
				this.gameHitter.damageAttribute = this.poweredAttribute;
				this.openHaptic.PlayIfHeldLocal(this.gameEntity);
			}
		}
		else
		{
			this.extendedCollider.enabled = false;
			this.idleCollider.enabled = true;
			for (int k = 0; k < this.meshAndMaterials.Count; k++)
			{
				MaterialUtils.SwapMaterial(this.meshAndMaterials[k], true);
			}
			this.humAudioSource.Stop();
			this.dullLight.SetActive(false);
			this.audioSource.PlayOneShot(this.retractAudio, this.retractVolume);
			for (int l = 0; l < this.humParticleEffects.Count; l++)
			{
				this.humParticleEffects[l].gameObject.SetActive(false);
			}
			this.gameHitter.hitFx = this.noPowerFx;
			this.gameHitter.damageAttribute = this.noPowerAttribute;
			this.closeHaptic.PlayIfHeldLocal(this.gameEntity);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x0011E098 File Offset: 0x0011C298
	private bool IsButtonHeld()
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x0011E0FA File Offset: 0x0011C2FA
	public void OnSuccessfulHit(GameHitData hitData)
	{
		if (this.state == GRToolClub.State.Extended)
		{
			this.tool.UseEnergy();
		}
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x0011E110 File Offset: 0x0011C310
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Knockback: <color=\"yellow\">x{0}<color=\"white\">", this.gameHitter.knockbackMultiplier));
	}

	// Token: 0x040043A0 RID: 17312
	public GameEntity gameEntity;

	// Token: 0x040043A1 RID: 17313
	public GameHitter gameHitter;

	// Token: 0x040043A2 RID: 17314
	public GRTool tool;

	// Token: 0x040043A3 RID: 17315
	public Rigidbody rigidBody;

	// Token: 0x040043A4 RID: 17316
	public AudioSource audioSource;

	// Token: 0x040043A5 RID: 17317
	public AudioSource humAudioSource;

	// Token: 0x040043A6 RID: 17318
	public List<ParticleSystem> humParticleEffects = new List<ParticleSystem>();

	// Token: 0x040043A7 RID: 17319
	public GRAttributes attributes;

	// Token: 0x040043A8 RID: 17320
	public AudioClip extendAudio;

	// Token: 0x040043A9 RID: 17321
	public float extendVolume = 0.5f;

	// Token: 0x040043AA RID: 17322
	public AudioClip retractAudio;

	// Token: 0x040043AB RID: 17323
	public float retractVolume = 0.5f;

	// Token: 0x040043AC RID: 17324
	public GameHitFx noPowerFx;

	// Token: 0x040043AD RID: 17325
	public GameHitFx poweredImpactFx;

	// Token: 0x040043AE RID: 17326
	public GameHitFx upgrade1ImpactVFX;

	// Token: 0x040043AF RID: 17327
	public GameHitFx upgrade2ImpactVFX;

	// Token: 0x040043B0 RID: 17328
	public GameHitFx upgrade3ImpactVFX;

	// Token: 0x040043B1 RID: 17329
	public GRAttributeType noPowerAttribute;

	// Token: 0x040043B2 RID: 17330
	public GRAttributeType poweredAttribute;

	// Token: 0x040043B3 RID: 17331
	public float minHitSpeed = 2.25f;

	// Token: 0x040043B4 RID: 17332
	public GameObject dullLight;

	// Token: 0x040043B5 RID: 17333
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x040043B6 RID: 17334
	public Transform retractableSection;

	// Token: 0x040043B7 RID: 17335
	public Collider idleCollider;

	// Token: 0x040043B8 RID: 17336
	public Collider extendedCollider;

	// Token: 0x040043B9 RID: 17337
	public float retractableSectionMin = -0.31f;

	// Token: 0x040043BA RID: 17338
	public float retractableSectionMax;

	// Token: 0x040043BB RID: 17339
	public float extensionTime = 0.15f;

	// Token: 0x040043BC RID: 17340
	[Header("Haptic")]
	public AbilityHaptic openHaptic;

	// Token: 0x040043BD RID: 17341
	public AbilityHaptic closeHaptic;

	// Token: 0x040043BE RID: 17342
	private float extendedAmount;

	// Token: 0x040043BF RID: 17343
	private GRToolClub.State state;

	// Token: 0x020007F4 RID: 2036
	private enum State
	{
		// Token: 0x040043C1 RID: 17345
		Idle,
		// Token: 0x040043C2 RID: 17346
		Extended
	}
}
