using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020009DA RID: 2522
public class SynchedMusicController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004095 RID: 16533 RVA: 0x00158A14 File Offset: 0x00156C14
	private void Start()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Start();
			return;
		}
		this.totalLoopTime = 0L;
		AudioSource[] array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
		}
		this.audioSource.mute = (PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0);
		this.muteButton.isOn = this.audioSource.mute;
		this.muteButton.UpdateColor();
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			this.muteButtons[j].isOn = this.audioSource.mute;
			this.muteButtons[j].UpdateColor();
		}
		this.randomNumberGenerator = new Random(this.mySeed);
		this.GenerateSongStartRandomTimes();
		if (this.twoLayer)
		{
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].clip.LoadAudioData();
			}
		}
	}

	// Token: 0x06004096 RID: 16534 RVA: 0x00158B30 File Offset: 0x00156D30
	public void SliceUpdate()
	{
		if (this.usingNewSyncedSongsCode)
		{
			this.New_Update();
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime == 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		this.isPlayingCurrently = this.audioSource.isPlaying;
		if (this.testPlay)
		{
			this.testPlay = false;
			if (this.usingMultipleSources && this.usingMultipleSongs)
			{
				this.audioSource = this.audioSourceArray[Random.Range(0, this.audioSourceArray.Length)];
				this.audioSource.clip = this.songsArray[Random.Range(0, this.songsArray.Length)];
				this.audioSource.time = 0f;
			}
			if (this.twoLayer)
			{
				this.StartPlayingSongs(0L, 0L);
			}
			else if (this.audioSource.volume != 0f)
			{
				this.audioSource.GTPlay();
			}
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		this.currentTime = (GorillaComputer.instance.startupMillis + (long)(Time.realtimeSinceStartup * 1000f)) % this.totalLoopTime;
		if (!this.audioSource.isPlaying)
		{
			if (this.lastPlayIndex >= 0 && this.songStartTimes[this.lastPlayIndex % this.songStartTimes.Length] < this.currentTime && this.currentTime < this.songStartTimes[(this.lastPlayIndex + 1) % this.songStartTimes.Length])
			{
				if (this.twoLayer)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
					{
						this.StartPlayingSongs(this.songStartTimes[this.lastPlayIndex], this.currentTime);
						return;
					}
				}
				else if (this.usingMultipleSongs && this.usingMultipleSources)
				{
					if (this.songStartTimes[this.lastPlayIndex] + (long)(this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]].length * 1000f) > this.currentTime)
					{
						this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime, this.songsArray[this.audioClipsForPlaying[this.lastPlayIndex]], this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]]);
						return;
					}
				}
				else if (this.songStartTimes[this.lastPlayIndex] + (long)(this.audioSource.clip.length * 1000f) > this.currentTime)
				{
					this.StartPlayingSong(this.songStartTimes[this.lastPlayIndex], this.currentTime);
					return;
				}
			}
			else
			{
				for (int i = 0; i < this.songStartTimes.Length; i++)
				{
					if (this.songStartTimes[i] > this.currentTime)
					{
						this.lastPlayIndex = (i - 1) % this.songStartTimes.Length;
						return;
					}
				}
			}
		}
	}

	// Token: 0x06004097 RID: 16535 RVA: 0x00158E0E File Offset: 0x0015700E
	private void StartPlayingSong(long timeStarted, long currentTime)
	{
		if (this.audioSource.volume != 0f)
		{
			this.audioSource.GTPlay();
		}
		this.audioSource.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x06004098 RID: 16536 RVA: 0x00158E44 File Offset: 0x00157044
	private void StartPlayingSongs(long timeStarted, long currentTime)
	{
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			if (audioSource.volume != 0f)
			{
				audioSource.GTPlay();
			}
			audioSource.time = (float)(currentTime - timeStarted) / 1000f;
		}
	}

	// Token: 0x06004099 RID: 16537 RVA: 0x00158E90 File Offset: 0x00157090
	private void StartPlayingSong(long timeStarted, long currentTime, AudioClip clipToPlay, AudioSource sourceToPlay)
	{
		this.audioSource = sourceToPlay;
		sourceToPlay.clip = clipToPlay;
		if (sourceToPlay.isActiveAndEnabled && sourceToPlay.volume != 0f)
		{
			sourceToPlay.GTPlay();
		}
		sourceToPlay.time = (float)(currentTime - timeStarted) / 1000f;
	}

	// Token: 0x0600409A RID: 16538 RVA: 0x00158EDC File Offset: 0x001570DC
	private void GenerateSongStartRandomTimes()
	{
		this.songStartTimes = new long[500];
		this.audioSourcesForPlaying = new int[500];
		this.audioClipsForPlaying = new int[500];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		if (this.usingMultipleSources)
		{
			for (int j = 0; j < this.audioSourcesForPlaying.Length; j++)
			{
				this.audioSourcesForPlaying[j] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			for (int k = 0; k < this.audioClipsForPlaying.Length; k++)
			{
				this.audioClipsForPlaying[k] = this.randomNumberGenerator.Next(this.songsArray.Length);
			}
		}
		if (this.usingMultipleSongs)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.songsArray[this.audioClipsForPlaying[this.audioClipsForPlaying.Length - 1]].length * 1000f);
			return;
		}
		if (this.audioSource.clip != null)
		{
			this.totalLoopTime = this.songStartTimes[this.songStartTimes.Length - 1] + (long)(this.audioSource.clip.length * 1000f);
		}
	}

	// Token: 0x0600409B RID: 16539 RVA: 0x0015906C File Offset: 0x0015726C
	public void MuteAudio(GorillaPressableButton pressedButton)
	{
		AudioSource[] array;
		if (this.audioSource.mute)
		{
			PlayerPrefs.SetInt(this.locationName + "Muted", 0);
			PlayerPrefs.Save();
			this.audioSource.mute = false;
			array = this.audioSourceArray;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].mute = false;
			}
			pressedButton.isOn = false;
			pressedButton.UpdateColor();
			for (int j = 0; j < this.muteButtons.Length; j++)
			{
				if (this.muteButtons[j] != null)
				{
					this.muteButtons[j].isOn = false;
					this.muteButtons[j].UpdateColor();
				}
			}
			return;
		}
		PlayerPrefs.SetInt(this.locationName + "Muted", 1);
		PlayerPrefs.Save();
		this.audioSource.mute = true;
		array = this.audioSourceArray;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = true;
		}
		pressedButton.isOn = true;
		pressedButton.UpdateColor();
		for (int k = 0; k < this.muteButtons.Length; k++)
		{
			if (this.muteButtons[k] != null)
			{
				this.muteButtons[k].isOn = true;
				this.muteButtons[k].UpdateColor();
			}
		}
	}

	// Token: 0x0600409C RID: 16540 RVA: 0x001591AC File Offset: 0x001573AC
	protected void New_Start()
	{
		string text = this.New_Validate();
		if (text.Length > 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Disabling SynchedMusicController on \"",
				base.name,
				"\" due to invalid setup: ",
				text,
				" Path: ",
				base.transform.GetPathQ()
			}), this);
			base.enabled = false;
		}
		if (this.usingMultipleSources && this.audioSource == null)
		{
			this.audioSource = this.audioSourceArray[0];
		}
		this.totalLoopTime = 0L;
		bool mute = PlayerPrefs.GetInt(this.locationName + "Muted", 0) != 0;
		if (this.muteButton == null && this.muteButtons.Length >= 1 && this.muteButtons[0] != null)
		{
			this.muteButton = this.muteButtons[0];
		}
		if (this.audioSource != null)
		{
			this.audioSource.mute = mute;
			this.muteButton.isOn = this.audioSource.mute;
		}
		foreach (AudioSource audioSource in this.audioSourceArray)
		{
			audioSource.mute = mute;
			this.muteButton.isOn = (audioSource.mute || this.muteButton.isOn);
		}
		for (int j = 0; j < this.muteButtons.Length; j++)
		{
			if (!(this.muteButtons[j] == null))
			{
				this.muteButtons[j].isOn = this.muteButton.isOn;
				this.muteButtons[j].UpdateColor();
			}
		}
		this.muteButton.UpdateColor();
		this.randomNumberGenerator = new Random(this.mySeed);
		this.New_GeneratePlaylistArrays();
		foreach (SynchedMusicController.SyncedSongInfo syncedSongInfo in this.syncedSongs)
		{
			if (syncedSongInfo.songLayers.Length > 1)
			{
				SynchedMusicController.SyncedSongLayerInfo[] songLayers = syncedSongInfo.songLayers;
				for (int k = 0; k < songLayers.Length; k++)
				{
					songLayers[k].audioClip.LoadAudioData();
				}
			}
		}
	}

	// Token: 0x0600409D RID: 16541 RVA: 0x001593D3 File Offset: 0x001575D3
	public void OnEnable()
	{
		this.lastPlayIndex = -1;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600409E RID: 16542 RVA: 0x001593E3 File Offset: 0x001575E3
	public void OnDisable()
	{
		this.StopAllAudioSources();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600409F RID: 16543 RVA: 0x001593F4 File Offset: 0x001575F4
	private void StopAllAudioSources()
	{
		for (int i = 0; i < this.audioSourceArray.Length; i++)
		{
			this.audioSourceArray[i].Stop();
		}
	}

	// Token: 0x060040A0 RID: 16544 RVA: 0x00159424 File Offset: 0x00157624
	private void New_Update()
	{
		if (!GorillaComputer.hasInstance)
		{
			return;
		}
		if (GorillaComputer.instance.startupMillis == 0L || this.totalLoopTime <= 0L || this.songStartTimes.Length == 0)
		{
			return;
		}
		long startupMillis = GorillaComputer.instance.startupMillis;
		if (startupMillis <= 0L)
		{
			return;
		}
		long num = startupMillis + (long)(Time.realtimeSinceStartup * 1000f);
		long num2 = (this.totalLoopTime > 0L) ? (num % this.totalLoopTime) : 0L;
		bool flag = false;
		if (this.lastPlayIndex < 0)
		{
			flag = true;
			for (int i = 1; i < 256; i++)
			{
				if (this.songStartTimes[i] > num2)
				{
					this.lastPlayIndex = (i - 1) % 256;
					break;
				}
			}
			if (this.lastPlayIndex < 0)
			{
				this.lastPlayIndex = 255;
			}
		}
		int num3 = (this.lastPlayIndex + 1) % 256;
		if (this.songStartTimes[num3] < num2)
		{
			this.lastPlayIndex = num3;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		long num4 = this.songStartTimes[this.lastPlayIndex];
		SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[this.audioClipsForPlaying[this.lastPlayIndex]];
		float length = syncedSongInfo.songLayers[0].audioClip.length;
		float num5 = (float)(num2 - num4) / 1000f;
		if (num5 < 0f || length < num5)
		{
			return;
		}
		for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
			if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.All)
			{
				foreach (AudioSource audioSource in this.audioSourceArray)
				{
					audioSource.clip = syncedSongLayerInfo.audioClip;
					if (audioSource.volume > 0f)
					{
						audioSource.GTPlay();
					}
					audioSource.time = num5;
				}
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
			{
				AudioSource audioSource2 = this.audioSourceArray[this.audioSourcesForPlaying[this.lastPlayIndex]];
				audioSource2.clip = syncedSongLayerInfo.audioClip;
				if (audioSource2.volume > 0f)
				{
					audioSource2.GTPlay();
				}
				audioSource2.time = num5;
			}
			else if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
			{
				foreach (AudioSource audioSource3 in syncedSongLayerInfo.audioSources)
				{
					audioSource3.clip = syncedSongLayerInfo.audioClip;
					if (audioSource3.volume > 0f)
					{
						audioSource3.GTPlay();
					}
					audioSource3.time = num5;
				}
			}
		}
	}

	// Token: 0x060040A1 RID: 16545 RVA: 0x001596A4 File Offset: 0x001578A4
	private string New_Validate()
	{
		if (this.syncedSongs == null)
		{
			return "syncedSongs array cannot be null.";
		}
		if (this.syncedSongs.Length == 0)
		{
			return "syncedSongs array cannot be empty.";
		}
		for (int i = 0; i < this.syncedSongs.Length; i++)
		{
			SynchedMusicController.SyncedSongInfo syncedSongInfo = this.syncedSongs[i];
			if (syncedSongInfo.songLayers == null)
			{
				return string.Format("Song {0}'s songLayers array is null.", i);
			}
			if (syncedSongInfo.songLayers.Length == 0)
			{
				return string.Format("Song {0}'s songLayers array is empty.", i);
			}
			for (int j = 0; j < syncedSongInfo.songLayers.Length; j++)
			{
				SynchedMusicController.SyncedSongLayerInfo syncedSongLayerInfo = syncedSongInfo.songLayers[j];
				if (syncedSongLayerInfo.audioClip == null)
				{
					return string.Format("Song {0}'s song layer {1} does not have an audio clip.", i, j);
				}
				if (syncedSongLayerInfo.audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Specific)
				{
					if (syncedSongLayerInfo.audioSources == null || syncedSongLayerInfo.audioSources.Length == 0)
					{
						return string.Format("Song {0}'s song layer {1} has audioSourcePickMode set to {2} ", i, j, syncedSongLayerInfo.audioSourcePickMode) + "but layer's audioSources array is empty or null.";
					}
				}
				else if (this.audioSourceArray == null || this.audioSourceArray.Length == 0)
				{
					return string.Format("{0} is null or empty, while Song {1}'s song layer {2} has ", "audioSourceArray", i, j) + string.Format("audioSourcePickMode set to {0}, which uses the ", syncedSongLayerInfo.audioSourcePickMode) + "component's audioSourceArray.";
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x060040A2 RID: 16546 RVA: 0x0015980C File Offset: 0x00157A0C
	private void New_GeneratePlaylistArrays()
	{
		if (this.syncedSongs == null || this.syncedSongs.Length == 0)
		{
			return;
		}
		this.songStartTimes = new long[256];
		this.songStartTimes[0] = this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		for (int i = 1; i < this.songStartTimes.Length; i++)
		{
			this.songStartTimes[i] = this.songStartTimes[i - 1] + this.minimumWait + (long)this.randomNumberGenerator.Next(this.randomInterval);
		}
		this.audioSourcesForPlaying = new int[256];
		bool flag = false;
		SynchedMusicController.SyncedSongInfo[] array = this.syncedSongs;
		for (int j = 0; j < array.Length; j++)
		{
			SynchedMusicController.SyncedSongLayerInfo[] songLayers = array[j].songLayers;
			for (int k = 0; k < songLayers.Length; k++)
			{
				if (songLayers[k].audioSourcePickMode == SynchedMusicController.AudioSourcePickMode.Shuffle)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			for (int l = 0; l < this.audioSourcesForPlaying.Length; l++)
			{
				this.audioSourcesForPlaying[l] = this.randomNumberGenerator.Next(this.audioSourceArray.Length);
			}
		}
		this.audioClipsForPlaying = new int[256];
		for (int m = 0; m < this.audioClipsForPlaying.Length; m++)
		{
			if (this.shufflePlaylist)
			{
				this.audioClipsForPlaying[m] = this.randomNumberGenerator.Next(this.syncedSongs.Length);
			}
			else
			{
				this.audioClipsForPlaying[m] = this.syncedSongs.Length - 1;
			}
		}
		SynchedMusicController.SyncedSongInfo[] array2 = this.syncedSongs;
		int[] array3 = this.audioClipsForPlaying;
		long num = (long)array2[array3[array3.Length - 1]].songLayers[0].audioClip.length * 1000L;
		long[] array4 = this.songStartTimes;
		long num2 = array4[array4.Length - 1];
		this.totalLoopTime = num + num2;
	}

	// Token: 0x0400511C RID: 20764
	[SerializeField]
	private bool usingNewSyncedSongsCode;

	// Token: 0x0400511D RID: 20765
	[SerializeField]
	private bool shufflePlaylist = true;

	// Token: 0x0400511E RID: 20766
	[SerializeField]
	private SynchedMusicController.SyncedSongInfo[] syncedSongs;

	// Token: 0x0400511F RID: 20767
	[Tooltip("This should be unique per sound post. Sound posts that share the same seed and the same song count will play songs a the same times.")]
	public int mySeed;

	// Token: 0x04005120 RID: 20768
	private Random randomNumberGenerator = new Random();

	// Token: 0x04005121 RID: 20769
	[Tooltip("In milliseconds.")]
	public long minimumWait = 900000L;

	// Token: 0x04005122 RID: 20770
	[Tooltip("In milliseconds. A random value between 0 and this will be picked. The max wait time is randomInterval + minimumWait.")]
	public int randomInterval = 600000;

	// Token: 0x04005123 RID: 20771
	[DebugReadout]
	public long[] songStartTimes;

	// Token: 0x04005124 RID: 20772
	[DebugReadout]
	public int[] audioSourcesForPlaying;

	// Token: 0x04005125 RID: 20773
	[DebugReadout]
	public int[] audioClipsForPlaying;

	// Token: 0x04005126 RID: 20774
	public AudioSource audioSource;

	// Token: 0x04005127 RID: 20775
	public AudioSource[] audioSourceArray;

	// Token: 0x04005128 RID: 20776
	public AudioClip[] songsArray;

	// Token: 0x04005129 RID: 20777
	[DebugReadout]
	public int lastPlayIndex;

	// Token: 0x0400512A RID: 20778
	[DebugReadout]
	public long currentTime;

	// Token: 0x0400512B RID: 20779
	[DebugReadout]
	public long totalLoopTime;

	// Token: 0x0400512C RID: 20780
	public GorillaPressableButton muteButton;

	// Token: 0x0400512D RID: 20781
	public GorillaPressableButton[] muteButtons;

	// Token: 0x0400512E RID: 20782
	public bool usingMultipleSongs;

	// Token: 0x0400512F RID: 20783
	public bool usingMultipleSources;

	// Token: 0x04005130 RID: 20784
	[DebugReadout]
	public bool isPlayingCurrently;

	// Token: 0x04005131 RID: 20785
	[DebugReadout]
	public bool testPlay;

	// Token: 0x04005132 RID: 20786
	public bool twoLayer;

	// Token: 0x04005133 RID: 20787
	[Tooltip("Used to store the muted sound posts in player prefs.")]
	public string locationName;

	// Token: 0x04005134 RID: 20788
	private const int kPlaylistLength = 256;

	// Token: 0x020009DB RID: 2523
	[Serializable]
	public struct SyncedSongInfo
	{
		// Token: 0x04005135 RID: 20789
		[Tooltip("A layer for a song. For no layers, just add a single entry.")]
		[RequiredListLength(1, null)]
		public SynchedMusicController.SyncedSongLayerInfo[] songLayers;
	}

	// Token: 0x020009DC RID: 2524
	[Serializable]
	public struct SyncedSongLayerInfo
	{
		// Token: 0x04005136 RID: 20790
		[Tooltip("The clip that will be played.")]
		public AudioClip audioClip;

		// Token: 0x04005137 RID: 20791
		public SynchedMusicController.AudioSourcePickMode audioSourcePickMode;

		// Token: 0x04005138 RID: 20792
		[Tooltip("The audio sources that should play the audio clip.")]
		public AudioSource[] audioSources;
	}

	// Token: 0x020009DD RID: 2525
	public enum AudioSourcePickMode
	{
		// Token: 0x0400513A RID: 20794
		All,
		// Token: 0x0400513B RID: 20795
		Shuffle,
		// Token: 0x0400513C RID: 20796
		Specific
	}
}
