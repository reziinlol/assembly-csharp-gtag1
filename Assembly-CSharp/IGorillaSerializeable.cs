using System;
using Photon.Pun;

// Token: 0x02000836 RID: 2102
public interface IGorillaSerializeable
{
	// Token: 0x0600361E RID: 13854
	void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x0600361F RID: 13855
	void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info);
}
