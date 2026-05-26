using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000BC0 RID: 3008
internal abstract class WarningsServer : MonoBehaviour
{
	// Token: 0x06004B58 RID: 19288
	public abstract Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token);

	// Token: 0x06004B59 RID: 19289
	public abstract Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token);

	// Token: 0x04005E70 RID: 24176
	public static volatile WarningsServer Instance;
}
