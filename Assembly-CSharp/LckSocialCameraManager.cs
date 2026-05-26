using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000406 RID: 1030
public class LckSocialCameraManager : MonoBehaviour
{
	// Token: 0x1700026B RID: 619
	// (get) Token: 0x0600186E RID: 6254 RVA: 0x0008A891 File Offset: 0x00088A91
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x0600186F RID: 6255 RVA: 0x0008A899 File Offset: 0x00088A99
	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x0008A8A0 File Offset: 0x00088AA0
	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x0008A8BC File Offset: 0x00088ABC
	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += this.OnRecordingStarted;
			service.Result.OnStreamingStarted += this.OnRecordingStarted;
			service.Result.OnRecordingStopped += this.OnRecordingStopped;
			service.Result.OnStreamingStopped += this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange += this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		this._cameraMode = this._gtLckController.CurrentCameraMode;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x0008A96C File Offset: 0x00088B6C
	private void Update()
	{
		if (this._lckCamera != null)
		{
			Transform transform = this._lckCamera.transform;
			if (this._networkedCococam != null)
			{
				this._networkedCococam.transform.position = transform.position;
				this._networkedCococam.transform.rotation = transform.rotation;
			}
			if (this._networkedTablet != null)
			{
				if (this._networkedTablet.IsOnNeck)
				{
					this._networkedTablet.transform.position = base.transform.position;
				}
				else
				{
					this._networkedTablet.transform.position = base.transform.position + this._tabletPositionOffset * this._networkedTablet.VrRig.scaleFactor;
				}
				this._networkedTablet.transform.rotation = base.transform.rotation;
			}
		}
		if (this._needsUpdate)
		{
			this.UpdateCococamVisibility(this._cameraState, this._cameraMode, this._isForceHidden, this.cameraActive);
			this.UpdateTabletVisibility(this._cameraState, this._isForceHidden, this.cameraActive);
			this.UpdateCococamRecording(this._isRecording);
			this.UpdateTabletRecording(this._isRecording);
			this._needsUpdate = false;
		}
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x0008AAC0 File Offset: 0x00088CC0
	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= this.OnRecordingStarted;
			service.Result.OnStreamingStarted -= this.OnRecordingStarted;
			service.Result.OnRecordingStopped -= this.OnRecordingStopped;
			service.Result.OnStreamingStopped -= this.OnRecordingStopped;
		}
		LckBodyCameraSpawner.OnCameraStateChange -= this.OnBodyCameraStateChanged;
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x0008AB5F File Offset: 0x00088D5F
	public void SetForceHidden(bool hidden)
	{
		if (this._isForceHidden == hidden)
		{
			return;
		}
		this._isForceHidden = hidden;
		this._needsUpdate = true;
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x0008AB79 File Offset: 0x00088D79
	public void SetLckSocialCococamCamera(LckSocialCamera socialCamera)
	{
		if (this._networkedCococam == socialCamera)
		{
			return;
		}
		this._networkedCococam = socialCamera;
		this._needsUpdate = true;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x0008AB98 File Offset: 0x00088D98
	public void SetLckSocialTabletCamera(LckSocialCamera socialCameraTablet)
	{
		if (this._networkedTablet == socialCameraTablet)
		{
			return;
		}
		this._networkedTablet = socialCameraTablet;
		this._needsUpdate = true;
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001877 RID: 6263 RVA: 0x0008ABB7 File Offset: 0x00088DB7
	// (set) Token: 0x06001878 RID: 6264 RVA: 0x0008ABC4 File Offset: 0x00088DC4
	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			if (this._localCameras.activeSelf == value)
			{
				return;
			}
			this._localCameras.SetActive(value);
			this._needsUpdate = true;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001879 RID: 6265 RVA: 0x0008ABE8 File Offset: 0x00088DE8
	// (set) Token: 0x0600187A RID: 6266 RVA: 0x0008ABF5 File Offset: 0x00088DF5
	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x0008AC03 File Offset: 0x00088E03
	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned(this);
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x0008AC1B File Offset: 0x00088E1B
	private void OnBodyCameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		if (this._cameraState == state)
		{
			return;
		}
		this._cameraState = state;
		this._needsUpdate = true;
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x0008AC35 File Offset: 0x00088E35
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		if (this._cameraMode == mode)
		{
			return;
		}
		this._cameraMode = mode;
		this._needsUpdate = true;
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x0008AC5B File Offset: 0x00088E5B
	private void OnRecordingStarted(LckResult result)
	{
		if (this._isRecording == result.Success)
		{
			return;
		}
		this._isRecording = result.Success;
		this._needsUpdate = true;
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x0008AC7F File Offset: 0x00088E7F
	private void OnRecordingStopped(LckResult result)
	{
		if (!this._isRecording)
		{
			return;
		}
		this._isRecording = false;
		this._needsUpdate = true;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x0008AC98 File Offset: 0x00088E98
	private void UpdateCococamRecording(bool recording)
	{
		this.CoconutCamera.SetRecordingState(recording);
		if (this._networkedCococam == null)
		{
			return;
		}
		this._networkedCococam.recording = recording;
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x0008ACC4 File Offset: 0x00088EC4
	private void UpdateCococamVisibility(LckBodyCameraSpawner.CameraState cameraState, CameraMode cameraMode, bool forceHidden, bool cameraActive)
	{
		if (cameraMode == CameraMode.ThirdPerson || cameraMode == CameraMode.Drone)
		{
			this.CoconutCamera.SetVisualsActive(cameraActive);
		}
		else
		{
			this.CoconutCamera.SetVisualsActive(false);
		}
		if (this._networkedCococam == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden || !cameraActive)
		{
			this._networkedCococam.visible = false;
			return;
		}
		this._networkedCococam.visible = (cameraMode == CameraMode.ThirdPerson || cameraMode == CameraMode.Drone);
	}

	// Token: 0x06001882 RID: 6274 RVA: 0x0008AD32 File Offset: 0x00088F32
	private void UpdateTabletRecording(bool recording)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		this._networkedTablet.recording = recording;
	}

	// Token: 0x06001883 RID: 6275 RVA: 0x0008AD50 File Offset: 0x00088F50
	private void UpdateTabletVisibility(LckBodyCameraSpawner.CameraState cameraState, bool forceHidden, bool cameraActive)
	{
		if (this._networkedTablet == null)
		{
			return;
		}
		if (cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || forceHidden)
		{
			this._networkedTablet.visible = false;
			this._networkedTablet.IsOnNeck = false;
			return;
		}
		this._networkedTablet.visible = cameraActive;
		this._networkedTablet.IsOnNeck = (cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck);
	}

	// Token: 0x04002391 RID: 9105
	[SerializeField]
	private GameObject _localUi;

	// Token: 0x04002392 RID: 9106
	[SerializeField]
	private GameObject _localCameras;

	// Token: 0x04002393 RID: 9107
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x04002394 RID: 9108
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x04002395 RID: 9109
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x04002396 RID: 9110
	private LckSocialCamera _networkedCococam;

	// Token: 0x04002397 RID: 9111
	private LckSocialCamera _networkedTablet;

	// Token: 0x04002398 RID: 9112
	private Camera _lckCamera;

	// Token: 0x04002399 RID: 9113
	private CameraMode _cameraMode;

	// Token: 0x0400239A RID: 9114
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x0400239B RID: 9115
	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	// Token: 0x0400239C RID: 9116
	public static Action<LckSocialCameraManager> OnManagerSpawned;

	// Token: 0x0400239D RID: 9117
	private bool _isRecording;

	// Token: 0x0400239E RID: 9118
	private bool _isForceHidden;

	// Token: 0x0400239F RID: 9119
	private bool _needsUpdate = true;

	// Token: 0x040023A0 RID: 9120
	private Vector3 _tabletPositionOffset = new Vector3(0f, 0.11f, -0.08f);
}
