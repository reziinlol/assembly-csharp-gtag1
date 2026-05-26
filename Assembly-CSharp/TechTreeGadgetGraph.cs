using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

// Token: 0x02000128 RID: 296
[CreateAssetMenu(fileName = "TechTreeGadgetGraph", menuName = "SuperInfection/TechTree Gadget Graph")]
public class TechTreeGadgetGraph : NodeGraph
{
	// Token: 0x1700007F RID: 127
	// (get) Token: 0x06000757 RID: 1879 RVA: 0x00029609 File Offset: 0x00027809
	public GadgetNode[] GadgetNodes
	{
		get
		{
			return (from n in this.nodes
			select n as GadgetNode).ToArray<GadgetNode>();
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000758 RID: 1880 RVA: 0x0002963C File Offset: 0x0002783C
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			if (eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				List<Node> nodes = this.nodes;
				return nodes != null && nodes.Count > 0;
			}
			return false;
		}
	}

	// Token: 0x0400098B RID: 2443
	public string nickName;

	// Token: 0x0400098C RID: 2444
	public SITechTreePageId pageId;

	// Token: 0x0400098D RID: 2445
	public Sprite icon;

	// Token: 0x0400098E RID: 2446
	public float costMultiplier = 1f;

	// Token: 0x0400098F RID: 2447
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000990 RID: 2448
	public EAssetReleaseTier releaseTier;

	// Token: 0x04000991 RID: 2449
	private const float XLayoutStep = 300f;

	// Token: 0x04000992 RID: 2450
	private const float YLayoutStep = 250f;
}
