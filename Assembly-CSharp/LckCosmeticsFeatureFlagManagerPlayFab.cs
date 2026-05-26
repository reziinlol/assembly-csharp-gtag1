using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Liv.Lck;
using UnityEngine.Scripting;

// Token: 0x020003FC RID: 1020
[Preserve]
public class LckCosmeticsFeatureFlagManagerPlayFab : ILckCosmeticsFeatureFlagManager
{
	// Token: 0x06001834 RID: 6196 RVA: 0x00089BF0 File Offset: 0x00087DF0
	[Preserve]
	public LckCosmeticsFeatureFlagManagerPlayFab()
	{
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x00089C04 File Offset: 0x00087E04
	public Task<bool> IsEnabledAsync()
	{
		if (this._initializationTask != null)
		{
			return this._initializationTask;
		}
		object @lock = this._lock;
		Task<bool> task2;
		lock (@lock)
		{
			Task<bool> task;
			if ((task = this._initializationTask) == null)
			{
				task2 = (this._initializationTask = this.GetEnabledStateWithRetryAsync());
				task = task2;
			}
			task2 = task;
		}
		return task2;
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x00089C68 File Offset: 0x00087E68
	private Task<bool> GetEnabledStateWithRetryAsync()
	{
		LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7 <GetEnabledStateWithRetryAsync>d__;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<GetEnabledStateWithRetryAsync>d__.<>1__state = -1;
		<GetEnabledStateWithRetryAsync>d__.<>t__builder.Start<LckCosmeticsFeatureFlagManagerPlayFab.<GetEnabledStateWithRetryAsync>d__7>(ref <GetEnabledStateWithRetryAsync>d__);
		return <GetEnabledStateWithRetryAsync>d__.<>t__builder.Task;
	}

	// Token: 0x04002362 RID: 9058
	private const string TitleDataKey = "EnableLckCosmetics";

	// Token: 0x04002363 RID: 9059
	private const int MaxRetries = 2;

	// Token: 0x04002364 RID: 9060
	private const int RetryDelayMilliseconds = 5000;

	// Token: 0x04002365 RID: 9061
	private Task<bool> _initializationTask;

	// Token: 0x04002366 RID: 9062
	private readonly object _lock = new object();
}
