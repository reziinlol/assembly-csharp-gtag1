using System;
using UnityEngine;

// Token: 0x02000CD8 RID: 3288
[CreateAssetMenu(menuName = "ScriptableObjects/FXSystemSettings", order = 2)]
public class FXSystemSettings : ScriptableObject
{
	// Token: 0x0600518A RID: 20874 RVA: 0x001AD7D0 File Offset: 0x001AB9D0
	public void Awake()
	{
		int num = (this.callLimits != null) ? this.callLimits.Length : 0;
		int num2 = (this.CallLimitsCooldown != null) ? this.CallLimitsCooldown.Length : 0;
		for (int i = 0; i < num; i++)
		{
			FXType key = this.callLimits[i].Key;
			int num3 = (int)key;
			if (num3 < 0 || num3 >= 25)
			{
				string str = "NO_PATH_AT_RUNTIME";
				Debug.LogError("FXSystemSettings: (this should never happen) `callLimits.Key` is out of bounds of `callSettings`! Path=\"" + str + "\"", this);
			}
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping.");
			}
			else
			{
				this.callSettings[num3] = this.callLimits[i];
			}
		}
		for (int i = 0; i < num2; i++)
		{
			FXType key = this.CallLimitsCooldown[i].Key;
			int num3 = (int)key;
			if (this.callSettings[num3] != null)
			{
				Debug.Log("FXSystemSettings: call setting for " + key.ToString() + " already exists, skipping");
			}
			else
			{
				this.callSettings[num3] = this.CallLimitsCooldown[i];
			}
		}
		for (int i = 0; i < this.callSettings.Length; i++)
		{
			if (this.callSettings[i] == null)
			{
				this.callSettings[i] = new LimiterType
				{
					CallLimitSettings = new CallLimiter(0, 0f, 0f),
					Key = (FXType)i
				};
			}
		}
	}

	// Token: 0x040062DF RID: 25311
	private const string preLog = "FXSystemSettings: ";

	// Token: 0x040062E0 RID: 25312
	private const string preErr = "ERROR!!!  FXSystemSettings: ";

	// Token: 0x040062E1 RID: 25313
	[SerializeField]
	private LimiterType[] callLimits;

	// Token: 0x040062E2 RID: 25314
	[SerializeField]
	private CooldownType[] CallLimitsCooldown;

	// Token: 0x040062E3 RID: 25315
	[NonSerialized]
	public bool forLocalRig;

	// Token: 0x040062E4 RID: 25316
	[NonSerialized]
	public CallLimitType<CallLimiter>[] callSettings = new CallLimitType<CallLimiter>[25];
}
