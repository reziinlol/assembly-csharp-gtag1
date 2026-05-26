using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
[Serializable]
public struct KeyValueStringPair
{
	// Token: 0x0600047D RID: 1149 RVA: 0x000198AD File Offset: 0x00017AAD
	public KeyValueStringPair(string key, string value)
	{
		this.Key = key;
		this.Value = value;
	}

	// Token: 0x040004DA RID: 1242
	public string Key;

	// Token: 0x040004DB RID: 1243
	[Multiline]
	public string Value;
}
