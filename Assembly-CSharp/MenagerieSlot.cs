using System;
using TMPro;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class MenagerieSlot : MonoBehaviour
{
	// Token: 0x06000384 RID: 900 RVA: 0x000148AA File Offset: 0x00012AAA
	private void Reset()
	{
		this.critterMountPoint = base.transform;
	}

	// Token: 0x04000402 RID: 1026
	public Transform critterMountPoint;

	// Token: 0x04000403 RID: 1027
	public TMP_Text label;

	// Token: 0x04000404 RID: 1028
	public MenagerieCritter critter;
}
