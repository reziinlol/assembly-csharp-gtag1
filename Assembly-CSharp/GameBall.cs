using System;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
public class GameBall : MonoBehaviour
{
	// Token: 0x170003EF RID: 1007
	// (get) Token: 0x06002590 RID: 9616 RVA: 0x000C6C4B File Offset: 0x000C4E4B
	public bool IsLaunched
	{
		get
		{
			return this._launched;
		}
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000C6C54 File Offset: 0x000C4E54
	private void Awake()
	{
		this.id = GameBallId.Invalid;
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.lastHeldByTeamId = -1;
		this.onlyGrabTeamId = -1;
		this._monkeBall = base.GetComponent<MonkeBall>();
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000C6CE8 File Offset: 0x000C4EE8
	private void FixedUpdate()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this._launched)
		{
			this._launchedTimer += Time.fixedDeltaTime;
			if (this.collider.isTrigger && this._launchedTimer > 1f && this.rigidBody.linearVelocity.y <= 0f)
			{
				this._launched = false;
				this.collider.isTrigger = false;
			}
		}
		Vector3 a = -Physics.gravity * (1f - this.gravityMult);
		this.rigidBody.AddForce(a * this.rigidBody.mass, ForceMode.Force);
		this._catchSoundDecay -= Time.deltaTime;
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000C6DAD File Offset: 0x000C4FAD
	public void WasLaunched()
	{
		this._launched = true;
		this.collider.isTrigger = true;
		this._launchedTimer = 0f;
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x000C6DCD File Offset: 0x000C4FCD
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.linearVelocity;
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x000C6DEE File Offset: 0x000C4FEE
	public void SetVelocity(Vector3 velocity)
	{
		this.rigidBody.linearVelocity = velocity;
	}

	// Token: 0x06002596 RID: 9622 RVA: 0x000C6DFC File Offset: 0x000C4FFC
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this._catchSoundDecay <= 0f && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.catchSound;
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.GTPlay();
			this._catchSoundDecay = 0.1f;
		}
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000C6E6C File Offset: 0x000C506C
	public void PlayThrowFx()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.throwSound;
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000C6EC4 File Offset: 0x000C50C4
	public void PlayBounceFX()
	{
		if (this.audioSource != null && this.audioSource.isActiveAndEnabled)
		{
			this.audioSource.clip = this.groundSound;
			this.audioSource.volume = this.groundSoundVolume;
			this.audioSource.GTPlay();
		}
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000C6F19 File Offset: 0x000C5119
	public void SetHeldByTeamId(int teamId)
	{
		this.lastHeldByTeamId = teamId;
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000C6F22 File Offset: 0x000C5122
	private bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000C6F31 File Offset: 0x000C5131
	public void SetVisualOffset(bool detach)
	{
		if (this._monkeBall != null)
		{
			this._monkeBall.SetVisualOffset(detach);
		}
	}

	// Token: 0x04003106 RID: 12550
	public GameBallId id;

	// Token: 0x04003107 RID: 12551
	public float gravityMult = 1f;

	// Token: 0x04003108 RID: 12552
	public bool disc;

	// Token: 0x04003109 RID: 12553
	public Vector3 localDiscUp;

	// Token: 0x0400310A RID: 12554
	public AudioSource audioSource;

	// Token: 0x0400310B RID: 12555
	public AudioClip catchSound;

	// Token: 0x0400310C RID: 12556
	public float catchSoundVolume;

	// Token: 0x0400310D RID: 12557
	private float _catchSoundDecay;

	// Token: 0x0400310E RID: 12558
	public AudioClip throwSound;

	// Token: 0x0400310F RID: 12559
	public float throwSoundVolume;

	// Token: 0x04003110 RID: 12560
	public AudioClip groundSound;

	// Token: 0x04003111 RID: 12561
	public float groundSoundVolume;

	// Token: 0x04003112 RID: 12562
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04003113 RID: 12563
	[SerializeField]
	private Collider collider;

	// Token: 0x04003114 RID: 12564
	public int heldByActorNumber;

	// Token: 0x04003115 RID: 12565
	public int lastHeldByActorNumber;

	// Token: 0x04003116 RID: 12566
	public int lastHeldByTeamId;

	// Token: 0x04003117 RID: 12567
	public int onlyGrabTeamId;

	// Token: 0x04003118 RID: 12568
	private bool _launched;

	// Token: 0x04003119 RID: 12569
	private float _launchedTimer;

	// Token: 0x0400311A RID: 12570
	public MonkeBall _monkeBall;
}
