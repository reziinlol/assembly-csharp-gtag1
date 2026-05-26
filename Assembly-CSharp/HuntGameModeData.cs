using System;
using Fusion;

// Token: 0x020005BD RID: 1469
[NetworkBehaviourWeaved(43)]
public class HuntGameModeData : FusionGameModeData
{
	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x060024F0 RID: 9456 RVA: 0x000C553F File Offset: 0x000C373F
	// (set) Token: 0x060024F1 RID: 9457 RVA: 0x000C554C File Offset: 0x000C374C
	public override object Data
	{
		get
		{
			return this.huntdata;
		}
		set
		{
			this.huntdata = (HuntData)value;
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x060024F2 RID: 9458 RVA: 0x000C555A File Offset: 0x000C375A
	// (set) Token: 0x060024F3 RID: 9459 RVA: 0x000C5584 File Offset: 0x000C3784
	[Networked]
	[NetworkedWeaved(0, 43)]
	private unsafe HuntData huntdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HuntData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HuntData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x000C55AF File Offset: 0x000C37AF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.huntdata = this._huntdata;
	}

	// Token: 0x060024F6 RID: 9462 RVA: 0x000C55C7 File Offset: 0x000C37C7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._huntdata = this.huntdata;
	}

	// Token: 0x04003053 RID: 12371
	[WeaverGenerated]
	[DefaultForProperty("huntdata", 0, 43)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HuntData _huntdata;
}
