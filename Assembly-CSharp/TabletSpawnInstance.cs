using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class TabletSpawnInstance : IDisposable
{
	// Token: 0x14000033 RID: 51
	// (add) Token: 0x060017AB RID: 6059 RVA: 0x00087AB4 File Offset: 0x00085CB4
	// (remove) Token: 0x060017AC RID: 6060 RVA: 0x00087AEC File Offset: 0x00085CEC
	public event Action onGrabbed;

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x060017AD RID: 6061 RVA: 0x00087B24 File Offset: 0x00085D24
	// (remove) Token: 0x060017AE RID: 6062 RVA: 0x00087B5C File Offset: 0x00085D5C
	public event Action onReleased;

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x060017AF RID: 6063 RVA: 0x00087B91 File Offset: 0x00085D91
	public LckDirectGrabbable directGrabbable
	{
		get
		{
			return this._lckSocialCameraManager.lckDirectGrabbable;
		}
	}

	// Token: 0x060017B0 RID: 6064 RVA: 0x00087B9E File Offset: 0x00085D9E
	public bool ResetLocalPose()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.localPosition = Vector3.zero;
		this._cameraSpawnInstanceTransform.localRotation = Quaternion.identity;
		return true;
	}

	// Token: 0x060017B1 RID: 6065 RVA: 0x00087BD1 File Offset: 0x00085DD1
	public bool ResetParent()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(this._cameraSpawnParentTransform);
		return true;
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00087BF5 File Offset: 0x00085DF5
	public bool SetParent(Transform transform)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(transform);
		return true;
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x060017B3 RID: 6067 RVA: 0x00087C14 File Offset: 0x00085E14
	// (set) Token: 0x060017B4 RID: 6068 RVA: 0x00087C1C File Offset: 0x00085E1C
	public bool cameraActive
	{
		get
		{
			return this._cameraActive;
		}
		set
		{
			this._cameraActive = value;
			if (!this._cameraActive && this.Controller != null)
			{
				this.Controller.StopRecording();
			}
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.cameraActive = this._cameraActive;
			}
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x060017B5 RID: 6069 RVA: 0x00087C71 File Offset: 0x00085E71
	// (set) Token: 0x060017B6 RID: 6070 RVA: 0x00087C79 File Offset: 0x00085E79
	public bool uiVisible
	{
		get
		{
			return this._uiVisible;
		}
		set
		{
			this._uiVisible = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.uiVisible = this._uiVisible;
			}
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x060017B7 RID: 6071 RVA: 0x00087CA1 File Offset: 0x00085EA1
	public bool isSpawned
	{
		get
		{
			return this._cameraGameObjectInstance != null;
		}
	}

	// Token: 0x060017B8 RID: 6072 RVA: 0x00087CAF File Offset: 0x00085EAF
	public TabletSpawnInstance(GameObject cameraSpawnPrefab, Transform cameraSpawnParentTransform)
	{
		this._cameraSpawnPrefab = cameraSpawnPrefab;
		this._cameraSpawnParentTransform = cameraSpawnParentTransform;
	}

	// Token: 0x060017B9 RID: 6073 RVA: 0x00087CC8 File Offset: 0x00085EC8
	public void Update()
	{
		if (this.Controller == null)
		{
			return;
		}
		Camera activeCamera = this.Controller.GetActiveCamera();
		Camera main = Camera.main;
		if (main != null)
		{
			activeCamera.nearClipPlane = main.nearClipPlane;
			activeCamera.farClipPlane = main.farClipPlane;
		}
	}

	// Token: 0x060017BA RID: 6074 RVA: 0x00087D18 File Offset: 0x00085F18
	public void SpawnCamera()
	{
		if (!this.isSpawned)
		{
			this._cameraGameObjectInstance = Object.Instantiate<GameObject>(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
			this._lckSocialCameraManager = this._cameraGameObjectInstance.GetComponent<LckSocialCameraManager>();
			this._lckSocialCameraManager.lckDirectGrabbable.onGrabbed += delegate()
			{
				Action action = this.onGrabbed;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._lckSocialCameraManager.lckDirectGrabbable.onReleased += delegate()
			{
				Action action = this.onReleased;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._cameraSpawnInstanceTransform = this._cameraGameObjectInstance.transform;
			this.Controller = this._cameraGameObjectInstance.GetComponent<GTLckController>();
		}
		this.uiVisible = this.uiVisible;
		this.cameraActive = this.cameraActive;
	}

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x060017BB RID: 6075 RVA: 0x00087DCA File Offset: 0x00085FCA
	public Vector3 position
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Vector3.zero;
			}
			return this._cameraSpawnInstanceTransform.position;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x060017BC RID: 6076 RVA: 0x00087DEB File Offset: 0x00085FEB
	public Quaternion rotation
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Quaternion.identity;
			}
			return this._cameraSpawnInstanceTransform.rotation;
		}
	}

	// Token: 0x060017BD RID: 6077 RVA: 0x00087E0C File Offset: 0x0008600C
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x060017BE RID: 6078 RVA: 0x00087E2A File Offset: 0x0008602A
	public void SetLocalScale(Vector3 scale)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.localScale = scale;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x00087E47 File Offset: 0x00086047
	public void Dispose()
	{
		if (this._cameraGameObjectInstance != null)
		{
			Object.Destroy(this._cameraGameObjectInstance);
			this._cameraGameObjectInstance = null;
		}
	}

	// Token: 0x040022E7 RID: 8935
	private GameObject _cameraGameObjectInstance;

	// Token: 0x040022E8 RID: 8936
	private GameObject _cameraSpawnPrefab;

	// Token: 0x040022E9 RID: 8937
	private GameEvents _GtCamera;

	// Token: 0x040022EA RID: 8938
	private Transform _cameraSpawnParentTransform;

	// Token: 0x040022EB RID: 8939
	private Transform _cameraSpawnInstanceTransform;

	// Token: 0x040022EC RID: 8940
	public GTLckController Controller;

	// Token: 0x040022ED RID: 8941
	private LckSocialCameraManager _lckSocialCameraManager;

	// Token: 0x040022EE RID: 8942
	private bool _cameraActive;

	// Token: 0x040022EF RID: 8943
	private bool _uiVisible;
}
