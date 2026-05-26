using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001260 RID: 4704
	public class ControllerButtonEvent : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x060075E0 RID: 30176 RVA: 0x002699DE File Offset: 0x00267BDE
		// (set) Token: 0x060075E1 RID: 30177 RVA: 0x002699E6 File Offset: 0x00267BE6
		public bool IsSpawned { get; set; }

		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x060075E2 RID: 30178 RVA: 0x002699EF File Offset: 0x00267BEF
		// (set) Token: 0x060075E3 RID: 30179 RVA: 0x002699F7 File Offset: 0x00267BF7
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060075E4 RID: 30180 RVA: 0x00269A00 File Offset: 0x00267C00
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x060075E5 RID: 30181 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void OnDespawn()
		{
		}

		// Token: 0x060075E6 RID: 30182 RVA: 0x00269A09 File Offset: 0x00267C09
		private bool IsMyItem()
		{
			return this.myRig != null && this.myRig.isOfflineVRRig;
		}

		// Token: 0x060075E7 RID: 30183 RVA: 0x00269A26 File Offset: 0x00267C26
		private void Awake()
		{
			this.triggerLastValue = 0f;
			this.gripLastValue = 0f;
			this.primaryLastValue = false;
			this.secondaryLastValue = false;
			this.frameCounter = 0;
		}

		// Token: 0x060075E8 RID: 30184 RVA: 0x00269A54 File Offset: 0x00267C54
		public void LateUpdate()
		{
			if (!this.IsMyItem())
			{
				return;
			}
			XRNode node = this.inLeftHand ? XRNode.LeftHand : XRNode.RightHand;
			switch (this.buttonType)
			{
			case ControllerButtonEvent.ButtonType.trigger:
			{
				float num = ControllerInputPoller.TriggerFloat(node);
				if (num > this.triggerValue)
				{
					this.frameCounter++;
				}
				if (num > this.triggerValue && this.triggerLastValue < this.triggerValue)
				{
					UnityEvent<bool, float> unityEvent = this.onButtonPressed;
					if (unityEvent != null)
					{
						unityEvent.Invoke(this.inLeftHand, num);
					}
				}
				else if (num <= this.triggerReleaseValue && this.triggerLastValue > this.triggerReleaseValue)
				{
					UnityEvent<bool, float> unityEvent2 = this.onButtonReleased;
					if (unityEvent2 != null)
					{
						unityEvent2.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				else if (num > this.triggerValue && this.triggerLastValue >= this.triggerValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent3 = this.onButtonPressStayed;
					if (unityEvent3 != null)
					{
						unityEvent3.Invoke(this.inLeftHand, num);
					}
					this.frameCounter = 0;
				}
				this.triggerLastValue = num;
				return;
			}
			case ControllerButtonEvent.ButtonType.primary:
			{
				bool flag = ControllerInputPoller.PrimaryButtonPress(node);
				if (flag)
				{
					this.frameCounter++;
				}
				if (flag && !this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent4 = this.onButtonPressed;
					if (unityEvent4 != null)
					{
						unityEvent4.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag && this.primaryLastValue)
				{
					UnityEvent<bool, float> unityEvent5 = this.onButtonReleased;
					if (unityEvent5 != null)
					{
						unityEvent5.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent6 = this.onButtonPressStayed;
					if (unityEvent6 != null)
					{
						unityEvent6.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.primaryLastValue = flag;
				return;
			}
			case ControllerButtonEvent.ButtonType.secondary:
			{
				bool flag2 = ControllerInputPoller.SecondaryButtonPress(node);
				if (flag2)
				{
					this.frameCounter++;
				}
				if (flag2 && !this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent7 = this.onButtonPressed;
					if (unityEvent7 != null)
					{
						unityEvent7.Invoke(this.inLeftHand, 1f);
					}
				}
				else if (!flag2 && this.secondaryLastValue)
				{
					UnityEvent<bool, float> unityEvent8 = this.onButtonReleased;
					if (unityEvent8 != null)
					{
						unityEvent8.Invoke(this.inLeftHand, 0f);
					}
					this.frameCounter = 0;
				}
				else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent9 = this.onButtonPressStayed;
					if (unityEvent9 != null)
					{
						unityEvent9.Invoke(this.inLeftHand, 1f);
					}
					this.frameCounter = 0;
				}
				this.secondaryLastValue = flag2;
				return;
			}
			case ControllerButtonEvent.ButtonType.grip:
			{
				float num2 = ControllerInputPoller.GripFloat(node);
				if (num2 > this.gripValue)
				{
					this.frameCounter++;
				}
				if (num2 > this.gripValue && this.gripLastValue < this.gripValue)
				{
					UnityEvent<bool, float> unityEvent10 = this.onButtonPressed;
					if (unityEvent10 != null)
					{
						unityEvent10.Invoke(this.inLeftHand, num2);
					}
				}
				else if (num2 <= this.gripReleaseValue && this.gripLastValue > this.gripReleaseValue)
				{
					UnityEvent<bool, float> unityEvent11 = this.onButtonReleased;
					if (unityEvent11 != null)
					{
						unityEvent11.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				else if (num2 > this.gripValue && this.gripLastValue >= this.gripValue && this.frameCounter % this.frameInterval == 0)
				{
					UnityEvent<bool, float> unityEvent12 = this.onButtonPressStayed;
					if (unityEvent12 != null)
					{
						unityEvent12.Invoke(this.inLeftHand, num2);
					}
					this.frameCounter = 0;
				}
				this.gripLastValue = num2;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x040087A1 RID: 34721
		[SerializeField]
		private float gripValue = 0.75f;

		// Token: 0x040087A2 RID: 34722
		[SerializeField]
		private float gripReleaseValue = 0.01f;

		// Token: 0x040087A3 RID: 34723
		[SerializeField]
		private float triggerValue = 0.75f;

		// Token: 0x040087A4 RID: 34724
		[SerializeField]
		private float triggerReleaseValue = 0.01f;

		// Token: 0x040087A5 RID: 34725
		[SerializeField]
		private ControllerButtonEvent.ButtonType buttonType;

		// Token: 0x040087A6 RID: 34726
		[Tooltip("How many frames should pass to trigger a press stayed button")]
		[SerializeField]
		private int frameInterval = 20;

		// Token: 0x040087A7 RID: 34727
		public UnityEvent<bool, float> onButtonPressed;

		// Token: 0x040087A8 RID: 34728
		public UnityEvent<bool, float> onButtonReleased;

		// Token: 0x040087A9 RID: 34729
		public UnityEvent<bool, float> onButtonPressStayed;

		// Token: 0x040087AA RID: 34730
		private float triggerLastValue;

		// Token: 0x040087AB RID: 34731
		private float gripLastValue;

		// Token: 0x040087AC RID: 34732
		private bool primaryLastValue;

		// Token: 0x040087AD RID: 34733
		private bool secondaryLastValue;

		// Token: 0x040087AE RID: 34734
		private int frameCounter;

		// Token: 0x040087AF RID: 34735
		private bool inLeftHand;

		// Token: 0x040087B0 RID: 34736
		private VRRig myRig;

		// Token: 0x02001261 RID: 4705
		private enum ButtonType
		{
			// Token: 0x040087B4 RID: 34740
			trigger,
			// Token: 0x040087B5 RID: 34741
			primary,
			// Token: 0x040087B6 RID: 34742
			secondary,
			// Token: 0x040087B7 RID: 34743
			grip
		}
	}
}
