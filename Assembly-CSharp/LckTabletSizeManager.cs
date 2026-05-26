using System;
using GorillaLocomotion;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class LckTabletSizeManager : MonoBehaviour
{
	// Token: 0x06001885 RID: 6277 RVA: 0x0008ADD1 File Offset: 0x00088FD1
	private void Start()
	{
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Combine(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
		this._controller.OnHorizontalModeChanged += this.OnHorizontalModeChanged;
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x0008AE11 File Offset: 0x00089011
	private void OnDestroy()
	{
		this._controller.OnHorizontalModeChanged -= this.OnHorizontalModeChanged;
		GTLckController controller = this._controller;
		controller.OnFOVUpdated = (Action<CameraMode>)Delegate.Remove(controller.OnFOVUpdated, new Action<CameraMode>(this.UpdateCustomNearClip));
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0008AE51 File Offset: 0x00089051
	private void OnHorizontalModeChanged(bool mode)
	{
		this.UpdateCustomNearClip(CameraMode.Selfie);
		this.UpdateCustomNearClip(CameraMode.FirstPerson);
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x0008AE64 File Offset: 0x00089064
	private void UpdateCustomNearClip(CameraMode mode)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		switch (mode)
		{
		case CameraMode.Selfie:
			this.SetCustomNearClip(this._selfieCamera);
			return;
		case CameraMode.FirstPerson:
			this.SetCustomNearClip(this._firstPersonCamera);
			break;
		case CameraMode.ThirdPerson:
		case CameraMode.Headset:
		case CameraMode.Drone:
			break;
		default:
			return;
		}
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x0008AEB4 File Offset: 0x000890B4
	private void SetCustomNearClip(Camera cam)
	{
		if (GTPlayer.Instance.IsDefaultScale)
		{
			return;
		}
		Matrix4x4 projectionMatrix;
		if (this._controller.HorizontalMode)
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 1.777778f, this._customNearClip, cam.farClipPlane);
		}
		else
		{
			projectionMatrix = Matrix4x4.Perspective(cam.fieldOfView, 0.5625f, this._customNearClip, cam.farClipPlane);
		}
		cam.projectionMatrix = projectionMatrix;
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0008AF1E File Offset: 0x0008911E
	private void ClearCustomNearClip()
	{
		this._selfieCamera.ResetProjectionMatrix();
		this._firstPersonCamera.ResetProjectionMatrix();
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x0008AF38 File Offset: 0x00089138
	private void PlayerBecameSmall()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamShrinkPosition;
		this._tabletFollower.SetPlayerSizeModifier(false, this._shrinkSize);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
		}
		this.SetCustomNearClip(this._selfieCamera);
		this.SetCustomNearClip(this._firstPersonCamera);
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x0008AF98 File Offset: 0x00089198
	private void PlayerBecameDefaultSize()
	{
		this._firstPersonCamera.transform.localPosition = this._firstPersonCamDefaultPosition;
		this._tabletFollower.SetPlayerSizeModifier(true, 1f);
		if (!this._lckDirectGrabbable.isGrabbed)
		{
			this.SetCameraOnNeck();
			base.transform.localScale = Vector3.one;
		}
		this.ClearCustomNearClip();
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x0008AFF8 File Offset: 0x000891F8
	private void SetCameraOnNeck()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find playerInstance!");
			return;
		}
		LckBodyCameraSpawner componentInChildren = instance.GetComponentInChildren<LckBodyCameraSpawner>(true);
		if (componentInChildren == null)
		{
			Debug.LogError("Unable to find bodyCameraSpawner!");
			return;
		}
		componentInChildren.ManuallySetCameraOnNeck();
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x0008B044 File Offset: 0x00089244
	private void Update()
	{
		if (!GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = false;
			this.PlayerBecameSmall();
			return;
		}
		if (GTPlayer.Instance.IsDefaultScale && this._isDefaultScale != GTPlayer.Instance.IsDefaultScale)
		{
			this._isDefaultScale = true;
			this.PlayerBecameDefaultSize();
		}
	}

	// Token: 0x040023A1 RID: 9121
	[SerializeField]
	private GTLckController _controller;

	// Token: 0x040023A2 RID: 9122
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x040023A3 RID: 9123
	[SerializeField]
	private GtTabletFollower _tabletFollower;

	// Token: 0x040023A4 RID: 9124
	[SerializeField]
	private Camera _firstPersonCamera;

	// Token: 0x040023A5 RID: 9125
	[SerializeField]
	private Camera _selfieCamera;

	// Token: 0x040023A6 RID: 9126
	private Vector3 _firstPersonCamShrinkPosition = new Vector3(0f, 0f, -0.78f);

	// Token: 0x040023A7 RID: 9127
	private Vector3 _firstPersonCamDefaultPosition = Vector3.zero;

	// Token: 0x040023A8 RID: 9128
	private float _shrinkSize = 0.06f;

	// Token: 0x040023A9 RID: 9129
	private Vector3 _shrinkVector = new Vector3(0.06f, 0.06f, 0.06f);

	// Token: 0x040023AA RID: 9130
	private float _customNearClip = 0.0006f;

	// Token: 0x040023AB RID: 9131
	private bool _isDefaultScale = true;
}
