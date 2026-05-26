using System;
using System.Reflection;

// Token: 0x02000D69 RID: 3433
public class OnPlayChange_BaseAttribute : Attribute
{
	// Token: 0x0600546B RID: 21611 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnEnterPlay(FieldInfo field)
	{
	}

	// Token: 0x0600546C RID: 21612 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnEnterPlay(MethodInfo method)
	{
	}
}
