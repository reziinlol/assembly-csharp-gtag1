using System;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001249 RID: 4681
	public class ChickenSword : MonoBehaviour
	{
		// Token: 0x0600755A RID: 30042 RVA: 0x00267190 File Offset: 0x00265390
		private void Awake()
		{
			this.lastHitTime = float.PositiveInfinity;
			this.SwitchState(ChickenSword.SwordState.Ready);
		}

		// Token: 0x0600755B RID: 30043 RVA: 0x002671A4 File Offset: 0x002653A4
		internal void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? ((this.transferrableObject.myRig.creator != null) ? this.transferrableObject.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnReachedLastTransformationStep;
			}
		}

		// Token: 0x0600755C RID: 30044 RVA: 0x00267288 File Offset: 0x00265488
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnReachedLastTransformationStep;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600755D RID: 30045 RVA: 0x002672D8 File Offset: 0x002654D8
		private void Update()
		{
			ChickenSword.SwordState swordState = this.currentState;
			if (swordState != ChickenSword.SwordState.Ready)
			{
				if (swordState != ChickenSword.SwordState.Deflated)
				{
					return;
				}
				if (Time.time - this.lastHitTime > this.rechargeCooldown)
				{
					this.lastHitTime = float.PositiveInfinity;
					this.SwitchState(ChickenSword.SwordState.Ready);
					UnityEvent onRechargedShared = this.OnRechargedShared;
					if (onRechargedShared != null)
					{
						onRechargedShared.Invoke();
					}
					if (this.transferrableObject && this.transferrableObject.IsMyItem())
					{
						UnityEvent<bool> onRechargedLocal = this.OnRechargedLocal;
						if (onRechargedLocal == null)
						{
							return;
						}
						onRechargedLocal.Invoke(this.transferrableObject.InLeftHand());
					}
				}
			}
			else if (this.hitReceievd)
			{
				this.hitReceievd = false;
				this.lastHitTime = Time.time;
				this.SwitchState(ChickenSword.SwordState.Deflated);
				UnityEvent onDeflatedShared = this.OnDeflatedShared;
				if (onDeflatedShared != null)
				{
					onDeflatedShared.Invoke();
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					UnityEvent<bool> onDeflatedLocal = this.OnDeflatedLocal;
					if (onDeflatedLocal == null)
					{
						return;
					}
					onDeflatedLocal.Invoke(this.transferrableObject.InLeftHand());
					return;
				}
			}
		}

		// Token: 0x0600755E RID: 30046 RVA: 0x002673D4 File Offset: 0x002655D4
		public void OnHitTargetSync(VRRig playerRig)
		{
			if (this.velocityTracker == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (this.currentState == ChickenSword.SwordState.Ready && averageVelocity.magnitude > this.hitVelocityThreshold)
			{
				this.hitReceievd = true;
				UnityEvent<VRRig> onHitTargetShared = this.OnHitTargetShared;
				if (onHitTargetShared != null)
				{
					onHitTargetShared.Invoke(playerRig);
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					bool arg = this.transferrableObject.InLeftHand();
					UnityEvent<bool> onHitTargetLocal = this.OnHitTargetLocal;
					if (onHitTargetLocal != null)
					{
						onHitTargetLocal.Invoke(arg);
					}
				}
				if (this.cosmeticSwapper != null && playerRig == GorillaTagger.Instance.offlineVRRig && this.cosmeticSwapper.GetCurrentStepIndex(playerRig) >= this.cosmeticSwapper.GetNumberOfSteps() && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseAll(Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600755F RID: 30047 RVA: 0x002674E8 File Offset: 0x002656E8
		private void OnReachedLastTransformationStep(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnReachedLastTransformationStep");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && rigContainer.Rig.IsPositionInRange(base.transform.position, 6f))
			{
				UnityEvent<VRRig> onReachedLastTransformationStepShared = this.OnReachedLastTransformationStepShared;
				if (onReachedLastTransformationStepShared == null)
				{
					return;
				}
				onReachedLastTransformationStepShared.Invoke(rigContainer.Rig);
			}
		}

		// Token: 0x06007560 RID: 30048 RVA: 0x00267570 File Offset: 0x00265770
		private void SwitchState(ChickenSword.SwordState newState)
		{
			this.currentState = newState;
		}

		// Token: 0x040086ED RID: 34541
		[SerializeField]
		private float rechargeCooldown;

		// Token: 0x040086EE RID: 34542
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x040086EF RID: 34543
		[SerializeField]
		private float hitVelocityThreshold;

		// Token: 0x040086F0 RID: 34544
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x040086F1 RID: 34545
		[SerializeField]
		private CosmeticSwapper cosmeticSwapper;

		// Token: 0x040086F2 RID: 34546
		[Space]
		[Space]
		public UnityEvent OnDeflatedShared;

		// Token: 0x040086F3 RID: 34547
		public UnityEvent<bool> OnDeflatedLocal;

		// Token: 0x040086F4 RID: 34548
		public UnityEvent OnRechargedShared;

		// Token: 0x040086F5 RID: 34549
		public UnityEvent<bool> OnRechargedLocal;

		// Token: 0x040086F6 RID: 34550
		public UnityEvent<VRRig> OnHitTargetShared;

		// Token: 0x040086F7 RID: 34551
		public UnityEvent<bool> OnHitTargetLocal;

		// Token: 0x040086F8 RID: 34552
		public UnityEvent<VRRig> OnReachedLastTransformationStepShared;

		// Token: 0x040086F9 RID: 34553
		private float lastHitTime;

		// Token: 0x040086FA RID: 34554
		private ChickenSword.SwordState currentState;

		// Token: 0x040086FB RID: 34555
		private bool hitReceievd;

		// Token: 0x040086FC RID: 34556
		private RubberDuckEvents _events;

		// Token: 0x040086FD RID: 34557
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x0200124A RID: 4682
		private enum SwordState
		{
			// Token: 0x040086FF RID: 34559
			Ready,
			// Token: 0x04008700 RID: 34560
			Deflated
		}
	}
}
