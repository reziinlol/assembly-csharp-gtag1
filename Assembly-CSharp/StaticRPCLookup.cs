using System;
using System.Collections.Generic;

// Token: 0x02000483 RID: 1155
public class StaticRPCLookup
{
	// Token: 0x06001C28 RID: 7208 RVA: 0x00098B4C File Offset: 0x00096D4C
	public void Add(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		int count = this.entries.Count;
		this.entries.Add(new StaticRPCEntry(placeholder, code, lookupMethod));
		this.eventCodeEntryLookup.Add(code, count);
		this.placeholderEntryLookup.Add(placeholder, count);
	}

	// Token: 0x06001C29 RID: 7209 RVA: 0x00098B92 File Offset: 0x00096D92
	public NetworkSystem.StaticRPC CodeToMethod(byte code)
	{
		return this.entries[this.eventCodeEntryLookup[code]].lookupMethod;
	}

	// Token: 0x06001C2A RID: 7210 RVA: 0x00098BB0 File Offset: 0x00096DB0
	public byte PlaceholderToCode(NetworkSystem.StaticRPCPlaceholder placeholder)
	{
		return this.entries[this.placeholderEntryLookup[placeholder]].code;
	}

	// Token: 0x0400263B RID: 9787
	public List<StaticRPCEntry> entries = new List<StaticRPCEntry>();

	// Token: 0x0400263C RID: 9788
	private Dictionary<byte, int> eventCodeEntryLookup = new Dictionary<byte, int>();

	// Token: 0x0400263D RID: 9789
	private Dictionary<NetworkSystem.StaticRPCPlaceholder, int> placeholderEntryLookup = new Dictionary<NetworkSystem.StaticRPCPlaceholder, int>();
}
