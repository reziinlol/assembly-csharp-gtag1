using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02001091 RID: 4241
	public class CacheImport
	{
		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x06006A55 RID: 27221 RVA: 0x00226193 File Offset: 0x00224393
		// (set) Token: 0x06006A56 RID: 27222 RVA: 0x0022619B File Offset: 0x0022439B
		public string DeploymentId { get; set; }

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x06006A57 RID: 27223 RVA: 0x002261A4 File Offset: 0x002243A4
		// (set) Token: 0x06006A58 RID: 27224 RVA: 0x002261AC File Offset: 0x002243AC
		public Dictionary<string, Dictionary<string, string>> TitleData { get; set; }
	}
}
