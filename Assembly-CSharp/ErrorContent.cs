using System;

// Token: 0x02000B16 RID: 2838
[Serializable]
public class ErrorContent
{
	// Token: 0x170006CB RID: 1739
	// (get) Token: 0x06004844 RID: 18500 RVA: 0x0018294B File Offset: 0x00180B4B
	// (set) Token: 0x06004845 RID: 18501 RVA: 0x00182953 File Offset: 0x00180B53
	public string Message { get; set; }

	// Token: 0x170006CC RID: 1740
	// (get) Token: 0x06004846 RID: 18502 RVA: 0x0018295C File Offset: 0x00180B5C
	// (set) Token: 0x06004847 RID: 18503 RVA: 0x00182964 File Offset: 0x00180B64
	public string Error { get; set; }

	// Token: 0x06004848 RID: 18504 RVA: 0x0018296D File Offset: 0x00180B6D
	public override string ToString()
	{
		return "Error: " + this.Error + ", Message: " + this.Message;
	}
}
