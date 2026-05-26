using System;
using GameObjectScheduling;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200092F RID: 2351
public class ModeSelectButton : GorillaPressableButton
{
	// Token: 0x17000589 RID: 1417
	// (get) Token: 0x06003D8B RID: 15755 RVA: 0x0014D959 File Offset: 0x0014BB59
	// (set) Token: 0x06003D8C RID: 15756 RVA: 0x0014D961 File Offset: 0x0014BB61
	public PartyGameModeWarning WarningScreen
	{
		get
		{
			return this.warningScreen;
		}
		set
		{
			this.warningScreen = value;
		}
	}

	// Token: 0x06003D8D RID: 15757 RVA: 0x0014D96A File Offset: 0x0014BB6A
	public override void Start()
	{
		base.Start();
		GorillaComputer.instance.currentGameMode.AddCallback(new Action<string>(this.OnGameModeChanged), true);
	}

	// Token: 0x06003D8E RID: 15758 RVA: 0x0014D990 File Offset: 0x0014BB90
	private void OnDestroy()
	{
		if (!ApplicationQuittingState.IsQuitting)
		{
			GorillaComputer.instance.currentGameMode.RemoveCallback(new Action<string>(this.OnGameModeChanged));
		}
	}

	// Token: 0x06003D8F RID: 15759 RVA: 0x0014D9B6 File Offset: 0x0014BBB6
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		if (this.warningScreen.ShouldShowWarning)
		{
			this.warningScreen.Show();
			return;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(this.gameMode, isLeftHand);
	}

	// Token: 0x06003D90 RID: 15760 RVA: 0x0014D9EB File Offset: 0x0014BBEB
	public void OnGameModeChanged(string newGameMode)
	{
		this.buttonRenderer.material = ((newGameMode.ToLower() == this.gameMode.ToLower()) ? this.pressedMaterial : this.unpressedMaterial);
	}

	// Token: 0x06003D91 RID: 15761 RVA: 0x0014DA20 File Offset: 0x0014BC20
	public void SetInfo(string Mode, string ModeTitle, bool NewMode, CountdownTextDate CountdownTo)
	{
		this.gameModeTitle.text = ModeTitle;
		this.gameMode = Mode;
		this.newModeSplash.SetActive(NewMode);
		this.limitedCountdown.gameObject.SetActive(false);
		if (CountdownTo == null)
		{
			return;
		}
		this.limitedCountdown.Countdown = CountdownTo;
		this.limitedCountdown.gameObject.SetActive(true);
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x0014DA86 File Offset: 0x0014BC86
	public void HideNewAndLimitedTimeInfo()
	{
		this.limitedCountdown.gameObject.SetActive(false);
		this.newModeSplash.SetActive(false);
	}

	// Token: 0x04004DE8 RID: 19944
	[SerializeField]
	public string gameMode;

	// Token: 0x04004DE9 RID: 19945
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x04004DEA RID: 19946
	[SerializeField]
	private TMP_Text gameModeTitle;

	// Token: 0x04004DEB RID: 19947
	[SerializeField]
	private GameObject newModeSplash;

	// Token: 0x04004DEC RID: 19948
	[SerializeField]
	private CountdownText limitedCountdown;
}
