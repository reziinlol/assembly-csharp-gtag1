using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000552 RID: 1362
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x060022A8 RID: 8872 RVA: 0x000B9F21 File Offset: 0x000B8121
	// (set) Token: 0x060022A9 RID: 8873 RVA: 0x000B9F2C File Offset: 0x000B812C
	[DebugOption]
	public Transform CurrentBadgeTransform
	{
		get
		{
			return this.currentBadgeTransform;
		}
		set
		{
			if (value != this.currentBadgeTransform)
			{
				this.ResetBadge();
				this.currentBadgeTransform = value;
				this.badgeDefaultRot = this.currentBadgeTransform.localRotation;
				this.badgeDefaultPos = this.currentBadgeTransform.localPosition;
				this.UpdateBadge();
			}
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x060022AA RID: 8874 RVA: 0x000B9F7C File Offset: 0x000B817C
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x060022AB RID: 8875 RVA: 0x000B9F84 File Offset: 0x000B8184
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x060022AC RID: 8876 RVA: 0x000B9F8C File Offset: 0x000B818C
	public Transform BuilderWatchAnchor
	{
		get
		{
			return this.builderResizeButtonDefaultAnchor;
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x060022AD RID: 8877 RVA: 0x000B9F94 File Offset: 0x000B8194
	public Transform BuilderWatch
	{
		get
		{
			return this.builderResizeButton;
		}
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x000B9F9C File Offset: 0x000B819C
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		int num = this.MapPositionToIndex(TransferrableObject.PositionState.OnChest);
		this.overrideAnchors[num] = this.chestDefaultTransform;
		this.huntDefaultTransform = this.huntComputer;
		this.builderResizeButtonDefaultTransform = this.builderResizeButton;
		this.activeAntiClippingOffsets = default(CosmeticAnchorAntiIntersectOffsets);
	}

	// Token: 0x060022AF RID: 8879 RVA: 0x000B9FFC File Offset: 0x000B81FC
	private void OnEnable()
	{
		if (this.nameDefaultAnchor && this.nameDefaultAnchor.parent)
		{
			this.nameTransform.parent = this.nameDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `nameTransform` because `nameDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.huntComputer = this.huntDefaultTransform;
		if (this.huntComputerDefaultAnchor && this.huntComputerDefaultAnchor.parent)
		{
			this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `huntComputer` because `huntComputerDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.builderResizeButton = this.builderResizeButtonDefaultTransform;
		if (this.builderResizeButtonDefaultAnchor && this.builderResizeButtonDefaultAnchor.parent)
		{
			this.builderResizeButton.parent = this.builderResizeButtonDefaultAnchor.parent;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent `builderResizeButton` because `builderResizeButtonDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x060022B0 RID: 8880 RVA: 0x000BA118 File Offset: 0x000B8318
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000BA138 File Offset: 0x000B8338
	public void ApplyAntiClippingOffsets(TransferrableObject.PositionState pos, XformOffset offset, bool enable, Transform defaultAnchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					GTDev.LogWarning<string>(string.Format("Anti Clipping offset for position {0} is not implemented", pos), null);
					return;
				}
				this.activeAntiClippingOffsets.chest.enabled = enable;
				this.activeAntiClippingOffsets.chest.offset = (enable ? offset : XformOffset.Identity);
			}
			else
			{
				this.activeAntiClippingOffsets.rightArm.enabled = enable;
				this.activeAntiClippingOffsets.rightArm.offset = (enable ? offset : XformOffset.Identity);
			}
		}
		else
		{
			this.activeAntiClippingOffsets.leftArm.enabled = enable;
			this.activeAntiClippingOffsets.leftArm.offset = (enable ? offset : XformOffset.Identity);
		}
		if (enable && (this.overrideAnchors[num] == null || (pos == TransferrableObject.PositionState.OnChest && this.overrideAnchors[num] == this.chestDefaultTransform)))
		{
			if (this.clippingOffsetTransforms[num] == null)
			{
				GameObject gameObject = new GameObject("Anti Clipping Offset");
				gameObject.transform.SetParent(defaultAnchor);
				this.clippingOffsetTransforms[num] = gameObject.transform;
			}
			Transform transform = this.clippingOffsetTransforms[num];
			transform.SetParent(defaultAnchor);
			transform.localPosition = offset.pos;
			transform.localRotation = offset.rot;
			transform.localScale = Vector3.one;
			this.OverrideAnchor(pos, transform);
			return;
		}
		if (!enable && this.overrideAnchors[num] == this.clippingOffsetTransforms[num])
		{
			if (pos == TransferrableObject.PositionState.OnChest)
			{
				this.OverrideAnchor(pos, this.chestDefaultTransform);
				return;
			}
			this.OverrideAnchor(pos, null);
		}
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000BA2DC File Offset: 0x000B84DC
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num] == this.chestDefaultTransform)
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				Transform transform = (Transform)obj;
				if (!transform.name.Equals("DropZoneChest") && transform != anchor)
				{
					transform.parent = null;
				}
			}
			this.overrideAnchors[num] = anchor;
			return;
		}
		if (this.overrideAnchors[num])
		{
			foreach (object obj2 in this.overrideAnchors[num])
			{
				Transform transform2 = (Transform)obj2;
				if (transform2 != anchor)
				{
					transform2.parent = null;
				}
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000BA3E8 File Offset: 0x000B85E8
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000BA40C File Offset: 0x000B860C
	public void UpdateHuntWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.huntComputer.enabled = enable;
		this.activeAntiClippingOffsets.huntComputer.offset = (enable ? offset : XformOffset.Identity);
		this.huntComputer.parent = this.HuntDefaultAnchor;
		this.huntComputer.localPosition = this.activeAntiClippingOffsets.huntComputer.offset.pos;
		this.huntComputer.localRotation = this.activeAntiClippingOffsets.huntComputer.offset.rot;
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000BA498 File Offset: 0x000B8698
	public void UpdateBuilderWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.builderWatch.enabled = enable;
		this.activeAntiClippingOffsets.builderWatch.offset = (enable ? offset : XformOffset.Identity);
		this.BuilderWatch.parent = this.BuilderWatchAnchor;
		this.BuilderWatch.localPosition = this.activeAntiClippingOffsets.builderWatch.offset.pos;
		this.BuilderWatch.localRotation = this.activeAntiClippingOffsets.builderWatch.offset.rot;
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000BA524 File Offset: 0x000B8724
	public void UpdateFriendshipBraceletOffset(XformOffset offset, bool left, bool enable)
	{
		if (left)
		{
			this.activeAntiClippingOffsets.friendshipBraceletLeft.enabled = enable;
			this.activeAntiClippingOffsets.friendshipBraceletLeft.offset = (enable ? offset : XformOffset.Identity);
			this.friendshipBraceletLeftAnchor.parent = this.friendshipBraceletLeftDefaultAnchor;
			this.friendshipBraceletLeftAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.pos;
			this.friendshipBraceletLeftAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.rot;
			this.friendshipBraceletLeftAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.scale;
			return;
		}
		this.activeAntiClippingOffsets.friendshipBraceletRight.enabled = enable;
		this.activeAntiClippingOffsets.friendshipBraceletRight.offset = (enable ? offset : XformOffset.Identity);
		this.friendshipBraceletRightAnchor.parent = this.friendshipBraceletRightDefaultAnchor;
		this.friendshipBraceletRightAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.pos;
		this.friendshipBraceletRightAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.rot;
		this.friendshipBraceletRightAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.scale;
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000BA674 File Offset: 0x000B8874
	public void UpdateNameTagOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Hat:
			this.nameOffsets[5].enabled = enable;
			this.nameOffsets[5].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Badge:
			this.nameOffsets[6].enabled = enable;
			this.nameOffsets[6].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Face:
			this.nameOffsets[4].enabled = enable;
			this.nameOffsets[4].offset = offset;
			break;
		default:
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.Fur:
				this.nameOffsets[1].enabled = enable;
				this.nameOffsets[1].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Shirt:
				this.nameOffsets[0].enabled = enable;
				this.nameOffsets[0].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Pants:
				this.nameOffsets[2].enabled = enable;
				this.nameOffsets[2].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Back:
				this.nameOffsets[3].enabled = enable;
				this.nameOffsets[3].offset = offset;
				break;
			}
			break;
		}
		this.UpdateName();
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000BA7C8 File Offset: 0x000B89C8
	[Obsolete("Use UpdateNameOffset", true)]
	public void UpdateNameAnchor(GameObject nameAnchor, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.nameAnchors[1] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.nameAnchors[0] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.nameAnchors[2] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.nameAnchors[3] = nameAnchor;
					break;
				}
			}
			else
			{
				this.nameAnchors[4] = nameAnchor;
			}
		}
		else
		{
			this.nameAnchors[5] = nameAnchor;
		}
		this.UpdateName();
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000BA840 File Offset: 0x000B8A40
	private void UpdateName()
	{
		for (int i = 0; i < this.nameOffsets.Length; i++)
		{
			if (this.nameOffsets[i].enabled)
			{
				this.nameTransform.parent = this.nameDefaultAnchor;
				this.nameTransform.localRotation = this.nameOffsets[i].offset.rot;
				this.nameTransform.localPosition = this.nameOffsets[i].offset.pos;
				return;
			}
		}
		if (this.nameDefaultAnchor)
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = Quaternion.identity;
			this.nameTransform.localPosition = Vector3.zero;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent for `nameTransform` because `nameDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000BA924 File Offset: 0x000B8B24
	public void UpdateBadgeOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Hat)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.badgeOffsets[1].enabled = enable;
					this.badgeOffsets[1].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.badgeOffsets[0].enabled = enable;
					this.badgeOffsets[0].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.badgeOffsets[2].enabled = enable;
					this.badgeOffsets[2].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.badgeOffsets[3].enabled = enable;
					this.badgeOffsets[3].offset = offset;
					break;
				}
			}
			else
			{
				this.badgeOffsets[4].enabled = enable;
				this.badgeOffsets[4].offset = offset;
			}
		}
		else
		{
			this.badgeOffsets[5].enabled = enable;
			this.badgeOffsets[5].offset = offset;
		}
		this.UpdateBadge();
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000BAA4C File Offset: 0x000B8C4C
	[Obsolete("Use UpdateBadgeOffset", true)]
	public void UpdateBadgeAnchor(GameObject badgeAnchor, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Fur:
			this.badgeAnchors[1] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeAnchors[0] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeAnchors[2] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeAnchors[3] = badgeAnchor;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000BAAA4 File Offset: 0x000B8CA4
	private void UpdateBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		for (int i = 0; i < this.badgeOffsets.Length; i++)
		{
			if (this.badgeOffsets[i].enabled)
			{
				Matrix4x4 rhs = Matrix4x4.TRS(this.badgeDefaultPos, this.badgeDefaultRot, this.currentBadgeTransform.localScale);
				Matrix4x4 matrix = Matrix4x4.TRS(this.badgeOffsets[i].offset.pos, this.badgeOffsets[i].offset.rot, Vector3.one) * rhs;
				this.currentBadgeTransform.localRotation = matrix.rotation;
				this.currentBadgeTransform.localPosition = matrix.Position();
				return;
			}
		}
		foreach (GameObject gameObject in this.badgeAnchors)
		{
			if (gameObject)
			{
				this.currentBadgeTransform.localRotation = gameObject.transform.localRotation;
				this.currentBadgeTransform.localPosition = gameObject.transform.localPosition;
				return;
			}
		}
		this.ResetBadge();
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000BABC6 File Offset: 0x000B8DC6
	private void ResetBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		this.currentBadgeTransform.localRotation = this.badgeDefaultRot;
		this.currentBadgeTransform.localPosition = this.badgeDefaultPos;
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000BABF8 File Offset: 0x000B8DF8
	private void OnDestroy()
	{
		for (int i = 0; i < this.clippingOffsetTransforms.Length; i++)
		{
			if (this.clippingOffsetTransforms[i] != null)
			{
				foreach (object obj in this.clippingOffsetTransforms[i])
				{
					((Transform)obj).parent = null;
				}
				Object.Destroy(this.clippingOffsetTransforms[i].gameObject);
			}
		}
	}

	// Token: 0x04002DB3 RID: 11699
	[SerializeField]
	public Transform nameDefaultAnchor;

	// Token: 0x04002DB4 RID: 11700
	[SerializeField]
	public Transform nameTransform;

	// Token: 0x04002DB5 RID: 11701
	[SerializeField]
	public Transform chestDefaultTransform;

	// Token: 0x04002DB6 RID: 11702
	[SerializeField]
	public Transform huntComputer;

	// Token: 0x04002DB7 RID: 11703
	[SerializeField]
	public Transform huntComputerDefaultAnchor;

	// Token: 0x04002DB8 RID: 11704
	public Transform huntDefaultTransform;

	// Token: 0x04002DB9 RID: 11705
	[SerializeField]
	protected Transform builderResizeButton;

	// Token: 0x04002DBA RID: 11706
	[SerializeField]
	protected Transform builderResizeButtonDefaultAnchor;

	// Token: 0x04002DBB RID: 11707
	private Transform builderResizeButtonDefaultTransform;

	// Token: 0x04002DBC RID: 11708
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04002DBD RID: 11709
	private CosmeticAnchorAntiIntersectOffsets activeAntiClippingOffsets;

	// Token: 0x04002DBE RID: 11710
	private Transform[] clippingOffsetTransforms = new Transform[8];

	// Token: 0x04002DBF RID: 11711
	private GameObject nameLastObjectToAttach;

	// Token: 0x04002DC0 RID: 11712
	private Transform currentBadgeTransform;

	// Token: 0x04002DC1 RID: 11713
	private Vector3 badgeDefaultPos;

	// Token: 0x04002DC2 RID: 11714
	private Quaternion badgeDefaultRot;

	// Token: 0x04002DC3 RID: 11715
	private GameObject[] badgeAnchors = new GameObject[4];

	// Token: 0x04002DC4 RID: 11716
	private GameObject[] nameAnchors = new GameObject[6];

	// Token: 0x04002DC5 RID: 11717
	private CosmeticAnchorAntiClipEntry[] badgeOffsets = new CosmeticAnchorAntiClipEntry[6];

	// Token: 0x04002DC6 RID: 11718
	private CosmeticAnchorAntiClipEntry[] nameOffsets = new CosmeticAnchorAntiClipEntry[7];

	// Token: 0x04002DC7 RID: 11719
	[SerializeField]
	public Transform friendshipBraceletLeftDefaultAnchor;

	// Token: 0x04002DC8 RID: 11720
	public Transform friendshipBraceletLeftAnchor;

	// Token: 0x04002DC9 RID: 11721
	[SerializeField]
	public Transform friendshipBraceletRightDefaultAnchor;

	// Token: 0x04002DCA RID: 11722
	public Transform friendshipBraceletRightAnchor;
}
