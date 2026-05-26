using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020007F9 RID: 2041
public class GRToolFlash : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x0600342A RID: 13354 RVA: 0x0011F173 File Offset: 0x0011D373
	private void Awake()
	{
		this.state = GRToolFlash.State.Idle;
		this.stateTimeRemaining = -1f;
		this.gameHitter = base.GetComponent<GameHitter>();
	}

	// Token: 0x0600342B RID: 13355 RVA: 0x0011F193 File Offset: 0x0011D393
	private void OnEnable()
	{
		this.StopFlash();
		this.SetState(GRToolFlash.State.Idle);
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x0011F1A2 File Offset: 0x0011D3A2
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x0011F1D8 File Offset: 0x0011D3D8
	private void OnToolUpgraded(GRTool tool)
	{
		this.stunDuration = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.FlashStunDuration);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage1))
		{
			this.flashSound = this.upgrade1FlashSound;
			this.flash = this.upgrade1FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage2))
		{
			this.flashSound = this.upgrade2FlashSound;
			this.flash = this.upgrade2FlashCone;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
		{
			this.flashSound = this.upgrade3FlashSound;
			this.flash = this.upgrade3FlashCone;
		}
	}

	// Token: 0x06003430 RID: 13360 RVA: 0x0011F25D File Offset: 0x0011D45D
	private bool IsHeldLocal()
	{
		return this.item.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06003431 RID: 13361 RVA: 0x0011F276 File Offset: 0x0011D476
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x0011F290 File Offset: 0x0011D490
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06003433 RID: 13363 RVA: 0x0011F2C4 File Offset: 0x0011D4C4
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Charging);
				this.activatedLocally = true;
				return;
			}
			break;
		case GRToolFlash.State.Charging:
		{
			bool flag = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Flash);
				return;
			}
			if (!flag)
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolFlash.State.Flash:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolFlash.State.Cooldown);
				return;
			}
			break;
		case GRToolFlash.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f && !this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolFlash.State.Idle);
				this.activatedLocally = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x0011F3AC File Offset: 0x0011D5AC
	private void OnUpdateRemote(float dt)
	{
		GRToolFlash.State state = (GRToolFlash.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			if (this.state == GRToolFlash.State.Charging && state == GRToolFlash.State.Cooldown)
			{
				this.SetState(GRToolFlash.State.Flash);
				return;
			}
			if (this.state == GRToolFlash.State.Flash && state == GRToolFlash.State.Cooldown)
			{
				if (Time.time > this.timeLastFlashed + this.flashDuration)
				{
					this.SetState(GRToolFlash.State.Cooldown);
					return;
				}
			}
			else
			{
				this.SetState(state);
			}
		}
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x0011F414 File Offset: 0x0011D614
	private void SetStateAuthority(GRToolFlash.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x0011F438 File Offset: 0x0011D638
	private void SetState(GRToolFlash.State newState)
	{
		if (!this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		switch (this.state)
		{
		case GRToolFlash.State.Idle:
			this.stateTimeRemaining = -1f;
			return;
		case GRToolFlash.State.Charging:
			this.StartCharge();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolFlash.State.Flash:
			this.StartFlash();
			this.stateTimeRemaining = this.flashDuration;
			return;
		case GRToolFlash.State.Cooldown:
			this.StopFlash();
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06003437 RID: 13367 RVA: 0x0011F4BC File Offset: 0x0011D6BC
	private void StartCharge()
	{
		this.audioSource.volume = this.chargeSoundVolume;
		this.audioSource.clip = this.chargeSound;
		this.audioSource.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06003438 RID: 13368 RVA: 0x0011F514 File Offset: 0x0011D714
	private void StartFlash()
	{
		this.flash.SetActive(true);
		this.audioSource.volume = this.flashSoundVolume;
		this.audioSource.clip = this.flashSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.timeLastFlashed = Time.time;
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 1f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 5f, this.enemyLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					GameHittable component = attachedRigidbody.GetComponent<GameHittable>();
					if (component != null && this.gameHitter != null)
					{
						GameHitData hitData = new GameHitData
						{
							hitTypeId = 1,
							hitEntityId = component.gameEntity.id,
							hitByEntityId = this.gameEntity.id,
							hitEntityPosition = component.gameEntity.transform.position,
							hitPosition = ((raycastHit.distance == 0f) ? this.shootFrom.position : raycastHit.point),
							hitImpulse = Vector3.zero,
							hitAmount = this.gameHitter.CalcHitAmount(GameHitType.Flash, component, this.gameEntity),
							hittablePoint = component.FindHittablePoint(raycastHit.collider)
						};
						component.RequestHit(hitData);
					}
				}
			}
		}
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x0011F6D9 File Offset: 0x0011D8D9
	private void StopFlash()
	{
		this.flash.SetActive(false);
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x0011F6E8 File Offset: 0x0011D8E8
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x0600343B RID: 13371 RVA: 0x0011F748 File Offset: 0x0011D948
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.item.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x0600343C RID: 13372 RVA: 0x0011F79C File Offset: 0x0011D99C
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L && ((int)newStateIndex != 2 || Time.time > this.timeLastFlashed + this.cooldownMinimum);
	}

	// Token: 0x0600343D RID: 13373 RVA: 0x0011F7C5 File Offset: 0x0011D9C5
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Stun Duration: <color=\"yellow\">{0}<color=\"white\">", this.stunDuration));
	}

	// Token: 0x0400440C RID: 17420
	public GameEntity gameEntity;

	// Token: 0x0400440D RID: 17421
	public GRTool tool;

	// Token: 0x0400440E RID: 17422
	public GRAttributes attributes;

	// Token: 0x0400440F RID: 17423
	public GameObject flash;

	// Token: 0x04004410 RID: 17424
	public Transform shootFrom;

	// Token: 0x04004411 RID: 17425
	public LayerMask enemyLayerMask;

	// Token: 0x04004412 RID: 17426
	public AudioSource audioSource;

	// Token: 0x04004413 RID: 17427
	public AudioClip chargeSound;

	// Token: 0x04004414 RID: 17428
	public float chargeSoundVolume = 0.2f;

	// Token: 0x04004415 RID: 17429
	public AudioClip flashSound;

	// Token: 0x04004416 RID: 17430
	public AudioClip upgrade1FlashSound;

	// Token: 0x04004417 RID: 17431
	public AudioClip upgrade2FlashSound;

	// Token: 0x04004418 RID: 17432
	public AudioClip upgrade3FlashSound;

	// Token: 0x04004419 RID: 17433
	public GameObject upgrade1FlashCone;

	// Token: 0x0400441A RID: 17434
	public GameObject upgrade2FlashCone;

	// Token: 0x0400441B RID: 17435
	public GameObject upgrade3FlashCone;

	// Token: 0x0400441C RID: 17436
	public float flashSoundVolume = 1f;

	// Token: 0x0400441D RID: 17437
	public float stunDuration;

	// Token: 0x0400441E RID: 17438
	public GRToolFlash.UpgradeTypes upgradesApplied;

	// Token: 0x0400441F RID: 17439
	public float chargeDuration = 0.75f;

	// Token: 0x04004420 RID: 17440
	public float flashDuration = 0.1f;

	// Token: 0x04004421 RID: 17441
	public float cooldownDuration;

	// Token: 0x04004422 RID: 17442
	private float timeLastFlashed;

	// Token: 0x04004423 RID: 17443
	private float cooldownMinimum = 0.35f;

	// Token: 0x04004424 RID: 17444
	private bool activatedLocally;

	// Token: 0x04004425 RID: 17445
	public GameEntity item;

	// Token: 0x04004426 RID: 17446
	private GameHitter gameHitter;

	// Token: 0x04004427 RID: 17447
	private GRToolFlash.State state;

	// Token: 0x04004428 RID: 17448
	private float stateTimeRemaining;

	// Token: 0x04004429 RID: 17449
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x020007FA RID: 2042
	[Flags]
	public enum UpgradeTypes
	{
		// Token: 0x0400442B RID: 17451
		None = 1,
		// Token: 0x0400442C RID: 17452
		UpagredA = 2,
		// Token: 0x0400442D RID: 17453
		UpagredB = 4,
		// Token: 0x0400442E RID: 17454
		UpagredC = 8
	}

	// Token: 0x020007FB RID: 2043
	private enum State
	{
		// Token: 0x04004430 RID: 17456
		Idle,
		// Token: 0x04004431 RID: 17457
		Charging,
		// Token: 0x04004432 RID: 17458
		Flash,
		// Token: 0x04004433 RID: 17459
		Cooldown,
		// Token: 0x04004434 RID: 17460
		Count
	}
}
