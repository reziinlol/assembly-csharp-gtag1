using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000AB0 RID: 2736
[DefaultExecutionOrder(0)]
public class VRRigJobManager : MonoBehaviour
{
	// Token: 0x1700066C RID: 1644
	// (get) Token: 0x06004604 RID: 17924 RVA: 0x0017AC05 File Offset: 0x00178E05
	public static VRRigJobManager Instance
	{
		get
		{
			return VRRigJobManager._instance;
		}
	}

	// Token: 0x06004605 RID: 17925 RVA: 0x0017AC0C File Offset: 0x00178E0C
	private void Awake()
	{
		VRRigJobManager._instance = this;
		this.cachedInput = new NativeArray<VRRigJobManager.VRRigTransformInput>(19, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.tAA = new TransformAccessArray(19, 2);
		this.job = default(VRRigJobManager.VRRigTransformJob);
	}

	// Token: 0x06004606 RID: 17926 RVA: 0x0017AC3D File Offset: 0x00178E3D
	private void OnDestroy()
	{
		this.jobHandle.Complete();
		this.cachedInput.Dispose();
		this.tAA.Dispose();
	}

	// Token: 0x06004607 RID: 17927 RVA: 0x0017AC60 File Offset: 0x00178E60
	public void RegisterVRRig(VRRig rig)
	{
		this.rigList.Add(rig);
		this.tAA.Add(rig.transform);
		this.actualListSz++;
	}

	// Token: 0x06004608 RID: 17928 RVA: 0x0017AC90 File Offset: 0x00178E90
	public void DeregisterVRRig(VRRig rig)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.rigList.Remove(rig);
		for (int i = this.actualListSz - 1; i >= 0; i--)
		{
			if (this.tAA[i] == rig.transform)
			{
				this.tAA.RemoveAtSwapBack(i);
				break;
			}
		}
		this.actualListSz--;
	}

	// Token: 0x06004609 RID: 17929 RVA: 0x0017ACFC File Offset: 0x00178EFC
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.cachedInput[i] = new VRRigJobManager.VRRigTransformInput
			{
				rigPosition = this.rigList[i].jobPos,
				rigRotaton = this.rigList[i].jobRotation
			};
			this.tAA[i] = this.rigList[i].transform;
		}
	}

	// Token: 0x0600460A RID: 17930 RVA: 0x0017AD7C File Offset: 0x00178F7C
	public void Update()
	{
		this.jobHandle.Complete();
		for (int i = 0; i < this.rigList.Count; i++)
		{
			this.rigList[i].RemoteRigUpdate();
		}
		this.CopyInput();
		this.job.input = this.cachedInput;
		this.jobHandle = this.job.Schedule(this.tAA, default(JobHandle));
	}

	// Token: 0x0400586B RID: 22635
	[OnEnterPlay_SetNull]
	private static VRRigJobManager _instance;

	// Token: 0x0400586C RID: 22636
	private const int MaxSize = 19;

	// Token: 0x0400586D RID: 22637
	private const int questJobThreads = 2;

	// Token: 0x0400586E RID: 22638
	private List<VRRig> rigList = new List<VRRig>(19);

	// Token: 0x0400586F RID: 22639
	private NativeArray<VRRigJobManager.VRRigTransformInput> cachedInput;

	// Token: 0x04005870 RID: 22640
	private TransformAccessArray tAA;

	// Token: 0x04005871 RID: 22641
	private int actualListSz;

	// Token: 0x04005872 RID: 22642
	private JobHandle jobHandle;

	// Token: 0x04005873 RID: 22643
	private VRRigJobManager.VRRigTransformJob job;

	// Token: 0x02000AB1 RID: 2737
	private struct VRRigTransformInput
	{
		// Token: 0x04005874 RID: 22644
		public Vector3 rigPosition;

		// Token: 0x04005875 RID: 22645
		public Quaternion rigRotaton;
	}

	// Token: 0x02000AB2 RID: 2738
	[BurstCompile]
	private struct VRRigTransformJob : IJobParallelForTransform
	{
		// Token: 0x0600460C RID: 17932 RVA: 0x0017AE07 File Offset: 0x00179007
		public void Execute(int i, TransformAccess tA)
		{
			if (i < this.input.Length)
			{
				tA.position = this.input[i].rigPosition;
				tA.rotation = this.input[i].rigRotaton;
			}
		}

		// Token: 0x04005876 RID: 22646
		[ReadOnly]
		public NativeArray<VRRigJobManager.VRRigTransformInput> input;
	}
}
