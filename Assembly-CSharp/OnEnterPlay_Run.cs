using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D6E RID: 3438
[AttributeUsage(AttributeTargets.Method)]
public class OnEnterPlay_Run : OnEnterPlay_Attribute
{
	// Token: 0x06005476 RID: 21622 RVA: 0x001B8E23 File Offset: 0x001B7023
	public override void OnEnterPlay(MethodInfo method)
	{
		if (!method.IsStatic)
		{
			Debug.LogError(string.Format("Can't Run non-static method {0}.{1}", method.DeclaringType, method.Name));
			return;
		}
		method.Invoke(null, new object[0]);
	}
}
