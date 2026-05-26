using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000D19 RID: 3353
[GTStripGameObjectFromBuild("!QATESTING")]
public class TestScript : MonoBehaviour
{
	// Token: 0x170007D6 RID: 2006
	// (get) Token: 0x060052D5 RID: 21205 RVA: 0x00002076 File Offset: 0x00000276
	public int callbackOrder
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170007D7 RID: 2007
	// (get) Token: 0x060052D6 RID: 21206 RVA: 0x00002076 File Offset: 0x00000276
	public static bool IsUIOpen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x04006443 RID: 25667
	public GameObject testDelete;
}
