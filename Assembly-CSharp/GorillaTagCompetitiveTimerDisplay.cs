using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020008B1 RID: 2225
public class GorillaTagCompetitiveTimerDisplay : MonoBehaviour
{
	// Token: 0x06003A06 RID: 14854 RVA: 0x0013BAF0 File Offset: 0x00139CF0
	private void Awake()
	{
		this.prevTime = -1;
		if (this.waitingForPlayersBackground)
		{
			this.waitingForPlayersBackground.SetActive(true);
			this.currentBackground = this.waitingForPlayersBackground;
		}
		if (this.startCountdownBackground)
		{
			this.startCountdownBackground.SetActive(false);
		}
		if (this.playingBackground)
		{
			this.playingBackground.SetActive(false);
		}
		if (this.postRoundBackground)
		{
			this.postRoundBackground.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(false);
		if (this.timerDisplay2)
		{
			this.timerDisplay2.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003A07 RID: 14855 RVA: 0x0013BBA4 File Offset: 0x00139DA4
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.HandleOnTimeChanged;
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager != null)
		{
			this.HandleOnGameStateChanged(gorillaTagCompetitiveManager.GetCurrentGameState());
		}
		this.myRig = base.GetComponentInParent<VRRig>();
		this.DisplayStandardTimer(false);
	}

	// Token: 0x06003A08 RID: 14856 RVA: 0x0013BC06 File Offset: 0x00139E06
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.HandleOnGameStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.HandleOnTimeChanged;
	}

	// Token: 0x06003A09 RID: 14857 RVA: 0x0013BC2C File Offset: 0x00139E2C
	private void HandleOnGameStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		this.SetNewBackground(newState);
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.DisplayStandardTimer(false);
			this.resultsDisplay.gameObject.SetActive(false);
			return;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.DisplayStandardTimer(true);
			return;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.DoPostRoundShow();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003A0A RID: 14858 RVA: 0x0013BC80 File Offset: 0x00139E80
	private void DisplayStandardTimer(bool bShow)
	{
		if (bShow)
		{
			this.resultsDisplay.gameObject.SetActive(false);
		}
		this.timerDisplay.gameObject.SetActive(bShow);
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.gameObject.SetActive(bShow);
		}
	}

	// Token: 0x06003A0B RID: 14859 RVA: 0x0013BCD4 File Offset: 0x00139ED4
	private void DoPostRoundShow()
	{
		GorillaTagCompetitiveManager gorillaTagCompetitiveManager = GorillaGameManager.instance as GorillaTagCompetitiveManager;
		if (gorillaTagCompetitiveManager == null)
		{
			return;
		}
		this.DisplayStandardTimer(false);
		this.resultsDisplay.gameObject.SetActive(true);
		List<VRRig> list = new List<VRRig>();
		List<RankedMultiplayerScore.PlayerScoreInRound> sortedScores = gorillaTagCompetitiveManager.GetScoring().GetSortedScores();
		float b = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[0].NumTags, sortedScores[0].PointsOnDefense);
		int num = 0;
		while (num < sortedScores.Count && num < 3)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(sortedScores[num].PlayerId, out rigContainer))
			{
				float a = gorillaTagCompetitiveManager.GetScoring().ComputeGameScore(sortedScores[num].NumTags, sortedScores[num].PointsOnDefense);
				if (num == 0 || a.Approx(b, 0.01f))
				{
					list.Add(rigContainer.Rig);
				}
				switch (num)
				{
				case 0:
					if (this.tintableCelebration != null)
					{
						Color playerColor = rigContainer.Rig.playerColor;
						float h;
						float s;
						float num2;
						Color.RGBToHSV(playerColor, out h, out s, out num2);
						Color max = Color.HSVToRGB(h, s, (num2 < 0.5f) ? (num2 + 0.5f) : (num2 - 0.5f));
						this.tintableCelebration.main.startColor = new ParticleSystem.MinMaxGradient(playerColor, max);
						this.tintableCelebration.gameObject.SetActive(true);
					}
					if (this.goldCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.goldCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 1:
					if (this.silverCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.silverCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				case 2:
					if (this.bronzeCelebration != null && rigContainer.Rig == this.myRig)
					{
						this.bronzeCelebration.gameObject.SetActive(true);
					}
					if (this.celebrationAudio != null)
					{
						this.celebrationAudio.Play();
					}
					break;
				}
			}
			num++;
		}
		for (int i = 0; i < this.postRoundTimerText.Length; i++)
		{
			this.postRoundTimerText[i].text = ((list.Count > 1) ? "SHARED WIN" : "WINNER");
		}
		string text = string.Empty;
		for (int j = 0; j < list.Count; j++)
		{
			text = text + list[j].playerText1.text.ToUpper() + "\n";
		}
		this.resultsDisplay.text = text.Trim();
		if (this.timerDisplay2 != null)
		{
			this.timerDisplay2.text = this.resultsDisplay.text;
		}
	}

	// Token: 0x06003A0C RID: 14860 RVA: 0x0013BFFC File Offset: 0x0013A1FC
	private void HandleOnTimeChanged(float time)
	{
		int num = Mathf.CeilToInt(time);
		num = Mathf.Max(num, 1);
		if (this.prevTime != num)
		{
			this.prevTime = num;
			if (this.currentState == GorillaTagCompetitiveManager.GameState.Playing)
			{
				int num2 = this.prevTime / 60;
				int num3 = this.prevTime % 60;
				this.timerDisplay.text = string.Format("{0}:{1:D2}", num2, num3);
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = string.Format("{0}:{1:D2}", num2, num3);
					return;
				}
			}
			else if (this.currentState != GorillaTagCompetitiveManager.GameState.PostRound)
			{
				this.timerDisplay.text = this.prevTime.ToString("#00");
				if (this.timerDisplay2)
				{
					this.timerDisplay2.text = this.prevTime.ToString("#00");
				}
			}
		}
	}

	// Token: 0x06003A0D RID: 14861 RVA: 0x0013C0E4 File Offset: 0x0013A2E4
	private void SetNewBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		if (this.currentBackground != null)
		{
			this.currentBackground.SetActive(false);
		}
		this.currentState = newState;
		GameObject x = this.SelectBackground(newState);
		this.GetTextColor(newState);
		this.currentBackground = null;
		if (x != null)
		{
			this.currentBackground = x;
			this.currentBackground.SetActive(true);
		}
	}

	// Token: 0x06003A0E RID: 14862 RVA: 0x0013C145 File Offset: 0x0013A345
	private GameObject SelectBackground(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			return this.waitingForPlayersBackground;
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.startCountdownBackground;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.playingBackground;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.postRoundBackground;
		default:
			return null;
		}
	}

	// Token: 0x06003A0F RID: 14863 RVA: 0x0013C17E File Offset: 0x0013A37E
	private Color GetTextColor(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.StartingCountdown:
			return this.timerColorStart;
		case GorillaTagCompetitiveManager.GameState.Playing:
			return this.timerColorPlaying;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			return this.timerColorPostRound;
		default:
			return Color.white;
		}
	}

	// Token: 0x040049F3 RID: 18931
	public TextMeshPro timerDisplay;

	// Token: 0x040049F4 RID: 18932
	public TextMeshPro timerDisplay2;

	// Token: 0x040049F5 RID: 18933
	public TextMeshPro resultsDisplay;

	// Token: 0x040049F6 RID: 18934
	public GameObject waitingForPlayersBackground;

	// Token: 0x040049F7 RID: 18935
	public GameObject startCountdownBackground;

	// Token: 0x040049F8 RID: 18936
	public Color timerColorStart = Color.white;

	// Token: 0x040049F9 RID: 18937
	public GameObject playingBackground;

	// Token: 0x040049FA RID: 18938
	public Color timerColorPlaying = Color.white;

	// Token: 0x040049FB RID: 18939
	public GameObject postRoundBackground;

	// Token: 0x040049FC RID: 18940
	public Color timerColorPostRound = Color.white;

	// Token: 0x040049FD RID: 18941
	public TextMeshPro[] postRoundTimerText;

	// Token: 0x040049FE RID: 18942
	private GorillaTagCompetitiveManager.GameState currentState;

	// Token: 0x040049FF RID: 18943
	private GameObject currentBackground;

	// Token: 0x04004A00 RID: 18944
	private int prevTime = -1;

	// Token: 0x04004A01 RID: 18945
	[SerializeField]
	private ParticleSystem tintableCelebration;

	// Token: 0x04004A02 RID: 18946
	[SerializeField]
	private ParticleSystem goldCelebration;

	// Token: 0x04004A03 RID: 18947
	[SerializeField]
	private ParticleSystem silverCelebration;

	// Token: 0x04004A04 RID: 18948
	[SerializeField]
	private ParticleSystem bronzeCelebration;

	// Token: 0x04004A05 RID: 18949
	private VRRig myRig;

	// Token: 0x04004A06 RID: 18950
	[SerializeField]
	private AudioSource celebrationAudio;
}
