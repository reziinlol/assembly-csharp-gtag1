using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D76 RID: 3446
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x06005487 RID: 21639 RVA: 0x001B8F90 File Offset: 0x001B7190
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
