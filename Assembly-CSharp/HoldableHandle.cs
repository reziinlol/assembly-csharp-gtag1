using System;
using UnityEngine;

// Token: 0x02000524 RID: 1316
public class HoldableHandle : InteractionPoint
{
	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06002103 RID: 8451 RVA: 0x000B08FB File Offset: 0x000AEAFB
	public new HoldableObject Holdable
	{
		get
		{
			return this.holdable;
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06002104 RID: 8452 RVA: 0x000B0903 File Offset: 0x000AEB03
	public CapsuleCollider Capsule
	{
		get
		{
			return this.handleCapsuleTrigger;
		}
	}

	// Token: 0x04002BCE RID: 11214
	[SerializeField]
	private HoldableObject holdable;

	// Token: 0x04002BCF RID: 11215
	[SerializeField]
	private CapsuleCollider handleCapsuleTrigger;
}
