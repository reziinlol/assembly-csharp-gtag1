using System;
using GorillaLocomotion;
using Liv.Lck;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020003E8 RID: 1000
public class LckBodyCameraSpawner : MonoBehaviourTick
{
	// Token: 0x060017C2 RID: 6082 RVA: 0x00087E8D File Offset: 0x0008608D
	public void SetFollowTransform(Transform transform)
	{
		this._followTransform = transform;
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060017C3 RID: 6083 RVA: 0x00087E96 File Offset: 0x00086096
	public TabletSpawnInstance tabletSpawnInstance
	{
		get
		{
			return this._tabletSpawnInstance;
		}
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x060017C4 RID: 6084 RVA: 0x00087EA0 File Offset: 0x000860A0
	// (remove) Token: 0x060017C5 RID: 6085 RVA: 0x00087ED4 File Offset: 0x000860D4
	public static event LckBodyCameraSpawner.CameraStateDelegate OnCameraStateChange;

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060017C6 RID: 6086 RVA: 0x00087F07 File Offset: 0x00086107
	// (set) Token: 0x060017C7 RID: 6087 RVA: 0x00087F10 File Offset: 0x00086110
	public LckBodyCameraSpawner.CameraState cameraState
	{
		get
		{
			return this._cameraState;
		}
		set
		{
			switch (value)
			{
			case LckBodyCameraSpawner.CameraState.CameraDisabled:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.NotVisible;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = false;
				this.ResetCameraModel();
				this.cameraVisible = false;
				this._shouldMoveCameraToNeck = false;
				break;
			case LckBodyCameraSpawner.CameraState.CameraOnNeck:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				if (this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
				{
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
					this._tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
				}
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = true;
				this.ResetCameraModel();
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(false);
				}
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(true);
				break;
			case LckBodyCameraSpawner.CameraState.CameraSpawned:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = true;
				this._tabletSpawnInstance.cameraActive = true;
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(true);
				}
				this.ResetCameraModel();
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetDummyTabletBodyState(false);
				break;
			}
			this._cameraState = value;
			LckBodyCameraSpawner.CameraStateDelegate onCameraStateChange = LckBodyCameraSpawner.OnCameraStateChange;
			if (onCameraStateChange == null)
			{
				return;
			}
			onCameraStateChange(this._cameraState);
		}
	}

	// Token: 0x060017C8 RID: 6088 RVA: 0x00088074 File Offset: 0x00086274
	private void SetPreviewActive(bool isActive)
	{
		LckResult<LckService> service = LckService.GetService();
		if (!service.Success)
		{
			Debug.LogError("LCK Could not get Service" + service.Error.ToString());
			return;
		}
		LckService result = service.Result;
		if (result == null)
		{
			return;
		}
		result.SetPreviewActive(isActive);
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x060017C9 RID: 6089 RVA: 0x000880C5 File Offset: 0x000862C5
	// (set) Token: 0x060017CA RID: 6090 RVA: 0x000880D0 File Offset: 0x000862D0
	public LckBodyCameraSpawner.CameraPosition cameraPosition
	{
		get
		{
			return this._cameraPosition;
		}
		set
		{
			if (this._cameraModelTransform != null && this._cameraPosition != value)
			{
				switch (value)
				{
				case LckBodyCameraSpawner.CameraPosition.CameraDefault:
					this.ChangeCameraModelParent(this._cameraPositionDefault);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
					return;
				case LckBodyCameraSpawner.CameraPosition.CameraSlingshot:
					this.ChangeCameraModelParent(this._cameraPositionSlingshot);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
					break;
				case LckBodyCameraSpawner.CameraPosition.NotVisible:
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x060017CB RID: 6091 RVA: 0x0008812E File Offset: 0x0008632E
	// (set) Token: 0x060017CC RID: 6092 RVA: 0x00088140 File Offset: 0x00086340
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.enabled = value;
		}
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x0008815F File Offset: 0x0008635F
	private void Awake()
	{
		this._tabletSpawnInstance = new TabletSpawnInstance(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
	}

	// Token: 0x060017CE RID: 6094 RVA: 0x00088178 File Offset: 0x00086378
	private new void OnEnable()
	{
		base.OnEnable();
		this.InitCameraStrap();
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
		ZoneManagement.OnZoneChange += this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x060017CF RID: 6095 RVA: 0x00088232 File Offset: 0x00086432
	private void Update()
	{
		this._tabletSpawnInstance.Update();
	}

	// Token: 0x060017D0 RID: 6096 RVA: 0x00088240 File Offset: 0x00086440
	private new void OnDisable()
	{
		base.OnDisable();
		ZoneManagement.OnZoneChange -= this.OnZoneChanged;
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
	}

	// Token: 0x060017D1 RID: 6097 RVA: 0x000882E8 File Offset: 0x000864E8
	public override void Tick()
	{
		if (this._followTransform != null && base.transform.parent != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.parent.localToWorldMatrix;
			Vector3 position = localToWorldMatrix.MultiplyPoint(this._followTransform.localPosition + this._followTransform.localRotation * new Vector3(0f, -0.05f, 0.1f));
			Quaternion rotation = Quaternion.LookRotation(localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.forward), localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.up));
			base.transform.SetPositionAndRotation(position, rotation);
		}
		LckBodyCameraSpawner.CameraState cameraState = this._cameraState;
		if (cameraState != LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			if (cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned)
			{
				this.UpdateCameraStrap();
				if (this._cameraModelGrabbable.isGrabbed)
				{
					GorillaGrabber grabber = this._cameraModelGrabbable.grabber;
					Transform transform = grabber.transform;
					if (this.ShouldSpawnCamera(transform))
					{
						this.SpawnCamera(grabber, transform);
					}
				}
				else
				{
					this.ResetCameraModel();
				}
				if (this._tabletSpawnInstance.isSpawned)
				{
					Transform transform3;
					if (this._tabletSpawnInstance.directGrabbable.isGrabbed)
					{
						GorillaGrabber grabber2 = this._tabletSpawnInstance.directGrabbable.grabber;
						Transform transform2 = grabber2.transform;
						if (!this.ShouldSpawnCamera(transform2))
						{
							this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
							this._cameraModelGrabbable.target.SetPositionAndRotation(transform2.position, transform2.rotation * Quaternion.Euler(this._chestSpawnRotationOffset.x, this._chestSpawnRotationOffset.y, this._chestSpawnRotationOffset.z));
							this._tabletSpawnInstance.directGrabbable.ForceRelease();
							this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
							this._tabletSpawnInstance.ResetLocalPose();
							this._cameraModelGrabbable.ForceGrab(grabber2);
							this._cameraModelGrabbable.onReleased += this.OnCameraModelReleased;
							if (this._tabletSpawnInstance.Controller.CurrentCameraMode == CameraMode.Selfie)
							{
								this._returnToCameraMode = new CameraMode?(CameraMode.Selfie);
								this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
							}
						}
					}
					else if (this._shouldMoveCameraToNeck && GtTag.TryGetTransform(GtTagType.HMD, out transform3) && Vector3.SqrMagnitude(base.transform.position - this.tabletSpawnInstance.position) >= this._snapToNeckDistance * this._snapToNeckDistance)
					{
						this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
						this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
						this._tabletSpawnInstance.ResetLocalPose();
						this._shouldMoveCameraToNeck = false;
					}
				}
			}
		}
		else
		{
			this.UpdateCameraStrap();
			if (this._cameraModelGrabbable.isGrabbed)
			{
				GorillaGrabber grabber3 = this._cameraModelGrabbable.grabber;
				Transform transform4 = grabber3.transform;
				if (this.ShouldSpawnCamera(transform4))
				{
					this.SpawnCamera(grabber3, transform4);
					if (this._returnToCameraMode != null)
					{
						TabletSpawnInstance tabletSpawnInstance = this._tabletSpawnInstance;
						if (tabletSpawnInstance != null)
						{
							tabletSpawnInstance.Controller.SetCameraMode(this._returnToCameraMode.Value);
						}
						this._returnToCameraMode = null;
					}
				}
			}
			else
			{
				this.ResetCameraModel();
			}
		}
		if (!this.IsSlingshotActiveInHierarchy())
		{
			this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
			return;
		}
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
	}

	// Token: 0x060017D2 RID: 6098 RVA: 0x00088644 File Offset: 0x00086844
	private void OnZoneChanged(ZoneData[] zones)
	{
		if (!this._tabletSpawnInstance.isSpawned || this._tabletSpawnInstance.directGrabbable.isGrabbed)
		{
			return;
		}
		this._shouldMoveCameraToNeck = true;
	}

	// Token: 0x060017D3 RID: 6099 RVA: 0x0008866D File Offset: 0x0008686D
	private void OnDestroy()
	{
		this._tabletSpawnInstance.Dispose();
	}

	// Token: 0x060017D4 RID: 6100 RVA: 0x0008867C File Offset: 0x0008687C
	[ContextMenu("Put tablet on neck")]
	public void ManuallySetCameraOnNeck()
	{
		if (this.cameraState == LckBodyCameraSpawner.CameraState.CameraOnNeck || this.cameraState == LckBodyCameraSpawner.CameraState.CameraDisabled || !this._tabletSpawnInstance.isSpawned)
		{
			return;
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
		this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
		this._tabletSpawnInstance.ResetLocalPose();
		this._shouldMoveCameraToNeck = false;
		if (this._tabletSpawnInstance.Controller.CurrentCameraMode == CameraMode.Selfie)
		{
			this._returnToCameraMode = new CameraMode?(CameraMode.Selfie);
			this._tabletSpawnInstance.Controller.SetCameraMode(CameraMode.FirstPerson);
		}
	}

	// Token: 0x060017D5 RID: 6101 RVA: 0x00088703 File Offset: 0x00086903
	private void OnCameraModelReleased()
	{
		this._cameraModelGrabbable.onReleased -= this.OnCameraModelReleased;
		this.ResetCameraModel();
	}

	// Token: 0x060017D6 RID: 6102 RVA: 0x00088724 File Offset: 0x00086924
	public void SpawnCamera(GorillaGrabber overrideGorillaGrabber, Transform transform)
	{
		if (!this._tabletSpawnInstance.isSpawned)
		{
			this._tabletSpawnInstance.SpawnCamera();
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraSpawned;
		this._cameraModelGrabbable.ForceRelease();
		this._tabletSpawnInstance.ResetParent();
		Vector3 vector = Vector3.zero;
		Vector3 euler = Vector3.zero;
		euler = this._rotationOffsetWindows;
		XRNode xrNode = overrideGorillaGrabber.XrNode;
		if (xrNode != XRNode.LeftHand)
		{
			if (xrNode == XRNode.RightHand)
			{
				vector = this._rightHandSpawnOffsetWindows;
				euler.z = -12f;
			}
		}
		else
		{
			vector = this._leftHandSpawnOffsetWindows;
			euler.z = 12f;
		}
		if (!GTPlayer.Instance.IsDefaultScale)
		{
			vector *= 0.06f;
		}
		vector = transform.rotation * vector;
		this._tabletSpawnInstance.SetPositionAndRotation(transform.position + vector, transform.rotation * Quaternion.Euler(euler));
		this._tabletSpawnInstance.directGrabbable.ForceGrab(overrideGorillaGrabber);
		this._tabletSpawnInstance.SetLocalScale(Vector3.one);
	}

	// Token: 0x060017D7 RID: 6103 RVA: 0x00088824 File Offset: 0x00086A24
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 a = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 b = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(a - b) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x060017D8 RID: 6104 RVA: 0x0008887C File Offset: 0x00086A7C
	private void ChangeCameraModelParent(Transform transform)
	{
		if (this._cameraModelTransform != null)
		{
			this._cameraModelGrabbable.SetOriginalTargetParent(transform);
			if (!this._cameraModelGrabbable.isGrabbed)
			{
				this._cameraModelTransform.transform.parent = transform;
				this._cameraModelTransform.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x060017D9 RID: 6105 RVA: 0x000888D6 File Offset: 0x00086AD6
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x060017DA RID: 6106 RVA: 0x00088900 File Offset: 0x00086B00
	private void UpdateCameraStrap()
	{
		for (int i = 0; i < this._cameraStrapPoints.Length; i++)
		{
			this._cameraStrapPositions[i] = this._cameraStrapPoints[i].position;
		}
		this._cameraStrapRenderer.SetPositions(this._cameraStrapPositions);
		Vector3 lossyScale = base.transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.3333333f;
		this._cameraStrapRenderer.widthMultiplier = num * 0.02f;
		Color color = (this.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned) ? this._ghostColor : this._normalColor;
		this._cameraStrapRenderer.startColor = color;
		this._cameraStrapRenderer.endColor = color;
	}

	// Token: 0x060017DB RID: 6107 RVA: 0x000889B3 File Offset: 0x00086BB3
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x060017DC RID: 6108 RVA: 0x000889D5 File Offset: 0x00086BD5
	private VRRig GetLocalRig()
	{
		if (this._localRig == null)
		{
			this._localRig = VRRigCache.Instance.localRig.Rig;
		}
		return this._localRig;
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x00088A00 File Offset: 0x00086C00
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig localRig = this.GetLocalRig();
		if (localRig == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = localRig.projectileWeapon.InLeftHand();
		rightHand = localRig.projectileWeapon.InRightHand();
		return localRig.projectileWeapon.InHand();
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x00088A4C File Offset: 0x00086C4C
	private bool IsSlingshotActiveInHierarchy()
	{
		VRRig localRig = this.GetLocalRig();
		return !(localRig == null) && !(localRig.projectileWeapon == null) && localRig.projectileWeapon.gameObject.activeInHierarchy;
	}

	// Token: 0x040022F0 RID: 8944
	[SerializeField]
	private GameObject _cameraSpawnPrefab;

	// Token: 0x040022F1 RID: 8945
	[SerializeField]
	private Transform _cameraSpawnParentTransform;

	// Token: 0x040022F2 RID: 8946
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x040022F3 RID: 8947
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x040022F4 RID: 8948
	[SerializeField]
	private LckDirectGrabbable _cameraModelGrabbable;

	// Token: 0x040022F5 RID: 8949
	[SerializeField]
	private Transform _cameraPositionDefault;

	// Token: 0x040022F6 RID: 8950
	[SerializeField]
	private Transform _cameraPositionSlingshot;

	// Token: 0x040022F7 RID: 8951
	private Vector3 _chestSpawnRotationOffset = new Vector3(90f, 0f, 0f);

	// Token: 0x040022F8 RID: 8952
	private Vector3 _rightHandSpawnOffsetAndroid = new Vector3(-0.265f, 0.02f, -0.065f);

	// Token: 0x040022F9 RID: 8953
	private Vector3 _leftHandSpawnOffsetAndroid = new Vector3(0.245f, 0.022f, -0.12f);

	// Token: 0x040022FA RID: 8954
	private Vector3 _rotationOffsetAndroid = new Vector3(-90f, 60f, 125f);

	// Token: 0x040022FB RID: 8955
	private Vector3 _rotationOffsetWindows = new Vector3(-70f, -180f, 0f);

	// Token: 0x040022FC RID: 8956
	private Vector3 _rightHandSpawnOffsetWindows = new Vector3(-0.23f, -0.035f, -0.225f);

	// Token: 0x040022FD RID: 8957
	private Vector3 _leftHandSpawnOffsetWindows = new Vector3(0.23f, -0.035f, -0.225f);

	// Token: 0x040022FE RID: 8958
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x040022FF RID: 8959
	[SerializeField]
	private float _snapToNeckDistance = 6f;

	// Token: 0x04002300 RID: 8960
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x04002301 RID: 8961
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x04002302 RID: 8962
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x04002303 RID: 8963
	[SerializeField]
	private Color _ghostColor = Color.gray;

	// Token: 0x04002304 RID: 8964
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x04002305 RID: 8965
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x04002306 RID: 8966
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x04002307 RID: 8967
	private Transform _followTransform;

	// Token: 0x04002308 RID: 8968
	private Vector3[] _cameraStrapPositions;

	// Token: 0x04002309 RID: 8969
	private TabletSpawnInstance _tabletSpawnInstance;

	// Token: 0x0400230A RID: 8970
	private VRRig _localRig;

	// Token: 0x0400230B RID: 8971
	private bool _shouldMoveCameraToNeck;

	// Token: 0x0400230C RID: 8972
	private CameraMode? _returnToCameraMode;

	// Token: 0x0400230E RID: 8974
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x0400230F RID: 8975
	private LckBodyCameraSpawner.CameraPosition _cameraPosition;

	// Token: 0x020003E9 RID: 1001
	public enum CameraState
	{
		// Token: 0x04002311 RID: 8977
		CameraDisabled,
		// Token: 0x04002312 RID: 8978
		CameraOnNeck,
		// Token: 0x04002313 RID: 8979
		CameraSpawned
	}

	// Token: 0x020003EA RID: 1002
	public enum CameraPosition
	{
		// Token: 0x04002315 RID: 8981
		CameraDefault,
		// Token: 0x04002316 RID: 8982
		CameraSlingshot,
		// Token: 0x04002317 RID: 8983
		NotVisible
	}

	// Token: 0x020003EB RID: 1003
	// (Invoke) Token: 0x060017E1 RID: 6113
	public delegate void CameraStateDelegate(LckBodyCameraSpawner.CameraState state);
}
