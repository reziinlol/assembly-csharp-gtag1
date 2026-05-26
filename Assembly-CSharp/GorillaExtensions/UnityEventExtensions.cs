using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GorillaExtensions
{
	// Token: 0x0200111D RID: 4381
	public static class UnityEventExtensions
	{
		// Token: 0x06006E41 RID: 28225 RVA: 0x00241064 File Offset: 0x0023F264
		public static void InvokeAll(this IEnumerable<UnityEvent> events)
		{
			foreach (UnityEvent unityEvent in events)
			{
				unityEvent.Invoke();
			}
		}

		// Token: 0x06006E42 RID: 28226 RVA: 0x002410AC File Offset: 0x0023F2AC
		public static void InvokeAll<TArg>(this IEnumerable<UnityEvent<TArg>> events, TArg arg)
		{
			foreach (UnityEvent<TArg> unityEvent in events)
			{
				unityEvent.Invoke(arg);
			}
		}
	}
}
