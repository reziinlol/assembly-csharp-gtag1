using System;
using Fusion;

// Token: 0x020005BA RID: 1466
[NetworkBehaviourWeaved(1)]
public class CasualGameModeData : FusionGameModeData
{
	// Token: 0x170003DC RID: 988
	// (get) Token: 0x060024E2 RID: 9442 RVA: 0x000C545F File Offset: 0x000C365F
	// (set) Token: 0x060024E3 RID: 9443 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override object Data
	{
		get
		{
			return this.casualData;
		}
		set
		{
		}
	}

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x060024E4 RID: 9444 RVA: 0x000C546C File Offset: 0x000C366C
	// (set) Token: 0x060024E5 RID: 9445 RVA: 0x000C5496 File Offset: 0x000C3696
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe CasualData casualData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(CasualData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(CasualData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000C54C1 File Offset: 0x000C36C1
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.casualData = this._casualData;
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x000C54D9 File Offset: 0x000C36D9
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._casualData = this.casualData;
	}

	// Token: 0x0400304C RID: 12364
	[WeaverGenerated]
	[DefaultForProperty("casualData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private CasualData _casualData;
}
