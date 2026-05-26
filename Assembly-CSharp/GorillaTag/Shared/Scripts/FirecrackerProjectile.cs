using System;
using System.Collections;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Shared.Scripts
{
	// Token: 0x020011DC RID: 4572
	public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
	{
		// Token: 0x17000B09 RID: 2825
		// (get) Token: 0x060072EB RID: 29419 RVA: 0x0025637B File Offset: 0x0025457B
		// (set) Token: 0x060072EC RID: 29420 RVA: 0x00256383 File Offset: 0x00254583
		public bool TickRunning { get; set; }

		// Token: 0x060072ED RID: 29421 RVA: 0x0025638C File Offset: 0x0025458C
		public void Tick()
		{
			if (Time.time - this.timeCreated > this.forceBackToPoolAfterSec || Time.time - this.timeExploded > this.explosionTime)
			{
				UnityEvent<FirecrackerProjectile> onDetonationComplete = this.OnDetonationComplete;
				if (onDetonationComplete == null)
				{
					return;
				}
				onDetonationComplete.Invoke(this);
			}
		}

		// Token: 0x060072EE RID: 29422 RVA: 0x002563C8 File Offset: 0x002545C8
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.m_timer.Start();
			this.timeExploded = float.PositiveInfinity;
			this.timeCreated = float.PositiveInfinity;
			this.collisionEntered = false;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(true);
			}
			UnityEvent onEnableObject = this.OnEnableObject;
			if (onEnableObject == null)
			{
				return;
			}
			onEnableObject.Invoke();
		}

		// Token: 0x060072EF RID: 29423 RVA: 0x0025642C File Offset: 0x0025462C
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.m_timer.Stop();
			if (this.useTransferrableObjectState)
			{
				UnityEvent onResetProjectileState = this.OnResetProjectileState;
				if (onResetProjectileState == null)
				{
					return;
				}
				onResetProjectileState.Invoke();
			}
		}

		// Token: 0x060072F0 RID: 29424 RVA: 0x00256457 File Offset: 0x00254657
		private void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.audioSource = base.GetComponent<AudioSource>();
			this.m_timer.callback = new Action(this.Detonate);
		}

		// Token: 0x060072F1 RID: 29425 RVA: 0x00256488 File Offset: 0x00254688
		private void Detonate()
		{
			this.m_timer.Stop();
			this.timeExploded = Time.time;
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x060072F2 RID: 29426 RVA: 0x002564C0 File Offset: 0x002546C0
		internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
		{
			if (!this.useTransferrableObjectState)
			{
				return;
			}
			if (syncType != TransferrableObject.SyncOptions.Bool)
			{
				if (syncType != TransferrableObject.SyncOptions.Int)
				{
					return;
				}
				UnityEvent<int> onItemStateIntChanged = this.OnItemStateIntChanged;
				if (onItemStateIntChanged == null)
				{
					return;
				}
				onItemStateIntChanged.Invoke(state);
				return;
			}
			else
			{
				bool flag = (state & 1) != 0;
				bool flag2 = (state & 2) != 0;
				bool flag3 = (state & 4) != 0;
				bool flag4 = (state & 8) != 0;
				if (flag)
				{
					UnityEvent onItemStateBoolATrue = this.OnItemStateBoolATrue;
					if (onItemStateBoolATrue != null)
					{
						onItemStateBoolATrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolAFalse = this.OnItemStateBoolAFalse;
					if (onItemStateBoolAFalse != null)
					{
						onItemStateBoolAFalse.Invoke();
					}
				}
				if (flag2)
				{
					UnityEvent onItemStateBoolBTrue = this.OnItemStateBoolBTrue;
					if (onItemStateBoolBTrue != null)
					{
						onItemStateBoolBTrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolBFalse = this.OnItemStateBoolBFalse;
					if (onItemStateBoolBFalse != null)
					{
						onItemStateBoolBFalse.Invoke();
					}
				}
				if (flag3)
				{
					UnityEvent onItemStateBoolCTrue = this.OnItemStateBoolCTrue;
					if (onItemStateBoolCTrue != null)
					{
						onItemStateBoolCTrue.Invoke();
					}
				}
				else
				{
					UnityEvent onItemStateBoolCFalse = this.OnItemStateBoolCFalse;
					if (onItemStateBoolCFalse != null)
					{
						onItemStateBoolCFalse.Invoke();
					}
				}
				if (flag4)
				{
					UnityEvent onItemStateBoolDTrue = this.OnItemStateBoolDTrue;
					if (onItemStateBoolDTrue == null)
					{
						return;
					}
					onItemStateBoolDTrue.Invoke();
					return;
				}
				else
				{
					UnityEvent onItemStateBoolDFalse = this.OnItemStateBoolDFalse;
					if (onItemStateBoolDFalse == null)
					{
						return;
					}
					onItemStateBoolDFalse.Invoke();
					return;
				}
			}
		}

		// Token: 0x060072F3 RID: 29427 RVA: 0x002565A8 File Offset: 0x002547A8
		public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
		{
			base.transform.position = startPosition;
			base.transform.rotation = startRotation;
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
			this.rb.linearVelocity = velocity;
		}

		// Token: 0x060072F4 RID: 29428 RVA: 0x002565F8 File Offset: 0x002547F8
		private void OnCollisionEnter(Collision other)
		{
			if (this.collisionEntered)
			{
				return;
			}
			Vector3 point = other.contacts[0].point;
			Vector3 normal = other.contacts[0].normal;
			UnityEvent<FirecrackerProjectile, Vector3> onCollisionEntered = this.OnCollisionEntered;
			if (onCollisionEntered != null)
			{
				onCollisionEntered.Invoke(this, normal);
			}
			if (this.sizzleDuration > 0f)
			{
				base.StartCoroutine(this.Sizzle(point, normal));
			}
			else
			{
				UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
				if (onDetonationStart != null)
				{
					onDetonationStart.Invoke(this, point);
				}
				this.Detonate(point, normal);
			}
			this.collisionEntered = true;
		}

		// Token: 0x060072F5 RID: 29429 RVA: 0x00256685 File Offset: 0x00254885
		private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
		{
			if (this.audioSource && this.sizzleAudioClip != null)
			{
				this.audioSource.GTPlayOneShot(this.sizzleAudioClip, 1f);
			}
			yield return new WaitForSeconds(this.sizzleDuration);
			UnityEvent<FirecrackerProjectile, Vector3> onDetonationStart = this.OnDetonationStart;
			if (onDetonationStart != null)
			{
				onDetonationStart.Invoke(this, contactPoint);
			}
			this.Detonate(contactPoint, normal);
			yield break;
		}

		// Token: 0x060072F6 RID: 29430 RVA: 0x002566A4 File Offset: 0x002548A4
		private void Detonate(Vector3 contactPoint, Vector3 normal)
		{
			this.timeExploded = Time.time;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint, true);
			gameObject.transform.up = normal;
			gameObject.transform.position = base.transform.position;
			SoundBankPlayer soundBankPlayer;
			if (gameObject.TryGetComponent<SoundBankPlayer>(out soundBankPlayer) && soundBankPlayer.soundBank)
			{
				soundBankPlayer.Play();
			}
			if (this.disableWhenHit)
			{
				this.disableWhenHit.SetActive(false);
			}
			this.collisionEntered = false;
		}

		// Token: 0x04008354 RID: 33620
		[SerializeField]
		private GameObject explosionEffect;

		// Token: 0x04008355 RID: 33621
		[SerializeField]
		private float forceBackToPoolAfterSec = 20f;

		// Token: 0x04008356 RID: 33622
		[SerializeField]
		private float explosionTime = 5f;

		// Token: 0x04008357 RID: 33623
		[SerializeField]
		private GameObject disableWhenHit;

		// Token: 0x04008358 RID: 33624
		[SerializeField]
		private float sizzleDuration;

		// Token: 0x04008359 RID: 33625
		[SerializeField]
		private AudioClip sizzleAudioClip;

		// Token: 0x0400835A RID: 33626
		[Space]
		public UnityEvent OnEnableObject;

		// Token: 0x0400835B RID: 33627
		public UnityEvent<FirecrackerProjectile, Vector3> OnCollisionEntered;

		// Token: 0x0400835C RID: 33628
		public UnityEvent<FirecrackerProjectile, Vector3> OnDetonationStart;

		// Token: 0x0400835D RID: 33629
		public UnityEvent<FirecrackerProjectile> OnDetonationComplete;

		// Token: 0x0400835E RID: 33630
		private Rigidbody rb;

		// Token: 0x0400835F RID: 33631
		private float timeCreated = float.PositiveInfinity;

		// Token: 0x04008360 RID: 33632
		private float timeExploded = float.PositiveInfinity;

		// Token: 0x04008361 RID: 33633
		private AudioSource audioSource;

		// Token: 0x04008362 RID: 33634
		private TickSystemTimer m_timer = new TickSystemTimer(40f);

		// Token: 0x04008363 RID: 33635
		private bool collisionEntered;

		// Token: 0x04008364 RID: 33636
		[SerializeField]
		private bool useTransferrableObjectState;

		// Token: 0x04008365 RID: 33637
		[SerializeField]
		protected UnityEvent OnResetProjectileState;

		// Token: 0x04008366 RID: 33638
		[SerializeField]
		protected string boolADebugName;

		// Token: 0x04008367 RID: 33639
		[SerializeField]
		protected UnityEvent OnItemStateBoolATrue;

		// Token: 0x04008368 RID: 33640
		[SerializeField]
		protected UnityEvent OnItemStateBoolAFalse;

		// Token: 0x04008369 RID: 33641
		[SerializeField]
		protected string boolBDebugName;

		// Token: 0x0400836A RID: 33642
		[SerializeField]
		protected UnityEvent OnItemStateBoolBTrue;

		// Token: 0x0400836B RID: 33643
		[SerializeField]
		protected UnityEvent OnItemStateBoolBFalse;

		// Token: 0x0400836C RID: 33644
		[SerializeField]
		protected string boolCDebugName;

		// Token: 0x0400836D RID: 33645
		[SerializeField]
		protected UnityEvent OnItemStateBoolCTrue;

		// Token: 0x0400836E RID: 33646
		[SerializeField]
		protected UnityEvent OnItemStateBoolCFalse;

		// Token: 0x0400836F RID: 33647
		[SerializeField]
		protected string boolDDebugName;

		// Token: 0x04008370 RID: 33648
		[SerializeField]
		protected UnityEvent OnItemStateBoolDTrue;

		// Token: 0x04008371 RID: 33649
		[SerializeField]
		protected UnityEvent OnItemStateBoolDFalse;

		// Token: 0x04008372 RID: 33650
		[SerializeField]
		protected UnityEvent<int> OnItemStateIntChanged;
	}
}
