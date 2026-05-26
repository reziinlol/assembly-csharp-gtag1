using System;
using System.Collections.Generic;

// Token: 0x020003C4 RID: 964
[Serializable]
public class SerializablePerformanceReport<T>
{
	// Token: 0x04002255 RID: 8789
	public string reportDate;

	// Token: 0x04002256 RID: 8790
	public string version;

	// Token: 0x04002257 RID: 8791
	public List<T> results;
}
