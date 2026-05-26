using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000808 RID: 2056
[RequireComponent(typeof(GameEntity))]
public class GRToolRevive : MonoBehaviour
{
	// Token: 0x060034B9 RID: 13497 RVA: 0x001227EF File Offset: 0x001209EF
	private void Awake()
	{
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x060034BA RID: 13498 RVA: 0x001227F8 File Offset: 0x001209F8
	private void OnEnable()
	{
		this.StopRevive();
		this.state = GRToolRevive.State.Idle;
	}

	// Token: 0x060034BB RID: 13499 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDestroy()
	{
	}

	// Token: 0x060034BC RID: 13500 RVA: 0x00122808 File Offset: 0x00120A08
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x060034BD RID: 13501 RVA: 0x00122838 File Offset: 0x00120A38
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolRevive.State.Idle:
			if (this.tool.HasEnoughEnergy() && this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Reviving);
				return;
			}
			break;
		case GRToolRevive.State.Reviving:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolRevive.State.Cooldown);
				return;
			}
			break;
		case GRToolRevive.State.Cooldown:
			if (!this.IsButtonHeld())
			{
				this.SetStateAuthority(GRToolRevive.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x001228B0 File Offset: 0x00120AB0
	private void OnUpdateRemote(float dt)
	{
		GRToolRevive.State state = (GRToolRevive.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x001228DA File Offset: 0x00120ADA
	private void SetStateAuthority(GRToolRevive.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060034C0 RID: 13504 RVA: 0x001228FC File Offset: 0x00120AFC
	private void SetState(GRToolRevive.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		if (this.state == GRToolRevive.State.Reviving)
		{
			this.StopRevive();
		}
		this.state = newState;
		GRToolRevive.State state = this.state;
		if (state != GRToolRevive.State.Idle)
		{
			if (state == GRToolRevive.State.Reviving)
			{
				this.StartRevive();
				this.stateTimeRemaining = this.reviveDuration;
				return;
			}
		}
		else
		{
			this.stateTimeRemaining = -1f;
		}
	}

	// Token: 0x060034C1 RID: 13505 RVA: 0x00122958 File Offset: 0x00120B58
	private void StartRevive()
	{
		this.reviveFx.SetActive(true);
		this.audioSource.volume = this.reviveSoundVolume;
		this.audioSource.clip = this.reviveSound;
		this.audioSource.Play();
		this.tool.UseEnergy();
		this.onHaptic.PlayIfHeldLocal(this.gameEntity);
		if (this.gameEntity.IsAuthority())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.5f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, this.reviveDistance, this.playerLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
					if (component != null && component.State != GRPlayer.GRPlayerState.Alive)
					{
						GhostReactorManager.Get(this.gameEntity).RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Alive);
						return;
					}
				}
			}
		}
	}

	// Token: 0x060034C2 RID: 13506 RVA: 0x00122A6A File Offset: 0x00120C6A
	private void StopRevive()
	{
		this.reviveFx.SetActive(false);
		this.audioSource.Stop();
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x00122A84 File Offset: 0x00120C84
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

	// Token: 0x040044E0 RID: 17632
	public GameEntity gameEntity;

	// Token: 0x040044E1 RID: 17633
	public GRTool tool;

	// Token: 0x040044E2 RID: 17634
	[SerializeField]
	private Transform shootFrom;

	// Token: 0x040044E3 RID: 17635
	[SerializeField]
	private LayerMask playerLayerMask;

	// Token: 0x040044E4 RID: 17636
	[SerializeField]
	private float reviveDistance = 1.5f;

	// Token: 0x040044E5 RID: 17637
	[SerializeField]
	private GameObject reviveFx;

	// Token: 0x040044E6 RID: 17638
	[SerializeField]
	private float reviveSoundVolume;

	// Token: 0x040044E7 RID: 17639
	[SerializeField]
	private AudioClip reviveSound;

	// Token: 0x040044E8 RID: 17640
	[SerializeField]
	private float reviveDuration = 0.75f;

	// Token: 0x040044E9 RID: 17641
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040044EA RID: 17642
	[Header("Haptic")]
	public AbilityHaptic onHaptic;

	// Token: 0x040044EB RID: 17643
	private GRToolRevive.State state;

	// Token: 0x040044EC RID: 17644
	private float stateTimeRemaining;

	// Token: 0x040044ED RID: 17645
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x02000809 RID: 2057
	private enum State
	{
		// Token: 0x040044EF RID: 17647
		Idle,
		// Token: 0x040044F0 RID: 17648
		Reviving,
		// Token: 0x040044F1 RID: 17649
		Cooldown
	}
}
