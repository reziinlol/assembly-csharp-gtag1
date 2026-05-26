using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using KID.Model;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x020001AB RID: 427
public class VODPlayer : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000B93 RID: 2963 RVA: 0x0003E376 File Offset: 0x0003C576
	public static Material StandbyMaterial
	{
		get
		{
			return VODPlayer._standbyMaterial;
		}
	}

	// Token: 0x06000B94 RID: 2964 RVA: 0x0003E37D File Offset: 0x0003C57D
	private void Awake()
	{
		VODPlayer._standbyMaterial = this.standbyMaterial;
	}

	// Token: 0x06000B95 RID: 2965 RVA: 0x0003E38C File Offset: 0x0003C58C
	public void OnEnable()
	{
		VODPlayer.<OnEnable>d__24 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<VODPlayer.<OnEnable>d__24>(ref <OnEnable>d__);
	}

	// Token: 0x06000B96 RID: 2966 RVA: 0x0003E3C4 File Offset: 0x0003C5C4
	private void waitOnServerTimeAndSchedule()
	{
		VODPlayer.<waitOnServerTimeAndSchedule>d__25 <waitOnServerTimeAndSchedule>d__;
		<waitOnServerTimeAndSchedule>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<waitOnServerTimeAndSchedule>d__.<>4__this = this;
		<waitOnServerTimeAndSchedule>d__.<>1__state = -1;
		<waitOnServerTimeAndSchedule>d__.<>t__builder.Start<VODPlayer.<waitOnServerTimeAndSchedule>d__25>(ref <waitOnServerTimeAndSchedule>d__);
	}

	// Token: 0x06000B97 RID: 2967 RVA: 0x0003E3FB File Offset: 0x0003C5FB
	private Material getStandby(VODTarget o)
	{
		if (!(o.StandbyOverride == null))
		{
			return o.StandbyOverride;
		}
		return this.standbyMaterial;
	}

	// Token: 0x06000B98 RID: 2968 RVA: 0x0003E418 File Offset: 0x0003C618
	private void VODTarget_AlertEnabled(VODTarget o)
	{
		if (!this.targets.Contains(o))
		{
			IEnumerable<VODPlayer.VODStream.VODStreamChannel> priorityChannelArray = this.getPriorityChannelArray();
			this.targets.Add(o);
			bool flag = !priorityChannelArray.SequenceEqual(this.getPriorityChannelArray());
			if (VODPlayer.state == VODPlayer.State.RUNNING && this.player.isPlaying && o.VerifyChannel(this.playerChannel))
			{
				o.Renderer.material = this.playBackMaterial;
				return;
			}
			o.Renderer.material = this.getStandby(o);
			o.SetNext(this.GetNextStream(o.Channel));
			if ((!this.player.isPlaying && this.targets.Count == 1) || flag)
			{
				this.PlayPreviouStream();
			}
		}
	}

	// Token: 0x06000B99 RID: 2969 RVA: 0x0003E4D8 File Offset: 0x0003C6D8
	private void VODTarget_AlertDisabled(VODTarget o)
	{
		if (this.targets.Contains(o))
		{
			IEnumerable<VODPlayer.VODStream.VODStreamChannel> priorityChannelArray = this.getPriorityChannelArray();
			this.targets.Remove(o);
			bool flag = !priorityChannelArray.SequenceEqual(this.getPriorityChannelArray());
			o.Renderer.material = ((o.StandbyOverride == null) ? this.standbyMaterial : o.StandbyOverride);
			o.ClearNext();
			if (this.playerBusy)
			{
				return;
			}
			if (this.player.isPlaying && (this.targets.Count == 0 || flag))
			{
				this.player.Stop();
			}
			if (this.targets.Count > 0 && flag)
			{
				this.PlayPreviouStream();
			}
		}
	}

	// Token: 0x06000B9A RID: 2970 RVA: 0x0003E590 File Offset: 0x0003C790
	private void Player_loopPointReached(VideoPlayer source)
	{
		if (!this.playerBusy)
		{
			this.player.Stop();
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].Renderer.material = this.getStandby(this.targets[i]);
				this.targets[i].SetNext(this.GetNextStream(this.targets[i].Channel));
			}
		}
	}

	// Token: 0x06000B9B RID: 2971 RVA: 0x0003E618 File Offset: 0x0003C818
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.player.loopPointReached -= this.Player_loopPointReached;
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	// Token: 0x06000B9C RID: 2972 RVA: 0x0003E684 File Offset: 0x0003C884
	private void OnDestroy()
	{
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	// Token: 0x06000B9D RID: 2973 RVA: 0x0003E6D4 File Offset: 0x0003C8D4
	void IGorillaSliceableSimple.SliceUpdate()
	{
		switch (VODPlayer.state)
		{
		case VODPlayer.State.INITIALIZING:
		case VODPlayer.State.CRASHED:
			return;
		case VODPlayer.State.IDLE:
			if (this.targets.Count > 0)
			{
				VODPlayer.state = VODPlayer.State.RUNNING;
			}
			return;
		case VODPlayer.State.RUNNING:
		{
			if (this.targets.Count == 0)
			{
				if (!this.playerBusy)
				{
					this.player.Stop();
				}
				VODPlayer.state = VODPlayer.State.IDLE;
				return;
			}
			if (this.player.isPlaying)
			{
				this.PositionAudio();
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			DayOfWeek dayOfWeek = serverTime.DayOfWeek;
			int hour = serverTime.Hour;
			int minute = serverTime.Minute;
			if (minute == this.lastCheck)
			{
				return;
			}
			this.lastCheck = minute;
			List<VODPlayer.VODStream> list = new List<VODPlayer.VODStream>();
			for (int i = 0; i < this.schedule.hourly.Length; i++)
			{
				if (this.schedule.hourly[i].minute - minute == 0 && this.schedule.hourly[i].IsDateInRange(serverTime))
				{
					list.Add(this.schedule.hourly[i].stream);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			if (list.Count == 1)
			{
				this.StartPlayback(list[0], 1.0);
				return;
			}
			List<VODPlayer.VODStream.VODStreamChannel> priorityChannels = this.getPriorityChannels();
			for (int j = 0; j < list.Count; j++)
			{
				if (priorityChannels.Contains(list[j].ch))
				{
					this.StartPlayback(list[j], 1.0);
					return;
				}
			}
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06000B9E RID: 2974 RVA: 0x0003E86C File Offset: 0x0003CA6C
	private List<VODPlayer.VODStream.VODStreamChannel> getPriorityChannels()
	{
		return new List<VODPlayer.VODStream.VODStreamChannel>(this.getPriorityChannelArray());
	}

	// Token: 0x06000B9F RID: 2975 RVA: 0x0003E87C File Offset: 0x0003CA7C
	private VODPlayer.VODStream.VODStreamChannel[] getPriorityChannelArray()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].Distance < num)
			{
				num = this.targets[i].Distance;
				vodtarget = this.targets[i];
			}
		}
		if (vodtarget != null)
		{
			return vodtarget.Channel;
		}
		return new VODPlayer.VODStream.VODStreamChannel[0];
	}

	// Token: 0x06000BA0 RID: 2976 RVA: 0x0003E8F0 File Offset: 0x0003CAF0
	private Task<string> GetCachedFile(string url, string extension)
	{
		VODPlayer.<GetCachedFile>d__36 <GetCachedFile>d__;
		<GetCachedFile>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetCachedFile>d__.<>4__this = this;
		<GetCachedFile>d__.url = url;
		<GetCachedFile>d__.extension = extension;
		<GetCachedFile>d__.<>1__state = -1;
		<GetCachedFile>d__.<>t__builder.Start<VODPlayer.<GetCachedFile>d__36>(ref <GetCachedFile>d__);
		return <GetCachedFile>d__.<>t__builder.Task;
	}

	// Token: 0x06000BA1 RID: 2977 RVA: 0x0003E944 File Offset: 0x0003CB44
	private void Start()
	{
		this.cache = new List<string>();
		string @string = PlayerPrefs.GetString("_VODCache_");
		if (@string.IsNullOrEmpty())
		{
			return;
		}
		List<string> list = JsonConvert.DeserializeObject<List<string>>(@string);
		for (int i = 0; i < list.Count; i++)
		{
			if (File.Exists(list[i]))
			{
				if ((DateTime.Now - File.GetCreationTime(list[i])).TotalDays > 30.0)
				{
					File.Delete(list[i]);
				}
				else
				{
					this.cache.Add(list[i]);
				}
			}
		}
		PlayerPrefs.SetString("_VODCache_", JsonConvert.SerializeObject(this.cache));
	}

	// Token: 0x06000BA2 RID: 2978 RVA: 0x0003E9F4 File Offset: 0x0003CBF4
	private void PositionAudio()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].AudioSettings.volume > 0f && this.targets[i].VerifyChannel(this.playerChannel))
			{
				float sqrMagnitude = (VRRig.LocalRig.transform.position - this.targets[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					vodtarget = this.targets[i];
					num = sqrMagnitude;
				}
			}
		}
		if (vodtarget == null)
		{
			this.audioSource.volume = 0f;
			return;
		}
		this.audioSource.transform.position = vodtarget.transform.position;
		if (this.voiceChatPerm == null)
		{
			this.voiceChatPerm = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
		}
		this.audioSource.volume = ((!this.voiceChatPermRequiredList.Contains(this.playerChannel) || this.voiceChatPerm.Enabled) ? vodtarget.AudioSettings.volume : 0f);
		this.audioSource.dopplerLevel = vodtarget.AudioSettings.dopplerLevel;
		this.audioSource.rolloffMode = vodtarget.AudioSettings.rolloffMode;
		this.audioSource.minDistance = vodtarget.AudioSettings.minDistance;
		this.audioSource.maxDistance = vodtarget.AudioSettings.maxDistance;
	}

	// Token: 0x06000BA3 RID: 2979 RVA: 0x0003EB80 File Offset: 0x0003CD80
	private void PlayPreviouStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		int hour = serverTime.Hour;
		int minute = serverTime.Minute;
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, hour, minute, 0);
		int num = -1;
		List<VODPlayer.VODStream.VODStreamChannel> priorityChannels = this.getPriorityChannels();
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (priorityChannels.Contains(this.schedule.hourly[i].stream.ch) && this.schedule.hourly[i].minute <= minute && this.schedule.hourly[i].IsDateInRange(serverTime))
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			int num2 = minute - this.schedule.hourly[num].minute;
			this.StartPlayback(this.schedule.hourly[num].stream, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num2))).TotalSeconds);
		}
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003ECA8 File Offset: 0x0003CEA8
	private void StartPlayback(VODPlayer.VODStream str, double time = 0.0)
	{
		VODPlayer.VODStream.VODStreamType type = str.type;
		if (type == VODPlayer.VODStream.VODStreamType.VIDEO)
		{
			this.StartVideoPlayback(str.url, str.ch, time);
			return;
		}
		if (type != VODPlayer.VODStream.VODStreamType.IMAGE)
		{
			return;
		}
		this.StartImagePlayback(str.url, str.duration, str.ch, time);
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0003ECF4 File Offset: 0x0003CEF4
	private void StartImagePlayback(string url, int duration, VODPlayer.VODStream.VODStreamChannel ch, double time = 0.0)
	{
		VODPlayer.<StartImagePlayback>d__45 <StartImagePlayback>d__;
		<StartImagePlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartImagePlayback>d__.<>4__this = this;
		<StartImagePlayback>d__.url = url;
		<StartImagePlayback>d__.duration = duration;
		<StartImagePlayback>d__.ch = ch;
		<StartImagePlayback>d__.time = time;
		<StartImagePlayback>d__.<>1__state = -1;
		<StartImagePlayback>d__.<>t__builder.Start<VODPlayer.<StartImagePlayback>d__45>(ref <StartImagePlayback>d__);
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003ED4C File Offset: 0x0003CF4C
	private void StartVideoPlayback(string url, VODPlayer.VODStream.VODStreamChannel ch, double time = 0.0)
	{
		VODPlayer.<StartVideoPlayback>d__46 <StartVideoPlayback>d__;
		<StartVideoPlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartVideoPlayback>d__.<>4__this = this;
		<StartVideoPlayback>d__.url = url;
		<StartVideoPlayback>d__.ch = ch;
		<StartVideoPlayback>d__.time = time;
		<StartVideoPlayback>d__.<>1__state = -1;
		<StartVideoPlayback>d__.<>t__builder.Start<VODPlayer.<StartVideoPlayback>d__46>(ref <StartVideoPlayback>d__);
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0003ED9C File Offset: 0x0003CF9C
	private void onTD(string s)
	{
		this.tdGot++;
		if (s.IsNullOrEmpty())
		{
			Debug.LogError("Crash(\"No schedule data\")");
			return;
		}
		VODPlayer.VODStreamSchedule vodstreamSchedule = default(VODPlayer.VODStreamSchedule);
		try
		{
			vodstreamSchedule = JsonConvert.DeserializeObject<VODPlayer.VODStreamSchedule>(s);
		}
		catch (Exception ex)
		{
			Debug.LogError("Crash(\"Malformed schedule data\") :: " + ex.Message + " :: " + s);
		}
		for (int i = 0; i < vodstreamSchedule.hourly.Length; i++)
		{
			vodstreamSchedule.hourly[i].ValidateDate();
		}
		this.schedule.Merge(vodstreamSchedule);
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003EE3C File Offset: 0x0003D03C
	private void onTDError(PlayFabError error)
	{
		this.tdGot++;
		Debug.LogError("TD Error: " + error.ErrorMessage);
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003EE64 File Offset: 0x0003D064
	private VODPlayer.VODNextStreamData GetNextStream(VODPlayer.VODStream.VODStreamChannel[] ch)
	{
		return this.GetNextStream(ch, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003EEB8 File Offset: 0x0003D0B8
	private VODPlayer.VODNextStreamData GetNextStream(VODPlayer.VODStream.VODStreamChannel[] ch, DateTime now)
	{
		List<VODPlayer.VODStream.VODStreamChannel> list = new List<VODPlayer.VODStream.VODStreamChannel>(ch);
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (!this.schedule.hourly[i].stream.hideUpNext && list.Contains(this.schedule.hourly[i].stream.ch) && this.schedule.hourly[i].minute > now.Minute)
			{
				DateTime startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, this.schedule.hourly[i].minute, 0);
				return new VODPlayer.VODNextStreamData(this.schedule.hourly[i].stream.name, startTime);
			}
		}
		for (int j = 0; j < this.schedule.hourly.Length; j++)
		{
			if (!this.schedule.hourly[j].stream.hideUpNext && list.Contains(this.schedule.hourly[j].stream.ch) && this.schedule.hourly[j].minute < now.Minute)
			{
				DateTime startTime2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, this.schedule.hourly[j].minute, 0).AddHours(1.0);
				return new VODPlayer.VODNextStreamData(this.schedule.hourly[j].stream.name, startTime2);
			}
		}
		return new VODPlayer.VODNextStreamData(string.Empty, DateTime.MinValue);
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0003F0AC File Offset: 0x0003D2AC
	public static string[] GetSchedule(VODPlayer.VODStreamSchedule schedule, VODPlayer.VODStream.VODStreamChannel[] ch)
	{
		return VODPlayer.GetSchedule(schedule, ch, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003F100 File Offset: 0x0003D300
	public static string[] GetSchedule(VODPlayer.VODStreamSchedule schedule, VODPlayer.VODStream.VODStreamChannel[] ch, DateTime now)
	{
		List<string> list = new List<string>();
		List<VODPlayer.VODStream.VODStreamChannel> list2 = new List<VODPlayer.VODStream.VODStreamChannel>(ch);
		for (int i = 0; i < schedule.hourly.Length; i++)
		{
			if (!schedule.hourly[i].stream.hideUpNext && list2.Contains(schedule.hourly[i].stream.ch) && schedule.hourly[i].minute > now.Minute)
			{
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[i].minute, 0);
				list.Add(dateTime.ToShortTimeString() + " --- " + schedule.hourly[i].stream.name);
			}
		}
		for (int j = 0; j < schedule.hourly.Length; j++)
		{
			if (!schedule.hourly[j].stream.hideUpNext && list2.Contains(schedule.hourly[j].stream.ch) && schedule.hourly[j].minute < now.Minute)
			{
				list.Add(new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[j].minute, 0).AddHours(1.0).ToShortTimeString() + " --- " + schedule.hourly[j].stream.name);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003F2DC File Offset: 0x0003D4DC
	public static Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> GetSchedule(VODPlayer.VODStreamSchedule schedule)
	{
		return VODPlayer.GetSchedule(schedule, (GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000) ? DateTime.UtcNow : GorillaComputer.instance.GetServerTime());
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0003F32C File Offset: 0x0003D52C
	public static Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> GetSchedule(VODPlayer.VODStreamSchedule schedule, DateTime now)
	{
		Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>> dictionary = new Dictionary<VODPlayer.VODStream.VODStreamChannel, List<string>>();
		for (int i = 0; i < schedule.hourly.Length; i++)
		{
			if (!schedule.hourly[i].stream.hideUpNext && schedule.hourly[i].minute > now.Minute)
			{
				DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[i].minute, 0);
				if (!dictionary.ContainsKey(schedule.hourly[i].stream.ch))
				{
					dictionary.Add(schedule.hourly[i].stream.ch, new List<string>());
				}
				dictionary[schedule.hourly[i].stream.ch].Add(dateTime.ToShortTimeString() + " --- " + schedule.hourly[i].stream.name);
			}
		}
		for (int j = 0; j < schedule.hourly.Length; j++)
		{
			if (!schedule.hourly[j].stream.hideUpNext && schedule.hourly[j].minute < now.Minute)
			{
				DateTime dateTime2 = new DateTime(now.Year, now.Month, now.Day, now.Hour, schedule.hourly[j].minute, 0).AddHours(1.0);
				if (!dictionary.ContainsKey(schedule.hourly[j].stream.ch))
				{
					dictionary.Add(schedule.hourly[j].stream.ch, new List<string>());
				}
				dictionary[schedule.hourly[j].stream.ch].Add(dateTime2.ToShortTimeString() + " --- " + schedule.hourly[j].stream.name);
			}
		}
		return dictionary;
	}

	// Token: 0x04000DF8 RID: 3576
	private static Material _standbyMaterial;

	// Token: 0x04000DF9 RID: 3577
	private const string PlayerPrefKey_Cache = "_VODCache_";

	// Token: 0x04000DFA RID: 3578
	public static Action OnCrash;

	// Token: 0x04000DFB RID: 3579
	public static VODPlayer.State state;

	// Token: 0x04000DFC RID: 3580
	private VideoPlayer player;

	// Token: 0x04000DFD RID: 3581
	private AudioSource audioSource;

	// Token: 0x04000DFE RID: 3582
	private VODPlayer.VODStreamSchedule schedule;

	// Token: 0x04000DFF RID: 3583
	[SerializeField]
	private string[] titleDataKey;

	// Token: 0x04000E00 RID: 3584
	[SerializeField]
	private Material standbyMaterial;

	// Token: 0x04000E01 RID: 3585
	[SerializeField]
	private Material playBackMaterial;

	// Token: 0x04000E02 RID: 3586
	[SerializeField]
	private Material busyMaterial;

	// Token: 0x04000E03 RID: 3587
	[SerializeField]
	private Material imageMaterial;

	// Token: 0x04000E04 RID: 3588
	[SerializeField]
	private VODPlayer.VODStream.VODStreamChannel[] voiceChatPermRequired;

	// Token: 0x04000E05 RID: 3589
	private List<VODTarget> targets = new List<VODTarget>();

	// Token: 0x04000E06 RID: 3590
	private List<VODPlayer.VODStream.VODStreamChannel> voiceChatPermRequiredList;

	// Token: 0x04000E07 RID: 3591
	private int lastCheck;

	// Token: 0x04000E08 RID: 3592
	private List<string> cache = new List<string>();

	// Token: 0x04000E09 RID: 3593
	private bool playerBusy;

	// Token: 0x04000E0A RID: 3594
	private VODPlayer.VODStream.VODStreamChannel playerChannel;

	// Token: 0x04000E0B RID: 3595
	private int tdGot;

	// Token: 0x04000E0C RID: 3596
	private Permission voiceChatPerm;

	// Token: 0x020001AC RID: 428
	public enum State
	{
		// Token: 0x04000E0E RID: 3598
		INITIALIZING,
		// Token: 0x04000E0F RID: 3599
		IDLE,
		// Token: 0x04000E10 RID: 3600
		RUNNING,
		// Token: 0x04000E11 RID: 3601
		CRASHED
	}

	// Token: 0x020001AD RID: 429
	public struct VODNextStreamData
	{
		// Token: 0x06000BB0 RID: 2992 RVA: 0x0003F58A File Offset: 0x0003D78A
		public VODNextStreamData(string title, DateTime startTime)
		{
			this.Title = title;
			this.StartTime = startTime;
		}

		// Token: 0x04000E12 RID: 3602
		public string Title;

		// Token: 0x04000E13 RID: 3603
		public DateTime StartTime;
	}

	// Token: 0x020001AE RID: 430
	[Serializable]
	public struct VODStreamSchedule
	{
		// Token: 0x06000BB1 RID: 2993 RVA: 0x0003F59C File Offset: 0x0003D79C
		internal void Merge(VODPlayer.VODStreamSchedule subSchedule)
		{
			List<VODPlayer.VODHourlyStream> list = new List<VODPlayer.VODHourlyStream>();
			if (this.hourly != null)
			{
				list.AddRange(this.hourly);
			}
			for (int i = 0; i < subSchedule.hourly.Length; i++)
			{
				list.Add(subSchedule.hourly[i]);
				int num = 0;
				while (subSchedule.hourly[i].repeats != null && num < subSchedule.hourly[i].repeats.Length)
				{
					VODPlayer.VODHourlyStream item = default(VODPlayer.VODHourlyStream);
					item.stream = subSchedule.hourly[i].stream;
					item.minute = subSchedule.hourly[i].repeats[num];
					item.startDateTime = subSchedule.hourly[i].startDateTime;
					item.endDateTime = subSchedule.hourly[i].endDateTime;
					item.ValidateDate();
					list.Add(item);
					num++;
				}
			}
			list.Sort();
			this.hourly = list.ToArray();
		}

		// Token: 0x04000E14 RID: 3604
		public VODPlayer.VODHourlyStream[] hourly;
	}

	// Token: 0x020001AF RID: 431
	[Serializable]
	public struct VODStream
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000BB2 RID: 2994 RVA: 0x0003F6AC File Offset: 0x0003D8AC
		public string displayTitle
		{
			get
			{
				if (!this.hideUpNext)
				{
					return this.name;
				}
				return string.Empty;
			}
		}

		// Token: 0x04000E15 RID: 3605
		public string name;

		// Token: 0x04000E16 RID: 3606
		public bool hideUpNext;

		// Token: 0x04000E17 RID: 3607
		public string url;

		// Token: 0x04000E18 RID: 3608
		public VODPlayer.VODStream.VODStreamType type;

		// Token: 0x04000E19 RID: 3609
		public int duration;

		// Token: 0x04000E1A RID: 3610
		public VODPlayer.VODStream.VODStreamChannel ch;

		// Token: 0x020001B0 RID: 432
		public enum VODStreamType
		{
			// Token: 0x04000E1C RID: 3612
			VIDEO,
			// Token: 0x04000E1D RID: 3613
			IMAGE
		}

		// Token: 0x020001B1 RID: 433
		public enum VODStreamChannel
		{
			// Token: 0x04000E1F RID: 3615
			DEFAULT,
			// Token: 0x04000E20 RID: 3616
			VIM,
			// Token: 0x04000E21 RID: 3617
			MM,
			// Token: 0x04000E22 RID: 3618
			GCORP,
			// Token: 0x04000E23 RID: 3619
			EVENT,
			// Token: 0x04000E24 RID: 3620
			FEATURED
		}
	}

	// Token: 0x020001B2 RID: 434
	[Serializable]
	public struct VODHourlyStream : IComparable<VODPlayer.VODHourlyStream>
	{
		// Token: 0x06000BB3 RID: 2995 RVA: 0x0003F6C2 File Offset: 0x0003D8C2
		public int CompareTo(VODPlayer.VODHourlyStream other)
		{
			return this.minute - other.minute;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0003F6D4 File Offset: 0x0003D8D4
		public void ValidateDate()
		{
			try
			{
				this.startDT = DateTime.Parse(this.startDateTime);
			}
			catch
			{
				this.startDT = DateTime.Parse("1/1/0001");
			}
			try
			{
				this.endDT = DateTime.Parse(this.endDateTime);
			}
			catch
			{
				this.endDT = DateTime.Parse("1/1/3001");
			}
			this.startDateTime = this.startDT.ToString();
			this.endDateTime = this.endDT.ToString();
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0003F76C File Offset: 0x0003D96C
		internal bool IsDateInRange(DateTime serverTime)
		{
			return serverTime >= this.startDT && serverTime <= this.endDT;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0003F78A File Offset: 0x0003D98A
		internal DateTime ClampedDateTime(DateTime dateTime)
		{
			if (dateTime < this.startDT)
			{
				return this.startDT;
			}
			if (dateTime > this.endDT)
			{
				return this.endDT;
			}
			return dateTime;
		}

		// Token: 0x04000E25 RID: 3621
		public VODPlayer.VODStream stream;

		// Token: 0x04000E26 RID: 3622
		[Range(0f, 59f)]
		public int minute;

		// Token: 0x04000E27 RID: 3623
		[Range(0f, 59f)]
		public int[] repeats;

		// Token: 0x04000E28 RID: 3624
		public string startDateTime;

		// Token: 0x04000E29 RID: 3625
		private DateTime startDT;

		// Token: 0x04000E2A RID: 3626
		public string endDateTime;

		// Token: 0x04000E2B RID: 3627
		private DateTime endDT;
	}
}
