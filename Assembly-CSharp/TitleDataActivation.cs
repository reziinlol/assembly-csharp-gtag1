using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

// Token: 0x02000DE4 RID: 3556
public class TitleDataActivation : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06005715 RID: 22293 RVA: 0x001C2ECC File Offset: 0x001C10CC
	[RuntimeInitializeOnLoadMethod]
	private static void RuntimeInit()
	{
		TitleDataActivation.<RuntimeInit>d__2 <RuntimeInit>d__;
		<RuntimeInit>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RuntimeInit>d__.<>1__state = -1;
		<RuntimeInit>d__.<>t__builder.Start<TitleDataActivation.<RuntimeInit>d__2>(ref <RuntimeInit>d__);
	}

	// Token: 0x06005716 RID: 22294 RVA: 0x001C2EFB File Offset: 0x001C10FB
	private static void onTDReferenceDate(string s)
	{
		if (!DateTime.TryParse(s, out TitleDataActivation.ReferenceDate))
		{
			Debug.LogError("TitleDataActivation :: onTDReferenceDate :: No Reference Date Set!!");
			return;
		}
		TitleDataActivation.UpdatedReferenceDateFromTitleData = true;
	}

	// Token: 0x06005717 RID: 22295 RVA: 0x001C2F1B File Offset: 0x001C111B
	private static void onTDReferenceDateError(PlayFabError error)
	{
		Debug.LogError("TitleDataActivation :: onTDReferenceDateError :: No Reference Date Set!! :: " + error.ErrorMessage);
	}

	// Token: 0x06005718 RID: 22296 RVA: 0x001C2F34 File Offset: 0x001C1134
	private void Initialize()
	{
		TitleDataActivation.<Initialize>d__16 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<TitleDataActivation.<Initialize>d__16>(ref <Initialize>d__);
	}

	// Token: 0x06005719 RID: 22297 RVA: 0x001C2F6C File Offset: 0x001C116C
	private void onTD(string s)
	{
		TitleDataActivation.TitleDataActivationData titleDataActivationData = null;
		try
		{
			titleDataActivationData = JsonConvert.DeserializeObject<TitleDataActivation.TitleDataActivationData>(s);
		}
		catch (Exception ex)
		{
			Debug.LogError("TitleDataActivation :: onTD ::" + ex.Message + " string was " + s);
			return;
		}
		for (int i = 0; i < titleDataActivationData.Data.Length; i++)
		{
			if (titleDataActivationData.Data[i].TitleDataObjectID == this.titleDataObjectID)
			{
				this.activationData = titleDataActivationData.Data[i];
				return;
			}
		}
	}

	// Token: 0x0600571A RID: 22298 RVA: 0x001C2FF0 File Offset: 0x001C11F0
	private void onTDError(PlayFabError error)
	{
		Debug.LogError(string.Format("TitleDataActivation on {0} :: onTDError :: {1} :: {2}", AssetUtils.GetGameObjectPath(base.gameObject), this.titleDataKey, error));
	}

	// Token: 0x0600571B RID: 22299 RVA: 0x001C3013 File Offset: 0x001C1213
	private void OnEnable()
	{
		this.Initialize();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600571C RID: 22300 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600571D RID: 22301 RVA: 0x001C3024 File Offset: 0x001C1224
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.activationData == null)
		{
			return;
		}
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return;
		}
		bool flag = false;
		float num = 0f;
		int num2 = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num2 < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num2].IsInWindow(serverTime, out flag, out num);
			num2++;
		}
		int num3 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num3 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num3].IsInWindow(serverTime, out flag, out num);
			num3++;
		}
		if (flag != this.onOffState)
		{
			this.SetState(flag, num);
			this.onOffState = flag;
		}
	}

	// Token: 0x0600571E RID: 22302 RVA: 0x001C30FC File Offset: 0x001C12FC
	private void SetState(bool onOff, float delayedActivation)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(onOff);
			if (onOff && delayedActivation > 0f)
			{
				Animator[] componentsInChildren = this.gameObjects[i].GetComponentsInChildren<Animator>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					int fullPathHash = componentsInChildren[j].GetCurrentAnimatorStateInfo(0).fullPathHash;
					componentsInChildren[j].PlayInFixedTime(fullPathHash, 0, delayedActivation);
				}
			}
		}
	}

	// Token: 0x0600571F RID: 22303 RVA: 0x001C3170 File Offset: 0x001C1370
	public float GetDelayedActivationTime()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (serverTime.Year < 2000)
		{
			return 0f;
		}
		bool flag = false;
		float b = 0f;
		int num = 0;
		while (this.activationData.AbsoluteDateTimeWindow != null && num < this.activationData.AbsoluteDateTimeWindow.Length && !flag)
		{
			this.activationData.AbsoluteDateTimeWindow[num].IsInWindow(serverTime, out flag, out b);
			num++;
		}
		int num2 = 0;
		while (this.activationData.RelativeDateTimeWindow != null && num2 < this.activationData.RelativeDateTimeWindow.Length && !flag)
		{
			this.activationData.RelativeDateTimeWindow[num2].IsInWindow(serverTime, out flag, out b);
			num2++;
		}
		return Mathf.Max(0f, b);
	}

	// Token: 0x06005720 RID: 22304 RVA: 0x001C3234 File Offset: 0x001C1434
	public void PlayAnimatorAtScheduledTime(Animator animator)
	{
		float delayedActivationTime = this.GetDelayedActivationTime();
		int fullPathHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		animator.PlayInFixedTime(fullPathHash, 0, this.GetDelayedActivationTime());
		AudioSource[] componentsInChildren = animator.GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].playOnAwake && componentsInChildren[i].clip != null && componentsInChildren[i].clip.length > delayedActivationTime)
			{
				componentsInChildren[i].time = delayedActivationTime;
			}
		}
	}

	// Token: 0x04006707 RID: 26375
	public static DateTime ReferenceDate = DateTime.Parse("1/1/2001");

	// Token: 0x04006708 RID: 26376
	public static bool UpdatedReferenceDateFromTitleData = false;

	// Token: 0x04006709 RID: 26377
	[SerializeField]
	private string titleDataKey;

	// Token: 0x0400670A RID: 26378
	[SerializeField]
	private string titleDataObjectID;

	// Token: 0x0400670B RID: 26379
	private TitleDataActivation.TitleDataObjectActivationData activationData;

	// Token: 0x0400670C RID: 26380
	private GameObject[] gameObjects;

	// Token: 0x0400670D RID: 26381
	private bool initialized;

	// Token: 0x0400670E RID: 26382
	private bool onOffState;

	// Token: 0x02000DE5 RID: 3557
	[Serializable]
	public class TitleDataActivationData
	{
		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06005723 RID: 22307 RVA: 0x001C32CD File Offset: 0x001C14CD
		// (set) Token: 0x06005724 RID: 22308 RVA: 0x001C32D5 File Offset: 0x001C14D5
		public TitleDataActivation.TitleDataObjectActivationData[] Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		// Token: 0x0400670F RID: 26383
		[SerializeField]
		private TitleDataActivation.TitleDataObjectActivationData[] data;

		// Token: 0x04006710 RID: 26384
		private bool validated;
	}

	// Token: 0x02000DE6 RID: 3558
	[Serializable]
	public class TitleDataObjectActivationData
	{
		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06005726 RID: 22310 RVA: 0x001C32DE File Offset: 0x001C14DE
		// (set) Token: 0x06005727 RID: 22311 RVA: 0x001C32E6 File Offset: 0x001C14E6
		public string TitleDataObjectID
		{
			get
			{
				return this.titleDataObjectID;
			}
			set
			{
				this.titleDataObjectID = value;
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06005728 RID: 22312 RVA: 0x001C32EF File Offset: 0x001C14EF
		// (set) Token: 0x06005729 RID: 22313 RVA: 0x001C32F7 File Offset: 0x001C14F7
		public TitleDataActivation.AbsoluteDateTimeWindow[] AbsoluteDateTimeWindow
		{
			get
			{
				return this.absoluteDateTimeWindow;
			}
			set
			{
				this.absoluteDateTimeWindow = value;
			}
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600572A RID: 22314 RVA: 0x001C3300 File Offset: 0x001C1500
		// (set) Token: 0x0600572B RID: 22315 RVA: 0x001C3308 File Offset: 0x001C1508
		public TitleDataActivation.RelativeDateTimeWindow[] RelativeDateTimeWindow
		{
			get
			{
				return this.relativeDateTimeWindow;
			}
			set
			{
				this.relativeDateTimeWindow = value;
			}
		}

		// Token: 0x04006711 RID: 26385
		[SerializeField]
		private string titleDataObjectID;

		// Token: 0x04006712 RID: 26386
		[SerializeField]
		private TitleDataActivation.AbsoluteDateTimeWindow[] absoluteDateTimeWindow;

		// Token: 0x04006713 RID: 26387
		[SerializeField]
		private TitleDataActivation.RelativeDateTimeWindow[] relativeDateTimeWindow;

		// Token: 0x04006714 RID: 26388
		private bool validated;
	}

	// Token: 0x02000DE7 RID: 3559
	[Serializable]
	public class AbsoluteDateTimeWindow
	{
		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x0600572D RID: 22317 RVA: 0x001C3311 File Offset: 0x001C1511
		// (set) Token: 0x0600572E RID: 22318 RVA: 0x001C3319 File Offset: 0x001C1519
		public string StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtStart))
				{
					this.startDateTime = this.dtStart.ToString();
				}
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x0600572F RID: 22319 RVA: 0x001C333A File Offset: 0x001C153A
		// (set) Token: 0x06005730 RID: 22320 RVA: 0x001C3342 File Offset: 0x001C1542
		public string EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				if (DateTime.TryParse(value, out this.dtEnd))
				{
					this.endDateTime = this.dtEnd.ToString();
				}
			}
		}

		// Token: 0x06005731 RID: 22321 RVA: 0x001C3364 File Offset: 0x001C1564
		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = (d >= this.dtStart && d <= this.dtEnd);
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		// Token: 0x04006715 RID: 26389
		protected DateTime dtStart;

		// Token: 0x04006716 RID: 26390
		protected DateTime dtEnd;

		// Token: 0x04006717 RID: 26391
		[SerializeField]
		private string startDateTime;

		// Token: 0x04006718 RID: 26392
		[SerializeField]
		private string endDateTime;
	}

	// Token: 0x02000DE8 RID: 3560
	[Serializable]
	public class RelativeDateTimeWindow
	{
		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06005733 RID: 22323 RVA: 0x001C33A7 File Offset: 0x001C15A7
		// (set) Token: 0x06005734 RID: 22324 RVA: 0x001C33B0 File Offset: 0x001C15B0
		public TitleDataActivation.RelativeDateTime StartDateTime
		{
			get
			{
				return this.startDateTime;
			}
			set
			{
				this.startDateTime = value;
				this.dtStart = TitleDataActivation.ReferenceDate.AddDays((double)this.startDateTime.DaysPast).AddHours((double)this.startDateTime.Hours).AddMinutes((double)this.startDateTime.Minutes).AddSeconds((double)this.startDateTime.Seconds);
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06005735 RID: 22325 RVA: 0x001C341C File Offset: 0x001C161C
		// (set) Token: 0x06005736 RID: 22326 RVA: 0x001C3424 File Offset: 0x001C1624
		public TitleDataActivation.RelativeDateTime EndDateTime
		{
			get
			{
				return this.endDateTime;
			}
			set
			{
				this.endDateTime = value;
				this.dtEnd = TitleDataActivation.ReferenceDate.AddDays((double)this.endDateTime.DaysPast).AddHours((double)this.endDateTime.Hours).AddMinutes((double)this.endDateTime.Minutes).AddSeconds((double)this.endDateTime.Seconds);
			}
		}

		// Token: 0x06005737 RID: 22327 RVA: 0x001C3490 File Offset: 0x001C1690
		public void IsInWindow(DateTime d, out bool inRange, out float delay)
		{
			inRange = (d >= this.dtStart && d <= this.dtEnd);
			delay = (float)(d - this.dtStart).TotalSeconds;
		}

		// Token: 0x04006719 RID: 26393
		protected DateTime dtStart;

		// Token: 0x0400671A RID: 26394
		protected DateTime dtEnd;

		// Token: 0x0400671B RID: 26395
		[SerializeField]
		private TitleDataActivation.RelativeDateTime startDateTime;

		// Token: 0x0400671C RID: 26396
		[SerializeField]
		private TitleDataActivation.RelativeDateTime endDateTime;
	}

	// Token: 0x02000DE9 RID: 3561
	[Serializable]
	public struct RelativeDateTime
	{
		// Token: 0x0400671D RID: 26397
		public int DaysPast;

		// Token: 0x0400671E RID: 26398
		public int Hours;

		// Token: 0x0400671F RID: 26399
		public int Minutes;

		// Token: 0x04006720 RID: 26400
		public int Seconds;
	}
}
