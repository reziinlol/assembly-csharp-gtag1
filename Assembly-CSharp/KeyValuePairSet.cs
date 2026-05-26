using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
[CreateAssetMenu(fileName = "New KeyValuePairSet", menuName = "Data/KeyValuePairSet", order = 0)]
public class KeyValuePairSet : ScriptableObject
{
	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600047B RID: 1147 RVA: 0x000198A5 File Offset: 0x00017AA5
	public KeyValueStringPair[] Entries
	{
		get
		{
			return this.m_entries;
		}
	}

	// Token: 0x040004D9 RID: 1241
	[SerializeField]
	private KeyValueStringPair[] m_entries;
}
