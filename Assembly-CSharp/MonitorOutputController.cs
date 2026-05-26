using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class MonitorOutputController : MonoBehaviour
{
	// Token: 0x060018AC RID: 6316 RVA: 0x0008B99A File Offset: 0x00089B9A
	private void Awake()
	{
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x060018AD RID: 6317 RVA: 0x0008B9AD File Offset: 0x00089BAD
	private void OnEnable()
	{
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		LckBodyCameraSpawner.OnCameraStateChange += this.CameraStateChanged;
	}

	// Token: 0x060018AE RID: 6318 RVA: 0x0008B9D8 File Offset: 0x00089BD8
	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Object.Destroy(this);
		}
		if (this._shoulderCamera == null)
		{
			this.FindShoulderCamera();
		}
		if (this._lckCamera != null)
		{
			this._shoulderCamera.transform.position = this._lckCamera.transform.position;
			this._shoulderCamera.transform.rotation = this._lckCamera.transform.rotation;
			this._shoulderCamera.fieldOfView = this._lckCamera.fieldOfView;
			return;
		}
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x060018AF RID: 6319 RVA: 0x0008BA7E File Offset: 0x00089C7E
	private void CameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this.RestoreShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this.TakeOverShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this.TakeOverShoulderCamera();
			return;
		default:
			return;
		}
	}

	// Token: 0x060018B0 RID: 6320 RVA: 0x0008BAA7 File Offset: 0x00089CA7
	private void OnDisable()
	{
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		LckBodyCameraSpawner.OnCameraStateChange -= this.CameraStateChanged;
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x0008BAE7 File Offset: 0x00089CE7
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x060018B2 RID: 6322 RVA: 0x0008BAFC File Offset: 0x00089CFC
	private void TakeOverShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = false;
		this._shoulderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LCKHide"));
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x0008BB3C File Offset: 0x00089D3C
	private void RestoreShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		this._shoulderCamera.cullingMask |= 1 << LayerMask.NameToLayer("LCKHide");
		this._shoulderCamera.fieldOfView = this._shoulderCameraFov;
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x0008BB98 File Offset: 0x00089D98
	private void FindShoulderCamera()
	{
		if (this._shoulderCamera != null)
		{
			return;
		}
		if (!GorillaTagger.hasInstance || !base.isActiveAndEnabled)
		{
			return;
		}
		this._shoulderCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		this._shoulderCameraFov = this._shoulderCamera.fieldOfView;
	}

	// Token: 0x040023C4 RID: 9156
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x040023C5 RID: 9157
	private Camera _lckCamera;

	// Token: 0x040023C6 RID: 9158
	private CameraMode _lckActiveCameraMode;

	// Token: 0x040023C7 RID: 9159
	private Camera _shoulderCamera;

	// Token: 0x040023C8 RID: 9160
	private float _shoulderCameraFov;
}
