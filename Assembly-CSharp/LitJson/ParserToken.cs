using System;

namespace LitJson
{
	// Token: 0x02000E89 RID: 3721
	internal enum ParserToken
	{
		// Token: 0x04006A39 RID: 27193
		None = 65536,
		// Token: 0x04006A3A RID: 27194
		Number,
		// Token: 0x04006A3B RID: 27195
		True,
		// Token: 0x04006A3C RID: 27196
		False,
		// Token: 0x04006A3D RID: 27197
		Null,
		// Token: 0x04006A3E RID: 27198
		CharSeq,
		// Token: 0x04006A3F RID: 27199
		Char,
		// Token: 0x04006A40 RID: 27200
		Text,
		// Token: 0x04006A41 RID: 27201
		Object,
		// Token: 0x04006A42 RID: 27202
		ObjectPrime,
		// Token: 0x04006A43 RID: 27203
		Pair,
		// Token: 0x04006A44 RID: 27204
		PairRest,
		// Token: 0x04006A45 RID: 27205
		Array,
		// Token: 0x04006A46 RID: 27206
		ArrayPrime,
		// Token: 0x04006A47 RID: 27207
		Value,
		// Token: 0x04006A48 RID: 27208
		ValueRest,
		// Token: 0x04006A49 RID: 27209
		String,
		// Token: 0x04006A4A RID: 27210
		End,
		// Token: 0x04006A4B RID: 27211
		Epsilon
	}
}
