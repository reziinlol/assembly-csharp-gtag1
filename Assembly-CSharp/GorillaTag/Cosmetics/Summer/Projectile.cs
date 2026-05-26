using System;
using System.Collections.Generic;
using GorillaTag.Reactions;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics.Summer
{
	// Token: 0x020012BD RID: 4797
	public class Projectile : MonoBehaviour, IProjectile
	{
		// Token: 0x060077F8 RID: 30712 RVA: 0x0027582D File Offset: 0x00273A2D
		protected void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.impactEffectSpawned = false;
			this.forceComponent = base.GetComponent<ConstantForce>();
		}

		// Token: 0x060077F9 RID: 30713 RVA: 0x000028C5 File Offset: 0x00000AC5
		protected void OnEnable()
		{
		}

		// Token: 0x060077FA RID: 30714 RVA: 0x00275850 File Offset: 0x00273A50
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep)
		{
			Transform transform = base.transform;
			transform.SetPositionAndRotation(startPosition, startRotation);
			transform.localScale = Vector3.one * ownerRig.scaleFactor;
			if (this.rigidbody != null)
			{
				this.rigidbody.isKinematic = false;
				this.rigidbody.position = startPosition;
				this.rigidbody.rotation = startRotation;
				this.rigidbody.linearVelocity = velocity;
			}
			if (this.audioSource && this.launchAudio)
			{
				this.audioSource.GTPlayOneShot(this.launchAudio, 1f);
			}
			UnityEvent<float> unityEvent = this.onLaunchShared;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(chargeFrac);
		}

		// Token: 0x060077FB RID: 30715 RVA: 0x00275901 File Offset: 0x00273B01
		private bool IsTagValid(GameObject obj)
		{
			return this.collisionTags.Contains(obj.tag);
		}

		// Token: 0x060077FC RID: 30716 RVA: 0x00275914 File Offset: 0x00273B14
		private void HandleImpact(GameObject hitObject, Vector3 hitPosition, Vector3 hitNormal)
		{
			if (this.impactEffectSpawned)
			{
				return;
			}
			if (this.collisionTags.Count > 0 && !this.IsTagValid(hitObject))
			{
				return;
			}
			if ((1 << hitObject.layer & this.collisionLayerMasks) == 0)
			{
				return;
			}
			this.SpawnImpactEffect(this.impactEffect, hitPosition, hitNormal);
			if (this.impactEffect != null)
			{
				SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
				if (component != null && !component.playOnEnable)
				{
					component.Play();
				}
			}
			this.impactEffectSpawned = true;
			if (this.destroyOnCollisionEnter)
			{
				if (this.destroyDelay > 0f)
				{
					base.Invoke("DestroyProjectile", this.destroyDelay);
					return;
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060077FD RID: 30717 RVA: 0x002759D0 File Offset: 0x00273BD0
		private void GetColliderHitInfo(Collider other, out Vector3 position, out Vector3 normal)
		{
			Vector3 vector = Time.fixedDeltaTime * 2f * this.rigidbody.linearVelocity;
			Vector3 origin = base.transform.position - vector;
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(origin, vector / magnitude), out raycastHit, 2f * magnitude);
			position = raycastHit.point;
			normal = raycastHit.normal;
		}

		// Token: 0x060077FE RID: 30718 RVA: 0x00275A4C File Offset: 0x00273C4C
		private void OnCollisionEnter(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x060077FF RID: 30719 RVA: 0x00275A7C File Offset: 0x00273C7C
		private void OnCollisionStay(Collision other)
		{
			ContactPoint contact = other.GetContact(0);
			this.HandleImpact(other.gameObject, contact.point, contact.normal);
		}

		// Token: 0x06007800 RID: 30720 RVA: 0x00275AAC File Offset: 0x00273CAC
		private void OnTriggerEnter(Collider other)
		{
			Vector3 hitPosition;
			Vector3 hitNormal;
			this.GetColliderHitInfo(other, out hitPosition, out hitNormal);
			this.HandleImpact(other.gameObject, hitPosition, hitNormal);
		}

		// Token: 0x06007801 RID: 30721 RVA: 0x00275AD4 File Offset: 0x00273CD4
		private void OnTriggerStay(Collider other)
		{
			Transform transform = base.transform;
			this.HandleImpact(other.gameObject, transform.position, -transform.forward);
		}

		// Token: 0x06007802 RID: 30722 RVA: 0x00275B08 File Offset: 0x00273D08
		private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
		{
			if (prefab != null)
			{
				Vector3 position2 = position + normal * this.impactEffectOffset;
				GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position2, true);
				gameObject.transform.up = normal;
				gameObject.transform.position = position2;
			}
			this.onImpactShared.Invoke();
			if (this.spawnWorldEffects != null)
			{
				this.spawnWorldEffects.RequestSpawn(position, normal);
			}
		}

		// Token: 0x06007803 RID: 30723 RVA: 0x00275B7C File Offset: 0x00273D7C
		private void DestroyProjectile()
		{
			this.impactEffectSpawned = false;
			if (this.forceComponent)
			{
				this.forceComponent.enabled = false;
			}
			if (ObjectPools.instance.DoesPoolExist(base.gameObject))
			{
				ObjectPools.instance.Destroy(base.gameObject);
				return;
			}
			Object.Destroy(base.gameObject);
		}

		// Token: 0x04008ADC RID: 35548
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008ADD RID: 35549
		[SerializeField]
		private GameObject impactEffect;

		// Token: 0x04008ADE RID: 35550
		[SerializeField]
		private AudioClip launchAudio;

		// Token: 0x04008ADF RID: 35551
		[SerializeField]
		private LayerMask collisionLayerMasks;

		// Token: 0x04008AE0 RID: 35552
		[SerializeField]
		private List<string> collisionTags = new List<string>();

		// Token: 0x04008AE1 RID: 35553
		[SerializeField]
		private bool destroyOnCollisionEnter;

		// Token: 0x04008AE2 RID: 35554
		[SerializeField]
		private float destroyDelay = 1f;

		// Token: 0x04008AE3 RID: 35555
		[Tooltip("Distance from the surface that the particle should spawn.")]
		[SerializeField]
		private float impactEffectOffset = 0.1f;

		// Token: 0x04008AE4 RID: 35556
		[SerializeField]
		private SpawnWorldEffects spawnWorldEffects;

		// Token: 0x04008AE5 RID: 35557
		private ConstantForce forceComponent;

		// Token: 0x04008AE6 RID: 35558
		public UnityEvent<float> onLaunchShared;

		// Token: 0x04008AE7 RID: 35559
		public UnityEvent onImpactShared;

		// Token: 0x04008AE8 RID: 35560
		private bool impactEffectSpawned;

		// Token: 0x04008AE9 RID: 35561
		private Rigidbody rigidbody;
	}
}
