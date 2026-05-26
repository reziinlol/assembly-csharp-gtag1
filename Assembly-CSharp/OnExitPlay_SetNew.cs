using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D75 RID: 3445
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_SetNew : OnExitPlay_Attribute
{
	// Token: 0x06005485 RID: 21637 RVA: 0x001B8F38 File Offset: 0x001B7138
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
