using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTagScripts
{
	// Token: 0x02000F26 RID: 3878
	public class RandomProjectileThrowable : MonoBehaviour
	{
		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x060060D7 RID: 24791 RVA: 0x001F34C0 File Offset: 0x001F16C0
		// (set) Token: 0x060060D8 RID: 24792 RVA: 0x001F34C8 File Offset: 0x001F16C8
		public float TimeEnabled { get; private set; }

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x060060D9 RID: 24793 RVA: 0x001F34D1 File Offset: 0x001F16D1
		// (set) Token: 0x060060DA RID: 24794 RVA: 0x001F34D9 File Offset: 0x001F16D9
		public bool ForceDestroy { get; set; }

		// Token: 0x060060DB RID: 24795 RVA: 0x001F34E2 File Offset: 0x001F16E2
		private void OnEnable()
		{
			this.TimeEnabled = Time.time;
			this.currentProjectile = this.projectilePrefab;
		}

		// Token: 0x060060DC RID: 24796 RVA: 0x001F34FB File Offset: 0x001F16FB
		private void OnDisable()
		{
			this.ForceDestroy = false;
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x001F3504 File Offset: 0x001F1704
		public void ForceDestroyThrowable()
		{
			this.ForceDestroy = true;
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x001F350D File Offset: 0x001F170D
		public void UpdateProjectilePrefab()
		{
			this.currentProjectile = this.alternativeProjectilePrefab;
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x001F351B File Offset: 0x001F171B
		public GameObject GetProjectilePrefab()
		{
			return this.currentProjectile;
		}

		// Token: 0x060060E0 RID: 24800 RVA: 0x001F3524 File Offset: 0x001F1724
		private void OnTriggerEnter(Collider other)
		{
			if (!this.destroyOnTrigger)
			{
				return;
			}
			if (other.gameObject.layer == LayerMask.NameToLayer(this.triggerTag))
			{
				if (this.audioSource && this.triggerClip)
				{
					this.audioSource.GTPlayOneShot(this.triggerClip, 1f);
				}
				if (GorillaTagger.hasInstance && other == GorillaTagger.Instance.headCollider)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
				UnityEvent onDestroyed = this.OnDestroyed;
				if (onDestroyed != null)
				{
					onDestroyed.Invoke();
				}
				this.DestroyProjectile();
			}
		}

		// Token: 0x060060E1 RID: 24801 RVA: 0x001F35BD File Offset: 0x001F17BD
		public void DestroyProjectile()
		{
			base.StartCoroutine(this.DestroyProjectileCoroutine(0.25f));
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x001F35D1 File Offset: 0x001F17D1
		private IEnumerator DestroyProjectileCoroutine(float delay)
		{
			yield return new WaitForSeconds(delay);
			UnityAction<bool> onDestroyRandomProjectile = this.OnDestroyRandomProjectile;
			if (onDestroyRandomProjectile != null)
			{
				onDestroyRandomProjectile(false);
			}
			yield break;
		}

		// Token: 0x04006F6C RID: 28524
		public GameObject projectilePrefab;

		// Token: 0x04006F6D RID: 28525
		[Tooltip("Use for a different/updated version of the projectile if needed.")]
		public GameObject alternativeProjectilePrefab;

		// Token: 0x04006F6E RID: 28526
		[FormerlySerializedAs("weightedChance")]
		[Range(0f, 1f)]
		public float spawnChance = 1f;

		// Token: 0x04006F6F RID: 28527
		[Tooltip("(Optional) name broadcast by PlayerGameEvents when the local player eats this projectile")]
		public string interactEventName;

		// Token: 0x04006F70 RID: 28528
		[Tooltip("Requires a collider")]
		public bool destroyOnTrigger = true;

		// Token: 0x04006F71 RID: 28529
		public string triggerTag = "Gorilla Head";

		// Token: 0x04006F72 RID: 28530
		[FormerlySerializedAs("onMoveToHead")]
		public UnityEvent OnDestroyed;

		// Token: 0x04006F73 RID: 28531
		public AudioSource audioSource;

		// Token: 0x04006F74 RID: 28532
		public AudioClip triggerClip;

		// Token: 0x04006F75 RID: 28533
		[Tooltip("Immediately destroys after the release")]
		public bool destroyAfterRelease;

		// Token: 0x04006F76 RID: 28534
		[Tooltip("Set a timer to destroy after X seconds is passed and the object is not thrown yet")]
		[FormerlySerializedAs("destroyAfterSeconds")]
		public float autoDestroyAfterSeconds = -1f;

		// Token: 0x04006F77 RID: 28535
		[Tooltip("If checked, any amount of passed time will be deducted from the lifetime of the slingshot projectile when thrownShould be less than or equal to lifetime of the slingshot projectile")]
		public bool moveOverPassedLifeTime;

		// Token: 0x04006F7A RID: 28538
		public UnityAction<bool> OnDestroyRandomProjectile;

		// Token: 0x04006F7B RID: 28539
		private GameObject currentProjectile;
	}
}
