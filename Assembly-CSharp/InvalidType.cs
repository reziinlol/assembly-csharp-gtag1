using System;

// Token: 0x02000DB8 RID: 3512
public class InvalidType : ProxyType
{
	// Token: 0x17000813 RID: 2067
	// (get) Token: 0x06005609 RID: 22025 RVA: 0x001BFA1D File Offset: 0x001BDC1D
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x17000814 RID: 2068
	// (get) Token: 0x0600560A RID: 22026 RVA: 0x001BFA2A File Offset: 0x001BDC2A
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x17000815 RID: 2069
	// (get) Token: 0x0600560B RID: 22027 RVA: 0x001BFA37 File Offset: 0x001BDC37
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x04006608 RID: 26120
	private Type _self = typeof(InvalidType);
}
