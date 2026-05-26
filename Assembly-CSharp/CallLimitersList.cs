using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000CCD RID: 3277
[Serializable]
public class CallLimitersList<Titem, Tenum> where Titem : CallLimiter, new() where Tenum : Enum
{
	// Token: 0x06005169 RID: 20841 RVA: 0x001AD50C File Offset: 0x001AB70C
	public bool IsSpamming(Tenum index)
	{
		return this.IsSpamming((int)((object)index));
	}

	// Token: 0x0600516A RID: 20842 RVA: 0x001AD51F File Offset: 0x001AB71F
	public bool IsSpamming(int index)
	{
		return !this.m_callLimiters[index].CheckCallTime(Time.unscaledTime);
	}

	// Token: 0x0600516B RID: 20843 RVA: 0x001AD53F File Offset: 0x001AB73F
	public bool IsSpamming(Tenum index, double serverTime)
	{
		return this.IsSpamming((int)((object)index), serverTime);
	}

	// Token: 0x0600516C RID: 20844 RVA: 0x001AD553 File Offset: 0x001AB753
	public bool IsSpamming(int index, double serverTime)
	{
		return !this.m_callLimiters[index].CheckCallServerTime(serverTime);
	}

	// Token: 0x0600516D RID: 20845 RVA: 0x001AD570 File Offset: 0x001AB770
	public void Reset()
	{
		Titem[] callLimiters = this.m_callLimiters;
		for (int i = 0; i < callLimiters.Length; i++)
		{
			callLimiters[i].Reset();
		}
	}

	// Token: 0x040062C0 RID: 25280
	[RequiredListLength("GetMaxLength")]
	[SerializeField]
	private Titem[] m_callLimiters;
}
