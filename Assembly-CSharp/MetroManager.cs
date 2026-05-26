using System;
using UnityEngine;

// Token: 0x020001BC RID: 444
public class MetroManager : MonoBehaviour
{
	// Token: 0x06000BE3 RID: 3043 RVA: 0x00040DBC File Offset: 0x0003EFBC
	private void Update()
	{
		for (int i = 0; i < this._blimps.Length; i++)
		{
			this._blimps[i].Tick();
		}
		for (int j = 0; j < this._spotlights.Length; j++)
		{
			this._spotlights[j].Tick();
		}
	}

	// Token: 0x04000E79 RID: 3705
	[SerializeField]
	private MetroBlimp[] _blimps = new MetroBlimp[0];

	// Token: 0x04000E7A RID: 3706
	[SerializeField]
	private MetroSpotlight[] _spotlights = new MetroSpotlight[0];

	// Token: 0x04000E7B RID: 3707
	[Space]
	[SerializeField]
	private Transform _blimpsRotationAnchor;
}
