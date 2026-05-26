using System;
using UnityEngine;

// Token: 0x02000CCC RID: 3276
[Serializable]
public class CallLimiterWithCooldown : CallLimiter
{
	// Token: 0x06005166 RID: 20838 RVA: 0x001AD4B9 File Offset: 0x001AB6B9
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown) : base(historyLength, coolDown, 0.5f)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06005167 RID: 20839 RVA: 0x001AD4CF File Offset: 0x001AB6CF
	public CallLimiterWithCooldown(float coolDownSpam, int historyLength, float coolDown, float latencyMax) : base(historyLength, coolDown, latencyMax)
	{
		this.spamCoolDown = coolDownSpam;
	}

	// Token: 0x06005168 RID: 20840 RVA: 0x001AD4E2 File Offset: 0x001AB6E2
	public override bool CheckCallTime(float time)
	{
		if (this.blockCall && time < this.blockStartTime + this.spamCoolDown)
		{
			this.blockStartTime = time;
			return false;
		}
		return base.CheckCallTime(time);
	}

	// Token: 0x040062BF RID: 25279
	[SerializeField]
	private float spamCoolDown;
}
