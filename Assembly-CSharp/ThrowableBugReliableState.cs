using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000DE0 RID: 3552
[NetworkBehaviourWeaved(3)]
public class ThrowableBugReliableState : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700082C RID: 2092
	// (get) Token: 0x060056F1 RID: 22257 RVA: 0x001C2B8D File Offset: 0x001C0D8D
	// (set) Token: 0x060056F2 RID: 22258 RVA: 0x001C2BB7 File Offset: 0x001C0DB7
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe ThrowableBugReliableState.BugData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ThrowableBugReliableState.BugData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ThrowableBugReliableState.BugData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060056F3 RID: 22259 RVA: 0x001C2BE2 File Offset: 0x001C0DE2
	public override void WriteDataFusion()
	{
		this.Data = new ThrowableBugReliableState.BugData(this.travelingDirection);
	}

	// Token: 0x060056F4 RID: 22260 RVA: 0x001C2BF8 File Offset: 0x001C0DF8
	public override void ReadDataFusion()
	{
		this.travelingDirection = this.Data.tDirection;
	}

	// Token: 0x060056F5 RID: 22261 RVA: 0x001C2C19 File Offset: 0x001C0E19
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.travelingDirection);
	}

	// Token: 0x060056F6 RID: 22262 RVA: 0x001C2C2C File Offset: 0x001C0E2C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.travelingDirection.SetValueSafe(vector);
	}

	// Token: 0x060056F7 RID: 22263 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060056F8 RID: 22264 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060056F9 RID: 22265 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060056FA RID: 22266 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x060056FB RID: 22267 RVA: 0x00002AF8 File Offset: 0x00000CF8
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x060056FD RID: 22269 RVA: 0x001C2C65 File Offset: 0x001C0E65
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x060056FE RID: 22270 RVA: 0x001C2C7D File Offset: 0x001C0E7D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040066F7 RID: 26359
	public Vector3 travelingDirection = Vector3.zero;

	// Token: 0x040066F8 RID: 26360
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ThrowableBugReliableState.BugData _Data;

	// Token: 0x02000DE1 RID: 3553
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	public struct BugData : INetworkStruct
	{
		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x060056FF RID: 22271 RVA: 0x001C2C91 File Offset: 0x001C0E91
		// (set) Token: 0x06005700 RID: 22272 RVA: 0x001C2CA3 File Offset: 0x001C0EA3
		[Networked]
		[NetworkedWeaved(0, 3)]
		public unsafe Vector3 tDirection
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection) = value;
			}
		}

		// Token: 0x06005701 RID: 22273 RVA: 0x001C2CB6 File Offset: 0x001C0EB6
		public BugData(Vector3 dir)
		{
			this.tDirection = dir;
		}

		// Token: 0x040066F9 RID: 26361
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@3 _tDirection;
	}
}
