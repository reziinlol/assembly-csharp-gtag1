using System;

// Token: 0x02000D6F RID: 3439
public struct EnterPlayID
{
	// Token: 0x06005478 RID: 21624 RVA: 0x001B8E57 File Offset: 0x001B7057
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x06005479 RID: 21625 RVA: 0x001B8E68 File Offset: 0x001B7068
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x170007F6 RID: 2038
	// (get) Token: 0x0600547A RID: 21626 RVA: 0x001B8E8A File Offset: 0x001B708A
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04006525 RID: 25893
	private static int currentID = 1;

	// Token: 0x04006526 RID: 25894
	private int id;
}
