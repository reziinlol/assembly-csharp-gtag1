using System;

namespace LitJson
{
	// Token: 0x02000E81 RID: 3713
	public enum JsonToken
	{
		// Token: 0x040069F6 RID: 27126
		None,
		// Token: 0x040069F7 RID: 27127
		ObjectStart,
		// Token: 0x040069F8 RID: 27128
		PropertyName,
		// Token: 0x040069F9 RID: 27129
		ObjectEnd,
		// Token: 0x040069FA RID: 27130
		ArrayStart,
		// Token: 0x040069FB RID: 27131
		ArrayEnd,
		// Token: 0x040069FC RID: 27132
		Int,
		// Token: 0x040069FD RID: 27133
		Long,
		// Token: 0x040069FE RID: 27134
		Double,
		// Token: 0x040069FF RID: 27135
		String,
		// Token: 0x04006A00 RID: 27136
		Boolean,
		// Token: 0x04006A01 RID: 27137
		Null
	}
}
