using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	// Token: 0x02000F67 RID: 3943
	public class HandScanner : ObservableBehavior, IClickable
	{
		// Token: 0x0600623A RID: 25146 RVA: 0x001FB20C File Offset: 0x001F940C
		protected override void ObservableSliceUpdate()
		{
			if (this.scanningRig == null)
			{
				return;
			}
			if (Time.time - this.scanStart > this.scanTime)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanSuccess;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		// Token: 0x0600623B RID: 25147 RVA: 0x001FB25F File Offset: 0x001F945F
		protected override void OnBecameObservable()
		{
			UnityEvent unityEvent = this.onHandScanInRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x0600623C RID: 25148 RVA: 0x001FB271 File Offset: 0x001F9471
		protected override void OnLostObservable()
		{
			UnityEvent unityEvent = this.onHandScanOutOfRange;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x0600623D RID: 25149 RVA: 0x001FB284 File Offset: 0x001F9484
		private void OnTriggerEnter(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent.isLocal)
			{
				this.scanningRig = componentInParent;
				this.scanStart = Time.time;
				UnityEvent<NetPlayer> unityEvent = this.onHandScanStart;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.scanningRig.creator);
			}
		}

		// Token: 0x0600623E RID: 25150 RVA: 0x001FB2E8 File Offset: 0x001F94E8
		private void OnTriggerExit(Collider other)
		{
			SIScannableHand component = other.GetComponent<SIScannableHand>();
			if (component == null)
			{
				return;
			}
			VRRig componentInParent = component.GetComponentInParent<VRRig>();
			if (componentInParent != null && componentInParent == this.scanningRig && componentInParent.isLocal)
			{
				UnityEvent<NetPlayer> unityEvent = this.onHandScanAbort;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this.scanningRig.creator);
				}
				this.scanningRig = null;
			}
		}

		// Token: 0x0600623F RID: 25151 RVA: 0x001FB34F File Offset: 0x001F954F
		public void Click(bool leftHand = false)
		{
			UnityEvent<NetPlayer> unityEvent = this.onHandScanStart;
			if (unityEvent != null)
			{
				unityEvent.Invoke(VRRig.LocalRig.creator);
			}
			UnityEvent<NetPlayer> unityEvent2 = this.onHandScanSuccess;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke(VRRig.LocalRig.creator);
		}

		// Token: 0x04007108 RID: 28936
		[SerializeField]
		private float scanTime = 1f;

		// Token: 0x04007109 RID: 28937
		public UnityEvent<NetPlayer> onHandScanStart;

		// Token: 0x0400710A RID: 28938
		public UnityEvent<NetPlayer> onHandScanAbort;

		// Token: 0x0400710B RID: 28939
		public UnityEvent<NetPlayer> onHandScanSuccess;

		// Token: 0x0400710C RID: 28940
		public UnityEvent onHandScanInRange;

		// Token: 0x0400710D RID: 28941
		public UnityEvent onHandScanOutOfRange;

		// Token: 0x0400710E RID: 28942
		private VRRig scanningRig;

		// Token: 0x0400710F RID: 28943
		private float scanStart;
	}
}
