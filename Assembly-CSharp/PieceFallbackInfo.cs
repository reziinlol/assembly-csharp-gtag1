using System;
using UnityEngine;

// Token: 0x0200062C RID: 1580
[Serializable]
public struct PieceFallbackInfo
{
	// Token: 0x040032E2 RID: 13026
	[Tooltip("Check if the piece has Material Options set and the default material is in a starter set")]
	public bool materialSwapThisPrefab;

	// Token: 0x040032E3 RID: 13027
	[Tooltip("A piece in a starter set with the same builder attach grid configuration\n(check BuilderSetManager _starterPieceSets for pieces in starter sets)")]
	public BuilderPiece prefab;
}
