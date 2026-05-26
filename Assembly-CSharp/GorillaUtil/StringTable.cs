using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaUtil
{
	// Token: 0x02000E8A RID: 3722
	[CreateAssetMenu(fileName = "StringTable", menuName = "Scriptable Objects/StringTable")]
	public class StringTable : ScriptableObject
	{
		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x06005B83 RID: 23427 RVA: 0x001D26A3 File Offset: 0x001D08A3
		public int Count
		{
			get
			{
				return this.entries.Length;
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x06005B84 RID: 23428 RVA: 0x001D26AD File Offset: 0x001D08AD
		public string KeyList
		{
			get
			{
				if (this.keyList.IsNullOrEmpty() && this.entries.Length != 0)
				{
					return this.buildKeyList();
				}
				return this.keyList;
			}
		}

		// Token: 0x06005B85 RID: 23429 RVA: 0x001D26D4 File Offset: 0x001D08D4
		private string buildKeyList()
		{
			this.keyList = string.Empty;
			if (this.entries.Length != 0)
			{
				for (int i = 0; i < this.entries.Length - 1; i++)
				{
					this.keyList = this.keyList + this.entries[i].Key + ", ";
				}
				this.keyList += this.entries[this.entries.Length - 1].Key;
			}
			return this.keyList;
		}

		// Token: 0x06005B86 RID: 23430 RVA: 0x001D2764 File Offset: 0x001D0964
		public bool ContainsKey(string key)
		{
			if (this.dict == null)
			{
				this.dict = new Dictionary<string, string>();
				for (int i = 0; i < this.entries.Length; i++)
				{
					this.dict.Add(this.entries[i].Key, this.entries[i].Value);
				}
			}
			return this.dict.ContainsKey(key);
		}

		// Token: 0x06005B87 RID: 23431 RVA: 0x001D27D0 File Offset: 0x001D09D0
		public string FetchValue(string key)
		{
			if (this.ContainsKey(key))
			{
				return this.dict[key];
			}
			return null;
		}

		// Token: 0x04006A4C RID: 27212
		[SerializeField]
		private StringTable.StringPair[] entries;

		// Token: 0x04006A4D RID: 27213
		private Dictionary<string, string> dict;

		// Token: 0x04006A4E RID: 27214
		private string keyList;

		// Token: 0x02000E8B RID: 3723
		[Serializable]
		private struct StringPair
		{
			// Token: 0x04006A4F RID: 27215
			public string Key;

			// Token: 0x04006A50 RID: 27216
			public string Value;
		}
	}
}
