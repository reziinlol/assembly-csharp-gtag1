using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000543 RID: 1347
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x17000398 RID: 920
	// (get) Token: 0x060021FA RID: 8698 RVA: 0x000B5DA7 File Offset: 0x000B3FA7
	// (set) Token: 0x060021FB RID: 8699 RVA: 0x000B5DB4 File Offset: 0x000B3FB4
	private XformOffset _EdXformOffsetRepresenationOf_overrideTransformMatrix
	{
		get
		{
			return new XformOffset(this.overrideTransformMatrix);
		}
		set
		{
			this.overrideTransformMatrix = Matrix4x4.TRS(value.pos, value.rot, value.scale);
		}
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x000B5DD4 File Offset: 0x000B3FD4
	public void Initialize(Component component, Transform anchor)
	{
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x000B5E80 File Offset: 0x000B4080
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x000B5E94 File Offset: 0x000B4094
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint item = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(item);
	}

	// Token: 0x04002CF0 RID: 11504
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	public Transform overrideTransform;

	// Token: 0x04002CF1 RID: 11505
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	[Delayed]
	public string overrideTransform_path;

	// Token: 0x04002CF2 RID: 11506
	public TransferrableObject.PositionState positionState;

	// Token: 0x04002CF3 RID: 11507
	public bool useAdvancedGrab;

	// Token: 0x04002CF4 RID: 11508
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x04002CF5 RID: 11509
	public Transform advancedGrabPointAnchor;

	// Token: 0x04002CF6 RID: 11510
	public Transform advancedGrabPointOrigin;

	// Token: 0x04002CF7 RID: 11511
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x04002CF8 RID: 11512
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x04002CF9 RID: 11513
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
