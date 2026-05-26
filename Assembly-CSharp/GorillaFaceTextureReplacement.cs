using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002C4 RID: 708
public class GorillaFaceTextureReplacement : MonoBehaviour, ISpawnable
{
	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06001242 RID: 4674 RVA: 0x00061A97 File Offset: 0x0005FC97
	// (set) Token: 0x06001243 RID: 4675 RVA: 0x00061A9F File Offset: 0x0005FC9F
	public bool IsSpawned { get; set; }

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06001244 RID: 4676 RVA: 0x00061AA8 File Offset: 0x0005FCA8
	// (set) Token: 0x06001245 RID: 4677 RVA: 0x00061AB0 File Offset: 0x0005FCB0
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001246 RID: 4678 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06001247 RID: 4679 RVA: 0x00061AB9 File Offset: 0x0005FCB9
	public void OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06001248 RID: 4680 RVA: 0x00061AC4 File Offset: 0x0005FCC4
	private void OnEnable()
	{
		Material sharedMaterial = this.myRig.GetComponent<GorillaMouthFlap>().SetFaceMaterialReplacement(this.newFaceMaterial);
		MeshRenderer[] array = this.alsoApplyFaceTo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = sharedMaterial;
		}
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00061B06 File Offset: 0x0005FD06
	private void OnDisable()
	{
		this.myRig.GetComponent<GorillaMouthFlap>().ClearFaceMaterialReplacement();
	}

	// Token: 0x04001624 RID: 5668
	[SerializeField]
	private Material newFaceMaterial;

	// Token: 0x04001625 RID: 5669
	private VRRig myRig;

	// Token: 0x04001626 RID: 5670
	[SerializeField]
	private MeshRenderer[] alsoApplyFaceTo;
}
