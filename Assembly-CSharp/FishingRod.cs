using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000686 RID: 1670
public class FishingRod : TransferrableObject
{
	// Token: 0x0600298E RID: 10638 RVA: 0x000E0224 File Offset: 0x000DE424
	public override void OnActivate()
	{
		base.OnActivate();
		Transform transform = base.transform;
		Vector3 force = transform.up + transform.forward * 640f;
		this.bobRigidbody.AddForce(force, ForceMode.Impulse);
		this.line.tensionScale = 0.86f;
		this.ReelOut();
	}

	// Token: 0x0600298F RID: 10639 RVA: 0x000E027D File Offset: 0x000DE47D
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.line.tensionScale = 1f;
		this.ReelStop();
	}

	// Token: 0x06002990 RID: 10640 RVA: 0x000E029B File Offset: 0x000DE49B
	protected override void Start()
	{
		base.Start();
		this.rig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000E02AF File Offset: 0x000DE4AF
	public void SetBobFloat(bool enable)
	{
		if (!this.bobRigidbody)
		{
			return;
		}
		this._bobFloatPlaneY = this.bobRigidbody.position.y;
		this._bobFloating = enable;
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000E02DC File Offset: 0x000DE4DC
	private void QuickReel()
	{
		if (this._lineResizing)
		{
			return;
		}
		this.bobCollider.enabled = false;
		this.ReelIn();
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000E02FC File Offset: 0x000DE4FC
	public bool IsFreeHandGripping()
	{
		bool flag = base.InLeftHand();
		Transform transform = flag ? this.rig.rightHandTransform : this.rig.leftHandTransform;
		float magnitude = (this.reelToSync.position - transform.position).magnitude;
		bool flag2 = this._grippingHand || magnitude <= 0.16f;
		this.disableStealing = flag2;
		if (!flag2)
		{
			return false;
		}
		VRMapThumb vrmapThumb = flag ? this.rig.rightThumb : this.rig.leftThumb;
		VRMapIndex vrmapIndex = flag ? this.rig.rightIndex : this.rig.leftIndex;
		VRMap vrmap = flag ? this.rig.rightMiddle : this.rig.leftMiddle;
		float calcT = vrmapThumb.calcT;
		float calcT2 = vrmapIndex.calcT;
		float calcT3 = vrmap.calcT;
		bool flag3 = calcT >= 0.1f && calcT2 >= 0.2f && calcT3 >= 0.2f;
		this._grippingHand = (flag3 ? transform : null);
		return flag3;
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000E0415 File Offset: 0x000DE615
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (this._grippingHand)
		{
			this._grippingHand = null;
		}
		this.ResetLineLength(this.lineLengthMin * 1.32f);
		return true;
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000E044C File Offset: 0x000DE64C
	public void ReelIn()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, true);
		this._lineResizing = true;
		this._lineExpanding = false;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000E04CC File Offset: 0x000DE6CC
	public void ReelOut()
	{
		this._manualReeling = false;
		FishingRod.SetHandleMotorUse(true, this.reelSpinRate, this.handleJoint, false);
		this._lineResizing = true;
		this._lineExpanding = true;
		float num = (float)this.line.segmentNumber + 0.0001f;
		this.line.segmentMinLength = (this._targetSegmentMin = this.lineLengthMin / num);
		this.line.segmentMaxLength = (this._targetSegmentMax = this.lineLengthMax / num);
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000E054C File Offset: 0x000DE74C
	public void ReelStop()
	{
		if (this._manualReeling)
		{
			this._localRotDelta = 0f;
		}
		else
		{
			FishingRod.SetHandleMotorUse(false, 0f, this.handleJoint, false);
		}
		this.bobCollider.enabled = true;
		if (this.line)
		{
			this.line.resizeScale = 1f;
		}
		this._lineResizing = false;
		this._lineExpanding = false;
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x000E05B8 File Offset: 0x000DE7B8
	private static void SetHandleMotorUse(bool useMotor, float spinRate, HingeJoint handleJoint, bool reverse)
	{
		JointMotor motor = handleJoint.motor;
		motor.force = (useMotor ? 1f : 0f) * spinRate;
		motor.targetVelocity = 16384f * (reverse ? -1f : 1f);
		handleJoint.motor = motor;
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000E0608 File Offset: 0x000DE808
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		this._manualReeling = (this._isGrippingHandle = this.IsFreeHandGripping());
		if (ControllerInputPoller.instance && ControllerInputPoller.PrimaryButtonPress(base.InLeftHand() ? XRNode.LeftHand : XRNode.RightHand))
		{
			this.QuickReel();
		}
		if (this._lineResetting && this._sinceReset.HasElapsed(this.line.resizeSpeed))
		{
			this.bobCollider.enabled = true;
			this._lineResetting = false;
		}
		this.handleTransform.localPosition = this.reelFreezeLocalPosition;
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000E069B File Offset: 0x000DE89B
	private void ResetLineLength(float length)
	{
		if (!this.line)
		{
			return;
		}
		this._lineResetting = true;
		this.bobCollider.enabled = false;
		this.line.ForceTotalLength(length);
		this._sinceReset = TimeSince.Now();
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000E06D8 File Offset: 0x000DE8D8
	private void FixedUpdate()
	{
		Transform transform = base.transform;
		this.handleRigidbody.useGravity = !this._manualReeling;
		if (this._bobFloating && this.bobRigidbody)
		{
			float y = this.bobRigidbody.position.y;
			float num = this.bobFloatForce * this.bobRigidbody.mass;
			float num2 = num * Mathf.Clamp01(this._bobFloatPlaneY - y);
			num += num2;
			if (y <= this._bobFloatPlaneY)
			{
				this.bobRigidbody.AddForce(0f, num, 0f);
			}
		}
		if (this._manualReeling)
		{
			if (this._isGrippingHandle && this._grippingHand)
			{
				this.reelTo.position = this._grippingHand.position;
			}
			Vector3 vector = this.reelFrom.InverseTransformPoint(this.reelTo.position);
			vector.x = 0f;
			vector.Normalize();
			vector *= 2f;
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, vector);
			quaternion = (base.InRightHand() ? quaternion : Quaternion.Inverse(quaternion));
			this._localRotDelta = FishingRod.GetSignedDeltaYZ(ref this._lastLocalRot, ref quaternion);
			this._lastLocalRot = quaternion;
			Quaternion rot = transform.rotation * quaternion;
			this.handleRigidbody.MoveRotation(rot);
		}
		else
		{
			this.reelTo.localPosition = transform.InverseTransformPoint(this.reelToSync.position);
		}
		if (!this.line)
		{
			return;
		}
		if (this._manualReeling)
		{
			this._lineResizing = (Mathf.Abs(this._localRotDelta) >= 0.001f);
			this._lineExpanding = (Mathf.Sign(this._localRotDelta) >= 0f);
		}
		if (!this._lineResizing)
		{
			return;
		}
		float num3 = this._manualReeling ? (Mathf.Abs(this._localRotDelta) * 0.66f * Time.fixedDeltaTime) : (this.lineResizeRate * this.lineCastFactor);
		this.line.resizeScale = this.lineCastFactor;
		float num4 = num3 * Time.fixedDeltaTime;
		float num5 = this.line.segmentTargetLength;
		if (this._manualReeling)
		{
			float num6 = 1f / ((float)this.line.segmentNumber + 0.0001f);
			float num7 = this.lineLengthMin * num6;
			float num8 = this.lineLengthMax * num6;
			num4 *= (this._lineExpanding ? 1f : -1f);
			num4 *= (base.InRightHand() ? -1f : 1f);
			float num9 = num5 + num4;
			if (num9 > num7 && num9 < num8)
			{
				num5 += num4;
			}
		}
		else if (this._lineExpanding)
		{
			if (num5 < this._targetSegmentMax)
			{
				num5 += num4;
			}
			else
			{
				this._lineResizing = false;
			}
		}
		else if (num5 > this._targetSegmentMin)
		{
			num5 -= num4;
		}
		else
		{
			this._lineResizing = false;
		}
		if (this._lineResizing)
		{
			this.line.segmentTargetLength = num5;
			return;
		}
		this.ReelStop();
	}

	// Token: 0x0600299C RID: 10652 RVA: 0x000E09D0 File Offset: 0x000DEBD0
	private static float GetSignedDeltaYZ(ref Quaternion a, ref Quaternion b)
	{
		Vector3 forward = Vector3.forward;
		Vector3 vector = a * forward;
		Vector3 vector2 = b * forward;
		float current = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		float target = Mathf.Atan2(vector2.y, vector2.z) * 57.29578f;
		return Mathf.DeltaAngle(current, target);
	}

	// Token: 0x04003624 RID: 13860
	public Transform handleTransform;

	// Token: 0x04003625 RID: 13861
	public HingeJoint handleJoint;

	// Token: 0x04003626 RID: 13862
	public Rigidbody handleRigidbody;

	// Token: 0x04003627 RID: 13863
	public BoxCollider handleCollider;

	// Token: 0x04003628 RID: 13864
	public Rigidbody bobRigidbody;

	// Token: 0x04003629 RID: 13865
	public Collider bobCollider;

	// Token: 0x0400362A RID: 13866
	public VerletLine line;

	// Token: 0x0400362B RID: 13867
	public GorillaVelocityEstimator tipTracker;

	// Token: 0x0400362C RID: 13868
	public Rigidbody tipBody;

	// Token: 0x0400362D RID: 13869
	[NonSerialized]
	public VRRig rig;

	// Token: 0x0400362E RID: 13870
	[Space]
	public Vector3 reelFreezeLocalPosition;

	// Token: 0x0400362F RID: 13871
	public Transform reelFrom;

	// Token: 0x04003630 RID: 13872
	public Transform reelTo;

	// Token: 0x04003631 RID: 13873
	public Transform reelToSync;

	// Token: 0x04003632 RID: 13874
	[Space]
	public float reelSpinRate = 1f;

	// Token: 0x04003633 RID: 13875
	public float lineResizeRate = 1f;

	// Token: 0x04003634 RID: 13876
	public float lineCastFactor = 3f;

	// Token: 0x04003635 RID: 13877
	public float lineLengthMin = 0.1f;

	// Token: 0x04003636 RID: 13878
	public float lineLengthMax = 8f;

	// Token: 0x04003637 RID: 13879
	[Space]
	[NonSerialized]
	private bool _bobFloating;

	// Token: 0x04003638 RID: 13880
	public float bobFloatForce = 8f;

	// Token: 0x04003639 RID: 13881
	public float bobStaticDrag = 3.2f;

	// Token: 0x0400363A RID: 13882
	public float bobDynamicDrag = 1.1f;

	// Token: 0x0400363B RID: 13883
	[NonSerialized]
	private float _bobFloatPlaneY;

	// Token: 0x0400363C RID: 13884
	[Space]
	[NonSerialized]
	private float _targetSegmentMin;

	// Token: 0x0400363D RID: 13885
	[NonSerialized]
	private float _targetSegmentMax;

	// Token: 0x0400363E RID: 13886
	[Space]
	[NonSerialized]
	private bool _manualReeling;

	// Token: 0x0400363F RID: 13887
	[NonSerialized]
	private bool _lineResizing;

	// Token: 0x04003640 RID: 13888
	[NonSerialized]
	private bool _lineExpanding;

	// Token: 0x04003641 RID: 13889
	[NonSerialized]
	private bool _lineResetting;

	// Token: 0x04003642 RID: 13890
	[NonSerialized]
	private TimeSince _sinceReset;

	// Token: 0x04003643 RID: 13891
	[Space]
	[NonSerialized]
	private Quaternion _lastLocalRot = Quaternion.identity;

	// Token: 0x04003644 RID: 13892
	[NonSerialized]
	private float _localRotDelta;

	// Token: 0x04003645 RID: 13893
	[NonSerialized]
	private bool _isGrippingHandle;

	// Token: 0x04003646 RID: 13894
	[NonSerialized]
	private Transform _grippingHand;

	// Token: 0x04003647 RID: 13895
	private TimeSince _sinceGripLoss;
}
