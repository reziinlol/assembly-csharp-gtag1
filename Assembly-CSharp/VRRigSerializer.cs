using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x02000847 RID: 2119
[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x060036B5 RID: 14005 RVA: 0x0012D231 File Offset: 0x0012B431
	// (set) Token: 0x060036B6 RID: 14006 RVA: 0x0012D25B File Offset: 0x0012B45B
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x060036B7 RID: 14007 RVA: 0x0012D286 File Offset: 0x0012B486
	// (set) Token: 0x060036B8 RID: 14008 RVA: 0x0012D2B4 File Offset: 0x0012B4B4
	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x060036B9 RID: 14009 RVA: 0x0012D2E3 File Offset: 0x0012B4E3
	// (set) Token: 0x060036BA RID: 14010 RVA: 0x0012D311 File Offset: 0x0012B511
	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x060036BB RID: 14011 RVA: 0x0012D340 File Offset: 0x0012B540
	private PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
	}

	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x060036BC RID: 14012 RVA: 0x0012D348 File Offset: 0x0012B548
	public VRRig VRRig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x060036BD RID: 14013 RVA: 0x0012D350 File Offset: 0x0012B550
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x060036BE RID: 14014 RVA: 0x0012D35D File Offset: 0x0012B55D
	// (set) Token: 0x060036BF RID: 14015 RVA: 0x0012D365 File Offset: 0x0012B565
	public InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> SuccesfullSpawnEvent { get; private set; } = new InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped>(2);

	// Token: 0x060036C0 RID: 14016 RVA: 0x0012D370 File Offset: 0x0012B570
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.netView.IsRoomView)
		{
			if (player != null)
			{
				MonkeAgent.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				MonkeAgent.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (player != this.netView.Owner)
		{
			MonkeAgent.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	// Token: 0x060036C1 RID: 14017 RVA: 0x0012D478 File Offset: 0x0012B678
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		bool initialized = this.rigContainer.Initialized;
		this.rigContainer.InitializeNetwork(this.netView, this.Voice, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.SetupLoudSpeakerNetwork(this.rigContainer);
		this.netView.GetView.AddCallbackTarget(this);
		if (!initialized)
		{
			object[] instantiationData = info.punInfo.photonView.InstantiationData;
			float red = 0f;
			float green = 0f;
			float blue = 0f;
			if (instantiationData != null && instantiationData.Length == 3)
			{
				object obj = instantiationData[0];
				if (obj is float)
				{
					float value = (float)obj;
					obj = instantiationData[1];
					if (obj is float)
					{
						float value2 = (float)obj;
						obj = instantiationData[2];
						if (obj is float)
						{
							float value3 = (float)obj;
							red = value.ClampSafe(0f, 1f);
							green = value2.ClampSafe(0f, 1f);
							blue = value3.ClampSafe(0f, 1f);
						}
					}
				}
			}
			this.vrrig.InitializeNoobMaterialLocal(red, green, blue);
		}
		this.SuccesfullSpawnEvent.InvokeSafe(this.rigContainer, info);
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
		if (VRRigCache.isInitialized)
		{
			VRRigCache.Instance.OnVrrigSerializerSuccesfullySpawned();
		}
	}

	// Token: 0x060036C2 RID: 14018 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x0012D5E4 File Offset: 0x0012B7E4
	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x0012D5F0 File Offset: 0x0012B7F0
	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else if (this.vrrig.isOfflineVRRig)
			{
				NetworkSystem.Instance.NetDestroy(base.gameObject);
			}
			if (this.vrrig.netView == this.netView)
			{
				this.vrrig.netView = null;
			}
			if (this.vrrig.rigSerializer == this)
			{
				this.vrrig.rigSerializer = null;
			}
		}
		if (this.networkSpeaker != null)
		{
			this.CleanupLoudSpeakerNetwork();
			this.networkSpeaker.gameObject.SetActive(false);
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
		}
		this.vrrig = null;
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x0012D6E7 File Offset: 0x0012B8E7
	private void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.CleanUp(false);
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x0012D6F6 File Offset: 0x0012B8F6
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			UnityEngine.Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x0012D734 File Offset: 0x0012B934
	[PunRPC]
	public void RPC_InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		this.InitializeNoobMaterialShared(red, green, blue, info);
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x0012D746 File Offset: 0x0012B946
	[PunRPC]
	public void RPC_RequestCosmetics(PhotonMessageInfo info)
	{
		this.RequestCosmeticsShared(info);
	}

	// Token: 0x060036C9 RID: 14025 RVA: 0x0012D754 File Offset: 0x0012B954
	[PunRPC]
	public void RPC_PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.PlayDrumShared(drumIndex, drumVolume, info);
	}

	// Token: 0x060036CA RID: 14026 RVA: 0x0012D764 File Offset: 0x0012B964
	[PunRPC]
	public void RPC_PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.PlaySelfOnlyInstrumentShared(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x060036CB RID: 14027 RVA: 0x0012D776 File Offset: 0x0012B976
	[PunRPC]
	public void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.PlayHandTapShared(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x0012D788 File Offset: 0x0012B988
	public void RPC_UpdateNativeSize(float value, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.UpdateNativeSizeShared(value, info);
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void RPC_UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void RPC_UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x060036CF RID: 14031 RVA: 0x0012D797 File Offset: 0x0012B997
	[PunRPC]
	public void RPC_UpdateCosmeticsWithTryonPacked(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfo info)
	{
		this.UpdateCosmeticsWithTryonShared(currentItemsPacked, tryOnItemsPacked, playfx, info);
	}

	// Token: 0x060036D0 RID: 14032 RVA: 0x0012D7A9 File Offset: 0x0012B9A9
	[PunRPC]
	public void RPC_UpdateCosmeticsWithCollectablesPacked(int[] data, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithCollectables(data ?? Array.Empty<int>(), info);
	}

	// Token: 0x060036D1 RID: 14033 RVA: 0x0012D7CB File Offset: 0x0012B9CB
	[PunRPC]
	public void RPC_SetCollectionCycleIndex(int[] data, PhotonMessageInfo info)
	{
		if (data == null || data.Length != 2)
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetCollectionCycleIndex(data[0], data[1], info);
	}

	// Token: 0x060036D2 RID: 14034 RVA: 0x0012D7F3 File Offset: 0x0012B9F3
	[PunRPC]
	public void RPC_HideAllCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.HideAllCosmetics(info);
	}

	// Token: 0x060036D3 RID: 14035 RVA: 0x0012D806 File Offset: 0x0012BA06
	[PunRPC]
	public void RPC_PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.PlaySplashEffectShared(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x060036D4 RID: 14036 RVA: 0x0012D81E File Offset: 0x0012BA1E
	[PunRPC]
	public void RPC_PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.PlayGeodeEffectShared(hitPosition, info);
	}

	// Token: 0x060036D5 RID: 14037 RVA: 0x0012D82D File Offset: 0x0012BA2D
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.EnableNonCosmeticHandItemShared(enable, isLeftHand, info);
	}

	// Token: 0x060036D6 RID: 14038 RVA: 0x0012D83D File Offset: 0x0012BA3D
	[PunRPC]
	public void OnHandTapRPC(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfo info)
	{
		this.OnHandTapRPCShared(audioClipIndex, isDownTap, isLeftHand, stiltID, handTapSpeed, packedDirFromHitToHand, info);
	}

	// Token: 0x060036D7 RID: 14039 RVA: 0x0012D855 File Offset: 0x0012BA55
	[PunRPC]
	public void RPC_UpdateQuestScore(int score, PhotonMessageInfo info)
	{
		this.UpdateQuestScore(score, info);
	}

	// Token: 0x060036D8 RID: 14040 RVA: 0x0012D864 File Offset: 0x0012BA64
	[PunRPC]
	public void RPC_UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfo info)
	{
		this.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x060036D9 RID: 14041 RVA: 0x0012D878 File Offset: 0x0012BA78
	private void SetupLoudSpeakerNetwork(RigContainer rigContainer)
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.AddSpeaker(component);
		}
	}

	// Token: 0x060036DA RID: 14042 RVA: 0x0012D8F0 File Offset: 0x0012BAF0
	private void CleanupLoudSpeakerNetwork()
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in this.rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.RemoveSpeaker(component);
		}
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x0012D96C File Offset: 0x0012BB6C
	public void BroadcastLoudSpeakerNetwork(bool toggleBroadcast, int actorNumber)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
		{
			return;
		}
		bool isLocal = actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.BroadcastLoudSpeakerNetworkShared(toggleBroadcast, rigContainer, actorNumber, isLocal);
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x0012D9B0 File Offset: 0x0012BBB0
	private void BroadcastLoudSpeakerNetworkShared(bool toggleBroadcast, RigContainer rigContainer, int actorNumber, bool isLocal)
	{
		this.SetupLoudSpeakerNetwork(rigContainer);
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			if (toggleBroadcast)
			{
				loudSpeakerNetwork.BroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
			else
			{
				loudSpeakerNetwork.StopBroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
		}
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x0012DA1C File Offset: 0x0012BC1C
	[PunRPC]
	public void GrabbedByPlayer(bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand, PhotonMessageInfo info)
	{
		GorillaGuardianManager gorillaGuardianManager = GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.IsPlayerGuardian(info.Sender))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.vrrig.GrabbedByPlayer(rigContainer.Rig, grabbedBody, grabbedLeftHand, grabbedWithLeftHand);
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x0012DA78 File Offset: 0x0012BC78
	[PunRPC]
	public void DroppedByPlayer(Vector3 throwVelocity, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "DroppedByPlayer");
		RigContainer rigContainer;
		if (this.vrrig.isOfflineVRRig && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			float num = 10000f;
			if (throwVelocity.IsValid(num))
			{
				this.vrrig.DroppedByPlayer(rigContainer.Rig, throwVelocity);
				return;
			}
		}
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x0012DAD5 File Offset: 0x0012BCD5
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x0012DAF4 File Offset: 0x0012BCF4
	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x0012DB0C File Offset: 0x0012BD0C
	private void OnHandTapRPCShared(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnHandTapRPCShared");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		if (audioClipIndex < 0 || audioClipIndex >= GTPlayer.Instance.materialData.Count)
		{
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink = isLeftHand ? this.vrrig.rightHandLink : this.vrrig.leftHandLink;
		NetPlayer grabbedPlayer = takeMyHand_HandLink.grabbedPlayer;
		if (grabbedPlayer != null && grabbedPlayer.IsLocal)
		{
			(takeMyHand_HandLink.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).PlayVicariousTapHaptic();
		}
		Vector3 tapDir = Utils.UnpackVector3FromLong(packedDirFromHitToHand);
		if (!Mathf.Approximately(tapDir.sqrMagnitude, 1f))
		{
			tapDir.Normalize();
		}
		float max = GorillaTagger.Instance.DefaultHandTapVolume;
		GorillaAmbushManager gorillaAmbushManager = GorillaGameModes.GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(this.rigContainer.Creator))
		{
			max = gorillaAmbushManager.crawlingSpeedForMaxVolume;
		}
		OnHandTapFX onHandTapFX = new OnHandTapFX
		{
			rig = this.vrrig,
			surfaceIndex = audioClipIndex,
			isDownTap = isDownTap,
			isLeftHand = isLeftHand,
			stiltID = stiltID,
			volume = handTapSpeed.ClampSafe(0f, max),
			speed = handTapSpeed,
			tapDir = tapDir
		};
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[isLeftHand ? 0 : 2].actorSet;
			if (crittersLoudNoise.IsNotNull())
			{
				crittersLoudNoise.PlayHandTapRemote(info.SentServerTime, isLeftHand);
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GTZone.ghostReactor);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			Vector3 tapPos = isLeftHand ? this.vrrig.leftHand.rigTarget.position : this.vrrig.rightHand.rigTarget.position;
			managerForZone.ghostReactorManager.OnSharedTap(this.vrrig, tapPos, handTapSpeed);
		}
		FXSystem.PlayFXForRig<HandEffectContext>(FXType.OnHandTap, onHandTapFX, info);
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x0012DD58 File Offset: 0x0012BF58
	private void PlayHandTapShared(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		MonkeAgent.IncrementRPCCall(info, "PlayHandTapShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent hand tap", sender.UserId, sender.NickName);
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x0012DDF8 File Offset: 0x0012BFF8
	private void UpdateNativeSizeShared(float value, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		MonkeAgent.IncrementRPCCall(info, "UpdateNativeSizeShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && RPCUtil.SafeValue(value, 0.1f, 10f) && RPCUtil.NotSpam("UpdateNativeSizeShared", info, 1f))
		{
			if (this.vrrig != null)
			{
				this.vrrig.NativeScale = value;
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent native size", sender.UserId, sender.NickName);
		}
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x0012DE88 File Offset: 0x0012C088
	private void PlayGeodeEffectShared(Vector3 hitPosition, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "PlayGeodeEffectShared");
		if (info.Sender == this.netView.Owner)
		{
			float num = 10000f;
			if (hitPosition.IsValid(num))
			{
				this.geoSoundArg.position = hitPosition;
				FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
				return;
			}
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x0012DF06 File Offset: 0x0012C106
	private void InitializeNoobMaterialShared(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, info);
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x0012DF1D File Offset: 0x0012C11D
	private void RequestMaterialColorShared(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, info);
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x0012DF34 File Offset: 0x0012C134
	private void RequestCosmeticsShared(PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestCosmetics");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[9].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x0012DF96 File Offset: 0x0012C196
	private void PlayDrumShared(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x0012DFAB File Offset: 0x0012C1AB
	private void PlaySelfOnlyInstrumentShared(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x0012DFC2 File Offset: 0x0012C1C2
	private void UpdateCosmeticsWithTryonShared(int[] currentItems, int[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, playfx, info);
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x0012DFD9 File Offset: 0x0012C1D9
	private void PlaySplashEffectShared(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x060036EC RID: 14060 RVA: 0x0012DFF6 File Offset: 0x0012C1F6
	private void EnableNonCosmeticHandItemShared(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	// Token: 0x060036ED RID: 14061 RVA: 0x0012E00B File Offset: 0x0012C20B
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateQuestScore(score, info);
	}

	// Token: 0x060036EE RID: 14062 RVA: 0x0012E01F File Offset: 0x0012C21F
	public void UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x060036F0 RID: 14064 RVA: 0x0012E060 File Offset: 0x0012C260
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x0012E090 File Offset: 0x0012C290
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	// Token: 0x040046FA RID: 18170
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _nickName;

	// Token: 0x040046FB RID: 18171
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _defaultName;

	// Token: 0x040046FC RID: 18172
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private bool _tutorialComplete;

	// Token: 0x040046FD RID: 18173
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x040046FE RID: 18174
	public Transform networkSpeaker;

	// Token: 0x040046FF RID: 18175
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04004700 RID: 18176
	private RigContainer rigContainer;

	// Token: 0x04004701 RID: 18177
	private HandTapArgs handTapArgs = new HandTapArgs();

	// Token: 0x04004702 RID: 18178
	private GeoSoundArg geoSoundArg = new GeoSoundArg();
}
