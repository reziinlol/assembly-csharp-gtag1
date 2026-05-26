using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020010FB RID: 4347
	public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x06006D6A RID: 28010 RVA: 0x0023C194 File Offset: 0x0023A394
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x06006D6B RID: 28011 RVA: 0x0023C19D File Offset: 0x0023A39D
		// (set) Token: 0x06006D6C RID: 28012 RVA: 0x0023C1A5 File Offset: 0x0023A3A5
		public bool isIdle { get; private set; }

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06006D6D RID: 28013 RVA: 0x0023C1AE File Offset: 0x0023A3AE
		// (set) Token: 0x06006D6E RID: 28014 RVA: 0x0023C1B6 File Offset: 0x0023A3B6
		public bool isFullyIdle { get; private set; }

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06006D6F RID: 28015 RVA: 0x0023C1BF File Offset: 0x0023A3BF
		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06006D70 RID: 28016 RVA: 0x0023C1C7 File Offset: 0x0023A3C7
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x06006D71 RID: 28017 RVA: 0x0023C1E4 File Offset: 0x0023A3E4
		protected virtual void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x06006D72 RID: 28018 RVA: 0x0023C247 File Offset: 0x0023A447
		protected virtual void Start()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			RopeSwingManager.Register(this);
			this.started = true;
		}

		// Token: 0x06006D73 RID: 28019 RVA: 0x0023C265 File Offset: 0x0023A465
		private void OnDestroy()
		{
			if (RopeSwingManager.instance != null)
			{
				RopeSwingManager.Unregister(this);
			}
		}

		// Token: 0x06006D74 RID: 28020 RVA: 0x0023C27C File Offset: 0x0023A47C
		protected virtual void OnEnable()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
		}

		// Token: 0x06006D75 RID: 28021 RVA: 0x0023C2EB File Offset: 0x0023A4EB
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
			GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
		}

		// Token: 0x06006D76 RID: 28022 RVA: 0x0023C30C File Offset: 0x0023A50C
		internal void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
			int staticHash2 = base.GetType().Name.GetStaticHash();
			int num = StaticHash.Compute(staticHash, staticHash2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					Vector3 position = transform.position;
					int i = StaticHash.Compute(position.x, position.y, position.z);
					int instanceID = transform.GetInstanceID();
					int num2 = StaticHash.Compute(num, i, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num2);
				}
				this.ropeId = this.staticId.GetStaticHash();
				return;
			}
			this.ropeId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x06006D77 RID: 28023 RVA: 0x0023C3C8 File Offset: 0x0023A5C8
		public void InvokeUpdate()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfoWrapped));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
			if (this.hasMonkeBlockParent && this.supportMovingAtRuntime)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
			}
		}

		// Token: 0x06006D78 RID: 28024 RVA: 0x0023C5FC File Offset: 0x0023A7FC
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -this.ropeBitGenOffset, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x06006D79 RID: 28025 RVA: 0x0023C6A3 File Offset: 0x0023A8A3
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

		// Token: 0x06006D7A RID: 28026 RVA: 0x0023C6C4 File Offset: 0x0023A8C4
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x06006D7B RID: 28027 RVA: 0x0023C700 File Offset: 0x0023A900
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x06006D7C RID: 28028 RVA: 0x0023C899 File Offset: 0x0023AA99
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x06006D7D RID: 28029 RVA: 0x0023C8D8 File Offset: 0x0023AAD8
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x06006D7E RID: 28030 RVA: 0x0023C93C File Offset: 0x0023AB3C
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x06006D7F RID: 28031 RVA: 0x0023C9C0 File Offset: 0x0023ABC0
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x06006D80 RID: 28032 RVA: 0x0023CA2D File Offset: 0x0023AC2D
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x06006D81 RID: 28033 RVA: 0x0023CA44 File Offset: 0x0023AC44
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			float num = 10000f;
			if (!velocity.IsValid(num))
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x06006D82 RID: 28034 RVA: 0x0023CB4C File Offset: 0x0023AD4C
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.monkeBlockParent = base.GetComponentInParent<BuilderPiece>();
			this.hasMonkeBlockParent = (this.monkeBlockParent != null);
			int num = StaticHash.Compute(pieceType, pieceId);
			this.staticId = string.Format("#ID_{0:X8}", num);
			this.ropeId = this.staticId.GetStaticHash();
			GorillaRopeSwing gorillaRopeSwing;
			if (this.started && !RopeSwingManager.instance.TryGetRope(this.ropeId, out gorillaRopeSwing))
			{
				RopeSwingManager.Register(this);
			}
		}

		// Token: 0x06006D83 RID: 28035 RVA: 0x0023CBC8 File Offset: 0x0023ADC8
		public void OnPieceDestroy()
		{
			RopeSwingManager.Unregister(this);
		}

		// Token: 0x06006D84 RID: 28036 RVA: 0x0023CBD0 File Offset: 0x0023ADD0
		public void OnPiecePlacementDeserialized()
		{
			VectorizedCustomRopeSimulation.Unregister(this);
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x06006D85 RID: 28037 RVA: 0x0023CC59 File Offset: 0x0023AE59
		public void OnPieceActivate()
		{
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x06006D86 RID: 28038 RVA: 0x0023CC78 File Offset: 0x0023AE78
		private bool IsAttachedToMovingPiece()
		{
			return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != null;
		}

		// Token: 0x06006D87 RID: 28039 RVA: 0x0023CCD8 File Offset: 0x0023AED8
		public void OnPieceDeactivate()
		{
			this.supportMovingAtRuntime = false;
		}

		// Token: 0x04007E6F RID: 32367
		public int ropeId;

		// Token: 0x04007E70 RID: 32368
		public string staticId;

		// Token: 0x04007E71 RID: 32369
		public bool useStaticId;

		// Token: 0x04007E72 RID: 32370
		protected float ropeBitGenOffset = 1f;

		// Token: 0x04007E73 RID: 32371
		[SerializeField]
		protected GameObject prefabRopeBit;

		// Token: 0x04007E74 RID: 32372
		[SerializeField]
		private bool supportMovingAtRuntime;

		// Token: 0x04007E75 RID: 32373
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x04007E76 RID: 32374
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x04007E77 RID: 32375
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x04007E78 RID: 32376
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x04007E79 RID: 32377
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x04007E7A RID: 32378
		private bool localPlayerOn;

		// Token: 0x04007E7B RID: 32379
		private int localPlayerBoneIndex;

		// Token: 0x04007E7C RID: 32380
		private XRNode localPlayerXRNode;

		// Token: 0x04007E7D RID: 32381
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x04007E7E RID: 32382
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04007E81 RID: 32385
		private float potentialIdleTimer;

		// Token: 0x04007E82 RID: 32386
		[SerializeField]
		protected int ropeLength = 8;

		// Token: 0x04007E83 RID: 32387
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x04007E84 RID: 32388
		private bool hasMonkeBlockParent;

		// Token: 0x04007E85 RID: 32389
		private BuilderPiece monkeBlockParent;

		// Token: 0x04007E86 RID: 32390
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x04007E87 RID: 32391
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x04007E88 RID: 32392
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x04007E89 RID: 32393
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x04007E8A RID: 32394
		private float scaleFactor = 1f;

		// Token: 0x04007E8B RID: 32395
		private bool started;

		// Token: 0x04007E8C RID: 32396
		private int lastNodeCheckIndex = 2;
	}
}
