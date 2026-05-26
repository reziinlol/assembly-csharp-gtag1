using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.Unity;
using Steamworks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020008B2 RID: 2226
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x06003A11 RID: 14865 RVA: 0x0013C1E0 File Offset: 0x0013A3E0
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x06003A12 RID: 14866 RVA: 0x0013C1E7 File Offset: 0x0013A3E7
	public bool ForcePerfRefreshRate
	{
		get
		{
			return this._forcePerfRefreshRate;
		}
	}

	// Token: 0x06003A13 RID: 14867 RVA: 0x0013C1F0 File Offset: 0x0013A3F0
	public void SetExtraHandPosition(StiltID stiltID, Vector3 position, bool canTag, bool canStun)
	{
		this.stiltTagData[(int)stiltID].currentPositionForTag = position;
		this.stiltTagData[(int)stiltID].hasCurrentPosition = true;
		this.stiltTagData[(int)stiltID].canTag = canTag;
		this.stiltTagData[(int)stiltID].canStun = canStun;
	}

	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x06003A14 RID: 14868 RVA: 0x0013C246 File Offset: 0x0013A446
	public NetworkView myVRRig
	{
		get
		{
			return this.offlineVRRig.netView;
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x06003A15 RID: 14869 RVA: 0x0013C253 File Offset: 0x0013A453
	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x06003A16 RID: 14870 RVA: 0x0013C260 File Offset: 0x0013A460
	public bool PerformanceOn
	{
		get
		{
			return this._performanceOn;
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x06003A17 RID: 14871 RVA: 0x0013C268 File Offset: 0x0013A468
	// (set) Token: 0x06003A18 RID: 14872 RVA: 0x0013C270 File Offset: 0x0013A470
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x06003A19 RID: 14873 RVA: 0x0013C279 File Offset: 0x0013A479
	public float DefaultHandTapVolume
	{
		get
		{
			return this.cacheHandTapVolume;
		}
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x06003A1A RID: 14874 RVA: 0x0013C281 File Offset: 0x0013A481
	// (set) Token: 0x06003A1B RID: 14875 RVA: 0x0013C289 File Offset: 0x0013A489
	public Recorder myRecorder { get; private set; }

	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06003A1C RID: 14876 RVA: 0x0013C292 File Offset: 0x0013A492
	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003A1D RID: 14877 RVA: 0x0013C2B4 File Offset: 0x0013A4B4
	// (remove) Token: 0x06003A1E RID: 14878 RVA: 0x0013C2EC File Offset: 0x0013A4EC
	public event Action<bool, Vector3, Vector3> OnHandTap;

	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x06003A1F RID: 14879 RVA: 0x0013C321 File Offset: 0x0013A521
	// (set) Token: 0x06003A20 RID: 14880 RVA: 0x0013C329 File Offset: 0x0013A529
	public bool hasTappedSurface { get; private set; }

	// Token: 0x06003A21 RID: 14881 RVA: 0x0013C332 File Offset: 0x0013A532
	public void ResetTappedSurfaceCheck()
	{
		this.hasTappedSurface = false;
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x0013C33B File Offset: 0x0013A53B
	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x0013C354 File Offset: 0x0013A554
	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.RecoverMissingRefs();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		this.stiltTagData[0].isLeftHand = true;
		this.stiltTagData[4].isLeftHand = true;
		this.stiltTagData[5].isLeftHand = true;
		this.stiltTagData[2].isLeftHand = true;
		this.stiltTagData[6].isLeftHand = true;
		this.stiltTagData[7].isLeftHand = true;
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			Action action = GorillaTagger.onPlayerSpawnedRootCallback;
			if (action != null)
			{
				action();
			}
		}
		GRFirstTimeUserExperience grfirstTimeUserExperience = Object.FindAnyObjectByType<GRFirstTimeUserExperience>(FindObjectsInactive.Include);
		GameObject gameObject = (grfirstTimeUserExperience != null) ? grfirstTimeUserExperience.gameObject : null;
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.AppVersion != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GTPlayer.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
			bool flag = true;
			if (gameObject != null && PlayerPrefs.GetString("spawnInWrongStump") == "flagged" && flag)
			{
				gameObject.SetActive(true);
				GRFirstTimeUserExperience grfirstTimeUserExperience2;
				if (gameObject.TryGetComponent<GRFirstTimeUserExperience>(out grfirstTimeUserExperience2) && grfirstTimeUserExperience2.spawnPoint != null)
				{
					GTPlayer.Instance.TeleportTo(grfirstTimeUserExperience2.spawnPoint.position, grfirstTimeUserExperience2.spawnPoint.rotation, false, false);
					GTPlayer.Instance.InitializeValues();
					PlayerPrefs.DeleteKey("spawnInWrongStump");
					PlayerPrefs.Save();
				}
			}
		}
		this.thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		this.wasInOverlay = false;
		this.baseSlideControl = GTPlayer.Instance.slideControl;
		this.gorillaTagColliderLayerMask = UnityLayer.GorillaTagCollider.ToLayerMask();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.cacheHandTapVolume = this.handTapVolume;
		OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.Medium;
		this._leftHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
		this._rightHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
		this.ClearFramerateTracker();
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x0013C679 File Offset: 0x0013A879
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x0013C694 File Offset: 0x0013A894
	private void IsXRSubsystemActive()
	{
		GorillaTagger.<IsXRSubsystemActive>d__149 <IsXRSubsystemActive>d__;
		<IsXRSubsystemActive>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<IsXRSubsystemActive>d__.<>4__this = this;
		<IsXRSubsystemActive>d__.<>1__state = -1;
		<IsXRSubsystemActive>d__.<>t__builder.Start<GorillaTagger.<IsXRSubsystemActive>d__149>(ref <IsXRSubsystemActive>d__);
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x0013C6CB File Offset: 0x0013A8CB
	public bool IsOculusQuest2()
	{
		return Application.platform == RuntimePlatform.Android && OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x0013C6E4 File Offset: 0x0013A8E4
	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			Quaternion rotation = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion rotation2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion lhs = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion lhs2 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GTPlayer.Instance.SetHandOffsets(true, new Vector3(-0.02f, 0f, -0.07f), lhs * Quaternion.Inverse(rotation));
			GTPlayer.Instance.SetHandOffsets(false, new Vector3(0.02f, 0f, -0.07f), lhs2 * Quaternion.Inverse(rotation2));
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	// Token: 0x06003A28 RID: 14888 RVA: 0x0013C81A File Offset: 0x0013AA1A
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = (pCallback.m_bActive > 0);
	}

	// Token: 0x06003A29 RID: 14889 RVA: 0x0013C82B File Offset: 0x0013AA2B
	[ContextMenu("Toggle Performance Refresh Rate")]
	public void ToggleForcedPerformanceRefresh()
	{
		this.SetForcedRefreshRate(true, 72f);
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x0013C839 File Offset: 0x0013AA39
	public void ToggleDefaultPerformanceRefresh()
	{
		this.SetForcedRefreshRate(false, this._defaultRefreshRate);
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x0013C848 File Offset: 0x0013AA48
	public void ToggleForcedRefreshRate(float newRefreshRate = 90f)
	{
		this.SetForcedRefreshRate(!this._forcePerfRefreshRate, newRefreshRate);
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x0013C85C File Offset: 0x0013AA5C
	public void SetForcedRefreshRate(bool forcePerf, float newRefreshRate = 90f)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - {0} / {1}", forcePerf, newRefreshRate));
		this._framerateUpdated = false;
		this._forceFramerateCheck = true;
		this._forcePerfRefreshRate = forcePerf;
		this._perfRefreshRate = Mathf.Clamp(newRefreshRate, 32f, 144f);
		this._performanceOn = (newRefreshRate <= 72f);
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - New refresh {0} with perf {1}", this._perfRefreshRate, this._performanceOn));
		this.UpdateResolutionScale(this._performanceOn);
		if (forcePerf)
		{
			DebugHudStats.FPS_THRESHOLD = (int)this._perfRefreshRate - 1;
		}
		else
		{
			DebugHudStats.FPS_THRESHOLD = (int)this._defaultRefreshRate - 1;
		}
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - New DebugHudStats FPS threshold {0}", DebugHudStats.FPS_THRESHOLD));
	}

	// Token: 0x06003A2D RID: 14893 RVA: 0x0013C938 File Offset: 0x0013AB38
	private void ClearFramerateTracker()
	{
		this._framerateIndex = 0;
		this._framerateTotal = 0f;
		for (int i = 0; i < this._framerateTracker.Length; i++)
		{
			this._framerateTracker[i] = 0f;
		}
	}

	// Token: 0x06003A2E RID: 14894 RVA: 0x0013C978 File Offset: 0x0013AB78
	private void UpdateResolutionScale(bool performanceMode)
	{
		float num = 1f;
		if (performanceMode)
		{
			num = 0.975f;
			if (Application.platform == RuntimePlatform.Android)
			{
				num = 0.95f;
				if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2)
				{
					num = 0.9f;
				}
			}
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			num = 0.975f;
			if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2)
			{
				num = 0.925f;
			}
		}
		XRSettings.eyeTextureResolutionScale = num;
		XRSettings.renderViewportScale = num;
		Debug.Log(string.Format("GorillaTagger - UpdateResolutionScale - {0}", num));
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x0013C9F4 File Offset: 0x0013ABF4
	protected void LateUpdate()
	{
		GorillaTagger.<>c__DisplayClass159_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = false;
		}
		this._framerateTimer -= Time.deltaTime;
		if (this._framerateTimer <= 0f)
		{
			this._framerateTimer += 0.1f;
			if (Time.smoothDeltaTime > 0f)
			{
				float num = 1f / Time.smoothDeltaTime;
				this._framerateTotal -= this._framerateTracker[this._framerateIndex];
				this._framerateTracker[this._framerateIndex] = num;
				this._framerateTotal += num;
				this._framerateIndex++;
				if (this._framerateIndex >= this._framerateTracker.Length)
				{
					this._framerateIndex = 0;
				}
				this._prevSmoothedFramerate = this.SmoothedFramerate;
				this.SmoothedFramerate = Mathf.RoundToInt(this._framerateTotal / (float)this._framerateTracker.Length);
				int smoothedFramerate = this.SmoothedFramerate;
				int fps_THRESHOLD = DebugHudStats.FPS_THRESHOLD;
			}
		}
		if (this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && this.activeXRDisplay != null && this.activeXRDisplay.TryGetDisplayRefreshRate(out this._defaultRefreshRate))
		{
			float num2 = this._forcePerfRefreshRate ? this._perfRefreshRate : this._defaultRefreshRate;
			float num3 = 1f / num2;
			if (num2 > 0f)
			{
				DebugHudStats.FPS_THRESHOLD = (int)num2 - 1;
			}
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num3) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				Debug.Log(" =========== Adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" Refresh rate         :\t" + num2.ToString());
				Time.fixedDeltaTime = num3;
				this.UpdateResolutionScale(num2 < this._defaultRefreshRate);
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" History size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(num2 * 0.083333336f), 10), 6);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log("New history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(num2);
				GTPlayer.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		else if ((this._forceFramerateCheck && OVRManager.instance != null) || (!this._framerateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf))
		{
			InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
			int num4 = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num5 = OVRManager.display.displayFrequenciesAvailable[num4];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			while (num5 > 90f)
			{
				num4--;
				if (num4 < 0)
				{
					break;
				}
				num5 = OVRManager.display.displayFrequenciesAvailable[num4];
			}
			this._defaultRefreshRate = num5;
			if (this._forcePerfRefreshRate)
			{
				num5 = this._perfRefreshRate;
			}
			float num6 = 1f;
			float num7 = 1f / num5;
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num7 * num6) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				float num8 = Time.fixedDeltaTime - num7 * num6;
				Debug.Log(" =========== ADJUSTING REFRESH SIZE ========= ");
				Debug.Log(string.Format("!!!! Time.fixedDeltaTime - (1f / newRefreshRate) * {0}) {1}", num6, num8));
				Debug.Log(string.Format("Old Refresh rate: {0}", systemDisplayFrequency));
				Debug.Log(string.Format("New Refresh rate: {0}", num5));
				Debug.Log(string.Format("   fixedDeltaTime before:\t{0}", Time.fixedDeltaTime));
				Debug.Log(string.Format("   fixedDeltaTime after :\t{0}", num7));
				Application.targetFrameRate = (int)num5;
				Time.fixedDeltaTime = num7 * num6;
				OVRPlugin.systemDisplayFrequency = num5;
				this.UpdateResolutionScale(num5 <= 72f);
				GTPlayer.Instance.velocityHistorySize = Mathf.FloorToInt(num5 * 0.083333336f);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(string.Format("   FixedDeltaTime after :\t{0}", Time.fixedDeltaTime));
				Debug.Log(string.Format("   History size before  :\t{0}", GTPlayer.Instance.velocityHistorySize));
				Debug.Log(string.Format("New history size: {0}", GTPlayer.Instance.velocityHistorySize));
				Debug.Log(" ============================================ ");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this._framerateUpdated = true;
				this.ConfirmUpdatedFrameRate();
			}
		}
		else if (!this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			this._defaultRefreshRate = 144f;
			int num9 = this._forcePerfRefreshRate ? ((int)this._perfRefreshRate) : ((int)this._defaultRefreshRate);
			float num10 = 1f / (float)num9;
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num10) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				Debug.Log(string.Format("Updating delta time. Was: {0}. Now it's {1} at framerate {2}.", Time.fixedDeltaTime, num10, num9));
				Application.targetFrameRate = num9;
				Time.fixedDeltaTime = num10;
				this.UpdateResolutionScale((float)num9 < this._defaultRefreshRate);
				GTPlayer.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt((float)num9 * 0.083333336f), 10);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(string.Format("New history size: {0}", GTPlayer.Instance.velocityHistorySize));
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl((float)num9);
				GTPlayer.Instance.InitializeValues();
			}
		}
		this.otherPlayer = null;
		this.touchedPlayer = null;
		CS$<>8__locals1.otherTouchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = null;
		}
		Vector3 position = this.leftHandTransform.position;
		Vector3 position2 = this.rightHandTransform.position;
		Vector3 position3 = this.headCollider.transform.position;
		Vector3 position4 = this.bodyCollider.transform.position;
		float scale = GTPlayer.Instance.scale;
		float num11 = this.sphereCastRadius * scale;
		CS$<>8__locals1.bodyHit = false;
		CS$<>8__locals1.leftHandHit = false;
		CS$<>8__locals1.canTagHit = false;
		CS$<>8__locals1.canStunHit = false;
		if (!(GorillaGameManager.instance is CasualGameMode))
		{
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastLeftHandPositionForTag, position, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastRightHandPositionForTag, position2, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position2, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			for (int i = 0; i < 12; i++)
			{
				GorillaTagger.StiltTagData stiltTagData = this.stiltTagData[i];
				if (stiltTagData.hasLastPosition && stiltTagData.hasCurrentPosition && (stiltTagData.canTag || stiltTagData.canStun))
				{
					this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(stiltTagData.currentPositionForTag, stiltTagData.lastPositionForTag, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
					this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(i == 0 || i == 2, this.maxStiltTagDistance, stiltTagData.canTag, stiltTagData.canStun, ref CS$<>8__locals1);
				}
			}
			this.topVector = this.lastHeadPositionForTag;
			this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
			this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GTPlayer.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num11), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsCapsulecast|159_1(this.maxTagDistance, true, false, ref CS$<>8__locals1);
		}
		if (this.otherPlayer != null)
		{
			if (CS$<>8__locals1.canTagHit && (!CS$<>8__locals1.canStunHit || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.otherPlayer)))
			{
				GameMode.ActiveGameMode.LocalTag(this.otherPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.bodyHit, CS$<>8__locals1.leftHandHit);
				GameMode.ReportTag(this.otherPlayer);
			}
			if (CS$<>8__locals1.canStunHit)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, this.otherPlayer);
			}
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null && GorillaGameManager.instance != null)
		{
			CustomGameMode.TouchPlayer(CS$<>8__locals1.otherTouchedPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null)
		{
			this.HitWithKnockBack(CS$<>8__locals1.otherTouchedPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.leftHandHit);
		}
		bool flag = true;
		StiltID stiltID = StiltID.None;
		this.ProcessHandTapping(flag, stiltID, ref this.lastLeftTap, ref this.lastLeftUpTap, ref this.leftHandWasTouching, this.leftHandSlideSource);
		flag = false;
		stiltID = StiltID.None;
		this.ProcessHandTapping(flag, stiltID, ref this.lastRightTap, ref this.lastRightUpTap, ref this.rightHandWasTouching, this.rightHandSlideSource);
		for (int j = 0; j < 12; j++)
		{
			GorillaTagger.StiltTagData stiltTagData2 = this.stiltTagData[j];
			if (stiltTagData2.hasLastPosition && stiltTagData2.hasCurrentPosition)
			{
				stiltID = (StiltID)j;
				this.ProcessHandTapping(stiltTagData2.isLeftHand, stiltID, ref stiltTagData2.lastTap, ref stiltTagData2.lastUpTap, ref stiltTagData2.wasTouching, this.leftHandSlideSource);
				this.stiltTagData[j] = stiltTagData2;
			}
		}
		this.CheckEndStatusEffect();
		this.lastLeftHandPositionForTag = position;
		this.lastRightHandPositionForTag = position2;
		this.lastBodyPositionForTag = position4;
		this.lastHeadPositionForTag = position3;
		for (int k = 0; k < 12; k++)
		{
			GorillaTagger.StiltTagData stiltTagData3 = this.stiltTagData[k];
			if (stiltTagData3.hasLastPosition || stiltTagData3.hasCurrentPosition)
			{
				stiltTagData3.lastPositionForTag = stiltTagData3.currentPositionForTag;
				stiltTagData3.hasLastPosition = stiltTagData3.hasCurrentPosition;
				stiltTagData3.hasCurrentPosition = false;
				this.stiltTagData[k] = stiltTagData3;
			}
		}
		if (GTPlayer.Instance.IsBodySliding && (double)GTPlayer.Instance.RigidbodyVelocity.magnitude >= 0.15)
		{
			if (!this.bodySlideSource.isPlaying)
			{
				this.bodySlideSource.Play();
			}
		}
		else
		{
			this.bodySlideSource.Stop();
		}
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (float.IsFinite(GorillaTagger.moderationMutedTime) && GorillaTagger.moderationMutedTime >= 0f)
		{
			GorillaTagger.moderationMutedTime -= Time.deltaTime;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaTagger.moderationMutedTime > 0f)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
						{
							this.myRecorder.TransmitEnabled = true;
							return;
						}
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = false;
					bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
					this.myRecorder.TransmitEnabled = false;
					return;
				}
			}
			else
			{
				if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	// Token: 0x06003A30 RID: 14896 RVA: 0x0013DA88 File Offset: 0x0013BC88
	private bool TryToTag(VRRig rig, Vector3 hitObjectPos, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (NetworkSystem.Instance.InRoom)
		{
			this.tempCreator = ((rig != null) ? rig.creator : null);
			if (this.tempCreator != null && NetworkSystem.Instance.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && (canStun || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.tempCreator)) && (this.headCollider.transform.position - hitObjectPos).sqrMagnitude < maxTagDistance * maxTagDistance * GTPlayer.Instance.scale)
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitObjectPos).magnitude < (this.rightHandTransform.position - hitObjectPos).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003A31 RID: 14897 RVA: 0x0013DBE8 File Offset: 0x0013BDE8
	private bool TryToTag(Collider hitCollider, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedNetPlayer)
	{
		VRRig vrrig;
		if (!this.tagRigDict.TryGetValue(hitCollider, out vrrig))
		{
			vrrig = hitCollider.GetComponentInParent<VRRig>();
			this.tagRigDict.Add(hitCollider, vrrig);
		}
		if (vrrig == null)
		{
			PropHuntTaggableProp componentInParent = hitCollider.GetComponentInParent<PropHuntTaggableProp>();
			if (!(componentInParent != null))
			{
				taggedPlayer = null;
				touchedNetPlayer = null;
				return false;
			}
			vrrig = componentInParent.ownerRig;
		}
		else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
		{
			taggedPlayer = null;
			touchedNetPlayer = null;
			return false;
		}
		return this.TryToTag(vrrig, hitCollider.transform.position, isBodyTag, canStun, maxTagDistance, out taggedPlayer, out touchedNetPlayer);
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x0013DC84 File Offset: 0x0013BE84
	private void HitWithKnockBack(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool leftHand)
	{
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 vector = leftHand ? (-vrmap.rigTarget.right) : vrmap.rigTarget.right;
		RigContainer rigContainer2;
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer2) && rigContainer2.Rig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
		{
			RoomSystem.HitPlayer(taggedPlayer, vector.normalized, averageVelocity.magnitude);
		}
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x0013DD2F File Offset: 0x0013BF2F
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x0013DD41 File Offset: 0x0013BF41
	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			device = ControllerInputPoller.instance.leftControllerDevice;
		}
		else
		{
			device = ControllerInputPoller.instance.rightControllerDevice;
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x0013DD68 File Offset: 0x0013BF68
	public void PlayHapticClip(bool forLeftController, AudioClip clip, float strength)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
			}
			this.leftHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
			return;
		}
		if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
		}
		this.rightHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x0013DDCB File Offset: 0x0013BFCB
	public void StopHapticClip(bool forLeftController)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
				this.leftHapticsRoutine = null;
				return;
			}
		}
		else if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
			this.rightHapticsRoutine = null;
		}
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x0013DE07 File Offset: 0x0013C007
	private IEnumerator AudioClipHapticPulses(bool forLeftController, AudioClip clip, float strength)
	{
		uint channel = 0U;
		int bufferSize = 8192;
		int sampleWindowSize = 256;
		float[] audioData;
		UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			float[] array;
			if ((array = this.leftHapticsBuffer) == null)
			{
				array = (this.leftHapticsBuffer = new float[bufferSize]);
			}
			audioData = array;
			device = ControllerInputPoller.instance.leftControllerDevice;
		}
		else
		{
			float[] array2;
			if ((array2 = this.rightHapticsBuffer) == null)
			{
				array2 = (this.rightHapticsBuffer = new float[bufferSize]);
			}
			audioData = array2;
			device = ControllerInputPoller.instance.rightControllerDevice;
		}
		int sampleOffset = -bufferSize;
		float startTime = Time.time;
		float length = clip.length;
		float endTime = Time.time + length;
		float sampleRate = (float)clip.samples;
		while (Time.time <= endTime)
		{
			float num = (Time.time - startTime) / length;
			int num2 = (int)(sampleRate * num);
			if (Mathf.Max(num2 + sampleWindowSize - 1, audioData.Length - 1) >= sampleOffset + bufferSize)
			{
				clip.GetData(audioData, num2);
				sampleOffset = num2;
			}
			float num3 = 0f;
			int num4 = Mathf.Min(clip.samples - num2, sampleWindowSize);
			for (int i = 0; i < num4; i++)
			{
				float num5 = audioData[num2 - sampleOffset + i];
				num3 += num5 * num5;
			}
			float amplitude = Mathf.Clamp01(((num4 > 0) ? Mathf.Sqrt(num3 / (float)num4) : 0f) * strength);
			device.SendHapticImpulse(channel, amplitude, Time.fixedDeltaTime);
			yield return null;
		}
		if (forLeftController)
		{
			this.leftHapticsRoutine = null;
		}
		else
		{
			this.rightHapticsRoutine = null;
		}
		yield break;
	}

	// Token: 0x06003A38 RID: 14904 RVA: 0x0013DE2C File Offset: 0x0013C02C
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		UnityEngine.XR.InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x06003A39 RID: 14905 RVA: 0x0013DE54 File Offset: 0x0013C054
	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.bodyRenderer.ResetBodyMaterial();
		}
	}

	// Token: 0x06003A3A RID: 14906 RVA: 0x0013DE90 File Offset: 0x0013C090
	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	// Token: 0x06003A3B RID: 14907 RVA: 0x0013DEB0 File Offset: 0x0013C0B0
	protected void OnTriggerExit(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxExited();
		}
	}

	// Token: 0x06003A3C RID: 14908 RVA: 0x0013DED0 File Offset: 0x0013C0D0
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	// Token: 0x06003A3D RID: 14909 RVA: 0x0013DF51 File Offset: 0x0013C151
	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x06003A3E RID: 14910 RVA: 0x0013DF91 File Offset: 0x0013C191
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x0013DFAC File Offset: 0x0013C1AC
	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	// Token: 0x06003A40 RID: 14912 RVA: 0x0013DFDB File Offset: 0x0013C1DB
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x06003A41 RID: 14913 RVA: 0x0013DFFF File Offset: 0x0013C1FF
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x06003A42 RID: 14914 RVA: 0x0013E02C File Offset: 0x0013C22C
	private void ProcessHandTapping(in bool isLeftHand, in StiltID stiltID, ref float lastTapTime, ref float lastTapUpTime, ref bool wasHandTouching, in AudioSource handSlideSource)
	{
		bool flag;
		bool flag2;
		int num;
		GorillaSurfaceOverride gorillaSurfaceOverride;
		RaycastHit raycastHit;
		Vector3 b;
		GorillaVelocityTracker gorillaVelocityTracker;
		GTPlayer.Instance.GetHandTapData(isLeftHand, stiltID, out flag, out flag2, out num, out gorillaSurfaceOverride, out raycastHit, out b, out gorillaVelocityTracker);
		GorillaTagger.DebouncedBool debouncedBool = isLeftHand ? this._leftHandDown : this._rightHandDown;
		if (GTPlayer.Instance.inOverlay)
		{
			handSlideSource.GTStop();
			return;
		}
		if (flag2)
		{
			this.StartVibration(isLeftHand, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!handSlideSource.isPlaying)
			{
				handSlideSource.GTPlay();
			}
			return;
		}
		handSlideSource.GTStop();
		bool wasStablyEnabled = debouncedBool.WasStablyEnabled;
		debouncedBool.Set(flag);
		bool flag3 = !wasHandTouching && flag && debouncedBool.JustEnabled;
		bool flag4 = wasHandTouching && !flag && wasStablyEnabled;
		wasHandTouching = flag;
		if (!flag4 && !flag3)
		{
			return;
		}
		Tappable tappable = null;
		bool flag5 = gorillaSurfaceOverride != null && gorillaSurfaceOverride.TryGetComponent<Tappable>(out tappable);
		HandEffectContext handEffect = this.offlineVRRig.GetHandEffect(isLeftHand, stiltID);
		if ((!flag5 || !tappable.overrideTapCooldown) && (!handEffect.SeparateUpTapCooldown || !flag4 || Time.time <= lastTapUpTime + this.tapCoolDown) && (!flag3 || Time.time <= lastTapTime + this.tapCoolDown))
		{
			return;
		}
		float sqrMagnitude = (gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false) / GTPlayer.Instance.scale).sqrMagnitude;
		float sqrMagnitude2 = gorillaVelocityTracker.GetAverageVelocity(false, 0.03f, false).sqrMagnitude;
		this.handTapSpeed = Mathf.Sqrt(Mathf.Max(sqrMagnitude, sqrMagnitude2));
		if (handEffect.SeparateUpTapCooldown && flag4)
		{
			lastTapUpTime = Time.time;
		}
		else
		{
			lastTapTime = Time.time;
		}
		this.dirFromHitToHand = Vector3.Normalize(raycastHit.point - b);
		GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(NetworkSystem.Instance.LocalPlayer))
		{
			this.handTapVolume = Mathf.Clamp(this.handTapSpeed, 0f, gorillaAmbushManager.crawlingSpeedForMaxVolume);
		}
		else
		{
			this.handTapVolume = this.cacheHandTapVolume;
		}
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null && gorillaFreezeTagManager.IsFrozen(NetworkSystem.Instance.LocalPlayer))
		{
			this.audioClipIndex = gorillaFreezeTagManager.GetFrozenHandTapAudioIndex();
		}
		else if (gorillaSurfaceOverride != null)
		{
			this.audioClipIndex = gorillaSurfaceOverride.overrideIndex;
		}
		else
		{
			this.audioClipIndex = num;
		}
		if (gorillaSurfaceOverride != null)
		{
			if (gorillaSurfaceOverride.sendOnTapEvent)
			{
				IBuilderTappable builderTappable;
				if (flag5)
				{
					tappable.OnTap(this.handTapVolume);
				}
				else if (gorillaSurfaceOverride.TryGetComponent<IBuilderTappable>(out builderTappable))
				{
					builderTappable.OnTapLocal(this.handTapVolume);
				}
			}
			PlayerGameEvents.TapObject(gorillaSurfaceOverride.name);
		}
		Vector3 averageVelocity = gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false);
		if (GameMode.ActiveGameMode != null)
		{
			GameMode.ActiveGameMode.HandleHandTap(NetworkSystem.Instance.LocalPlayer, tappable, isLeftHand, averageVelocity, raycastHit.normal);
		}
		this.StartVibration(isLeftHand, this.tapHapticStrength, this.tapHapticDuration);
		this.offlineVRRig.SetHandEffectData(handEffect, this.audioClipIndex, flag3, isLeftHand, stiltID, this.handTapVolume, this.handTapSpeed, this.dirFromHitToHand);
		FXSystem.PlayFX(handEffect);
		Action<bool, Vector3, Vector3> onHandTap = this.OnHandTap;
		if (onHandTap != null)
		{
			onHandTap(isLeftHand, raycastHit.point, raycastHit.normal);
		}
		this.hasTappedSurface = true;
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority())
		{
			CrittersRigActorSetup crittersRigActorSetup = CrittersManager.instance.rigSetupByRig[this.offlineVRRig];
			if (crittersRigActorSetup.IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)crittersRigActorSetup.rigActors[isLeftHand ? 0 : 2].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapLocal(isLeftHand);
				}
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.offlineVRRig.zoneEntity.currentZone);
		if (managerForZone.IsNotNull() && managerForZone.ghostReactorManager.IsNotNull() && !averageVelocity.AlmostZero())
		{
			Transform handFollower = GTPlayer.Instance.GetHandFollower(isLeftHand);
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Ray(handFollower.position, averageVelocity.normalized), out raycastHit2, 10f))
			{
				Vector3 vector = Vector3.ProjectOnPlane(-handFollower.forward, raycastHit2.normal);
				managerForZone.ghostReactorManager.OnTapLocal(isLeftHand, raycastHit2.point + raycastHit2.normal * 0.005f, Quaternion.LookRotation(vector.normalized, isLeftHand ? (-raycastHit2.normal) : raycastHit2.normal), gorillaSurfaceOverride, averageVelocity);
			}
		}
		if (NetworkSystem.Instance.InRoom && this.myVRRig.IsNotNull() && this.myVRRig != null)
		{
			this.myVRRig.GetView.RPC("OnHandTapRPC", RpcTarget.Others, new object[]
			{
				this.audioClipIndex,
				flag3,
				isLeftHand,
				stiltID,
				this.handTapSpeed,
				Utils.PackVector3ToLong(this.dirFromHitToHand)
			});
		}
	}

	// Token: 0x06003A43 RID: 14915 RVA: 0x0013E568 File Offset: 0x0013C768
	public void ConfirmUpdatedFrameRate()
	{
		GorillaTagger.<ConfirmUpdatedFrameRate>d__180 <ConfirmUpdatedFrameRate>d__;
		<ConfirmUpdatedFrameRate>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ConfirmUpdatedFrameRate>d__.<>4__this = this;
		<ConfirmUpdatedFrameRate>d__.<>1__state = -1;
		<ConfirmUpdatedFrameRate>d__.<>t__builder.Start<GorillaTagger.<ConfirmUpdatedFrameRate>d__180>(ref <ConfirmUpdatedFrameRate>d__);
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x0013E5A0 File Offset: 0x0013C7A0
	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GTPlayer.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x0013E67B File Offset: 0x0013C87B
	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x06003A46 RID: 14918 RVA: 0x0013E69A File Offset: 0x0013C89A
	private void RecoverMissingRefs()
	{
		if (!this.offlineVRRig)
		{
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.leftHandSlideSource, "leftHandSlideSource", "./**/Left Arm IK/SlideAudio");
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.rightHandSlideSource, "rightHandSlideSource", "./**/Right Arm IK/SlideAudio");
		}
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x0013E6D8 File Offset: 0x0013C8D8
	private void RecoverMissingRefs_Asdf<T>(ref T objRef, string objFieldName, string recoveryPath) where T : Object
	{
		if (objRef)
		{
			return;
		}
		Transform transform;
		if (!this.offlineVRRig.transform.TryFindByPath(recoveryPath, out transform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference missing and could not find by path: \"",
				recoveryPath,
				"\""
			}), this);
		}
		objRef = transform.GetComponentInChildren<T>();
		if (!objRef)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference is missing. Found transform with recover path, but did not find the component. Recover path: \"",
				recoveryPath,
				"\""
			}), this);
		}
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x0013E78E File Offset: 0x0013C98E
	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x06003A49 RID: 14921 RVA: 0x0013E7A7 File Offset: 0x0013C9A7
	// (set) Token: 0x06003A4A RID: 14922 RVA: 0x0013E7AF File Offset: 0x0013C9AF
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x06003A4B RID: 14923 RVA: 0x0013E7B8 File Offset: 0x0013C9B8
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = (target.targetMono.GuidedRefTargetObject as VRRig);
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	// Token: 0x06003A50 RID: 14928 RVA: 0x00086271 File Offset: 0x00084471
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x06003A51 RID: 14929 RVA: 0x00018FAD File Offset: 0x000171AD
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x06003A52 RID: 14930 RVA: 0x0013E930 File Offset: 0x0013CB30
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsOverlap|159_0(bool isLeftHand, float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass159_0 A_5)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig x;
			if (this.colliderOverlaps[i].gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.colliderOverlaps[i], out x) || !(x == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.colliderOverlaps[i], true, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_5.bodyHit = false;
					A_5.leftHandHit = isLeftHand;
					A_5.canTagHit = canTag;
					A_5.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_5.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x06003A53 RID: 14931 RVA: 0x0013E9F4 File Offset: 0x0013CBF4
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsCapsulecast|159_1(float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass159_0 A_4)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig x;
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.nonAllocRaycastHits[i].collider, out x) || !(x == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.nonAllocRaycastHits[i].collider, false, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_4.bodyHit = true;
					A_4.canTagHit = canTag;
					A_4.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_4.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x04004A07 RID: 18951
	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	// Token: 0x04004A08 RID: 18952
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04004A09 RID: 18953
	public static float moderationMutedTime = -1f;

	// Token: 0x04004A0A RID: 18954
	public int SmoothedFramerate;

	// Token: 0x04004A0B RID: 18955
	private int _prevSmoothedFramerate;

	// Token: 0x04004A0C RID: 18956
	public int FramerateHealth;

	// Token: 0x04004A0D RID: 18957
	private int _prevFramerateHealth;

	// Token: 0x04004A0E RID: 18958
	private float _framerateHealthTimer;

	// Token: 0x04004A0F RID: 18959
	private float[] _framerateTracker = new float[30];

	// Token: 0x04004A10 RID: 18960
	private float _framerateTotal;

	// Token: 0x04004A11 RID: 18961
	private int _framerateIndex;

	// Token: 0x04004A12 RID: 18962
	private float _framerateTimer;

	// Token: 0x04004A13 RID: 18963
	private bool _forcePerfRefreshRate;

	// Token: 0x04004A14 RID: 18964
	private float _perfRefreshRate = 72f;

	// Token: 0x04004A15 RID: 18965
	private float _defaultRefreshRate = 90f;

	// Token: 0x04004A16 RID: 18966
	public bool inCosmeticsRoom;

	// Token: 0x04004A17 RID: 18967
	public SphereCollider headCollider;

	// Token: 0x04004A18 RID: 18968
	public CapsuleCollider bodyCollider;

	// Token: 0x04004A19 RID: 18969
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04004A1A RID: 18970
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04004A1B RID: 18971
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04004A1C RID: 18972
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04004A1D RID: 18973
	private GorillaTagger.StiltTagData[] stiltTagData = new GorillaTagger.StiltTagData[12];

	// Token: 0x04004A1E RID: 18974
	public Transform rightHandTransform;

	// Token: 0x04004A1F RID: 18975
	public Transform leftHandTransform;

	// Token: 0x04004A20 RID: 18976
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04004A21 RID: 18977
	public float handTapVolume = 0.1f;

	// Token: 0x04004A22 RID: 18978
	public float handTapSpeed;

	// Token: 0x04004A23 RID: 18979
	public float tapCoolDown = 0.15f;

	// Token: 0x04004A24 RID: 18980
	public float lastLeftTap;

	// Token: 0x04004A25 RID: 18981
	public float lastLeftUpTap;

	// Token: 0x04004A26 RID: 18982
	public float lastRightTap;

	// Token: 0x04004A27 RID: 18983
	public float lastRightUpTap;

	// Token: 0x04004A28 RID: 18984
	private bool leftHandWasTouching;

	// Token: 0x04004A29 RID: 18985
	private bool rightHandWasTouching;

	// Token: 0x04004A2A RID: 18986
	public float tapHapticDuration = 0.05f;

	// Token: 0x04004A2B RID: 18987
	public float tapHapticStrength = 0.5f;

	// Token: 0x04004A2C RID: 18988
	public float tagHapticDuration = 0.15f;

	// Token: 0x04004A2D RID: 18989
	public float tagHapticStrength = 1f;

	// Token: 0x04004A2E RID: 18990
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04004A2F RID: 18991
	public float taggedHapticStrength = 1f;

	// Token: 0x04004A30 RID: 18992
	public float taggedTime;

	// Token: 0x04004A31 RID: 18993
	public float tagCooldown;

	// Token: 0x04004A32 RID: 18994
	public float slowCooldown = 3f;

	// Token: 0x04004A33 RID: 18995
	public float maxTagDistance = 2.2f;

	// Token: 0x04004A34 RID: 18996
	public float maxStiltTagDistance = 3.2f;

	// Token: 0x04004A35 RID: 18997
	public VRRig offlineVRRig;

	// Token: 0x04004A36 RID: 18998
	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	// Token: 0x04004A37 RID: 18999
	public GameObject thirdPersonCamera;

	// Token: 0x04004A38 RID: 19000
	public GameObject mainCamera;

	// Token: 0x04004A39 RID: 19001
	public bool testTutorial;

	// Token: 0x04004A3A RID: 19002
	public bool disableTutorial;

	// Token: 0x04004A3B RID: 19003
	private bool _framerateUpdated;

	// Token: 0x04004A3C RID: 19004
	private bool _performanceOn;

	// Token: 0x04004A3D RID: 19005
	public GameObject leftHandTriggerCollider;

	// Token: 0x04004A3E RID: 19006
	public GameObject rightHandTriggerCollider;

	// Token: 0x04004A3F RID: 19007
	public AudioSource leftHandSlideSource;

	// Token: 0x04004A40 RID: 19008
	public AudioSource rightHandSlideSource;

	// Token: 0x04004A41 RID: 19009
	public AudioSource bodySlideSource;

	// Token: 0x04004A42 RID: 19010
	public bool overrideNotInFocus;

	// Token: 0x04004A44 RID: 19012
	private Vector3 leftRaycastSweep;

	// Token: 0x04004A45 RID: 19013
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x04004A46 RID: 19014
	private Vector3 rightRaycastSweep;

	// Token: 0x04004A47 RID: 19015
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x04004A48 RID: 19016
	private Vector3 headRaycastSweep;

	// Token: 0x04004A49 RID: 19017
	private Vector3 bodyRaycastSweep;

	// Token: 0x04004A4A RID: 19018
	private UnityEngine.XR.InputDevice rightDevice;

	// Token: 0x04004A4B RID: 19019
	private UnityEngine.XR.InputDevice leftDevice;

	// Token: 0x04004A4C RID: 19020
	private bool primaryButtonPressRight;

	// Token: 0x04004A4D RID: 19021
	private bool secondaryButtonPressRight;

	// Token: 0x04004A4E RID: 19022
	private bool primaryButtonPressLeft;

	// Token: 0x04004A4F RID: 19023
	private bool secondaryButtonPressLeft;

	// Token: 0x04004A50 RID: 19024
	private RaycastHit hitInfo;

	// Token: 0x04004A51 RID: 19025
	public NetPlayer otherPlayer;

	// Token: 0x04004A52 RID: 19026
	private NetPlayer tryPlayer;

	// Token: 0x04004A53 RID: 19027
	private NetPlayer touchedPlayer;

	// Token: 0x04004A54 RID: 19028
	private Vector3 topVector;

	// Token: 0x04004A55 RID: 19029
	private Vector3 bottomVector;

	// Token: 0x04004A56 RID: 19030
	private Vector3 bodyVector;

	// Token: 0x04004A57 RID: 19031
	private Vector3 dirFromHitToHand;

	// Token: 0x04004A58 RID: 19032
	private int audioClipIndex;

	// Token: 0x04004A59 RID: 19033
	private UnityEngine.XR.InputDevice inputDevice;

	// Token: 0x04004A5A RID: 19034
	private bool wasInOverlay;

	// Token: 0x04004A5B RID: 19035
	private PhotonView tempView;

	// Token: 0x04004A5C RID: 19036
	private NetPlayer tempCreator;

	// Token: 0x04004A5D RID: 19037
	private float cacheHandTapVolume;

	// Token: 0x04004A5E RID: 19038
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04004A5F RID: 19039
	public float statusStartTime;

	// Token: 0x04004A60 RID: 19040
	public float statusEndTime;

	// Token: 0x04004A61 RID: 19041
	private float refreshRate;

	// Token: 0x04004A62 RID: 19042
	private float baseSlideControl;

	// Token: 0x04004A63 RID: 19043
	private int gorillaTagColliderLayerMask;

	// Token: 0x04004A64 RID: 19044
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x04004A65 RID: 19045
	private Collider[] colliderOverlaps = new Collider[30];

	// Token: 0x04004A66 RID: 19046
	private Dictionary<Collider, VRRig> tagRigDict = new Dictionary<Collider, VRRig>();

	// Token: 0x04004A67 RID: 19047
	private int nonAllocHits;

	// Token: 0x04004A69 RID: 19049
	private bool xrSubsystemIsActive;

	// Token: 0x04004A6A RID: 19050
	public string loadedDeviceName = "";

	// Token: 0x04004A6B RID: 19051
	private bool _forceFramerateCheck = true;

	// Token: 0x04004A6C RID: 19052
	[SerializeField]
	private int _framesForHandTrigger = 5;

	// Token: 0x04004A6D RID: 19053
	private GorillaTagger.DebouncedBool _leftHandDown;

	// Token: 0x04004A6E RID: 19054
	private GorillaTagger.DebouncedBool _rightHandDown;

	// Token: 0x04004A6F RID: 19055
	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	// Token: 0x04004A70 RID: 19056
	public Watchable<int> MirrorCameraCullingMask;

	// Token: 0x04004A71 RID: 19057
	private float[] leftHapticsBuffer;

	// Token: 0x04004A72 RID: 19058
	private float[] rightHapticsBuffer;

	// Token: 0x04004A73 RID: 19059
	private Coroutine leftHapticsRoutine;

	// Token: 0x04004A74 RID: 19060
	private Coroutine rightHapticsRoutine;

	// Token: 0x04004A75 RID: 19061
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x04004A76 RID: 19062
	private bool isGameOverlayActive;

	// Token: 0x04004A77 RID: 19063
	private float? tagRadiusOverride;

	// Token: 0x04004A78 RID: 19064
	private int tagRadiusOverrideFrame = -1;

	// Token: 0x04004A7B RID: 19067
	public XRDisplaySubsystem activeXRDisplay;

	// Token: 0x04004A7C RID: 19068
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x020008B3 RID: 2227
	private struct StiltTagData
	{
		// Token: 0x04004A7E RID: 19070
		public bool isLeftHand;

		// Token: 0x04004A7F RID: 19071
		public bool hasCurrentPosition;

		// Token: 0x04004A80 RID: 19072
		public bool hasLastPosition;

		// Token: 0x04004A81 RID: 19073
		public Vector3 currentPositionForTag;

		// Token: 0x04004A82 RID: 19074
		public Vector3 lastPositionForTag;

		// Token: 0x04004A83 RID: 19075
		public bool wasTouching;

		// Token: 0x04004A84 RID: 19076
		public float lastTap;

		// Token: 0x04004A85 RID: 19077
		public float lastUpTap;

		// Token: 0x04004A86 RID: 19078
		public bool canTag;

		// Token: 0x04004A87 RID: 19079
		public bool canStun;
	}

	// Token: 0x020008B4 RID: 2228
	public enum StatusEffect
	{
		// Token: 0x04004A89 RID: 19081
		None,
		// Token: 0x04004A8A RID: 19082
		Frozen,
		// Token: 0x04004A8B RID: 19083
		Slowed,
		// Token: 0x04004A8C RID: 19084
		Dead,
		// Token: 0x04004A8D RID: 19085
		Infected,
		// Token: 0x04004A8E RID: 19086
		It
	}

	// Token: 0x020008B5 RID: 2229
	private class DebouncedBool
	{
		// Token: 0x17000536 RID: 1334
		// (get) Token: 0x06003A54 RID: 14932 RVA: 0x0013EAC7 File Offset: 0x0013CCC7
		// (set) Token: 0x06003A55 RID: 14933 RVA: 0x0013EACF File Offset: 0x0013CCCF
		public bool Value { get; private set; }

		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x06003A56 RID: 14934 RVA: 0x0013EAD8 File Offset: 0x0013CCD8
		// (set) Token: 0x06003A57 RID: 14935 RVA: 0x0013EAE0 File Offset: 0x0013CCE0
		public bool JustEnabled { get; private set; }

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x06003A58 RID: 14936 RVA: 0x0013EAE9 File Offset: 0x0013CCE9
		// (set) Token: 0x06003A59 RID: 14937 RVA: 0x0013EAF1 File Offset: 0x0013CCF1
		public bool WasStablyEnabled { get; private set; }

		// Token: 0x06003A5A RID: 14938 RVA: 0x0013EAFA File Offset: 0x0013CCFA
		public DebouncedBool(int callsUntilDisable, bool initialValue = false)
		{
			this._callsUntilStable = callsUntilDisable;
			this.Value = initialValue;
			this._lastValue = initialValue;
		}

		// Token: 0x06003A5B RID: 14939 RVA: 0x0013EB18 File Offset: 0x0013CD18
		public void Set(bool value)
		{
			this._lastValue = this.Value;
			if (!value)
			{
				this.WasStablyEnabled = false;
				this._callsSinceDisable++;
				if (this._callsSinceDisable == this._callsUntilStable)
				{
					this.Value = false;
				}
			}
			else
			{
				this.Value = true;
				this._callsSinceDisable = 0;
				this._callsSinceEnable++;
				if (this._callsSinceEnable >= this._callsUntilStable)
				{
					this.WasStablyEnabled = true;
				}
			}
			this.JustEnabled = (this.Value && !this._lastValue);
		}

		// Token: 0x04004A8F RID: 19087
		private readonly int _callsUntilStable;

		// Token: 0x04004A90 RID: 19088
		private int _callsSinceDisable;

		// Token: 0x04004A91 RID: 19089
		private int _callsSinceEnable;

		// Token: 0x04004A92 RID: 19090
		private bool _lastValue;
	}
}
