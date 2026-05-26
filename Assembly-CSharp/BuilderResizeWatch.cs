using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000643 RID: 1603
public class BuilderResizeWatch : MonoBehaviour
{
	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002801 RID: 10241 RVA: 0x000D840C File Offset: 0x000D660C
	public int SizeLayerMaskGrow
	{
		get
		{
			int num = 0;
			if (this.growSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.growSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.growSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.growSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x06002802 RID: 10242 RVA: 0x000D8460 File Offset: 0x000D6660
	public int SizeLayerMaskShrink
	{
		get
		{
			int num = 0;
			if (this.shrinkSettings.affectLayerA)
			{
				num |= 1;
			}
			if (this.shrinkSettings.affectLayerB)
			{
				num |= 2;
			}
			if (this.shrinkSettings.affectLayerC)
			{
				num |= 4;
			}
			if (this.shrinkSettings.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x06002803 RID: 10243 RVA: 0x000D84B4 File Offset: 0x000D66B4
	private void Start()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.AddListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.AddListener(new UnityAction(this.OnShrinkButtonPressed));
		}
		this.ownerRig = base.GetComponentInParent<VRRig>();
		this.enableDist = GTPlayer.Instance.bodyCollider.height;
		this.enableDistSq = this.enableDist * this.enableDist;
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000D854C File Offset: 0x000D674C
	private void OnDestroy()
	{
		if (this.enlargeButton != null)
		{
			this.enlargeButton.onPressButton.RemoveListener(new UnityAction(this.OnEnlargeButtonPressed));
		}
		if (this.shrinkButton != null)
		{
			this.shrinkButton.onPressButton.RemoveListener(new UnityAction(this.OnShrinkButtonPressed));
		}
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000D85B0 File Offset: 0x000D67B0
	private void OnEnlargeButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
				return;
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskGrow && !this.updateCollision)
		{
			this.DisableCollisionWithPieces();
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskGrow;
			if (this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, this.ownerRig.transform.position, true);
			}
			this.timeToCheckCollision = (double)(Time.time + this.growDelay);
			this.updateCollision = true;
		}
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x000D8680 File Offset: 0x000D6880
	private void DisableCollisionWithPieces()
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(this.ownerRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		int num = Physics.OverlapSphereNonAlloc(GTPlayer.Instance.headCollider.transform.position, 1f, this.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable && !this.collisionDisabledPieces.Contains(builderPieceFromCollider))
			{
				foreach (Collider collider in builderPieceFromCollider.colliders)
				{
					collider.enabled = false;
				}
				foreach (Collider collider2 in builderPieceFromCollider.placedOnlyColliders)
				{
					collider2.enabled = false;
				}
				this.collisionDisabledPieces.Add(builderPieceFromCollider);
			}
		}
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000D87BC File Offset: 0x000D69BC
	private void EnableCollisionWithPieces()
	{
		for (int i = this.collisionDisabledPieces.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.collisionDisabledPieces[i];
			if (builderPiece == null)
			{
				this.collisionDisabledPieces.RemoveAt(i);
			}
			else if (Vector3.SqrMagnitude(GTPlayer.Instance.bodyCollider.transform.position - builderPiece.transform.position) >= this.enableDistSq)
			{
				this.EnableCollisionWithPiece(builderPiece);
				this.collisionDisabledPieces.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002808 RID: 10248 RVA: 0x000D884C File Offset: 0x000D6A4C
	private void EnableCollisionWithPiece(BuilderPiece piece)
	{
		foreach (Collider collider in piece.colliders)
		{
			collider.enabled = (piece.state != BuilderPiece.State.None && piece.state != BuilderPiece.State.Displayed);
		}
		foreach (Collider collider2 in piece.placedOnlyColliders)
		{
			collider2.enabled = (piece.state == BuilderPiece.State.AttachedAndPlaced);
		}
	}

	// Token: 0x06002809 RID: 10249 RVA: 0x000D88FC File Offset: 0x000D6AFC
	private void Update()
	{
		if (this.updateCollision && (double)Time.time >= this.timeToCheckCollision)
		{
			this.EnableCollisionWithPieces();
			if (this.collisionDisabledPieces.Count <= 0)
			{
				this.updateCollision = false;
			}
		}
	}

	// Token: 0x0600280A RID: 10250 RVA: 0x000D8930 File Offset: 0x000D6B30
	private void OnShrinkButtonPressed()
	{
		if (this.sizeManager == null)
		{
			if (this.ownerRig == null)
			{
				Debug.LogWarning("Builder resize watch has no owner rig");
			}
			this.sizeManager = this.ownerRig.sizeManager;
		}
		if (this.sizeManager != null && this.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMaskShrink)
		{
			this.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMaskShrink;
		}
	}

	// Token: 0x0400341C RID: 13340
	[SerializeField]
	private HeldButton enlargeButton;

	// Token: 0x0400341D RID: 13341
	[SerializeField]
	private HeldButton shrinkButton;

	// Token: 0x0400341E RID: 13342
	[SerializeField]
	private GameObject fxForLayerChange;

	// Token: 0x0400341F RID: 13343
	private VRRig ownerRig;

	// Token: 0x04003420 RID: 13344
	private SizeManager sizeManager;

	// Token: 0x04003421 RID: 13345
	[HideInInspector]
	public Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x04003422 RID: 13346
	[HideInInspector]
	public List<BuilderPiece> collisionDisabledPieces = new List<BuilderPiece>();

	// Token: 0x04003423 RID: 13347
	private float enableDist = 1f;

	// Token: 0x04003424 RID: 13348
	private float enableDistSq = 1f;

	// Token: 0x04003425 RID: 13349
	private bool updateCollision;

	// Token: 0x04003426 RID: 13350
	private float growDelay = 1f;

	// Token: 0x04003427 RID: 13351
	private double timeToCheckCollision;

	// Token: 0x04003428 RID: 13352
	public BuilderResizeWatch.BuilderSizeChangeSettings growSettings;

	// Token: 0x04003429 RID: 13353
	public BuilderResizeWatch.BuilderSizeChangeSettings shrinkSettings;

	// Token: 0x02000644 RID: 1604
	[Serializable]
	public struct BuilderSizeChangeSettings
	{
		// Token: 0x0400342A RID: 13354
		public bool affectLayerA;

		// Token: 0x0400342B RID: 13355
		public bool affectLayerB;

		// Token: 0x0400342C RID: 13356
		public bool affectLayerC;

		// Token: 0x0400342D RID: 13357
		public bool affectLayerD;
	}
}
