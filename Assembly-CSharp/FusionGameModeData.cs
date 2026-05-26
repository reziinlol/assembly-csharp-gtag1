using System;
using Fusion;

// Token: 0x020005BB RID: 1467
[NetworkBehaviourWeaved(0)]
public abstract class FusionGameModeData : NetworkBehaviour
{
	// Token: 0x170003DE RID: 990
	// (get) Token: 0x060024E9 RID: 9449
	// (set) Token: 0x060024EA RID: 9450
	public abstract object Data { get; set; }

	// Token: 0x060024EC RID: 9452 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000028C5 File Offset: 0x00000AC5
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x0400304D RID: 12365
	protected INetworkStruct data;
}
