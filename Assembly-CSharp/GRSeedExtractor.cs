using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x020007D1 RID: 2001
public class GRSeedExtractor : MonoBehaviour
{
	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x06003303 RID: 13059 RVA: 0x0011755F File Offset: 0x0011575F
	public bool StationOpen
	{
		get
		{
			return this.stationOpen;
		}
	}

	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06003304 RID: 13060 RVA: 0x00117567 File Offset: 0x00115767
	public bool StationOpenForLocalPlayer
	{
		get
		{
			return this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x06003305 RID: 13061 RVA: 0x0011758A File Offset: 0x0011578A
	public int CurrentPlayerActorNumber
	{
		get
		{
			return this.currentPlayerActorNumber;
		}
	}

	// Token: 0x06003306 RID: 13062 RVA: 0x00117594 File Offset: 0x00115794
	private void Awake()
	{
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		this.coreDepositTriggerNotifier.TriggerEnterEvent += this.DepositorTriggerEntered;
		this.idCardScanner.OnPlayerCardSwipe += this.OnPlayerCardSwipe;
		for (int i = 0; i < this.maxVisualChaosSeedCount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.chaosSeedVisualPrefab, base.transform);
			gameObject.SetActive(false);
			this.chaosSeedVisuals.Add(gameObject);
		}
		this.UpdateOverdrivePurchaseButtons();
		base.enabled = false;
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x00117640 File Offset: 0x00115840
	public void Init(GRToolProgressionManager progression, GhostReactor gr)
	{
		this.ghostReactor = gr;
		this.toolProgressionManager = progression;
		this.toolProgressionManager.OnProgressionUpdated += this.OnResearchPointsUpdated;
		ProgressionManager.Instance.OnJucierStatusUpdated += this.OnPlayerStatusReceived;
		ProgressionManager.Instance.OnPurchaseOverdrive += this.OnPurchaseOverdrive;
		ProgressionManager.Instance.OnChaosDepositSuccess += this.TryDepositSeedServerResponse;
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x06003309 RID: 13065 RVA: 0x001176B4 File Offset: 0x001158B4
	private void OnDisable()
	{
		this.ClearSeedVisuals();
		this.machineHumAudioSource.gameObject.SetActive(false);
		this.juicerSlowParticles.gameObject.SetActive(false);
		base.StopAllCoroutines();
		for (int i = 0; i < this.disableDuringOverdrive.Count; i++)
		{
			this.disableDuringOverdrive[i].gameObject.SetActive(true);
		}
		for (int j = 0; j < this.enableDuringOverdrive.Count; j++)
		{
			this.enableDuringOverdrive[j].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		this.processingAmount = 0f;
		this.processingAmountVisual = 0f;
		this.overdriveAmount = 0f;
		this.overdriveAmountVisual = 0f;
		this.currentPlayerData = default(GRSeedExtractor.PlayerData);
		this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
		this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x00117800 File Offset: 0x00115A00
	private void Update()
	{
		this.ValidateCurrentPlayer();
		if (this.stationOpen && this.shutterDoorOpenAmount < 1f)
		{
			float num = Time.time - this.currentPlayerData.latestRefreshTime;
			if (Time.time - this.stationOpenRequestTime >= 1f || num <= 5f)
			{
				float num2 = 1f / this.shutterDoorAnimTime;
				this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 1f, num2 * Time.deltaTime);
				Vector3 localPosition = this.shutterDoorParent.transform.localPosition;
				localPosition.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
				this.shutterDoorParent.transform.localPosition = localPosition;
			}
		}
		else if (!this.stationOpen && this.shutterDoorOpenAmount > 0f)
		{
			float num3 = 1f / this.shutterDoorAnimTime;
			this.shutterDoorOpenAmount = Mathf.MoveTowards(this.shutterDoorOpenAmount, 0f, num3 * Time.deltaTime);
			Vector3 localPosition2 = this.shutterDoorParent.transform.localPosition;
			localPosition2.y = Mathf.Lerp(this.shutterDoorLiftRange.x, this.shutterDoorLiftRange.y, this.shutterDoorOpenAmount);
			this.shutterDoorParent.transform.localPosition = localPosition2;
			if (this.shutterDoorOpenAmount <= 0f)
			{
				this.processingAmount = 0f;
				this.overdriveAmount = 0f;
			}
		}
		bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
		if (this.overdriveActive)
		{
			this.overdriveLightSpinnerOn.Rotate(Vector3.forward, 360f * this.overdriveLightSpinRate * Time.deltaTime, Space.Self);
			this.overdriveAmountVisual = this.overdriveAmount;
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			this.processingAmountVisual = this.processingAmount;
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		else
		{
			float num4 = 1f / this.overdriveFillTime;
			if (flag || this.overdriveAmount > this.overdriveAmountVisual || !this.stationOpen)
			{
				this.overdriveAmountVisual = Mathf.MoveTowards(this.overdriveAmountVisual, this.overdriveAmount, num4 * Time.deltaTime);
			}
			this.overdriveLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.overdriveAmountVisual), 1f);
			if (this.stationOpen)
			{
				float num5 = Mathf.Max(Time.time - this.currentPlayerData.latestRefreshTime, 0f);
				float num6 = this.currentPlayerData.coreProcessingPercentage + num5 / this.PROCESSING_TIME_SECONDS;
				this.processingAmount = Mathf.Clamp01(num6);
				this.estimatedJuiceTimeRemaining = (1f - this.processingAmount) * this.PROCESSING_TIME_SECONDS;
				if (this.StationOpenForLocalPlayer && num6 >= 1f && Time.time - this.lastServerRequestTime > this.timeBetweenServerRequests)
				{
					this.lastServerRequestTime = Time.time;
					ProgressionManager.Instance.GetJuicerStatus();
				}
			}
			if (flag)
			{
				this.machineHumAudioSource.gameObject.SetActive(true);
				this.juicerSlowParticles.gameObject.SetActive(true);
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, this.processingAmount, num4 * Time.deltaTime);
			}
			else
			{
				this.processingAmountVisual = Mathf.MoveTowards(this.processingAmountVisual, 0f, num4 * Time.deltaTime);
				this.machineHumAudioSource.gameObject.SetActive(false);
				this.juicerSlowParticles.gameObject.SetActive(false);
			}
			this.processingLiquidScaleParent.transform.localScale = new Vector3(1f, Mathf.Clamp01(this.processingAmountVisual), 1f);
		}
		this.StepSeedVisualAnimation(Time.deltaTime);
		this.UpdateScreenDisplay();
		if (!this.stationOpen && this.shutterDoorOpenAmount <= 0f && this.overdriveAmountVisual <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x00117C58 File Offset: 0x00115E58
	private void ValidateCurrentPlayer()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.CloseStation();
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && this.stationOpen)
		{
			bool flag = false;
			NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				float num = 5f;
				if (rigContainer.Rig != null && rigContainer.Rig.OwningNetPlayer == player && (rigContainer.Rig.GetMouthPosition() - base.transform.position).sqrMagnitude < num * num)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
		}
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x00117D30 File Offset: 0x00115F30
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null && component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x00117D88 File Offset: 0x00115F88
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component != null && component.OwningNetPlayer != null)
		{
			if (component.OwningNetPlayer.ActorNumber == this.currentPlayerActorNumber && this.stationOpen && this.ghostReactor.grManager.IsAuthority() && NetworkSystem.Instance.InRoom)
			{
				this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorCloseStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			}
			if (component.OwningNetPlayer.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.localPlayerData = default(GRSeedExtractor.PlayerData);
			}
		}
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x00117E38 File Offset: 0x00116038
	public void OnPlayerCardSwipe(int playerActorNumber)
	{
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
			ProgressionManager.Instance.GetJuicerStatus();
		}
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x00117E90 File Offset: 0x00116090
	public void DepositorTriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (this.ghostReactor == null || this.ghostReactor.grManager == null || other == null || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if (this.ghostReactor.grManager.IsAuthority() && other.attachedRigidbody != null)
		{
			GRCollectible component = other.attachedRigidbody.GetComponent<GRCollectible>();
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
			if (managerForZone != null && component != null && component.type == ProgressionManager.CoreType.ChaosSeed)
			{
				int netIdFromEntityId = managerForZone.GetNetIdFromEntityId(component.entity.id);
				int lastHeldByActorNumber = component.entity.lastHeldByActorNumber;
				bool player = NetworkSystem.Instance.GetPlayer(lastHeldByActorNumber) != null;
				float time = Time.time;
				if (player)
				{
					bool flag = false;
					for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
					{
						if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
						{
							this.seedDepositsPending.RemoveAt(i);
						}
						else if (this.seedDepositsPending[i].Item1 == netIdFromEntityId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(netIdFromEntityId, lastHeldByActorNumber, Time.time, false));
						this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorTryDepositSeed, lastHeldByActorNumber, netIdFromEntityId);
					}
				}
			}
		}
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x00118041 File Offset: 0x00116241
	public void OverdrivePurchaseButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
		}
		else if (this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchasePending = true;
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x0011806C File Offset: 0x0011626C
	private bool LocalPlayerCanPurchaseOverdrive()
	{
		if (Time.time - this.overdrivePurchaseTime > 5f)
		{
			this.overdriveServerConfirmationPending = false;
		}
		return this.StationOpenForLocalPlayer && !this.overdriveServerConfirmationPending && CosmeticsController.instance.CurrencyBalance >= 250 && this.localPlayerData.overdriveSupply <= 0f;
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x001180D4 File Offset: 0x001162D4
	public void OverdrivePurchaseConfirmButtonPressed()
	{
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchasePending = false;
			if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.overdriveServerConfirmationPending = true;
				this.overdrivePurchaseTime = Time.time;
				ProgressionManager.Instance.PurchaseOverdrive();
			}
		}
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x00118134 File Offset: 0x00116334
	public void OnPlayerStatusReceived(ProgressionManager.JuicerStatusResponse statusResponse)
	{
		if (statusResponse.MothershipId == GRPlayer.GetLocal().mothershipId && statusResponse.RefreshJuice)
		{
			this.toolProgressionManager.UpdateInventory();
		}
		this.PROCESSING_TIME_SECONDS = (float)statusResponse.CoreProcessingTimeSec;
		this.MAX_OVERDRIVE_USES = statusResponse.OverdriveCap / 100;
		float num = Mathf.Clamp01((float)statusResponse.OverdriveSupply / (float)statusResponse.OverdriveCap);
		int num2 = 0;
		bool flag = num < this.localPlayerData.overdriveSupply;
		bool flag2 = this.localPlayerData.overdriveSupply == 0f && this.localPlayerData.coreCount > statusResponse.CurrentCoreCount;
		if (statusResponse.CoresProcessedByOverdrive > 0 && (flag || flag2))
		{
			num2 = statusResponse.CoresProcessedByOverdrive;
		}
		this.localPlayerData.actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.localPlayerData.coreCount = statusResponse.CurrentCoreCount;
		this.localPlayerData.coreProcessingPercentage = Mathf.Clamp01(statusResponse.CoreProcessingPercent);
		this.localPlayerData.overdriveSupply = num;
		this.localPlayerData.coresProcessedByOverdrive = statusResponse.CoresProcessedByOverdrive;
		this.localPlayerData.coresPendingOverdriveProcessing = this.localPlayerData.coresPendingOverdriveProcessing + num2;
		this.localPlayerData.latestRefreshTime = Time.time;
		this.localPlayerData.researchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (this.overdriveServerConfirmationPending && (this.localPlayerData.overdriveSupply > 0f || this.localPlayerData.coresProcessedByOverdrive > 0))
		{
			this.overdriveServerConfirmationPending = false;
		}
		if (this.stationOpen && this.currentPlayerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber && NetworkSystem.Instance.InRoom)
		{
			this.currentPlayerData = this.localPlayerData;
			this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
			this.OnStateUpdated();
		}
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x00118334 File Offset: 0x00116534
	private void TryDepositSeedServerResponse(bool succeeded)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		int num = -1;
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		for (int i = 0; i < this.seedDepositsPending.Count; i++)
		{
			if (this.seedDepositsPending[i].Item2 == actorNumber)
			{
				num = this.seedDepositsPending[i].Item1;
			}
		}
		if (num == -1)
		{
			return;
		}
		if (succeeded)
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedSucceeded, actorNumber, num);
			this.RemovePendingSeedDeposit(num);
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			grplayer.SendSeedDepositedTelemetry(this.PROCESSING_TIME_SECONDS.ToString(), this.currentPlayerData.coreCount);
			grplayer.IncrementChaosSeedsCollected(1);
			return;
		}
		this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorDepositSeedFailed, actorNumber, num);
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x00118400 File Offset: 0x00116600
	public void CardSwipeSuccess()
	{
		this.idCardScanner.onSucceeded.Invoke();
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x00118412 File Offset: 0x00116612
	public void CardSwipeFail()
	{
		this.idCardScanner.onFailed.Invoke();
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x00118424 File Offset: 0x00116624
	public void TryDepositSeed(int playerActorNumber, int seedNetId)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(playerActorNumber);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (player == null || managerForZone == null)
		{
			return;
		}
		this.depositorAudioSource.PlayOneShot(this.seedDepositAttemptAudio, this.seedDepositAttemptVolume);
		if (player.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			bool flag = false;
			float time = Time.time;
			for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
			{
				if (time - this.seedDepositsPending[i].Item3 > 5f || managerForZone.GetGameEntityFromNetId(this.seedDepositsPending[i].Item1) == null || NetworkSystem.Instance.GetPlayer(this.seedDepositsPending[i].Item2) == null)
				{
					this.seedDepositsPending.RemoveAt(i);
				}
				else if (this.seedDepositsPending[i].Item1 == seedNetId)
				{
					flag = true;
					if (this.seedDepositsPending[i].Item2 == NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.seedDepositsPending[i].Item4)
					{
						ValueTuple<int, int, float, bool> value = this.seedDepositsPending[i];
						value.Item4 = true;
						this.seedDepositsPending[i] = value;
						ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
					}
				}
			}
			if (!flag)
			{
				this.seedDepositsPending.Add(new ValueTuple<int, int, float, bool>(seedNetId, playerActorNumber, Time.time, true));
				ProgressionManager.Instance.DepositCore(ProgressionManager.CoreType.ChaosSeed);
			}
		}
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x001185C0 File Offset: 0x001167C0
	public bool ValidateSeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (this.ghostReactor.grManager.IsAuthority())
		{
			bool result = false;
			for (int i = 0; i < this.seedDepositsPending.Count; i++)
			{
				if (this.seedDepositsPending[i].Item1 == entityNetId && this.seedDepositsPending[i].Item2 == playerActorNumber)
				{
					result = true;
				}
			}
			return result;
		}
		return false;
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x00118624 File Offset: 0x00116824
	public void SeedDepositSucceeded(int playerActorNumber, int entityNetId)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		this.depositorParticles.Play();
		this.depositorAudioSource.PlayOneShot(this.seedDepositAudio, this.seedDepositVolume);
		this.RemovePendingSeedDeposit(entityNetId);
		if (playerActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		if (!this.stationOpen && this.ghostReactor.grManager.IsAuthority())
		{
			this.ghostReactor.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SeedExtractorOpenStation, playerActorNumber, 0);
		}
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x001186B1 File Offset: 0x001168B1
	public void SeedDepositFailed(int playerActorNumber, int entityNetId)
	{
		this.depositorAudioSource.PlayOneShot(this.seedDepositFailedAudio, this.seedDepositFailedVolume);
		this.RemovePendingSeedDeposit(entityNetId);
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x001186D4 File Offset: 0x001168D4
	private void RemovePendingSeedDeposit(int entityId)
	{
		for (int i = this.seedDepositsPending.Count - 1; i >= 0; i--)
		{
			if (this.seedDepositsPending[i].Item1 == entityId)
			{
				this.seedDepositsPending.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x0011871C File Offset: 0x0011691C
	public void ApplyState(int playerActorNumber, int coreCount, int coresProcessedByOverdrive, int researchPoints, float coreProcessingPercentage, float overdriveSupply)
	{
		if (playerActorNumber == this.currentPlayerActorNumber)
		{
			if (this.currentPlayerData.actorNumber != playerActorNumber)
			{
				this.currentPlayerData = default(GRSeedExtractor.PlayerData);
			}
			coreCount = Mathf.Clamp(coreCount, 0, this.maxVisualChaosSeedCount);
			coresProcessedByOverdrive = Mathf.Clamp(coresProcessedByOverdrive, 0, this.MAX_OVERDRIVE_USES);
			coreProcessingPercentage = Mathf.Clamp(coreProcessingPercentage, 0f, 1f);
			overdriveSupply = Mathf.Clamp(overdriveSupply, 0f, 1f);
			bool flag = overdriveSupply < this.currentPlayerData.overdriveSupply;
			bool flag2 = this.currentPlayerData.overdriveSupply == 0f && this.currentPlayerData.coreCount > coreCount;
			if (playerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && coresProcessedByOverdrive > 0 && (flag || flag2))
			{
				this.currentPlayerData.coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing + coresProcessedByOverdrive;
			}
			this.currentPlayerData.actorNumber = playerActorNumber;
			this.currentPlayerData.coreCount = coreCount;
			this.currentPlayerData.coresProcessedByOverdrive = coresProcessedByOverdrive;
			this.currentPlayerData.coreProcessingPercentage = coreProcessingPercentage;
			this.currentPlayerData.overdriveSupply = overdriveSupply;
			this.currentPlayerData.latestRefreshTime = Time.time;
			this.currentPlayerData.researchPoints = researchPoints;
			this.OnStateUpdated();
		}
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x00118854 File Offset: 0x00116A54
	public void OpenStation(int playerActorNumber)
	{
		if (NetworkSystem.Instance.GetPlayer(playerActorNumber) == null)
		{
			return;
		}
		if (!this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorOpenAudio, this.doorOpenVolume);
		}
		base.enabled = true;
		this.currentPlayerActorNumber = playerActorNumber;
		this.stationOpen = true;
		this.stationOpenRequestTime = Time.time;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x001188B4 File Offset: 0x00116AB4
	public void CloseStation()
	{
		if (this.stationOpen)
		{
			this.doorAudioSource.PlayOneShot(this.doorCloseAudio, this.doorCloseVolume);
		}
		this.currentPlayerActorNumber = -1;
		this.stationOpen = false;
		this.UpdateOverdrivePurchaseButtons();
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x001188EC File Offset: 0x00116AEC
	private void UpdateOverdrivePurchaseButtons()
	{
		if (!this.LocalPlayerCanPurchaseOverdrive())
		{
			this.overdrivePurchaseButton.myTmpText.text = "";
			this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "";
			this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
			return;
		}
		if (this.overdrivePurchasePending)
		{
			this.overdrivePurchaseButton.myTmpText.text = "CANCEL";
			this.overdrivePurchaseButton.buttonRenderer.material = this.redButtonMaterial;
			this.overdriveConfirmButton.myTmpText.text = "CONFIRM";
			this.overdriveConfirmButton.buttonRenderer.material = this.greenButtonMaterial;
			return;
		}
		this.overdrivePurchaseButton.myTmpText.text = "BUY";
		this.overdrivePurchaseButton.buttonRenderer.material = this.defaultButtonMaterial;
		this.overdriveConfirmButton.myTmpText.text = "";
		this.overdriveConfirmButton.buttonRenderer.material = this.defaultButtonMaterial;
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x00118A10 File Offset: 0x00116C10
	public void OnStateUpdated()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null)
		{
			this.CloseStation();
		}
		this.UpdateOverdrivePurchaseButtons();
		if (this.stationOpen && player != null)
		{
			if (this.overdriveActive)
			{
				return;
			}
			if (this.currentPlayerData.coresPendingOverdriveProcessing > 0)
			{
				int coresPendingOverdriveProcessing = this.currentPlayerData.coresPendingOverdriveProcessing;
				this.currentPlayerData.coresPendingOverdriveProcessing = 0;
				if (this.StationOpenForLocalPlayer)
				{
					this.localPlayerData.coresPendingOverdriveProcessing = 0;
				}
				this.overdrivePurchaseAnimationRoutine = base.StartCoroutine(this.OverdrivePurchaseAnimationVisual(coresPendingOverdriveProcessing));
				return;
			}
			this.processingAmount = this.currentPlayerData.coreProcessingPercentage;
			this.overdriveAmount = this.currentPlayerData.overdriveSupply;
			int num = Mathf.Clamp(this.currentPlayerData.coreCount, 0, this.maxVisualChaosSeedCount) - this.seedProcessingStates.Count;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.DepositSeedVisual();
				}
				return;
			}
			if (num < 0)
			{
				for (int j = 0; j > num; j--)
				{
					this.CompleteSeedVisual();
				}
				return;
			}
		}
		else
		{
			this.screenText.text = "Player Data Lookup Failed.";
			this.overdriveAmount = 0f;
			this.processingAmount = 0f;
		}
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x00118B44 File Offset: 0x00116D44
	private void DepositSeedVisual()
	{
		for (int i = 0; i < this.chaosSeedVisuals.Count; i++)
		{
			if (!this.chaosSeedVisuals[i].activeSelf)
			{
				GRSeedExtractor.SeedProcessingVisualState item = new GRSeedExtractor.SeedProcessingVisualState
				{
					poolIndex = i,
					rollAngle = 0f,
					speed = 0f,
					rampProgress = 0f,
					dropProgress = 0f
				};
				this.seedProcessingStates.Add(item);
				this.chaosSeedVisuals[i].SetActive(true);
				this.chaosSeedVisuals[i].transform.localPosition = this.seedTubeStart.localPosition;
				this.chaosSeedVisuals[i].transform.localRotation = Quaternion.identity;
				this.chaosSeedVisuals[i].transform.localScale = Vector3.one * this.seedVisualScaleRange.y;
				this.seedTubeAudioSource.PlayOneShot(this.seedMovementAudio, this.seedMovementVolume);
				return;
			}
		}
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x00118C64 File Offset: 0x00116E64
	private void CompleteSeedVisual()
	{
		if (this.seedProcessingStates.Count > 0)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[0];
			this.chaosSeedVisuals[seedProcessingVisualState.poolIndex].SetActive(false);
			this.seedProcessingStates.RemoveAt(0);
		}
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x00118CB0 File Offset: 0x00116EB0
	private void ClearSeedVisuals()
	{
		int count = this.seedProcessingStates.Count;
		for (int i = 0; i < count; i++)
		{
			this.CompleteSeedVisual();
		}
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x00118CDC File Offset: 0x00116EDC
	private void UpdateScreenDisplay()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.currentPlayerActorNumber);
		if (player == null || !this.stationOpen)
		{
			return;
		}
		int num = (int)this.estimatedJuiceTimeRemaining;
		if (this.currentPlayerActorNumber != this.currentDisplayData.playerActorNumber || this.currentPlayerData.coreCount != this.currentDisplayData.coreCount || this.currentPlayerData.overdriveSupply != this.currentDisplayData.overdriveSupply || this.currentPlayerData.researchPoints != this.currentDisplayData.researchPoints || num != this.currentDisplayData.juiceSecondsLeft)
		{
			this.currentDisplayData.playerActorNumber = this.currentPlayerActorNumber;
			this.currentDisplayData.coreCount = this.currentPlayerData.coreCount;
			this.currentDisplayData.overdriveSupply = this.currentPlayerData.overdriveSupply;
			this.currentDisplayData.researchPoints = this.currentPlayerData.researchPoints;
			this.currentDisplayData.juiceSecondsLeft = num;
			this.UpdateScreenSB.Clear();
			this.UpdateScreenSB.Append(player.SanitizedNickName + "\n");
			this.UpdateScreenSB.Append(string.Format("JUICE: <color=purple>⑮ {0}</color>\n\n", this.currentDisplayData.researchPoints));
			if (this.currentDisplayData.coreCount > 0)
			{
				this.UpdateScreenSB.Append(string.Format("Processing {0} Seeds", this.currentDisplayData.coreCount));
				int num2 = this.currentDisplayData.juiceSecondsLeft % 3;
				if (num2 == 2)
				{
					this.UpdateScreenSB.Append(".");
				}
				else if (num2 == 1)
				{
					this.UpdateScreenSB.Append("..");
				}
				else
				{
					this.UpdateScreenSB.Append("...");
				}
				int num3 = num / 3600;
				int num4 = num / 60 % 60;
				int num5 = num % 60;
				if (num3 > 0)
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}:{2:00}\n", num3, num4, num5));
				}
				else
				{
					this.UpdateScreenSB.Append(string.Format("\nNext <color=purple>⑮</color> in {0}:{1:00}\n", num4, num5));
				}
			}
			else
			{
				this.UpdateScreenSB.Append("Deposit Chaos Seed\nFor Juice Processing\n");
			}
			this.screenText.text = this.UpdateScreenSB.ToString();
		}
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x00118F40 File Offset: 0x00117140
	private void StepSeedVisualAnimation(float dt)
	{
		float magnitude = (this.seedTubeStart.position - this.seedTubeEnd.position).magnitude;
		float num = magnitude / this.seedVisualRollTime;
		for (int i = 0; i < this.seedProcessingStates.Count; i++)
		{
			GRSeedExtractor.SeedProcessingVisualState seedProcessingVisualState = this.seedProcessingStates[i];
			float num2 = 2f;
			if (i > 0)
			{
				num2 = this.seedProcessingStates[i - 1].rampProgress - 2f * this.visualChaosSeedRadius / magnitude;
			}
			if (seedProcessingVisualState.rampProgress < 1f)
			{
				GameObject gameObject = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				seedProcessingVisualState.speed = Mathf.MoveTowards(seedProcessingVisualState.speed, num, num * dt);
				float num3 = seedProcessingVisualState.speed * dt;
				float num4 = num3 / magnitude;
				seedProcessingVisualState.rampProgress = Mathf.Clamp01(seedProcessingVisualState.rampProgress + num4);
				if (seedProcessingVisualState.rampProgress >= num2)
				{
					seedProcessingVisualState.rampProgress = num2;
					seedProcessingVisualState.speed = 0f;
					num3 = 0f;
				}
				gameObject.transform.localPosition = Vector3.Lerp(this.seedTubeStart.localPosition, this.seedTubeEnd.localPosition, seedProcessingVisualState.rampProgress);
				seedProcessingVisualState.rollAngle += num3 / this.visualChaosSeedRadius;
				gameObject.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
			}
			if (i == 0 && seedProcessingVisualState.rampProgress >= 1f)
			{
				GameObject gameObject2 = this.chaosSeedVisuals[seedProcessingVisualState.poolIndex];
				if (seedProcessingVisualState.dropProgress < 1f)
				{
					seedProcessingVisualState.dropProgress += 1f / this.seedVisualDropTime * dt;
					seedProcessingVisualState.rampProgress = 1f + seedProcessingVisualState.dropProgress;
					float t = this.tubeEndToProcessingPathY.Evaluate(seedProcessingVisualState.dropProgress);
					float t2 = this.tubeEndToProcessingPathX.Evaluate(seedProcessingVisualState.dropProgress);
					Vector3 localPosition = gameObject2.transform.localPosition;
					localPosition.y = Mathf.Lerp(this.seedTubeEnd.localPosition.y, this.seedProcessingPosition.localPosition.y, t);
					localPosition.x = Mathf.Lerp(this.seedTubeEnd.localPosition.x, this.seedProcessingPosition.localPosition.x, t2);
					gameObject2.transform.localPosition = localPosition;
					float num5 = seedProcessingVisualState.speed * dt;
					seedProcessingVisualState.rollAngle += num5 / this.visualChaosSeedRadius;
					gameObject2.transform.localRotation = Quaternion.AngleAxis(seedProcessingVisualState.rollAngle * 57.29578f, Vector3.forward);
					if (seedProcessingVisualState.dropProgress >= 1f)
					{
						this.juicerAudioSource.PlayOneShot(this.seedDropAudio, this.seedDropVolume);
					}
				}
				if (seedProcessingVisualState.dropProgress >= 1f && !this.drainingProcessingBeaker)
				{
					gameObject2.transform.localScale = Vector3.one * Mathf.Lerp(this.seedVisualScaleRange.y, this.seedVisualScaleRange.x, this.processingAmountVisual);
				}
			}
			this.seedProcessingStates[i] = seedProcessingVisualState;
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x0011928B File Offset: 0x0011748B
	private IEnumerator OverdrivePurchaseAnimationVisual(int coresToProcess)
	{
		this.overdriveActive = true;
		this.overdriveBeepAudioSource.loop = true;
		this.overdriveBeepAudioSource.volume = this.overdriveBeepingVolume;
		this.overdriveBeepAudioSource.clip = this.overdriveBeepingAudio;
		this.overdriveBeepAudioSource.Play();
		int num = Math.Min(coresToProcess + this.currentPlayerData.coreCount, this.maxVisualChaosSeedCount);
		while (this.seedProcessingStates.Count < num)
		{
			this.DepositSeedVisual();
		}
		for (int j = 0; j < this.disableDuringOverdrive.Count; j++)
		{
			this.disableDuringOverdrive[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < this.enableDuringOverdrive.Count; k++)
		{
			this.enableDuringOverdrive[k].gameObject.SetActive(true);
		}
		this.overdriveMeterAudioSource.PlayOneShot(this.overdriveFillAudio, this.overdriveFillVolume);
		float overdriveFillRate = 1f / this.overdriveFillTime;
		float maxOverdriveFill = Mathf.Clamp01(this.currentPlayerData.overdriveSupply + (float)coresToProcess / (float)this.MAX_OVERDRIVE_USES);
		while (this.overdriveAmount < maxOverdriveFill)
		{
			this.overdriveAmount = Mathf.MoveTowards(this.overdriveAmount, maxOverdriveFill, overdriveFillRate * Time.deltaTime);
			yield return null;
		}
		this.overdriveMeterAudioSource.Stop();
		int num4;
		for (int i = 0; i < coresToProcess; i = num4)
		{
			float waitForSeedDepositStartTime = Time.time;
			bool flag = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag && Time.time - waitForSeedDepositStartTime < 3f)
			{
				yield return null;
				flag = (this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f);
			}
			this.juicerAudioSource.PlayOneShot(this.seedJuicingAudio, this.seedJuicingVolume);
			this.juicerOverdriveParticles.gameObject.SetActive(true);
			float num2 = Mathf.Clamp01(1f - this.processingAmount);
			float timeToProcess = num2 * this.overdriveProcessTime;
			float startingProcessingAmount = this.processingAmount;
			float num3 = num2 / (float)this.MAX_OVERDRIVE_USES;
			float startingOverdrive = this.overdriveAmount;
			float resultingOverdrive = Mathf.Clamp01(this.overdriveAmount - num3);
			float timeProcessing = 0f;
			while (timeProcessing < timeToProcess)
			{
				timeProcessing += Time.deltaTime;
				float t = timeProcessing / timeToProcess;
				this.overdriveAmount = Mathf.Lerp(startingOverdrive, resultingOverdrive, t);
				this.processingAmount = Mathf.Lerp(startingProcessingAmount, 1f, t);
				this.estimatedJuiceTimeRemaining = timeToProcess - timeProcessing;
				yield return null;
			}
			this.CompleteSeedVisual();
			this.juicerOverdriveParticles.gameObject.SetActive(false);
			this.drainingProcessingBeaker = true;
			float timeDepositing = 0f;
			while (timeDepositing < this.juiceDepositTime)
			{
				timeDepositing += Time.deltaTime;
				float t2 = timeDepositing / this.juiceDepositTime;
				this.processingAmount = Mathf.Lerp(1f, 0f, t2);
				yield return null;
			}
			this.drainingProcessingBeaker = false;
			num4 = i + 1;
		}
		if (this.currentPlayerData.coresPendingOverdriveProcessing == 0 && this.currentPlayerData.coreCount == 1)
		{
			if (this.seedProcessingStates.Count == 0)
			{
				this.DepositSeedVisual();
			}
			float timeDepositing = Time.time;
			bool flag2 = this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f;
			while (!flag2 && Time.time - timeDepositing < 3f)
			{
				yield return null;
				flag2 = (this.seedProcessingStates.Count > 0 && this.seedProcessingStates[0].dropProgress >= 1f);
			}
			float timeProcessing = 0f;
			float resultingOverdrive = this.processingAmount;
			float startingOverdrive = this.overdriveAmount;
			float startingProcessingAmount = Mathf.Clamp01(this.currentPlayerData.coreProcessingPercentage - resultingOverdrive) * this.overdriveProcessTime;
			while (timeProcessing < startingProcessingAmount)
			{
				timeProcessing += Time.deltaTime;
				float t3 = timeProcessing / startingProcessingAmount;
				this.processingAmount = Mathf.Clamp01(Mathf.Lerp(resultingOverdrive, this.currentPlayerData.coreProcessingPercentage, t3));
				this.overdriveAmount = Mathf.Clamp01(Mathf.Lerp(startingOverdrive, this.currentPlayerData.overdriveSupply, t3));
				yield return null;
			}
		}
		for (int l = 0; l < this.disableDuringOverdrive.Count; l++)
		{
			this.disableDuringOverdrive[l].gameObject.SetActive(true);
		}
		for (int m = 0; m < this.enableDuringOverdrive.Count; m++)
		{
			this.enableDuringOverdrive[m].gameObject.SetActive(false);
		}
		this.overdriveLightSpinnerOff.localRotation = this.overdriveLightSpinnerOn.localRotation;
		this.overdriveBeepAudioSource.Stop();
		this.overdriveActive = false;
		if (this.StationOpenForLocalPlayer)
		{
			ProgressionManager.Instance.GetJuicerStatus();
		}
		this.OnStateUpdated();
		yield break;
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x001192A4 File Offset: 0x001174A4
	public void OnResearchPointsUpdated()
	{
		int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
		if (numberOfResearchPoints > this.localPlayerData.researchPoints)
		{
			GRPlayer.GetLocal().SendJuiceCollectedTelemetry(numberOfResearchPoints - this.localPlayerData.researchPoints, this.localPlayerData.coresProcessedByOverdrive);
		}
		this.localPlayerData.researchPoints = numberOfResearchPoints;
		if (this.StationOpenForLocalPlayer)
		{
			bool flag = this.currentPlayerData.researchPoints != this.localPlayerData.researchPoints;
			this.currentPlayerData.researchPoints = this.localPlayerData.researchPoints;
			if (flag)
			{
				this.ghostReactor.grManager.RequestApplySeedExtractorState(this.localPlayerData.coreCount, this.localPlayerData.coresProcessedByOverdrive, this.localPlayerData.researchPoints, this.localPlayerData.coreProcessingPercentage, this.localPlayerData.overdriveSupply);
				this.OnStateUpdated();
			}
		}
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x00119384 File Offset: 0x00117584
	public void OnPurchaseOverdrive(bool success)
	{
		this.overdriveServerConfirmationPending = false;
		if (!success)
		{
			return;
		}
		GRPlayer.GetLocal().SendOverdrivePurchasedTelemetry(250, this.localPlayerData.coreCount);
	}

	// Token: 0x04004246 RID: 16966
	private float PROCESSING_TIME_SECONDS = 600f;

	// Token: 0x04004247 RID: 16967
	private int MAX_OVERDRIVE_USES = 6;

	// Token: 0x04004248 RID: 16968
	[SerializeField]
	private GTZone zone;

	// Token: 0x04004249 RID: 16969
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x0400424A RID: 16970
	[SerializeField]
	private TriggerEventNotifier coreDepositTriggerNotifier;

	// Token: 0x0400424B RID: 16971
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x0400424C RID: 16972
	[SerializeField]
	private IDCardScanner idCardScanner;

	// Token: 0x0400424D RID: 16973
	[SerializeField]
	private GameObject chaosSeedVisualPrefab;

	// Token: 0x0400424E RID: 16974
	[Header("Overdrive Purchase Buttons")]
	[SerializeField]
	private GorillaPressableButton overdrivePurchaseButton;

	// Token: 0x0400424F RID: 16975
	[SerializeField]
	private GorillaPressableButton overdriveConfirmButton;

	// Token: 0x04004250 RID: 16976
	[SerializeField]
	private Material defaultButtonMaterial;

	// Token: 0x04004251 RID: 16977
	[SerializeField]
	private Material redButtonMaterial;

	// Token: 0x04004252 RID: 16978
	[SerializeField]
	private Material greenButtonMaterial;

	// Token: 0x04004253 RID: 16979
	[Header("Shutter Door Visual")]
	[SerializeField]
	private Transform shutterDoorParent;

	// Token: 0x04004254 RID: 16980
	[SerializeField]
	private Vector2 shutterDoorLiftRange = new Vector2(1.245f, 2.07f);

	// Token: 0x04004255 RID: 16981
	[SerializeField]
	private float shutterDoorAnimTime;

	// Token: 0x04004256 RID: 16982
	[Header("Seed Processing Visual")]
	[SerializeField]
	private Transform processingLiquidScaleParent;

	// Token: 0x04004257 RID: 16983
	[SerializeField]
	[Range(0f, 1f)]
	public float processingAmount;

	// Token: 0x04004258 RID: 16984
	private float processingAmountVisual;

	// Token: 0x04004259 RID: 16985
	[SerializeField]
	private Transform seedTubeStart;

	// Token: 0x0400425A RID: 16986
	[SerializeField]
	private Transform seedTubeEnd;

	// Token: 0x0400425B RID: 16987
	[SerializeField]
	private Transform seedProcessingPosition;

	// Token: 0x0400425C RID: 16988
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400425D RID: 16989
	[SerializeField]
	private AnimationCurve tubeEndToProcessingPathX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400425E RID: 16990
	[SerializeField]
	private float visualChaosSeedRadius = 1f;

	// Token: 0x0400425F RID: 16991
	[SerializeField]
	private int maxVisualChaosSeedCount = 6;

	// Token: 0x04004260 RID: 16992
	[SerializeField]
	private float seedVisualRollTime = 2f;

	// Token: 0x04004261 RID: 16993
	[SerializeField]
	private float seedVisualDropTime = 0.5f;

	// Token: 0x04004262 RID: 16994
	[SerializeField]
	private Vector2 seedVisualScaleRange = new Vector2(0.1f, 1.25f);

	// Token: 0x04004263 RID: 16995
	[Header("Overdrive Visual")]
	[SerializeField]
	private Transform overdriveLiquidScaleParent;

	// Token: 0x04004264 RID: 16996
	[SerializeField]
	private Transform overdriveLightSpinnerOff;

	// Token: 0x04004265 RID: 16997
	[SerializeField]
	private Transform overdriveLightSpinnerOn;

	// Token: 0x04004266 RID: 16998
	[SerializeField]
	private List<Transform> enableDuringOverdrive = new List<Transform>();

	// Token: 0x04004267 RID: 16999
	[SerializeField]
	private List<Transform> disableDuringOverdrive = new List<Transform>();

	// Token: 0x04004268 RID: 17000
	[SerializeField]
	private float overdriveLightSpinRate = 1f;

	// Token: 0x04004269 RID: 17001
	[SerializeField]
	[Range(0f, 1f)]
	public float overdriveAmount;

	// Token: 0x0400426A RID: 17002
	private float overdriveAmountVisual;

	// Token: 0x0400426B RID: 17003
	[Header("VFX")]
	[SerializeField]
	private ParticleSystem depositorParticles;

	// Token: 0x0400426C RID: 17004
	[SerializeField]
	private ParticleSystem juicerSlowParticles;

	// Token: 0x0400426D RID: 17005
	[SerializeField]
	private ParticleSystem juicerOverdriveParticles;

	// Token: 0x0400426E RID: 17006
	[Header("Audio")]
	[SerializeField]
	private AudioSource depositorAudioSource;

	// Token: 0x0400426F RID: 17007
	[SerializeField]
	private AudioSource doorAudioSource;

	// Token: 0x04004270 RID: 17008
	[SerializeField]
	private AudioSource seedTubeAudioSource;

	// Token: 0x04004271 RID: 17009
	[SerializeField]
	private AudioSource juicerAudioSource;

	// Token: 0x04004272 RID: 17010
	[SerializeField]
	private AudioSource machineHumAudioSource;

	// Token: 0x04004273 RID: 17011
	[SerializeField]
	private AudioSource overdriveMeterAudioSource;

	// Token: 0x04004274 RID: 17012
	[SerializeField]
	private AudioSource overdriveBeepAudioSource;

	// Token: 0x04004275 RID: 17013
	[SerializeField]
	private AudioClip seedDepositAudio;

	// Token: 0x04004276 RID: 17014
	[SerializeField]
	private float seedDepositVolume = 0.5f;

	// Token: 0x04004277 RID: 17015
	[SerializeField]
	private AudioClip seedDepositFailedAudio;

	// Token: 0x04004278 RID: 17016
	[SerializeField]
	private float seedDepositFailedVolume = 0.5f;

	// Token: 0x04004279 RID: 17017
	[SerializeField]
	private AudioClip seedDepositAttemptAudio;

	// Token: 0x0400427A RID: 17018
	[SerializeField]
	private float seedDepositAttemptVolume = 0.5f;

	// Token: 0x0400427B RID: 17019
	[SerializeField]
	private AudioClip seedMovementAudio;

	// Token: 0x0400427C RID: 17020
	[SerializeField]
	private float seedMovementVolume = 0.5f;

	// Token: 0x0400427D RID: 17021
	[SerializeField]
	private AudioClip seedDropAudio;

	// Token: 0x0400427E RID: 17022
	[SerializeField]
	private float seedDropVolume = 0.5f;

	// Token: 0x0400427F RID: 17023
	[SerializeField]
	private AudioClip seedJuicingAudio;

	// Token: 0x04004280 RID: 17024
	[SerializeField]
	private float seedJuicingVolume = 0.5f;

	// Token: 0x04004281 RID: 17025
	[SerializeField]
	private AudioClip doorOpenAudio;

	// Token: 0x04004282 RID: 17026
	[SerializeField]
	private float doorOpenVolume = 0.5f;

	// Token: 0x04004283 RID: 17027
	[SerializeField]
	private AudioClip doorCloseAudio;

	// Token: 0x04004284 RID: 17028
	[SerializeField]
	private float doorCloseVolume = 0.5f;

	// Token: 0x04004285 RID: 17029
	[SerializeField]
	private AudioClip processingHumAudio;

	// Token: 0x04004286 RID: 17030
	[SerializeField]
	private float processingHumVolume = 0.5f;

	// Token: 0x04004287 RID: 17031
	[SerializeField]
	private AudioClip overdriveFillAudio;

	// Token: 0x04004288 RID: 17032
	[SerializeField]
	private float overdriveFillVolume = 0.5f;

	// Token: 0x04004289 RID: 17033
	[SerializeField]
	private AudioClip overdriveEngineAudio;

	// Token: 0x0400428A RID: 17034
	[SerializeField]
	private float overdriveEngineVolume = 0.5f;

	// Token: 0x0400428B RID: 17035
	[SerializeField]
	private AudioClip overdriveBeepingAudio;

	// Token: 0x0400428C RID: 17036
	[SerializeField]
	private float overdriveBeepingVolume = 0.5f;

	// Token: 0x0400428D RID: 17037
	private GRSeedExtractor.PlayerData localPlayerData;

	// Token: 0x0400428E RID: 17038
	private GRSeedExtractor.PlayerData currentPlayerData;

	// Token: 0x0400428F RID: 17039
	private GRSeedExtractor.ScreenDisplayData currentDisplayData;

	// Token: 0x04004290 RID: 17040
	private bool stationOpen;

	// Token: 0x04004291 RID: 17041
	private float stationOpenRequestTime;

	// Token: 0x04004292 RID: 17042
	private int currentPlayerActorNumber = -1;

	// Token: 0x04004293 RID: 17043
	private float shutterDoorOpenAmount;

	// Token: 0x04004294 RID: 17044
	private List<GameObject> chaosSeedVisuals = new List<GameObject>();

	// Token: 0x04004295 RID: 17045
	private bool overdrivePurchasePending;

	// Token: 0x04004296 RID: 17046
	private bool overdriveServerConfirmationPending;

	// Token: 0x04004297 RID: 17047
	private float overdrivePurchaseTime;

	// Token: 0x04004298 RID: 17048
	private bool overdriveActive;

	// Token: 0x04004299 RID: 17049
	private bool drainingProcessingBeaker;

	// Token: 0x0400429A RID: 17050
	private float estimatedJuiceTimeRemaining;

	// Token: 0x0400429B RID: 17051
	private float processingLiquidFollowRate = Mathf.Exp(2f);

	// Token: 0x0400429C RID: 17052
	private List<ValueTuple<int, int, float, bool>> seedDepositsPending = new List<ValueTuple<int, int, float, bool>>();

	// Token: 0x0400429D RID: 17053
	private Coroutine overdrivePurchaseAnimationRoutine;

	// Token: 0x0400429E RID: 17054
	private List<GRSeedExtractor.SeedProcessingVisualState> seedProcessingStates = new List<GRSeedExtractor.SeedProcessingVisualState>();

	// Token: 0x0400429F RID: 17055
	private float timeBetweenServerRequests = 3f;

	// Token: 0x040042A0 RID: 17056
	private float lastServerRequestTime;

	// Token: 0x040042A1 RID: 17057
	private GhostReactor ghostReactor;

	// Token: 0x040042A2 RID: 17058
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x040042A3 RID: 17059
	private StringBuilder UpdateScreenSB = new StringBuilder(256);

	// Token: 0x040042A4 RID: 17060
	[Header("Debug Animation")]
	public int debugSeedCount;

	// Token: 0x040042A5 RID: 17061
	public float debugSeedProcessingTime = 10f;

	// Token: 0x040042A6 RID: 17062
	public float overdriveFillTime = 2f;

	// Token: 0x040042A7 RID: 17063
	public float overdriveProcessTime = 1.5f;

	// Token: 0x040042A8 RID: 17064
	public float juiceDepositTime = 0.75f;

	// Token: 0x020007D2 RID: 2002
	public struct PlayerData
	{
		// Token: 0x040042A9 RID: 17065
		public int actorNumber;

		// Token: 0x040042AA RID: 17066
		public int coreCount;

		// Token: 0x040042AB RID: 17067
		public float coreProcessingPercentage;

		// Token: 0x040042AC RID: 17068
		public float overdriveSupply;

		// Token: 0x040042AD RID: 17069
		public int coresProcessedByOverdrive;

		// Token: 0x040042AE RID: 17070
		public int coresPendingOverdriveProcessing;

		// Token: 0x040042AF RID: 17071
		public int researchPoints;

		// Token: 0x040042B0 RID: 17072
		public float latestRefreshTime;
	}

	// Token: 0x020007D3 RID: 2003
	private struct ScreenDisplayData
	{
		// Token: 0x040042B1 RID: 17073
		public int playerActorNumber;

		// Token: 0x040042B2 RID: 17074
		public int coreCount;

		// Token: 0x040042B3 RID: 17075
		public float overdriveSupply;

		// Token: 0x040042B4 RID: 17076
		public int researchPoints;

		// Token: 0x040042B5 RID: 17077
		public int juiceSecondsLeft;
	}

	// Token: 0x020007D4 RID: 2004
	private struct SeedProcessingVisualState
	{
		// Token: 0x040042B6 RID: 17078
		public int poolIndex;

		// Token: 0x040042B7 RID: 17079
		public float speed;

		// Token: 0x040042B8 RID: 17080
		public float rollAngle;

		// Token: 0x040042B9 RID: 17081
		public float rampProgress;

		// Token: 0x040042BA RID: 17082
		public float dropProgress;
	}
}
