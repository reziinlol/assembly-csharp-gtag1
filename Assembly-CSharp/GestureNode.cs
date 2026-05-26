using System;
using UnityEngine;

// Token: 0x020002C7 RID: 711
[Serializable]
public class GestureNode
{
	// Token: 0x04001629 RID: 5673
	public bool track;

	// Token: 0x0400162A RID: 5674
	public GestureHandState state;

	// Token: 0x0400162B RID: 5675
	public GestureDigitFlexion flexion;

	// Token: 0x0400162C RID: 5676
	public GestureAlignment alignment;

	// Token: 0x0400162D RID: 5677
	[Space]
	public GestureNodeFlags flags;
}
