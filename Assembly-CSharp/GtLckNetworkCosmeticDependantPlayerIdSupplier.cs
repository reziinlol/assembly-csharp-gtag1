using System;
using Liv.Lck.Cosmetics;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class GtLckNetworkCosmeticDependantPlayerIdSupplier : MonoBehaviour, ILckCosmeticDependantPlayerIdSupplier
{
	// Token: 0x14000032 RID: 50
	// (add) Token: 0x060017A0 RID: 6048 RVA: 0x000879A4 File Offset: 0x00085BA4
	// (remove) Token: 0x060017A1 RID: 6049 RVA: 0x000879DC File Offset: 0x00085BDC
	public event PlayerIdUpdatedEvent PlayerIdUpdated;

	// Token: 0x060017A2 RID: 6050 RVA: 0x00087A11 File Offset: 0x00085C11
	public string GetPlayerId()
	{
		return this.vrrig.OwningNetPlayer.UserId;
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x00087A23 File Offset: 0x00085C23
	public void UpdatePlayerId()
	{
		PlayerIdUpdatedEvent playerIdUpdated = this.PlayerIdUpdated;
		if (playerIdUpdated == null)
		{
			return;
		}
		playerIdUpdated();
	}

	// Token: 0x040022DD RID: 8925
	[SerializeField]
	private VRRig vrrig;
}
