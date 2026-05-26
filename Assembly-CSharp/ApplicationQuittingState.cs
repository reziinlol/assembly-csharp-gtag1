using System;
using UnityEngine;

// Token: 0x0200029E RID: 670
public static class ApplicationQuittingState
{
	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x0600119D RID: 4509 RVA: 0x0005E71A File Offset: 0x0005C91A
	// (set) Token: 0x0600119E RID: 4510 RVA: 0x0005E721 File Offset: 0x0005C921
	public static bool IsQuitting { get; private set; }

	// Token: 0x0600119F RID: 4511 RVA: 0x0005E729 File Offset: 0x0005C929
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += ApplicationQuittingState.HandleApplicationQuitting;
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0005E73C File Offset: 0x0005C93C
	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}
}
