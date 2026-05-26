using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000611 RID: 1553
[CreateAssetMenu(fileName = "BuilderTableSerializationConfig", menuName = "Gorilla Tag/Builder/Serialization", order = 0)]
public class BuilderTableSerializationConfig : ScriptableObject
{
	// Token: 0x04003219 RID: 12825
	public string tableConfigurationKey;

	// Token: 0x0400321A RID: 12826
	public string titleDataKey;

	// Token: 0x0400321B RID: 12827
	public string startingMapConfigKey;

	// Token: 0x0400321C RID: 12828
	public List<string> scanSlotMothershipKeys;

	// Token: 0x0400321D RID: 12829
	public string scanSlotDevKey;

	// Token: 0x0400321E RID: 12830
	public string publishedScanMothershipKey;

	// Token: 0x0400321F RID: 12831
	public string timeAppend;

	// Token: 0x04003220 RID: 12832
	public string playfabScanKey;

	// Token: 0x04003221 RID: 12833
	public string sharedBlocksApiBaseURL;

	// Token: 0x04003222 RID: 12834
	public string recentVotesPrefsKey;

	// Token: 0x04003223 RID: 12835
	public string localMapsPrefsKey;
}
