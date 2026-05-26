using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C96 RID: 3222
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x06004FD2 RID: 20434 RVA: 0x001A6675 File Offset: 0x001A4875
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x06004FD3 RID: 20435 RVA: 0x001A6689 File Offset: 0x001A4889
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x17000778 RID: 1912
	// (get) Token: 0x06004FD4 RID: 20436 RVA: 0x001A6692 File Offset: 0x001A4892
	// (set) Token: 0x06004FD5 RID: 20437 RVA: 0x001A66BC File Offset: 0x001A48BC
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06004FD6 RID: 20438 RVA: 0x001A66E7 File Offset: 0x001A48E7
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x06004FD7 RID: 20439 RVA: 0x001A6701 File Offset: 0x001A4901
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06004FD8 RID: 20440 RVA: 0x001A671B File Offset: 0x001A491B
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x06004FD9 RID: 20441 RVA: 0x001A6756 File Offset: 0x001A4956
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06004FDB RID: 20443 RVA: 0x001A6791 File Offset: 0x001A4991
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06004FDC RID: 20444 RVA: 0x001A67A9 File Offset: 0x001A49A9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x0400619C RID: 24988
	private bool willTeleport;

	// Token: 0x0400619D RID: 24989
	private GorillaNetworkTransform transformView;

	// Token: 0x0400619E RID: 24990
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkBool _Data;
}
