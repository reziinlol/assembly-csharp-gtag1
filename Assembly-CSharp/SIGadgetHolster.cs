using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200010B RID: 267
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
public class SIGadgetHolster : SIGadget, I_SIDisruptable
{
	// Token: 0x06000655 RID: 1621 RVA: 0x000235BB File Offset: 0x000217BB
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Disrupt(float disruptTime)
	{
	}

	// Token: 0x040007B8 RID: 1976
	[SerializeField]
	private Image imageMask;

	// Token: 0x040007B9 RID: 1977
	public List<SuperInfectionSnapPoint> snapPoints;

	// Token: 0x040007BA RID: 1978
	private SIGadgetHolster.State state;

	// Token: 0x040007BB RID: 1979
	private GTPlayer gtPlayer;

	// Token: 0x0200010C RID: 268
	private enum State
	{
		// Token: 0x040007BD RID: 1981
		Unequipped,
		// Token: 0x040007BE RID: 1982
		Equipped
	}
}
