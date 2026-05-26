using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200127F RID: 4735
	public class FingerFlexEvent : MonoBehaviourTick
	{
		// Token: 0x060076BF RID: 30399 RVA: 0x0026F167 File Offset: 0x0026D367
		private void Awake()
		{
			this._rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
		}

		// Token: 0x060076C0 RID: 30400 RVA: 0x0026F18D File Offset: 0x0026D38D
		private bool IsMyItem()
		{
			return this._rig != null && this._rig.isOfflineVRRig;
		}

		// Token: 0x060076C1 RID: 30401 RVA: 0x0026F1AC File Offset: 0x0026D3AC
		public override void Tick()
		{
			for (int i = 0; i < this.eventListeners.Length; i++)
			{
				FingerFlexEvent.Listener listener = this.eventListeners[i];
				this.FireEvents(listener);
			}
		}

		// Token: 0x060076C2 RID: 30402 RVA: 0x0026F1DC File Offset: 0x0026D3DC
		private void FireEvents(FingerFlexEvent.Listener listener)
		{
			if (!listener.syncForEveryoneInRoom && !this.IsMyItem())
			{
				return;
			}
			bool flag = this.parentTransferable != null || this.myHeldItem != null;
			bool flag2;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag2 = (heldItem == null || heldItem.InHand());
			}
			else
			{
				flag2 = this.parentTransferable.InHand();
			}
			bool flag3 = flag2;
			if (!this.ignoreTransferable && listener.fireOnlyWhileHeld && flag && !flag3 && listener.eventType == FingerFlexEvent.EventType.OnFingerReleased)
			{
				if (listener.fingerRightLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(false, 0f);
					}
					listener.fingerRightLastValue = 0f;
				}
				if (listener.fingerLeftLastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(true, 0f);
					}
					listener.fingerLeftLastValue = 0f;
				}
			}
			if (!this.ignoreTransferable && flag && listener.fireOnlyWhileHeld && !flag3)
			{
				return;
			}
			switch (this.fingerType)
			{
			case FingerFlexEvent.FingerType.Thumb:
			{
				float calcT = this._rig.leftThumb.calcT;
				float calcT2 = this._rig.rightThumb.calcT;
				this.FireEvents(listener, calcT, calcT2);
				return;
			}
			case FingerFlexEvent.FingerType.Index:
			{
				float calcT3 = this._rig.leftIndex.calcT;
				float calcT4 = this._rig.rightIndex.calcT;
				this.FireEvents(listener, calcT3, calcT4);
				return;
			}
			case FingerFlexEvent.FingerType.Middle:
			{
				float calcT5 = this._rig.leftMiddle.calcT;
				float calcT6 = this._rig.rightMiddle.calcT;
				this.FireEvents(listener, calcT5, calcT6);
				return;
			}
			case FingerFlexEvent.FingerType.IndexAndMiddleMin:
			{
				float leftFinger = Mathf.Min(this._rig.leftIndex.calcT, this._rig.leftMiddle.calcT);
				float rightFinger = Mathf.Min(this._rig.rightIndex.calcT, this._rig.rightMiddle.calcT);
				this.FireEvents(listener, leftFinger, rightFinger);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x060076C3 RID: 30403 RVA: 0x0026F3E4 File Offset: 0x0026D5E4
		private void FireEvents(FingerFlexEvent.Listener listener, float leftFinger, float rightFinger)
		{
			bool flag = this.parentTransferable != null || this.myHeldItem != null;
			if ((this.ignoreTransferable && listener.checkLeftHand) || (!this.ignoreTransferable && flag && this.FingerFlexValidation(true)))
			{
				this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
				return;
			}
			if ((this.ignoreTransferable && !listener.checkLeftHand) || (!this.ignoreTransferable && flag && this.FingerFlexValidation(false)))
			{
				this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
				return;
			}
			this.CheckFingerValue(listener, leftFinger, true, ref listener.fingerLeftLastValue);
			this.CheckFingerValue(listener, rightFinger, false, ref listener.fingerRightLastValue);
		}

		// Token: 0x060076C4 RID: 30404 RVA: 0x0026F498 File Offset: 0x0026D698
		private void CheckFingerValue(FingerFlexEvent.Listener listener, float fingerValue, bool isLeft, ref float lastValue)
		{
			if (fingerValue > listener.fingerFlexValue)
			{
				listener.frameCounter++;
			}
			switch (listener.eventType)
			{
			case FingerFlexEvent.EventType.OnFingerFlexed:
				if (fingerValue > listener.fingerFlexValue && lastValue < listener.fingerFlexValue)
				{
					UnityEvent<bool, float> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(isLeft, fingerValue);
					}
				}
				break;
			case FingerFlexEvent.EventType.OnFingerReleased:
				if (fingerValue <= listener.fingerReleaseValue && lastValue > listener.fingerReleaseValue)
				{
					UnityEvent<bool, float> listenerComponent2 = listener.listenerComponent;
					if (listenerComponent2 != null)
					{
						listenerComponent2.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			case FingerFlexEvent.EventType.OnFingerFlexStayed:
				if (fingerValue > listener.fingerFlexValue && lastValue >= listener.fingerFlexValue && listener.frameCounter % listener.frameInterval == 0)
				{
					UnityEvent<bool, float> listenerComponent3 = listener.listenerComponent;
					if (listenerComponent3 != null)
					{
						listenerComponent3.Invoke(isLeft, fingerValue);
					}
					listener.frameCounter = 0;
				}
				break;
			}
			lastValue = fingerValue;
		}

		// Token: 0x060076C5 RID: 30405 RVA: 0x0026F57C File Offset: 0x0026D77C
		private bool FingerFlexValidation(bool isLeftHand)
		{
			bool flag;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag = (heldItem != null && heldItem.InLeftHand());
			}
			else
			{
				flag = this.parentTransferable.InLeftHand();
			}
			bool flag2 = flag;
			return (!flag2 || isLeftHand) && (flag2 || !isLeftHand);
		}

		// Token: 0x040088CC RID: 35020
		[SerializeField]
		public bool ignoreTransferable;

		// Token: 0x040088CD RID: 35021
		[SerializeField]
		private FingerFlexEvent.FingerType fingerType = FingerFlexEvent.FingerType.Index;

		// Token: 0x040088CE RID: 35022
		public FingerFlexEvent.Listener[] eventListeners = new FingerFlexEvent.Listener[0];

		// Token: 0x040088CF RID: 35023
		private VRRig _rig;

		// Token: 0x040088D0 RID: 35024
		private TransferrableObject parentTransferable;

		// Token: 0x040088D1 RID: 35025
		private IHeldItem myHeldItem;

		// Token: 0x02001280 RID: 4736
		[Serializable]
		public class Listener
		{
			// Token: 0x040088D2 RID: 35026
			public FingerFlexEvent.EventType eventType;

			// Token: 0x040088D3 RID: 35027
			public UnityEvent<bool, float> listenerComponent;

			// Token: 0x040088D4 RID: 35028
			public float fingerFlexValue = 0.75f;

			// Token: 0x040088D5 RID: 35029
			public float fingerReleaseValue = 0.01f;

			// Token: 0x040088D6 RID: 35030
			[Tooltip("How many frames should pass to fire a finger flex stayed event")]
			public int frameInterval = 20;

			// Token: 0x040088D7 RID: 35031
			[Tooltip("This event will be fired for everyone in the room (synced) by default unless you uncheck this box so that it will be fired only for the local player.")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x040088D8 RID: 35032
			[Tooltip("Fire these events only when the item is held in hand, only works if there is a transferable component somewhere on the object or its parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x040088D9 RID: 35033
			[Tooltip("Whether to check the left hand or the right hand, only works if \"ignoreTransferable\" is true.")]
			public bool checkLeftHand;

			// Token: 0x040088DA RID: 35034
			internal int frameCounter;

			// Token: 0x040088DB RID: 35035
			internal float fingerRightLastValue;

			// Token: 0x040088DC RID: 35036
			internal float fingerLeftLastValue;
		}

		// Token: 0x02001281 RID: 4737
		public enum EventType
		{
			// Token: 0x040088DE RID: 35038
			OnFingerFlexed,
			// Token: 0x040088DF RID: 35039
			OnFingerReleased,
			// Token: 0x040088E0 RID: 35040
			OnFingerFlexStayed
		}

		// Token: 0x02001282 RID: 4738
		private enum FingerType
		{
			// Token: 0x040088E2 RID: 35042
			Thumb,
			// Token: 0x040088E3 RID: 35043
			Index,
			// Token: 0x040088E4 RID: 35044
			Middle,
			// Token: 0x040088E5 RID: 35045
			IndexAndMiddleMin
		}
	}
}
