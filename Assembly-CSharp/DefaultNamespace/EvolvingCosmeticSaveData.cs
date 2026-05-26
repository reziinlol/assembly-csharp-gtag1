using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x02001300 RID: 4864
	[NullableContext(1)]
	[Nullable(0)]
	public class EvolvingCosmeticSaveData
	{
		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x0600798C RID: 31116 RVA: 0x0027FB94 File Offset: 0x0027DD94
		public static EvolvingCosmeticSaveData Instance
		{
			get
			{
				EvolvingCosmeticSaveData result;
				if ((result = EvolvingCosmeticSaveData.s_instance) == null)
				{
					result = (EvolvingCosmeticSaveData.s_instance = new EvolvingCosmeticSaveData());
				}
				return result;
			}
		}

		// Token: 0x0600798D RID: 31117 RVA: 0x0027FBAC File Offset: 0x0027DDAC
		private EvolvingCosmeticSaveData()
		{
			string @string = PlayerPrefs.GetString("EvolvingCosmeticSaveData");
			if (@string != null)
			{
				this.ReadFromJson(@string);
			}
		}

		// Token: 0x0600798E RID: 31118 RVA: 0x0027FBE0 File Offset: 0x0027DDE0
		public string Write()
		{
			JsonSerializer jsonSerializer = new JsonSerializer();
			string result;
			using (TextWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
				{
					jsonSerializer.Serialize(jsonWriter, this);
					result = textWriter.ToString();
				}
			}
			return result;
		}

		// Token: 0x0600798F RID: 31119 RVA: 0x0027FC48 File Offset: 0x0027DE48
		private void ReadFromJson(string json)
		{
			using (TextReader textReader = new StringReader(json))
			{
				using (JsonReader jsonReader = new JsonTextReader(textReader))
				{
					while (jsonReader.Read())
					{
						if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "SelectedIndices")
						{
							this.ReadSelectedIndices(jsonReader);
						}
					}
				}
			}
		}

		// Token: 0x06007990 RID: 31120 RVA: 0x0027FCC8 File Offset: 0x0027DEC8
		private void ReadSelectedIndices(JsonReader reader)
		{
			int num = 0;
			string text = null;
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType <= JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.StartObject)
					{
						if (tokenType == JsonToken.PropertyName)
						{
							if (text != null)
							{
								throw new Exception("Json read error");
							}
							string text2 = reader.Value as string;
							if (text2 == null)
							{
								throw new Exception("Json read error");
							}
							text = text2;
						}
					}
					else
					{
						num++;
					}
				}
				else if (tokenType != JsonToken.Integer)
				{
					if (tokenType == JsonToken.EndObject)
					{
						num--;
					}
				}
				else
				{
					if (text == null)
					{
						throw new Exception("Json read error");
					}
					object value = reader.Value;
					if (!(value is long))
					{
						throw new Exception("Json read error");
					}
					long num2 = (long)value;
					this.SelectedIndices[text] = (int)num2;
				}
				if (num <= 0)
				{
					return;
				}
			}
			throw new Exception("Json read error");
		}

		// Token: 0x04008C4E RID: 35918
		public readonly Dictionary<string, int> SelectedIndices = new Dictionary<string, int>();

		// Token: 0x04008C4F RID: 35919
		[Nullable(2)]
		private static EvolvingCosmeticSaveData s_instance;

		// Token: 0x04008C50 RID: 35920
		public const string PlayerPrefsKey = "EvolvingCosmeticSaveData";
	}
}
