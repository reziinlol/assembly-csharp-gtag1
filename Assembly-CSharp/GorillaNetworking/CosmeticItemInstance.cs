using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001019 RID: 4121
	public class CosmeticItemInstance
	{
		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x060066D5 RID: 26325 RVA: 0x002111BC File Offset: 0x0020F3BC
		public CosmeticsController.CosmeticSlots ActiveSlot
		{
			get
			{
				return this._activeSlot;
			}
		}

		// Token: 0x060066D6 RID: 26326 RVA: 0x002111C4 File Offset: 0x0020F3C4
		private void EnableItem(GameObject obj, bool enable)
		{
			try
			{
				obj.SetActive(enable);
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Exception while enabling cosmetic: {0}", arg));
			}
		}

		// Token: 0x060066D7 RID: 26327 RVA: 0x00211200 File Offset: 0x0020F400
		private void ApplyClippingOffsets(bool itemEnabled)
		{
			if (this._bodyDockPositions == null)
			{
				return;
			}
			if (this._anchorOverrides != null)
			{
				if (this.clippingOffsets.nameTag.enabled)
				{
					this._anchorOverrides.UpdateNameTagOffset(itemEnabled ? this.clippingOffsets.nameTag.offset : XformOffset.Identity, itemEnabled, this._activeSlot);
				}
				if (this.clippingOffsets.leftArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnLeftArm, this.clippingOffsets.leftArm.offset, itemEnabled, this._bodyDockPositions.leftArmTransform);
				}
				if (this.clippingOffsets.rightArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnRightArm, this.clippingOffsets.rightArm.offset, itemEnabled, this._bodyDockPositions.rightArmTransform);
				}
				if (this.clippingOffsets.chest.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnChest, this.clippingOffsets.chest.offset, itemEnabled, this._anchorOverrides.chestDefaultTransform);
				}
				if (this.clippingOffsets.huntComputer.enabled)
				{
					this._anchorOverrides.UpdateHuntWatchOffset(this.clippingOffsets.huntComputer.offset, itemEnabled);
				}
				if (this.clippingOffsets.badge.enabled)
				{
					this._anchorOverrides.UpdateBadgeOffset(itemEnabled ? this.clippingOffsets.badge.offset : XformOffset.Identity, itemEnabled, this._activeSlot);
				}
				if (this.clippingOffsets.builderWatch.enabled)
				{
					this._anchorOverrides.UpdateBuilderWatchOffset(this.clippingOffsets.builderWatch.offset, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletLeft.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletLeft.offset, true, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletRight.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletRight.offset, false, itemEnabled);
				}
			}
		}

		// Token: 0x060066D8 RID: 26328 RVA: 0x00211414 File Offset: 0x0020F614
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject obj in this.objects)
			{
				this.EnableItem(obj, false);
			}
			if (flag)
			{
				foreach (GameObject obj2 in this.leftObjects)
				{
					this.EnableItem(obj2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj3 in this.rightObjects)
				{
					this.EnableItem(obj3, false);
				}
			}
			this.ApplyClippingOffsets(false);
		}

		// Token: 0x060066D9 RID: 26329 RVA: 0x00211510 File Offset: 0x0020F710
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot, VRRig rig)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			this._activeSlot = cosmeticSlot;
			if (rig != null && this._anchorOverrides == null)
			{
				this._anchorOverrides = rig.gameObject.GetComponent<VRRigAnchorOverrides>();
				this._bodyDockPositions = rig.GetComponent<BodyDockPositions>();
			}
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					if (this.objects.Count > 1)
					{
						GTHardCodedBones.EBone ebone;
						Transform transform;
						if (GTHardCodedBones.TryGetFirstBoneInParents(gameObject.transform, out ebone, out transform) && ebone == GTHardCodedBones.EBone.body)
						{
							this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
						}
					}
					else
					{
						this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
					}
				}
			}
			if (flag)
			{
				foreach (GameObject obj in this.leftObjects)
				{
					this.EnableItem(obj, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj2 in this.rightObjects)
				{
					this.EnableItem(obj2, true);
				}
			}
			this.ApplyClippingOffsets(true);
		}

		// Token: 0x060066DA RID: 26330 RVA: 0x00211690 File Offset: 0x0020F890
		public void ToggleRenderers(bool enabled)
		{
			for (int i = 0; i < this.allRenderers.Count; i++)
			{
				this.allRenderers[i].enabled = enabled;
			}
		}

		// Token: 0x060066DB RID: 26331 RVA: 0x002116C8 File Offset: 0x0020F8C8
		public void ToggleParticles(bool enabled)
		{
			for (int i = 0; i < this.allParticles.Count; i++)
			{
				this.allParticles[i].emission.enabled = enabled;
			}
		}

		// Token: 0x04007638 RID: 30264
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x04007639 RID: 30265
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x0400763A RID: 30266
		public List<GameObject> objects = new List<GameObject>();

		// Token: 0x0400763B RID: 30267
		public List<GameObject> holdableObjects = new List<GameObject>();

		// Token: 0x0400763C RID: 30268
		public List<Renderer> allRenderers = new List<Renderer>();

		// Token: 0x0400763D RID: 30269
		public List<ParticleSystem> allParticles = new List<ParticleSystem>();

		// Token: 0x0400763E RID: 30270
		public CosmeticAnchorAntiIntersectOffsets clippingOffsets;

		// Token: 0x0400763F RID: 30271
		public bool isHoldableItem;

		// Token: 0x04007640 RID: 30272
		public string dbgname;

		// Token: 0x04007641 RID: 30273
		private BodyDockPositions _bodyDockPositions;

		// Token: 0x04007642 RID: 30274
		private VRRigAnchorOverrides _anchorOverrides;

		// Token: 0x04007643 RID: 30275
		private CosmeticsController.CosmeticSlots _activeSlot;
	}
}
