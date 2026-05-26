using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000EFB RID: 3835
	public class DecorativeItem : TransferrableObject
	{
		// Token: 0x06005F4A RID: 24394 RVA: 0x001DACF0 File Offset: 0x001D8EF0
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06005F4B RID: 24395 RVA: 0x001EABCA File Offset: 0x001E8DCA
		public override void OnSpawn(VRRig rig)
		{
			base.OnSpawn(rig);
			this.parent = base.transform.parent;
		}

		// Token: 0x06005F4C RID: 24396 RVA: 0x001DAD73 File Offset: 0x001D8F73
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06005F4D RID: 24397 RVA: 0x001EABE4 File Offset: 0x001E8DE4
		private new void OnStateChanged()
		{
			TransferrableObject.ItemStates itemState = this.itemState;
			if (itemState == TransferrableObject.ItemStates.State2)
			{
				this.SnapItem(this.reliableState.isSnapped, this.reliableState.snapPosition);
				return;
			}
			if (itemState != TransferrableObject.ItemStates.State3)
			{
				return;
			}
			this.Respawn(this.reliableState.respawnPosition, this.reliableState.respawnRotation);
		}

		// Token: 0x06005F4E RID: 24398 RVA: 0x001EAC3C File Offset: 0x001E8E3C
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			DecorativeItem.DecorativeItemState itemState = (DecorativeItem.DecorativeItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
		}

		// Token: 0x06005F4F RID: 24399 RVA: 0x001EAC7B File Offset: 0x001E8E7B
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
			if (this.itemState == TransferrableObject.ItemStates.State4 && this.worldShareableInstance && this.worldShareableInstance.guard.isTrulyMine)
			{
				this.InvokeRespawn();
			}
		}

		// Token: 0x06005F50 RID: 24400 RVA: 0x001EACB2 File Offset: 0x001E8EB2
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06005F51 RID: 24401 RVA: 0x001EACC3 File Offset: 0x001E8EC3
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			return true;
		}

		// Token: 0x06005F52 RID: 24402 RVA: 0x001C2D8C File Offset: 0x001C0F8C
		private void SetWillTeleport()
		{
			this.worldShareableInstance.SetWillTeleport();
		}

		// Token: 0x06005F53 RID: 24403 RVA: 0x001EACE4 File Offset: 0x001E8EE4
		public void Respawn(Vector3 randPosition, Quaternion randRotation)
		{
			if (base.InHand())
			{
				return;
			}
			if (this.shatterVFX && this.ShouldPlayFX())
			{
				this.PlayVFX(this.shatterVFX);
			}
			this.itemState = TransferrableObject.ItemStates.State3;
			this.SetWillTeleport();
			Transform transform = base.transform;
			transform.position = randPosition;
			transform.rotation = randRotation;
			if (this.reliableState)
			{
				this.reliableState.respawnPosition = randPosition;
				this.reliableState.respawnRotation = randRotation;
			}
		}

		// Token: 0x06005F54 RID: 24404 RVA: 0x000D1F56 File Offset: 0x000D0156
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x06005F55 RID: 24405 RVA: 0x001EAD60 File Offset: 0x001E8F60
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x06005F56 RID: 24406 RVA: 0x001EAD9C File Offset: 0x001E8F9C
		public void SnapItem(bool snap, Vector3 attachPoint)
		{
			if (!this.reliableState)
			{
				return;
			}
			if (snap)
			{
				AttachPoint currentAttachPointByPosition = DecorativeItemsManager.Instance.getCurrentAttachPointByPosition(attachPoint);
				if (!currentAttachPointByPosition)
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				Transform attachPoint2 = currentAttachPointByPosition.attachPoint;
				if (!this.Reparent(attachPoint2))
				{
					this.reliableState.isSnapped = false;
					this.reliableState.snapPosition = Vector3.zero;
					return;
				}
				this.itemState = TransferrableObject.ItemStates.State2;
				base.transform.parent.localPosition = Vector3.zero;
				base.transform.localPosition = Vector3.zero;
				this.reliableState.isSnapped = true;
				if (this.audioSource && this.snapAudio && this.ShouldPlayFX())
				{
					this.audioSource.GTPlayOneShot(this.snapAudio, 1f);
				}
				currentAttachPointByPosition.SetIsHook(true);
			}
			else
			{
				this.Reparent(null);
				this.reliableState.isSnapped = false;
			}
			this.reliableState.snapPosition = attachPoint;
		}

		// Token: 0x06005F57 RID: 24407 RVA: 0x001EAEB7 File Offset: 0x001E90B7
		private void InvokeRespawn()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				return;
			}
			UnityAction<DecorativeItem> unityAction = this.respawnItem;
			if (unityAction == null)
			{
				return;
			}
			unityAction(this);
		}

		// Token: 0x06005F58 RID: 24408 RVA: 0x001EAED4 File Offset: 0x001E90D4
		private bool ShouldPlayFX()
		{
			return this.previousItemState == DecorativeItem.DecorativeItemState.isHeld || this.previousItemState == DecorativeItem.DecorativeItemState.dropped;
		}

		// Token: 0x06005F59 RID: 24409 RVA: 0x001EAEEB File Offset: 0x001E90EB
		private void OnCollisionEnter(Collision other)
		{
			if (this.breakItemLayerMask != (this.breakItemLayerMask | 1 << other.gameObject.layer))
			{
				return;
			}
			this.InvokeRespawn();
		}

		// Token: 0x04006E08 RID: 28168
		public DecorativeItemReliableState reliableState;

		// Token: 0x04006E09 RID: 28169
		public UnityAction<DecorativeItem> respawnItem;

		// Token: 0x04006E0A RID: 28170
		public LayerMask breakItemLayerMask;

		// Token: 0x04006E0B RID: 28171
		private Coroutine respawnTimer;

		// Token: 0x04006E0C RID: 28172
		private Transform parent;

		// Token: 0x04006E0D RID: 28173
		private float _respawnTimestamp;

		// Token: 0x04006E0E RID: 28174
		private bool isSnapped;

		// Token: 0x04006E0F RID: 28175
		private Vector3 currentPosition;

		// Token: 0x04006E10 RID: 28176
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006E11 RID: 28177
		public AudioClip snapAudio;

		// Token: 0x04006E12 RID: 28178
		public GameObject shatterVFX;

		// Token: 0x04006E13 RID: 28179
		private new DecorativeItem.DecorativeItemState previousItemState = DecorativeItem.DecorativeItemState.dropped;

		// Token: 0x02000EFC RID: 3836
		private enum DecorativeItemState
		{
			// Token: 0x04006E15 RID: 28181
			isHeld = 1,
			// Token: 0x04006E16 RID: 28182
			dropped,
			// Token: 0x04006E17 RID: 28183
			snapped = 4,
			// Token: 0x04006E18 RID: 28184
			respawn = 8,
			// Token: 0x04006E19 RID: 28185
			none = 16
		}
	}
}
