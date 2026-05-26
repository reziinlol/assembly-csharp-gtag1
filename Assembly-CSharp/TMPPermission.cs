using System;
using Newtonsoft.Json;

// Token: 0x02000B0E RID: 2830
[Serializable]
public class TMPPermission
{
	// Token: 0x170006BF RID: 1727
	// (get) Token: 0x06004824 RID: 18468 RVA: 0x00182877 File Offset: 0x00180A77
	// (set) Token: 0x06004825 RID: 18469 RVA: 0x0018287F File Offset: 0x00180A7F
	[JsonProperty("name")]
	public string Name { get; set; }

	// Token: 0x170006C0 RID: 1728
	// (get) Token: 0x06004826 RID: 18470 RVA: 0x00182888 File Offset: 0x00180A88
	// (set) Token: 0x06004827 RID: 18471 RVA: 0x00182890 File Offset: 0x00180A90
	[JsonProperty("enabled")]
	public bool Enabled { get; set; }

	// Token: 0x170006C1 RID: 1729
	// (get) Token: 0x06004828 RID: 18472 RVA: 0x00182899 File Offset: 0x00180A99
	// (set) Token: 0x06004829 RID: 18473 RVA: 0x001828A1 File Offset: 0x00180AA1
	[JsonProperty("managedBy")]
	public ManagedBy ManagedBy { get; set; }
}
