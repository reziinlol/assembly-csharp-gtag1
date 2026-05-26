using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class Clock3x3 : ObservableBehavior
{
	// Token: 0x060000DF RID: 223 RVA: 0x00005A14 File Offset: 0x00003C14
	protected override void ObservableSliceUpdate()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime now = DateTime.Now;
		if (serverTime.Year < 2000)
		{
			return;
		}
		this.display.text = string.Format(this.formatString, new object[]
		{
			this.headings[0],
			this.headings[1],
			serverTime.ToString("hh:mm:sstt"),
			DateTime.Now.ToString("hh:mm:sstt"),
			this.HexColor(this.color.Evaluate(((float)serverTime.Hour + (float)serverTime.Minute / 60f) / 24f)),
			this.HexColor(this.color.Evaluate(((float)now.Hour + (float)now.Minute / 60f) / 24f))
		});
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00005AFC File Offset: 0x00003CFC
	public string HexColor(Color color)
	{
		return "#" + Mathf.FloorToInt(Mathf.Clamp01(color.r) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.g) * 255f).ToString("X2") + Mathf.FloorToInt(Mathf.Clamp01(color.b) * 255f).ToString("X2");
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00005B7C File Offset: 0x00003D7C
	protected override void OnBecameObservable()
	{
		this.display.gameObject.SetActive(true);
		this.Initialize();
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00005B98 File Offset: 0x00003D98
	private void Initialize()
	{
		Clock3x3.<Initialize>d__9 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<Clock3x3.<Initialize>d__9>(ref <Initialize>d__);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00005BCF File Offset: 0x00003DCF
	private void onTD(string s)
	{
		this.headings = s.Split(";", StringSplitOptions.None);
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00005BE3 File Offset: 0x00003DE3
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("Clock3x3 :: onTDError :: {0} :: {1}", this.titleDataKey, error));
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00005BFB File Offset: 0x00003DFB
	protected override void OnLostObservable()
	{
		this.display.gameObject.SetActive(false);
	}

	// Token: 0x040000EB RID: 235
	[SerializeField]
	private string titleDataKey;

	// Token: 0x040000EC RID: 236
	[SerializeField]
	private TMP_Text display;

	// Token: 0x040000ED RID: 237
	[SerializeField]
	private Gradient color;

	// Token: 0x040000EE RID: 238
	private string formatString;

	// Token: 0x040000EF RID: 239
	private bool initialized;

	// Token: 0x040000F0 RID: 240
	[SerializeField]
	private string[] headings;
}
