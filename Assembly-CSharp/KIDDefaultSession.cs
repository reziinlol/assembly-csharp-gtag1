using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B05 RID: 2821
[Serializable]
public class KIDDefaultSession
{
	// Token: 0x170006BC RID: 1724
	// (get) Token: 0x0600481D RID: 18461 RVA: 0x00182844 File Offset: 0x00180A44
	// (set) Token: 0x0600481E RID: 18462 RVA: 0x0018284C File Offset: 0x00180A4C
	public List<Permission> Permissions { get; set; }

	// Token: 0x170006BD RID: 1725
	// (get) Token: 0x0600481F RID: 18463 RVA: 0x00182855 File Offset: 0x00180A55
	// (set) Token: 0x06004820 RID: 18464 RVA: 0x0018285D File Offset: 0x00180A5D
	public AgeStatusType AgeStatus { get; set; }

	// Token: 0x170006BE RID: 1726
	// (get) Token: 0x06004821 RID: 18465 RVA: 0x00182866 File Offset: 0x00180A66
	// (set) Token: 0x06004822 RID: 18466 RVA: 0x0018286E File Offset: 0x00180A6E
	public int Age { get; set; }
}
