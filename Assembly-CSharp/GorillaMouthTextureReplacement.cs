using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002CF RID: 719
public class GorillaMouthTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170001CB RID: 459
	// (get) Token: 0x0600126E RID: 4718 RVA: 0x00062691 File Offset: 0x00060891
	// (set) Token: 0x0600126F RID: 4719 RVA: 0x00062699 File Offset: 0x00060899
	public bool IsSpawned { get; set; }

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06001270 RID: 4720 RVA: 0x000626A2 File Offset: 0x000608A2
	// (set) Token: 0x06001271 RID: 4721 RVA: 0x000626AA File Offset: 0x000608AA
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001272 RID: 4722 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x000626B3 File Offset: 0x000608B3
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x000626BC File Offset: 0x000608BC
	private void OnEnable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().SetMouthTextureReplacement(this.newMouthAtlas);
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x000626D4 File Offset: 0x000608D4
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearMouthTextureReplacement();
	}

	// Token: 0x04001679 RID: 5753
	[SerializeField]
	private Texture2D newMouthAtlas;

	// Token: 0x0400167A RID: 5754
	private VRRig myRig;
}
