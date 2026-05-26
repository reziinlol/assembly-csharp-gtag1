using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D6C RID: 3436
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x06005472 RID: 21618 RVA: 0x001B8D70 File Offset: 0x001B6F70
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object value = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, value);
	}
}
