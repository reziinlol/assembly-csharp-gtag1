using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x0200086F RID: 2159
public class GorillaIKMgr : MonoBehaviour
{
	// Token: 0x170004F7 RID: 1271
	// (get) Token: 0x0600384E RID: 14414 RVA: 0x00133BF9 File Offset: 0x00131DF9
	public static GorillaIKMgr Instance
	{
		get
		{
			return GorillaIKMgr._instance;
		}
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x00133C00 File Offset: 0x00131E00
	private void Awake()
	{
		GorillaIKMgr._instance = this;
		this.firstFrame = true;
		this.tAA = new TransformAccessArray(0, -1);
		this.transformList = new List<Transform>();
		this.job = new GorillaIKMgr.IKJob
		{
			constantInput = new NativeArray<GorillaIKMgr.IKConstantInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			input = new NativeArray<GorillaIKMgr.IKInput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			output = new NativeArray<GorillaIKMgr.IKOutput>(40, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
		this.jobXform = new GorillaIKMgr.IKTransformJob
		{
			transformRotations = new NativeArray<Quaternion>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			transformPositions = new NativeArray<Vector3>(160, Allocator.Persistent, NativeArrayOptions.ClearMemory)
		};
	}

	// Token: 0x06003850 RID: 14416 RVA: 0x00133CA8 File Offset: 0x00131EA8
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.jobXformHandle.Complete();
		this.jobXform.transformRotations.Dispose();
		this.jobXform.transformPositions.Dispose();
		this.tAA.Dispose();
		this.job.input.Dispose();
		this.job.constantInput.Dispose();
		this.job.output.Dispose();
	}

	// Token: 0x06003851 RID: 14417 RVA: 0x00133D28 File Offset: 0x00131F28
	public void RegisterIK(GorillaIK ik)
	{
		this.ikList.Add(ik);
		this.actualListSz += 2;
		this.updatedSinceLastRun = true;
		if (this.job.constantInput.IsCreated)
		{
			this.SetConstantData(ik, this.actualListSz - 2);
		}
	}

	// Token: 0x06003852 RID: 14418 RVA: 0x00133D78 File Offset: 0x00131F78
	public void DeregisterIK(GorillaIK ik)
	{
		int num = this.ikList.FindIndex((GorillaIK curr) => curr == ik);
		this.updatedSinceLastRun = true;
		this.ikList.RemoveAt(num);
		this.actualListSz -= 2;
		if (this.job.constantInput.IsCreated)
		{
			for (int i = num; i < this.actualListSz; i++)
			{
				this.job.constantInput[i] = this.job.constantInput[i + 2];
			}
		}
	}

	// Token: 0x06003853 RID: 14419 RVA: 0x00133E14 File Offset: 0x00132014
	private void SetConstantData(GorillaIK ik, int index)
	{
		this.job.constantInput[index] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerLeft,
			initRotUpper = ik.initialUpperLeft,
			shoulderPosition = new Vector3(-0.018300775f, -0.04206751f, 0.08612572f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.59150106f, 0.3665933f, 0.20795153f, 0.68738055f)
		};
		this.job.constantInput[index + 1] = new GorillaIKMgr.IKConstantInput
		{
			initRotLower = ik.initialLowerRight,
			initRotUpper = ik.initialUpperRight,
			shoulderPosition = new Vector3(0.018300813f, -0.042066876f, 0.08613044f),
			bodyPivotPos = new Vector3(0f, 0.011406422f, 1.6582015f),
			shoulderRot = new Quaternion(-0.591501f, -0.3665933f, -0.20795153f, 0.6873807f)
		};
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x00133F38 File Offset: 0x00132138
	private void CopyInput()
	{
		int num = 0;
		int i = 0;
		while (i < this.actualListSz)
		{
			GorillaIK gorillaIK = this.ikList[i / 2];
			bool flag = gorillaIK.usingUpdatedIK && SubscriptionManager.GetSubscriptionDetails(gorillaIK.myRig).active;
			if (gorillaIK != GorillaIKMgr.playerIK)
			{
				gorillaIK.lerpLeftElbowDirection = Vector3.Lerp(gorillaIK.lerpLeftElbowDirection, gorillaIK.leftElbowDirection, this.lerpValue);
				gorillaIK.lerpRightElbowDirection = Vector3.Lerp(gorillaIK.lerpRightElbowDirection, gorillaIK.rightElbowDirection, this.lerpValue);
				gorillaIK.lerpBodyRot = (flag ? Quaternion.Lerp(gorillaIK.lerpBodyRot, gorillaIK.targetBodyRot, this.lerpValue) : gorillaIK.bodyInitialRot);
			}
			else
			{
				gorillaIK.lerpLeftElbowDirection = gorillaIK.leftElbowDirection;
				gorillaIK.lerpRightElbowDirection = gorillaIK.rightElbowDirection;
				gorillaIK.lerpBodyRot = (flag ? gorillaIK.targetBodyRot : gorillaIK.bodyInitialRot);
			}
			this.job.input[i] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Left(flag),
				elbowDir = gorillaIK.lerpLeftElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = flag
			};
			this.job.input[i + 1] = new GorillaIKMgr.IKInput
			{
				targetPos = gorillaIK.GetShoulderLocalTargetPos_Right(flag),
				elbowDir = gorillaIK.lerpRightElbowDirection,
				bodyRot = gorillaIK.lerpBodyRot,
				usingNewIK = flag
			};
			gorillaIK.ClearOverrides();
			i += 2;
			num++;
		}
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x001340CC File Offset: 0x001322CC
	private void CopyOutput()
	{
		bool flag = false;
		if (this.updatedSinceLastRun || this.tAA.length != this.ikList.Count * 8)
		{
			flag = true;
			this.tAA.Dispose();
			this.transformList.Clear();
		}
		for (int i = 0; i < this.ikList.Count; i++)
		{
			GorillaIK gorillaIK = this.ikList[i];
			if (flag || this.updatedSinceLastRun)
			{
				this.transformList.Add(gorillaIK.leftUpperArm);
				this.transformList.Add(gorillaIK.leftLowerArm);
				this.transformList.Add(gorillaIK.rightUpperArm);
				this.transformList.Add(gorillaIK.rightLowerArm);
				this.transformList.Add(gorillaIK.bodyBone);
				this.transformList.Add(gorillaIK.headBone);
				this.transformList.Add(gorillaIK.leftHand);
				this.transformList.Add(gorillaIK.rightHand);
			}
			this.jobXform.transformRotations[8 * i] = this.job.output[i * 2].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 1] = this.job.output[i * 2].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 2] = this.job.output[i * 2 + 1].upperArmLocalRot;
			this.jobXform.transformRotations[8 * i + 3] = this.job.output[i * 2 + 1].lowerArmLocalRot;
			this.jobXform.transformRotations[8 * i + 4] = gorillaIK.lerpBodyRot;
			this.jobXform.transformRotations[8 * i + 5] = gorillaIK.targetHead.rotation;
			this.jobXform.transformRotations[8 * i + 6] = gorillaIK.targetLeft.rotation;
			this.jobXform.transformRotations[8 * i + 7] = gorillaIK.targetRight.rotation;
			this.jobXform.transformPositions[8 * i + 6] = this.job.output[i * 2].handLocalPosition;
			this.jobXform.transformPositions[8 * i + 7] = this.job.output[i * 2 + 1].handLocalPosition;
		}
		if (flag)
		{
			this.tAA = new TransformAccessArray(this.transformList.ToArray(), -1);
		}
		this.updatedSinceLastRun = false;
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x00134384 File Offset: 0x00132584
	public void LateUpdate()
	{
		GorillaIK gorillaIK = GorillaIKMgr.playerIK;
		if (gorillaIK != null)
		{
			gorillaIK.SkeletonUpdate();
		}
		if (!this.firstFrame)
		{
			this.jobXformHandle.Complete();
		}
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
		this.jobHandle.Complete();
		this.CopyOutput();
		this.jobXformHandle = this.jobXform.Schedule(this.tAA, default(JobHandle));
		this.firstFrame = false;
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x00134414 File Offset: 0x00132614
	public static void AddPlayerIK(GorillaIK _playerIK)
	{
		GorillaIKMgr.playerIK = _playerIK;
	}

	// Token: 0x0400484D RID: 18509
	[OnEnterPlay_SetNull]
	private static GorillaIKMgr _instance;

	// Token: 0x0400484E RID: 18510
	private const int MaxSize = 20;

	// Token: 0x0400484F RID: 18511
	private List<GorillaIK> ikList = new List<GorillaIK>(20);

	// Token: 0x04004850 RID: 18512
	private int actualListSz;

	// Token: 0x04004851 RID: 18513
	private JobHandle jobHandle;

	// Token: 0x04004852 RID: 18514
	private JobHandle jobXformHandle;

	// Token: 0x04004853 RID: 18515
	private bool firstFrame = true;

	// Token: 0x04004854 RID: 18516
	private TransformAccessArray tAA;

	// Token: 0x04004855 RID: 18517
	private List<Transform> transformList;

	// Token: 0x04004856 RID: 18518
	private bool updatedSinceLastRun;

	// Token: 0x04004857 RID: 18519
	public const int tFormCount = 8;

	// Token: 0x04004858 RID: 18520
	public static GorillaIK playerIK;

	// Token: 0x04004859 RID: 18521
	private float lerpValue = 0.155f;

	// Token: 0x0400485A RID: 18522
	private GorillaIKMgr.IKJob job;

	// Token: 0x0400485B RID: 18523
	private GorillaIKMgr.IKTransformJob jobXform;

	// Token: 0x02000870 RID: 2160
	private struct IKConstantInput
	{
		// Token: 0x0400485C RID: 18524
		public Quaternion initRotLower;

		// Token: 0x0400485D RID: 18525
		public Quaternion initRotUpper;

		// Token: 0x0400485E RID: 18526
		public Vector3 shoulderPosition;

		// Token: 0x0400485F RID: 18527
		public Vector3 bodyPivotPos;

		// Token: 0x04004860 RID: 18528
		public Quaternion bodyStartRot;

		// Token: 0x04004861 RID: 18529
		public Quaternion shoulderRot;
	}

	// Token: 0x02000871 RID: 2161
	private struct IKInput
	{
		// Token: 0x04004862 RID: 18530
		public bool usingNewIK;

		// Token: 0x04004863 RID: 18531
		public Vector3 targetPos;

		// Token: 0x04004864 RID: 18532
		public Vector3 elbowDir;

		// Token: 0x04004865 RID: 18533
		public Quaternion bodyRot;
	}

	// Token: 0x02000872 RID: 2162
	private struct IKOutput
	{
		// Token: 0x06003859 RID: 14425 RVA: 0x00134443 File Offset: 0x00132643
		public IKOutput(Quaternion upperArmLocalRot_, Quaternion lowerArmLocalRot_, Vector3 _handLocalPosition)
		{
			this.upperArmLocalRot = upperArmLocalRot_;
			this.lowerArmLocalRot = lowerArmLocalRot_;
			this.handLocalPosition = _handLocalPosition;
		}

		// Token: 0x04004866 RID: 18534
		public Quaternion upperArmLocalRot;

		// Token: 0x04004867 RID: 18535
		public Quaternion lowerArmLocalRot;

		// Token: 0x04004868 RID: 18536
		public Vector3 handLocalPosition;
	}

	// Token: 0x02000873 RID: 2163
	[BurstCompile]
	private struct IKJob : IJobParallelFor
	{
		// Token: 0x0600385A RID: 14426 RVA: 0x0013445C File Offset: 0x0013265C
		public void Execute(int i)
		{
			Quaternion initRotUpper = this.constantInput[i].initRotUpper;
			Vector3 vector = GorillaIKMgr.IKJob.upperArmLocalPos;
			Quaternion rotation = initRotUpper * this.constantInput[i].initRotLower;
			Vector3 vector2 = vector + initRotUpper * GorillaIKMgr.IKJob.forearmLocalPos;
			Vector3 vector3 = vector2 + rotation * GorillaIKMgr.IKJob.handLocalPos;
			float num = 0.001f;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (vector2 - vector3).magnitude;
			float max = magnitude + magnitude2 - num;
			Vector3 normalized = (vector3 - vector).normalized;
			Vector3 normalized2 = (vector2 - vector).normalized;
			Vector3 normalized3 = (vector3 - vector2).normalized;
			Vector3 normalized4 = (this.input[i].targetPos - vector).normalized;
			float num2 = Mathf.Clamp((this.input[i].targetPos - vector).magnitude, num, max);
			float num3 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized2), -1f, 1f));
			float num4 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(-normalized2, normalized3), -1f, 1f));
			float num5 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, normalized4), -1f, 1f));
			float num6 = Mathf.Acos(Mathf.Clamp((magnitude2 * magnitude2 - magnitude * magnitude - num2 * num2) / (-2f * magnitude * num2), -1f, 1f));
			float num7 = Mathf.Acos(Mathf.Clamp((num2 * num2 - magnitude * magnitude - magnitude2 * magnitude2) / (-2f * magnitude * magnitude2), -1f, 1f));
			Vector3 normalized5 = Vector3.Cross(normalized, normalized2).normalized;
			Vector3 normalized6 = Vector3.Cross(normalized, normalized4).normalized;
			Quaternion rhs = Quaternion.AngleAxis((num6 - num3) * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized5);
			Quaternion rhs2 = Quaternion.AngleAxis((num7 - num4) * 57.29578f, Quaternion.Inverse(rotation) * normalized5);
			Quaternion rhs3 = Quaternion.AngleAxis(num5 * 57.29578f, Quaternion.Inverse(initRotUpper) * normalized6);
			Quaternion quaternion = this.constantInput[i].initRotUpper * rhs3 * rhs;
			Quaternion quaternion2 = this.constantInput[i].initRotLower * rhs2;
			Quaternion quaternion3 = this.input[i].bodyRot * this.constantInput[i].shoulderRot;
			Quaternion quaternion4 = quaternion3 * quaternion;
			Quaternion rotation2 = quaternion4 * quaternion2;
			Vector3 handLocalPosition = this.constantInput[i].bodyPivotPos + this.input[i].bodyRot * this.constantInput[i].shoulderPosition + quaternion3 * GorillaIKMgr.IKJob.upperArmLocalPos + quaternion4 * GorillaIKMgr.IKJob.forearmLocalPos + rotation2 * GorillaIKMgr.IKJob.handLocalPos;
			if (!this.input[i].usingNewIK)
			{
				this.output[i] = new GorillaIKMgr.IKOutput(quaternion, quaternion2, handLocalPosition);
				return;
			}
			Vector3 normalized7 = this.input[i].elbowDir.normalized;
			Vector3 normalized8 = (vector + quaternion * GorillaIKMgr.IKJob.forearmLocalPos - vector).normalized;
			Vector3 normalized9 = Vector3.Cross(normalized4, normalized7).normalized;
			quaternion = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.Cross(normalized4, normalized8).normalized, normalized9, normalized4), normalized4) * quaternion;
			this.output[i] = new GorillaIKMgr.IKOutput(quaternion, quaternion2, handLocalPosition);
		}

		// Token: 0x04004869 RID: 18537
		public NativeArray<GorillaIKMgr.IKConstantInput> constantInput;

		// Token: 0x0400486A RID: 18538
		public NativeArray<GorillaIKMgr.IKInput> input;

		// Token: 0x0400486B RID: 18539
		public NativeArray<GorillaIKMgr.IKOutput> output;

		// Token: 0x0400486C RID: 18540
		private static readonly Vector3 upperArmLocalPos = new Vector3(0f, 0.1454885f, -0.02598158f);

		// Token: 0x0400486D RID: 18541
		private static readonly Vector3 forearmLocalPos = new Vector3(0f, 0.4061671f, 0f);

		// Token: 0x0400486E RID: 18542
		private static readonly Vector3 handLocalPos = new Vector3(0f, 0.3816895f, 0f);
	}

	// Token: 0x02000874 RID: 2164
	[BurstCompile]
	private struct IKTransformJob : IJobParallelForTransform
	{
		// Token: 0x0600385C RID: 14428 RVA: 0x001348C0 File Offset: 0x00132AC0
		public void Execute(int index, TransformAccess xform)
		{
			if (index % 8 <= 4)
			{
				xform.localRotation = this.transformRotations[index];
			}
			else
			{
				xform.rotation = this.transformRotations[index];
			}
			if (index % 8 >= 6)
			{
				xform.localPosition = this.transformPositions[index];
			}
		}

		// Token: 0x0400486F RID: 18543
		public NativeArray<Quaternion> transformRotations;

		// Token: 0x04004870 RID: 18544
		public NativeArray<Vector3> transformPositions;
	}
}
