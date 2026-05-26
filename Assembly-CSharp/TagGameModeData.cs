using System;
using Fusion;

// Token: 0x020005BF RID: 1471
[NetworkBehaviourWeaved(22)]
public class TagGameModeData : FusionGameModeData
{
	// Token: 0x170003E5 RID: 997
	// (get) Token: 0x060024FA RID: 9466 RVA: 0x000C5614 File Offset: 0x000C3814
	// (set) Token: 0x060024FB RID: 9467 RVA: 0x000C5621 File Offset: 0x000C3821
	public override object Data
	{
		get
		{
			return this.tagData;
		}
		set
		{
			this.tagData = (TagData)value;
		}
	}

	// Token: 0x170003E6 RID: 998
	// (get) Token: 0x060024FC RID: 9468 RVA: 0x000C562F File Offset: 0x000C382F
	// (set) Token: 0x060024FD RID: 9469 RVA: 0x000C5659 File Offset: 0x000C3859
	[Networked]
	[NetworkedWeaved(0, 22)]
	private unsafe TagData tagData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(TagData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(TagData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000C5684 File Offset: 0x000C3884
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.tagData = this._tagData;
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000C569C File Offset: 0x000C389C
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._tagData = this.tagData;
	}

	// Token: 0x04003057 RID: 12375
	[WeaverGenerated]
	[DefaultForProperty("tagData", 0, 22)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private TagData _tagData;
}
