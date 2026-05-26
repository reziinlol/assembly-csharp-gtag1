using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000833 RID: 2099
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x060035F4 RID: 13812 RVA: 0x0012AC8B File Offset: 0x00128E8B
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x0012ACA5 File Offset: 0x00128EA5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x0012ACB1 File Offset: 0x00128EB1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
