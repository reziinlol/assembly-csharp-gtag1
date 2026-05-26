using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000C8B RID: 3211
internal class OwnershipGaurd : MonoBehaviour
{
	// Token: 0x06004FB4 RID: 20404 RVA: 0x001A6153 File Offset: 0x001A4353
	private void Start()
	{
		if (this.autoRegisterAll)
		{
			this.NetViews = base.GetComponents<PhotonView>();
		}
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGuardHandler.RegisterViews(this.NetViews);
	}

	// Token: 0x06004FB5 RID: 20405 RVA: 0x001A617D File Offset: 0x001A437D
	private void OnDestroy()
	{
		if (this.NetViews == null)
		{
			return;
		}
		OwnershipGuardHandler.RemoveViews(this.NetViews);
	}

	// Token: 0x04006170 RID: 24944
	[SerializeField]
	private PhotonView[] NetViews;

	// Token: 0x04006171 RID: 24945
	[SerializeField]
	private bool autoRegisterAll = true;
}
