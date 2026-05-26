using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000E77 RID: 3703
	internal struct ObjectMetadata
	{
		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x06005AC7 RID: 23239 RVA: 0x001CECB0 File Offset: 0x001CCEB0
		// (set) Token: 0x06005AC8 RID: 23240 RVA: 0x001CECD1 File Offset: 0x001CCED1
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

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x06005AC9 RID: 23241 RVA: 0x001CECDA File Offset: 0x001CCEDA
		// (set) Token: 0x06005ACA RID: 23242 RVA: 0x001CECE2 File Offset: 0x001CCEE2
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x06005ACB RID: 23243 RVA: 0x001CECEB File Offset: 0x001CCEEB
		// (set) Token: 0x06005ACC RID: 23244 RVA: 0x001CECF3 File Offset: 0x001CCEF3
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x040069C5 RID: 27077
		private Type element_type;

		// Token: 0x040069C6 RID: 27078
		private bool is_dictionary;

		// Token: 0x040069C7 RID: 27079
		private IDictionary<string, PropertyMetadata> properties;
	}
}
