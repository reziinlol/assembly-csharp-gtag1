using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020003BB RID: 955
public class PhotonViewXSceneRef : MonoBehaviour
{
	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060016F7 RID: 5879 RVA: 0x00085348 File Offset: 0x00083548
	public PhotonView photonView
	{
		get
		{
			PhotonView result;
			if (this.reference.TryResolve<PhotonView>(out result))
			{
				return result;
			}
			return null;
		}
	}

	// Token: 0x04002224 RID: 8740
	[SerializeField]
	private XSceneRef reference;
}
