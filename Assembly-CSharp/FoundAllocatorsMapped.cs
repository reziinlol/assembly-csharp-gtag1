using System;
using System.Collections.Generic;

// Token: 0x0200028C RID: 652
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x040014D7 RID: 5335
	public string path;

	// Token: 0x040014D8 RID: 5336
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x040014D9 RID: 5337
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}
