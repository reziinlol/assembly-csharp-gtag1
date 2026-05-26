using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class ArtilleryCrank : HoldableObject
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000ABE RID: 2750 RVA: 0x00039A08 File Offset: 0x00037C08
	public bool IsHeld
	{
		get
		{
			return this.isHeld;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000ABF RID: 2751 RVA: 0x00039A10 File Offset: 0x00037C10
	public bool IsHeldLeftHand
	{
		get
		{
			return this.isHeldLeftHand;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000AC0 RID: 2752 RVA: 0x00039A18 File Offset: 0x00037C18
	public float CurrentAngle
	{
		get
		{
			return this.currentAngle;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000AC1 RID: 2753 RVA: 0x00039A20 File Offset: 0x00037C20
	private int CrankIndex
	{
		get
		{
			if (this.crankType != ArtilleryCrankType.Pitch)
			{
				return 1;
			}
			return 0;
		}
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00039A30 File Offset: 0x00037C30
	private void Awake()
	{
		if (this.rotatingPart == null)
		{
			this.rotatingPart = base.transform;
		}
		Vector3 vector = this.rotatingPart.parent.InverseTransformPoint(this.rotatingPart.TransformPoint(Vector3.right));
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		this.baseLocalAngle = this.rotatingPart.localRotation;
		this.baseLocalAngleInverse = Quaternion.Inverse(this.baseLocalAngle);
		this.crankRadius = new Vector2(this.crankHandleX, this.crankHandleY).magnitude;
		this.crankAngleOffset = Mathf.Atan2(this.crankHandleY, this.crankHandleX) * 57.29578f;
		if (this.crankHandleMaxZ < this.crankHandleMinZ)
		{
			float num = this.crankHandleMaxZ;
			float num2 = this.crankHandleMinZ;
			this.crankHandleMinZ = num;
			this.crankHandleMaxZ = num2;
		}
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00039B20 File Offset: 0x00037D20
	private void LateUpdate()
	{
		if (!this.isHeld)
		{
			return;
		}
		if (!this.cannon.IsCrankHeldLocally(this.CrankIndex))
		{
			this.DropItemCleanup();
			return;
		}
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.rotatingPart.InverseTransformPoint(controllerTransform.position);
		Vector3 position = (vector.xy().normalized * this.crankRadius).WithZ(Mathf.Clamp(vector.z, this.crankHandleMinZ, this.crankHandleMaxZ));
		Vector3 vector2 = this.rotatingPart.TransformPoint(position);
		if (this.maxHandSnapDistance > 0f && (controllerTransform.position - vector2).IsLongerThan(this.maxHandSnapDistance))
		{
			this.OnRelease(null, this.isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = vector2;
		float num = this.ComputeAngleFromWorldPos(controllerTransform.position);
		float num2 = Mathf.DeltaAngle(this.lastAngle, num);
		this.lastAngle = num;
		this.currentAngle = num;
		if (num2 != 0f)
		{
			this.cannon.OnCrankInput(this.CrankIndex, num2);
		}
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00039C60 File Offset: 0x00037E60
	public void UpdateFromRemoteHand(VRRig rig, bool leftHand)
	{
		VRMap vrmap = leftHand ? rig.leftHand : rig.rightHand;
		Vector3 vector = vrmap.GetExtrapolatedControllerPosition();
		vector -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(leftHand) * rig.scaleFactor;
		float angle = this.ComputeAngleFromWorldPos(vector);
		this.currentAngle = angle;
		this.ApplyVisualAngle(angle);
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00039CC9 File Offset: 0x00037EC9
	public void SetVisualAngle(float angle)
	{
		if (this.rotatingPart != null)
		{
			this.currentAngle = angle;
			this.ApplyVisualAngle(angle);
		}
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00039CE8 File Offset: 0x00037EE8
	private float ComputeAngleFromWorldPos(Vector3 worldPos)
	{
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (worldPos - this.rotatingPart.position);
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00039D43 File Offset: 0x00037F43
	private void ApplyVisualAngle(float angle)
	{
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(angle - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x00039D70 File Offset: 0x00037F70
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (!this.cannon.OnCrankGrabbed(this.CrankIndex, this.isHeldLeftHand))
		{
			return;
		}
		this.isHeld = true;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (controllerTransform.position - this.rotatingPart.position);
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00039E33 File Offset: 0x00038033
	public override void DropItemCleanup()
	{
		if (this.isHeld)
		{
			this.isHeld = false;
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
		}
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00039E5C File Offset: 0x0003805C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		if (this.isHeld)
		{
			this.isHeld = false;
			this.cannon.OnCrankReleased(this.CrankIndex, this.currentAngle);
		}
		return true;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00039EB0 File Offset: 0x000380B0
	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04000D02 RID: 3330
	[SerializeField]
	private ArtilleryCannon cannon;

	// Token: 0x04000D03 RID: 3331
	[SerializeField]
	private ArtilleryCrankType crankType;

	// Token: 0x04000D04 RID: 3332
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04000D05 RID: 3333
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04000D06 RID: 3334
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04000D07 RID: 3335
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04000D08 RID: 3336
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04000D09 RID: 3337
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04000D0A RID: 3338
	private float crankAngleOffset;

	// Token: 0x04000D0B RID: 3339
	private float crankRadius;

	// Token: 0x04000D0C RID: 3340
	private float lastAngle;

	// Token: 0x04000D0D RID: 3341
	private float currentAngle;

	// Token: 0x04000D0E RID: 3342
	private Quaternion baseLocalAngle;

	// Token: 0x04000D0F RID: 3343
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04000D10 RID: 3344
	private bool isHeld;

	// Token: 0x04000D11 RID: 3345
	private bool isHeldLeftHand;
}
