using System;
using UnityEngine;

// Token: 0x02000362 RID: 866
[CreateAssetMenu(fileName = "PlatformTagJoin", menuName = "ScriptableObjects/PlatformTagJoin", order = 0)]
public class PlatformTagJoin : ScriptableObject
{
	// Token: 0x06001530 RID: 5424 RVA: 0x000707CA File Offset: 0x0006E9CA
	public override string ToString()
	{
		return this.PlatformTag;
	}

	// Token: 0x04001A04 RID: 6660
	public string PlatformTag = " ";
}
