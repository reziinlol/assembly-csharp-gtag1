using System;

// Token: 0x02000482 RID: 1154
public class StaticRPCEntry
{
	// Token: 0x06001C27 RID: 7207 RVA: 0x00098B2C File Offset: 0x00096D2C
	public StaticRPCEntry(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		this.placeholder = placeholder;
		this.code = code;
		this.lookupMethod = lookupMethod;
	}

	// Token: 0x04002638 RID: 9784
	public NetworkSystem.StaticRPCPlaceholder placeholder;

	// Token: 0x04002639 RID: 9785
	public byte code;

	// Token: 0x0400263A RID: 9786
	public NetworkSystem.StaticRPC lookupMethod;
}
