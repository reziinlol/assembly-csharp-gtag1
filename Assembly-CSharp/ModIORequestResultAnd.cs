using System;

// Token: 0x02000A5B RID: 2651
public struct ModIORequestResultAnd<T>
{
	// Token: 0x060043FB RID: 17403 RVA: 0x0016C074 File Offset: 0x0016A274
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x060043FC RID: 17404 RVA: 0x0016C098 File Offset: 0x0016A298
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x040055D3 RID: 21971
	public ModIORequestResult result;

	// Token: 0x040055D4 RID: 21972
	public T data;
}
