using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

// Token: 0x0200049F RID: 1183
public class PrivateUIRoom : MonoBehaviourTick
{
	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001C9B RID: 7323 RVA: 0x0009AEC3 File Offset: 0x000990C3
	private bool overlayForcedActive
	{
		get
		{
			return this.overlayForcedSources > (PrivateUIRoom.OverlaySource)0;
		}
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001C9C RID: 7324 RVA: 0x0009A386 File Offset: 0x00098586
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0009AED0 File Offset: 0x000990D0
	private void Awake()
	{
		if (PrivateUIRoom.instance == null)
		{
			PrivateUIRoom.instance = this;
			this.occluder.SetActive(false);
			this.leftHandObject.SetActive(false);
			this.rightHandObject.SetActive(false);
			this.ui = new List<Transform>();
			this.uiParents = new Dictionary<Transform, Transform>();
			this.backgroundDirectionPropertyID = Shader.PropertyToID(this.backgroundDirectionPropertyName);
			this._uiRoot = new GameObject("UIRoot").transform;
			this._uiRoot.parent = base.transform;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001C9E RID: 7326 RVA: 0x0009AF68 File Offset: 0x00099168
	private new void OnEnable()
	{
		base.OnEnable();
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Listen(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001C9F RID: 7327 RVA: 0x0009AF8B File Offset: 0x0009918B
	private new void OnDisable()
	{
		base.OnDisable();
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Remove(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x0009AFB0 File Offset: 0x000991B0
	private static bool FindShoulderCamera()
	{
		if (PrivateUIRoom._shoulderCameraReference.IsNotNull())
		{
			return true;
		}
		if (GorillaTagger.Instance.IsNull())
		{
			return false;
		}
		PrivateUIRoom._shoulderCameraReference = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		if (PrivateUIRoom._shoulderCameraReference == null)
		{
			Debug.LogError("[PRIVATE_UI_ROOMS] Could not find Shoulder Camera");
			return false;
		}
		PrivateUIRoom._virtualCameraReference = PrivateUIRoom._shoulderCameraReference.GetComponentInChildren<CinemachineVirtualCamera>();
		return true;
	}

	// Token: 0x06001CA1 RID: 7329 RVA: 0x0009B018 File Offset: 0x00099218
	private void ToggleHands(VREvent_t ev)
	{
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] Toggling hands visibility. Event: {0} ({1})", ev.eventType, (EVREventType)ev.eventType));
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] _handsShowing: {0}", PrivateUIRoom.instance.rightHandObject.activeSelf));
		if (PrivateUIRoom.instance.rightHandObject.activeSelf)
		{
			this.HideHands();
			return;
		}
		this.ShowHands();
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x0009B08B File Offset: 0x0009928B
	private void HideHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu shown, disabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x0009B0B7 File Offset: 0x000992B7
	private void ShowHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu hidden, re-enabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x0009B0E4 File Offset: 0x000992E4
	private void ToggleLevelVisibility(bool levelShouldBeVisible)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (levelShouldBeVisible)
		{
			component.cullingMask = this.savedCullingLayers;
			if (this.savedCullingLayersShoudlerCam != null)
			{
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.savedCullingLayersShoudlerCam.Value;
				this.savedCullingLayersShoudlerCam = null;
				return;
			}
		}
		else
		{
			this.savedCullingLayers = component.cullingMask;
			component.cullingMask = this.visibleLayers;
			if (PrivateUIRoom.FindShoulderCamera())
			{
				this.savedCullingLayersShoudlerCam = new int?(PrivateUIRoom._shoulderCameraReference.cullingMask);
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.visibleLayers;
				PrivateUIRoom._virtualCameraReference.enabled = false;
			}
		}
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x0009B198 File Offset: 0x00099398
	private static void StopOverlay()
	{
		PrivateUIRoom.instance.localPlayer.inOverlay = false;
		PrivateUIRoom.instance.inOverlay = false;
		PrivateUIRoom.instance.localPlayer.disableMovement = false;
		PrivateUIRoom.instance.localPlayer.InReportMenu = false;
		PrivateUIRoom.instance.ToggleLevelVisibility(true);
		PrivateUIRoom.instance.occluder.SetActive(false);
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
		PrivateUIRoom._virtualCameraReference.enabled = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(false);
		Debug.Log("[PrivateUIRoom::StopOverlay] Re-enabling Game Audio");
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x0009B23C File Offset: 0x0009943C
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * Vector3.zero * scale.x;
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x0009B2C8 File Offset: 0x000994C8
	private static void AssignShoulderCameraToCanvases(Transform focus)
	{
		Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] setting up canvases with shoulder camera.");
		if (!PrivateUIRoom.FindShoulderCamera())
		{
			return;
		}
		Canvas componentInChildren = focus.GetComponentInChildren<Canvas>(true);
		if (componentInChildren != null)
		{
			componentInChildren.worldCamera = PrivateUIRoom._shoulderCameraReference;
			Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] Assigned shoulder camera to Canvas: " + componentInChildren.name);
			return;
		}
		Debug.LogError("[KID::PrivateUIRoom::CanvasCameraAssigner] No Canvas component found on this GameObject.");
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x0009B324 File Offset: 0x00099524
	public static void AddUI(Transform focus)
	{
		if (PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		PrivateUIRoom.instance._text.text = "";
		PrivateUIRoom.AssignShoulderCameraToCanvases(focus);
		PrivateUIRoom.instance.uiParents.Add(focus, focus.parent);
		focus.gameObject.SetActive(false);
		focus.parent = PrivateUIRoom.instance._uiRoot;
		focus.localPosition = Vector3.zero;
		focus.localRotation = Quaternion.identity;
		PrivateUIRoom.instance.ui.Add(focus);
		if (PrivateUIRoom.instance.ui.Count == 1 && PrivateUIRoom.instance.focusTransform == null)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			if (!PrivateUIRoom.instance.inOverlay)
			{
				PrivateUIRoom.StartOverlay();
			}
		}
		PrivateUIRoom.instance.UpdateUIPositionAndRotation();
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x0009B424 File Offset: 0x00099624
	public static void RemoveUI(Transform focus)
	{
		if (!PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		focus.gameObject.SetActive(false);
		PrivateUIRoom.instance.ui.Remove(focus);
		if (PrivateUIRoom.instance.focusTransform == focus)
		{
			PrivateUIRoom.instance.focusTransform = null;
		}
		if (PrivateUIRoom.instance.uiParents[focus] != null)
		{
			focus.parent = PrivateUIRoom.instance.uiParents[focus];
			PrivateUIRoom.instance.uiParents.Remove(focus);
		}
		else
		{
			Object.Destroy(focus.gameObject);
		}
		if (PrivateUIRoom.instance.ui.Count > 0)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			return;
		}
		if (!PrivateUIRoom.instance.overlayForcedActive)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x0009B520 File Offset: 0x00099720
	public static void ForceStartOverlay(PrivateUIRoom.OverlaySource source, string text = "")
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedSources |= source;
		if (PrivateUIRoom.instance.inOverlay)
		{
			return;
		}
		PrivateUIRoom.instance._text.text = text;
		PrivateUIRoom.StartOverlay();
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x0009B570 File Offset: 0x00099770
	public static void StopForcedOverlay(PrivateUIRoom.OverlaySource source)
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedSources &= ~source;
		if (PrivateUIRoom.instance.overlayForcedActive)
		{
			return;
		}
		if (PrivateUIRoom.instance.ui.Count == 0 && PrivateUIRoom.instance.inOverlay)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x0009B5D0 File Offset: 0x000997D0
	private static void StartOverlay()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 localScale;
		PrivateUIRoom.instance.GetIdealScreenPositionRotation(out vector, out quaternion, out localScale);
		PrivateUIRoom.instance.leftHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.rightHandObject.transform.localScale = localScale;
		PrivateUIRoom.instance.occluder.transform.localScale = localScale;
		PrivateUIRoom.instance.localPlayer.InReportMenu = true;
		PrivateUIRoom.instance.localPlayer.disableMovement = true;
		PrivateUIRoom.instance.occluder.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.ToggleLevelVisibility(false);
		PrivateUIRoom.instance.localPlayer.inOverlay = true;
		PrivateUIRoom.instance.inOverlay = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(true);
		Debug.Log("[PrivateUIRoom::StartOverlay] Muting Game Audio");
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x0009B6B8 File Offset: 0x000998B8
	public override void Tick()
	{
		if (!this.localPlayer.InReportMenu)
		{
			return;
		}
		this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
		Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
		Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
		this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation);
		if (this.ShouldUpdateRotation())
		{
			this.UpdateUIPositionAndRotation();
			return;
		}
		if (this.ShouldUpdatePosition())
		{
			this.UpdateUIPosition();
		}
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0009B768 File Offset: 0x00099968
	private bool ShouldUpdateRotation()
	{
		float magnitude = (GorillaTagger.Instance.mainCamera.transform.position - this.lastStablePosition).X_Z().magnitude;
		Quaternion b = Quaternion.Euler(0f, GorillaTagger.Instance.mainCamera.transform.rotation.eulerAngles.y, 0f);
		float num = Quaternion.Angle(this.lastStableRotation, b);
		return magnitude > this.lateralPlay || num >= this.rotationalPlay;
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x0009B7F5 File Offset: 0x000999F5
	private bool ShouldUpdatePosition()
	{
		return Mathf.Abs(GorillaTagger.Instance.mainCamera.transform.position.y - this.lastStablePosition.y) > this.verticalPlay;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x0009B82C File Offset: 0x00099A2C
	private void UpdateUIPositionAndRotation()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this.lastStableRotation = transform.rotation;
		Vector3 normalized = transform.forward.X_Z().normalized;
		this._uiRoot.SetPositionAndRotation(this.lastStablePosition + normalized * 0.02f, Quaternion.LookRotation(normalized));
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		PrivateUIRoom._shoulderCameraReference.transform.rotation = this._uiRoot.rotation;
		this.backgroundRenderer.material.SetVector(this.backgroundDirectionPropertyID, this.backgroundRenderer.transform.InverseTransformDirection(normalized));
		this.SetTextPositionAndRotation(transform);
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x0009B904 File Offset: 0x00099B04
	private void SetTextPositionAndRotation(Transform pov)
	{
		if (!this._text.enabled || string.IsNullOrEmpty(this._text.text))
		{
			return;
		}
		this._text.transform.position = pov.position + this._textDistance * (pov.rotation * Vector3.forward);
		this._text.transform.rotation = Quaternion.LookRotation(pov.rotation * Vector3.forward, Vector3.up);
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x0009B994 File Offset: 0x00099B94
	private void UpdateUIPosition()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this._uiRoot.position = this.lastStablePosition + this.lastStableRotation * new Vector3(0f, 0f, 0.02f);
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		this.SetTextPositionAndRotation(transform);
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x0009BA13 File Offset: 0x00099C13
	public static bool GetInOverlay()
	{
		return !(PrivateUIRoom.instance == null) && PrivateUIRoom.instance.inOverlay;
	}

	// Token: 0x040026B7 RID: 9911
	[SerializeField]
	private TextMeshPro _text;

	// Token: 0x040026B8 RID: 9912
	[SerializeField]
	private float _textDistance = 4f;

	// Token: 0x040026B9 RID: 9913
	[SerializeField]
	private GameObject occluder;

	// Token: 0x040026BA RID: 9914
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x040026BB RID: 9915
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x040026BC RID: 9916
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x040026BD RID: 9917
	[SerializeField]
	private MeshRenderer backgroundRenderer;

	// Token: 0x040026BE RID: 9918
	[SerializeField]
	private string backgroundDirectionPropertyName = "_SpotDirection";

	// Token: 0x040026BF RID: 9919
	private int backgroundDirectionPropertyID;

	// Token: 0x040026C0 RID: 9920
	private int savedCullingLayers;

	// Token: 0x040026C1 RID: 9921
	private Transform _uiRoot;

	// Token: 0x040026C2 RID: 9922
	private Transform focusTransform;

	// Token: 0x040026C3 RID: 9923
	private List<Transform> ui;

	// Token: 0x040026C4 RID: 9924
	private Dictionary<Transform, Transform> uiParents;

	// Token: 0x040026C5 RID: 9925
	private float _initialAudioVolume;

	// Token: 0x040026C6 RID: 9926
	private bool inOverlay;

	// Token: 0x040026C7 RID: 9927
	private PrivateUIRoom.OverlaySource overlayForcedSources;

	// Token: 0x040026C8 RID: 9928
	private static PrivateUIRoom instance;

	// Token: 0x040026C9 RID: 9929
	private Vector3 lastStablePosition;

	// Token: 0x040026CA RID: 9930
	private Quaternion lastStableRotation;

	// Token: 0x040026CB RID: 9931
	[SerializeField]
	private float verticalPlay = 0.1f;

	// Token: 0x040026CC RID: 9932
	[SerializeField]
	private float lateralPlay = 0.5f;

	// Token: 0x040026CD RID: 9933
	[SerializeField]
	private float rotationalPlay = 45f;

	// Token: 0x040026CE RID: 9934
	private int? savedCullingLayersShoudlerCam;

	// Token: 0x040026CF RID: 9935
	private static Camera _shoulderCameraReference;

	// Token: 0x040026D0 RID: 9936
	private static CinemachineVirtualCamera _virtualCameraReference;

	// Token: 0x020004A0 RID: 1184
	[Flags]
	public enum OverlaySource
	{
		// Token: 0x040026D2 RID: 9938
		KID = 1,
		// Token: 0x040026D3 RID: 9939
		ModIO = 2,
		// Token: 0x040026D4 RID: 9940
		CustomMap = 4,
		// Token: 0x040026D5 RID: 9941
		AlarmClock = 8
	}
}
