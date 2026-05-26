using System;
using UnityEngine;

// Token: 0x020005B1 RID: 1457
public static class DeepLinkSender
{
	// Token: 0x060024C0 RID: 9408 RVA: 0x000C4E60 File Offset: 0x000C3060
	public static bool SendDeepLink(ulong deepLinkAppID, string deepLinkMessage, Action<string> onSent)
	{
		Debug.LogError("[DeepLinkSender::SendDeepLink] Called on non-oculus platform!");
		return false;
	}

	// Token: 0x04003034 RID: 12340
	private static Action<string> currentDeepLinkSentCallback;
}
