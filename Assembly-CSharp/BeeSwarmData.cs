using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x0200021A RID: 538
[NetworkStructWeaved(3)]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct BeeSwarmData : INetworkStruct
{
	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000E26 RID: 3622 RVA: 0x0004D8AC File Offset: 0x0004BAAC
	// (set) Token: 0x06000E27 RID: 3623 RVA: 0x0004D8B4 File Offset: 0x0004BAB4
	public int TargetActorNumber { readonly get; set; }

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000E28 RID: 3624 RVA: 0x0004D8BD File Offset: 0x0004BABD
	// (set) Token: 0x06000E29 RID: 3625 RVA: 0x0004D8C5 File Offset: 0x0004BAC5
	public int CurrentState { readonly get; set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000E2A RID: 3626 RVA: 0x0004D8CE File Offset: 0x0004BACE
	// (set) Token: 0x06000E2B RID: 3627 RVA: 0x0004D8D6 File Offset: 0x0004BAD6
	public float CurrentSpeed { readonly get; set; }

	// Token: 0x06000E2C RID: 3628 RVA: 0x0004D8DF File Offset: 0x0004BADF
	public BeeSwarmData(int actorNr, int state, float speed)
	{
		this.TargetActorNumber = actorNr;
		this.CurrentState = state;
		this.CurrentSpeed = speed;
	}
}
