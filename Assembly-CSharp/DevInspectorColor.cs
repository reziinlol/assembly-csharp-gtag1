using System;

// Token: 0x0200031D RID: 797
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060013E0 RID: 5088 RVA: 0x0006BA08 File Offset: 0x00069C08
	public string Color { get; }

	// Token: 0x060013E1 RID: 5089 RVA: 0x0006BA10 File Offset: 0x00069C10
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}
