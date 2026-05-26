using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001245 RID: 4677
	public class HeadlessHead : HoldableObject
	{
		// Token: 0x06007511 RID: 29969 RVA: 0x00265E08 File Offset: 0x00264008
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.ownerRig.isOfflineVRRig;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
			this.baseLocalPosition = base.transform.localPosition;
			this.hasFirstPersonRenderer = (this.firstPersonRenderer != null);
		}

		// Token: 0x06007512 RID: 29970 RVA: 0x00265E8C File Offset: 0x0026408C
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("HeadlessHead \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
		}

		// Token: 0x06007513 RID: 29971 RVA: 0x00265EE5 File Offset: 0x002640E5
		private void OnDisable()
		{
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
		}

		// Token: 0x06007514 RID: 29972 RVA: 0x00265EF8 File Offset: 0x002640F8
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x06007515 RID: 29973 RVA: 0x00265F16 File Offset: 0x00264116
		protected virtual void LateUpdateLocal()
		{
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
		}

		// Token: 0x06007516 RID: 29974 RVA: 0x00265F54 File Offset: 0x00264154
		protected virtual void LateUpdateReplicated()
		{
			int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
			this.isHeld = (num != 0);
			this.isHeldLeftHand = ((num & 2) != 0);
		}

		// Token: 0x06007517 RID: 29975 RVA: 0x00265FA0 File Offset: 0x002641A0
		protected virtual void LateUpdateShared()
		{
			if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
			{
				this.blendingFromPosition = base.transform.position;
				this.blendingFromRotation = base.transform.rotation;
				this.blendFraction = 0f;
			}
			Quaternion quaternion;
			Vector3 vector;
			if (this.isHeldLeftHand)
			{
				quaternion = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
				vector = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else if (this.isHeld)
			{
				quaternion = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
				vector = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else
			{
				quaternion = base.transform.parent.rotation;
				vector = base.transform.parent.TransformPoint(this.baseLocalPosition);
			}
			if (this.blendFraction < 1f)
			{
				this.blendFraction += Time.deltaTime / this.blendDuration;
				quaternion = Quaternion.Lerp(this.blendingFromRotation, quaternion, this.blendFraction);
				vector = Vector3.Lerp(this.blendingFromPosition, vector, this.blendFraction);
			}
			base.transform.rotation = quaternion;
			base.transform.position = vector;
			if (this.hasFirstPersonRenderer)
			{
				float x = base.transform.lossyScale.x;
				this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * x);
			}
			this.wasHeld = this.isHeld;
			this.wasHeldLeftHand = this.isHeldLeftHand;
		}

		// Token: 0x06007518 RID: 29976 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06007519 RID: 29977 RVA: 0x002661A7 File Offset: 0x002643A7
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			this.isHeld = true;
			this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
			EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		}

		// Token: 0x0600751A RID: 29978 RVA: 0x002661DB File Offset: 0x002643DB
		public override void DropItemCleanup()
		{
			this.isHeld = false;
			this.isHeldLeftHand = false;
		}

		// Token: 0x0600751B RID: 29979 RVA: 0x002661EC File Offset: 0x002643EC
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
			{
				return false;
			}
			if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
			{
				return false;
			}
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
			this.isHeld = false;
			this.isHeldLeftHand = false;
			return true;
		}

		// Token: 0x040086A8 RID: 34472
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;

		// Token: 0x040086A9 RID: 34473
		[SerializeField]
		private Vector3 offsetFromLeftHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x040086AA RID: 34474
		[SerializeField]
		private Vector3 offsetFromRightHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x040086AB RID: 34475
		[SerializeField]
		private Quaternion rotationFromLeftHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x040086AC RID: 34476
		[SerializeField]
		private Quaternion rotationFromRightHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x040086AD RID: 34477
		private Vector3 baseLocalPosition;

		// Token: 0x040086AE RID: 34478
		private VRRig ownerRig;

		// Token: 0x040086AF RID: 34479
		private bool isLocal;

		// Token: 0x040086B0 RID: 34480
		private bool isHeld;

		// Token: 0x040086B1 RID: 34481
		private bool isHeldLeftHand;

		// Token: 0x040086B2 RID: 34482
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x040086B3 RID: 34483
		[SerializeField]
		private MeshRenderer firstPersonRenderer;

		// Token: 0x040086B4 RID: 34484
		[SerializeField]
		private float firstPersonHiddenRadius;

		// Token: 0x040086B5 RID: 34485
		[SerializeField]
		private Transform firstPersonHideCenter;

		// Token: 0x040086B6 RID: 34486
		[SerializeField]
		private Transform holdAnchorPoint;

		// Token: 0x040086B7 RID: 34487
		private bool hasFirstPersonRenderer;

		// Token: 0x040086B8 RID: 34488
		private Vector3 blendingFromPosition;

		// Token: 0x040086B9 RID: 34489
		private Quaternion blendingFromRotation;

		// Token: 0x040086BA RID: 34490
		private float blendFraction;

		// Token: 0x040086BB RID: 34491
		private bool wasHeld;

		// Token: 0x040086BC RID: 34492
		private bool wasHeldLeftHand;

		// Token: 0x040086BD RID: 34493
		[SerializeField]
		private float blendDuration = 0.3f;
	}
}
