using System;
using UnityEngine;

// Token: 0x02000680 RID: 1664
public class EmitSignalToBiter : GTSignalEmitter
{
	// Token: 0x0600297F RID: 10623 RVA: 0x000DFEF0 File Offset: 0x000DE0F0
	public override void Emit()
	{
		if (this.onEdibleState == EmitSignalToBiter.EdibleState.None)
		{
			return;
		}
		if (!this.targetEdible)
		{
			return;
		}
		if (this.targetEdible.lastBiterActorID == -1)
		{
			return;
		}
		TransferrableObject.ItemStates itemState = this.targetEdible.itemState;
		if (itemState - TransferrableObject.ItemStates.State0 <= 1 || itemState == TransferrableObject.ItemStates.State2 || itemState == TransferrableObject.ItemStates.State3)
		{
			int num = (int)itemState;
			if ((this.onEdibleState & (EmitSignalToBiter.EdibleState)num) == (EmitSignalToBiter.EdibleState)num)
			{
				GTSignal.Emit(this.targetEdible.lastBiterActorID, this.signal, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Emit(int targetActor)
	{
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void Emit(params object[] data)
	{
	}

	// Token: 0x04003607 RID: 13831
	[Space]
	public EdibleHoldable targetEdible;

	// Token: 0x04003608 RID: 13832
	[Space]
	[SerializeField]
	private EmitSignalToBiter.EdibleState onEdibleState;

	// Token: 0x02000681 RID: 1665
	[Flags]
	private enum EdibleState
	{
		// Token: 0x0400360A RID: 13834
		None = 0,
		// Token: 0x0400360B RID: 13835
		State0 = 1,
		// Token: 0x0400360C RID: 13836
		State1 = 2,
		// Token: 0x0400360D RID: 13837
		State2 = 4,
		// Token: 0x0400360E RID: 13838
		State3 = 8
	}
}
