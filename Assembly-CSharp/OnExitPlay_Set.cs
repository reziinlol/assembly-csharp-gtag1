using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000D74 RID: 3444
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_Set : OnExitPlay_Attribute
{
	// Token: 0x06005483 RID: 21635 RVA: 0x001B8EF4 File Offset: 0x001B70F4
	public OnExitPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x06005484 RID: 21636 RVA: 0x001B8F03 File Offset: 0x001B7103
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x0400652A RID: 25898
	private object value;
}
