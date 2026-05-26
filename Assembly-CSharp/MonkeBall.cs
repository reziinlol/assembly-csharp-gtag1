using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005F4 RID: 1524
public class MonkeBall : MonoBehaviourTick
{
	// Token: 0x060025ED RID: 9709 RVA: 0x000C8D64 File Offset: 0x000C6F64
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x060025EE RID: 9710 RVA: 0x000C8D6C File Offset: 0x000C6F6C
	public override void Tick()
	{
		this.UpdateVisualOffset();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._resyncPosition)
			{
				this._resyncDelay -= Time.deltaTime;
				if (this._resyncDelay <= 0f)
				{
					this._resyncPosition = false;
					GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
				}
			}
			if (this._positionFailsafe)
			{
				if (base.transform.position.y < -500f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					if (PhotonNetwork.IsConnected)
					{
						GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
					}
					else
					{
						base.transform.position = GameBallManager.Instance.transform.position;
					}
					this._positionFailsafe = false;
					this._positionFailsafeTimer = 3f;
					return;
				}
			}
			else
			{
				this._positionFailsafeTimer -= Time.deltaTime;
				if (this._positionFailsafeTimer <= 0f)
				{
					this._positionFailsafe = true;
				}
			}
			return;
		}
		if (this.gameBall.onlyGrabTeamId != -1 && Time.timeAsDouble >= this.restrictTeamGrabEndTime)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, -1);
		}
		if (this.AlreadyDropped())
		{
			this._droppedTimer += Time.deltaTime;
			if (this._droppedTimer >= 7.5f)
			{
				this._droppedTimer = 0f;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._justGrabbed)
		{
			this._justGrabbedTimer -= Time.deltaTime;
			if (this._justGrabbedTimer <= 0f)
			{
				this._justGrabbed = false;
			}
		}
		if (this._resyncPosition)
		{
			this._resyncDelay -= Time.deltaTime;
			if (this._resyncDelay <= 0f)
			{
				this._resyncPosition = false;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.linearVelocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._positionFailsafe)
		{
			if (base.transform.position.y < -250f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
			{
				MonkeBallGame.Instance.LaunchBallNeutral(this.gameBall.id);
				this._positionFailsafe = false;
				this._positionFailsafeTimer = 3f;
				return;
			}
		}
		else
		{
			this._positionFailsafeTimer -= Time.deltaTime;
			if (this._positionFailsafeTimer <= 0f)
			{
				this._positionFailsafe = true;
			}
		}
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000C9078 File Offset: 0x000C7278
	public void OnCollisionEnter(Collision collision)
	{
		if (this.AlreadyDropped() || this._justGrabbed)
		{
			return;
		}
		if (MonkeBall.IsGamePlayer(collision.collider))
		{
			return;
		}
		this.alreadyDropped = true;
		this._droppedTimer = 0f;
		this.gameBall.PlayBounceFX();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
			{
				this._resyncPosition = true;
				this._resyncDelay = 1.5f;
			}
			int lastHeldByActorNumber = this.gameBall.lastHeldByActorNumber;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			return;
		}
		if (this._rigidBody.linearVelocity.sqrMagnitude > 1f)
		{
			this._resyncPosition = true;
			this._resyncDelay = 0.5f;
		}
		if (this._launchAfterScore)
		{
			this._launchAfterScore = false;
			MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
			return;
		}
		MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
	}

	// Token: 0x060025F0 RID: 9712 RVA: 0x000C919F File Offset: 0x000C739F
	public void TriggerDelayedResync()
	{
		this._resyncPosition = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._resyncDelay = 0.5f;
			return;
		}
		this._resyncDelay = 1.5f;
	}

	// Token: 0x060025F1 RID: 9713 RVA: 0x000C91C6 File Offset: 0x000C73C6
	public void SetRigidbodyDiscrete()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x060025F2 RID: 9714 RVA: 0x000C91D4 File Offset: 0x000C73D4
	public void SetRigidbodyContinuous()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	// Token: 0x060025F3 RID: 9715 RVA: 0x000C91E2 File Offset: 0x000C73E2
	public static MonkeBall Get(GameBall ball)
	{
		if (ball == null)
		{
			return null;
		}
		return ball.GetComponent<MonkeBall>();
	}

	// Token: 0x060025F4 RID: 9716 RVA: 0x000C91F5 File Offset: 0x000C73F5
	public bool AlreadyDropped()
	{
		return this.alreadyDropped;
	}

	// Token: 0x060025F5 RID: 9717 RVA: 0x000C91FD File Offset: 0x000C73FD
	public void OnGrabbed()
	{
		this.alreadyDropped = false;
		this._justGrabbed = true;
		this._justGrabbedTimer = 0.1f;
		this._resyncPosition = false;
	}

	// Token: 0x060025F6 RID: 9718 RVA: 0x000C921F File Offset: 0x000C741F
	public void OnSwitchHeldByTeam(int teamId)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, teamId);
		}
	}

	// Token: 0x060025F7 RID: 9719 RVA: 0x000C923E File Offset: 0x000C743E
	public void ClearCannotGrabTeamId()
	{
		this.gameBall.onlyGrabTeamId = -1;
		this.restrictTeamGrabEndTime = -1.0;
		this.Refresh();
	}

	// Token: 0x060025F8 RID: 9720 RVA: 0x000C9264 File Offset: 0x000C7464
	public bool RestrictBallToTeam(int teamId, float duration)
	{
		if (teamId == this.gameBall.onlyGrabTeamId && Time.timeAsDouble + (double)duration < this.restrictTeamGrabEndTime)
		{
			return false;
		}
		this.gameBall.onlyGrabTeamId = teamId;
		this.restrictTeamGrabEndTime = Time.timeAsDouble + (double)duration;
		this.Refresh();
		return true;
	}

	// Token: 0x060025F9 RID: 9721 RVA: 0x000C92B2 File Offset: 0x000C74B2
	private void Refresh()
	{
		if (this.gameBall.onlyGrabTeamId == -1)
		{
			this.mainRenderer.material = this.defaultMaterial;
			return;
		}
		this.mainRenderer.material = this.teamMaterial[this.gameBall.onlyGrabTeamId];
	}

	// Token: 0x060025FA RID: 9722 RVA: 0x000C92F1 File Offset: 0x000C74F1
	private static bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x060025FB RID: 9723 RVA: 0x000C9300 File Offset: 0x000C7500
	public void SetVisualOffset(bool detach)
	{
		if (detach)
		{
			this.lastVisiblePosition = this.mainRenderer.transform.position;
			this._visualOffset = true;
			this._timeOffset = Time.time;
			this.mainRenderer.transform.SetParent(null, true);
			return;
		}
		this.ReattachVisuals();
	}

	// Token: 0x060025FC RID: 9724 RVA: 0x000C9354 File Offset: 0x000C7554
	private void ReattachVisuals()
	{
		if (!this._visualOffset)
		{
			return;
		}
		this.mainRenderer.transform.SetParent(base.transform);
		this.mainRenderer.transform.localPosition = Vector3.zero;
		this.mainRenderer.transform.localRotation = Quaternion.identity;
		this._visualOffset = false;
	}

	// Token: 0x060025FD RID: 9725 RVA: 0x000C93B4 File Offset: 0x000C75B4
	private void UpdateVisualOffset()
	{
		if (this._visualOffset)
		{
			this.mainRenderer.transform.position = Vector3.Lerp(this.mainRenderer.transform.position, this._rigidBody.position, Mathf.Clamp((Time.time - this._timeOffset) / this.maxLerpTime, this.offsetLerp, 1f));
			if ((this.mainRenderer.transform.position - this._rigidBody.position).sqrMagnitude < this._offsetThreshold)
			{
				this.ReattachVisuals();
			}
		}
	}

	// Token: 0x04003151 RID: 12625
	public GameBall gameBall;

	// Token: 0x04003152 RID: 12626
	public MeshRenderer mainRenderer;

	// Token: 0x04003153 RID: 12627
	public Material defaultMaterial;

	// Token: 0x04003154 RID: 12628
	public Material[] teamMaterial;

	// Token: 0x04003155 RID: 12629
	public double restrictTeamGrabEndTime;

	// Token: 0x04003156 RID: 12630
	public bool alreadyDropped;

	// Token: 0x04003157 RID: 12631
	private bool _justGrabbed;

	// Token: 0x04003158 RID: 12632
	private float _justGrabbedTimer;

	// Token: 0x04003159 RID: 12633
	private bool _launchAfterScore;

	// Token: 0x0400315A RID: 12634
	private float _droppedTimer;

	// Token: 0x0400315B RID: 12635
	private bool _resyncPosition;

	// Token: 0x0400315C RID: 12636
	private float _resyncDelay;

	// Token: 0x0400315D RID: 12637
	private bool _visualOffset;

	// Token: 0x0400315E RID: 12638
	private float _offsetThreshold = 0.05f;

	// Token: 0x0400315F RID: 12639
	private float _timeOffset;

	// Token: 0x04003160 RID: 12640
	public float maxLerpTime = 0.5f;

	// Token: 0x04003161 RID: 12641
	public float offsetLerp = 0.2f;

	// Token: 0x04003162 RID: 12642
	private bool _positionFailsafe = true;

	// Token: 0x04003163 RID: 12643
	private float _positionFailsafeTimer;

	// Token: 0x04003164 RID: 12644
	public Vector3 lastVisiblePosition;

	// Token: 0x04003165 RID: 12645
	[SerializeField]
	private Rigidbody _rigidBody;
}
