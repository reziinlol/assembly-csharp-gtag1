using System;
using Photon.Pun;

// Token: 0x02000CB4 RID: 3252
[Serializable]
public struct PhotonSignalInfo
{
	// Token: 0x06005090 RID: 20624 RVA: 0x001AA698 File Offset: 0x001A8898
	public PhotonSignalInfo(NetPlayer sender, int timestamp)
	{
		this.sender = sender;
		this.timestamp = timestamp;
	}

	// Token: 0x17000783 RID: 1923
	// (get) Token: 0x06005091 RID: 20625 RVA: 0x001AA6A8 File Offset: 0x001A88A8
	public double sentServerTime
	{
		get
		{
			return this.timestamp / 1000.0;
		}
	}

	// Token: 0x06005092 RID: 20626 RVA: 0x001AA6BC File Offset: 0x001A88BC
	public override string ToString()
	{
		return string.Format("[{0}: Sender = '{1}' sentTime = {2}]", "PhotonSignalInfo", this.sender.ActorNumber, this.sentServerTime);
	}

	// Token: 0x06005093 RID: 20627 RVA: 0x001AA6E8 File Offset: 0x001A88E8
	public static implicit operator PhotonMessageInfo(PhotonSignalInfo psi)
	{
		return new PhotonMessageInfo(psi.sender.GetPlayerRef(), psi.timestamp, null);
	}

	// Token: 0x0400625D RID: 25181
	public readonly int timestamp;

	// Token: 0x0400625E RID: 25182
	public readonly NetPlayer sender;
}
