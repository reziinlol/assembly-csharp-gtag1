using System;
using UnityEngine;

// Token: 0x02000608 RID: 1544
[CreateAssetMenu(fileName = "BuilderPieceEffectInfo", menuName = "Gorilla Tag/Builder/EffectInfo", order = 0)]
public class BuilderPieceEffectInfo : ScriptableObject
{
	// Token: 0x040031FC RID: 12796
	public GameObject placeVFX;

	// Token: 0x040031FD RID: 12797
	public GameObject disconnectVFX;

	// Token: 0x040031FE RID: 12798
	public GameObject grabbedVFX;

	// Token: 0x040031FF RID: 12799
	public GameObject locationLockVFX;

	// Token: 0x04003200 RID: 12800
	public GameObject recycleVFX;

	// Token: 0x04003201 RID: 12801
	public GameObject tooHeavyVFX;
}
