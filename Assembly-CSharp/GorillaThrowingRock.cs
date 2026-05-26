using System;
using Photon.Pun;

// Token: 0x02000A26 RID: 2598
public class GorillaThrowingRock : GorillaThrowable, IPunInstantiateMagicCallback
{
	// Token: 0x0600427B RID: 17019 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x0400546D RID: 21613
	public float bonkSpeedMin = 1f;

	// Token: 0x0400546E RID: 21614
	public float bonkSpeedMax = 5f;

	// Token: 0x0400546F RID: 21615
	public VRRig hitRig;
}
