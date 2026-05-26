using System;
using Fusion;
using UnityEngine;

// Token: 0x02000843 RID: 2115
[NetworkBehaviourWeaved(0)]
internal class VrrigReliableSerializer : GorillaWrappedSerializer
{
	// Token: 0x060036AA RID: 13994 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnBeforeDespawn()
	{
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x0012D150 File Offset: 0x0012B350
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		if (wrappedInfo.punInfo.Sender != wrappedInfo.punInfo.photonView.Owner || wrappedInfo.punInfo.photonView.IsRoomView)
		{
			return false;
		}
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(wrappedInfo.Sender, out rigContainer))
		{
			outTargetObject = rigContainer.gameObject;
			outTargetType = typeof(VRRigReliableState);
			return true;
		}
		return false;
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x060036AF RID: 13999 RVA: 0x0012ACA5 File Offset: 0x00128EA5
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x0012ACB1 File Offset: 0x00128EB1
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
