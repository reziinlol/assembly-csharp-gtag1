using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D6D RID: 3437
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	// Token: 0x06005474 RID: 21620 RVA: 0x001B8DC8 File Offset: 0x001B6FC8
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		MethodInfo method = field.FieldType.GetMethod("Clear");
		object value = field.GetValue(null);
		if (value != null)
		{
			method.Invoke(value, new object[0]);
		}
	}
}
