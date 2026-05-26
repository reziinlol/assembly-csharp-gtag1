using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x02000617 RID: 1559
public class BuilderArmShelf : MonoBehaviour
{
	// Token: 0x060026BC RID: 9916 RVA: 0x000CCEF4 File Offset: 0x000CB0F4
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000CCF02 File Offset: 0x000CB102
	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000CCF1F File Offset: 0x000CB11F
	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000CCF48 File Offset: 0x000CB148
	public void DropAttachedPieces()
	{
		if (this.ownerRig != null && this.piece != null)
		{
			Vector3 velocity = Vector3.zero;
			if (this.piece.firstChildPiece == null)
			{
				return;
			}
			BuilderTable table = this.piece.GetTable();
			Vector3 point = table.roomCenter.position - this.piece.transform.position;
			point.Normalize();
			Vector3 a = Quaternion.Euler(0f, 180f, 0f) * point;
			velocity = BuilderTable.DROP_ZONE_REPEL * a;
			BuilderPiece builderPiece = this.piece.firstChildPiece;
			while (builderPiece != null)
			{
				table.RequestDropPiece(builderPiece, builderPiece.transform.position + a * 0.1f, builderPiece.transform.rotation, velocity, Vector3.zero);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}

	// Token: 0x04003249 RID: 12873
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x0400324A RID: 12874
	public Transform pieceAnchor;

	// Token: 0x0400324B RID: 12875
	private VRRig ownerRig;
}
