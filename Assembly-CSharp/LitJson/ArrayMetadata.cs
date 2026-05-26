using System;

namespace LitJson
{
	// Token: 0x02000E76 RID: 3702
	internal struct ArrayMetadata
	{
		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x06005AC1 RID: 23233 RVA: 0x001CEC64 File Offset: 0x001CCE64
		// (set) Token: 0x06005AC2 RID: 23234 RVA: 0x001CEC85 File Offset: 0x001CCE85
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x06005AC3 RID: 23235 RVA: 0x001CEC8E File Offset: 0x001CCE8E
		// (set) Token: 0x06005AC4 RID: 23236 RVA: 0x001CEC96 File Offset: 0x001CCE96
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x06005AC5 RID: 23237 RVA: 0x001CEC9F File Offset: 0x001CCE9F
		// (set) Token: 0x06005AC6 RID: 23238 RVA: 0x001CECA7 File Offset: 0x001CCEA7
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x040069C2 RID: 27074
		private Type element_type;

		// Token: 0x040069C3 RID: 27075
		private bool is_array;

		// Token: 0x040069C4 RID: 27076
		private bool is_list;
	}
}
