using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020007DA RID: 2010
public class GRSentientCore : MonoBehaviour, IGRSleepableEntity
{
	// Token: 0x170004A1 RID: 1185
	// (get) Token: 0x0600334F RID: 13135 RVA: 0x000AA148 File Offset: 0x000A8348
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06003350 RID: 13136 RVA: 0x0011A552 File Offset: 0x00118752
	public float WakeUpRadius
	{
		get
		{
			return this.wakeupRadius;
		}
	}

	// Token: 0x06003351 RID: 13137 RVA: 0x0011A55C File Offset: 0x0011875C
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		GhostReactor.instance.sleepableEntities.Add(this);
		this.gameEntity.OnStateChanged += this.OnStateChanged;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnSnapped = (Action)Delegate.Combine(gameEntity3.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnDetached = (Action)Delegate.Combine(gameEntity4.OnDetached, new Action(this.OnDetached));
		this.Sleep();
	}

	// Token: 0x06003352 RID: 13138 RVA: 0x0011A640 File Offset: 0x00118840
	private void OnDestroy()
	{
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.sleepableEntities.Remove(this);
		}
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnStateChanged;
			GameEntity gameEntity = this.gameEntity;
			gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
			GameEntity gameEntity2 = this.gameEntity;
			gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
			GameEntity gameEntity3 = this.gameEntity;
			gameEntity3.OnSnapped = (Action)Delegate.Remove(gameEntity3.OnSnapped, new Action(this.OnSnapped));
			GameEntity gameEntity4 = this.gameEntity;
			gameEntity4.OnDetached = (Action)Delegate.Remove(gameEntity4.OnDetached, new Action(this.OnDetached));
		}
	}

	// Token: 0x06003353 RID: 13139 RVA: 0x0011A72F File Offset: 0x0011892F
	public bool IsSleeping()
	{
		return this.gameEntity.GetState() == 0L;
	}

	// Token: 0x06003354 RID: 13140 RVA: 0x0011A740 File Offset: 0x00118940
	public void WakeUp()
	{
		if (this.gameEntity.IsAuthority() && this.IsSleeping())
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
		}
		if (this.localState == GRSentientCore.SentientCoreState.Asleep)
		{
			this.localState = GRSentientCore.SentientCoreState.Awake;
			this.localStateStartTime = Time.time;
		}
		this.sleepRequested = false;
		base.enabled = true;
	}

	// Token: 0x06003355 RID: 13141 RVA: 0x0011A7A2 File Offset: 0x001189A2
	public void Sleep()
	{
		this.sleepRequested = true;
	}

	// Token: 0x06003356 RID: 13142 RVA: 0x0011A7AB File Offset: 0x001189AB
	private void OnStateChanged(long prevState, long nextState)
	{
		if ((int)nextState == 0)
		{
			this.sleepRequested = false;
		}
		else if (!base.enabled)
		{
			this.WakeUp();
		}
		this.SetState((GRSentientCore.SentientCoreState)nextState);
	}

	// Token: 0x06003357 RID: 13143 RVA: 0x0011A7D0 File Offset: 0x001189D0
	private void OnGrabbed()
	{
		this.WakeUp();
		this.SetState(GRSentientCore.SentientCoreState.Held);
		this.timeUntilNextAlert = Mathf.Min(this.timeUntilFirstAlert, this.timeUntilNextAlert);
	}

	// Token: 0x06003358 RID: 13144 RVA: 0x0011A7F6 File Offset: 0x001189F6
	private void OnReleased()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x06003359 RID: 13145 RVA: 0x0011A7FF File Offset: 0x001189FF
	private void OnSnapped()
	{
		this.SetState(GRSentientCore.SentientCoreState.AttachedToPlayer);
	}

	// Token: 0x0600335A RID: 13146 RVA: 0x0011A7F6 File Offset: 0x001189F6
	private void OnDetached()
	{
		this.SetState(GRSentientCore.SentientCoreState.Dropped);
	}

	// Token: 0x0600335B RID: 13147 RVA: 0x0011A808 File Offset: 0x00118A08
	private void Update()
	{
		if (this.debugDraw)
		{
			DebugUtil.DrawSphere(base.transform.position, 0.15f, 12, 12, Color.cyan, true, DebugUtil.Style.Wireframe);
		}
		if (this.gameEntity.IsAuthority())
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x0600335C RID: 13148 RVA: 0x0011A858 File Offset: 0x00118A58
	private void AuthorityUpdate()
	{
		if (this.trailFX != null)
		{
			if (this.gameEntity.snappedByActorNumber != -1 || this.gameEntity.heldByActorNumber != -1)
			{
				if (this.trailFX.isPlaying)
				{
					this.trailFX.Stop();
				}
			}
			else if (!this.trailFX.isPlaying)
			{
				this.trailFX.Play();
			}
		}
		switch (this.localState)
		{
		case GRSentientCore.SentientCoreState.Asleep:
		case GRSentientCore.SentientCoreState.JumpAnticipation:
		case GRSentientCore.SentientCoreState.Jumping:
		case GRSentientCore.SentientCoreState.HeldAlert:
		case GRSentientCore.SentientCoreState.Dropped:
			break;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
			}
			if (this.gameEntity.heldByActorNumber != -1)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			if (!this.sleepRequested && Time.time > this.localStateStartTime + this.jumpCooldownTime)
			{
				this.AuthorityInitiateJump();
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpInitiated:
			if (this.sleepRequested)
			{
				this.sleepRequested = false;
				this.SetState(GRSentientCore.SentientCoreState.Asleep);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Held:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.SetState(GRSentientCore.SentientCoreState.HeldAlert);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
			this.timeUntilNextAlert -= Time.deltaTime;
			if (this.timeUntilNextAlert < 0f)
			{
				this.timeUntilNextAlert = Random.Range(this.timeRangeBetweenAlerts.x, this.timeRangeBetweenAlerts.y);
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x0011AA1C File Offset: 0x00118C1C
	private void SharedUpdate()
	{
		switch (this.localState)
		{
		default:
			base.enabled = false;
			return;
		case GRSentientCore.SentientCoreState.Awake:
			if (this.visualCore != null && this.visualCore.transform.localScale != Vector3.one)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.JumpAnticipation:
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			if (Time.time <= this.jumpStartTime)
			{
				Vector3 normalized = (this.surfaceNormal + this.jumpDirection).normalized;
				float num = (this.jumpStartTime - Time.time) / this.jumpAnticipationTime * 0.25f + 0.75f;
				float num2 = Mathf.Sqrt(1f / num);
				this.visualCore.transform.localScale = new Vector3(num2, num, num2);
				this.visualCore.transform.position = this.visualCore.parent.position - normalized * (1f - num) * this.radius;
				this.visualCore.transform.rotation = Quaternion.FromToRotation(Vector3.up, normalized);
				return;
			}
			this.SetState(GRSentientCore.SentientCoreState.Jumping);
			this.jumpSound.Play(null);
			if (this.visualCore != null)
			{
				this.visualCore.transform.localScale = Vector3.one;
				this.visualCore.transform.localPosition = Vector3.zero;
				this.visualCore.transform.localRotation = Quaternion.identity;
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.Jumping:
		{
			if (this.debugDraw)
			{
				this.DrawJumpPath(Color.yellow);
			}
			float deltaTime = Time.deltaTime;
			Vector3 vector = base.transform.position + this.jumpVelocity * deltaTime;
			Vector3 a = this.useSurfaceNormalForGravityDirection ? (-this.surfaceNormal) : Vector3.down;
			this.jumpVelocity += a * (this.jumpGravityAccel * deltaTime);
			float magnitude = this.jumpVelocity.magnitude;
			if (magnitude > this.maxSpeed && this.maxSpeed > 0f)
			{
				this.jumpVelocity *= this.maxSpeed / magnitude;
			}
			float magnitude2 = (vector - base.transform.position).magnitude;
			Vector3 vector2 = (magnitude2 > 0.001f) ? ((vector - base.transform.position) / magnitude2) : Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(base.transform.position, vector2), this.radius, out raycastHit, magnitude2, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector = base.transform.position + vector2 * raycastHit.distance;
				this.surfaceNormal = raycastHit.normal;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
				this.landSound.Play(null);
			}
			base.transform.position = vector;
			return;
		}
		case GRSentientCore.SentientCoreState.Held:
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer != null)
			{
				grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
			}
			this.isPlayingAlert = false;
			return;
		}
		case GRSentientCore.SentientCoreState.HeldAlert:
			if (!this.isPlayingAlert)
			{
				this.isPlayingAlert = true;
				this.alertEnemiesSound.Play(null);
				GRNoiseEventManager.instance.AddNoiseEvent(base.transform.position, this.alertNoiseEventMagnitude, this.enemyAlertDuration);
			}
			if (Time.time - this.localStateStartTime > this.enemyAlertDuration)
			{
				this.SetState(GRSentientCore.SentientCoreState.Held);
				return;
			}
			break;
		case GRSentientCore.SentientCoreState.AttachedToPlayer:
		{
			GRPlayer grplayer2 = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
			if (grplayer2 != null)
			{
				grplayer2.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.TimeChaosExposure, Time.deltaTime);
				return;
			}
			break;
		}
		case GRSentientCore.SentientCoreState.Dropped:
		{
			float deltaTime2 = Time.deltaTime;
			Vector3 vector3 = base.transform.position + this.rb.linearVelocity * deltaTime2;
			float magnitude3 = (vector3 - base.transform.position).magnitude;
			Vector3 vector4 = (magnitude3 > 0.001f) ? ((vector3 - base.transform.position) / magnitude3) : Vector3.zero;
			RaycastHit raycastHit2;
			if (Physics.SphereCast(new Ray(base.transform.position, vector4), this.radius, out raycastHit2, magnitude3, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector3 = base.transform.position + vector4 * raycastHit2.distance;
				this.surfaceNormal = raycastHit2.normal;
				base.transform.position = vector3;
				this.rb.isKinematic = true;
				this.SetState(GRSentientCore.SentientCoreState.Awake);
			}
			break;
		}
		}
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x0011AF38 File Offset: 0x00119138
	private void SetState(GRSentientCore.SentientCoreState nextState)
	{
		if (this.localState != nextState)
		{
			this.localState = nextState;
			this.localStateStartTime = Time.time;
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, (long)nextState);
			}
		}
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x0011AF88 File Offset: 0x00119188
	public void PerformJump(Vector3 startPos, Vector3 normal, Vector3 direction, double jumpNetworkTime)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!base.enabled || this.IsSleeping())
		{
			this.WakeUp();
		}
		base.transform.position = startPos;
		float num = Mathf.Clamp((float)(jumpNetworkTime - PhotonNetwork.Time), 0f, this.jumpAnticipationTime);
		this.jumpStartTime = Time.time + num;
		this.jumpDirection = direction;
		this.jumpDirection.Normalize();
		this.jumpStartPosition = startPos;
		this.surfaceNormal = normal;
		this.jumpVelocity = this.jumpDirection * this.jumpSpeed;
		this.SetState(GRSentientCore.SentientCoreState.JumpAnticipation);
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x0011B024 File Offset: 0x00119224
	private void DrawJumpPath(Color pathColor)
	{
		DebugUtil.DrawLine(this.jumpStartPosition, this.jumpStartPosition + this.surfaceNormal * 0.15f, Color.cyan, true);
		float num = 0.016666f;
		int num2 = 100;
		Vector3 vector = this.jumpStartPosition;
		Vector3 a = this.jumpDirection * this.jumpSpeed;
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector2 = vector + a * num;
			a += -this.surfaceNormal * (this.jumpGravityAccel * num);
			float magnitude = (vector2 - vector).magnitude;
			Vector3 direction = (magnitude > 0.001f) ? ((vector2 - vector) / magnitude) : Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.SphereCast(new Ray(vector, direction), this.radius, out raycastHit, magnitude, GTPlayer.Instance.locomotionEnabledLayers.value, QueryTriggerInteraction.Ignore))
			{
				vector2 = raycastHit.point;
				DebugUtil.DrawLine(vector, vector2, pathColor, true);
				DebugUtil.DrawLine(vector2, vector2 + raycastHit.normal * 0.15f, Color.cyan, true);
				DebugUtil.DrawSphere(raycastHit.point, 0.1f, 12, 12, pathColor, true, DebugUtil.Style.Wireframe);
				return;
			}
			DebugUtil.DrawLine(vector, vector2, pathColor, true);
			vector = vector2;
		}
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x0011B180 File Offset: 0x00119380
	public void AuthorityInitiateJump()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		Vector3 insideUnitSphere = Random.insideUnitSphere;
		if (Vector3.Dot(insideUnitSphere, this.surfaceNormal) > 0.99f)
		{
			insideUnitSphere = new Vector3(this.surfaceNormal.y, this.surfaceNormal.z, this.surfaceNormal.x);
		}
		float num = Random.Range(this.jumpAngleMinMax.x, this.jumpAngleMinMax.y);
		Vector3 direction = Quaternion.AngleAxis(90f - num, Vector3.Cross(this.surfaceNormal, insideUnitSphere)) * this.surfaceNormal;
		direction.Normalize();
		this.SetState(GRSentientCore.SentientCoreState.JumpInitiated);
		this.gameEntity.manager.ghostReactorManager.RequestSentientCorePerformJump(this.gameEntity, base.transform.position, this.surfaceNormal, direction, this.jumpAnticipationTime);
	}

	// Token: 0x040042EA RID: 17130
	public GameEntity gameEntity;

	// Token: 0x040042EB RID: 17131
	public Vector2 jumpAngleMinMax = new Vector2(30f, 60f);

	// Token: 0x040042EC RID: 17132
	public float jumpSpeed = 3f;

	// Token: 0x040042ED RID: 17133
	public float jumpGravityAccel = 10f;

	// Token: 0x040042EE RID: 17134
	public float maxSpeed = 5f;

	// Token: 0x040042EF RID: 17135
	public float radius = 0.14f;

	// Token: 0x040042F0 RID: 17136
	public float jumpAnticipationTime = 1f;

	// Token: 0x040042F1 RID: 17137
	public float jumpCooldownTime = 2f;

	// Token: 0x040042F2 RID: 17138
	public bool useSurfaceNormalForGravityDirection = true;

	// Token: 0x040042F3 RID: 17139
	public Vector2 timeRangeBetweenAlerts = new Vector2(7f, 12f);

	// Token: 0x040042F4 RID: 17140
	public float timeUntilFirstAlert = 0.5f;

	// Token: 0x040042F5 RID: 17141
	public float alertNoiseEventMagnitude = 1f;

	// Token: 0x040042F6 RID: 17142
	public AbilitySound jumpSound;

	// Token: 0x040042F7 RID: 17143
	public AbilitySound landSound;

	// Token: 0x040042F8 RID: 17144
	public AbilitySound alertEnemiesSound;

	// Token: 0x040042F9 RID: 17145
	public float wakeupRadius = 3f;

	// Token: 0x040042FA RID: 17146
	public bool debugDraw;

	// Token: 0x040042FB RID: 17147
	public Transform visualCore;

	// Token: 0x040042FC RID: 17148
	public ParticleSystem trailFX;

	// Token: 0x040042FD RID: 17149
	private Vector3 surfaceNormal = Vector3.up;

	// Token: 0x040042FE RID: 17150
	private Vector3 jumpDirection = Vector3.up;

	// Token: 0x040042FF RID: 17151
	private Vector3 jumpStartPosition;

	// Token: 0x04004300 RID: 17152
	private Vector3 jumpVelocity;

	// Token: 0x04004301 RID: 17153
	private float jumpStartTime;

	// Token: 0x04004302 RID: 17154
	private Rigidbody rb;

	// Token: 0x04004303 RID: 17155
	private float timeUntilNextAlert = 7f;

	// Token: 0x04004304 RID: 17156
	private float enemyAlertDuration = 1f;

	// Token: 0x04004305 RID: 17157
	private bool isPlayingAlert;

	// Token: 0x04004306 RID: 17158
	private bool sleepRequested;

	// Token: 0x04004307 RID: 17159
	[ReadOnly]
	public GRSentientCore.SentientCoreState localState = GRSentientCore.SentientCoreState.Awake;

	// Token: 0x04004308 RID: 17160
	private float localStateStartTime;

	// Token: 0x020007DB RID: 2011
	public enum SentientCoreState
	{
		// Token: 0x0400430A RID: 17162
		Asleep,
		// Token: 0x0400430B RID: 17163
		Awake,
		// Token: 0x0400430C RID: 17164
		JumpInitiated,
		// Token: 0x0400430D RID: 17165
		JumpAnticipation,
		// Token: 0x0400430E RID: 17166
		Jumping,
		// Token: 0x0400430F RID: 17167
		Held,
		// Token: 0x04004310 RID: 17168
		HeldAlert,
		// Token: 0x04004311 RID: 17169
		AttachedToPlayer,
		// Token: 0x04004312 RID: 17170
		Dropped
	}
}
