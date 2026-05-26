using System;
using UnityEngine;

// Token: 0x02000DB3 RID: 3507
public class AssetContentAPI : ScriptableObject
{
	// Token: 0x040065FE RID: 26110
	public string bundleName;

	// Token: 0x040065FF RID: 26111
	public LazyLoadReference<TextAsset> bundleFile;

	// Token: 0x04006600 RID: 26112
	public Object[] assets = new Object[0];
}
