using System;
using TMPro;
using UnityEngine;

// Token: 0x020007B1 RID: 1969
public class GRNameDisplayPlate : MonoBehaviour
{
	// Token: 0x06003228 RID: 12840 RVA: 0x00113804 File Offset: 0x00111A04
	public void RefreshPlayerName(VRRig vrRig)
	{
		GRPlayer x = GRPlayer.Get(vrRig);
		if (vrRig != null && x != null)
		{
			if (!this.namePlateLabel.text.Equals(vrRig.playerNameVisible))
			{
				this.namePlateLabel.text = vrRig.playerNameVisible;
				return;
			}
		}
		else
		{
			this.namePlateLabel.text = "";
		}
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x00113864 File Offset: 0x00111A64
	public void Clear()
	{
		this.namePlateLabel.text = "";
	}

	// Token: 0x04004128 RID: 16680
	public TMP_Text namePlateLabel;
}
