using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000C92 RID: 3218
public class SteamAuthTicket : IDisposable
{
	// Token: 0x06004FC7 RID: 20423 RVA: 0x001A636C File Offset: 0x001A456C
	private SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		this.m_hAuthTicket = hAuthTicket;
	}

	// Token: 0x06004FC8 RID: 20424 RVA: 0x001A637B File Offset: 0x001A457B
	public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		return new SteamAuthTicket(hAuthTicket);
	}

	// Token: 0x06004FC9 RID: 20425 RVA: 0x001A6384 File Offset: 0x001A4584
	~SteamAuthTicket()
	{
		this.Dispose();
	}

	// Token: 0x06004FCA RID: 20426 RVA: 0x001A63B0 File Offset: 0x001A45B0
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (this.m_hAuthTicket != HAuthTicket.Invalid)
		{
			try
			{
				SteamUser.CancelAuthTicket(this.m_hAuthTicket);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
			}
			this.m_hAuthTicket = HAuthTicket.Invalid;
		}
	}

	// Token: 0x04006191 RID: 24977
	private HAuthTicket m_hAuthTicket;
}
