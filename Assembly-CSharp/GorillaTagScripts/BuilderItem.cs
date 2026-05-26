using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED5 RID: 3797
	public class BuilderItem : TransferrableObject
	{
		// Token: 0x06005D8D RID: 23949 RVA: 0x001DACF0 File Offset: 0x001D8EF0
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06005D8E RID: 23950 RVA: 0x001DAD10 File Offset: 0x001D8F10
		protected override void Awake()
		{
			base.Awake();
			this.parent = base.transform.parent;
			this.currTable = null;
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			this.initialGrabInteractorScale = this.gripInteractor.transform.localScale;
		}

		// Token: 0x06005D8F RID: 23951 RVA: 0x000AFE88 File Offset: 0x000AE088
		internal override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x06005D90 RID: 23952 RVA: 0x0004B3FC File Offset: 0x000495FC
		internal override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x06005D91 RID: 23953 RVA: 0x001DAD73 File Offset: 0x001D8F73
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06005D92 RID: 23954 RVA: 0x001DAD90 File Offset: 0x001D8F90
		public void AttachPiece(BuilderPiece piece)
		{
			base.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
			piece.transform.localScale = Vector3.one;
			piece.transform.SetParent(this.itemRoot.transform);
			Debug.LogFormat(piece.gameObject, "Attach Piece {0} to container {1}", new object[]
			{
				piece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = piece;
		}

		// Token: 0x06005D93 RID: 23955 RVA: 0x001DAE28 File Offset: 0x001D9028
		public void DetachPiece(BuilderPiece piece)
		{
			if (piece != this.attachedPiece)
			{
				Debug.LogErrorFormat("Trying to detach piece {0} from a container containing {1}", new object[]
				{
					piece.pieceId,
					this.attachedPiece.pieceId
				});
				return;
			}
			piece.transform.SetParent(null);
			Debug.LogFormat(this.attachedPiece.gameObject, "Detach Piece {0} from container {1}", new object[]
			{
				this.attachedPiece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = null;
		}

		// Token: 0x06005D94 RID: 23956 RVA: 0x001DAED0 File Offset: 0x001D90D0
		private new void OnStateChanged()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				this.enableCollidersWhenReady = true;
				this.gripInteractor.transform.localScale = this.initialGrabInteractorScale * 2f;
				this.handsFreeOfCollidersTime = 0f;
				return;
			}
			this.enableCollidersWhenReady = false;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			this.handsFreeOfCollidersTime = 0f;
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001DAF44 File Offset: 0x001D9144
		public override Matrix4x4 GetDefaultTransformationMatrix()
		{
			if (this.reliableState.dirty)
			{
				base.SetupHandMatrix(this.reliableState.leftHandAttachPos, this.reliableState.leftHandAttachRot, this.reliableState.rightHandAttachPos, this.reliableState.rightHandAttachRot);
				this.reliableState.dirty = false;
			}
			return base.GetDefaultTransformationMatrix();
		}

		// Token: 0x06005D96 RID: 23958 RVA: 0x001DAFA4 File Offset: 0x001D91A4
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			BuilderItem.BuilderItemState itemState = (BuilderItem.BuilderItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
			if (this.enableCollidersWhenReady)
			{
				bool flag = this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsRight) || this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsLeft);
				this.handsFreeOfCollidersTime += (flag ? 0f : Time.deltaTime);
				if (this.handsFreeOfCollidersTime > 0.1f)
				{
					this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
					this.enableCollidersWhenReady = false;
				}
			}
		}

		// Token: 0x06005D97 RID: 23959 RVA: 0x001DB05C File Offset: 0x001D925C
		private bool IsOverlapping(List<InteractionPoint> interactionPoints)
		{
			if (interactionPoints == null)
			{
				return false;
			}
			for (int i = 0; i < interactionPoints.Count; i++)
			{
				if (interactionPoints[i] == this.gripInteractor)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005D98 RID: 23960 RVA: 0x001DB096 File Offset: 0x001D9296
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x001DB09E File Offset: 0x001D929E
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06005D9A RID: 23962 RVA: 0x001DB0C6 File Offset: 0x001D92C6
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			this.parentItem = null;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			return true;
		}

		// Token: 0x06005D9B RID: 23963 RVA: 0x001DB101 File Offset: 0x001D9301
		public void OnHoverOverTableStart(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06005D9C RID: 23964 RVA: 0x001DB10A File Offset: 0x001D930A
		public void OnHoverOverTableEnd(BuilderTable table)
		{
			this.currTable = null;
		}

		// Token: 0x06005D9D RID: 23965 RVA: 0x001DB113 File Offset: 0x001D9313
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x001DB11C File Offset: 0x001D931C
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			base.transform.position = this.initialPosition;
			base.transform.rotation = this.initialRotation;
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transform.position = this.initialPosition;
				this.worldShareableInstance.transform.rotation = this.initialRotation;
			}
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06005D9F RID: 23967 RVA: 0x000D1F56 File Offset: 0x000D0156
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x06005DA0 RID: 23968 RVA: 0x001DB19E File Offset: 0x001D939E
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x06005DA1 RID: 23969 RVA: 0x001DB1D7 File Offset: 0x001D93D7
		private bool ShouldPlayFX()
		{
			return this.previousItemState == BuilderItem.BuilderItemState.isHeld || this.previousItemState == BuilderItem.BuilderItemState.dropped;
		}

		// Token: 0x06005DA2 RID: 23970 RVA: 0x001DB1EE File Offset: 0x001D93EE
		public static GameObject BuildEnvItem(int prefabHash, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(prefabHash, true);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x001DB20C File Offset: 0x001D940C
		protected override void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
		{
			if (leftHand)
			{
				this.reliableState.leftHandAttachPos = localPosition;
				this.reliableState.leftHandAttachRot = localRotation;
			}
			else
			{
				this.reliableState.rightHandAttachPos = localPosition;
				this.reliableState.rightHandAttachRot = localRotation;
			}
			this.reliableState.dirty = true;
		}

		// Token: 0x06005DA4 RID: 23972 RVA: 0x001DB25A File Offset: 0x001D945A
		public int GetPhotonViewId()
		{
			if (this.worldShareableInstance == null)
			{
				return -1;
			}
			return this.worldShareableInstance.ViewID;
		}

		// Token: 0x04006C21 RID: 27681
		public BuilderItemReliableState reliableState;

		// Token: 0x04006C22 RID: 27682
		public string builtItemPath;

		// Token: 0x04006C23 RID: 27683
		public GameObject itemRoot;

		// Token: 0x04006C24 RID: 27684
		private bool enableCollidersWhenReady;

		// Token: 0x04006C25 RID: 27685
		private float handsFreeOfCollidersTime;

		// Token: 0x04006C26 RID: 27686
		[NonSerialized]
		public BuilderPiece attachedPiece;

		// Token: 0x04006C27 RID: 27687
		public List<Behaviour> onlyWhenPlacedBehaviours;

		// Token: 0x04006C28 RID: 27688
		[NonSerialized]
		public BuilderItem parentItem;

		// Token: 0x04006C29 RID: 27689
		public List<BuilderAttachGridPlane> gridPlanes;

		// Token: 0x04006C2A RID: 27690
		public List<BuilderAttachEdge> edges;

		// Token: 0x04006C2B RID: 27691
		private List<Collider> colliders;

		// Token: 0x04006C2C RID: 27692
		private Transform parent;

		// Token: 0x04006C2D RID: 27693
		private Vector3 initialPosition;

		// Token: 0x04006C2E RID: 27694
		private Quaternion initialRotation;

		// Token: 0x04006C2F RID: 27695
		private Vector3 initialGrabInteractorScale;

		// Token: 0x04006C30 RID: 27696
		private BuilderTable currTable;

		// Token: 0x04006C31 RID: 27697
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006C32 RID: 27698
		public AudioClip snapAudio;

		// Token: 0x04006C33 RID: 27699
		public AudioClip placeAudio;

		// Token: 0x04006C34 RID: 27700
		public GameObject placeVFX;

		// Token: 0x04006C35 RID: 27701
		private new BuilderItem.BuilderItemState previousItemState = BuilderItem.BuilderItemState.dropped;

		// Token: 0x02000ED6 RID: 3798
		private enum BuilderItemState
		{
			// Token: 0x04006C37 RID: 27703
			isHeld = 1,
			// Token: 0x04006C38 RID: 27704
			dropped,
			// Token: 0x04006C39 RID: 27705
			placed = 4,
			// Token: 0x04006C3A RID: 27706
			unused0 = 8,
			// Token: 0x04006C3B RID: 27707
			none = 16
		}
	}
}
