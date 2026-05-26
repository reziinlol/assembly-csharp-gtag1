using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.Gravity;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B9 RID: 1209
public class SlingshotProjectile : MonoBehaviour
{
	// Token: 0x17000321 RID: 801
	// (get) Token: 0x06001D7E RID: 7550 RVA: 0x0009F578 File Offset: 0x0009D778
	// (set) Token: 0x06001D7F RID: 7551 RVA: 0x0009F580 File Offset: 0x0009D780
	public Vector3 launchPosition { get; private set; }

	// Token: 0x1400003C RID: 60
	// (add) Token: 0x06001D80 RID: 7552 RVA: 0x0009F58C File Offset: 0x0009D78C
	// (remove) Token: 0x06001D81 RID: 7553 RVA: 0x0009F5C4 File Offset: 0x0009D7C4
	public event SlingshotProjectile.ProjectileImpactEvent OnImpact;

	// Token: 0x06001D82 RID: 7554 RVA: 0x0009F5FC File Offset: 0x0009D7FC
	public void Launch(Vector3 position, Vector3 velocity, NetPlayer player, bool blueTeam, bool orangeTeam, int projectileCount, float scale, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (this.launchSoundBankPlayer != null)
		{
			this.launchSoundBankPlayer.Play();
		}
		this.particleLaunched = true;
		this.timeCreated = Time.time;
		this.launchPosition = position;
		Transform transform = base.transform;
		transform.position = position;
		transform.localScale = Vector3.one * scale;
		base.GetComponent<Collider>().contactOffset = 0.01f * scale;
		RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
		if (component != null)
		{
			component.objectRadiusForWaterCollision = 0.02f * scale;
		}
		this.gravityController.GravityMultiplier = this.gravityMultiplier * ((scale < 1f) ? scale : 1f);
		this.projectileRigidbody.isKinematic = false;
		this.projectileRigidbody.useGravity = false;
		this.projectileRigidbody.linearVelocity = velocity;
		this.projectileOwner = player;
		this.myProjectileCount = projectileCount;
		this.projectileRigidbody.position = position;
		this.ApplyTeamModelAndColor(blueTeam, orangeTeam, shouldOverrideColor, overrideColor);
		this.remainingLifeTime = this.lifeTime;
		if (this.useForwardForce && this.forceComponent)
		{
			this.forceComponent.enabled = true;
			this.forceComponent.force = this.projectileRigidbody.linearVelocity.normalized * this.forwardForceMultiplier;
		}
		this.isSettled = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.gravityController.SetPersonalGravityDirection(-rigContainer.Rig.transform.up);
		}
		UnityEvent<NetPlayer> onLaunch = this.OnLaunch;
		if (onLaunch == null)
		{
			return;
		}
		onLaunch.Invoke(this.projectileOwner);
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x0009F7A0 File Offset: 0x0009D9A0
	protected void Awake()
	{
		if (this.playerImpactEffectPrefab == null)
		{
			this.playerImpactEffectPrefab = this.surfaceImpactEffectPrefab;
		}
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.forceComponent = base.GetComponent<ConstantForce>();
		this.initialScale = base.transform.localScale.x;
		this.matPropBlock = new MaterialPropertyBlock();
		this.spawnWorldEffects = base.GetComponent<SpawnWorldEffects>();
		this.remainingLifeTime = this.lifeTime;
		this.gravityController = base.GetComponent<MonkeGravityController>();
		if (this.gravityController == null)
		{
			this.gravityController = base.gameObject.AddComponent<MonkeGravityController>();
		}
	}

	// Token: 0x06001D84 RID: 7556 RVA: 0x0009F844 File Offset: 0x0009DA44
	public void Deactivate()
	{
		base.transform.localScale = Vector3.one * this.initialScale;
		this.projectileRigidbody.useGravity = true;
		if (this.forceComponent)
		{
			this.forceComponent.force = Vector3.zero;
		}
		this.OnImpact = null;
		this.aoeKnockbackConfig = null;
		this.impactSoundVolumeOverride = null;
		this.impactSoundPitchOverride = null;
		this.impactEffectScaleMultiplier = 1f;
		this.projectileRigidbody.isKinematic = false;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x06001D85 RID: 7557 RVA: 0x0009F8E8 File Offset: 0x0009DAE8
	private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
	{
		if (prefab == null)
		{
			return;
		}
		Vector3 position2 = position + normal * this.impactEffectOffset;
		GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
		Vector3 localScale = base.transform.localScale;
		gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
		gameObject.transform.up = normal;
		GorillaColorizableBase component = gameObject.GetComponent<GorillaColorizableBase>();
		if (component != null)
		{
			component.SetColor(this.teamColor);
		}
		SurfaceImpactFX component2 = gameObject.GetComponent<SurfaceImpactFX>();
		if (component2 != null)
		{
			component2.SetScale(localScale.x * this.impactEffectScaleMultiplier);
		}
		SoundBankPlayer component3 = gameObject.GetComponent<SoundBankPlayer>();
		if (component3 != null && !component3.playOnEnable)
		{
			component3.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
		}
		if (this.spawnWorldEffects != null)
		{
			this.spawnWorldEffects.RequestSpawn(position, normal);
		}
		UnityEvent<Vector3> onImapctEvent = this.OnImapctEvent;
		if (onImapctEvent == null)
		{
			return;
		}
		onImapctEvent.Invoke(position);
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x0009F9E8 File Offset: 0x0009DBE8
	public void CheckForAOEKnockback(Vector3 impactPosition, float impactSpeed)
	{
		if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
		{
			Vector3 a = GTPlayer.Instance.HeadCenterPosition - impactPosition;
			if (a.sqrMagnitude < this.aoeKnockbackConfig.Value.aeoOuterRadius * this.aoeKnockbackConfig.Value.aeoOuterRadius)
			{
				float magnitude = a.magnitude;
				Vector3 direction = (magnitude > 0.001f) ? (a / magnitude) : Vector3.up;
				float num = Mathf.InverseLerp(this.aoeKnockbackConfig.Value.aeoOuterRadius, this.aoeKnockbackConfig.Value.aeoInnerRadius, magnitude);
				float num2 = Mathf.InverseLerp(0f, this.aoeKnockbackConfig.Value.impactVelocityThreshold, impactSpeed);
				GTPlayer.Instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity * num * num2, false);
				this.impactEffectScaleMultiplier = Mathf.Lerp(1f, this.impactEffectScaleMultiplier, num2);
				if (this.impactSoundVolumeOverride != null)
				{
					this.impactSoundVolumeOverride = new float?(Mathf.Lerp(this.impactSoundVolumeOverride.Value * 0.5f, this.impactSoundVolumeOverride.Value, num2));
				}
				float num3 = Mathf.Lerp(this.aoeKnockbackConfig.Value.aeoInnerRadius, this.aoeKnockbackConfig.Value.aeoOuterRadius, 0.25f);
				if (this.aoeKnockbackConfig.Value.playerProximityEffect != PlayerEffect.NONE && a.sqrMagnitude < num3 * num3)
				{
					RoomSystem.SendPlayerEffect(PlayerEffect.SNOWBALL_IMPACT, NetworkSystem.Instance.LocalPlayer);
				}
			}
		}
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x0009FB8C File Offset: 0x0009DD8C
	public void ApplyTeamModelAndColor(bool blueTeam, bool orangeTeam, bool shouldOverrideColor = false, Color overrideColor = default(Color))
	{
		if (shouldOverrideColor)
		{
			this.teamColor = overrideColor;
		}
		else
		{
			this.teamColor = (blueTeam ? this.blueColor : (orangeTeam ? this.orangeColor : this.defaultColor));
		}
		this.blueBall.enabled = blueTeam;
		this.orangeBall.enabled = orangeTeam;
		this.defaultBall.enabled = (!blueTeam && !orangeTeam);
		this.teamRenderer = (blueTeam ? this.blueBall : (orangeTeam ? this.orangeBall : this.defaultBall));
		this.ApplyColor(this.teamRenderer, (this.colorizeBalls || shouldOverrideColor) ? this.teamColor : Color.white);
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x0009FC3A File Offset: 0x0009DE3A
	protected void OnEnable()
	{
		this.timeCreated = 0f;
		this.particleLaunched = false;
		SlingshotProjectileManager.RegisterSP(this);
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x0009FC54 File Offset: 0x0009DE54
	protected void OnDisable()
	{
		this.particleLaunched = false;
		SlingshotProjectileManager.UnregisterSP(this);
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x0009FC64 File Offset: 0x0009DE64
	public void InvokeUpdate()
	{
		if (this.particleLaunched || this.dontDestroyOnHit)
		{
			if (Time.time > this.timeCreated + this.GetRemainingLifeTime())
			{
				this.DestroyAfterRelease();
			}
			if (this.faceDirectionOfTravel)
			{
				Transform transform = base.transform;
				Vector3 position = transform.position;
				Vector3 forward = position - this.previousPosition;
				transform.rotation = ((forward.sqrMagnitude > 0f) ? Quaternion.LookRotation(forward) : transform.rotation);
				this.previousPosition = position;
			}
		}
		if (this.dontDestroyOnHit)
		{
			this.SettleProjectile();
		}
	}

	// Token: 0x06001D8B RID: 7563 RVA: 0x0009FCF5 File Offset: 0x0009DEF5
	public void DestroyAfterRelease()
	{
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
		this.Deactivate();
	}

	// Token: 0x06001D8C RID: 7564 RVA: 0x0009FD19 File Offset: 0x0009DF19
	public float GetRemainingLifeTime()
	{
		return this.remainingLifeTime;
	}

	// Token: 0x06001D8D RID: 7565 RVA: 0x0009FD21 File Offset: 0x0009DF21
	public void UpdateRemainingLifeTime(float newLifeTime)
	{
		this.remainingLifeTime = newLifeTime;
	}

	// Token: 0x06001D8E RID: 7566 RVA: 0x0009FD2C File Offset: 0x0009DF2C
	public float GetDistanceTraveled()
	{
		return (base.transform.position - this.launchPosition).magnitude;
	}

	// Token: 0x06001D8F RID: 7567 RVA: 0x0009FD58 File Offset: 0x0009DF58
	private void SettleProjectile()
	{
		if (!this.isSettled)
		{
			int value = this.floorLayerMask.value;
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f, value, QueryTriggerInteraction.Ignore) && Vector3.Angle(raycastHit.normal, Vector3.up) < 40f)
			{
				if (this.forceComponent)
				{
					this.forceComponent.force = Vector3.zero;
				}
				this.projectileRigidbody.angularVelocity = Vector3.zero;
				this.projectileRigidbody.linearVelocity = Vector3.zero;
				this.projectileRigidbody.isKinematic = true;
				base.transform.position = raycastHit.point + Vector3.up * this.placementOffset;
				this.isSettled = true;
				return;
			}
		}
		else if (this.keepRotationUpright)
		{
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.up, Vector3.up).normalized, Vector3.up);
			base.transform.rotation = rotation;
		}
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x0009FE70 File Offset: 0x0009E070
	protected void OnCollisionEnter(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.collider.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeHit(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001D91 RID: 7569 RVA: 0x0009FF08 File Offset: 0x0009E108
	protected void OnCollisionStay(Collision collision)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		if (this.dontDestroyOnHit)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (collision.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeCollisionStay(this, collision);
		}
		ContactPoint contact = collision.GetContact(0);
		this.CheckForAOEKnockback(contact.point, collision.relativeVelocity.magnitude);
		this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
		SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
		if (onImpact != null)
		{
			onImpact(this, contact.point, null);
		}
		this.Deactivate();
	}

	// Token: 0x06001D92 RID: 7570 RVA: 0x0009FF9C File Offset: 0x0009E19C
	protected void OnTriggerExit(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerExit(this, other);
		}
	}

	// Token: 0x06001D93 RID: 7571 RVA: 0x0009FFCC File Offset: 0x0009E1CC
	protected void OnTriggerEnter(Collider other)
	{
		if (!this.particleLaunched)
		{
			return;
		}
		SlingshotProjectileHitNotifier slingshotProjectileHitNotifier;
		if (other.gameObject.TryGetComponent<SlingshotProjectileHitNotifier>(out slingshotProjectileHitNotifier))
		{
			slingshotProjectileHitNotifier.InvokeTriggerEnter(this, other);
		}
		if (this.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaSlingshotCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			SlingshotProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, base.transform.position, netPlayer);
			}
			if (NetworkSystem.Instance.LocalPlayer == netPlayer)
			{
				return;
			}
			if (component && !component.LocalCanHit(NetworkSystem.Instance.LocalPlayer, netPlayer))
			{
				return;
			}
			if (component && GameMode.ActiveNetworkHandler)
			{
				GameMode.ActiveNetworkHandler.SendRPC("RPC_ReportSlingshotHit", false, new object[]
				{
					(netPlayer as PunNetPlayer).PlayerRef,
					base.transform.position,
					this.myProjectileCount
				});
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
			if (this.m_sendNetworkedImpact)
			{
				RoomSystem.SendImpactEffect(base.transform.position, this.teamColor.r, this.teamColor.g, this.teamColor.b, this.teamColor.a, this.myProjectileCount);
			}
			this.Deactivate();
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		VRRig arg;
		if (attachedRigidbody.IsNotNull() && attachedRigidbody.gameObject.TryGetComponent<VRRig>(out arg))
		{
			UnityEvent<VRRig> onHitPlayer = this.OnHitPlayer;
			if (onHitPlayer == null)
			{
				return;
			}
			onHitPlayer.Invoke(arg);
		}
	}

	// Token: 0x06001D94 RID: 7572 RVA: 0x000A0199 File Offset: 0x0009E399
	private void ApplyColor(Renderer rend, Color color)
	{
		if (!rend)
		{
			return;
		}
		this.matPropBlock.SetColor(ShaderProps._BaseColor, color);
		this.matPropBlock.SetColor(ShaderProps._Color, color);
		rend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x040027C9 RID: 10185
	public NetPlayer projectileOwner;

	// Token: 0x040027CA RID: 10186
	[Tooltip("Rotates to point along the Y axis after spawn.")]
	public GameObject surfaceImpactEffectPrefab;

	// Token: 0x040027CB RID: 10187
	[Tooltip("if left empty, the default player impact that is set in Room System Setting will be played")]
	public GameObject playerImpactEffectPrefab;

	// Token: 0x040027CC RID: 10188
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x040027CD RID: 10189
	[SerializeField]
	private SoundBankPlayer launchSoundBankPlayer;

	// Token: 0x040027CE RID: 10190
	[SerializeField]
	private bool dontDestroyOnHit;

	// Token: 0x040027CF RID: 10191
	[SerializeField]
	private LayerMask floorLayerMask;

	// Token: 0x040027D0 RID: 10192
	[SerializeField]
	private float placementOffset = 0.01f;

	// Token: 0x040027D1 RID: 10193
	[SerializeField]
	private bool keepRotationUpright = true;

	// Token: 0x040027D2 RID: 10194
	public float lifeTime = 20f;

	// Token: 0x040027D3 RID: 10195
	public float gravityMultiplier = 1f;

	// Token: 0x040027D4 RID: 10196
	public bool useForwardForce;

	// Token: 0x040027D5 RID: 10197
	public float forwardForceMultiplier = 0.1f;

	// Token: 0x040027D6 RID: 10198
	public Color defaultColor = Color.white;

	// Token: 0x040027D7 RID: 10199
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x040027D8 RID: 10200
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x040027D9 RID: 10201
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer defaultBall;

	// Token: 0x040027DA RID: 10202
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer orangeBall;

	// Token: 0x040027DB RID: 10203
	[Tooltip("Renderers with team specific meshes, materials, effects, etc.")]
	public Renderer blueBall;

	// Token: 0x040027DC RID: 10204
	public bool colorizeBalls;

	// Token: 0x040027DD RID: 10205
	public bool faceDirectionOfTravel = true;

	// Token: 0x040027DE RID: 10206
	private bool particleLaunched;

	// Token: 0x040027DF RID: 10207
	private float timeCreated;

	// Token: 0x040027E1 RID: 10209
	private Rigidbody projectileRigidbody;

	// Token: 0x040027E2 RID: 10210
	private Color teamColor = Color.white;

	// Token: 0x040027E3 RID: 10211
	private Renderer teamRenderer;

	// Token: 0x040027E4 RID: 10212
	public int myProjectileCount;

	// Token: 0x040027E5 RID: 10213
	private float initialScale;

	// Token: 0x040027E6 RID: 10214
	private Vector3 previousPosition;

	// Token: 0x040027E7 RID: 10215
	[HideInInspector]
	public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

	// Token: 0x040027E8 RID: 10216
	[HideInInspector]
	public float? impactSoundVolumeOverride;

	// Token: 0x040027E9 RID: 10217
	[HideInInspector]
	public float? impactSoundPitchOverride;

	// Token: 0x040027EA RID: 10218
	[HideInInspector]
	public float impactEffectScaleMultiplier = 1f;

	// Token: 0x040027EB RID: 10219
	private ConstantForce forceComponent;

	// Token: 0x040027EC RID: 10220
	public bool m_sendNetworkedImpact = true;

	// Token: 0x040027EE RID: 10222
	public UnityEvent<NetPlayer> OnLaunch;

	// Token: 0x040027EF RID: 10223
	public UnityEvent<Vector3> OnImapctEvent;

	// Token: 0x040027F0 RID: 10224
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x040027F1 RID: 10225
	private SpawnWorldEffects spawnWorldEffects;

	// Token: 0x040027F2 RID: 10226
	public UnityEvent<VRRig> OnHitPlayer;

	// Token: 0x040027F3 RID: 10227
	private float remainingLifeTime;

	// Token: 0x040027F4 RID: 10228
	private bool isSettled;

	// Token: 0x040027F5 RID: 10229
	private float distanceTraveled;

	// Token: 0x040027F6 RID: 10230
	private MonkeGravityController gravityController;

	// Token: 0x020004BA RID: 1210
	[Serializable]
	public struct AOEKnockbackConfig
	{
		// Token: 0x040027F7 RID: 10231
		public bool applyAOEKnockback;

		// Token: 0x040027F8 RID: 10232
		[Tooltip("Full knockback velocity is imparted within the inner radius")]
		public float aeoInnerRadius;

		// Token: 0x040027F9 RID: 10233
		[Tooltip("Partial knockback velocity is imparted between the inner and outer radius")]
		public float aeoOuterRadius;

		// Token: 0x040027FA RID: 10234
		public float knockbackVelocity;

		// Token: 0x040027FB RID: 10235
		[Tooltip("The required impact velocity to achieve full knockback velocity")]
		public float impactVelocityThreshold;

		// Token: 0x040027FC RID: 10236
		[SerializeField]
		public PlayerEffect playerProximityEffect;
	}

	// Token: 0x020004BB RID: 1211
	// (Invoke) Token: 0x06001D97 RID: 7575
	public delegate void ProjectileImpactEvent(SlingshotProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
}
