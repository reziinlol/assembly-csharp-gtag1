using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012AF RID: 4783
	public class TransferrableObjectHoldablePart_Pin : TransferrableObjectHoldablePart
	{
		// Token: 0x060077B0 RID: 30640 RVA: 0x0027410D File Offset: 0x0027230D
		protected void OnEnable()
		{
			UnityEvent onEnableHoldable = this.OnEnableHoldable;
			if (onEnableHoldable == null)
			{
				return;
			}
			onEnableHoldable.Invoke();
		}

		// Token: 0x060077B1 RID: 30641 RVA: 0x00274120 File Offset: 0x00272320
		protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
		{
			if (rig.isOfflineVRRig)
			{
				Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
				if (GTPlayer.Instance.GetInteractPointVelocityTracker(isHeldLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude > this.breakStrengthThreshold || (controllerTransform.position - this.pin.transform.position).IsLongerThan(this.maxHandSnapDistance))
				{
					this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
					UnityEvent onBreak = this.OnBreak;
					if (onBreak != null)
					{
						onBreak.Invoke();
					}
					if (this.transferrableParentObject && this.transferrableParentObject.IsMyItem())
					{
						UnityEvent onBreakLocal = this.OnBreakLocal;
						if (onBreakLocal == null)
						{
							return;
						}
						onBreakLocal.Invoke();
					}
					return;
				}
				controllerTransform.position = this.pin.position;
			}
		}

		// Token: 0x04008A4D RID: 35405
		[SerializeField]
		private float breakStrengthThreshold = 0.8f;

		// Token: 0x04008A4E RID: 35406
		[SerializeField]
		private float maxHandSnapDistance = 0.5f;

		// Token: 0x04008A4F RID: 35407
		[SerializeField]
		private Transform pin;

		// Token: 0x04008A50 RID: 35408
		public UnityEvent OnBreak;

		// Token: 0x04008A51 RID: 35409
		public UnityEvent OnBreakLocal;

		// Token: 0x04008A52 RID: 35410
		public UnityEvent OnEnableHoldable;
	}
}
