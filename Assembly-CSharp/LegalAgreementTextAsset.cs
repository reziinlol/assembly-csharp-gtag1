using System;
using UnityEngine;

// Token: 0x02000BD3 RID: 3027
[CreateAssetMenu(fileName = "NewLegalAgreementAsset", menuName = "Gorilla Tag/Legal Agreement Asset")]
public class LegalAgreementTextAsset : ScriptableObject
{
	// Token: 0x04005EDE RID: 24286
	public string title;

	// Token: 0x04005EDF RID: 24287
	public string playFabKey;

	// Token: 0x04005EE0 RID: 24288
	public string latestVersionKey;

	// Token: 0x04005EE1 RID: 24289
	[TextArea(3, 5)]
	public string errorMessage;

	// Token: 0x04005EE2 RID: 24290
	public bool optional;

	// Token: 0x04005EE3 RID: 24291
	public LegalAgreementTextAsset.PostAcceptAction optInAction;

	// Token: 0x04005EE4 RID: 24292
	public string confirmString;

	// Token: 0x02000BD4 RID: 3028
	public enum PostAcceptAction
	{
		// Token: 0x04005EE6 RID: 24294
		NONE
	}
}
