using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001298 RID: 4760
	[RequireComponent(typeof(Collider))]
	public class OnTriggerEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06007720 RID: 30496 RVA: 0x0027125C File Offset: 0x0026F45C
		private bool IsMyItem()
		{
			return this.rig != null && this.rig.isOfflineVRRig;
		}

		// Token: 0x06007721 RID: 30497 RVA: 0x0027127C File Offset: 0x0026F47C
		private void Awake()
		{
			Collider[] components = base.GetComponents<Collider>();
			if (components == null || components.Length == 0)
			{
				Debug.LogError("OnTriggerEventsCosmetic requires at least one Collider on the same GameObject.");
				base.enabled = false;
				return;
			}
			bool flag = false;
			foreach (Collider collider in components)
			{
				if (collider != null && (collider.isTrigger || collider.attachedRigidbody != null))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogWarning("OnTriggerEventsCosmetic: Collider is not set to Trigger. OnTrigger will not fire. Path=" + base.transform.GetPathQ(), base.transform);
			}
			this.rig = base.GetComponentInParent<VRRig>();
			if (this.rig == null && base.gameObject.GetComponentInParent<GTPlayer>() != null)
			{
				this.rig = GorillaTagger.Instance.offlineVRRig;
			}
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			List<OnTriggerEventsCosmetic.Listener> list = new List<OnTriggerEventsCosmetic.Listener>();
			List<OnTriggerEventsCosmetic.Listener> list2 = new List<OnTriggerEventsCosmetic.Listener>();
			List<OnTriggerEventsCosmetic.Listener> list3 = new List<OnTriggerEventsCosmetic.Listener>();
			if (this.eventListeners != null)
			{
				for (int j = 0; j < this.eventListeners.Length; j++)
				{
					OnTriggerEventsCosmetic.Listener listener = this.eventListeners[j];
					if (listener.tagSet == null)
					{
						if (listener.triggerTagsList != null && listener.triggerTagsList.Count > 0)
						{
							listener.tagSet = new HashSet<string>(listener.triggerTagsList);
						}
						else
						{
							listener.tagSet = new HashSet<string>();
						}
					}
					if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerEnter)
					{
						list.Add(listener);
					}
					else if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerStay)
					{
						list2.Add(listener);
					}
					else if (listener.eventType == OnTriggerEventsCosmetic.EventType.TriggerExit)
					{
						list3.Add(listener);
					}
				}
			}
			this.enterListeners = ((list.Count > 0) ? list.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
			this.stayListeners = ((list2.Count > 0) ? list2.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
			this.exitListeners = ((list3.Count > 0) ? list3.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>());
		}

		// Token: 0x06007722 RID: 30498 RVA: 0x0027147E File Offset: 0x0026F67E
		private void OnTriggerEnter(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.enterListeners, other);
		}

		// Token: 0x06007723 RID: 30499 RVA: 0x00271496 File Offset: 0x0026F696
		private void OnTriggerStay(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.stayListeners, other);
		}

		// Token: 0x06007724 RID: 30500 RVA: 0x002714AE File Offset: 0x0026F6AE
		private void OnTriggerExit(Collider other)
		{
			if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
			{
				return;
			}
			this.Dispatch(this.exitListeners, other);
		}

		// Token: 0x06007725 RID: 30501 RVA: 0x002714C8 File Offset: 0x0026F6C8
		private static bool IsOtherUsable(Collider other)
		{
			if (other == null)
			{
				return false;
			}
			GameObject gameObject = other.gameObject;
			return !(gameObject == null) && gameObject.activeInHierarchy;
		}

		// Token: 0x06007726 RID: 30502 RVA: 0x002714FC File Offset: 0x0026F6FC
		private void Dispatch(OnTriggerEventsCosmetic.Listener[] listeners, Collider other)
		{
			if (listeners == null || listeners.Length == 0)
			{
				return;
			}
			int layer = other.gameObject.layer;
			GorillaGrabber gorillaGrabber = null;
			bool flag = other.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled;
			bool flag2 = flag && gorillaGrabber.IsLeftHand;
			bool flag3;
			if (!(this.parentTransferable != null))
			{
				IHeldItem heldItem = this.myHeldItem;
				flag3 = (heldItem != null && heldItem.InLeftHand());
			}
			else
			{
				flag3 = this.parentTransferable.InLeftHand();
			}
			bool flag4 = flag3;
			Vector3 position = (this.myCollider != null) ? this.myCollider.bounds.center : base.transform.position;
			foreach (OnTriggerEventsCosmetic.Listener listener in listeners)
			{
				bool arg = (listener.handSource == OnTriggerEventsCosmetic.HandSource.HoldingHand) ? flag4 : (flag ? flag2 : flag4);
				if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnTriggerEventsCosmetic.CompareTagAny(other.gameObject, listener.tagSet)) && (1 << layer & listener.triggerLayerMask.value) != 0)
				{
					UnityEvent<bool, Collider> listenerComponent = listener.listenerComponent;
					if (listenerComponent != null)
					{
						listenerComponent.Invoke(arg, other);
					}
					Vector3 arg2 = other.ClosestPoint(position);
					UnityEvent<Vector3> listenerComponentContactPoint = listener.listenerComponentContactPoint;
					if (listenerComponentContactPoint != null)
					{
						listenerComponentContactPoint.Invoke(arg2);
					}
					VRRig componentInParent = other.GetComponentInParent<VRRig>();
					if (componentInParent != null)
					{
						UnityEvent<VRRig> onTriggeredVRRig = listener.onTriggeredVRRig;
						if (onTriggeredVRRig != null)
						{
							onTriggeredVRRig.Invoke(componentInParent);
						}
					}
				}
			}
		}

		// Token: 0x06007727 RID: 30503 RVA: 0x002716AC File Offset: 0x0026F8AC
		private static bool CompareTagAny(GameObject go, HashSet<string> tagSet)
		{
			if (tagSet == null || tagSet.Count == 0)
			{
				return true;
			}
			foreach (string text in tagSet)
			{
				if (!string.IsNullOrEmpty(text) && go.CompareTag(text))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007728 RID: 30504 RVA: 0x00271718 File Offset: 0x0026F918
		private bool IsTagValid(GameObject obj, OnTriggerEventsCosmetic.Listener listener)
		{
			return listener == null || (listener.tagSet == null || listener.tagSet.Count == 0) || OnTriggerEventsCosmetic.CompareTagAny(obj, listener.tagSet);
		}

		// Token: 0x04008967 RID: 35175
		[Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to trigger with (layers/tags), and which UnityEvents to fire.")]
		public OnTriggerEventsCosmetic.Listener[] eventListeners = new OnTriggerEventsCosmetic.Listener[0];

		// Token: 0x04008968 RID: 35176
		private OnTriggerEventsCosmetic.Listener[] enterListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x04008969 RID: 35177
		private OnTriggerEventsCosmetic.Listener[] stayListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x0400896A RID: 35178
		private OnTriggerEventsCosmetic.Listener[] exitListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();

		// Token: 0x0400896B RID: 35179
		private Collider myCollider;

		// Token: 0x0400896C RID: 35180
		private VRRig rig;

		// Token: 0x0400896D RID: 35181
		private TransferrableObject parentTransferable;

		// Token: 0x0400896E RID: 35182
		private IHeldItem myHeldItem;

		// Token: 0x02001299 RID: 4761
		[Serializable]
		public class Listener
		{
			// Token: 0x0400896F RID: 35183
			[Tooltip("Only trigger interactions with objects on these layers.")]
			public LayerMask triggerLayerMask;

			// Token: 0x04008970 RID: 35184
			[Tooltip("Optional tag whitelist. If non-empty, triggers must match at least one of these tags.")]
			public List<string> triggerTagsList = new List<string>();

			// Token: 0x04008971 RID: 35185
			[Tooltip("Choose which trigger phase invokes this listener: Enter, Stay, or Exit.")]
			public OnTriggerEventsCosmetic.EventType eventType;

			// Token: 0x04008972 RID: 35186
			public UnityEvent<bool, Collider> listenerComponent;

			// Token: 0x04008973 RID: 35187
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x04008974 RID: 35188
			public UnityEvent<VRRig> onTriggeredVRRig;

			// Token: 0x04008975 RID: 35189
			[Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x04008976 RID: 35190
			[Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x04008977 RID: 35191
			[Tooltip("Which hand determines the isLeftHand argument passed to the event.")]
			public OnTriggerEventsCosmetic.HandSource handSource;

			// Token: 0x04008978 RID: 35192
			[NonSerialized]
			public HashSet<string> tagSet;
		}

		// Token: 0x0200129A RID: 4762
		public enum EventType
		{
			// Token: 0x0400897A RID: 35194
			TriggerEnter,
			// Token: 0x0400897B RID: 35195
			TriggerStay,
			// Token: 0x0400897C RID: 35196
			TriggerExit
		}

		// Token: 0x0200129B RID: 4763
		public enum HandSource
		{
			// Token: 0x0400897E RID: 35198
			[Tooltip("isLeftHand = which hand is physically touching this trigger (GorillaGrabber). Falls back to the holding hand if no hand collider is detected.")]
			TouchingHand,
			// Token: 0x0400897F RID: 35199
			[Tooltip("isLeftHand = which hand this cosmetic is equipped in (TransferrableObject). Falls back to the touching hand if no TransferrableObject is found.")]
			HoldingHand
		}
	}
}
