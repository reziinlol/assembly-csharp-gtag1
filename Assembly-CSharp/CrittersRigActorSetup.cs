using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class CrittersRigActorSetup : MonoBehaviour
{
	// Token: 0x060002E8 RID: 744 RVA: 0x0001159F File Offset: 0x0000F79F
	public void OnEnable()
	{
		CrittersManager.RegisterRigActorSetup(this);
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x000115A8 File Offset: 0x0000F7A8
	public void OnDisable()
	{
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			this.rigActors[i].actorSet = null;
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x000115DC File Offset: 0x0000F7DC
	private CrittersActor RefreshActorForIndex(int index)
	{
		CrittersRigActorSetup.RigActor rigActor = this.rigActors[index];
		if (rigActor.actorSet.IsNotNull())
		{
			rigActor.actorSet.gameObject.SetActive(false);
		}
		CrittersActor crittersActor = CrittersManager.instance.SpawnActor(rigActor.type, rigActor.subIndex);
		if (crittersActor.IsNull())
		{
			return null;
		}
		crittersActor.isOnPlayer = true;
		crittersActor.rigIndex = index;
		crittersActor.rigPlayerId = this.myRig.Creator.ActorNumber;
		if (crittersActor.rigPlayerId == -1 && PhotonNetwork.InRoom)
		{
			crittersActor.rigPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
		}
		crittersActor.PlacePlayerCrittersActor();
		return crittersActor;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00011684 File Offset: 0x0000F884
	public void CheckUpdate(ref List<object> refActorData, bool forceCheck = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		for (int i = 0; i < this.rigActors.Length; i++)
		{
			CrittersRigActorSetup.RigActor rigActor = this.rigActors[i];
			RigContainer rigContainer;
			if (forceCheck || rigActor.actorSet == null || (rigActor.actorSet.rigPlayerId != this.myRig.Creator.ActorNumber && VRRigCache.Instance.TryGetVrrig(this.myRig.Creator, out rigContainer) && CrittersManager.instance.rigSetupByRig.ContainsKey(this.myRig)))
			{
				CrittersActor crittersActor = this.RefreshActorForIndex(i);
				if (crittersActor != null)
				{
					crittersActor.AddPlayerCrittersActorDataToList(ref refActorData);
				}
			}
		}
	}

	// Token: 0x04000350 RID: 848
	public CrittersRigActorSetup.RigActor[] rigActors;

	// Token: 0x04000351 RID: 849
	public List<object> rigActorData = new List<object>();

	// Token: 0x04000352 RID: 850
	public VRRig myRig;

	// Token: 0x02000077 RID: 119
	[Serializable]
	public struct RigActor
	{
		// Token: 0x04000353 RID: 851
		public Transform location;

		// Token: 0x04000354 RID: 852
		public CrittersActor.CrittersActorType type;

		// Token: 0x04000355 RID: 853
		public int subIndex;

		// Token: 0x04000356 RID: 854
		public CrittersActor actorSet;
	}
}
