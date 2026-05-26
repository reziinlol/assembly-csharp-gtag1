using System;
using Photon.Realtime;

// Token: 0x02000466 RID: 1126
public class NetEventOptions
{
	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06001B54 RID: 6996 RVA: 0x00094ACE File Offset: 0x00092CCE
	public bool HasWebHooks
	{
		get
		{
			return this.Flags != WebFlags.Default;
		}
	}

	// Token: 0x06001B55 RID: 6997 RVA: 0x00094AE0 File Offset: 0x00092CE0
	public NetEventOptions()
	{
	}

	// Token: 0x06001B56 RID: 6998 RVA: 0x00094AF3 File Offset: 0x00092CF3
	public NetEventOptions(int reciever, int[] actors, byte flags)
	{
		this.Reciever = (NetEventOptions.RecieverTarget)reciever;
		this.TargetActors = actors;
		this.Flags = new WebFlags(flags);
	}

	// Token: 0x04002584 RID: 9604
	public NetEventOptions.RecieverTarget Reciever;

	// Token: 0x04002585 RID: 9605
	public int[] TargetActors;

	// Token: 0x04002586 RID: 9606
	public WebFlags Flags = WebFlags.Default;

	// Token: 0x02000467 RID: 1127
	public enum RecieverTarget
	{
		// Token: 0x04002588 RID: 9608
		others,
		// Token: 0x04002589 RID: 9609
		all,
		// Token: 0x0400258A RID: 9610
		master
	}
}
