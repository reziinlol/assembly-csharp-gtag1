using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000838 RID: 2104
internal interface IWrappedSerializable : INetworkStruct
{
	// Token: 0x06003623 RID: 13859
	void OnSerializeRead(object newData);

	// Token: 0x06003624 RID: 13860
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06003625 RID: 13861
	object OnSerializeWrite();

	// Token: 0x06003626 RID: 13862
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
