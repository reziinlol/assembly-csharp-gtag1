using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000455 RID: 1109
[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	// Token: 0x06001A9F RID: 6815 RVA: 0x00093D70 File Offset: 0x00091F70
	public override void ReadDataFusion()
	{
		this.ReadData();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x00093D7D File Offset: 0x00091F7D
	public override void WriteDataFusion()
	{
		this.WriteData();
	}

	// Token: 0x06001AA1 RID: 6817 RVA: 0x00093D8A File Offset: 0x00091F8A
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData(stream, info);
	}

	// Token: 0x06001AA2 RID: 6818 RVA: 0x00093D99 File Offset: 0x00091F99
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData(stream, info);
	}

	// Token: 0x06001AA4 RID: 6820 RVA: 0x00002B07 File Offset: 0x00000D07
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001AA5 RID: 6821 RVA: 0x00002B13 File Offset: 0x00000D13
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002539 RID: 9529
	public Action ReadData;

	// Token: 0x0400253A RID: 9530
	public Action WriteData;

	// Token: 0x0400253B RID: 9531
	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	// Token: 0x0400253C RID: 9532
	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
