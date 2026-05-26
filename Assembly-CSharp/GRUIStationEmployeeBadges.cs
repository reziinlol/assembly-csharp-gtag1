using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000822 RID: 2082
public class GRUIStationEmployeeBadges : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003582 RID: 13698 RVA: 0x0012843C File Offset: 0x0012663C
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Setup(reactor, i);
		}
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x0012847C File Offset: 0x0012667C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.registeredBadges = new List<GRBadge>();
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].index = i;
			this.badgeDispensers[i].actorNr = -1;
		}
		this.dispenserForActorNr = new Dictionary<int, int>();
		VRRigCache.OnRigActivated += this.UpdateRigs;
		VRRigCache.OnRigDeactivated += this.UpdateRigs;
		RoomSystem.JoinedRoomEvent += new Action(this.UpdateRigs);
		this.UpdateRigs();
	}

	// Token: 0x06003584 RID: 13700 RVA: 0x00128524 File Offset: 0x00126724
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		VRRigCache.OnRigActivated -= this.UpdateRigs;
		VRRigCache.OnRigDeactivated -= this.UpdateRigs;
		RoomSystem.JoinedRoomEvent -= new Action(this.UpdateRigs);
	}

	// Token: 0x06003585 RID: 13701 RVA: 0x00128576 File Offset: 0x00126776
	public void UpdateRigs(RigContainer container)
	{
		this.UpdateRigs();
	}

	// Token: 0x06003586 RID: 13702 RVA: 0x0012857E File Offset: 0x0012677E
	public void UpdateRigs()
	{
		GRUIStationEmployeeBadges.tempRigs.Clear();
		GRUIStationEmployeeBadges.tempRigs.Add(VRRig.LocalRig);
		if (VRRigCache.Instance != null)
		{
			VRRigCache.Instance.GetAllUsedRigs(GRUIStationEmployeeBadges.tempRigs);
		}
	}

	// Token: 0x06003587 RID: 13703 RVA: 0x001285B8 File Offset: 0x001267B8
	public void RefreshBadgesAuthority()
	{
		for (int i = 0; i < GRUIStationEmployeeBadges.tempRigs.Count; i++)
		{
			NetPlayer netPlayer = GRUIStationEmployeeBadges.tempRigs[i].isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : GRUIStationEmployeeBadges.tempRigs[i].OwningNetPlayer;
			int num;
			if (netPlayer != null && netPlayer.ActorNumber != -1 && !this.dispenserForActorNr.TryGetValue(netPlayer.ActorNumber, out num))
			{
				for (int j = 0; j < this.badgeDispensers.Count; j++)
				{
					if (this.badgeDispensers[j].actorNr == -1)
					{
						this.badgeDispensers[j].CreateBadge(netPlayer, this.reactor.grManager.gameEntityManager);
						break;
					}
				}
			}
		}
		for (int k = this.registeredBadges.Count - 1; k >= 0; k--)
		{
			int num2;
			if (NetworkSystem.Instance.GetNetPlayerByID(this.registeredBadges[k].actorNr) == null || !this.dispenserForActorNr.TryGetValue(this.registeredBadges[k].actorNr, out num2) || num2 != this.registeredBadges[k].dispenserIndex)
			{
				this.reactor.grManager.gameEntityManager.RequestDestroyItem(this.registeredBadges[k].GetComponent<GameEntity>().id);
			}
		}
	}

	// Token: 0x06003588 RID: 13704 RVA: 0x00128724 File Offset: 0x00126924
	public void SliceUpdate()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (!this.reactor.grManager.IsZoneActive())
		{
			return;
		}
		if (this.reactor.grManager.gameEntityManager.IsAuthority())
		{
			this.RefreshBadgesAuthority();
		}
		for (int i = 0; i < this.badgeDispensers.Count; i++)
		{
			this.badgeDispensers[i].Refresh();
		}
	}

	// Token: 0x06003589 RID: 13705 RVA: 0x001287AC File Offset: 0x001269AC
	public void RemoveBadge(GRBadge badge)
	{
		if (this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Remove(badge);
		}
		if (this.badgeDispensers[badge.dispenserIndex].idBadge == badge)
		{
			this.dispenserForActorNr.Remove(badge.actorNr);
			this.badgeDispensers[badge.dispenserIndex].ClearBadge();
		}
	}

	// Token: 0x0600358A RID: 13706 RVA: 0x0012881C File Offset: 0x00126A1C
	public void LinkBadgeToDispenser(GRBadge badge, long createData)
	{
		if (!this.registeredBadges.Contains(badge))
		{
			this.registeredBadges.Add(badge);
		}
		int num = (int)(createData % 100L);
		if (num > this.badgeDispensers.Count)
		{
			return;
		}
		NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID((int)(createData / 100L));
		if (netPlayerByID != null)
		{
			this.dispenserForActorNr[netPlayerByID.ActorNumber] = num;
			this.badgeDispensers[num].AttachIDBadge(badge, netPlayerByID);
		}
	}

	// Token: 0x0600358B RID: 13707 RVA: 0x00128894 File Offset: 0x00126A94
	public GRUIEmployeeBadgeDispenser GetDispenserForPlayer(int actorNumber)
	{
		int index;
		if (!this.dispenserForActorNr.TryGetValue(actorNumber, out index))
		{
			return null;
		}
		return this.badgeDispensers[index];
	}

	// Token: 0x0400461B RID: 17947
	[SerializeField]
	public List<GRUIEmployeeBadgeDispenser> badgeDispensers;

	// Token: 0x0400461C RID: 17948
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400461D RID: 17949
	public Dictionary<int, int> dispenserForActorNr;

	// Token: 0x0400461E RID: 17950
	public List<GRBadge> registeredBadges;

	// Token: 0x0400461F RID: 17951
	private GhostReactor reactor;
}
