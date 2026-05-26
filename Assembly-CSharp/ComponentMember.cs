using System;

// Token: 0x02000322 RID: 802
public class ComponentMember
{
	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060013E6 RID: 5094 RVA: 0x0006BA39 File Offset: 0x00069C39
	public string Name { get; }

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060013E7 RID: 5095 RVA: 0x0006BA41 File Offset: 0x00069C41
	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060013E8 RID: 5096 RVA: 0x0006BA4E File Offset: 0x00069C4E
	public bool IsStarred { get; }

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060013E9 RID: 5097 RVA: 0x0006BA56 File Offset: 0x00069C56
	public string Color { get; }

	// Token: 0x060013EA RID: 5098 RVA: 0x0006BA5E File Offset: 0x00069C5E
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x040018C3 RID: 6339
	private Func<string> getValue;

	// Token: 0x040018C4 RID: 6340
	public string computedPrefix;

	// Token: 0x040018C5 RID: 6341
	public string computedSuffix;
}
