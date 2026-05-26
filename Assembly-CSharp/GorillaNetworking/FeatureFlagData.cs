using System;
using System.Collections.Generic;

namespace GorillaNetworking
{
	// Token: 0x02001070 RID: 4208
	[Serializable]
	internal class FeatureFlagData
	{
		// Token: 0x040079AC RID: 31148
		public string name;

		// Token: 0x040079AD RID: 31149
		public int value;

		// Token: 0x040079AE RID: 31150
		public string valueType;

		// Token: 0x040079AF RID: 31151
		public List<string> alwaysOnForUsers;
	}
}
