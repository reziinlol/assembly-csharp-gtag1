using System;
using System.Collections.Generic;
using KID.Model;

// Token: 0x02000B10 RID: 2832
[Serializable]
public class KIDSession
{
	// Token: 0x170006C2 RID: 1730
	// (get) Token: 0x0600482C RID: 18476 RVA: 0x001828AA File Offset: 0x00180AAA
	// (set) Token: 0x0600482D RID: 18477 RVA: 0x001828B2 File Offset: 0x00180AB2
	public SessionStatus SessionStatus { get; set; }

	// Token: 0x170006C3 RID: 1731
	// (get) Token: 0x0600482E RID: 18478 RVA: 0x001828BB File Offset: 0x00180ABB
	// (set) Token: 0x0600482F RID: 18479 RVA: 0x001828C3 File Offset: 0x00180AC3
	public GTAgeStatusType AgeStatus { get; set; }

	// Token: 0x170006C4 RID: 1732
	// (get) Token: 0x06004830 RID: 18480 RVA: 0x001828CC File Offset: 0x00180ACC
	// (set) Token: 0x06004831 RID: 18481 RVA: 0x001828D4 File Offset: 0x00180AD4
	public Guid SessionId { get; set; }

	// Token: 0x170006C5 RID: 1733
	// (get) Token: 0x06004832 RID: 18482 RVA: 0x001828DD File Offset: 0x00180ADD
	// (set) Token: 0x06004833 RID: 18483 RVA: 0x001828E5 File Offset: 0x00180AE5
	public string KUID { get; set; }

	// Token: 0x170006C6 RID: 1734
	// (get) Token: 0x06004834 RID: 18484 RVA: 0x001828EE File Offset: 0x00180AEE
	// (set) Token: 0x06004835 RID: 18485 RVA: 0x001828F6 File Offset: 0x00180AF6
	public string etag { get; set; }

	// Token: 0x170006C7 RID: 1735
	// (get) Token: 0x06004836 RID: 18486 RVA: 0x001828FF File Offset: 0x00180AFF
	// (set) Token: 0x06004837 RID: 18487 RVA: 0x00182907 File Offset: 0x00180B07
	public List<Permission> Permissions { get; set; }

	// Token: 0x170006C8 RID: 1736
	// (get) Token: 0x06004838 RID: 18488 RVA: 0x00182910 File Offset: 0x00180B10
	// (set) Token: 0x06004839 RID: 18489 RVA: 0x00182918 File Offset: 0x00180B18
	public DateTime DateOfBirth { get; set; }

	// Token: 0x170006C9 RID: 1737
	// (get) Token: 0x0600483A RID: 18490 RVA: 0x00182921 File Offset: 0x00180B21
	// (set) Token: 0x0600483B RID: 18491 RVA: 0x00182929 File Offset: 0x00180B29
	public string Jurisdiction { get; set; }
}
