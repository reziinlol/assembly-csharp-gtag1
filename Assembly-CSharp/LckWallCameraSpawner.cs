using System;
using System.Collections;
using GorillaLocomotion;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000408 RID: 1032
public class LckWallCameraSpawner : MonoBehaviour
{
	// Token: 0x06001890 RID: 6288 RVA: 0x0008B118 File Offset: 0x00089318
	private LckBodyCameraSpawner GetOrCreateBodyCameraSpawner()
	{
		if (LckWallCameraSpawner._bodySpawner != null)
		{
			return LckWallCameraSpawner._bodySpawner;
		}
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("Unable to find Player!");
			return null;
		}
		LckWallCameraSpawner.AddGTag(Camera.main.gameObject, GtTagType.HMD);
		LckWallCameraSpawner.AddGTag(instance.gameObject, GtTagType.Player);
		Transform transform = instance.bodyCollider.transform;
		GameObject gameObject = Object.Instantiate<GameObject>(this._lckBodySpawnerPrefab, transform.parent);
		Transform transform2 = gameObject.transform;
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		transform2.localScale = Vector3.one;
		LckWallCameraSpawner._bodySpawner = gameObject.GetComponent<LckBodyCameraSpawner>();
		LckWallCameraSpawner._bodySpawner.SetFollowTransform(transform);
		GorillaTagger instance2 = GorillaTagger.Instance;
		if (instance2 != null)
		{
			LckWallCameraSpawner.AddGTag(instance2.leftHandTriggerCollider, GtTagType.LeftHand);
			LckWallCameraSpawner.AddGTag(instance2.rightHandTriggerCollider, GtTagType.RightHand);
		}
		else
		{
			Debug.LogError("Unable to find GorillaTagger!");
		}
		return LckWallCameraSpawner._bodySpawner;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x0008B1FF File Offset: 0x000893FF
	private static void AddGTag(GameObject go, GtTagType gtTagType)
	{
		if (go.GetComponent<GtTag>())
		{
			return;
		}
		GtTag gtTag = go.AddComponent<GtTag>();
		gtTag.gtTagType = gtTagType;
		gtTag.enabled = true;
	}

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001892 RID: 6290 RVA: 0x0008B222 File Offset: 0x00089422
	// (set) Token: 0x06001893 RID: 6291 RVA: 0x0008B22C File Offset: 0x0008942C
	public LckWallCameraSpawner.WallSpawnerState wallSpawnerState
	{
		get
		{
			return this._wallSpawnerState;
		}
		set
		{
			switch (value)
			{
			case LckWallCameraSpawner.WallSpawnerState.CameraOnHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			case LckWallCameraSpawner.WallSpawnerState.CameraOffHook:
				this.ResetCameraModel();
				this.UpdateCameraStrap();
				this.cameraVisible = true;
				break;
			}
			this._wallSpawnerState = value;
		}
	}

	// Token: 0x06001894 RID: 6292 RVA: 0x0008B27C File Offset: 0x0008947C
	private void Awake()
	{
		this.InitCameraStrap();
	}

	// Token: 0x06001895 RID: 6293 RVA: 0x0008B284 File Offset: 0x00089484
	private void OnEnable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Combine(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed += this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased += this.OnReleased;
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x0008B348 File Offset: 0x00089548
	private void Start()
	{
		this.CreatePrewarmCamera();
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x0008B350 File Offset: 0x00089550
	private void Update()
	{
		LckWallCameraSpawner.WallSpawnerState wallSpawnerState = this._wallSpawnerState;
		if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraOnHook)
		{
			if (wallSpawnerState != LckWallCameraSpawner.WallSpawnerState.CameraDragging)
			{
				return;
			}
			this.UpdateCameraStrap();
			if (this.ShouldSpawnCamera(this._cameraHandleGrabbable.grabber.transform))
			{
				this.SpawnCamera(this._cameraHandleGrabbable.grabber);
			}
		}
		else
		{
			if (this.GetOrCreateBodyCameraSpawner() == null)
			{
				Debug.LogError("Lck, Unable to find LckBodyCameraSpawner");
				base.gameObject.SetActive(false);
				return;
			}
			if (LckWallCameraSpawner._bodySpawner.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.isSpawned && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable.isGrabbed)
			{
				LckDirectGrabbable directGrabbable = LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.directGrabbable;
				GorillaGrabber grabber = directGrabbable.grabber;
				if (!this.ShouldSpawnCamera(grabber.transform))
				{
					directGrabbable.ForceRelease();
					LckWallCameraSpawner._bodySpawner.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
					this._cameraHandleGrabbable.target.SetPositionAndRotation(grabber.transform.position, grabber.transform.rotation * Quaternion.Euler(this._spawnRotationOffsetWindows, 180f, 0f));
					this._cameraHandleGrabbable.ForceGrab(grabber);
					return;
				}
			}
		}
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x0008B488 File Offset: 0x00089688
	private void OnDisable()
	{
		if (this._swapTablet != null && this._swapEmobi != null && this._dummyTablet != null)
		{
			LckGameObjectSwapCosmetic swapTablet = this._swapTablet;
			swapTablet.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapTablet.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnTabletCosmeticSpawned));
			LckGameObjectSwapCosmetic swapEmobi = this._swapEmobi;
			swapEmobi.OnCosmeticSpawned = (Action<GameObject>)Delegate.Remove(swapEmobi.OnCosmeticSpawned, new Action<GameObject>(this._dummyTablet.OnEmobiCosmeticSpawned));
		}
		this._cameraHandleGrabbable.onGrabbed -= this.OnGrabbed;
		this._cameraHandleGrabbable.onReleased -= this.OnReleased;
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001899 RID: 6297 RVA: 0x0008B545 File Offset: 0x00089745
	// (set) Token: 0x0600189A RID: 6298 RVA: 0x0008B557 File Offset: 0x00089757
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.gameObject.SetActive(value);
		}
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x0008B57C File Offset: 0x0008977C
	private void SpawnCamera(GorillaGrabber lastGorillaGrabber)
	{
		if (LckWallCameraSpawner._bodySpawner == null)
		{
			Debug.LogError("Lck, unable to spawn camera, body spawner is null!");
			return;
		}
		if (LckWallCameraSpawner._bodySpawner.tabletSpawnInstance != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup != null && LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor())
		{
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.GetCurrentTriggerProcessor().ResetToDefaultAndTriggerButton();
			LckWallCameraSpawner._bodySpawner.tabletSpawnInstance.Controller.GtColliderTriggerProcessorsGroup.ClearAllTriggers();
		}
		this.cameraVisible = false;
		this._cameraHandleGrabbable.ForceRelease();
		LckWallCameraSpawner._bodySpawner.SpawnCamera(lastGorillaGrabber, lastGorillaGrabber.transform);
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0008B65D File Offset: 0x0008985D
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x0008B688 File Offset: 0x00089888
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
		this._cameraStrapRenderer.startColor = (this._cameraStrapRenderer.endColor = this._normalColor);
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x0008B72A File Offset: 0x0008992A
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x0600189F RID: 6303 RVA: 0x0008B74C File Offset: 0x0008994C
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 a = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 b = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(a - b) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x0008B7A2 File Offset: 0x000899A2
	private void OnGrabbed()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraDragging;
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x0008B7AB File Offset: 0x000899AB
	private void OnReleased()
	{
		this.wallSpawnerState = LckWallCameraSpawner.WallSpawnerState.CameraOnHook;
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x0008B7B4 File Offset: 0x000899B4
	private void CreatePrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("prewarm camera");
		gameObject.transform.SetParent(base.transform);
		LckWallCameraSpawner._prewarmCamera = gameObject.AddComponent<Camera>();
		Camera main = Camera.main;
		LckWallCameraSpawner._prewarmCamera.clearFlags = main.clearFlags;
		LckWallCameraSpawner._prewarmCamera.fieldOfView = main.fieldOfView;
		LckWallCameraSpawner._prewarmCamera.nearClipPlane = main.nearClipPlane;
		LckWallCameraSpawner._prewarmCamera.farClipPlane = main.farClipPlane;
		LckWallCameraSpawner._prewarmCamera.cullingMask = main.cullingMask;
		LckWallCameraSpawner._prewarmCamera.tag = "Untagged";
		LckWallCameraSpawner._prewarmCamera.stereoTargetEye = StereoTargetEyeMask.None;
		LckWallCameraSpawner._prewarmCamera.targetTexture = new RenderTexture(32, 32, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt);
		LckWallCameraSpawner._prewarmCamera.transform.SetPositionAndRotation(main.transform.position, main.transform.rotation);
		base.StartCoroutine(this.DestroyPrewarmCameraDelayed());
	}

	// Token: 0x060018A3 RID: 6307 RVA: 0x0008B8AC File Offset: 0x00089AAC
	private IEnumerator DestroyPrewarmCameraDelayed()
	{
		yield return new WaitForSeconds(1f);
		this.DestroyPrewarmCamera();
		yield break;
	}

	// Token: 0x060018A4 RID: 6308 RVA: 0x0008B8BB File Offset: 0x00089ABB
	private void DestroyPrewarmCamera()
	{
		if (LckWallCameraSpawner._prewarmCamera == null)
		{
			return;
		}
		RenderTexture targetTexture = LckWallCameraSpawner._prewarmCamera.targetTexture;
		LckWallCameraSpawner._prewarmCamera.targetTexture = null;
		targetTexture.Release();
		Object.Destroy(LckWallCameraSpawner._prewarmCamera.gameObject);
		LckWallCameraSpawner._prewarmCamera = null;
	}

	// Token: 0x040023AC RID: 9132
	[SerializeField]
	private GameObject _lckBodySpawnerPrefab;

	// Token: 0x040023AD RID: 9133
	[SerializeField]
	private LckDirectGrabbable _cameraHandleGrabbable;

	// Token: 0x040023AE RID: 9134
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x040023AF RID: 9135
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x040023B0 RID: 9136
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x040023B1 RID: 9137
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x040023B2 RID: 9138
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x040023B3 RID: 9139
	private Vector3[] _cameraStrapPositions;

	// Token: 0x040023B4 RID: 9140
	private float _spawnRotationOffsetAndroid = -80f;

	// Token: 0x040023B5 RID: 9141
	private float _spawnRotationOffsetWindows = -55f;

	// Token: 0x040023B6 RID: 9142
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x040023B7 RID: 9143
	[Header("Cosmetics References")]
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x040023B8 RID: 9144
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapTablet;

	// Token: 0x040023B9 RID: 9145
	[SerializeField]
	private LckGameObjectSwapCosmetic _swapEmobi;

	// Token: 0x040023BA RID: 9146
	private static LckBodyCameraSpawner _bodySpawner;

	// Token: 0x040023BB RID: 9147
	private static Camera _prewarmCamera;

	// Token: 0x040023BC RID: 9148
	private LckWallCameraSpawner.WallSpawnerState _wallSpawnerState;

	// Token: 0x02000409 RID: 1033
	public enum WallSpawnerState
	{
		// Token: 0x040023BE RID: 9150
		CameraOnHook,
		// Token: 0x040023BF RID: 9151
		CameraDragging,
		// Token: 0x040023C0 RID: 9152
		CameraOffHook
	}
}
