using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using PlayFab;
using UnityEngine;

// Token: 0x02000CC0 RID: 3264
public class PUNErrorLogging : MonoBehaviour
{
	// Token: 0x06005138 RID: 20792 RVA: 0x001ACBA8 File Offset: 0x001AADA8
	private void Start()
	{
		PhotonNetwork.InternalEventError = (Action<EventData, Exception>)Delegate.Combine(PhotonNetwork.InternalEventError, new Action<EventData, Exception>(this.PUNError));
		PlayFabTitleDataCache.Instance.GetTitleData("PUNErrorLogging", delegate(string data)
		{
			int num;
			if (!int.TryParse(data, out num))
			{
				return;
			}
			PUNErrorLogging.LogFlags logFlags = (PUNErrorLogging.LogFlags)num;
			this.m_logSerializeView = logFlags.HasFlag(PUNErrorLogging.LogFlags.SerializeView);
			this.m_logOwnershipTransfer = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipTransfer);
			this.m_logOwnershipRequest = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipRequest);
			this.m_logOwnershipUpdate = logFlags.HasFlag(PUNErrorLogging.LogFlags.OwnershipUpdate);
			this.m_logRPC = logFlags.HasFlag(PUNErrorLogging.LogFlags.RPC);
			this.m_logInstantiate = logFlags.HasFlag(PUNErrorLogging.LogFlags.Instantiate);
			this.m_logDestroy = logFlags.HasFlag(PUNErrorLogging.LogFlags.Destroy);
			this.m_logDestroyPlayer = logFlags.HasFlag(PUNErrorLogging.LogFlags.DestroyPlayer);
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x06005139 RID: 20793 RVA: 0x001ACC10 File Offset: 0x001AAE10
	private void PUNError(EventData data, Exception exception)
	{
		NetworkSystem.Instance.GetPlayer(data.Sender);
		byte code = data.Code;
		switch (code)
		{
		case 200:
			this.PrintException(exception, this.m_logRPC);
			return;
		case 201:
		case 206:
			this.PrintException(exception, this.m_logSerializeView);
			return;
		case 202:
			this.PrintException(exception, this.m_logInstantiate);
			return;
		case 203:
		case 205:
		case 208:
		case 211:
			break;
		case 204:
			this.PrintException(exception, this.m_logDestroy);
			return;
		case 207:
			this.PrintException(exception, this.m_logDestroyPlayer);
			return;
		case 209:
			this.PrintException(exception, this.m_logOwnershipRequest);
			return;
		case 210:
			this.PrintException(exception, this.m_logOwnershipTransfer);
			return;
		case 212:
			this.PrintException(exception, this.m_logOwnershipUpdate);
			return;
		default:
			if (code == 254)
			{
				this.PrintException(exception, true);
				return;
			}
			break;
		}
		this.PrintException(exception, true);
	}

	// Token: 0x0600513A RID: 20794 RVA: 0x001ACCFE File Offset: 0x001AAEFE
	private void PrintException(Exception e, bool print)
	{
		if (print)
		{
			Debug.LogException(e);
		}
	}

	// Token: 0x0400628C RID: 25228
	[SerializeField]
	private bool m_logSerializeView = true;

	// Token: 0x0400628D RID: 25229
	[SerializeField]
	private bool m_logOwnershipTransfer = true;

	// Token: 0x0400628E RID: 25230
	[SerializeField]
	private bool m_logOwnershipRequest = true;

	// Token: 0x0400628F RID: 25231
	[SerializeField]
	private bool m_logOwnershipUpdate = true;

	// Token: 0x04006290 RID: 25232
	[SerializeField]
	private bool m_logRPC = true;

	// Token: 0x04006291 RID: 25233
	[SerializeField]
	private bool m_logInstantiate = true;

	// Token: 0x04006292 RID: 25234
	[SerializeField]
	private bool m_logDestroy = true;

	// Token: 0x04006293 RID: 25235
	[SerializeField]
	private bool m_logDestroyPlayer = true;

	// Token: 0x02000CC1 RID: 3265
	[Flags]
	private enum LogFlags
	{
		// Token: 0x04006295 RID: 25237
		SerializeView = 1,
		// Token: 0x04006296 RID: 25238
		OwnershipTransfer = 2,
		// Token: 0x04006297 RID: 25239
		OwnershipRequest = 4,
		// Token: 0x04006298 RID: 25240
		OwnershipUpdate = 8,
		// Token: 0x04006299 RID: 25241
		RPC = 16,
		// Token: 0x0400629A RID: 25242
		Instantiate = 32,
		// Token: 0x0400629B RID: 25243
		Destroy = 64,
		// Token: 0x0400629C RID: 25244
		DestroyPlayer = 128
	}
}
