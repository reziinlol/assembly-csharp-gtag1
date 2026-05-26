using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D77 RID: 3447
[AttributeUsage(AttributeTargets.Method)]
public class OnExitPlay_Run : OnExitPlay_Attribute
{
	// Token: 0x06005489 RID: 21641 RVA: 0x001B8E23 File Offset: 0x001B7023
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
