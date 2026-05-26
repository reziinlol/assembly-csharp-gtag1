using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000276 RID: 630
public class PropHuntHandFollower : MonoBehaviour, ICallBack
{
	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06001100 RID: 4352 RVA: 0x0005AC10 File Offset: 0x00058E10
	// (set) Token: 0x06001101 RID: 4353 RVA: 0x0005AC18 File Offset: 0x00058E18
	public bool hasProp
	{
		get
		{
			return this._hasProp;
		}
		private set
		{
			this._hasProp = value;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06001102 RID: 4354 RVA: 0x0005AC21 File Offset: 0x00058E21
	// (set) Token: 0x06001103 RID: 4355 RVA: 0x0005AC29 File Offset: 0x00058E29
	public bool IsInstantiatingAsync { get; private set; }

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06001104 RID: 4356 RVA: 0x0005AC32 File Offset: 0x00058E32
	// (set) Token: 0x06001105 RID: 4357 RVA: 0x0005AC3A File Offset: 0x00058E3A
	public VRRig attachedToRig { get; private set; }

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06001106 RID: 4358 RVA: 0x0005AC43 File Offset: 0x00058E43
	public bool IsLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0005AC4B File Offset: 0x00058E4B
	public void Awake()
	{
		this.attachedToRig = base.GetComponent<VRRig>();
		this.attachedToRig.propHuntHandFollower = this;
		this._isLocal = this.attachedToRig.isOfflineVRRig;
		this.raycastHits = new RaycastHit[20];
	}

	// Token: 0x06001108 RID: 4360 RVA: 0x0005AC83 File Offset: 0x00058E83
	public void Start()
	{
		this.attachedToRig.AddLateUpdateCallback(this);
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x0005AC91 File Offset: 0x00058E91
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropHandFollower(this);
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x0005AC99 File Offset: 0x00058E99
	private void OnDisable()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		this.DestroyProp();
		GorillaPropHuntGameManager.UnregisterPropHandFollower(this);
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x0005ACB0 File Offset: 0x00058EB0
	public void DestroyProp()
	{
		if (!this.hasProp || this._prop == null)
		{
			return;
		}
		PropHuntGrabbableProp prop;
		PropHuntTaggableProp prop2;
		if (this._prop.TryGetComponent<PropHuntGrabbableProp>(out prop))
		{
			PropHuntPools.ReturnGrabbableProp(prop);
		}
		else if (this._prop.TryGetComponent<PropHuntTaggableProp>(out prop2))
		{
			PropHuntPools.ReturnTaggableProp(prop2);
		}
		this._prop = null;
		this.hasProp = false;
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x0005AD10 File Offset: 0x00058F10
	public static void DestroyProp_NoPool(List<MeshCollider> _colliders, ref bool hasProp, ref GameObject _prop)
	{
		foreach (MeshCollider meshCollider in _colliders)
		{
			if (!(meshCollider == null))
			{
				meshCollider.gameObject.transform.parent = null;
				meshCollider.gameObject.SetActive(false);
			}
		}
		if (hasProp)
		{
			Object.Destroy(_prop);
		}
		_prop = null;
		hasProp = false;
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnRoundStart()
	{
	}

	// Token: 0x0600110E RID: 4366 RVA: 0x0005AD90 File Offset: 0x00058F90
	public void CreateProp()
	{
		if (this.hasProp)
		{
			this.DestroyProp();
		}
		this._isLeftHand = false;
		int num = GorillaPropHuntGameManager.instance.GetSeed();
		if (NetworkSystem.Instance.InRoom)
		{
			num += this.attachedToRig.OwningNetPlayer.ActorNumber;
		}
		SRand srand = new SRand(num);
		string cosmeticId = GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt());
		PropHuntTaggableProp propHuntTaggableProp;
		if (this._isLocal)
		{
			PropHuntGrabbableProp propHuntGrabbableProp;
			if (PropHuntPools.TryGetGrabbableProp(cosmeticId, out propHuntGrabbableProp))
			{
				this._grabbableProp = propHuntGrabbableProp;
				this._taggableProp = null;
				this._prop = propHuntGrabbableProp.gameObject;
				this._propOffset = this._grabbableProp.offset;
				propHuntGrabbableProp.handFollower = this;
				this.hasProp = true;
				for (int i = 0; i < propHuntGrabbableProp.interactionPoints.Count; i++)
				{
					propHuntGrabbableProp.interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
		}
		else if (PropHuntPools.TryGetTaggableProp(cosmeticId, out propHuntTaggableProp))
		{
			this._taggableProp = propHuntTaggableProp;
			this._grabbableProp = null;
			this._prop = propHuntTaggableProp.gameObject;
			this._propOffset = propHuntTaggableProp.offset;
			propHuntTaggableProp.ownerRig = this.attachedToRig;
			this.hasProp = true;
		}
	}

	// Token: 0x0600110F RID: 4367 RVA: 0x0005AEC0 File Offset: 0x000590C0
	public void OnPropLoaded(AsyncOperationHandle<GameObject> handle)
	{
		this.IsInstantiatingAsync = false;
		CosmeticSO debugCosmeticSO = null;
		if (PropHuntHandFollower.TryPrepPropTemplate(handle.Result, this._isLocal, debugCosmeticSO, this._colliders, this._interactionPoints, out this._grabbableProp, out this._taggableProp))
		{
			this._prop = handle.Result;
			this.hasProp = (this._prop != null);
			this._prop.SetActive(true);
			if (this._isLocal)
			{
				this._propOffset = this._grabbableProp.offset;
				this._grabbableProp.handFollower = this;
				for (int i = 0; i < this._interactionPoints.Count; i++)
				{
					this._interactionPoints[i].OnSpawn(this.attachedToRig);
				}
				return;
			}
			this._propOffset = this._taggableProp.offset;
			this._taggableProp.ownerRig = this.attachedToRig;
		}
	}

	// Token: 0x06001110 RID: 4368 RVA: 0x0005AFA8 File Offset: 0x000591A8
	public static bool TryPrepPropTemplate(GameObject _prop, bool _isLocal, CosmeticSO debugCosmeticSO, List<MeshCollider> _colliders, List<InteractionPoint> ref_interactionPoints, out PropHuntGrabbableProp grabbableProp, out PropHuntTaggableProp taggableProp)
	{
		if (_isLocal)
		{
			grabbableProp = _prop.AddComponent<PropHuntGrabbableProp>();
			taggableProp = null;
			grabbableProp.interactionPoints = ref_interactionPoints;
		}
		else
		{
			taggableProp = _prop.AddComponent<PropHuntTaggableProp>();
			grabbableProp = null;
		}
		bool flag = false;
		bool flag2 = true;
		Bounds bounds = default(Bounds);
		int num = 0;
		foreach (MeshRenderer meshRenderer in _prop.GetComponentsInChildren<MeshRenderer>())
		{
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			if (!(component == null))
			{
				Mesh sharedMesh = component.sharedMesh;
				if (!(sharedMesh == null) && sharedMesh.isReadable)
				{
					flag = true;
					if (flag2)
					{
						bounds = meshRenderer.bounds;
					}
					else
					{
						bounds.Encapsulate(meshRenderer.bounds);
					}
					MeshCollider meshCollider;
					if (num >= _colliders.Count)
					{
						GameObject gameObject = new GameObject("PropHuntTaggable");
						gameObject.layer = 14;
						meshCollider = gameObject.AddComponent<MeshCollider>();
						meshCollider.convex = true;
						meshCollider.isTrigger = true;
						if (_isLocal)
						{
							ref_interactionPoints.Add(gameObject.AddComponent<InteractionPoint>());
						}
						_colliders.Add(meshCollider);
					}
					else
					{
						meshCollider = _colliders[num];
						meshCollider.gameObject.SetActive(true);
					}
					meshCollider.transform.parent = _prop.transform;
					meshCollider.transform.position = meshRenderer.transform.position;
					meshCollider.transform.rotation = meshRenderer.transform.rotation;
					meshCollider.sharedMesh = sharedMesh;
					num++;
					flag2 = false;
				}
			}
		}
		if (!flag)
		{
			bool flag3 = true;
			PropHuntHandFollower.DestroyProp_NoPool(_colliders, ref flag3, ref _prop);
			return false;
		}
		Vector3 offset = _prop.transform.InverseTransformPoint(bounds.center);
		if (_isLocal)
		{
			grabbableProp.interactionPoints = ref_interactionPoints;
			grabbableProp.offset = offset;
		}
		else
		{
			taggableProp.offset = offset;
		}
		return true;
	}

	// Token: 0x06001111 RID: 4369 RVA: 0x0005B16C File Offset: 0x0005936C
	void ICallBack.CallBack()
	{
		if (!this.hasProp || this._prop.IsNull())
		{
			return;
		}
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		Vector3 sourcePos = transform.position;
		if (this.attachedToRig.isLocal)
		{
			sourcePos = (this._isLeftHand ? this.attachedToRig.leftHand.overrideTarget.position : this.attachedToRig.rightHand.overrideTarget.position);
		}
		if ((this._isLeftHand ? Mathf.Max(this.attachedToRig.leftIndex.calcT, this.attachedToRig.leftMiddle.calcT) : Mathf.Max(this.attachedToRig.rightIndex.calcT, this.attachedToRig.rightMiddle.calcT)) > 0.5f)
		{
			this._prop.transform.rotation = transform.TransformRotation(this._lastRelativeAngle);
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, transform.TransformPoint(this._lastRelativePos) + this._prop.transform.TransformVector(this._propOffset)) - this._prop.transform.TransformVector(this._propOffset);
			this._networkLastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
			this._networkLastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
			return;
		}
		Vector3 v = transform.transform.position - this._prop.transform.TransformPoint(this._propOffset);
		if (v.IsLongerThan(GorillaPropHuntGameManager.instance.HandFollowDistance))
		{
			float d = v.magnitude - GorillaPropHuntGameManager.instance.HandFollowDistance;
			this._prop.transform.position = this.GeoCollisionPoint(sourcePos, this._prop.transform.position + this._prop.transform.TransformVector(this._propOffset) + v.normalized * d) - this._prop.transform.TransformVector(this._propOffset);
		}
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
		this._networkLastRelativePos = this._lastRelativePos;
		this._networkLastRelativeAngle = this._lastRelativeAngle;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x0005B420 File Offset: 0x00059620
	public Vector3 GeoCollisionPoint(Vector3 sourcePos, Vector3 targetPos)
	{
		Vector3 vector = targetPos - sourcePos;
		int num = Physics.RaycastNonAlloc(sourcePos, vector.normalized, this.raycastHits, vector.magnitude, this.collisionLayers, QueryTriggerInteraction.Ignore);
		if (num > 0)
		{
			float sqrMagnitude = vector.sqrMagnitude;
			Vector3 result = targetPos;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector2 = this.raycastHits[i].point - sourcePos;
				if (vector2.sqrMagnitude < sqrMagnitude)
				{
					result = this.raycastHits[i].point;
					sqrMagnitude = vector2.sqrMagnitude;
				}
			}
			return result;
		}
		return targetPos;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x0005B4BC File Offset: 0x000596BC
	public void SwitchHand(bool newIsLeftHand)
	{
		if (this._isLeftHand == newIsLeftHand)
		{
			return;
		}
		this._isLeftHand = newIsLeftHand;
		Transform transform = this._isLeftHand ? this.attachedToRig.leftHand.rigTarget : this.attachedToRig.rightHand.rigTarget;
		this._lastRelativePos = transform.InverseTransformPoint(this._prop.transform.position);
		this._lastRelativeAngle = transform.InverseTransformRotation(this._prop.transform.rotation);
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x0005B53D File Offset: 0x0005973D
	public void SetProp(bool isLeftHand, Vector3 propPos, Quaternion propRot)
	{
		this._isLeftHand = isLeftHand;
		this._lastRelativePos = propPos;
		this._lastRelativeAngle = propRot;
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0005B554 File Offset: 0x00059754
	public long GetRelativePosRotLong()
	{
		if (this._prop.IsNull())
		{
			return BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity);
		}
		return BitPackUtils.PackHandPosRotForNetwork(this._lastRelativePos, this._lastRelativeAngle);
	}

	// Token: 0x04001433 RID: 5171
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x04001434 RID: 5172
	private const bool _k_isBetaOrEditor = false;

	// Token: 0x04001435 RID: 5173
	private const float HandFollowDistance = 0.1f;

	// Token: 0x04001436 RID: 5174
	private bool _hasProp;

	// Token: 0x04001439 RID: 5177
	private bool _isLocal;

	// Token: 0x0400143A RID: 5178
	private GameObject _prop;

	// Token: 0x0400143B RID: 5179
	private bool _isLeftHand;

	// Token: 0x0400143C RID: 5180
	private Vector3 _propOffset;

	// Token: 0x0400143D RID: 5181
	private readonly List<MeshCollider> _colliders = new List<MeshCollider>(4);

	// Token: 0x0400143E RID: 5182
	private readonly List<InteractionPoint> _interactionPoints = new List<InteractionPoint>(4);

	// Token: 0x0400143F RID: 5183
	private Vector3 _lastRelativePos;

	// Token: 0x04001440 RID: 5184
	private Quaternion _lastRelativeAngle;

	// Token: 0x04001441 RID: 5185
	private Vector3 _networkLastRelativePos;

	// Token: 0x04001442 RID: 5186
	private Quaternion _networkLastRelativeAngle;

	// Token: 0x04001443 RID: 5187
	public LayerMask collisionLayers;

	// Token: 0x04001444 RID: 5188
	private Vector3 targetPoint;

	// Token: 0x04001445 RID: 5189
	private RaycastHit[] raycastHits;

	// Token: 0x04001446 RID: 5190
	private PropHuntGrabbableProp _grabbableProp;

	// Token: 0x04001447 RID: 5191
	private PropHuntTaggableProp _taggableProp;
}
