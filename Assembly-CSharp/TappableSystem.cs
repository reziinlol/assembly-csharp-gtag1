using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000915 RID: 2325
public class TappableSystem : GTSystem<Tappable>
{
	// Token: 0x06003CD8 RID: 15576 RVA: 0x0014B1C8 File Offset: 0x001493C8
	[PunRPC]
	public void SendOnTapRPC(int key, float tapStrength, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "SendOnTapRPC");
		if (key < 0 || key >= this._instances.Count || !float.IsFinite(tapStrength))
		{
			return;
		}
		tapStrength = Mathf.Clamp(tapStrength, 0f, 1f);
		this._instances[key].OnTapLocal(tapStrength, Time.time, new PhotonMessageInfoWrapped(info));
	}
}
