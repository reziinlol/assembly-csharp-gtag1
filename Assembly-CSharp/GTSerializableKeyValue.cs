using System;

// Token: 0x0200037D RID: 893
[Serializable]
public struct GTSerializableKeyValue<T1, T2>
{
	// Token: 0x060015C6 RID: 5574 RVA: 0x000734A5 File Offset: 0x000716A5
	public GTSerializableKeyValue(T1 k, T2 v)
	{
		this.k = k;
		this.v = v;
	}

	// Token: 0x04001A95 RID: 6805
	public T1 k;

	// Token: 0x04001A96 RID: 6806
	public T2 v;
}
