using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007C7 RID: 1991
public class GRReadyRoom : MonoBehaviour
{
	// Token: 0x060032C3 RID: 12995 RVA: 0x00116448 File Offset: 0x00114648
	public void RefreshRigs(List<VRRig> vrRigs)
	{
		for (int i = 0; i < this.nameDisplayPlates.Count; i++)
		{
			if (this.nameDisplayPlates != null)
			{
				if (i < vrRigs.Count && vrRigs[i] != null && vrRigs[i].OwningNetPlayer != null)
				{
					this.nameDisplayPlates[i].RefreshPlayerName(vrRigs[i]);
				}
				else
				{
					this.nameDisplayPlates[i].Clear();
				}
			}
		}
	}

	// Token: 0x040041F8 RID: 16888
	public List<GRNameDisplayPlate> nameDisplayPlates;
}
