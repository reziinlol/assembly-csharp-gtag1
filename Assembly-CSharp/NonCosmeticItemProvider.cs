using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public class NonCosmeticItemProvider : MonoBehaviour
{
	// Token: 0x06001C34 RID: 7220 RVA: 0x00098C50 File Offset: 0x00096E50
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component != null)
		{
			GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, new object[]
			{
				true,
				component.isLeftHand
			});
		}
	}

	// Token: 0x04002644 RID: 9796
	public GTZone zone;

	// Token: 0x04002645 RID: 9797
	[Tooltip("only for honeycomb")]
	public bool useCondition;

	// Token: 0x04002646 RID: 9798
	public int conditionThreshold;

	// Token: 0x04002647 RID: 9799
	public NonCosmeticItemProvider.ItemType itemType;

	// Token: 0x02000488 RID: 1160
	public enum ItemType
	{
		// Token: 0x04002649 RID: 9801
		honeycomb
	}
}
