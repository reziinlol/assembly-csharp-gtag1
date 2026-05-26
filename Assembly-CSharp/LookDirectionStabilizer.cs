using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class LookDirectionStabilizer : MonoBehaviour, ISpawnable
{
	// Token: 0x170001DA RID: 474
	// (get) Token: 0x060012CB RID: 4811 RVA: 0x00063FE2 File Offset: 0x000621E2
	// (set) Token: 0x060012CC RID: 4812 RVA: 0x00063FEA File Offset: 0x000621EA
	public bool IsSpawned { get; set; }

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x060012CD RID: 4813 RVA: 0x00063FF3 File Offset: 0x000621F3
	// (set) Token: 0x060012CE RID: 4814 RVA: 0x00063FFB File Offset: 0x000621FB
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x060012CF RID: 4815 RVA: 0x00064004 File Offset: 0x00062204
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060012D0 RID: 4816 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012D1 RID: 4817 RVA: 0x00064010 File Offset: 0x00062210
	private void Update()
	{
		Transform rigTarget = this.myRig.head.rigTarget;
		Vector3 up = this.myRig.transform.up;
		if (Vector3.Dot(rigTarget.forward, up) < 0f)
		{
			Quaternion b = Quaternion.LookRotation(rigTarget.up.ProjectOntoPlane(up), up);
			Quaternion rotation = base.transform.parent.rotation;
			float value = Vector3.Dot(rigTarget.up, up);
			base.transform.rotation = Quaternion.Lerp(rotation, b, Mathf.InverseLerp(1f, 0.7f, value));
			return;
		}
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x04001704 RID: 5892
	private VRRig myRig;
}
