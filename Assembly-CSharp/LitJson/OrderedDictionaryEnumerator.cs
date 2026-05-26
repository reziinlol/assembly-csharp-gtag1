using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000E73 RID: 3699
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x06005AB3 RID: 23219 RVA: 0x001CEB42 File Offset: 0x001CCD42
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x06005AB4 RID: 23220 RVA: 0x001CEB50 File Offset: 0x001CCD50
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x06005AB5 RID: 23221 RVA: 0x001CEB7C File Offset: 0x001CCD7C
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x06005AB6 RID: 23222 RVA: 0x001CEB9C File Offset: 0x001CCD9C
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06005AB7 RID: 23223 RVA: 0x001CEBBC File Offset: 0x001CCDBC
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06005AB8 RID: 23224 RVA: 0x001CEBCB File Offset: 0x001CCDCB
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06005AB9 RID: 23225 RVA: 0x001CEBD8 File Offset: 0x001CCDD8
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040069BE RID: 27070
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}
