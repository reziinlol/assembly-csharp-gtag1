using System;
using System.Linq;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020005B4 RID: 1460
public sealed class FlickerManager : MonoBehaviour
{
	// Token: 0x060024C5 RID: 9413 RVA: 0x000C4ED8 File Offset: 0x000C30D8
	private void Awake()
	{
		if (this.FlickerDurations.Length % 2 != 0)
		{
			Debug.LogWarning("FlickerManager should have an even number of steps; removing last entry.");
			this.FlickerDurations = this.FlickerDurations.Take(this.FlickerDurations.Length - 1).ToArray<float>();
		}
		if (this.FlickerDurations.Length == 0)
		{
			Debug.LogWarning("No flicker durations set for FlickerManager, disabling.");
			Object.Destroy(this);
			return;
		}
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000C4F38 File Offset: 0x000C3138
	private void Update()
	{
		float serverTime = FlickerManager.GetServerTime();
		if (serverTime < this._nextFlickerTime)
		{
			return;
		}
		BetterDayNightManager.instance.AnimateLightFlash(this.LightmapIndex, this.FlickerFadeInDuration, this.FlickerDurations[this._flickerIndex], this.FlickerFadeOutDuration);
		this._nextFlickerTime = serverTime + this.FlickerDurations[this._flickerIndex + 1];
		this._flickerIndex = (this._flickerIndex + 2) % this.FlickerDurations.Length;
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000C4FB0 File Offset: 0x000C31B0
	private static float GetServerTime()
	{
		return (float)(GorillaComputer.instance.GetServerTime() - GorillaComputer.instance.startupTime).TotalSeconds;
	}

	// Token: 0x04003035 RID: 12341
	public float[] FlickerDurations;

	// Token: 0x04003036 RID: 12342
	public float FlickerFadeInDuration;

	// Token: 0x04003037 RID: 12343
	public float FlickerFadeOutDuration;

	// Token: 0x04003038 RID: 12344
	public int LightmapIndex;

	// Token: 0x04003039 RID: 12345
	private int _flickerIndex;

	// Token: 0x0400303A RID: 12346
	private float _nextFlickerTime = float.MinValue;
}
