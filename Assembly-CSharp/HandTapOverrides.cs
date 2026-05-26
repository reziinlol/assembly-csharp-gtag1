using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020002D8 RID: 728
[Serializable]
public class HandTapOverrides
{
	// Token: 0x040016A9 RID: 5801
	private const string PREFAB_TOOLTIP = "Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.";

	// Token: 0x040016AA RID: 5802
	public bool overrideSurfacePrefab;

	// Token: 0x040016AB RID: 5803
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper surfaceTapPrefab;

	// Token: 0x040016AC RID: 5804
	public bool overrideGamemodePrefab;

	// Token: 0x040016AD RID: 5805
	[Tooltip("Must be in the global object pool and have a tag.\n\nPrefabs can have an FXModifier component to be adjusted after creation.")]
	public HashWrapper gamemodeTapPrefab;

	// Token: 0x040016AE RID: 5806
	public bool overrideSound;

	// Token: 0x040016AF RID: 5807
	public AudioClip tapSound;
}
