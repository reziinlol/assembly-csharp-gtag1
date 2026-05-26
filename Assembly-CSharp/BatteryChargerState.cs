using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000197 RID: 407
[RequireComponent(typeof(XSceneRefTarget))]
[NetworkBehaviourWeaved(62)]
public class BatteryChargerState : NetworkComponent
{
	// Token: 0x14000018 RID: 24
	// (add) Token: 0x06000AFB RID: 2811 RVA: 0x0003AC20 File Offset: 0x00038E20
	// (remove) Token: 0x06000AFC RID: 2812 RVA: 0x0003AC58 File Offset: 0x00038E58
	internal event Action onChargeChanged;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000AFD RID: 2813 RVA: 0x0003AC90 File Offset: 0x00038E90
	// (remove) Token: 0x06000AFE RID: 2814 RVA: 0x0003ACC8 File Offset: 0x00038EC8
	internal event Action onFullyCharged;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000AFF RID: 2815 RVA: 0x0003AD00 File Offset: 0x00038F00
	// (remove) Token: 0x06000B00 RID: 2816 RVA: 0x0003AD38 File Offset: 0x00038F38
	internal event Action<int> onEventPhaseChanged;

	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000B01 RID: 2817 RVA: 0x0003AD6D File Offset: 0x00038F6D
	internal float CurrentCharge
	{
		get
		{
			return this.currentCharge;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000B02 RID: 2818 RVA: 0x0003AD75 File Offset: 0x00038F75
	internal float MaxCharge
	{
		get
		{
			return this.maxCharge;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000B03 RID: 2819 RVA: 0x0003AD7D File Offset: 0x00038F7D
	internal float ChargePercent
	{
		get
		{
			if (this.maxCharge <= 0f)
			{
				return 0f;
			}
			return this.currentCharge / this.maxCharge;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000B04 RID: 2820 RVA: 0x0003AD9F File Offset: 0x00038F9F
	internal float ChargePerCrankDegree
	{
		get
		{
			return this.chargePerCrankDegree;
		}
	}

	// Token: 0x17000108 RID: 264
	// (get) Token: 0x06000B05 RID: 2821 RVA: 0x0003ADA7 File Offset: 0x00038FA7
	internal int EventPhase
	{
		get
		{
			return this.eventPhase;
		}
	}

	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000B06 RID: 2822 RVA: 0x00038BCD File Offset: 0x00036DCD
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x0003ADAF File Offset: 0x00038FAF
	public void SetChargePerCrankDegree(float chargeRate)
	{
		this.chargePerCrankDegree = chargeRate;
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x0003ADB8 File Offset: 0x00038FB8
	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < 20; i++)
		{
			this.crankSyncs[i].holderActorNr = -1;
		}
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x0003ADEC File Offset: 0x00038FEC
	private void Update()
	{
		if (this.activeCrankerCount <= 0 && this.currentCharge > 0f)
		{
			this.currentCharge = Mathf.Max(0f, this.currentCharge - this.drainPerSecond * Time.deltaTime);
			Action action = this.onChargeChanged;
			if (action != null)
			{
				action();
			}
		}
		this.activeCrankerCount = 0;
		for (int i = 0; i < 20; i++)
		{
			if (this.crankSyncs[i].holderActorNr != -1)
			{
				this.activeCrankerCount++;
			}
		}
	}

	// Token: 0x06000B0A RID: 2826 RVA: 0x0003AE7C File Offset: 0x0003907C
	internal void UpdateLocalCrankState(int crankIndex, bool isLeftHand, float angle)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[crankIndex];
		int localActorNr = this.LocalActorNr;
		if (ptr.holderActorNr == localActorNr)
		{
			ptr.isLeftHand = isLeftHand;
			ptr.angle = angle;
		}
	}

	// Token: 0x06000B0B RID: 2827 RVA: 0x0003AEC0 File Offset: 0x000390C0
	internal static VRRig FindRigForActor(int actorNr)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(actorNr, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0003AEE4 File Offset: 0x000390E4
	internal bool NotifyCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return false;
		}
		ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[crankIndex];
		if (ptr.holderActorNr != -1)
		{
			return false;
		}
		ptr.holderActorNr = this.LocalActorNr;
		this.pendingGrabTime[crankIndex] = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_BatteryMessage", RpcTarget.MasterClient, new object[]
			{
				isLeftHand ? 0 : 1,
				(byte)crankIndex,
				0f
			});
		}
		return true;
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x0003AF70 File Offset: 0x00039170
	internal void NotifyCrankReleased(int crankIndex, float finalAngle)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		this.FlushCrankRPC();
		BatteryChargerState.CrankSyncState[] array = this.crankSyncs;
		array[crankIndex].holderActorNr = -1;
		array[crankIndex].angle = finalAngle;
		this.pendingGrabTime[crankIndex] = 0f;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_BatteryMessage", RpcTarget.All, new object[]
			{
				2,
				(byte)crankIndex,
				finalAngle
			});
		}
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x0003AFE8 File Offset: 0x000391E8
	internal void NotifyCrankInput(int crankIndex, float degrees)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		float num = Mathf.Abs(degrees) * this.chargePerCrankDegree;
		if (num <= 0f)
		{
			return;
		}
		float num2 = this.currentCharge;
		this.currentCharge = Mathf.Clamp(this.currentCharge + num, 0f, this.maxCharge);
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (num2 < this.maxCharge && this.currentCharge >= this.maxCharge)
		{
			Action action2 = this.onFullyCharged;
			if (action2 != null)
			{
				action2();
			}
		}
		if (PhotonNetwork.InRoom)
		{
			this.pendingCrankCharge = this.currentCharge;
			this.pendingCrankIndex = crankIndex;
			if (Time.time >= this.nextCrankRPCTimestamp)
			{
				this.FlushCrankRPC();
			}
		}
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0003B0A0 File Offset: 0x000392A0
	private void FlushCrankRPC()
	{
		if (this.pendingCrankIndex < 0 || this.pendingCrankCharge <= 0f)
		{
			return;
		}
		base.SendRPC("RPC_BatteryMessage", RpcTarget.MasterClient, new object[]
		{
			3,
			(byte)this.pendingCrankIndex,
			this.pendingCrankCharge
		});
		this.nextCrankRPCTimestamp = Time.time + 1f;
		this.pendingCrankIndex = -1;
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0003B114 File Offset: 0x00039314
	public void DisableNetworking()
	{
		this.m_disableNetworking = true;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0003B11D File Offset: 0x0003931D
	public void EnableNetworking()
	{
		this.m_disableNetworking = false;
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003B128 File Offset: 0x00039328
	[PunRPC]
	public void RPC_BatteryMessage(byte msgType, byte crankIndex, float floatParam, PhotonMessageInfo info)
	{
		if (this.m_disableNetworking)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "RPC_BatteryMessage");
		if (crankIndex >= 20)
		{
			return;
		}
		switch (msgType)
		{
		case 0:
		case 1:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[(int)crankIndex];
			if (ptr.holderActorNr == -1)
			{
				ptr.holderActorNr = info.Sender.ActorNumber;
				ptr.isLeftHand = (msgType == 0);
				return;
			}
			break;
		}
		case 2:
		{
			ref BatteryChargerState.CrankSyncState ptr2 = ref this.crankSyncs[(int)crankIndex];
			if (ptr2.holderActorNr == info.Sender.ActorNumber)
			{
				ptr2.holderActorNr = -1;
				ptr2.angle = floatParam.ClampSafe(-360f, 360f);
				return;
			}
			break;
		}
		case 3:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if (this.crankSyncs[(int)crankIndex].holderActorNr != info.Sender.ActorNumber)
			{
				return;
			}
			float num = this.currentCharge;
			this.currentCharge = floatParam.ClampSafe(0f, this.maxCharge);
			Action action = this.onChargeChanged;
			if (action != null)
			{
				action();
			}
			if (num < this.maxCharge && this.currentCharge >= this.maxCharge)
			{
				Action action2 = this.onFullyCharged;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0003B265 File Offset: 0x00039465
	public void SetEventPhase(int phase)
	{
		if ((PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) || phase == this.eventPhase)
		{
			return;
		}
		this.eventPhase = phase;
		Action<int> action = this.onEventPhaseChanged;
		if (action == null)
		{
			return;
		}
		action(phase);
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0003B298 File Offset: 0x00039498
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.m_disableNetworking)
		{
			return;
		}
		stream.SendNext(this.currentCharge);
		stream.SendNext(this.eventPhase);
		for (int i = 0; i < 20; i++)
		{
			stream.SendNext(this.crankSyncs[i].holderActorNr);
			stream.SendNext(this.crankSyncs[i].isLeftHand);
			stream.SendNext(this.crankSyncs[i].angle);
		}
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0003B334 File Offset: 0x00039534
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_disableNetworking)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		int localActorNr = this.LocalActorNr;
		int i = 0;
		while (i < 20)
		{
			int num3 = (int)stream.ReceiveNext();
			bool isLeftHand = (bool)stream.ReceiveNext();
			float angle = (float)stream.ReceiveNext();
			if (this.pendingGrabTime[i] <= 0f || this.crankSyncs[i].holderActorNr != localActorNr)
			{
				goto IL_D7;
			}
			if (num3 == localActorNr)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			if (num3 != -1)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			if (Time.time - this.pendingGrabTime[i] > 1f)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			IL_113:
			i++;
			continue;
			IL_D7:
			this.crankSyncs[i].holderActorNr = num3;
			this.crankSyncs[i].isLeftHand = isLeftHand;
			this.crankSyncs[i].angle = angle;
			goto IL_113;
		}
		bool flag = false;
		for (int j = 0; j < 20; j++)
		{
			if (this.crankSyncs[j].holderActorNr == localActorNr)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.currentCharge = num;
		}
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (num2 != this.eventPhase)
		{
			this.eventPhase = num2;
			Action<int> action2 = this.onEventPhaseChanged;
			if (action2 == null)
			{
				return;
			}
			action2(this.eventPhase);
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0003B4D0 File Offset: 0x000396D0
	// (set) Token: 0x06000B17 RID: 2839 RVA: 0x0003B4FA File Offset: 0x000396FA
	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe BatteryChargerState.FusionSyncState FusionData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BatteryChargerState.FusionSyncState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BatteryChargerState.FusionSyncState*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0003B528 File Offset: 0x00039728
	[Networked]
	[Capacity(20)]
	[NetworkedWeaved(2, 60)]
	[NetworkedWeavedArray(20, 3, typeof(ReaderWriter@BatteryChargerState__FusionCrankData))]
	private unsafe NetworkArray<BatteryChargerState.FusionCrankData> FusionCranks
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionCranks. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<BatteryChargerState.FusionCrankData>((byte*)(this.Ptr + 2), 20, ReaderWriter@BatteryChargerState__FusionCrankData.GetInstance());
		}
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003B568 File Offset: 0x00039768
	public override void WriteDataFusion()
	{
		this.FusionData = new BatteryChargerState.FusionSyncState
		{
			charge = this.currentCharge,
			eventPhase = this.eventPhase
		};
		for (int i = 0; i < 20; i++)
		{
			this.FusionCranks.Set(i, new BatteryChargerState.FusionCrankData
			{
				holderActorNr = this.crankSyncs[i].holderActorNr,
				isLeftHand = this.crankSyncs[i].isLeftHand,
				angle = this.crankSyncs[i].angle
			});
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0003B614 File Offset: 0x00039814
	public override void ReadDataFusion()
	{
		BatteryChargerState.FusionSyncState fusionData = this.FusionData;
		int localActorNr = this.LocalActorNr;
		int i = 0;
		while (i < 20)
		{
			BatteryChargerState.FusionCrankData fusionCrankData = this.FusionCranks[i];
			int holderActorNr = fusionCrankData.holderActorNr;
			if (this.pendingGrabTime[i] <= 0f || this.crankSyncs[i].holderActorNr != localActorNr)
			{
				goto IL_9D;
			}
			if (holderActorNr == localActorNr)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			if (holderActorNr != -1)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			if (Time.time - this.pendingGrabTime[i] > 1f)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			IL_E5:
			i++;
			continue;
			IL_9D:
			this.crankSyncs[i].holderActorNr = holderActorNr;
			this.crankSyncs[i].isLeftHand = fusionCrankData.isLeftHand;
			this.crankSyncs[i].angle = fusionCrankData.angle;
			goto IL_E5;
		}
		bool flag = false;
		for (int j = 0; j < 20; j++)
		{
			if (this.crankSyncs[j].holderActorNr == localActorNr)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.currentCharge = fusionData.charge;
		}
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (fusionData.eventPhase != this.eventPhase)
		{
			this.eventPhase = fusionData.eventPhase;
			Action<int> action2 = this.onEventPhaseChanged;
			if (action2 == null)
			{
				return;
			}
			action2(this.eventPhase);
		}
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0003B7EC File Offset: 0x000399EC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.FusionData = this._FusionData;
		NetworkBehaviourUtils.InitializeNetworkArray<BatteryChargerState.FusionCrankData>(this.FusionCranks, this._FusionCranks, "FusionCranks");
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0003B81A File Offset: 0x00039A1A
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._FusionData = this.FusionData;
		NetworkBehaviourUtils.CopyFromNetworkArray<BatteryChargerState.FusionCrankData>(this.FusionCranks, ref this._FusionCranks);
	}

	// Token: 0x04000D3D RID: 3389
	internal const int MAX_CRANKS = 20;

	// Token: 0x04000D3E RID: 3390
	[Header("Charging")]
	[Tooltip("Charge added per degree of crank rotation (across all cranks)")]
	[SerializeField]
	private float chargePerCrankDegree = 0.001f;

	// Token: 0x04000D3F RID: 3391
	[Tooltip("Charge drains at this rate per second when no one is cranking")]
	[SerializeField]
	private float drainPerSecond = 0.02f;

	// Token: 0x04000D40 RID: 3392
	[Tooltip("Maximum charge level (0 to 1)")]
	[SerializeField]
	private float maxCharge = 1f;

	// Token: 0x04000D41 RID: 3393
	private float currentCharge;

	// Token: 0x04000D42 RID: 3394
	private int activeCrankerCount;

	// Token: 0x04000D43 RID: 3395
	private int eventPhase = -1;

	// Token: 0x04000D44 RID: 3396
	internal BatteryChargerState.CrankSyncState[] crankSyncs = new BatteryChargerState.CrankSyncState[20];

	// Token: 0x04000D45 RID: 3397
	private const float GRAB_GRACE_PERIOD = 1f;

	// Token: 0x04000D46 RID: 3398
	private float[] pendingGrabTime = new float[20];

	// Token: 0x04000D4A RID: 3402
	private const float CRANK_RPC_INTERVAL = 1f;

	// Token: 0x04000D4B RID: 3403
	private float nextCrankRPCTimestamp;

	// Token: 0x04000D4C RID: 3404
	private float pendingCrankCharge;

	// Token: 0x04000D4D RID: 3405
	private int pendingCrankIndex = -1;

	// Token: 0x04000D4E RID: 3406
	private bool m_disableNetworking;

	// Token: 0x04000D4F RID: 3407
	[WeaverGenerated]
	[DefaultForProperty("FusionData", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BatteryChargerState.FusionSyncState _FusionData;

	// Token: 0x04000D50 RID: 3408
	[WeaverGenerated]
	[DefaultForProperty("FusionCranks", 2, 60)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BatteryChargerState.FusionCrankData[] _FusionCranks;

	// Token: 0x02000198 RID: 408
	internal struct CrankSyncState
	{
		// Token: 0x04000D51 RID: 3409
		public int holderActorNr;

		// Token: 0x04000D52 RID: 3410
		public bool isLeftHand;

		// Token: 0x04000D53 RID: 3411
		public float angle;
	}

	// Token: 0x02000199 RID: 409
	private enum BatteryMsg : byte
	{
		// Token: 0x04000D55 RID: 3413
		CrankGrabLeft,
		// Token: 0x04000D56 RID: 3414
		CrankGrabRight,
		// Token: 0x04000D57 RID: 3415
		CrankRelease,
		// Token: 0x04000D58 RID: 3416
		CrankInput
	}

	// Token: 0x0200019A RID: 410
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	private struct FusionCrankData : INetworkStruct
	{
		// Token: 0x04000D59 RID: 3417
		[FieldOffset(0)]
		public int holderActorNr;

		// Token: 0x04000D5A RID: 3418
		[FieldOffset(4)]
		public NetworkBool isLeftHand;

		// Token: 0x04000D5B RID: 3419
		[FieldOffset(8)]
		public float angle;
	}

	// Token: 0x0200019B RID: 411
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct FusionSyncState : INetworkStruct
	{
		// Token: 0x04000D5C RID: 3420
		[FieldOffset(0)]
		public float charge;

		// Token: 0x04000D5D RID: 3421
		[FieldOffset(4)]
		public int eventPhase;
	}
}
