using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020008DF RID: 2271
public class HandTrackingFingerCurl : MonoBehaviour
{
	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06003B67 RID: 15207 RVA: 0x00145944 File Offset: 0x00143B44
	// (set) Token: 0x06003B68 RID: 15208 RVA: 0x0014594C File Offset: 0x00143B4C
	public float ThumbCurl { get; private set; }

	// Token: 0x17000549 RID: 1353
	// (get) Token: 0x06003B69 RID: 15209 RVA: 0x00145955 File Offset: 0x00143B55
	// (set) Token: 0x06003B6A RID: 15210 RVA: 0x0014595D File Offset: 0x00143B5D
	public float TriggerCurl { get; private set; }

	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x06003B6B RID: 15211 RVA: 0x00145966 File Offset: 0x00143B66
	// (set) Token: 0x06003B6C RID: 15212 RVA: 0x0014596E File Offset: 0x00143B6E
	public float GripCurl { get; private set; }

	// Token: 0x06003B6D RID: 15213 RVA: 0x00145977 File Offset: 0x00143B77
	private void Awake()
	{
		if (this.isLeft)
		{
			HandTrackingFingerCurl.leftCurl = this;
		}
		else
		{
			HandTrackingFingerCurl.rightCurl = this;
		}
		if (this.skeleton == null)
		{
			this.skeleton = base.GetComponent<OVRSkeleton>();
		}
		this.boneXforms = new Transform[84];
	}

	// Token: 0x06003B6E RID: 15214 RVA: 0x001459B8 File Offset: 0x00143BB8
	private void LateUpdate()
	{
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (!SubscriptionManager.IsLocalSubscribed() || !SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.HandTracking))
		{
			return;
		}
		if (this.boneXforms[0] == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
		}
		this.ThumbCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb0);
		this.TriggerCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index1);
		this.GripCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Middle3);
	}

	// Token: 0x06003B6F RID: 15215 RVA: 0x00145A9C File Offset: 0x00143C9C
	private float CalcFingerCurl(OVRSkeleton.BoneId distal, OVRSkeleton.BoneId intermediate, OVRSkeleton.BoneId proximal, OVRSkeleton.BoneId metacarpal)
	{
		Transform transform = this.boneXforms[(int)distal];
		Transform transform2 = this.boneXforms[(int)intermediate];
		Transform transform3 = this.boneXforms[(int)proximal];
		Transform transform4 = this.boneXforms[(int)metacarpal];
		if (transform == null || transform2 == null || transform3 == null || transform4 == null)
		{
			return 0f;
		}
		Vector3 from = transform.position - transform2.position;
		Vector3 vector = transform2.position - transform3.position;
		Vector3 to = transform3.position - transform4.position;
		float num = Vector3.Angle(from, vector);
		float num2 = Vector3.Angle(vector, to);
		float num3 = (num + num2) * 0.5f;
		num3 *= this.CurlMultiplier;
		num3 = Mathf.InverseLerp(this.ActivationStart, this.ActivationEnd, num3);
		return Mathf.Clamp01(num3);
	}

	// Token: 0x04004BEB RID: 19435
	[SerializeField]
	private OVRSkeleton skeleton;

	// Token: 0x04004BEC RID: 19436
	public float ActivationStart = 5f;

	// Token: 0x04004BED RID: 19437
	public float ActivationEnd = 95f;

	// Token: 0x04004BEE RID: 19438
	public float CurlMultiplier = 1.2f;

	// Token: 0x04004BF2 RID: 19442
	private Transform[] boneXforms;

	// Token: 0x04004BF3 RID: 19443
	public static HandTrackingFingerCurl leftCurl;

	// Token: 0x04004BF4 RID: 19444
	public static HandTrackingFingerCurl rightCurl;

	// Token: 0x04004BF5 RID: 19445
	[SerializeField]
	private bool isLeft;
}
