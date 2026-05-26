using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D6B RID: 3435
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x06005470 RID: 21616 RVA: 0x001B8CF0 File Offset: 0x001B6EF0
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06005471 RID: 21617 RVA: 0x001B8D00 File Offset: 0x001B6F00
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		if (field.FieldType == typeof(ushort))
		{
			field.SetValue(null, Convert.ToUInt16(this.value));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04006524 RID: 25892
	private object value;
}
