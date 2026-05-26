using System;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class SIScannableHand : MonoBehaviour
{
	// Token: 0x06000945 RID: 2373 RVA: 0x00031FFD File Offset: 0x000301FD
	private void Awake()
	{
		this.parentPlayer = base.GetComponentInParent<SIPlayer>();
	}

	// Token: 0x04000B5B RID: 2907
	public SIPlayer parentPlayer;
}
