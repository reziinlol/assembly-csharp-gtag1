using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000D53 RID: 3411
public class Vector3Converter : JsonConverter
{
	// Token: 0x060053CA RID: 21450 RVA: 0x001B66E0 File Offset: 0x001B48E0
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Vector3 vector = (Vector3)value;
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(vector.x);
		writer.WritePropertyName("y");
		writer.WriteValue(vector.y);
		writer.WritePropertyName("z");
		writer.WriteValue(vector.z);
		writer.WriteEndObject();
	}

	// Token: 0x060053CB RID: 21451 RVA: 0x001B6748 File Offset: 0x001B4948
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		JObject jobject = JObject.Load(reader);
		return new Vector3((float)jobject["x"], (float)jobject["y"], (float)jobject["z"]);
	}

	// Token: 0x060053CC RID: 21452 RVA: 0x001B6799 File Offset: 0x001B4999
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}
}
