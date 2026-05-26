using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class GreyZoneManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x0005101A File Offset: 0x0004F21A
	public bool GreyZoneActive
	{
		get
		{
			return this.greyZoneActive;
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000EC6 RID: 3782 RVA: 0x00051024 File Offset: 0x0004F224
	public bool GreyZoneAvailable
	{
		get
		{
			bool result = false;
			if (GorillaComputer.instance != null)
			{
				result = (GorillaComputer.instance.GetServerTime().DayOfYear >= this.greyZoneAvailableDayOfYear);
			}
			return result;
		}
	}

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x00051063 File Offset: 0x0004F263
	public int GravityFactorSelection
	{
		get
		{
			return this.gravityFactorOptionSelection;
		}
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000EC8 RID: 3784 RVA: 0x0005106B File Offset: 0x0004F26B
	// (set) Token: 0x06000EC9 RID: 3785 RVA: 0x00051073 File Offset: 0x0004F273
	public bool TickRunning
	{
		get
		{
			return this._tickRunning;
		}
		set
		{
			this._tickRunning = value;
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000ECA RID: 3786 RVA: 0x0005107C File Offset: 0x0004F27C
	public bool HasAuthority
	{
		get
		{
			return !PhotonNetwork.InRoom || base.photonView.IsMine;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000ECB RID: 3787 RVA: 0x00051092 File Offset: 0x0004F292
	public float SummoningProgress
	{
		get
		{
			return this.summoningProgress;
		}
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x0005109A File Offset: 0x0004F29A
	public void RegisterSummoner(GreyZoneSummoner summoner)
	{
		if (!this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Add(summoner);
		}
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x000510B6 File Offset: 0x0004F2B6
	public void DeregisterSummoner(GreyZoneSummoner summoner)
	{
		if (this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Remove(summoner);
		}
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x000510D3 File Offset: 0x0004F2D3
	public void RegisterMoon(MoonController moon)
	{
		this.moonController = moon;
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x000510DC File Offset: 0x0004F2DC
	public void UnregisterMoon(MoonController moon)
	{
		if (this.moonController == moon)
		{
			this.moonController = null;
		}
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x000510F3 File Offset: 0x0004F2F3
	public void ActivateGreyZoneAuthority()
	{
		this.greyZoneActive = true;
		this.photonConnectedDuringActivation = PhotonNetwork.InRoom;
		this.greyZoneActivationTime = (this.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		this.ActivateGreyZoneLocal();
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x00051128 File Offset: 0x0004F328
	private void ActivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 1);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gravityOverrideSet = true;
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutMusic(2f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioIn(this.greyZoneAmbience, this.greyZoneAmbienceVolume, this.ambienceFadeTime));
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.GTPlay();
		}
		this.greyZoneParticles.gameObject.SetActive(true);
		this.summoningProgress = 1f;
		this.UpdateSummonerVisuals();
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].OnGreyZoneActivated();
		}
		if (this.OnGreyZoneActivated != null)
		{
			this.OnGreyZoneActivated();
		}
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x0005123C File Offset: 0x0004F43C
	public void LocalSimpleActivation(bool onOff, float gravityFactor)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			return;
		}
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			return;
		}
		this.simpleGravityFactor = Mathf.Clamp(gravityFactor, 0f, 5f);
		Shader.SetGlobalInt(this._GreyZoneActive, onOff ? 1 : 0);
		if (onOff)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.SimpleGravityOverrideFunction));
		}
		else
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = onOff;
		this.greyZoneParticles.gameObject.SetActive(onOff);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x000512DC File Offset: 0x0004F4DC
	public void DeactivateGreyZoneAuthority()
	{
		this.greyZoneActive = false;
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			this.summoningPlayerProgress[keyValuePair.Key] = 0f;
		}
		this.DeactivateGreyZoneLocal();
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x0005134C File Offset: 0x0004F54C
	private void DeactivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(4f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioOut(this.greyZoneAmbience, this.ambienceFadeTime));
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000ED5 RID: 3797 RVA: 0x000513F4 File Offset: 0x0004F5F4
	public void ForceStopGreyZone()
	{
		this.greyZoneActive = false;
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = false;
		if (this.moonController != null)
		{
			this.moonController.UpdateDistance(1f);
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(0f);
		}
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.volume = 0f;
			this.greyZoneAmbience.GTStop();
		}
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000ED6 RID: 3798 RVA: 0x000514D4 File Offset: 0x0004F6D4
	public void GravityOverrideFunction(GTPlayer player)
	{
		this.gravityReductionAmount = 0f;
		if (this.moonController != null)
		{
			this.gravityReductionAmount = Mathf.InverseLerp(1f - this.skyMonsterDistGravityRampBuffer, this.skyMonsterDistGravityRampBuffer, this.moonController.Distance);
		}
		float d = Mathf.Lerp(1f, this.gravityFactorOptions[this.gravityFactorOptionSelection], this.gravityReductionAmount);
		player.AddForce(Physics.gravity * d * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x06000ED7 RID: 3799 RVA: 0x0005155D File Offset: 0x0004F75D
	public void SimpleGravityOverrideFunction(GTPlayer player)
	{
		player.AddForce(Physics.gravity * this.simpleGravityFactor * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x00051581 File Offset: 0x0004F781
	private IEnumerator FadeAudioIn(AudioSource source, float maxVolume, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			source.GTPlay();
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, maxVolume, num);
				yield return null;
			}
			source.volume = maxVolume;
		}
		yield break;
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0005159E File Offset: 0x0004F79E
	private IEnumerator FadeAudioOut(AudioSource source, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, 0f, num);
				yield return null;
			}
			source.volume = 0f;
			source.Stop();
		}
		yield break;
	}

	// Token: 0x06000EDA RID: 3802 RVA: 0x000515B4 File Offset: 0x0004F7B4
	public void VRRigEnteredSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (!this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Add(rig.Creator.ActorNumber, new ValueTuple<VRRig, GreyZoneSummoner>(rig, summoner));
			this.summoningPlayerProgress.Add(rig.Creator.ActorNumber, 0f);
		}
	}

	// Token: 0x06000EDB RID: 3803 RVA: 0x00051614 File Offset: 0x0004F814
	public void VRRigExitedSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Remove(rig.Creator.ActorNumber);
			this.summoningPlayerProgress.Remove(rig.Creator.ActorNumber);
		}
	}

	// Token: 0x06000EDC RID: 3804 RVA: 0x00051668 File Offset: 0x0004F868
	private void UpdateSummonerVisuals()
	{
		bool greyZoneAvailable = this.GreyZoneAvailable;
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].UpdateProgressFeedback(greyZoneAvailable);
		}
	}

	// Token: 0x06000EDD RID: 3805 RVA: 0x000516A4 File Offset: 0x0004F8A4
	private void ValidateSummoningPlayers()
	{
		this.invalidSummoners.Clear();
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			VRRig item = keyValuePair.Value.Item1;
			GreyZoneSummoner item2 = keyValuePair.Value.Item2;
			if (item.Creator.ActorNumber != keyValuePair.Key || (item.head.rigTarget.position - item2.SummoningFocusPoint).sqrMagnitude > item2.SummonerMaxDistance * item2.SummonerMaxDistance)
			{
				this.invalidSummoners.Add(keyValuePair.Key);
			}
		}
		foreach (int key in this.invalidSummoners)
		{
			this.summoningPlayers.Remove(key);
			this.summoningPlayerProgress.Remove(key);
		}
	}

	// Token: 0x06000EDE RID: 3806 RVA: 0x000517CC File Offset: 0x0004F9CC
	private int DayNightOverrideFunction(int inputIndex)
	{
		int num = 0;
		int num2 = 8;
		int num3 = inputIndex - num;
		int num4 = num2 - inputIndex;
		if (num3 <= 0 || num4 <= 0)
		{
			return inputIndex;
		}
		if (num4 > num3)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000EDF RID: 3807 RVA: 0x000517F6 File Offset: 0x0004F9F6
	private void Awake()
	{
		if (GreyZoneManager.Instance == null)
		{
			GreyZoneManager.Instance = this;
			this.greyZoneAmbienceVolume = this.greyZoneAmbience.volume;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000EE0 RID: 3808 RVA: 0x0005182C File Offset: 0x0004FA2C
	private void OnEnable()
	{
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.SetTimeIndexOverrideFunction(new Func<int, int>(this.DayNightOverrideFunction));
			}
		}
	}

	// Token: 0x06000EE1 RID: 3809 RVA: 0x00051864 File Offset: 0x0004FA64
	private void OnDisable()
	{
		this.ForceStopGreyZone();
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.UnsetTimeIndexOverrideFunction();
			}
		}
	}

	// Token: 0x06000EE2 RID: 3810 RVA: 0x00051896 File Offset: 0x0004FA96
	private void Update()
	{
		if (this.HasAuthority)
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000EE3 RID: 3811 RVA: 0x000518AC File Offset: 0x0004FAAC
	private void AuthorityUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (this.greyZoneActive)
		{
			this.summoningProgress = 1f;
			double num;
			if (this.photonConnectedDuringActivation && PhotonNetwork.InRoom)
			{
				num = PhotonNetwork.Time;
			}
			else if (!this.photonConnectedDuringActivation && !PhotonNetwork.InRoom)
			{
				num = (double)Time.time;
			}
			else
			{
				num = -100.0;
			}
			if (num > this.greyZoneActivationTime + (double)this.greyZoneActiveDuration || num < this.greyZoneActivationTime - 10.0)
			{
				this.DeactivateGreyZoneAuthority();
				return;
			}
		}
		else if (this.GreyZoneAvailable)
		{
			this.roomPlayerList = PhotonNetwork.PlayerList;
			int num2 = 1;
			if (this.roomPlayerList != null && this.roomPlayerList.Length != 0)
			{
				num2 = Mathf.Max((this.roomPlayerList.Length + 1) / 2, 1);
			}
			float num3 = 0f;
			float num4 = 1f / this.summoningActivationTime;
			foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
			{
				VRRig item = keyValuePair.Value.Item1;
				GreyZoneSummoner item2 = keyValuePair.Value.Item2;
				float num5 = this.summoningPlayerProgress[keyValuePair.Key];
				Vector3 lhs = item2.SummoningFocusPoint - item.leftHand.rigTarget.position;
				Vector3 rhs = -item.leftHand.rigTarget.right;
				bool flag = Vector3.Dot(lhs, rhs) > 0f;
				Vector3 lhs2 = item2.SummoningFocusPoint - item.rightHand.rigTarget.position;
				Vector3 right = item.rightHand.rigTarget.right;
				bool flag2 = Vector3.Dot(lhs2, right) > 0f;
				if (flag && flag2)
				{
					num5 = Mathf.MoveTowards(num5, 1f, num4 * deltaTime);
				}
				else
				{
					num5 = Mathf.MoveTowards(num5, 0f, num4 * deltaTime);
				}
				num3 += num5;
				this.summoningPlayerProgress[keyValuePair.Key] = num5;
			}
			float num6 = 0.95f;
			this.summoningProgress = Mathf.Clamp01(num3 / num6 / (float)num2);
			this.UpdateSummonerVisuals();
			if (this.summoningProgress > 0.99f)
			{
				this.ActivateGreyZoneAuthority();
			}
		}
	}

	// Token: 0x06000EE4 RID: 3812 RVA: 0x00051B04 File Offset: 0x0004FD04
	private void SharedUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.greyZoneActive)
		{
			Vector3 b = Vector3.ClampMagnitude(instance.InstantaneousVelocity * this.particlePredictiveSpawnVelocityFactor, this.particlePredictiveSpawnMaxDist);
			this.greyZoneParticles.transform.position = instance.HeadCenterPosition + Vector3.down * 0.5f + b;
		}
		else if (this.gravityOverrideSet && this.gravityReductionAmount < 0.01f)
		{
			instance.UnsetGravityOverride(this);
			this.gravityOverrideSet = false;
		}
		float num = this.greyZoneActive ? 0f : 1f;
		float smoothTime = this.greyZoneActive ? this.skyMonsterMovementEnterTime : this.skyMonsterMovementExitTime;
		if (this.moonController != null && this.moonController.Distance != num)
		{
			float num2 = Mathf.SmoothDamp(this.moonController.Distance, num, ref this.skyMonsterMovementVelocity, smoothTime);
			if ((double)Mathf.Abs(num2 - num) < 0.001)
			{
				num2 = num;
			}
			this.moonController.UpdateDistance(num2);
		}
	}

	// Token: 0x06000EE5 RID: 3813 RVA: 0x00051C18 File Offset: 0x0004FE18
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.greyZoneActive);
			stream.SendNext(this.greyZoneActivationTime);
			stream.SendNext(this.photonConnectedDuringActivation);
			stream.SendNext(this.gravityFactorOptionSelection);
			stream.SendNext(this.summoningProgress);
			return;
		}
		if (stream.IsReading && info.Sender.IsMasterClient)
		{
			bool flag = this.greyZoneActive;
			this.greyZoneActive = (bool)stream.ReceiveNext();
			this.greyZoneActivationTime = ((double)stream.ReceiveNext()).GetFinite();
			this.photonConnectedDuringActivation = (bool)stream.ReceiveNext();
			this.gravityFactorOptionSelection = (int)stream.ReceiveNext();
			this.gravityFactorOptionSelection = Mathf.Clamp(this.gravityFactorOptionSelection, 0, this.gravityFactorOptions.Length - 1);
			this.summoningProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.UpdateSummonerVisuals();
			if (this.greyZoneActive && !flag)
			{
				this.ActivateGreyZoneLocal();
				return;
			}
			if (!this.greyZoneActive && flag)
			{
				this.DeactivateGreyZoneLocal();
			}
		}
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x00051D55 File Offset: 0x0004FF55
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x06000EE8 RID: 3816 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x00051D55 File Offset: 0x0004FF55
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x040011EB RID: 4587
	[OnEnterPlay_SetNull]
	public static volatile GreyZoneManager Instance;

	// Token: 0x040011EC RID: 4588
	[SerializeField]
	private float greyZoneActiveDuration = 90f;

	// Token: 0x040011ED RID: 4589
	[SerializeField]
	private float[] gravityFactorOptions = new float[]
	{
		0.25f,
		0.5f,
		0.75f
	};

	// Token: 0x040011EE RID: 4590
	[SerializeField]
	private int gravityFactorOptionSelection = 1;

	// Token: 0x040011EF RID: 4591
	[SerializeField]
	private float summoningActivationTime = 3f;

	// Token: 0x040011F0 RID: 4592
	[SerializeField]
	private AudioSource greyZoneAmbience;

	// Token: 0x040011F1 RID: 4593
	[SerializeField]
	private float ambienceFadeTime = 4f;

	// Token: 0x040011F2 RID: 4594
	[SerializeField]
	private bool forceTimeOfDayToNight;

	// Token: 0x040011F3 RID: 4595
	[SerializeField]
	private float skyMonsterMovementEnterTime = 4.5f;

	// Token: 0x040011F4 RID: 4596
	[SerializeField]
	private float skyMonsterMovementExitTime = 3.2f;

	// Token: 0x040011F5 RID: 4597
	[SerializeField]
	private float skyMonsterDistGravityRampBuffer = 0.15f;

	// Token: 0x040011F6 RID: 4598
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityReductionAmount = 1f;

	// Token: 0x040011F7 RID: 4599
	private float simpleGravityFactor = 1f;

	// Token: 0x040011F8 RID: 4600
	[SerializeField]
	private ParticleSystem greyZoneParticles;

	// Token: 0x040011F9 RID: 4601
	[SerializeField]
	private float particlePredictiveSpawnMaxDist = 4f;

	// Token: 0x040011FA RID: 4602
	[SerializeField]
	private float particlePredictiveSpawnVelocityFactor = 0.5f;

	// Token: 0x040011FB RID: 4603
	private bool photonConnectedDuringActivation;

	// Token: 0x040011FC RID: 4604
	private double greyZoneActivationTime;

	// Token: 0x040011FD RID: 4605
	private bool greyZoneActive;

	// Token: 0x040011FE RID: 4606
	private bool _tickRunning;

	// Token: 0x040011FF RID: 4607
	private float summoningProgress;

	// Token: 0x04001200 RID: 4608
	private List<GreyZoneSummoner> activeSummoners = new List<GreyZoneSummoner>();

	// Token: 0x04001201 RID: 4609
	private Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>> summoningPlayers = new Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>>();

	// Token: 0x04001202 RID: 4610
	private Dictionary<int, float> summoningPlayerProgress = new Dictionary<int, float>();

	// Token: 0x04001203 RID: 4611
	private HashSet<int> invalidSummoners = new HashSet<int>();

	// Token: 0x04001204 RID: 4612
	private Coroutine audioFadeCoroutine;

	// Token: 0x04001205 RID: 4613
	private Player[] roomPlayerList;

	// Token: 0x04001206 RID: 4614
	private ShaderHashId _GreyZoneActive = new ShaderHashId("_GreyZoneActive");

	// Token: 0x04001207 RID: 4615
	private MoonController moonController;

	// Token: 0x04001208 RID: 4616
	private float skyMonsterMovementVelocity;

	// Token: 0x04001209 RID: 4617
	private bool gravityOverrideSet;

	// Token: 0x0400120A RID: 4618
	private float greyZoneAmbienceVolume = 0.15f;

	// Token: 0x0400120B RID: 4619
	private int greyZoneAvailableDayOfYear = new DateTime(2024, 10, 25).DayOfYear;

	// Token: 0x0400120C RID: 4620
	public Action OnGreyZoneActivated;

	// Token: 0x0400120D RID: 4621
	public Action OnGreyZoneDeactivated;
}
