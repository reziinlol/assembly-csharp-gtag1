using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000196 RID: 406
public class BatteryChargerCrank : HoldableObject
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x0003A582 File Offset: 0x00038782
	public bool IsHeld
	{
		get
		{
			return this.isHeld;
		}
	}

	// Token: 0x17000101 RID: 257
	// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x0003A58A File Offset: 0x0003878A
	public bool IsHeldLeftHand
	{
		get
		{
			return this.isHeldLeftHand;
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x06000AEA RID: 2794 RVA: 0x0003A592 File Offset: 0x00038792
	public float CurrentAngle
	{
		get
		{
			return this.currentAngle;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x06000AEB RID: 2795 RVA: 0x0003A59A File Offset: 0x0003879A
	internal int CrankIndex
	{
		get
		{
			return this.crankIndex;
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003A5A4 File Offset: 0x000387A4
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

	// Token: 0x06000AED RID: 2797 RVA: 0x0003A691 File Offset: 0x00038891
	private void Start()
	{
		this.crankIndex = this.charger.RegisterCrank(this);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003A6A8 File Offset: 0x000388A8
	private void LateUpdate()
	{
		if (!this.isHeld || this.crankIndex < 0)
		{
			return;
		}
		if (!this.charger.IsCrankHeldLocally(this.crankIndex))
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
			this.charger.OnCrankInput(this.crankIndex, num2);
			GorillaTagger.Instance.DoVibration(this.isHeldLeftHand ? XRNode.LeftHand : XRNode.RightHand, Mathf.Abs(num2 / 30f) * this.vibrationAmplitude, Time.deltaTime);
		}
		this.UpdateCrankSound(num2);
		this.ApplyVisualAngle(num);
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003A828 File Offset: 0x00038A28
	public void UpdateFromRemoteHand(VRRig rig, bool leftHand)
	{
		VRMap vrmap = leftHand ? rig.leftHand : rig.rightHand;
		Vector3 vector = vrmap.GetExtrapolatedControllerPosition();
		vector -= vrmap.rigTarget.rotation * GTPlayer.Instance.GetHandOffset(leftHand) * rig.scaleFactor;
		float angle = this.ComputeAngleFromWorldPos(vector);
		this.currentAngle = angle;
		this.ApplyVisualAngle(angle);
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0003A891 File Offset: 0x00038A91
	public void SetVisualAngle(float angle)
	{
		if (this.rotatingPart != null)
		{
			this.currentAngle = angle;
			this.ApplyVisualAngle(angle);
		}
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003A8B0 File Offset: 0x00038AB0
	private float ComputeAngleFromWorldPos(Vector3 worldPos)
	{
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (worldPos - this.rotatingPart.position);
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0003A90B File Offset: 0x00038B0B
	private void ApplyVisualAngle(float angle)
	{
		this.rotatingPart.localRotation = this.baseLocalAngle * Quaternion.AngleAxis(angle - this.crankAngleOffset, Vector3.forward);
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0003A938 File Offset: 0x00038B38
	private void UpdateCrankSound(float crankAmount)
	{
		if (this.crankSound == null)
		{
			return;
		}
		float b = Mathf.Abs(crankAmount / 30f) * this.vibrationAmplitude;
		this.smoothCrankSpeed = Mathf.Lerp(this.smoothCrankSpeed, b, 10f * Time.deltaTime);
		if (this.smoothCrankSpeed > 0.01f)
		{
			if (!this.crankSound.isPlaying)
			{
				this.crankSound.Play();
			}
			float t = Mathf.Clamp01(this.smoothCrankSpeed);
			this.crankSound.pitch = Mathf.Lerp(this.crankSoundMinPitch, this.crankSoundMaxPitch, t);
			return;
		}
		if (this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
			this.smoothCrankSpeed = 0f;
		}
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0003A9F7 File Offset: 0x00038BF7
	private void StopCrankSound()
	{
		if (this.crankSound != null && this.crankSound.isPlaying)
		{
			this.crankSound.Stop();
		}
		this.smoothCrankSpeed = 0f;
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x0003AA2C File Offset: 0x00038C2C
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (this.crankIndex < 0)
		{
			return;
		}
		this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
		if (!this.charger.OnCrankGrabbed(this.crankIndex, this.isHeldLeftHand))
		{
			return;
		}
		this.isHeld = true;
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(this.isHeldLeftHand);
		Vector3 vector = this.baseLocalAngleInverse * Quaternion.Inverse(this.rotatingPart.parent.rotation) * (controllerTransform.position - this.rotatingPart.position);
		this.lastAngle = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0003AAF9 File Offset: 0x00038CF9
	public override void DropItemCleanup()
	{
		if (this.isHeld)
		{
			this.isHeld = false;
			this.StopCrankSound();
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0003AB28 File Offset: 0x00038D28
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
			this.charger.OnCrankReleased(this.crankIndex, this.currentAngle);
		}
		return true;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003AB7C File Offset: 0x00038D7C
	private void OnDrawGizmosSelected()
	{
		Transform transform = (this.rotatingPart != null) ? this.rotatingPart : base.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMinZ)), transform.TransformPoint(new Vector3(this.crankHandleX, this.crankHandleY, this.crankHandleMaxZ)));
	}

	// Token: 0x04000D28 RID: 3368
	[SerializeField]
	private BatteryCharger charger;

	// Token: 0x04000D29 RID: 3369
	[SerializeField]
	private float crankHandleX;

	// Token: 0x04000D2A RID: 3370
	[SerializeField]
	private float crankHandleY;

	// Token: 0x04000D2B RID: 3371
	[SerializeField]
	private float crankHandleMinZ;

	// Token: 0x04000D2C RID: 3372
	[SerializeField]
	private float crankHandleMaxZ;

	// Token: 0x04000D2D RID: 3373
	[SerializeField]
	private float maxHandSnapDistance;

	// Token: 0x04000D2E RID: 3374
	[SerializeField]
	private Transform rotatingPart;

	// Token: 0x04000D2F RID: 3375
	[SerializeField]
	private float vibrationAmplitude = 0.3f;

	// Token: 0x04000D30 RID: 3376
	[SerializeField]
	private AudioSource crankSound;

	// Token: 0x04000D31 RID: 3377
	[SerializeField]
	private float crankSoundMinPitch = 0.6f;

	// Token: 0x04000D32 RID: 3378
	[SerializeField]
	private float crankSoundMaxPitch = 1.4f;

	// Token: 0x04000D33 RID: 3379
	private float crankAngleOffset;

	// Token: 0x04000D34 RID: 3380
	private float crankRadius;

	// Token: 0x04000D35 RID: 3381
	private float lastAngle;

	// Token: 0x04000D36 RID: 3382
	private float currentAngle;

	// Token: 0x04000D37 RID: 3383
	private float smoothCrankSpeed;

	// Token: 0x04000D38 RID: 3384
	private Quaternion baseLocalAngle;

	// Token: 0x04000D39 RID: 3385
	private Quaternion baseLocalAngleInverse;

	// Token: 0x04000D3A RID: 3386
	private int crankIndex = -1;

	// Token: 0x04000D3B RID: 3387
	private bool isHeld;

	// Token: 0x04000D3C RID: 3388
	private bool isHeldLeftHand;
}
