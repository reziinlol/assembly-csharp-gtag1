using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012B2 RID: 4786
	[RequireComponent(typeof(TransferrableObject))]
	public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x060077BE RID: 30654 RVA: 0x00274498 File Offset: 0x00272698
		// (set) Token: 0x060077BF RID: 30655 RVA: 0x002744A0 File Offset: 0x002726A0
		public bool TickRunning { get; set; }

		// Token: 0x060077C0 RID: 30656 RVA: 0x002744A9 File Offset: 0x002726A9
		private void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x060077C1 RID: 30657 RVA: 0x002744B8 File Offset: 0x002726B8
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent += this.TriggerEntered;
			this.state = VenusFlyTrapHoldable.VenusState.Open;
			this.localRotA = this.lipA.transform.localRotation;
			this.localRotB = this.lipB.transform.localRotation;
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnTriggerEvent;
			}
		}

		// Token: 0x060077C2 RID: 30658 RVA: 0x002745D0 File Offset: 0x002727D0
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
			this.triggerEventNotifier.TriggerEnterEvent -= this.TriggerEntered;
			if (this._events != null)
			{
				this._events.Activate -= this.OnTriggerEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060077C3 RID: 30659 RVA: 0x0027463C File Offset: 0x0027283C
		public void Tick()
		{
			if (this.transferrableObject.InHand() && this.audioSource && !this.audioSource.isPlaying && this.flyLoopingAudio != null)
			{
				this.audioSource.clip = this.flyLoopingAudio;
				this.audioSource.GTPlay();
			}
			if (!this.transferrableObject.InHand() && this.audioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed && Time.time - this.closedStartedTime >= this.closedDuration)
			{
				this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
				if (this.audioSource && this.openingAudio != null)
				{
					this.audioSource.GTPlayOneShot(this.openingAudio, 1f);
				}
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
			{
				this.SmoothRotation(true);
				return;
			}
			if (this.state == VenusFlyTrapHoldable.VenusState.Opening)
			{
				this.SmoothRotation(false);
			}
		}

		// Token: 0x060077C4 RID: 30660 RVA: 0x0027474C File Offset: 0x0027294C
		private void SmoothRotation(bool isClosing)
		{
			if (isClosing)
			{
				Quaternion quaternion = Quaternion.Euler(this.targetRotationB);
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, quaternion, Time.deltaTime * this.speed);
				Quaternion quaternion2 = Quaternion.Euler(this.targetRotationA);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, quaternion2, Time.deltaTime * this.speed);
				if (Quaternion.Angle(this.lipB.transform.localRotation, quaternion) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, quaternion2) < 1f)
				{
					this.lipB.transform.localRotation = quaternion;
					this.lipA.transform.localRotation = quaternion2;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
					return;
				}
			}
			else
			{
				this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, Time.deltaTime * this.speed / 2f);
				this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, Time.deltaTime * this.speed / 2f);
				if (Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) < 1f && Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) < 1f)
				{
					this.lipB.transform.localRotation = this.localRotB;
					this.lipA.transform.localRotation = this.localRotA;
					this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
				}
			}
		}

		// Token: 0x060077C5 RID: 30661 RVA: 0x00274936 File Offset: 0x00272B36
		private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
		{
			this.state = newState;
			if (this.state == VenusFlyTrapHoldable.VenusState.Closed)
			{
				this.closedStartedTime = Time.time;
			}
		}

		// Token: 0x060077C6 RID: 30662 RVA: 0x00274954 File Offset: 0x00272B54
		private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
		{
			if (this.state != VenusFlyTrapHoldable.VenusState.Open)
			{
				return;
			}
			if (!other.gameObject.IsOnLayer(this.layers))
			{
				return;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(Array.Empty<object>());
			}
			this.OnTriggerLocal();
			GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
			if (componentInChildren == null)
			{
				return;
			}
			GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x060077C7 RID: 30663 RVA: 0x002749EF File Offset: 0x00272BEF
		private void OnTriggerEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnTriggerEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.OnTriggerLocal();
		}

		// Token: 0x060077C8 RID: 30664 RVA: 0x00274A1B File Offset: 0x00272C1B
		private void OnTriggerLocal()
		{
			this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
			if (this.audioSource && this.closingAudio != null)
			{
				this.audioSource.GTPlayOneShot(this.closingAudio, 1f);
			}
		}

		// Token: 0x04008A63 RID: 35427
		[SerializeField]
		private GameObject lipA;

		// Token: 0x04008A64 RID: 35428
		[SerializeField]
		private GameObject lipB;

		// Token: 0x04008A65 RID: 35429
		[SerializeField]
		private Vector3 targetRotationA;

		// Token: 0x04008A66 RID: 35430
		[SerializeField]
		private Vector3 targetRotationB;

		// Token: 0x04008A67 RID: 35431
		[SerializeField]
		private float closedDuration = 3f;

		// Token: 0x04008A68 RID: 35432
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04008A69 RID: 35433
		[SerializeField]
		private UnityLayer layers;

		// Token: 0x04008A6A RID: 35434
		[SerializeField]
		private TriggerEventNotifier triggerEventNotifier;

		// Token: 0x04008A6B RID: 35435
		[SerializeField]
		private float hapticStrength = 0.5f;

		// Token: 0x04008A6C RID: 35436
		[SerializeField]
		private float hapticDuration = 0.1f;

		// Token: 0x04008A6D RID: 35437
		[SerializeField]
		private GameObject bug;

		// Token: 0x04008A6E RID: 35438
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04008A6F RID: 35439
		[SerializeField]
		private AudioClip closingAudio;

		// Token: 0x04008A70 RID: 35440
		[SerializeField]
		private AudioClip openingAudio;

		// Token: 0x04008A71 RID: 35441
		[SerializeField]
		private AudioClip flyLoopingAudio;

		// Token: 0x04008A72 RID: 35442
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04008A73 RID: 35443
		private float closedStartedTime;

		// Token: 0x04008A74 RID: 35444
		private VenusFlyTrapHoldable.VenusState state;

		// Token: 0x04008A75 RID: 35445
		private Quaternion localRotA;

		// Token: 0x04008A76 RID: 35446
		private Quaternion localRotB;

		// Token: 0x04008A77 RID: 35447
		private RubberDuckEvents _events;

		// Token: 0x04008A78 RID: 35448
		private TransferrableObject transferrableObject;

		// Token: 0x020012B3 RID: 4787
		private enum VenusState
		{
			// Token: 0x04008A7B RID: 35451
			Closed,
			// Token: 0x04008A7C RID: 35452
			Open,
			// Token: 0x04008A7D RID: 35453
			Closing,
			// Token: 0x04008A7E RID: 35454
			Opening
		}
	}
}
