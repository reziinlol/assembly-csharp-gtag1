using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000E71 RID: 3697
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06005A41 RID: 23105
		bool IsArray { get; }

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06005A42 RID: 23106
		bool IsBoolean { get; }

		// Token: 0x1700088D RID: 2189
		// (get) Token: 0x06005A43 RID: 23107
		bool IsDouble { get; }

		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x06005A44 RID: 23108
		bool IsInt { get; }

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x06005A45 RID: 23109
		bool IsLong { get; }

		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x06005A46 RID: 23110
		bool IsObject { get; }

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x06005A47 RID: 23111
		bool IsString { get; }

		// Token: 0x06005A48 RID: 23112
		bool GetBoolean();

		// Token: 0x06005A49 RID: 23113
		double GetDouble();

		// Token: 0x06005A4A RID: 23114
		int GetInt();

		// Token: 0x06005A4B RID: 23115
		JsonType GetJsonType();

		// Token: 0x06005A4C RID: 23116
		long GetLong();

		// Token: 0x06005A4D RID: 23117
		string GetString();

		// Token: 0x06005A4E RID: 23118
		void SetBoolean(bool val);

		// Token: 0x06005A4F RID: 23119
		void SetDouble(double val);

		// Token: 0x06005A50 RID: 23120
		void SetInt(int val);

		// Token: 0x06005A51 RID: 23121
		void SetJsonType(JsonType type);

		// Token: 0x06005A52 RID: 23122
		void SetLong(long val);

		// Token: 0x06005A53 RID: 23123
		void SetString(string val);

		// Token: 0x06005A54 RID: 23124
		string ToJson();

		// Token: 0x06005A55 RID: 23125
		void ToJson(JsonWriter writer);
	}
}
