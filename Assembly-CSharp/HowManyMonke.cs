using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class HowManyMonke : MonoBehaviour
{
	// Token: 0x17000247 RID: 583
	// (get) Token: 0x06001778 RID: 6008 RVA: 0x00086AC3 File Offset: 0x00084CC3
	public static float RecheckDelay
	{
		get
		{
			return Mathf.Max((float)HowManyMonke.recheckDelay / 1000f, 1f);
		}
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x00086ADC File Offset: 0x00084CDC
	public void Start()
	{
		HowManyMonke.<Start>d__11 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<HowManyMonke.<Start>d__11>(ref <Start>d__);
	}

	// Token: 0x0600177A RID: 6010 RVA: 0x00086B14 File Offset: 0x00084D14
	private Task FetchRecheckDelay()
	{
		HowManyMonke.<FetchRecheckDelay>d__12 <FetchRecheckDelay>d__;
		<FetchRecheckDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<FetchRecheckDelay>d__.<>4__this = this;
		<FetchRecheckDelay>d__.<>1__state = -1;
		<FetchRecheckDelay>d__.<>t__builder.Start<HowManyMonke.<FetchRecheckDelay>d__12>(ref <FetchRecheckDelay>d__);
		return <FetchRecheckDelay>d__.<>t__builder.Task;
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x00086B57 File Offset: 0x00084D57
	private void onTDError(PlayFabError error)
	{
		this.state = HowManyMonke.State.READY;
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x00086B66 File Offset: 0x00084D66
	private void onTD(string obj)
	{
		this.state = HowManyMonke.State.READY;
		if (int.TryParse(obj, out HowManyMonke.recheckDelay))
		{
			HowManyMonke.recheckDelay *= 1000;
			return;
		}
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x00086B94 File Offset: 0x00084D94
	private Task<int> FetchThisMany()
	{
		HowManyMonke.<FetchThisMany>d__15 <FetchThisMany>d__;
		<FetchThisMany>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<FetchThisMany>d__.<>4__this = this;
		<FetchThisMany>d__.<>1__state = -1;
		<FetchThisMany>d__.<>t__builder.Start<HowManyMonke.<FetchThisMany>d__15>(ref <FetchThisMany>d__);
		return <FetchThisMany>d__.<>t__builder.Task;
	}

	// Token: 0x040022A9 RID: 8873
	private const string preLog = "[GT/HowManyMonke]  ";

	// Token: 0x040022AA RID: 8874
	private const string preErr = "ERROR!!!  ";

	// Token: 0x040022AB RID: 8875
	public static int ThisMany = 12549;

	// Token: 0x040022AC RID: 8876
	public static Action<int> OnCheck;

	// Token: 0x040022AD RID: 8877
	[SerializeField]
	private string titleDataKey;

	// Token: 0x040022AE RID: 8878
	private HowManyMonke.State state;

	// Token: 0x040022AF RID: 8879
	private static int recheckDelay;

	// Token: 0x040022B0 RID: 8880
	[SerializeField]
	private string CCUEndpoint;

	// Token: 0x020003D9 RID: 985
	private enum State
	{
		// Token: 0x040022B2 RID: 8882
		READY,
		// Token: 0x040022B3 RID: 8883
		TD_LOOKUP,
		// Token: 0x040022B4 RID: 8884
		HMM_LOOKUP
	}

	// Token: 0x020003DA RID: 986
	private class CCUResponse
	{
		// Token: 0x040022B5 RID: 8885
		public int CCUTotal;

		// Token: 0x040022B6 RID: 8886
		public string ErrorMessage;
	}
}
