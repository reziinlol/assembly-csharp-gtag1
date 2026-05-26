using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200124B RID: 4683
	public class CloserCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x06007562 RID: 30050 RVA: 0x00267598 File Offset: 0x00265798
		// (set) Token: 0x06007563 RID: 30051 RVA: 0x002675A0 File Offset: 0x002657A0
		public bool TickRunning { get; set; }

		// Token: 0x06007564 RID: 30052 RVA: 0x002675AC File Offset: 0x002657AC
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.localRotA = this.sideA.transform.localRotation;
			this.localRotB = this.sideB.transform.localRotation;
			this.fingerValue = 0f;
			this.UpdateState(CloserCosmetic.State.Opening);
		}

		// Token: 0x06007565 RID: 30053 RVA: 0x000412B3 File Offset: 0x0003F4B3
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06007566 RID: 30054 RVA: 0x00267600 File Offset: 0x00265800
		public void Tick()
		{
			switch (this.currentState)
			{
			case CloserCosmetic.State.Closing:
				this.Closing();
				return;
			case CloserCosmetic.State.Opening:
				this.Opening();
				break;
			case CloserCosmetic.State.None:
				break;
			default:
				return;
			}
		}

		// Token: 0x06007567 RID: 30055 RVA: 0x00267634 File Offset: 0x00265834
		public void Close(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Closing);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06007568 RID: 30056 RVA: 0x00267644 File Offset: 0x00265844
		public void Open(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Opening);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06007569 RID: 30057 RVA: 0x00267654 File Offset: 0x00265854
		private void Closing()
		{
			float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion b = Quaternion.Euler(this.maxRotationB);
			Quaternion quaternion = Quaternion.Slerp(this.localRotB, b, t);
			this.sideB.transform.localRotation = quaternion;
			Quaternion b2 = Quaternion.Euler(this.maxRotationA);
			Quaternion quaternion2 = Quaternion.Slerp(this.localRotA, b2, t);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x0600756A RID: 30058 RVA: 0x00267718 File Offset: 0x00265918
		private void Opening()
		{
			float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
			Quaternion quaternion = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, t);
			this.sideB.transform.localRotation = quaternion;
			Quaternion quaternion2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, t);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x0600756B RID: 30059 RVA: 0x002677DD File Offset: 0x002659DD
		private void UpdateState(CloserCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04008701 RID: 34561
		[SerializeField]
		private GameObject sideA;

		// Token: 0x04008702 RID: 34562
		[SerializeField]
		private GameObject sideB;

		// Token: 0x04008703 RID: 34563
		[SerializeField]
		private Vector3 maxRotationA;

		// Token: 0x04008704 RID: 34564
		[SerializeField]
		private Vector3 maxRotationB;

		// Token: 0x04008705 RID: 34565
		[SerializeField]
		private bool useFingerFlexValueAsStrength;

		// Token: 0x04008706 RID: 34566
		private Quaternion localRotA;

		// Token: 0x04008707 RID: 34567
		private Quaternion localRotB;

		// Token: 0x04008708 RID: 34568
		private CloserCosmetic.State currentState;

		// Token: 0x04008709 RID: 34569
		private float fingerValue;

		// Token: 0x0200124C RID: 4684
		private enum State
		{
			// Token: 0x0400870C RID: 34572
			Closing,
			// Token: 0x0400870D RID: 34573
			Opening,
			// Token: 0x0400870E RID: 34574
			None
		}
	}
}
