using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag.Shared.Scripts.Utilities;
using TagEffects;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x020003D5 RID: 981
[DefaultExecutionOrder(10000)]
public class HandEffectsTriggerRegistry : MonoBehaviour, ITickSystemTick, ITickSystemPost
{
	// Token: 0x17000243 RID: 579
	// (get) Token: 0x0600175C RID: 5980 RVA: 0x000864D3 File Offset: 0x000846D3
	// (set) Token: 0x0600175D RID: 5981 RVA: 0x000864DB File Offset: 0x000846DB
	public bool TickRunning { get; set; }

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x0600175E RID: 5982 RVA: 0x000864E4 File Offset: 0x000846E4
	// (set) Token: 0x0600175F RID: 5983 RVA: 0x000864EC File Offset: 0x000846EC
	public bool PostTickRunning { get; set; }

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x06001760 RID: 5984 RVA: 0x000864F5 File Offset: 0x000846F5
	// (set) Token: 0x06001761 RID: 5985 RVA: 0x000864FC File Offset: 0x000846FC
	public static HandEffectsTriggerRegistry Instance { get; private set; }

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x06001762 RID: 5986 RVA: 0x00086504 File Offset: 0x00084704
	// (set) Token: 0x06001763 RID: 5987 RVA: 0x0008650B File Offset: 0x0008470B
	public static bool HasInstance { get; private set; }

	// Token: 0x06001764 RID: 5988 RVA: 0x00086513 File Offset: 0x00084713
	public static void FindInstance()
	{
		HandEffectsTriggerRegistry.Instance = Object.FindAnyObjectByType<HandEffectsTriggerRegistry>();
		HandEffectsTriggerRegistry.HasInstance = true;
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x00086528 File Offset: 0x00084728
	private void Awake()
	{
		HandEffectsTriggerRegistry.Instance = this;
		HandEffectsTriggerRegistry.HasInstance = true;
		this.job = new HandEffectsTriggerRegistry.HandEffectsJob
		{
			positionInput = new NativeArray<Vector3>(50, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			closeOutput = new NativeArray<bool>(2500, Allocator.Persistent, NativeArrayOptions.ClearMemory),
			actualListSize = this.actualListSz
		};
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x00086580 File Offset: 0x00084780
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x0008658E File Offset: 0x0008478E
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x0008659C File Offset: 0x0008479C
	public void Register(IHandEffectsTrigger trigger)
	{
		if (this.triggers.Count < 50)
		{
			this.actualListSz++;
			this.triggers.Add(trigger);
		}
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x000865C8 File Offset: 0x000847C8
	public void Unregister(IHandEffectsTrigger trigger)
	{
		int num = this.triggers.IndexOf(trigger);
		if (num >= 0)
		{
			this.actualListSz--;
			this.triggers.RemoveAt(num);
		}
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x00086600 File Offset: 0x00084800
	private void OnDestroy()
	{
		if (!this.jobHandle.IsCompleted)
		{
			this.jobHandle.Complete();
		}
		this.job.Dispose();
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x00086628 File Offset: 0x00084828
	public void Tick()
	{
		this.CopyInput();
		this.jobHandle = this.job.Schedule(this.actualListSz, 20, default(JobHandle));
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0008665D File Offset: 0x0008485D
	public void PostTick()
	{
		this.jobHandle.Complete();
		this.CheckForHandEffectOnProcessedOutput();
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00086670 File Offset: 0x00084870
	public void CheckForHandEffectOnProcessedOutput()
	{
		this.newCollisionBits.Clear();
		for (int i = 0; i < this.triggers.Count; i++)
		{
			IHandEffectsTrigger handEffectsTrigger = this.triggers[i];
			int num = i * 50;
			for (int j = i + 1; j < this.triggers.Count; j++)
			{
				if (this.job.closeOutput[i * 50 + j])
				{
					IHandEffectsTrigger handEffectsTrigger2 = this.triggers[j];
					if (handEffectsTrigger.InTriggerZone(handEffectsTrigger2) || handEffectsTrigger2.InTriggerZone(handEffectsTrigger))
					{
						int idx = num + j;
						this.newCollisionBits[idx] = true;
						if (!this.existingCollisionBits[idx] && Time.time - this.triggerTimes[i] > 0.5f && Time.time - this.triggerTimes[j] > 0.5f)
						{
							handEffectsTrigger.OnTriggerEntered(handEffectsTrigger2);
							handEffectsTrigger2.OnTriggerEntered(handEffectsTrigger);
							this.triggerTimes[i] = (this.triggerTimes[j] = Time.time);
						}
					}
				}
			}
		}
		this.existingCollisionBits.CopyFrom(this.newCollisionBits);
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x00086798 File Offset: 0x00084998
	private void CopyInput()
	{
		for (int i = 0; i < this.actualListSz; i++)
		{
			this.job.positionInput[i] = this.triggers[i].Transform.position;
		}
		if (this.job.actualListSize != this.actualListSz)
		{
			this.job.actualListSize = this.actualListSz;
		}
	}

	// Token: 0x04002293 RID: 8851
	private const int MAX_TRIGGERS = 50;

	// Token: 0x04002294 RID: 8852
	private const int BIT_ARRAY_SIZE = 2500;

	// Token: 0x04002295 RID: 8853
	private const float COOLDOWN_TIME = 0.5f;

	// Token: 0x04002296 RID: 8854
	private const float DEFAULT_RADIUS = 0.5f;

	// Token: 0x04002297 RID: 8855
	private readonly List<IHandEffectsTrigger> triggers = new List<IHandEffectsTrigger>();

	// Token: 0x04002298 RID: 8856
	private readonly float[] triggerTimes = new float[50];

	// Token: 0x04002299 RID: 8857
	private readonly GTBitArray existingCollisionBits = new GTBitArray(2500);

	// Token: 0x0400229A RID: 8858
	private readonly GTBitArray newCollisionBits = new GTBitArray(2500);

	// Token: 0x0400229B RID: 8859
	private int actualListSz;

	// Token: 0x0400229C RID: 8860
	private JobHandle jobHandle;

	// Token: 0x0400229D RID: 8861
	private HandEffectsTriggerRegistry.HandEffectsJob job;

	// Token: 0x020003D6 RID: 982
	[BurstCompile]
	private struct HandEffectsJob : IJobParallelFor, IDisposable
	{
		// Token: 0x06001770 RID: 6000 RVA: 0x00086844 File Offset: 0x00084A44
		public void Execute(int i)
		{
			for (int j = i + 1; j < this.actualListSize; j++)
			{
				this.closeOutput[i * 50 + j] = (this.positionInput[i] - this.positionInput[j]).IsShorterThan(0.5f);
			}
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x0008689C File Offset: 0x00084A9C
		public void Dispose()
		{
			this.positionInput.Dispose();
			this.closeOutput.Dispose();
		}

		// Token: 0x040022A2 RID: 8866
		[NativeDisableParallelForRestriction]
		public NativeArray<Vector3> positionInput;

		// Token: 0x040022A3 RID: 8867
		[NativeDisableParallelForRestriction]
		public NativeArray<bool> closeOutput;

		// Token: 0x040022A4 RID: 8868
		public int actualListSize;
	}
}
