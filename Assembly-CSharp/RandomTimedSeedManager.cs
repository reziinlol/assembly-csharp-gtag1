using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200036F RID: 879
[NetworkBehaviourWeaved(2)]
public class RandomTimedSeedManager : NetworkComponent, ITickSystemTick
{
	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06001581 RID: 5505 RVA: 0x00071FE2 File Offset: 0x000701E2
	// (set) Token: 0x06001582 RID: 5506 RVA: 0x00071FE9 File Offset: 0x000701E9
	public static RandomTimedSeedManager instance { get; private set; }

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06001583 RID: 5507 RVA: 0x00071FF1 File Offset: 0x000701F1
	// (set) Token: 0x06001584 RID: 5508 RVA: 0x00071FF9 File Offset: 0x000701F9
	public int seed { get; private set; }

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001585 RID: 5509 RVA: 0x00072002 File Offset: 0x00070202
	// (set) Token: 0x06001586 RID: 5510 RVA: 0x0007200A File Offset: 0x0007020A
	public float currentSyncTime { get; private set; }

	// Token: 0x06001587 RID: 5511 RVA: 0x00072013 File Offset: 0x00070213
	protected override void Awake()
	{
		base.Awake();
		RandomTimedSeedManager.instance = this;
		this.seed = Random.Range(-1000000, -1000000);
		this.idealSyncTime = 0f;
		this.currentSyncTime = 0f;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x00072052 File Offset: 0x00070252
	public void AddCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Add(callback);
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x00072060 File Offset: 0x00070260
	public void RemoveCallbackOnSeedChanged(Action callback)
	{
		this.callbacksOnSeedChanged.Remove(callback);
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x0600158A RID: 5514 RVA: 0x0007206F File Offset: 0x0007026F
	// (set) Token: 0x0600158B RID: 5515 RVA: 0x00072077 File Offset: 0x00070277
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x0600158C RID: 5516 RVA: 0x00072080 File Offset: 0x00070280
	void ITickSystemTick.Tick()
	{
		this.currentSyncTime += Time.deltaTime;
		this.idealSyncTime += Time.deltaTime;
		if (this.idealSyncTime > 1E+09f)
		{
			this.idealSyncTime -= 1E+09f;
			this.currentSyncTime -= 1E+09f;
		}
		if (!base.GetView.AmOwner)
		{
			this.currentSyncTime = Mathf.Lerp(this.currentSyncTime, this.idealSyncTime, 0.1f);
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x0600158D RID: 5517 RVA: 0x0007210B File Offset: 0x0007030B
	// (set) Token: 0x0600158E RID: 5518 RVA: 0x00072135 File Offset: 0x00070335
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe RandomTimedSeedManager.RandomTimedSeedManagerData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing RandomTimedSeedManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(RandomTimedSeedManager.RandomTimedSeedManagerData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x00072160 File Offset: 0x00070360
	public override void WriteDataFusion()
	{
		this.Data = new RandomTimedSeedManager.RandomTimedSeedManagerData(this.seed, this.currentSyncTime);
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0007217C File Offset: 0x0007037C
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data.seed, this.Data.currentSyncTime);
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x000721AB File Offset: 0x000703AB
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.seed);
		stream.SendNext(this.currentSyncTime);
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x000721E0 File Offset: 0x000703E0
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int seedVal = (int)stream.ReceiveNext();
		float testTime = (float)stream.ReceiveNext();
		this.ReadDataShared(seedVal, testTime);
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x0007221C File Offset: 0x0007041C
	private void ReadDataShared(int seedVal, float testTime)
	{
		if (!float.IsFinite(testTime))
		{
			return;
		}
		this.seed = seedVal;
		if (testTime >= 0f && testTime <= 1E+09f)
		{
			if (this.idealSyncTime - testTime > 500000000f)
			{
				this.currentSyncTime = testTime;
			}
			this.idealSyncTime = testTime;
		}
		if (this.seed != this.cachedSeed && this.seed >= -1000000 && this.seed <= -1000000)
		{
			this.currentSyncTime = this.idealSyncTime;
			this.cachedSeed = this.seed;
			foreach (Action action in this.callbacksOnSeedChanged)
			{
				action();
			}
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000722FF File Offset: 0x000704FF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x00072317 File Offset: 0x00070517
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04001A58 RID: 6744
	private List<Action> callbacksOnSeedChanged = new List<Action>();

	// Token: 0x04001A5A RID: 6746
	private float idealSyncTime;

	// Token: 0x04001A5C RID: 6748
	private int cachedSeed;

	// Token: 0x04001A5D RID: 6749
	private const int SeedMin = -1000000;

	// Token: 0x04001A5E RID: 6750
	private const int SeedMax = -1000000;

	// Token: 0x04001A5F RID: 6751
	private const float MaxSyncTime = 1E+09f;

	// Token: 0x04001A61 RID: 6753
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private RandomTimedSeedManager.RandomTimedSeedManagerData _Data;

	// Token: 0x02000370 RID: 880
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct RandomTimedSeedManagerData : INetworkStruct
	{
		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x0007232B File Offset: 0x0007052B
		// (set) Token: 0x06001598 RID: 5528 RVA: 0x00072339 File Offset: 0x00070539
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe int seed
		{
			readonly get
			{
				return *(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed);
			}
			set
			{
				*(int*)Native.ReferenceToPointer<FixedStorage@1>(ref this._seed) = value;
			}
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06001599 RID: 5529 RVA: 0x00072348 File Offset: 0x00070548
		// (set) Token: 0x0600159A RID: 5530 RVA: 0x00072356 File Offset: 0x00070556
		[Networked]
		[NetworkedWeaved(1, 1)]
		public unsafe float currentSyncTime
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._currentSyncTime) = value;
			}
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x00072365 File Offset: 0x00070565
		public RandomTimedSeedManagerData(int seed, float currentSyncTime)
		{
			this.seed = seed;
			this.currentSyncTime = currentSyncTime;
		}

		// Token: 0x04001A62 RID: 6754
		[FixedBufferProperty(typeof(int), typeof(UnityValueSurrogate@ElementReaderWriterInt32), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@1 _seed;

		// Token: 0x04001A63 RID: 6755
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(4)]
		private FixedStorage@1 _currentSyncTime;
	}
}
