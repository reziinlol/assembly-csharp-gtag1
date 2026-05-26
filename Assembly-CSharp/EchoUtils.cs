using System;
using UnityEngine;

// Token: 0x02000D49 RID: 3401
public static class EchoUtils
{
	// Token: 0x060053B9 RID: 21433 RVA: 0x001B649F File Offset: 0x001B469F
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x060053BA RID: 21434 RVA: 0x001B64AD File Offset: 0x001B46AD
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
