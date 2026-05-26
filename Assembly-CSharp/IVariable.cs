using System;

// Token: 0x020008F2 RID: 2290
public interface IVariable<T> : IVariable
{
	// Token: 0x1700055D RID: 1373
	// (get) Token: 0x06003BE6 RID: 15334 RVA: 0x00147A51 File Offset: 0x00145C51
	// (set) Token: 0x06003BE7 RID: 15335 RVA: 0x00147A59 File Offset: 0x00145C59
	T Value
	{
		get
		{
			return this.Get();
		}
		set
		{
			this.Set(value);
		}
	}

	// Token: 0x06003BE8 RID: 15336
	T Get();

	// Token: 0x06003BE9 RID: 15337
	void Set(T value);

	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x06003BEA RID: 15338 RVA: 0x00147A62 File Offset: 0x00145C62
	Type IVariable.ValueType
	{
		get
		{
			return typeof(T);
		}
	}
}
