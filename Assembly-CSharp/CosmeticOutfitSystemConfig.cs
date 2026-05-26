using System;
using UnityEngine;

// Token: 0x0200056E RID: 1390
[CreateAssetMenu(fileName = "CosmeticOutfitSystemConfig", menuName = "Gorilla Tag/Cosmetics/OutfitSystem", order = 0)]
public class CosmeticOutfitSystemConfig : ScriptableObject
{
	// Token: 0x04002E6F RID: 11887
	public int nonSubscriberMaxOutfits = 5;

	// Token: 0x04002E70 RID: 11888
	public int subscriberMaxOutfits = 10;

	// Token: 0x04002E71 RID: 11889
	public string mothershipKey;

	// Token: 0x04002E72 RID: 11890
	public char outfitSeparator;

	// Token: 0x04002E73 RID: 11891
	public char itemSeparator;

	// Token: 0x04002E74 RID: 11892
	public string selectedOutfitPref;
}
