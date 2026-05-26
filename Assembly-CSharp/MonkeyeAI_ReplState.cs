using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001A6 RID: 422
[NetworkBehaviourWeaved(42)]
public class MonkeyeAI_ReplState : NetworkComponent
{
	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000B71 RID: 2929 RVA: 0x0003DCDE File Offset: 0x0003BEDE
	// (set) Token: 0x06000B72 RID: 2930 RVA: 0x0003DD08 File Offset: 0x0003BF08
	[Networked]
	[NetworkedWeaved(0, 42)]
	private unsafe MonkeyeAI_ReplState.MonkeyeAI_RepStateData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing MonkeyeAI_ReplState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(MonkeyeAI_ReplState.MonkeyeAI_RepStateData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x0003DD34 File Offset: 0x0003BF34
	public override void WriteDataFusion()
	{
		MonkeyeAI_ReplState.MonkeyeAI_RepStateData data = new MonkeyeAI_ReplState.MonkeyeAI_RepStateData(this.userId, this.attackPos, this.timer, this.floorEnabled, this.portalEnabled, this.freezePlayer, this.alpha, this.state);
		this.Data = data;
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x0003DD80 File Offset: 0x0003BF80
	public override void ReadDataFusion()
	{
		this.userId = this.Data.UserId.Value;
		this.attackPos = this.Data.AttackPos;
		this.timer = this.Data.Timer;
		this.floorEnabled = this.Data.FloorEnabled;
		this.portalEnabled = this.Data.PortalEnabled;
		this.freezePlayer = this.Data.FreezePlayer;
		this.alpha = this.Data.Alpha;
		this.state = this.Data.State;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x0003DE44 File Offset: 0x0003C044
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.userId);
		stream.SendNext(this.attackPos);
		stream.SendNext(this.timer);
		stream.SendNext(this.floorEnabled);
		stream.SendNext(this.portalEnabled);
		stream.SendNext(this.freezePlayer);
		stream.SendNext(this.alpha);
		stream.SendNext(this.state);
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x0003DED4 File Offset: 0x0003C0D4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.photonView.Owner == null)
		{
			return;
		}
		if (info.Sender.ActorNumber != info.photonView.Owner.ActorNumber)
		{
			return;
		}
		this.userId = (string)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.attackPos.SetValueSafe(vector);
		this.timer = (float)stream.ReceiveNext();
		this.floorEnabled = (bool)stream.ReceiveNext();
		this.portalEnabled = (bool)stream.ReceiveNext();
		this.freezePlayer = (bool)stream.ReceiveNext();
		this.alpha = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
		this.state = (MonkeyeAI_ReplState.EStates)stream.ReceiveNext();
	}

	// Token: 0x06000B78 RID: 2936 RVA: 0x0003DFAC File Offset: 0x0003C1AC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000B79 RID: 2937 RVA: 0x0003DFC4 File Offset: 0x0003C1C4
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04000DD7 RID: 3543
	public MonkeyeAI_ReplState.EStates state;

	// Token: 0x04000DD8 RID: 3544
	public string userId;

	// Token: 0x04000DD9 RID: 3545
	public Vector3 attackPos;

	// Token: 0x04000DDA RID: 3546
	public float timer;

	// Token: 0x04000DDB RID: 3547
	public bool floorEnabled;

	// Token: 0x04000DDC RID: 3548
	public bool portalEnabled;

	// Token: 0x04000DDD RID: 3549
	public bool freezePlayer;

	// Token: 0x04000DDE RID: 3550
	public float alpha;

	// Token: 0x04000DDF RID: 3551
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 42)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private MonkeyeAI_ReplState.MonkeyeAI_RepStateData _Data;

	// Token: 0x020001A7 RID: 423
	public enum EStates
	{
		// Token: 0x04000DE1 RID: 3553
		Sleeping,
		// Token: 0x04000DE2 RID: 3554
		Patrolling,
		// Token: 0x04000DE3 RID: 3555
		Chasing,
		// Token: 0x04000DE4 RID: 3556
		ReturnToSleepPt,
		// Token: 0x04000DE5 RID: 3557
		GoToSleep,
		// Token: 0x04000DE6 RID: 3558
		BeginAttack,
		// Token: 0x04000DE7 RID: 3559
		OpenFloor,
		// Token: 0x04000DE8 RID: 3560
		DropPlayer,
		// Token: 0x04000DE9 RID: 3561
		CloseFloor
	}

	// Token: 0x020001A8 RID: 424
	[NetworkStructWeaved(42)]
	[StructLayout(LayoutKind.Explicit, Size = 168)]
	public struct MonkeyeAI_RepStateData : INetworkStruct
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x0003DFD8 File Offset: 0x0003C1D8
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x0003DFEA File Offset: 0x0003C1EA
		[Networked]
		[NetworkedWeaved(0, 33)]
		public unsafe NetworkString<_32> UserId
		{
			readonly get
			{
				return *(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId);
			}
			set
			{
				*(NetworkString<_32>*)Native.ReferenceToPointer<FixedStorage@33>(ref this._UserId) = value;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x0003DFFD File Offset: 0x0003C1FD
		// (set) Token: 0x06000B7D RID: 2941 RVA: 0x0003E00F File Offset: 0x0003C20F
		[Networked]
		[NetworkedWeaved(33, 3)]
		public unsafe Vector3 AttackPos
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._AttackPos) = value;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000B7E RID: 2942 RVA: 0x0003E022 File Offset: 0x0003C222
		// (set) Token: 0x06000B7F RID: 2943 RVA: 0x0003E030 File Offset: 0x0003C230
		[Networked]
		[NetworkedWeaved(36, 1)]
		public unsafe float Timer
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Timer) = value;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x0003E03F File Offset: 0x0003C23F
		// (set) Token: 0x06000B81 RID: 2945 RVA: 0x0003E047 File Offset: 0x0003C247
		public NetworkBool FloorEnabled { readonly get; set; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000B82 RID: 2946 RVA: 0x0003E050 File Offset: 0x0003C250
		// (set) Token: 0x06000B83 RID: 2947 RVA: 0x0003E058 File Offset: 0x0003C258
		public NetworkBool PortalEnabled { readonly get; set; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x06000B84 RID: 2948 RVA: 0x0003E061 File Offset: 0x0003C261
		// (set) Token: 0x06000B85 RID: 2949 RVA: 0x0003E069 File Offset: 0x0003C269
		public NetworkBool FreezePlayer { readonly get; set; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000B86 RID: 2950 RVA: 0x0003E072 File Offset: 0x0003C272
		// (set) Token: 0x06000B87 RID: 2951 RVA: 0x0003E080 File Offset: 0x0003C280
		[Networked]
		[NetworkedWeaved(40, 1)]
		public unsafe float Alpha
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._Alpha) = value;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000B88 RID: 2952 RVA: 0x0003E08F File Offset: 0x0003C28F
		// (set) Token: 0x06000B89 RID: 2953 RVA: 0x0003E097 File Offset: 0x0003C297
		public MonkeyeAI_ReplState.EStates State { readonly get; set; }

		// Token: 0x06000B8A RID: 2954 RVA: 0x0003E0A0 File Offset: 0x0003C2A0
		public MonkeyeAI_RepStateData(string id, Vector3 atPos, float timer, bool floorOn, bool portalOn, bool freezePlayer, float alpha, MonkeyeAI_ReplState.EStates state)
		{
			this.UserId = id;
			this.AttackPos = atPos;
			this.Timer = timer;
			this.FloorEnabled = floorOn;
			this.PortalEnabled = portalOn;
			this.FreezePlayer = freezePlayer;
			this.Alpha = alpha;
			this.State = state;
		}

		// Token: 0x04000DEA RID: 3562
		[FixedBufferProperty(typeof(NetworkString<_32>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@33 _UserId;

		// Token: 0x04000DEB RID: 3563
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(132)]
		private FixedStorage@3 _AttackPos;

		// Token: 0x04000DEC RID: 3564
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(144)]
		private FixedStorage@1 _Timer;

		// Token: 0x04000DF0 RID: 3568
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(160)]
		private FixedStorage@1 _Alpha;
	}
}
