using System;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FB1 RID: 4017
	public class BuilderProjectile : MonoBehaviour
	{
		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06006454 RID: 25684 RVA: 0x002054F0 File Offset: 0x002036F0
		// (set) Token: 0x06006455 RID: 25685 RVA: 0x002054F8 File Offset: 0x002036F8
		public Vector3 launchPosition { get; private set; }

		// Token: 0x140000A8 RID: 168
		// (add) Token: 0x06006456 RID: 25686 RVA: 0x00205504 File Offset: 0x00203704
		// (remove) Token: 0x06006457 RID: 25687 RVA: 0x0020553C File Offset: 0x0020373C
		public event BuilderProjectile.ProjectileImpactEvent OnImpact;

		// Token: 0x06006458 RID: 25688 RVA: 0x00205574 File Offset: 0x00203774
		public void Launch(Vector3 position, Vector3 velocity, BuilderProjectileLauncher sourceObject, int projectileCount, float scale, int timeStamp)
		{
			this.particleLaunched = true;
			this.timeCreated = Time.time;
			this.projectileSource = sourceObject;
			float num = (NetworkSystem.Instance.ServerTimestamp - timeStamp) / 1000f;
			if (num >= this.lifeTime)
			{
				this.Deactivate();
				return;
			}
			this.timeCreated -= num;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, Vector3.up);
			float f = 0.017453292f * Vector3.Angle(vector, velocity);
			float num2 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * 9.8f;
			Vector3 b = num * Mathf.Cos(f) * vector;
			float d = velocity.z * num * Mathf.Sin(f) - 0.5f * num2 * num * num;
			this.launchPosition = position + b + d * Vector3.down;
			Transform transform = base.transform;
			transform.position = position;
			transform.localScale = Vector3.one * scale;
			base.GetComponent<Collider>().contactOffset = 0.01f * scale;
			RigidbodyWaterInteraction component = base.GetComponent<RigidbodyWaterInteraction>();
			if (component != null)
			{
				component.objectRadiusForWaterCollision = 0.02f * scale;
			}
			this.projectileRigidbody.useGravity = false;
			Vector3 vector2 = this.projectileRigidbody.mass * this.gravityMultiplier * ((scale < 1f) ? scale : 1f) * Physics.gravity;
			this.forceComponent.force = vector2;
			this.projectileRigidbody.linearVelocity = velocity + num * vector2;
			this.projectileId = projectileCount;
			this.projectileRigidbody.position = position;
			this.projectileSource.RegisterProjectile(this);
		}

		// Token: 0x06006459 RID: 25689 RVA: 0x00205735 File Offset: 0x00203935
		protected void Awake()
		{
			this.projectileRigidbody = base.GetComponent<Rigidbody>();
			this.forceComponent = base.GetComponent<ConstantForce>();
			this.initialScale = base.transform.localScale.x;
		}

		// Token: 0x0600645A RID: 25690 RVA: 0x00205768 File Offset: 0x00203968
		public void Deactivate()
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			this.projectileRigidbody.useGravity = true;
			this.forceComponent.force = Vector3.zero;
			this.OnImpact = null;
			this.aoeKnockbackConfig = null;
			this.impactSoundVolumeOverride = null;
			this.impactSoundPitchOverride = null;
			this.impactEffectScaleMultiplier = 1f;
			this.gravityMultiplier = 1f;
			ObjectPools.instance.Destroy(base.gameObject);
		}

		// Token: 0x0600645B RID: 25691 RVA: 0x00205800 File Offset: 0x00203A00
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			Vector3 position2 = position + normal * this.impactEffectOffset;
			GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
			Vector3 localScale = base.transform.localScale;
			gameObject.transform.localScale = localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
			SurfaceImpactFX component = gameObject.GetComponent<SurfaceImpactFX>();
			if (component != null)
			{
				component.SetScale(localScale.x * this.impactEffectScaleMultiplier);
			}
			SoundBankPlayer component2 = gameObject.GetComponent<SoundBankPlayer>();
			if (component2 != null && !component2.playOnEnable)
			{
				component2.Play(this.impactSoundVolumeOverride, this.impactSoundPitchOverride);
			}
		}

		// Token: 0x0600645C RID: 25692 RVA: 0x002058A8 File Offset: 0x00203AA8
		public void ApplyHitKnockback(Vector3 hitNormal)
		{
			if (this.aoeKnockbackConfig != null && this.aoeKnockbackConfig.Value.applyAOEKnockback)
			{
				Vector3 a = Vector3.ProjectOnPlane(hitNormal, Vector3.up);
				a.Normalize();
				Vector3 direction = 0.75f * a + 0.25f * Vector3.up;
				direction.Normalize();
				GTPlayer instance = GTPlayer.Instance;
				instance.ApplyKnockback(direction, this.aoeKnockbackConfig.Value.knockbackVelocity, instance.scale < 0.9f);
			}
		}

		// Token: 0x0600645D RID: 25693 RVA: 0x00205938 File Offset: 0x00203B38
		private void OnEnable()
		{
			this.timeCreated = 0f;
			this.particleLaunched = false;
		}

		// Token: 0x0600645E RID: 25694 RVA: 0x0020594C File Offset: 0x00203B4C
		protected void OnDisable()
		{
			this.particleLaunched = false;
			if (this.projectileSource != null)
			{
				this.projectileSource.UnRegisterProjectile(this);
			}
			this.projectileSource = null;
		}

		// Token: 0x0600645F RID: 25695 RVA: 0x00205978 File Offset: 0x00203B78
		public void UpdateProjectile()
		{
			if (this.particleLaunched)
			{
				if (Time.time > this.timeCreated + this.lifeTime)
				{
					this.Deactivate();
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
		}

		// Token: 0x06006460 RID: 25696 RVA: 0x002059F4 File Offset: 0x00203BF4
		private void OnCollisionEnter(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x00205AAC File Offset: 0x00203CAC
		protected void OnCollisionStay(Collision other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			BuilderPieceCollider component = other.transform.GetComponent<BuilderPieceCollider>();
			if (component != null && component.piece.gameObject.Equals(this.projectileSource.gameObject))
			{
				return;
			}
			ContactPoint contact = other.GetContact(0);
			if (other.collider.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ApplyHitKnockback(-1f * contact.normal);
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, contact.point, contact.normal);
			BuilderProjectile.ProjectileImpactEvent onImpact = this.OnImpact;
			if (onImpact != null)
			{
				onImpact(this, contact.point, null);
			}
			this.Deactivate();
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x00205B64 File Offset: 0x00203D64
		protected void OnTriggerEnter(Collider other)
		{
			if (!this.particleLaunched)
			{
				return;
			}
			if (!NetworkSystem.Instance.InRoom || GorillaGameManager.instance == null)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaTagCollider))
			{
				return;
			}
			VRRig componentInParent = other.GetComponentInParent<VRRig>();
			NetPlayer netPlayer = (componentInParent != null) ? componentInParent.creator : null;
			if (netPlayer == null)
			{
				return;
			}
			if (netPlayer.IsLocal)
			{
				return;
			}
			this.SpawnImpactEffect(this.surfaceImpactEffectPrefab, base.transform.position, Vector3.up);
			this.Deactivate();
		}

		// Token: 0x0400731F RID: 29471
		public BuilderProjectileLauncher projectileSource;

		// Token: 0x04007320 RID: 29472
		[Tooltip("Rotates to point along the Y axis after spawn.")]
		public GameObject surfaceImpactEffectPrefab;

		// Token: 0x04007321 RID: 29473
		[Tooltip("Distance from the surface that the particle should spawn.")]
		private float impactEffectOffset;

		// Token: 0x04007322 RID: 29474
		public float lifeTime = 20f;

		// Token: 0x04007323 RID: 29475
		public bool faceDirectionOfTravel = true;

		// Token: 0x04007324 RID: 29476
		private bool particleLaunched;

		// Token: 0x04007325 RID: 29477
		private float timeCreated;

		// Token: 0x04007327 RID: 29479
		private Rigidbody projectileRigidbody;

		// Token: 0x04007328 RID: 29480
		public int projectileId;

		// Token: 0x04007329 RID: 29481
		private float initialScale;

		// Token: 0x0400732A RID: 29482
		private Vector3 previousPosition;

		// Token: 0x0400732B RID: 29483
		[HideInInspector]
		public SlingshotProjectile.AOEKnockbackConfig? aoeKnockbackConfig;

		// Token: 0x0400732C RID: 29484
		[HideInInspector]
		public float? impactSoundVolumeOverride;

		// Token: 0x0400732D RID: 29485
		[HideInInspector]
		public float? impactSoundPitchOverride;

		// Token: 0x0400732E RID: 29486
		[HideInInspector]
		public float impactEffectScaleMultiplier = 1f;

		// Token: 0x0400732F RID: 29487
		[HideInInspector]
		public float gravityMultiplier = 1f;

		// Token: 0x04007330 RID: 29488
		private ConstantForce forceComponent;

		// Token: 0x02000FB2 RID: 4018
		// (Invoke) Token: 0x06006465 RID: 25701
		public delegate void ProjectileImpactEvent(BuilderProjectile projectile, Vector3 impactPos, NetPlayer hitPlayer);
	}
}
