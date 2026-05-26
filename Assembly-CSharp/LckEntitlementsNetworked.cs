using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

// Token: 0x020003FB RID: 1019
public class LckEntitlementsNetworked : MonoBehaviour
{
	// Token: 0x06001830 RID: 6192 RVA: 0x00089AD0 File Offset: 0x00087CD0
	public void Awake()
	{
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			Debug.LogError("LCK: Unable to find VRRigSerializer for LckEntitlementsNetworked.");
			return;
		}
		InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		if (succesfullSpawnEvent == null)
		{
			return;
		}
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
		succesfullSpawnEvent.Add(inAction);
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x00089B34 File Offset: 0x00087D34
	private void OnSuccessfulSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		if (LckEntitlementsManager.Instance == null)
		{
			Debug.LogError("LCK: LckEntitlementsManager.Instance is not available in the scene!");
			return;
		}
		string userId = this.m_rigNetworkController.VRRig.OwningNetPlayer.UserId;
		if (userId.IsNullOrEmpty())
		{
			Debug.LogError("LCK: owningUserId is null on spawn. Cannot process entitlements.");
			return;
		}
		if (rig.Rig.isLocal)
		{
			LckEntitlementsManager.Instance.OnLocalPlayerSpawned(userId);
			return;
		}
		LckEntitlementsManager.Instance.OnRemotePlayerSpawned(userId);
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x00089BA8 File Offset: 0x00087DA8
	private void OnDestroy()
	{
		if (this.m_rigNetworkController != null && this.m_rigNetworkController.SuccesfullSpawnEvent != null)
		{
			ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
			InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
			succesfullSpawnEvent.Remove(inAction);
		}
	}

	// Token: 0x04002361 RID: 9057
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;
}
