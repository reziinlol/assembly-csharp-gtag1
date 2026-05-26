using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200062F RID: 1583
public class BuilderPiece : MonoBehaviour
{
	// Token: 0x0600274E RID: 10062 RVA: 0x000D045C File Offset: 0x000CE65C
	private void Awake()
	{
		if (this.fXInfo == null)
		{
			Debug.LogErrorFormat("BuilderPiece {0} is missing Effect Info", new object[]
			{
				base.gameObject.name
			});
		}
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.placedOnlyColliders = new List<Collider>(4);
		List<Collider> list = new List<Collider>(4);
		foreach (GameObject gameObject in this.onlyWhenPlaced)
		{
			list.Clear();
			gameObject.GetComponentsInChildren<Collider>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isTrigger)
				{
					BuilderPieceCollider builderPieceCollider = list[i].GetComponent<BuilderPieceCollider>();
					if (builderPieceCollider == null)
					{
						builderPieceCollider = list[i].AddComponent<BuilderPieceCollider>();
					}
					builderPieceCollider.piece = this;
					this.placedOnlyColliders.Add(list[i]);
				}
			}
		}
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		for (int j = this.colliders.Count - 1; j >= 0; j--)
		{
			if (this.colliders[j].isTrigger)
			{
				this.colliders.RemoveAt(j);
			}
			else
			{
				BuilderPieceCollider builderPieceCollider2 = this.colliders[j].GetComponent<BuilderPieceCollider>();
				if (builderPieceCollider2 == null)
				{
					builderPieceCollider2 = this.colliders[j].AddComponent<BuilderPieceCollider>();
				}
				builderPieceCollider2.piece = this;
			}
		}
		this.gridPlanes = new List<BuilderAttachGridPlane>(8);
		base.GetComponentsInChildren<BuilderAttachGridPlane>(this.gridPlanes);
		this.pieceComponents = new List<IBuilderPieceComponent>(1);
		base.GetComponentsInChildren<IBuilderPieceComponent>(true, this.pieceComponents);
		this.pieceComponentsActive = false;
		this.functionalPieceComponent = base.GetComponentInChildren<IBuilderPieceFunctional>(true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		this.renderingIndirect = new List<MeshRenderer>(4);
		this.renderingDirect = new List<MeshRenderer>(4);
		this.FindActiveRenderers();
		this.paintingCount = 0;
		this.potentialGrabCount = 0;
		this.potentialGrabChildCount = 0;
		this.isPrivatePlot = (this.plotComponent != null);
		this.privatePlotIndex = -1;
		this.ClearCollisionHistory();
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000D0718 File Offset: 0x000CE918
	public void SetTable(BuilderTable table)
	{
		this.tableOwner = table;
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000D0721 File Offset: 0x000CE921
	public BuilderTable GetTable()
	{
		return this.tableOwner;
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000D072C File Offset: 0x000CE92C
	public void OnReturnToPool()
	{
		this.tableOwner.builderRenderer.RemovePiece(this);
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceDestroy();
		}
		this.functionalPieceState = 0;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.overrideSavedPiece = false;
		this.savedMaterialType = -1;
		this.savedPieceType = -1;
		this.shelfOwner = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.activatedTimeStamp = 0;
		this.forcedFrozen = false;
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		base.transform.localScale = Vector3.one;
		if (this.isArmShelf)
		{
			if (this.armShelf != null)
			{
				this.armShelf.piece = null;
			}
			this.armShelf = null;
		}
		for (int j = 0; j < this.gridPlanes.Count; j++)
		{
			this.gridPlanes[j].OnReturnToPool(this.tableOwner.builderPool);
		}
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000D08AE File Offset: 0x000CEAAE
	public void OnCreatedByPool()
	{
		this.materialSwapTargets = new List<MeshRenderer>(4);
		base.GetComponentsInChildren<MeshRenderer>(this.areMeshesToggledOnPlace, this.materialSwapTargets);
		this.surfaceOverrides = new List<GorillaSurfaceOverride>(4);
		base.GetComponentsInChildren<GorillaSurfaceOverride>(this.areMeshesToggledOnPlace, this.surfaceOverrides);
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000D08EC File Offset: 0x000CEAEC
	public void SetupPiece(float gridSize)
	{
		for (int i = 0; i < this.gridPlanes.Count; i++)
		{
			this.gridPlanes[i].Setup(this, i, gridSize);
		}
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000D0924 File Offset: 0x000CEB24
	public void SetMaterial(int inMaterialType, bool force = false)
	{
		if (this.materialOptions == null || this.materialSwapTargets == null || this.materialSwapTargets.Count < 1)
		{
			return;
		}
		if (this.materialType == inMaterialType && !force)
		{
			return;
		}
		this.materialType = inMaterialType;
		Material material = null;
		int num = -1;
		if (inMaterialType == -1)
		{
			this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
		}
		else
		{
			this.materialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material == null)
			{
				this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
			}
		}
		if (material == null)
		{
			Debug.LogErrorFormat("Piece {0} has no material matching Type {1}", new object[]
			{
				this.GetPieceId(),
				inMaterialType
			});
			return;
		}
		foreach (MeshRenderer meshRenderer in this.materialSwapTargets)
		{
			if (!(meshRenderer == null) && meshRenderer.enabled)
			{
				meshRenderer.material = material;
			}
		}
		if (this.surfaceOverrides != null && num != -1)
		{
			foreach (GorillaSurfaceOverride gorillaSurfaceOverride in this.surfaceOverrides)
			{
				gorillaSurfaceOverride.overrideIndex = num;
			}
		}
		if (this.renderingIndirect.Count > 0)
		{
			this.tableOwner.builderRenderer.ChangePieceIndirectMaterial(this, this.materialSwapTargets, material);
		}
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000D0AB8 File Offset: 0x000CECB8
	public int GetPieceId()
	{
		return this.pieceId;
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000D0AC0 File Offset: 0x000CECC0
	public int GetParentPieceId()
	{
		if (!(this.parentPiece == null))
		{
			return this.parentPiece.pieceId;
		}
		return -1;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000D0ADD File Offset: 0x000CECDD
	public int GetAttachIndex()
	{
		return this.attachIndex;
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000D0AE5 File Offset: 0x000CECE5
	public int GetParentAttachIndex()
	{
		return this.parentAttachIndex;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000D0AF0 File Offset: 0x000CECF0
	private void SetPieceActive(List<IBuilderPieceComponent> components, bool active)
	{
		if (components == null || active == this.pieceComponentsActive)
		{
			return;
		}
		this.pieceComponentsActive = active;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				if (active)
				{
					components[i].OnPieceActivate();
				}
				else
				{
					components[i].OnPieceDeactivate();
				}
			}
		}
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000D0B48 File Offset: 0x000CED48
	private void SetBehavioursEnabled<T>(List<T> components, bool enabled) where T : Behaviour
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000D0B90 File Offset: 0x000CED90
	public void UpdateCollidersEnabled(bool _enabled)
	{
		this.SetCollidersEnabled<Collider>(this.colliders, _enabled);
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000D0BA0 File Offset: 0x000CEDA0
	private void SetCollidersEnabled<T>(List<T> components, bool enabled) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000D0BE8 File Offset: 0x000CEDE8
	public void SetColliderLayers<T>(List<T> components, int layer) where T : Collider
	{
		this.currentColliderLayer = layer;
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000D0C3C File Offset: 0x000CEE3C
	private void SetActive(List<GameObject> gameObjects, bool active)
	{
		if (gameObjects == null)
		{
			return;
		}
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] != null)
			{
				gameObjects[i].SetActive(active);
			}
		}
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000D0C7A File Offset: 0x000CEE7A
	public void SetFunctionalPieceState(byte fState, NetPlayer instigator, int timeStamp)
	{
		if (this.functionalPieceComponent == null || !this.functionalPieceComponent.IsStateValid(fState))
		{
			fState = 0;
		}
		this.functionalPieceState = fState;
		IBuilderPieceFunctional builderPieceFunctional = this.functionalPieceComponent;
		if (builderPieceFunctional == null)
		{
			return;
		}
		builderPieceFunctional.OnStateChanged(fState, instigator, timeStamp);
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000D0CAF File Offset: 0x000CEEAF
	public void SetScale(float scale)
	{
		if (this.scaleRoot != null)
		{
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		this.pieceScale = scale;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000D0CDC File Offset: 0x000CEEDC
	public float GetScale()
	{
		return this.pieceScale;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000D0CE4 File Offset: 0x000CEEE4
	public void PaintingTint(bool enable)
	{
		if (enable)
		{
			this.paintingCount++;
			if (this.paintingCount == 1)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.paintingCount--;
			if (this.paintingCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000D0D24 File Offset: 0x000CEF24
	public void PotentialGrab(bool enable)
	{
		if (enable)
		{
			this.potentialGrabCount++;
			if (this.potentialGrabCount == 1 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.potentialGrabCount--;
			if (this.potentialGrabCount == 0 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000D0D80 File Offset: 0x000CEF80
	public static void PotentialGrabChildren(BuilderPiece piece, bool enable)
	{
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			if (enable)
			{
				builderPiece.potentialGrabChildCount++;
				if (builderPiece.potentialGrabChildCount == 1 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			else
			{
				builderPiece.potentialGrabChildCount--;
				if (builderPiece.potentialGrabChildCount == 0 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			BuilderPiece.PotentialGrabChildren(builderPiece, enable);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000D0DFC File Offset: 0x000CEFFC
	private void RefreshTint()
	{
		if (this.potentialGrabCount > 0 || this.potentialGrabChildCount > 0)
		{
			this.SetTint(this.tableOwner.potentialGrabTint);
			return;
		}
		if (this.paintingCount > 0)
		{
			this.SetTint(this.tableOwner.paintingTint);
			return;
		}
		switch (this.state)
		{
		case BuilderPiece.State.AttachedToDropped:
		case BuilderPiece.State.Dropped:
			this.SetTint(this.tableOwner.droppedTint);
			return;
		case BuilderPiece.State.Grabbed:
		case BuilderPiece.State.GrabbedLocal:
		case BuilderPiece.State.AttachedToArm:
			this.SetTint(this.tableOwner.grabbedTint);
			return;
		case BuilderPiece.State.OnShelf:
		case BuilderPiece.State.OnConveyor:
			this.SetTint(this.tableOwner.shelfTint);
			return;
		}
		this.SetTint(this.tableOwner.defaultTint);
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000D0EC0 File Offset: 0x000CF0C0
	private void SetTint(float tint)
	{
		if (tint == this.tint)
		{
			return;
		}
		this.tint = tint;
		this.tableOwner.builderRenderer.SetPieceTint(this, tint);
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000D0EE8 File Offset: 0x000CF0E8
	public void SetParentPiece(int newAttachIndex, BuilderPiece newParentPiece, int newParentAttachIndex)
	{
		if (this.parentHeld != null)
		{
			Debug.LogErrorFormat(newParentPiece.gameObject, "Cannot attach to piece {0} while already held", new object[]
			{
				(newParentPiece == null) ? null : newParentPiece.gameObject.name
			});
			return;
		}
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = newAttachIndex;
		this.parentPiece = newParentPiece;
		this.parentAttachIndex = newParentAttachIndex;
		this.AddPieceToParent(this);
		Transform parent = null;
		if (newParentPiece != null)
		{
			if (newParentAttachIndex >= 0)
			{
				parent = newParentPiece.gridPlanes[newParentAttachIndex].transform;
			}
			else
			{
				parent = newParentPiece.transform;
			}
		}
		base.transform.SetParent(parent, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000D0FA0 File Offset: 0x000CF1A0
	public void ClearParentPiece(bool ignoreSnaps = false)
	{
		if (this.parentPiece == null)
		{
			if (!ignoreSnaps)
			{
				BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this, this.tableOwner.builderPool);
			}
			return;
		}
		BuilderPiece builderPiece = this.parentPiece;
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = -1;
		this.parentPiece = null;
		this.parentAttachIndex = -1;
		base.transform.SetParent(null, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
		if (!ignoreSnaps)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this.GetRootPiece(), this.tableOwner.builderPool);
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000D1030 File Offset: 0x000CF230
	public static void RemoveOverlapsWithDifferentPieceRoot(BuilderPiece piece, BuilderPiece root, BuilderPool pool)
	{
		for (int i = 0; i < piece.gridPlanes.Count; i++)
		{
			piece.gridPlanes[i].RemoveSnapsWithDifferentRoot(root, pool);
		}
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(builderPiece, root, pool);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000D1088 File Offset: 0x000CF288
	private void AddPieceToParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		this.nextSiblingPiece = builderPiece.firstChildPiece;
		builderPiece.firstChildPiece = piece;
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(1 + piece.GetChildCount());
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000D10F4 File Offset: 0x000CF2F4
	private static void RemovePieceFromParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		BuilderPiece builderPiece2 = builderPiece.firstChildPiece;
		if (builderPiece2 == null)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have any children", new object[]
			{
				builderPiece.name,
				piece.name
			});
		}
		bool flag = false;
		if (builderPiece2 == piece)
		{
			builderPiece.firstChildPiece = builderPiece2.nextSiblingPiece;
			flag = true;
		}
		else
		{
			while (builderPiece2 != null)
			{
				if (builderPiece2.nextSiblingPiece == piece)
				{
					builderPiece2.nextSiblingPiece = piece.nextSiblingPiece;
					piece.nextSiblingPiece = null;
					flag = true;
					break;
				}
				builderPiece2 = builderPiece2.nextSiblingPiece;
			}
		}
		if (!flag)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have the piece a child", new object[]
			{
				builderPiece.name,
				piece.name
			});
			return;
		}
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(-1 * (1 + piece.GetChildCount()));
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000D11F8 File Offset: 0x000CF3F8
	public void SetParentHeld(Transform parentHeld, int heldByPlayerActorNumber, bool heldInLeftHand)
	{
		if (this.parentPiece != null)
		{
			Debug.LogErrorFormat(this.parentPiece.gameObject, "Cannot hold while already attached to piece {0}", new object[]
			{
				this.parentPiece.gameObject.name
			});
			return;
		}
		this.heldByPlayerActorNumber = heldByPlayerActorNumber;
		this.parentHeld = parentHeld;
		this.heldInLeftHand = heldInLeftHand;
		base.transform.SetParent(parentHeld);
		this.tableOwner.UpdatePieceData(this);
		if (heldByPlayerActorNumber != -1)
		{
			this.OnGrabbedAsRoot();
			return;
		}
		this.OnReleasedAsRoot();
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000D1280 File Offset: 0x000CF480
	public void ClearParentHeld()
	{
		if (this.parentHeld == null)
		{
			return;
		}
		if (this.isArmShelf && this.armShelf != null)
		{
			this.armShelf.piece = null;
			this.armShelf = null;
		}
		this.heldByPlayerActorNumber = -1;
		this.parentHeld = null;
		this.heldInLeftHand = false;
		base.transform.SetParent(this.parentHeld);
		this.tableOwner.UpdatePieceData(this);
		this.OnReleasedAsRoot();
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000D12FD File Offset: 0x000CF4FD
	public bool IsHeldLocal()
	{
		return this.heldByPlayerActorNumber != -1 && this.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000D131C File Offset: 0x000CF51C
	public bool IsHeldBy(int actorNumber)
	{
		return actorNumber != -1 && this.heldByPlayerActorNumber == actorNumber;
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000D132D File Offset: 0x000CF52D
	public bool IsHeldInLeftHand()
	{
		return this.heldInLeftHand;
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000D1335 File Offset: 0x000CF535
	public static bool IsDroppedState(BuilderPiece.State state)
	{
		return state == BuilderPiece.State.Dropped || state == BuilderPiece.State.AttachedToDropped || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.OnConveyor;
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000D134C File Offset: 0x000CF54C
	public void SetActivateTimeStamp(int timeStamp)
	{
		this.activatedTimeStamp = timeStamp;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetActivateTimeStamp(timeStamp);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000D1380 File Offset: 0x000CF580
	public void SetState(BuilderPiece.State newState, bool force = false)
	{
		if (newState == this.state && !force)
		{
			if (newState == BuilderPiece.State.Grabbed)
			{
				int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
				if (this.currentColliderLayer != expectedGrabCollisionLayer)
				{
					this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
					this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
				}
			}
			return;
		}
		if (newState == BuilderPiece.State.Dropped && this.state != BuilderPiece.State.Dropped)
		{
			this.tableOwner.AddPieceToDropList(this);
		}
		else if (this.state == BuilderPiece.State.Dropped && newState != BuilderPiece.State.Dropped)
		{
			this.tableOwner.RemovePieceFromDropList(this);
		}
		BuilderPiece.State state = this.state;
		this.state = newState;
		if (this.pieceDataIndex >= 0)
		{
			this.tableOwner.UpdatePieceData(this);
		}
		switch (this.state)
		{
		case BuilderPiece.State.None:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, false);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.None, force);
			this.tableOwner.builderRenderer.RemovePiece(this);
			this.isStatic = true;
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedAndPlaced:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, true);
			this.SetActive(this.onlyWhenPlaced, true);
			this.SetActive(this.onlyWhenNotPlaced, false);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.placedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedAndPlaced, force);
			this.SetStatic(false, force || this.areMeshesToggledOnPlace);
			this.SetPieceActive(this.pieceComponents, true);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToDropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Grabbed:
		{
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			int expectedGrabCollisionLayer2 = this.GetExpectedGrabCollisionLayer();
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer2);
			this.SetChildrenState(BuilderPiece.State.Grabbed, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		}
		case BuilderPiece.State.Dropped:
			this.ClearCollisionHistory();
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(false, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnShelf:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnShelf, force);
			this.SetStatic(true, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Displayed:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetChildrenState(BuilderPiece.State.Displayed, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.GrabbedLocal:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.GrabbedLocal, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			this.forcedFrozen = false;
			return;
		case BuilderPiece.State.OnConveyor:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnConveyor, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToArm:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.AttachedToArm, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000D1914 File Offset: 0x000CFB14
	public void OnGrabbedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.heldByPlayerActorNumber != NetworkSystem.Instance.LocalPlayer.ActorNumber && !this.listeningToHandLinks)
		{
			TakeMyHand_HandLink.OnHandLinkChanged = (Action)Delegate.Combine(TakeMyHand_HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = true;
		}
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000D1970 File Offset: 0x000CFB70
	public void OnReleasedAsRoot()
	{
		if (this.isArmShelf)
		{
			return;
		}
		if (this.listeningToHandLinks)
		{
			TakeMyHand_HandLink.OnHandLinkChanged = (Action)Delegate.Remove(TakeMyHand_HandLink.OnHandLinkChanged, new Action(this.UpdateGrabbedPieceCollisionLayer));
			this.listeningToHandLinks = false;
		}
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000D19AC File Offset: 0x000CFBAC
	public void SetKinematic(bool kinematic, bool destroyImmediate = true)
	{
		if (kinematic && this.rigidBody != null)
		{
			if (destroyImmediate)
			{
				Object.DestroyImmediate(this.rigidBody);
				this.rigidBody = null;
			}
			else
			{
				Object.Destroy(this.rigidBody);
				this.rigidBody = null;
			}
		}
		else if (!kinematic && this.rigidBody == null)
		{
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			if (this.rigidBody != null)
			{
				Debug.LogErrorFormat("We should never already have a rigid body here {0} {1}", new object[]
				{
					this.pieceId,
					this.pieceType
				});
			}
			if (this.rigidBody == null)
			{
				this.rigidBody = base.gameObject.AddComponent<Rigidbody>();
			}
			if (this.rigidBody != null)
			{
				this.rigidBody.isKinematic = kinematic;
			}
		}
		if (this.rigidBody != null)
		{
			this.rigidBody.mass = 1f;
		}
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000D1AB4 File Offset: 0x000CFCB4
	public void ClearCollisionHistory()
	{
		if (this.collisionEnterHistory == null)
		{
			this.collisionEnterHistory = new float[this.collisionEnterLimit];
		}
		for (int i = 0; i < this.collisionEnterLimit; i++)
		{
			this.collisionEnterHistory[i] = float.MinValue;
		}
		this.collidersEntered.Clear();
		this.oldCollisionTimeIndex = 0;
		this.forcedFrozen = false;
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000D1B14 File Offset: 0x000CFD14
	private void OnCollisionEnter(Collision other)
	{
		if (this.state != BuilderPiece.State.Dropped || this.forcedFrozen)
		{
			return;
		}
		BuilderPieceCollider component = other.collider.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if ((piece.state == BuilderPiece.State.AttachedAndPlaced || piece.forcedFrozen) && !this.collidersEntered.Add(other.collider.GetInstanceID()))
			{
				if (this.collisionEnterHistory[this.oldCollisionTimeIndex] > Time.time)
				{
					this.tableOwner.FreezeDroppedPiece(this);
					return;
				}
				this.collisionEnterHistory[this.oldCollisionTimeIndex] = Time.time + this.collisionEnterCooldown;
				int num = this.oldCollisionTimeIndex + 1;
				this.oldCollisionTimeIndex = num;
				this.oldCollisionTimeIndex = num % this.collisionEnterLimit;
			}
		}
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000D1BD4 File Offset: 0x000CFDD4
	public int GetExpectedGrabCollisionLayer()
	{
		if (this.heldByPlayerActorNumber != -1)
		{
			if (!GorillaTagger.Instance.offlineVRRig.IsInHandHoldChainWithOtherPlayer(this.heldByPlayerActorNumber))
			{
				return BuilderTable.heldLayer;
			}
			return BuilderTable.heldLayerLocal;
		}
		else
		{
			if (this.parentPiece != null)
			{
				return this.parentPiece.currentColliderLayer;
			}
			return BuilderTable.heldLayer;
		}
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000D1C2C File Offset: 0x000CFE2C
	public void UpdateGrabbedPieceCollisionLayer()
	{
		int expectedGrabCollisionLayer = this.GetExpectedGrabCollisionLayer();
		if (this.currentColliderLayer != expectedGrabCollisionLayer)
		{
			this.SetColliderLayers<Collider>(this.colliders, expectedGrabCollisionLayer);
			this.SetChildrenCollisionLayer(expectedGrabCollisionLayer);
		}
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000D1C60 File Offset: 0x000CFE60
	private void SetChildrenCollisionLayer(int layer)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetColliderLayers<Collider>(builderPiece.colliders, layer);
			builderPiece.SetChildrenCollisionLayer(layer);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000D1C9C File Offset: 0x000CFE9C
	public void SetStatic(bool isStatic, bool force = false)
	{
		isStatic = true;
		if (this.isStatic == isStatic && !force)
		{
			return;
		}
		this.SetDirectRenderersVisible(true);
		this.tableOwner.builderRenderer.RemovePiece(this);
		this.isStatic = isStatic;
		if (this.areMeshesToggledOnPlace)
		{
			this.FindActiveRenderers();
		}
		this.tableOwner.builderRenderer.AddPiece(this);
		this.SetDirectRenderersVisible(this.tableOwner.IsInBuilderZone());
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000D1D08 File Offset: 0x000CFF08
	private void FindActiveRenderers()
	{
		if (this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = true;
			}
		}
		this.renderingDirect.Clear();
		BuilderPiece.tempRenderers.Clear();
		base.GetComponentsInChildren<MeshRenderer>(false, BuilderPiece.tempRenderers);
		foreach (MeshRenderer meshRenderer2 in BuilderPiece.tempRenderers)
		{
			if (meshRenderer2.enabled)
			{
				this.renderingDirect.Add(meshRenderer2);
			}
		}
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000D1DD8 File Offset: 0x000CFFD8
	public void SetDirectRenderersVisible(bool visible)
	{
		if (this.renderingDirect != null && this.renderingDirect.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.renderingDirect)
			{
				meshRenderer.enabled = visible;
			}
		}
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000D1E40 File Offset: 0x000D0040
	private void SetChildrenState(BuilderPiece.State newState, bool force)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetState(newState, force);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000D1E70 File Offset: 0x000D0070
	public void OnCreate()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceCreate(this.pieceType, this.pieceId);
		}
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000D1EB0 File Offset: 0x000D00B0
	public void OnPlacementDeserialized()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPiecePlacementDeserialized();
		}
	}

	// Token: 0x06002782 RID: 10114 RVA: 0x000D1EE4 File Offset: 0x000D00E4
	public void PlayPlacementFx()
	{
		this.PlayFX(this.fXInfo.placeVFX);
	}

	// Token: 0x06002783 RID: 10115 RVA: 0x000D1EF7 File Offset: 0x000D00F7
	public void PlayDisconnectFx()
	{
		this.PlayFX(this.fXInfo.disconnectVFX);
	}

	// Token: 0x06002784 RID: 10116 RVA: 0x000D1F0A File Offset: 0x000D010A
	public void PlayGrabbedFx()
	{
		this.PlayFX(this.fXInfo.grabbedVFX);
	}

	// Token: 0x06002785 RID: 10117 RVA: 0x000D1F1D File Offset: 0x000D011D
	public void PlayTooHeavyFx()
	{
		this.PlayFX(this.fXInfo.tooHeavyVFX);
	}

	// Token: 0x06002786 RID: 10118 RVA: 0x000D1F30 File Offset: 0x000D0130
	public void PlayLocationLockFx()
	{
		this.PlayFX(this.fXInfo.locationLockVFX);
	}

	// Token: 0x06002787 RID: 10119 RVA: 0x000D1F43 File Offset: 0x000D0143
	public void PlayRecycleFx()
	{
		this.PlayFX(this.fXInfo.recycleVFX);
	}

	// Token: 0x06002788 RID: 10120 RVA: 0x000D1F56 File Offset: 0x000D0156
	private void PlayFX(GameObject fx)
	{
		ObjectPools.instance.Instantiate(fx, base.transform.position, true);
	}

	// Token: 0x06002789 RID: 10121 RVA: 0x000D1F70 File Offset: 0x000D0170
	public static BuilderPiece GetBuilderPieceFromCollider(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		BuilderPieceCollider component = collider.GetComponent<BuilderPieceCollider>();
		if (!(component == null))
		{
			return component.piece;
		}
		return null;
	}

	// Token: 0x0600278A RID: 10122 RVA: 0x000D1FA0 File Offset: 0x000D01A0
	public static BuilderPiece GetBuilderPieceFromTransform(Transform transform)
	{
		while (transform != null)
		{
			BuilderPiece component = transform.GetComponent<BuilderPiece>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0600278B RID: 10123 RVA: 0x000D1FD4 File Offset: 0x000D01D4
	public static void MakePieceRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (piece.parentPiece == null || piece.parentPiece.isBuiltIntoTable)
		{
			return;
		}
		BuilderPiece.MakePieceRoot(piece.parentPiece);
		int newAttachIndex = piece.parentAttachIndex;
		int newParentAttachIndex = piece.attachIndex;
		BuilderPiece builderPiece = piece.parentPiece;
		bool ignoreSnaps = true;
		piece.ClearParentPiece(ignoreSnaps);
		builderPiece.SetParentPiece(newAttachIndex, piece, newParentAttachIndex);
	}

	// Token: 0x0600278C RID: 10124 RVA: 0x000D2038 File Offset: 0x000D0238
	public BuilderPiece GetRootPiece()
	{
		BuilderPiece builderPiece = this;
		while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
		{
			builderPiece = builderPiece.parentPiece;
		}
		return builderPiece;
	}

	// Token: 0x0600278D RID: 10125 RVA: 0x000D206C File Offset: 0x000D026C
	public bool IsPrivatePlot()
	{
		return this.isPrivatePlot;
	}

	// Token: 0x0600278E RID: 10126 RVA: 0x000D2074 File Offset: 0x000D0274
	public bool TryGetPlotComponent(out BuilderPiecePrivatePlot plot)
	{
		plot = this.plotComponent;
		return this.isPrivatePlot;
	}

	// Token: 0x0600278F RID: 10127 RVA: 0x000D208C File Offset: 0x000D028C
	public static bool CanPlayerAttachPieceToPiece(int playerActorNumber, BuilderPiece attachingPiece, BuilderPiece attachToPiece)
	{
		if (attachToPiece.state != BuilderPiece.State.AttachedAndPlaced && !attachToPiece.IsPrivatePlot() && attachToPiece.state != BuilderPiece.State.AttachedToArm)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = attachToPiece.GetAttachedBuiltInPiece();
		if (attachedBuiltInPiece == null || (!attachedBuiltInPiece.isPrivatePlot && !attachedBuiltInPiece.isArmShelf))
		{
			return true;
		}
		if (attachedBuiltInPiece.isArmShelf)
		{
			return attachedBuiltInPiece.heldByPlayerActorNumber == playerActorNumber && attachedBuiltInPiece.armShelf != null && attachedBuiltInPiece.armShelf.CanAttachToArmPiece();
		}
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || (builderPiecePrivatePlot.CanPlayerAttachToPlot(playerActorNumber) && builderPiecePrivatePlot.IsChainUnderCapacity(attachingPiece));
	}

	// Token: 0x06002790 RID: 10128 RVA: 0x000D2124 File Offset: 0x000D0324
	public bool CanPlayerGrabPiece(int actorNumber, Vector3 worldPosition)
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced && !this.isPrivatePlot)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = this.GetAttachedBuiltInPiece();
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return attachedBuiltInPiece == null || !attachedBuiltInPiece.isPrivatePlot || !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || builderPiecePrivatePlot.CanPlayerGrabFromPlot(actorNumber, worldPosition) || this.tableOwner.IsLocationWithinSharedBuildArea(worldPosition);
	}

	// Token: 0x06002791 RID: 10129 RVA: 0x000D2184 File Offset: 0x000D0384
	public bool IsPieceMoving()
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return false;
		}
		if (this.attachPlayerToPiece)
		{
			return true;
		}
		if (this.attachIndex < 0 || this.attachIndex >= this.gridPlanes.Count)
		{
			return false;
		}
		if (this.gridPlanes[this.attachIndex].IsAttachedToMovingGrid())
		{
			return true;
		}
		using (List<BuilderAttachGridPlane>.Enumerator enumerator = this.gridPlanes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isMoving)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000D222C File Offset: 0x000D042C
	public BuilderPiece GetAttachedBuiltInPiece()
	{
		if (this.isBuiltIntoTable)
		{
			return this;
		}
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return null;
		}
		BuilderPiece rootPiece = this.GetRootPiece();
		if (rootPiece.parentPiece != null)
		{
			rootPiece = rootPiece.parentPiece;
		}
		if (rootPiece.isBuiltIntoTable)
		{
			return rootPiece;
		}
		return null;
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000D2274 File Offset: 0x000D0474
	public int GetChainCostAndCount(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		return 1 + this.GetChildCountAndCost(costArray);
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000D2308 File Offset: 0x000D0508
	public int GetChildCountAndCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			num += builderPiece.GetChildCountAndCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
		return num;
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000D23AC File Offset: 0x000D05AC
	public int GetChildCount()
	{
		int num = 0;
		foreach (BuilderAttachGridPlane builderAttachGridPlane in this.gridPlanes)
		{
			num += builderAttachGridPlane.GetChildCount();
		}
		return num;
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000D2404 File Offset: 0x000D0604
	public void GetChainCost(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		this.AddChildCost(costArray);
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000D2498 File Offset: 0x000D0698
	public void AddChildCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			builderPiece.AddChildCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000D2538 File Offset: 0x000D0738
	public void BumpTwistToPositionRotation(byte twist, sbyte xOffset, sbyte zOffset, int potentialAttachIndex, BuilderAttachGridPlane potentialParentGridPlane, out Vector3 localPosition, out Quaternion localRotation, out Vector3 worldPosition, out Quaternion worldRotation)
	{
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		Transform center = potentialParentGridPlane.center;
		Vector3 position = center.position;
		Quaternion rotation = center.rotation;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - potentialParentGridPlane.widthOffset;
		float num4 = num2 - potentialParentGridPlane.lengthOffset;
		Quaternion quaternion = Quaternion.Euler(0f, (float)twist * 90f, 0f);
		Quaternion lhs = rotation * quaternion;
		float x = (float)xOffset * gridSize + num3;
		float z = (float)zOffset * gridSize + num4;
		Vector3 point = new Vector3(x, 0f, z);
		Vector3 a = position + rotation * point;
		Transform center2 = builderAttachGridPlane.center;
		Quaternion quaternion2 = lhs * Quaternion.Inverse(center2.localRotation);
		Vector3 point2 = base.transform.InverseTransformPoint(center2.position);
		Vector3 vector = a - quaternion2 * point2;
		localPosition = potentialParentGridPlane.transform.InverseTransformPoint(vector);
		localRotation = quaternion * Quaternion.Inverse(center2.localRotation);
		worldPosition = vector;
		worldRotation = quaternion2;
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000D268C File Offset: 0x000D088C
	public Quaternion TwistToLocalRotation(byte twist, int potentialAttachIndex)
	{
		float y = 90f * (float)twist;
		Quaternion quaternion = Quaternion.Euler(0f, y, 0f);
		if (potentialAttachIndex < 0 || potentialAttachIndex >= this.gridPlanes.Count)
		{
			return quaternion;
		}
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		Transform transform = (builderAttachGridPlane.center != null) ? builderAttachGridPlane.center : builderAttachGridPlane.transform;
		return quaternion * Quaternion.Inverse(transform.localRotation);
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000D2704 File Offset: 0x000D0904
	public int GetPiecePlacement()
	{
		byte pieceTwist = this.GetPieceTwist();
		sbyte xOffset;
		sbyte zOffset;
		this.GetPieceBumpOffset(pieceTwist, out xOffset, out zOffset);
		return BuilderTable.PackPiecePlacement(pieceTwist, xOffset, zOffset);
	}

	// Token: 0x0600279B RID: 10139 RVA: 0x000D272C File Offset: 0x000D092C
	public byte GetPieceTwist()
	{
		if (this.attachIndex == -1)
		{
			return 0;
		}
		Quaternion localRotation = base.transform.localRotation;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		Quaternion rotation = localRotation * builderAttachGridPlane.transform.localRotation;
		float num = 0.866f;
		Vector3 lhs = rotation * Vector3.forward;
		float num2 = Vector3.Dot(lhs, Vector3.forward);
		float num3 = Vector3.Dot(lhs, Vector3.right);
		bool flag = Mathf.Abs(num2) > num;
		bool flag2 = Mathf.Abs(num3) > num;
		if (!flag && !flag2)
		{
			return 0;
		}
		uint num4;
		if (flag)
		{
			num4 = ((num2 > 0f) ? 0U : 2U);
		}
		else
		{
			num4 = ((num3 > 0f) ? 1U : 3U);
		}
		return (byte)num4;
	}

	// Token: 0x0600279C RID: 10140 RVA: 0x000D27E0 File Offset: 0x000D09E0
	public void GetPieceBumpOffset(byte twist, out sbyte xOffset, out sbyte zOffset)
	{
		if (this.attachIndex == -1 || this.parentPiece == null)
		{
			xOffset = 0;
			zOffset = 0;
			return;
		}
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		BuilderAttachGridPlane builderAttachGridPlane2 = this.parentPiece.gridPlanes[this.parentAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		float num = flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset;
		float num2 = flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset;
		float num3 = num - builderAttachGridPlane2.widthOffset;
		float num4 = num2 - builderAttachGridPlane2.lengthOffset;
		Vector3 position = builderAttachGridPlane.center.position;
		Vector3 position2 = builderAttachGridPlane2.center.position;
		Vector3 vector = Quaternion.Inverse(builderAttachGridPlane2.center.rotation) * (position - position2);
		xOffset = (sbyte)Mathf.RoundToInt((vector.x - num3) / gridSize);
		zOffset = (sbyte)Mathf.RoundToInt((vector.z - num4) / gridSize);
	}

	// Token: 0x040032E8 RID: 13032
	public const int INVALID = -1;

	// Token: 0x040032E9 RID: 13033
	public const float LIGHT_MASS = 1f;

	// Token: 0x040032EA RID: 13034
	public const float HEAVY_MASS = 10000f;

	// Token: 0x040032EB RID: 13035
	[Tooltip("Name for debug text")]
	public string displayName;

	// Token: 0x040032EC RID: 13036
	[Tooltip("(Optional) scriptable object containing material swaps")]
	public BuilderMaterialOptions materialOptions;

	// Token: 0x040032ED RID: 13037
	[Tooltip("Builder Resources used by this object\nbuilderRscBasic for simple meshes\nbuilderRscDecorative for detailed meshes\nbuilderRscFunctional for extra scripts or effects")]
	public BuilderResources cost;

	// Token: 0x040032EE RID: 13038
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfOffset = Vector3.zero;

	// Token: 0x040032EF RID: 13039
	[Tooltip("Spawn Offset")]
	public Vector3 desiredShelfRotationOffset = Vector3.zero;

	// Token: 0x040032F0 RID: 13040
	[FormerlySerializedAs("vFXInfo")]
	[Tooltip("sounds for block actions. everything uses BuilderPieceEffectInfo_Default")]
	[SerializeField]
	private BuilderPieceEffectInfo fXInfo;

	// Token: 0x040032F1 RID: 13041
	private List<MeshRenderer> materialSwapTargets;

	// Token: 0x040032F2 RID: 13042
	private List<GorillaSurfaceOverride> surfaceOverrides;

	// Token: 0x040032F3 RID: 13043
	[Tooltip("parent object of everything scaled with the piece")]
	public Transform scaleRoot;

	// Token: 0x040032F4 RID: 13044
	[Tooltip("Is the block part of the room / immovable (used for the base terrain)")]
	public bool isBuiltIntoTable;

	// Token: 0x040032F5 RID: 13045
	public bool isArmShelf;

	// Token: 0x040032F6 RID: 13046
	[HideInInspector]
	public BuilderArmShelf armShelf;

	// Token: 0x040032F7 RID: 13047
	[Tooltip("Used to prevent log warnings from materials incompatible with the builder renderer\nAnything that needs text/transparency/or particles uses the normal rendering pipeline")]
	public bool suppressMaterialWarnings;

	// Token: 0x040032F8 RID: 13048
	[Tooltip("Only used by private plots")]
	private bool isPrivatePlot;

	// Token: 0x040032F9 RID: 13049
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x040032FA RID: 13050
	[Tooltip("Only used by private plots")]
	public BuilderPiecePrivatePlot plotComponent;

	// Token: 0x040032FB RID: 13051
	[Tooltip("Add piece movement to player movement when touched")]
	public bool attachPlayerToPiece;

	// Token: 0x040032FC RID: 13052
	public int pieceType;

	// Token: 0x040032FD RID: 13053
	public int pieceId;

	// Token: 0x040032FE RID: 13054
	public int pieceDataIndex;

	// Token: 0x040032FF RID: 13055
	public int materialType = -1;

	// Token: 0x04003300 RID: 13056
	public int heldByPlayerActorNumber;

	// Token: 0x04003301 RID: 13057
	public bool heldInLeftHand;

	// Token: 0x04003302 RID: 13058
	public Transform parentHeld;

	// Token: 0x04003303 RID: 13059
	[HideInInspector]
	public BuilderPiece parentPiece;

	// Token: 0x04003304 RID: 13060
	[HideInInspector]
	public BuilderPiece firstChildPiece;

	// Token: 0x04003305 RID: 13061
	[HideInInspector]
	public BuilderPiece nextSiblingPiece;

	// Token: 0x04003306 RID: 13062
	[HideInInspector]
	public int attachIndex;

	// Token: 0x04003307 RID: 13063
	[HideInInspector]
	public int parentAttachIndex;

	// Token: 0x04003308 RID: 13064
	public int shelfOwner = -1;

	// Token: 0x04003309 RID: 13065
	[HideInInspector]
	public List<BuilderAttachGridPlane> gridPlanes;

	// Token: 0x0400330A RID: 13066
	[HideInInspector]
	public List<Collider> colliders;

	// Token: 0x0400330B RID: 13067
	public List<Collider> placedOnlyColliders;

	// Token: 0x0400330C RID: 13068
	private int currentColliderLayer = BuilderTable.droppedLayer;

	// Token: 0x0400330D RID: 13069
	[Tooltip("Components enabled when the block is snapped to the build table")]
	public List<Behaviour> onlyWhenPlacedBehaviours;

	// Token: 0x0400330E RID: 13070
	[Tooltip("Game objects enabled when the block is snapped to the build table\nAny concave collision should be here")]
	public List<GameObject> onlyWhenPlaced;

	// Token: 0x0400330F RID: 13071
	[Tooltip("Game objects enabled when the block is not snapped to the build table\n Convex collision should be here if there is concave collision when placed")]
	public List<GameObject> onlyWhenNotPlaced;

	// Token: 0x04003310 RID: 13072
	public List<IBuilderPieceComponent> pieceComponents;

	// Token: 0x04003311 RID: 13073
	public IBuilderPieceFunctional functionalPieceComponent;

	// Token: 0x04003312 RID: 13074
	public byte functionalPieceState;

	// Token: 0x04003313 RID: 13075
	public List<IBuilderPieceFunctional> pieceFunctionComponents;

	// Token: 0x04003314 RID: 13076
	private bool pieceComponentsActive;

	// Token: 0x04003315 RID: 13077
	[Tooltip("Check if any renderers are in the onlyWhenPlaced or onlyWhenNotPlaced lists")]
	public bool areMeshesToggledOnPlace;

	// Token: 0x04003316 RID: 13078
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x04003317 RID: 13079
	[NonSerialized]
	public int activatedTimeStamp;

	// Token: 0x04003318 RID: 13080
	[HideInInspector]
	public int preventSnapUntilMoved;

	// Token: 0x04003319 RID: 13081
	[HideInInspector]
	public Vector3 preventSnapUntilMovedFromPos;

	// Token: 0x0400331A RID: 13082
	[HideInInspector]
	public BuilderPiece requestedParentPiece;

	// Token: 0x0400331B RID: 13083
	private BuilderTable tableOwner;

	// Token: 0x0400331C RID: 13084
	public PieceFallbackInfo fallbackInfo;

	// Token: 0x0400331D RID: 13085
	[NonSerialized]
	public bool overrideSavedPiece;

	// Token: 0x0400331E RID: 13086
	[NonSerialized]
	public int savedPieceType = -1;

	// Token: 0x0400331F RID: 13087
	[NonSerialized]
	public int savedMaterialType = -1;

	// Token: 0x04003320 RID: 13088
	private float pieceScale;

	// Token: 0x04003321 RID: 13089
	private float[] collisionEnterHistory;

	// Token: 0x04003322 RID: 13090
	private int collisionEnterLimit = 10;

	// Token: 0x04003323 RID: 13091
	private float collisionEnterCooldown = 2f;

	// Token: 0x04003324 RID: 13092
	private int oldCollisionTimeIndex;

	// Token: 0x04003325 RID: 13093
	[HideInInspector]
	public BuilderPiece.State state;

	// Token: 0x04003326 RID: 13094
	[HideInInspector]
	public bool isStatic;

	// Token: 0x04003327 RID: 13095
	[NonSerialized]
	private bool listeningToHandLinks;

	// Token: 0x04003328 RID: 13096
	[HideInInspector]
	public List<MeshRenderer> renderingDirect;

	// Token: 0x04003329 RID: 13097
	[HideInInspector]
	public List<MeshRenderer> renderingIndirect;

	// Token: 0x0400332A RID: 13098
	[HideInInspector]
	public List<int> renderingIndirectTransformIndex;

	// Token: 0x0400332B RID: 13099
	[HideInInspector]
	public float tint;

	// Token: 0x0400332C RID: 13100
	private int paintingCount;

	// Token: 0x0400332D RID: 13101
	private int potentialGrabCount;

	// Token: 0x0400332E RID: 13102
	private int potentialGrabChildCount;

	// Token: 0x0400332F RID: 13103
	internal bool forcedFrozen;

	// Token: 0x04003330 RID: 13104
	private HashSet<int> collidersEntered = new HashSet<int>(128);

	// Token: 0x04003331 RID: 13105
	private static List<MeshRenderer> tempRenderers = new List<MeshRenderer>(48);

	// Token: 0x02000630 RID: 1584
	public enum State
	{
		// Token: 0x04003333 RID: 13107
		None = -1,
		// Token: 0x04003334 RID: 13108
		AttachedAndPlaced,
		// Token: 0x04003335 RID: 13109
		AttachedToDropped,
		// Token: 0x04003336 RID: 13110
		Grabbed,
		// Token: 0x04003337 RID: 13111
		Dropped,
		// Token: 0x04003338 RID: 13112
		OnShelf,
		// Token: 0x04003339 RID: 13113
		Displayed,
		// Token: 0x0400333A RID: 13114
		GrabbedLocal,
		// Token: 0x0400333B RID: 13115
		OnConveyor,
		// Token: 0x0400333C RID: 13116
		AttachedToArm
	}
}
