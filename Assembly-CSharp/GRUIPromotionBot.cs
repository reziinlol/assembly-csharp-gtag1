using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200081D RID: 2077
public class GRUIPromotionBot : MonoBehaviourTick
{
	// Token: 0x06003559 RID: 13657 RVA: 0x001270D4 File Offset: 0x001252D4
	public string FormattedUserInfo()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return "ERROR";
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		string titleNameAndGrade = GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints);
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		int num2 = grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints + item2;
		string str = (player != null) ? player.SanitizedNickName : "RANDO MONKE";
		this.cachedStringBuilder.Clear();
		this.cachedStringBuilder.Append("<color=#808080>EMPLOYEE:</color>     " + str + "\n");
		this.cachedStringBuilder.Append("<color=#808080>TITLE:</color>        " + titleNameAndGrade + "\n");
		this.cachedStringBuilder.Append(string.Format("<color=#808080>XP:</color>           {0}/{1}\n", num2, item));
		if (grplayer == GRPlayer.GetLocal())
		{
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDITS:</color>      <color=#00ff00>⑭ {0}</color>\n", grplayer.ShiftCredits));
			this.cachedStringBuilder.Append(string.Format("<color=#808080>CREDIT LIMIT:</color> <color=#00a000>⑭ {0}</color>\n", num));
			if (this.reactor != null && this.reactor.toolProgression != null)
			{
				int numberOfResearchPoints = this.reactor.toolProgression.GetNumberOfResearchPoints();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>JUICE:</color>        <color=purple>⑮ {0}</color>\n", numberOfResearchPoints));
			}
			if (ProgressionManager.Instance != null)
			{
				int shinyRocksTotal = ProgressionManager.Instance.GetShinyRocksTotal();
				this.cachedStringBuilder.Append(string.Format("<color=#808080>SHINY ROCKS:</color>  <color=white>⑯ {0}</color>\n", shinyRocksTotal));
			}
		}
		return this.cachedStringBuilder.ToString();
	}

	// Token: 0x0600355A RID: 13658 RVA: 0x001272C8 File Offset: 0x001254C8
	public bool ActivePlayerEligibleForPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return false;
		}
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(grplayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		return item - item2 < grplayer.CurrentProgression.points - grplayer.CurrentProgression.redeemedPoints;
	}

	// Token: 0x0600355B RID: 13659 RVA: 0x00127328 File Offset: 0x00125528
	public void Init(GhostReactor _reactor)
	{
		this.reactor = _reactor;
		this.currentPlayerActorNumber = -1;
		this.currentState = GRUIPromotionBot.PromotionBotState.WaitingForLogin;
	}

	// Token: 0x0600355C RID: 13660 RVA: 0x0012733F File Offset: 0x0012553F
	public void Refresh()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x0600355D RID: 13661 RVA: 0x00127348 File Offset: 0x00125548
	public override void Tick()
	{
		if (this.reactor == null || this.reactor.grManager == null || !this.reactor.grManager.IsAuthority())
		{
			return;
		}
		float time = Time.time;
		if (this.currentPlayerActorNumber != -1 && (this.timeLastDistanceCheck > time || time > this.timeLastDistanceCheck + this.timeBetweenDistanceChecks))
		{
			GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
			if (grplayer == null || (base.transform.position - grplayer.transform.position).sqrMagnitude > this.distanceForAutoLogout * this.distanceForAutoLogout)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
			}
		}
	}

	// Token: 0x0600355E RID: 13662 RVA: 0x00127400 File Offset: 0x00125600
	public bool CheckIsActivePlayer()
	{
		Object x = GRPlayer.Get(VRRig.LocalRig);
		GRPlayer y = GRPlayer.Get(this.currentPlayerActorNumber);
		return x == y;
	}

	// Token: 0x0600355F RID: 13663 RVA: 0x0012742C File Offset: 0x0012562C
	public void UpPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits)
			{
				this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePromotion, false);
		}
	}

	// Token: 0x06003560 RID: 13664 RVA: 0x00127464 File Offset: 0x00125664
	public void DownPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState == GRUIPromotionBot.PromotionBotState.ChoosePromotion)
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease, false);
			return;
		}
		if (promotionBotState != GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease)
		{
			return;
		}
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x06003561 RID: 13665 RVA: 0x0012749C File Offset: 0x0012569C
	public void YesPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		switch (this.currentState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.AttemptPromotion();
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.AttemptPurchaseShiftCreditIncrease();
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits, false);
			return;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003562 RID: 13666 RVA: 0x001274F4 File Offset: 0x001256F4
	public void NoPressed()
	{
		if (!this.CheckIsActivePlayer())
		{
			return;
		}
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		if (promotionBotState - GRUIPromotionBot.PromotionBotState.ChoosePromotion > 2)
		{
			if (promotionBotState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
			{
				this.AttemptPurchaseShiftCreditRefillToMax();
				return;
			}
		}
		else
		{
			this.SwitchState(GRUIPromotionBot.PromotionBotState.WaitingForLogin, false);
		}
	}

	// Token: 0x06003563 RID: 13667 RVA: 0x0012752C File Offset: 0x0012572C
	public void SwitchState(GRUIPromotionBot.PromotionBotState newState, bool fromRPC = false)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		GRPlayer grplayer2 = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer2 == null)
		{
			return;
		}
		this.RefreshPlayerData();
		GRUIPromotionBot.PromotionBotState promotionBotState = this.currentState;
		this.currentState = newState;
		this.SetScreenVisibility();
		this.SetMenuText(newState);
		switch (newState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			if (this.ActivePlayerEligibleForPromotion())
			{
				this.descriptionText.text = "<color=#c0c0c0>     YOU ARE ELIGIBLE FOR A PROMOTION!\n     PRESS 'YES' TO CONTINUE</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c04040>     YOU ARE NOT ELIGIBLE FOR A PROMOTION\n     EARN MORE XP BY COMPLETING SHIFT GOALS</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			if (grplayer.ShiftCreditCapIncreases != grplayer.ShiftCreditCapIncreasesMax)
			{
				this.descriptionText.text = "<color=#c0c0c0>     INCREASE CREDIT LIMIT BY <color=#00ff00>⑭ 100</color>\n     FOR <color=purple>⑮ 2</color> JUICE?</color>";
			}
			else
			{
				this.descriptionText.text = "<color=#c0c0c0>     CREDIT LIMIT CAN'T BE INCREASED AT THIS TIME\n</color>";
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
			if (grplayer == null)
			{
				this.descriptionText.text = "No active player";
			}
			else
			{
				int purchaseToCreditCapAmount = this.GetPurchaseToCreditCapAmount();
				if (purchaseToCreditCapAmount > 0)
				{
					this.descriptionText.text = string.Format("<color=#c0c0c0>     PURCHASE <color=#00ff00>+⑭{0}</color> CREDITS\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount);
				}
				else
				{
					this.descriptionText.text = "<color=#c0c0c0>     YOU ARE AT FULL CREDITS";
				}
			}
			break;
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
		{
			int purchaseToCreditCapAmount2 = this.GetPurchaseToCreditCapAmount();
			this.descriptionText.text = string.Format("<color=#c0c0c0>     CONFIRM PURCHASE <color=#00ff00>+⑭{0}</color>\n     FOR <color=white>100 SHINY ROCKS?</color>", purchaseToCreditCapAmount2);
			break;
		}
		}
		if (this.currentState == GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits)
		{
			this.yesText.text = "<size=0.4>CANCEL</size>";
			this.noText.text = "<size=0.4>CONFIRM</size>";
		}
		else
		{
			if (this.yesText.text != "YES")
			{
				this.yesText.text = "YES";
			}
			if (this.noText.text != "NO")
			{
				this.noText.text = "NO";
			}
		}
		if (this.reactor != null && this.reactor.grManager != null && !fromRPC && (grplayer == grplayer2 || this.reactor.grManager.IsAuthority()))
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest((int)this.currentState);
		}
	}

	// Token: 0x06003564 RID: 13668 RVA: 0x0012774C File Offset: 0x0012594C
	public int GetPurchaseToCreditCapAmount()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		int shiftCredits = grplayer.ShiftCredits;
		int num = 1000 + grplayer.ShiftCreditCapIncreases * 100;
		return Math.Max(0, num - shiftCredits);
	}

	// Token: 0x06003565 RID: 13669 RVA: 0x00127788 File Offset: 0x00125988
	public void CelebratePromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			return;
		}
		this.particlesGO.SetActive(false);
		this.particlesGO.SetActive(true);
		this.levelUpSound.Play();
		this.popSound.Play();
		PlayerGameEvents.MiscEvent(GRUIPromotionBot.EVENT_PROMOTED, 1);
		grplayer.SendRankUpTelemetry(GhostReactorProgression.GetTitleNameAndGrade(grplayer.CurrentProgression.redeemedPoints));
	}

	// Token: 0x06003566 RID: 13670 RVA: 0x001277FC File Offset: 0x001259FC
	public void SetMenuText(GRUIPromotionBot.PromotionBotState menuState)
	{
		switch (menuState)
		{
		case GRUIPromotionBot.PromotionBotState.ChoosePromotion:
			this.menuText.text = "-> REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChooseCreditIncrease:
			this.menuText.text = "   REQUEST PROMOTION\n-> INCREASE CREDIT LIMIT\n   BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		case GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits:
		case GRUIPromotionBot.PromotionBotState.ConfirmPurchaseCredits:
			this.menuText.text = "   REQUEST PROMOTION\n   INCREASE CREDIT LIMIT\n-> BRIBE ACCOUNTING FOR CREDITS\n";
			return;
		default:
			return;
		}
	}

	// Token: 0x06003567 RID: 13671 RVA: 0x00127854 File Offset: 0x00125A54
	public void SetScreenVisibility()
	{
		this.startScreenText.gameObject.SetActive(this.currentState == GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.userInfo.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.menuText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.descriptionText.gameObject.SetActive(this.currentState > GRUIPromotionBot.PromotionBotState.WaitingForLogin);
		this.purchaseSuccessText.gameObject.SetActive(false);
	}

	// Token: 0x06003568 RID: 13672 RVA: 0x001278D6 File Offset: 0x00125AD6
	public void RefreshPlayerData()
	{
		this.userInfo.text = this.FormattedUserInfo();
	}

	// Token: 0x06003569 RID: 13673 RVA: 0x001278EC File Offset: 0x00125AEC
	public void OnPurchaseCallback(bool success)
	{
		if (success)
		{
			this.purchaseSuccessText.text = "<color=#80ff80>     PURCHASE SUCCEEDED!</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else
		{
			this.purchaseSuccessText.text = "<color=#ff8080>     FAILED PURCHASE. NO CHARGE.</color>";
			this.RefreshPlayerData();
			this.purchaseSuccessText.gameObject.SetActive(true);
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x0012733F File Offset: 0x0012553F
	public void OnJuiceUpdated()
	{
		this.RefreshPlayerData();
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x00127978 File Offset: 0x00125B78
	public void OnGetShiftCredit(string mothershipId, int credit)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x001279B0 File Offset: 0x00125BB0
	public void OnShinyRocksUpdated()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.gamePlayer.IsLocal())
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x001279E8 File Offset: 0x00125BE8
	public void OnGetShiftCreditCapData(string mothershipId, int creditCap, int creditCapMax)
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && grplayer.mothershipId == mothershipId)
		{
			this.RefreshPlayerData();
		}
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x00127A20 File Offset: 0x00125C20
	public void AttemptPromotion()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer && grplayer.AttemptPromotion() && this.reactor != null && this.reactor.grManager != null)
		{
			this.CelebratePromotion();
			this.RefreshPlayerData();
			this.RefreshActivePlayerBadge();
			string titleName = GhostReactorProgression.GetTitleName(grplayer.CurrentProgression.redeemedPoints);
			int grade = GhostReactorProgression.GetGrade(grplayer.CurrentProgression.redeemedPoints);
			this.purchaseSuccessText.text = string.Format("CONGRATULATIONS, {0} {1}!", titleName, grade);
			this.purchaseSuccessText.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x00127AD4 File Offset: 0x00125CD4
	public void AttemptPurchaseShiftCreditIncrease()
	{
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		if (grplayer.ShiftCreditCapIncreases == grplayer.ShiftCreditCapIncreasesMax)
		{
			return;
		}
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCreditCapIncreases {0} ShiftCreditCapIncreasesMax {1}", grplayer.ShiftCreditCapIncreases, grplayer.ShiftCreditCapIncreasesMax));
		int num = 2;
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCreditCapIncreases < grplayer.ShiftCreditCapIncreasesMax && this.reactor.toolProgression.GetNumberOfResearchPoints() >= num && ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.PurchaseShiftCreditCapIncrease();
		}
		this.RefreshPlayerData();
	}

	// Token: 0x06003570 RID: 13680 RVA: 0x00127B90 File Offset: 0x00125D90
	public void AttemptPurchaseShiftCreditRefillToMax()
	{
		if (this.GetPurchaseToCreditCapAmount() == 0)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer == null)
		{
			Debug.Log("AttemptPurchaseShiftCreditIncrease currentPlayer null");
			return;
		}
		int num = 1000;
		int num2 = 100;
		int num3 = num + grplayer.ShiftCreditCapIncreases * num2;
		Debug.Log(string.Format("AttemptPurchaseShiftCreditIncrease currentPlayer ShiftCredits {0} ShiftCreditMax {1}", grplayer.ShiftCredits, num3));
		if (grplayer != null && grplayer.gamePlayer.IsLocal() && grplayer.ShiftCredits < num3)
		{
			int num4 = 100;
			if (ProgressionManager.Instance != null && ProgressionManager.Instance.GetShinyRocksTotal() >= num4)
			{
				ProgressionManager.Instance.PurchaseShiftCredit();
			}
		}
		this.RefreshPlayerData();
		this.SwitchState(GRUIPromotionBot.PromotionBotState.ChoosePurchaseCredits, false);
	}

	// Token: 0x06003571 RID: 13681 RVA: 0x00127C4C File Offset: 0x00125E4C
	public void PlayerSwipedID()
	{
		if (this.reactor == null || this.reactor.grManager == null)
		{
			return;
		}
		if (this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded == null)
			{
				return;
			}
			onSucceeded.Invoke();
			return;
		}
		else if (this.currentPlayerActorNumber != -1 && GRPlayer.Get(this.currentPlayerActorNumber) != null)
		{
			UnityEvent onFailed = this.scanner.onFailed;
			if (onFailed == null)
			{
				return;
			}
			onFailed.Invoke();
			return;
		}
		else
		{
			this.reactor.grManager.PromotionBotActivePlayerRequest(6);
			UnityEvent onSucceeded2 = this.scanner.onSucceeded;
			if (onSucceeded2 == null)
			{
				return;
			}
			onSucceeded2.Invoke();
			return;
		}
	}

	// Token: 0x06003572 RID: 13682 RVA: 0x00127CFC File Offset: 0x00125EFC
	public void RefreshActivePlayerBadge()
	{
		if (this.currentPlayerActorNumber == -1)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(this.currentPlayerActorNumber);
		if (grplayer != null && this.currentPlayerActorNumber != -1)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(this.currentPlayerActorNumber);
			if (netPlayerByID != null && grplayer.badge != null)
			{
				grplayer.badge.RefreshText(netPlayerByID);
			}
		}
	}

	// Token: 0x06003573 RID: 13683 RVA: 0x00127D60 File Offset: 0x00125F60
	public void SetActivePlayerStateChange(int actorNumber, int state)
	{
		if (state == 0)
		{
			this.RefreshActivePlayerBadge();
			actorNumber = -1;
		}
		bool flag = this.currentPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		bool flag2 = actorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		if (flag && !flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit -= this.OnPurchaseCallback;
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease -= this.OnPurchaseCallback;
				ProgressionManager.Instance.OnInventoryUpdated -= this.OnJuiceUpdated;
				ProgressionManager.Instance.OnGetShiftCredit -= this.OnGetShiftCredit;
				ProgressionManager.Instance.OnGetShiftCreditCapData -= this.OnGetShiftCreditCapData;
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance = CosmeticsController.instance;
				instance.OnGetCurrency = (Action)Delegate.Remove(instance.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		else if (!flag && flag2)
		{
			if (ProgressionManager.Instance != null)
			{
				ProgressionManager.Instance.OnPurchaseShiftCredit += this.OnPurchaseCallback;
				ProgressionManager.Instance.OnPurchaseShiftCreditCapIncrease += this.OnPurchaseCallback;
				ProgressionManager.Instance.OnInventoryUpdated += this.OnJuiceUpdated;
				ProgressionManager.Instance.OnGetShiftCredit += this.OnGetShiftCredit;
				ProgressionManager.Instance.OnGetShiftCreditCapData += this.OnGetShiftCreditCapData;
			}
			if (CosmeticsController.instance != null)
			{
				CosmeticsController instance2 = CosmeticsController.instance;
				instance2.OnGetCurrency = (Action)Delegate.Combine(instance2.OnGetCurrency, new Action(this.OnShinyRocksUpdated));
			}
		}
		this.currentPlayerActorNumber = actorNumber;
		this.SwitchState((GRUIPromotionBot.PromotionBotState)state, true);
	}

	// Token: 0x06003574 RID: 13684 RVA: 0x00127F2C File Offset: 0x0012612C
	public int GetCurrentPlayerActorNumber()
	{
		return this.currentPlayerActorNumber;
	}

	// Token: 0x040045E3 RID: 17891
	private static string EVENT_PROMOTED = "GRPromoted";

	// Token: 0x040045E4 RID: 17892
	private GhostReactor reactor;

	// Token: 0x040045E5 RID: 17893
	public TMP_Text startScreenText;

	// Token: 0x040045E6 RID: 17894
	public TMP_Text userInfo;

	// Token: 0x040045E7 RID: 17895
	public TMP_Text menuText;

	// Token: 0x040045E8 RID: 17896
	public TMP_Text descriptionText;

	// Token: 0x040045E9 RID: 17897
	public TMP_Text yesText;

	// Token: 0x040045EA RID: 17898
	public TMP_Text noText;

	// Token: 0x040045EB RID: 17899
	public TMP_Text purchaseSuccessText;

	// Token: 0x040045EC RID: 17900
	public IDCardScanner scanner;

	// Token: 0x040045ED RID: 17901
	public GameObject particlesGO;

	// Token: 0x040045EE RID: 17902
	public AudioSource levelUpSound;

	// Token: 0x040045EF RID: 17903
	public AudioSource popSound;

	// Token: 0x040045F0 RID: 17904
	private string defaultText = "-N/A-\n-N/A-\n-N/A-\n-N/A-\n-N/A-\n\n-N/A-";

	// Token: 0x040045F1 RID: 17905
	private string promotionTextStr1 = "CONGRATULATIONS\n ";

	// Token: 0x040045F2 RID: 17906
	private string promotionTextStr2 = ".\n\nYOU ARE NOW A GRADE ";

	// Token: 0x040045F3 RID: 17907
	private string promotionTextStr3 = ".\n\nYOU MAY TAKE TWO UNPAID MINUTES TO CELEBRATE, THEN RETURN TO WORK.";

	// Token: 0x040045F4 RID: 17908
	private string inertButtonText = "-";

	// Token: 0x040045F5 RID: 17909
	private string buttonReturnText = "-RETURN-";

	// Token: 0x040045F6 RID: 17910
	private string requestPromotionText = "REQUEST PROMOTION";

	// Token: 0x040045F7 RID: 17911
	public const string newLine = "\n";

	// Token: 0x040045F8 RID: 17912
	public int currentPlayerActorNumber;

	// Token: 0x040045F9 RID: 17913
	public GRUIPromotionBot.PromotionBotState currentState;

	// Token: 0x040045FA RID: 17914
	public float timeOutTime;

	// Token: 0x040045FB RID: 17915
	public float distanceForAutoLogout = 2.5f;

	// Token: 0x040045FC RID: 17916
	private StringBuilder cachedStringBuilder = new StringBuilder(512);

	// Token: 0x040045FD RID: 17917
	private float timeLastDistanceCheck;

	// Token: 0x040045FE RID: 17918
	private float timeBetweenDistanceChecks = 0.5f;

	// Token: 0x0200081E RID: 2078
	public enum PromotionBotState
	{
		// Token: 0x04004600 RID: 17920
		WaitingForLogin,
		// Token: 0x04004601 RID: 17921
		ChoosePromotion,
		// Token: 0x04004602 RID: 17922
		ChooseCreditIncrease,
		// Token: 0x04004603 RID: 17923
		ChoosePurchaseCredits,
		// Token: 0x04004604 RID: 17924
		ConfirmPurchaseCredits,
		// Token: 0x04004605 RID: 17925
		CelebratePromotion,
		// Token: 0x04004606 RID: 17926
		TryingLogIn
	}
}
