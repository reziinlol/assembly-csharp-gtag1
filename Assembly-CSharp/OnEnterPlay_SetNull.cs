using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D6A RID: 3434
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_SetNull : OnEnterPlay_Attribute
{
	// Token: 0x0600546E RID: 21614 RVA: 0x001B8CBA File Offset: 0x001B6EBA
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
