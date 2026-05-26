using System;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x0200041B RID: 1051
[RequireComponent(typeof(PhotonView))]
public class MaterialCyclerNetworked : MonoBehaviour
{
	// Token: 0x17000276 RID: 630
	// (get) Token: 0x060018F2 RID: 6386 RVA: 0x0008D2AB File Offset: 0x0008B4AB
	public float SyncTimeOut
	{
		get
		{
			return this.syncTimeOut;
		}
	}

	// Token: 0x14000038 RID: 56
	// (add) Token: 0x060018F3 RID: 6387 RVA: 0x0008D2B4 File Offset: 0x0008B4B4
	// (remove) Token: 0x060018F4 RID: 6388 RVA: 0x0008D2EC File Offset: 0x0008B4EC
	public event Action<int, int3> OnSynchronize;

	// Token: 0x060018F5 RID: 6389 RVA: 0x0008D321 File Offset: 0x0008B521
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060018F6 RID: 6390 RVA: 0x0008D330 File Offset: 0x0008B530
	public void Synchronize(int materialIndex, Color c)
	{
		if (!this.masterClientOnly || PhotonNetwork.IsMasterClient)
		{
			int num = Mathf.CeilToInt(c.r * 9f);
			int num2 = Mathf.CeilToInt(c.g * 9f);
			int num3 = Mathf.CeilToInt(c.b * 9f);
			int num4 = num | num2 << 8 | num3 << 16;
			this.photonView.RPC("RPC_SynchronizePacked", RpcTarget.Others, new object[]
			{
				materialIndex,
				num4
			});
		}
	}

	// Token: 0x060018F7 RID: 6391 RVA: 0x0008D3B4 File Offset: 0x0008B5B4
	[PunRPC]
	public void RPC_SynchronizePacked(int index, int colourPacked, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RPC_SynchronizePacked");
		RigContainer rigContainer;
		if (this.OnSynchronize == null || (this.masterClientOnly && !info.Sender.IsMasterClient) || !VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.IsPositionInRange(base.transform.position, 5f) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 21, info.SentServerTime))
		{
			return;
		}
		int num = colourPacked & 255;
		int num2 = colourPacked >> 8 & 255;
		int num3 = colourPacked >> 16 & 255;
		num = Mathf.Clamp(num, 0, 9);
		num2 = Mathf.Clamp(num2, 0, 9);
		num3 = Mathf.Clamp(num3, 0, 9);
		this.OnSynchronize(index, new int3(num, num2, num3));
	}

	// Token: 0x04002414 RID: 9236
	[SerializeField]
	private float syncTimeOut = 1f;

	// Token: 0x04002415 RID: 9237
	private PhotonView photonView;

	// Token: 0x04002416 RID: 9238
	[SerializeField]
	private bool masterClientOnly;
}
