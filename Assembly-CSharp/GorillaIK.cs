using System;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Android;

// Token: 0x0200086E RID: 2158
public class GorillaIK : MonoBehaviour
{
	// Token: 0x06003840 RID: 14400 RVA: 0x001336D5 File Offset: 0x001318D5
	private void Awake()
	{
		this.bodyInitialRot = this.bodyBone.localRotation;
		this.myRig = base.GetComponent<VRRig>();
		this.ResetIKData();
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x001336FA File Offset: 0x001318FA
	private void OnEnable()
	{
		GorillaIKMgr.Instance.RegisterIK(this);
		if (this.skeleton == null)
		{
			return;
		}
		GorillaIK.playerIK = this;
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x0013371C File Offset: 0x0013191C
	private void OnDisable()
	{
		GorillaIKMgr.Instance.DeregisterIK(this);
		this.ResetIKData();
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x00133730 File Offset: 0x00131930
	public void ResetIKData()
	{
		this.leftElbowDirection = Vector3.zero;
		this.lerpLeftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.lerpRightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.lerpBodyRot = this.targetBodyRot;
		if (this.projectedBodyRotation != null)
		{
			this.projectedBodyRotation.localRotation = this.targetBodyRot;
		}
		this.usingUpdatedIK = false;
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x06003844 RID: 14404 RVA: 0x001337A7 File Offset: 0x001319A7
	// (set) Token: 0x06003845 RID: 14405 RVA: 0x001337AF File Offset: 0x001319AF
	public bool TickRunning { get; set; }

	// Token: 0x06003846 RID: 14406 RVA: 0x001337B8 File Offset: 0x001319B8
	public void OverrideTargetPos(bool isLeftHand, Vector3 targetWorldPos)
	{
		if (isLeftHand)
		{
			this.hasLeftOverride = true;
			this.leftOverrideWorldPos = targetWorldPos;
			return;
		}
		this.hasRightOverride = true;
		this.rightOverrideWorldPos = targetWorldPos;
	}

	// Token: 0x06003847 RID: 14407 RVA: 0x001337DC File Offset: 0x001319DC
	public Vector3 GetShoulderLocalTargetPos_Left(bool updatedIK)
	{
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedLeftShoulderPosition.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
		}
		return this.leftUpperArm.parent.InverseTransformPoint(this.hasLeftOverride ? this.leftOverrideWorldPos : this.targetLeft.position);
	}

	// Token: 0x06003848 RID: 14408 RVA: 0x0013384C File Offset: 0x00131A4C
	public Vector3 GetShoulderLocalTargetPos_Right(bool updatedIK)
	{
		if (this.projectedBodyRotation != null && updatedIK)
		{
			return this.projectedRightShoulderPosition.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
		}
		return this.rightUpperArm.parent.InverseTransformPoint(this.hasRightOverride ? this.rightOverrideWorldPos : this.targetRight.position);
	}

	// Token: 0x06003849 RID: 14409 RVA: 0x001338BB File Offset: 0x00131ABB
	public void ClearOverrides()
	{
		this.hasLeftOverride = false;
		this.hasRightOverride = false;
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x001338CC File Offset: 0x00131ACC
	public void SkeletonUpdate()
	{
		if (!this.canUseUpdatedIK)
		{
			return;
		}
		if (!SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		bool subscriptionSettingBool = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
		if (subscriptionSettingBool != this.skeleton.gameObject.activeSelf)
		{
			this.skeleton.gameObject.SetActive(subscriptionSettingBool);
			this.usingUpdatedIK = subscriptionSettingBool;
			if (!subscriptionSettingBool)
			{
				this.ResetIKData();
			}
			return;
		}
		if (!subscriptionSettingBool)
		{
			return;
		}
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (this.boneXforms[0] == null || this.body == null || this.leftArmUpper == null || this.leftArmLower == null || this.rightArmUpper == null || this.rightArmLower == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
			this.body = this.boneXforms[5];
			this.leftArmUpper = this.boneXforms[10];
			this.leftArmLower = this.boneXforms[11];
			this.rightArmUpper = this.boneXforms[15];
			this.rightArmLower = this.boneXforms[16];
			return;
		}
		this.usingUpdatedIK = true;
		this.targetBodyRot = Quaternion.Inverse(this.bodyBone.parent.rotation) * this.skeleton.transform.rotation * this.body.localRotation * this.bodyOffsetRotation;
		this.projectedBodyRotation.localRotation = this.targetBodyRot;
		this.leftElbowDirection = this.projectedLeftShoulderPosition.InverseTransformDirection((this.leftArmLower.position - this.leftArmLower.up * this.biasDistance - this.targetLeft.position).normalized).normalized;
		this.rightElbowDirection = this.projectedRightShoulderPosition.InverseTransformDirection((this.rightArmLower.position + this.rightArmLower.up * this.biasDistance - this.targetRight.position).normalized).normalized;
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x00133B60 File Offset: 0x00131D60
	private void CheckPermissions()
	{
		if (!Permission.HasUserAuthorizedPermission("com.oculus.permission.BODY_TRACKING"))
		{
			PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
			permissionCallbacks.PermissionGranted += this.PermissionGranted;
			Permission.RequestUserPermission("com.oculus.permission.BODY_TRACKING", permissionCallbacks);
			return;
		}
		this.PermissionGranted("");
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x00133BA8 File Offset: 0x00131DA8
	private void PermissionGranted(string permissionName)
	{
		GorillaIKMgr.AddPlayerIK(this);
		this.boneXforms = new Transform[84];
		this.leftElbowDirection = Vector3.zero;
		this.rightElbowDirection = Vector3.zero;
		this.targetBodyRot = this.bodyInitialRot;
		this.canUseUpdatedIK = true;
	}

	// Token: 0x04004822 RID: 18466
	public static GorillaIK playerIK;

	// Token: 0x04004823 RID: 18467
	public Transform headBone;

	// Token: 0x04004824 RID: 18468
	public Transform bodyBone;

	// Token: 0x04004825 RID: 18469
	public Transform leftUpperArm;

	// Token: 0x04004826 RID: 18470
	public Transform leftLowerArm;

	// Token: 0x04004827 RID: 18471
	public Transform leftHand;

	// Token: 0x04004828 RID: 18472
	public Transform rightUpperArm;

	// Token: 0x04004829 RID: 18473
	public Transform rightLowerArm;

	// Token: 0x0400482A RID: 18474
	public Transform rightHand;

	// Token: 0x0400482B RID: 18475
	public Transform targetLeft;

	// Token: 0x0400482C RID: 18476
	public Transform targetRight;

	// Token: 0x0400482D RID: 18477
	public Transform targetHead;

	// Token: 0x0400482E RID: 18478
	public Quaternion initialUpperLeft;

	// Token: 0x0400482F RID: 18479
	public Quaternion initialLowerLeft;

	// Token: 0x04004830 RID: 18480
	public Quaternion initialUpperRight;

	// Token: 0x04004831 RID: 18481
	public Quaternion initialLowerRight;

	// Token: 0x04004832 RID: 18482
	[NonSerialized]
	public Quaternion targetBodyRot;

	// Token: 0x04004833 RID: 18483
	[NonSerialized]
	public Quaternion lerpBodyRot;

	// Token: 0x04004834 RID: 18484
	[NonSerialized]
	public Vector3 leftElbowDirection;

	// Token: 0x04004835 RID: 18485
	[NonSerialized]
	public Vector3 lerpLeftElbowDirection;

	// Token: 0x04004836 RID: 18486
	[NonSerialized]
	public Vector3 rightElbowDirection;

	// Token: 0x04004837 RID: 18487
	[NonSerialized]
	public Vector3 lerpRightElbowDirection;

	// Token: 0x04004838 RID: 18488
	public bool usingUpdatedIK;

	// Token: 0x04004839 RID: 18489
	public bool canUseUpdatedIK;

	// Token: 0x0400483A RID: 18490
	public Quaternion bodyOffsetRotation;

	// Token: 0x0400483B RID: 18491
	public OVRSkeleton skeleton;

	// Token: 0x0400483C RID: 18492
	private Transform[] boneXforms;

	// Token: 0x0400483D RID: 18493
	[NonSerialized]
	public Quaternion bodyInitialRot;

	// Token: 0x0400483E RID: 18494
	public Transform projectedBodyRotation;

	// Token: 0x0400483F RID: 18495
	public Transform projectedLeftShoulderPosition;

	// Token: 0x04004840 RID: 18496
	public Transform projectedRightShoulderPosition;

	// Token: 0x04004841 RID: 18497
	[NonSerialized]
	public VRRig myRig;

	// Token: 0x04004842 RID: 18498
	public float biasDistance = 0.2f;

	// Token: 0x04004843 RID: 18499
	private bool hasLeftOverride;

	// Token: 0x04004844 RID: 18500
	private Vector3 leftOverrideWorldPos;

	// Token: 0x04004845 RID: 18501
	private bool hasRightOverride;

	// Token: 0x04004846 RID: 18502
	private Vector3 rightOverrideWorldPos;

	// Token: 0x04004848 RID: 18504
	private Transform body;

	// Token: 0x04004849 RID: 18505
	private Transform leftArmUpper;

	// Token: 0x0400484A RID: 18506
	private Transform leftArmLower;

	// Token: 0x0400484B RID: 18507
	private Transform rightArmUpper;

	// Token: 0x0400484C RID: 18508
	private Transform rightArmLower;
}
