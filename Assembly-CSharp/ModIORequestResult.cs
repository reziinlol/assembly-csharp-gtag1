using System;
using Modio;

// Token: 0x02000A5A RID: 2650
public struct ModIORequestResult
{
	// Token: 0x060043F8 RID: 17400 RVA: 0x0016BFEC File Offset: 0x0016A1EC
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult result;
		result.success = false;
		result.message = inMessage;
		return result;
	}

	// Token: 0x060043F9 RID: 17401 RVA: 0x0016C00C File Offset: 0x0016A20C
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult result;
		result.success = true;
		result.message = "";
		return result;
	}

	// Token: 0x060043FA RID: 17402 RVA: 0x0016C030 File Offset: 0x0016A230
	public static ModIORequestResult CreateFromError(Error error)
	{
		ModIORequestResult result;
		if (error)
		{
			result.success = false;
			result.message = error.GetMessage();
		}
		else
		{
			result.success = true;
			result.message = "";
		}
		return result;
	}

	// Token: 0x040055D1 RID: 21969
	public bool success;

	// Token: 0x040055D2 RID: 21970
	public string message;
}
