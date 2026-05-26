using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
[RequireComponent(typeof(PlayerColoredCosmetic))]
public class FXModifierPlayerColorSetter : FXModifier
{
	// Token: 0x06001437 RID: 5175 RVA: 0x0006CACB File Offset: 0x0006ACCB
	public override void UpdateScale(float scale, Color color)
	{
		this.playerColoredCosmetic.UpdateColor(color);
	}

	// Token: 0x040018F9 RID: 6393
	[SerializeField]
	private PlayerColoredCosmetic playerColoredCosmetic;
}
