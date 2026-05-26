using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class FlagCauldronColorer : MonoBehaviour
{
	// Token: 0x040018EF RID: 6383
	public FlagCauldronColorer.ColorMode mode;

	// Token: 0x040018F0 RID: 6384
	public Transform colorPoint;

	// Token: 0x02000333 RID: 819
	public enum ColorMode
	{
		// Token: 0x040018F2 RID: 6386
		None,
		// Token: 0x040018F3 RID: 6387
		Red,
		// Token: 0x040018F4 RID: 6388
		Green,
		// Token: 0x040018F5 RID: 6389
		Blue,
		// Token: 0x040018F6 RID: 6390
		Black,
		// Token: 0x040018F7 RID: 6391
		Clear
	}
}
