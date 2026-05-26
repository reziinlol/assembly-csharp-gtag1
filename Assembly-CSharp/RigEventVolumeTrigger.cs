using System;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class RigEventVolumeTrigger : MonoBehaviour
{
	// Token: 0x1700032E RID: 814
	// (get) Token: 0x06001E54 RID: 7764 RVA: 0x000A25E4 File Offset: 0x000A07E4
	public VRRig Rig
	{
		get
		{
			return this._rig;
		}
	}

	// Token: 0x0400287F RID: 10367
	[SerializeField]
	private VRRig _rig;
}
