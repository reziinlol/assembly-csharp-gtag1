using System;

// Token: 0x02000CD7 RID: 3287
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06005188 RID: 20872 RVA: 0x001AD79F File Offset: 0x001AB99F
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x040062DC RID: 25308
	public FXType Key;

	// Token: 0x040062DD RID: 25309
	public bool UseNetWorkTime;

	// Token: 0x040062DE RID: 25310
	public T CallLimitSettings;
}
