using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// Token: 0x020009B6 RID: 2486
public class ProgressionUtil
{
	// Token: 0x06003FAE RID: 16302 RVA: 0x001548B4 File Offset: 0x00152AB4
	public static Task WaitForMothershipSessionToken()
	{
		ProgressionUtil.<WaitForMothershipSessionToken>d__0 <WaitForMothershipSessionToken>d__;
		<WaitForMothershipSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForMothershipSessionToken>d__.<>1__state = -1;
		<WaitForMothershipSessionToken>d__.<>t__builder.Start<ProgressionUtil.<WaitForMothershipSessionToken>d__0>(ref <WaitForMothershipSessionToken>d__);
		return <WaitForMothershipSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x06003FAF RID: 16303 RVA: 0x001548F0 File Offset: 0x00152AF0
	public static Task WaitForPlayFabSessionTicket()
	{
		ProgressionUtil.<WaitForPlayFabSessionTicket>d__1 <WaitForPlayFabSessionTicket>d__;
		<WaitForPlayFabSessionTicket>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForPlayFabSessionTicket>d__.<>1__state = -1;
		<WaitForPlayFabSessionTicket>d__.<>t__builder.Start<ProgressionUtil.<WaitForPlayFabSessionTicket>d__1>(ref <WaitForPlayFabSessionTicket>d__);
		return <WaitForPlayFabSessionTicket>d__.<>t__builder.Task;
	}
}
