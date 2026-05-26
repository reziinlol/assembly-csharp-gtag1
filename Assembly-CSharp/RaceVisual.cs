using System;
using TMPro;
using UnityEngine;

// Token: 0x02000367 RID: 871
public class RaceVisual : MonoBehaviour
{
	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06001541 RID: 5441 RVA: 0x00070B13 File Offset: 0x0006ED13
	// (set) Token: 0x06001542 RID: 5442 RVA: 0x00070B1B File Offset: 0x0006ED1B
	public int raceId { get; private set; }

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06001543 RID: 5443 RVA: 0x00070B24 File Offset: 0x0006ED24
	// (set) Token: 0x06001544 RID: 5444 RVA: 0x00070B2C File Offset: 0x0006ED2C
	public bool TickRunning { get; set; }

	// Token: 0x06001545 RID: 5445 RVA: 0x00070B35 File Offset: 0x0006ED35
	private void Awake()
	{
		this.checkpoints = base.GetComponent<RaceCheckpointManager>();
		this.finishLineText.text = "";
		this.SetScoreboardText("", "");
		this.SetRaceStartScoreboardText("", "");
	}

	// Token: 0x06001546 RID: 5446 RVA: 0x00070B73 File Offset: 0x0006ED73
	private void OnEnable()
	{
		RacingManager.instance.RegisterVisual(this);
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x00070B80 File Offset: 0x0006ED80
	public void Button_StartRace(int laps)
	{
		RacingManager.instance.Button_StartRace(this.raceId, laps);
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x00070B93 File Offset: 0x0006ED93
	public void ShowFinishLineText(string text)
	{
		this.finishLineText.text = text;
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00070BA1 File Offset: 0x0006EDA1
	public void UpdateCountdown(int timeRemaining)
	{
		if (timeRemaining != this.lastDisplayedCountdown)
		{
			this.countdownText.text = timeRemaining.ToString();
			this.finishLineText.text = "";
			this.lastDisplayedCountdown = timeRemaining;
		}
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x00070BD8 File Offset: 0x0006EDD8
	public void SetScoreboardText(string mainText, string timesText)
	{
		foreach (RacingScoreboard racingScoreboard in this.raceScoreboards)
		{
			racingScoreboard.mainDisplay.text = mainText;
			racingScoreboard.timesDisplay.text = timesText;
		}
	}

	// Token: 0x0600154B RID: 5451 RVA: 0x00070C14 File Offset: 0x0006EE14
	public void SetRaceStartScoreboardText(string mainText, string timesText)
	{
		this.raceStartScoreboard.mainDisplay.text = mainText;
		this.raceStartScoreboard.timesDisplay.text = timesText;
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00070C38 File Offset: 0x0006EE38
	public void ActivateStartingWall(bool enable)
	{
		this.startingWall.SetActive(enable);
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x00070C46 File Offset: 0x0006EE46
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpoint)
	{
		return this.checkpoints.IsPlayerNearCheckpoint(player, checkpoint);
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x00070C55 File Offset: 0x0006EE55
	public void OnCountdownStart(int laps, float goAfterInterval)
	{
		this.raceConsoleVisual.ShowRaceInProgress(laps);
		this.countdownSoundPlayer.Play();
		this.countdownSoundPlayer.time = this.countdownSoundGoTime - goAfterInterval;
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x00070C81 File Offset: 0x0006EE81
	public void OnRaceStart()
	{
		this.finishLineText.text = "GO!";
		this.checkpoints.OnRaceStart();
		this.lastDisplayedCountdown = 0;
		this.startingWall.SetActive(false);
		this.isRaceEndSoundEnabled = false;
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x00070CB8 File Offset: 0x0006EEB8
	public void OnRaceEnded()
	{
		this.finishLineText.text = "";
		this.lastDisplayedCountdown = 0;
		this.checkpoints.OnRaceEnd();
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x00070CDC File Offset: 0x0006EEDC
	public void OnRaceReset()
	{
		this.raceConsoleVisual.ShowCanStartRace();
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x00070CE9 File Offset: 0x0006EEE9
	public void EnableRaceEndSound()
	{
		this.isRaceEndSoundEnabled = true;
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00070CF2 File Offset: 0x0006EEF2
	public void OnCheckpointPassed(int index, SoundBankPlayer checkpointSound)
	{
		if (index == 0 && this.isRaceEndSoundEnabled)
		{
			this.countdownSoundPlayer.PlayOneShot(this.raceEndSound);
		}
		else
		{
			checkpointSound.Play();
		}
		RacingManager.instance.OnCheckpointPassed(this.raceId, index);
	}

	// Token: 0x04001A19 RID: 6681
	[SerializeField]
	private TextMeshPro finishLineText;

	// Token: 0x04001A1A RID: 6682
	[SerializeField]
	private TextMeshPro countdownText;

	// Token: 0x04001A1B RID: 6683
	[SerializeField]
	private RacingScoreboard[] raceScoreboards;

	// Token: 0x04001A1C RID: 6684
	[SerializeField]
	private RacingScoreboard raceStartScoreboard;

	// Token: 0x04001A1D RID: 6685
	[SerializeField]
	private RaceConsoleVisual raceConsoleVisual;

	// Token: 0x04001A1E RID: 6686
	private float nextVisualRefreshTimestamp;

	// Token: 0x04001A1F RID: 6687
	private RaceCheckpointManager checkpoints;

	// Token: 0x04001A20 RID: 6688
	[SerializeField]
	private AudioClip raceEndSound;

	// Token: 0x04001A21 RID: 6689
	[SerializeField]
	private float countdownSoundGoTime;

	// Token: 0x04001A22 RID: 6690
	[SerializeField]
	private AudioSource countdownSoundPlayer;

	// Token: 0x04001A23 RID: 6691
	[SerializeField]
	private GameObject startingWall;

	// Token: 0x04001A24 RID: 6692
	private int lastDisplayedCountdown;

	// Token: 0x04001A25 RID: 6693
	private bool isRaceEndSoundEnabled;
}
