using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000E72 RID: 3698
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x06005A56 RID: 23126 RVA: 0x001CDD42 File Offset: 0x001CBF42
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x06005A57 RID: 23127 RVA: 0x001CDD4F File Offset: 0x001CBF4F
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x17000894 RID: 2196
		// (get) Token: 0x06005A58 RID: 23128 RVA: 0x001CDD5A File Offset: 0x001CBF5A
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06005A59 RID: 23129 RVA: 0x001CDD65 File Offset: 0x001CBF65
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x17000896 RID: 2198
		// (get) Token: 0x06005A5A RID: 23130 RVA: 0x001CDD70 File Offset: 0x001CBF70
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x17000897 RID: 2199
		// (get) Token: 0x06005A5B RID: 23131 RVA: 0x001CDD7B File Offset: 0x001CBF7B
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x17000898 RID: 2200
		// (get) Token: 0x06005A5C RID: 23132 RVA: 0x001CDD86 File Offset: 0x001CBF86
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x17000899 RID: 2201
		// (get) Token: 0x06005A5D RID: 23133 RVA: 0x001CDD91 File Offset: 0x001CBF91
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x1700089A RID: 2202
		// (get) Token: 0x06005A5E RID: 23134 RVA: 0x001CDD9C File Offset: 0x001CBF9C
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x06005A5F RID: 23135 RVA: 0x001CDDA4 File Offset: 0x001CBFA4
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x06005A60 RID: 23136 RVA: 0x001CDDB1 File Offset: 0x001CBFB1
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x06005A61 RID: 23137 RVA: 0x001CDDBE File Offset: 0x001CBFBE
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x06005A62 RID: 23138 RVA: 0x001CDDCB File Offset: 0x001CBFCB
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x06005A63 RID: 23139 RVA: 0x001CDDD8 File Offset: 0x001CBFD8
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x06005A64 RID: 23140 RVA: 0x001CDE40 File Offset: 0x001CC040
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x06005A65 RID: 23141 RVA: 0x001CDEA8 File Offset: 0x001CC0A8
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x06005A66 RID: 23142 RVA: 0x001CDEB0 File Offset: 0x001CC0B0
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x06005A67 RID: 23143 RVA: 0x001CDEB8 File Offset: 0x001CC0B8
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x06005A68 RID: 23144 RVA: 0x001CDEC0 File Offset: 0x001CC0C0
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x06005A69 RID: 23145 RVA: 0x001CDEC8 File Offset: 0x001CC0C8
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x06005A6A RID: 23146 RVA: 0x001CDED0 File Offset: 0x001CC0D0
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06005A6B RID: 23147 RVA: 0x001CDED8 File Offset: 0x001CC0D8
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x06005A6C RID: 23148 RVA: 0x001CDEE0 File Offset: 0x001CC0E0
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x06005A6D RID: 23149 RVA: 0x001CDEED File Offset: 0x001CC0ED
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x170008AA RID: 2218
		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = this.ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		// Token: 0x170008AB RID: 2219
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData value2 = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = value2;
				KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
				this.object_list[idx] = value3;
			}
		}

		// Token: 0x170008AC RID: 2220
		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData value2 = this.ToJsonData(value);
				this[index] = value2;
			}
		}

		// Token: 0x170008AD RID: 2221
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x170008AE RID: 2222
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = value2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x06005A78 RID: 23160 RVA: 0x00002050 File Offset: 0x00000250
		public JsonData()
		{
		}

		// Token: 0x06005A79 RID: 23161 RVA: 0x001CE14B File Offset: 0x001CC34B
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x06005A7A RID: 23162 RVA: 0x001CE161 File Offset: 0x001CC361
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x06005A7B RID: 23163 RVA: 0x001CE177 File Offset: 0x001CC377
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x06005A7C RID: 23164 RVA: 0x001CE18D File Offset: 0x001CC38D
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x06005A7D RID: 23165 RVA: 0x001CE1A4 File Offset: 0x001CC3A4
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x06005A7E RID: 23166 RVA: 0x001CE24D File Offset: 0x001CC44D
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x001CE263 File Offset: 0x001CC463
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x001CE26B File Offset: 0x001CC46B
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x001CE273 File Offset: 0x001CC473
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x001CE27B File Offset: 0x001CC47B
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001CE283 File Offset: 0x001CC483
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06005A84 RID: 23172 RVA: 0x001CE28B File Offset: 0x001CC48B
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06005A85 RID: 23173 RVA: 0x001CE2A7 File Offset: 0x001CC4A7
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06005A86 RID: 23174 RVA: 0x001CE2C3 File Offset: 0x001CC4C3
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06005A87 RID: 23175 RVA: 0x001CE2DF File Offset: 0x001CC4DF
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x001CE2FB File Offset: 0x001CC4FB
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x001CE317 File Offset: 0x001CC517
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x06005A8A RID: 23178 RVA: 0x001CE328 File Offset: 0x001CC528
		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			this.object_list.Add(item);
			this.json = null;
		}

		// Token: 0x06005A8B RID: 23179 RVA: 0x001CE36B File Offset: 0x001CC56B
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x06005A8C RID: 23180 RVA: 0x001CE38A File Offset: 0x001CC58A
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x06005A8D RID: 23181 RVA: 0x001CE398 File Offset: 0x001CC598
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x06005A8E RID: 23182 RVA: 0x001CE3A0 File Offset: 0x001CC5A0
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x06005A8F RID: 23183 RVA: 0x001CE405 File Offset: 0x001CC605
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x06005A90 RID: 23184 RVA: 0x001CE412 File Offset: 0x001CC612
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x06005A91 RID: 23185 RVA: 0x001CE42E File Offset: 0x001CC62E
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06005A92 RID: 23186 RVA: 0x001CE44A File Offset: 0x001CC64A
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06005A93 RID: 23187 RVA: 0x001CE466 File Offset: 0x001CC666
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06005A94 RID: 23188 RVA: 0x001CE482 File Offset: 0x001CC682
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06005A95 RID: 23189 RVA: 0x001CE49E File Offset: 0x001CC69E
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06005A96 RID: 23190 RVA: 0x001CE4B5 File Offset: 0x001CC6B5
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06005A97 RID: 23191 RVA: 0x001CE4CC File Offset: 0x001CC6CC
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06005A98 RID: 23192 RVA: 0x001CE4E3 File Offset: 0x001CC6E3
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06005A99 RID: 23193 RVA: 0x001CE4FA File Offset: 0x001CC6FA
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x06005A9A RID: 23194 RVA: 0x001CE511 File Offset: 0x001CC711
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x06005A9B RID: 23195 RVA: 0x001CE519 File Offset: 0x001CC719
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x06005A9C RID: 23196 RVA: 0x001CE522 File Offset: 0x001CC722
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x06005A9D RID: 23197 RVA: 0x001CE52B File Offset: 0x001CC72B
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x06005A9E RID: 23198 RVA: 0x001CE53F File Offset: 0x001CC73F
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x06005A9F RID: 23199 RVA: 0x001CE54D File Offset: 0x001CC74D
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x06005AA0 RID: 23200 RVA: 0x001CE55B File Offset: 0x001CC75B
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x06005AA1 RID: 23201 RVA: 0x001CE571 File Offset: 0x001CC771
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06005AA2 RID: 23202 RVA: 0x001CE586 File Offset: 0x001CC786
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06005AA3 RID: 23203 RVA: 0x001CE59B File Offset: 0x001CC79B
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06005AA4 RID: 23204 RVA: 0x001CE5B4 File Offset: 0x001CC7B4
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this.ToJsonData(value);
			this[text] = value2;
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			this.object_list.Insert(idx, item);
		}

		// Token: 0x06005AA5 RID: 23205 RVA: 0x001CE5F0 File Offset: 0x001CC7F0
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06005AA6 RID: 23206 RVA: 0x001CE630 File Offset: 0x001CC830
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x06005AA7 RID: 23207 RVA: 0x001CE668 File Offset: 0x001CC868
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x06005AA8 RID: 23208 RVA: 0x001CE6C8 File Offset: 0x001CC8C8
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x06005AA9 RID: 23209 RVA: 0x001CE71A File Offset: 0x001CC91A
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x06005AAA RID: 23210 RVA: 0x001CE738 File Offset: 0x001CC938
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x06005AAB RID: 23211 RVA: 0x001CE880 File Offset: 0x001CCA80
		public int Add(object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(value2);
		}

		// Token: 0x06005AAC RID: 23212 RVA: 0x001CE8A8 File Offset: 0x001CCAA8
		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		// Token: 0x06005AAD RID: 23213 RVA: 0x001CE8C8 File Offset: 0x001CCAC8
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x06005AAE RID: 23214 RVA: 0x001CE99D File Offset: 0x001CCB9D
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x06005AAF RID: 23215 RVA: 0x001CE9A8 File Offset: 0x001CCBA8
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x06005AB0 RID: 23216 RVA: 0x001CEA48 File Offset: 0x001CCC48
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x06005AB1 RID: 23217 RVA: 0x001CEA94 File Offset: 0x001CCC94
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001CEAC0 File Offset: 0x001CCCC0
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x040069B4 RID: 27060
		private IList<JsonData> inst_array;

		// Token: 0x040069B5 RID: 27061
		private bool inst_boolean;

		// Token: 0x040069B6 RID: 27062
		private double inst_double;

		// Token: 0x040069B7 RID: 27063
		private int inst_int;

		// Token: 0x040069B8 RID: 27064
		private long inst_long;

		// Token: 0x040069B9 RID: 27065
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x040069BA RID: 27066
		private string inst_string;

		// Token: 0x040069BB RID: 27067
		private string json;

		// Token: 0x040069BC RID: 27068
		private JsonType type;

		// Token: 0x040069BD RID: 27069
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}
