using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200083B RID: 2107
public class PhotonViewCache : MonoBehaviour, IPunInstantiateMagicCallback
{
	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06003633 RID: 13875 RVA: 0x0012B5CC File Offset: 0x001297CC
	// (set) Token: 0x06003634 RID: 13876 RVA: 0x0012B5D4 File Offset: 0x001297D4
	public bool Initialized { get; private set; }

	// Token: 0x06003635 RID: 13877 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x040046B1 RID: 18097
	private PhotonView[] m_photonViews;

	// Token: 0x040046B2 RID: 18098
	[SerializeField]
	private bool m_isRoomObject;
}
