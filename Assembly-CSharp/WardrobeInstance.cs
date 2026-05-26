using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class WardrobeInstance : MonoBehaviour
{
	// Token: 0x06002395 RID: 9109 RVA: 0x000BFCA7 File Offset: 0x000BDEA7
	public void Start()
	{
		CosmeticsController.instance.AddWardrobeInstance(this);
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000BFCB6 File Offset: 0x000BDEB6
	public void OnDestroy()
	{
		CosmeticsController.instance.RemoveWardrobeInstance(this);
	}

	// Token: 0x04002EBA RID: 11962
	public WardrobeItemButton[] wardrobeItemButtons;

	// Token: 0x04002EBB RID: 11963
	public HeadModel selfDoll;
}
