using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D73 RID: 3443
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_SetNull : OnExitPlay_Attribute
{
	// Token: 0x06005481 RID: 21633 RVA: 0x001B8CBA File Offset: 0x001B6EBA
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNull non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, null);
	}
}
