using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001294 RID: 4756
	[RequireComponent(typeof(Collider))]
	public class OnCollisionEventsCosmetic : MonoBehaviour
	{
		// Token: 0x06007715 RID: 30485 RVA: 0x00270D49 File Offset: 0x0026EF49
		private bool IsMyItem()
		{
			return this.rig != null && this.rig.isOfflineVRRig;
		}

		// Token: 0x06007716 RID: 30486 RVA: 0x00270D68 File Offset: 0x0026EF68
		private void Awake()
		{
			this.myCollider = base.GetComponent<Collider>();
			if (this.myCollider == null)
			{
				Debug.LogError("OnCollisionEventsCosmetic requires a Collider on the same GameObject.");
				base.enabled = false;
				return;
			}
			if (this.myCollider.isTrigger)
			{
				Debug.LogWarning("OnCollisionEventsCosmetic: Collider is set to Trigger. OnCollision will not fire. Set it to non-trigger for collisions.");
			}
			this.rig = base.GetComponentInParent<VRRig>();
			this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			this.myHeldItem = base.GetComponentInParent<IHeldItem>();
			List<OnCollisionEventsCosmetic.Listener> list = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list2 = new List<OnCollisionEventsCosmetic.Listener>();
			List<OnCollisionEventsCosmetic.Listener> list3 = new List<OnCollisionEventsCosmetic.Listener>();
			if (this.eventListeners != null)
			{
				for (int i = 0; i < this.eventListeners.Length; i++)
				{
					OnCollisionEventsCosmetic.Listener listener = this.eventListeners[i];
					if (listener.tagSet == null)
					{
						if (listener.collisionTagsList != null && listener.collisionTagsList.Count > 0)
						{
							listener.tagSet = new HashSet<string>(listener.collisionTagsList);
						}
						else
						{
							listener.tagSet = new HashSet<string>();
						}
					}
					if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
					{
						list.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
					{
						list2.Add(listener);
					}
					else if (listener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
					{
						list3.Add(listener);
					}
				}
			}
			this.enterListeners = ((list.Count > 0) ? list.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.stayListeners = ((list2.Count > 0) ? list2.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
			this.exitListeners = ((list3.Count > 0) ? list3.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>());
		}

		// Token: 0x06007717 RID: 30487 RVA: 0x00270EEB File Offset: 0x0026F0EB
		private void OnCollisionEnter(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.enterListeners, collision);
		}

		// Token: 0x06007718 RID: 30488 RVA: 0x00270F03 File Offset: 0x0026F103
		private void OnCollisionStay(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.stayListeners, collision);
		}

		// Token: 0x06007719 RID: 30489 RVA: 0x00270F1B File Offset: 0x0026F11B
		private void OnCollisionExit(Collision collision)
		{
			if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
			{
				return;
			}
			this.Dispatch(this.exitListeners, collision);
		}

		// Token: 0x0600771A RID: 30490 RVA: 0x00270F34 File Offset: 0x0026F134
		private static bool IsCollisionUsable(Collision collision)
		{
			if (collision == null)
			{
				return false;
			}
			Collider collider = collision.collider;
			if (collider == null)
			{
				return false;
			}
			GameObject gameObject = collider.gameObject;
			return !(gameObject == null) && gameObject.activeInHierarchy;
		}

		// Token: 0x0600771B RID: 30491 RVA: 0x00270F74 File Offset: 0x0026F174
		private void Dispatch(OnCollisionEventsCosmetic.Listener[] listeners, Collision collision)
		{
			if (listeners == null || listeners.Length == 0)
			{
				return;
			}
			Collider collider = collision.collider;
			GameObject gameObject = (collider != null) ? collider.gameObject : null;
			if (gameObject == null)
			{
				return;
			}
			int layer = gameObject.layer;
			GorillaGrabber gorillaGrabber = null;
			bool flag = collider != null && collider.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled;
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
			Vector3 arg;
			if (collision.contactCount > 0)
			{
				arg = collision.GetContact(0).point;
			}
			else
			{
				arg = collider.ClosestPoint(position);
			}
			foreach (OnCollisionEventsCosmetic.Listener listener in listeners)
			{
				bool arg2 = (listener.handSource == OnCollisionEventsCosmetic.HandSource.HoldingHand) ? flag4 : (flag ? flag2 : flag4);
				if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnCollisionEventsCosmetic.CompareTagAny(gameObject, listener.tagSet)) && (1 << layer & listener.collisionLayerMask.value) != 0)
				{
					if (listener.listenerComponent != null)
					{
						listener.listenerComponent.Invoke(arg2, collision);
					}
					if (listener.listenerComponentContactPoint != null)
					{
						listener.listenerComponentContactPoint.Invoke(arg);
					}
					VRRig componentInParent = gameObject.GetComponentInParent<VRRig>();
					if (componentInParent != null && listener.onCollidedVRRig != null)
					{
						listener.onCollidedVRRig.Invoke(componentInParent);
					}
				}
			}
		}

		// Token: 0x0600771C RID: 30492 RVA: 0x00271170 File Offset: 0x0026F370
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

		// Token: 0x0600771D RID: 30493 RVA: 0x002711DC File Offset: 0x0026F3DC
		private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
		{
			return listener == null || (listener.tagSet == null || listener.tagSet.Count == 0) || OnCollisionEventsCosmetic.CompareTagAny(obj, listener.tagSet);
		}

		// Token: 0x0400894E RID: 35150
		[Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to collide with (layers/tags), and which UnityEvents to fire.")]
		public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];

		// Token: 0x0400894F RID: 35151
		private OnCollisionEventsCosmetic.Listener[] enterListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008950 RID: 35152
		private OnCollisionEventsCosmetic.Listener[] stayListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008951 RID: 35153
		private OnCollisionEventsCosmetic.Listener[] exitListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();

		// Token: 0x04008952 RID: 35154
		private Collider myCollider;

		// Token: 0x04008953 RID: 35155
		private VRRig rig;

		// Token: 0x04008954 RID: 35156
		private TransferrableObject parentTransferable;

		// Token: 0x04008955 RID: 35157
		private IHeldItem myHeldItem;

		// Token: 0x02001295 RID: 4757
		[Serializable]
		public class Listener
		{
			// Token: 0x04008956 RID: 35158
			[Tooltip("Only collisions with objects on these layers will be considered.")]
			public LayerMask collisionLayerMask;

			// Token: 0x04008957 RID: 35159
			[Tooltip("Optional tag whitelist. If non-empty, collisions must match at least one of these tags.")]
			public List<string> collisionTagsList = new List<string>();

			// Token: 0x04008958 RID: 35160
			[Tooltip("Choose which collision phase triggers this listener: Enter, Stay, or Exit.")]
			public OnCollisionEventsCosmetic.EventType eventType;

			// Token: 0x04008959 RID: 35161
			public UnityEvent<bool, Collision> listenerComponent;

			// Token: 0x0400895A RID: 35162
			public UnityEvent<Vector3> listenerComponentContactPoint;

			// Token: 0x0400895B RID: 35163
			public UnityEvent<VRRig> onCollidedVRRig;

			// Token: 0x0400895C RID: 35164
			[Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
			public bool syncForEveryoneInRoom = true;

			// Token: 0x0400895D RID: 35165
			[Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
			public bool fireOnlyWhileHeld = true;

			// Token: 0x0400895E RID: 35166
			[Tooltip("Which hand determines the isLeftHand argument passed to the event.")]
			public OnCollisionEventsCosmetic.HandSource handSource;

			// Token: 0x0400895F RID: 35167
			[NonSerialized]
			public HashSet<string> tagSet;
		}

		// Token: 0x02001296 RID: 4758
		public enum EventType
		{
			// Token: 0x04008961 RID: 35169
			CollisionEnter,
			// Token: 0x04008962 RID: 35170
			CollisionStay,
			// Token: 0x04008963 RID: 35171
			CollisionExit
		}

		// Token: 0x02001297 RID: 4759
		public enum HandSource
		{
			// Token: 0x04008965 RID: 35173
			[Tooltip("isLeftHand = which hand is physically colliding with this object (GorillaGrabber). Falls back to the holding hand if no hand collider is detected.")]
			TouchingHand,
			// Token: 0x04008966 RID: 35174
			[Tooltip("isLeftHand = which hand this cosmetic is equipped in (TransferrableObject). Falls back to the touching hand if no TransferrableObject is found.")]
			HoldingHand
		}
	}
}
