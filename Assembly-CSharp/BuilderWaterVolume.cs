using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000612 RID: 1554
public class BuilderWaterVolume : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x060026A7 RID: 9895 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPieceDestroy()
	{
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000CC81C File Offset: 0x000CAA1C
	public void OnPiecePlacementDeserialized()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000CC8A8 File Offset: 0x000CAAA8
	public void OnPieceActivate()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000CC934 File Offset: 0x000CAB34
	public void OnPieceDeactivate()
	{
		this.waterVolume.SetActive(false);
		this.waterMesh.SetActive(true);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = this.floating.localPosition;
		}
	}

	// Token: 0x04003224 RID: 12836
	[SerializeField]
	private BuilderPiece piece;

	// Token: 0x04003225 RID: 12837
	[SerializeField]
	private GameObject waterVolume;

	// Token: 0x04003226 RID: 12838
	[SerializeField]
	private GameObject waterMesh;

	// Token: 0x04003227 RID: 12839
	[FormerlySerializedAs("lillyPads")]
	[SerializeField]
	private Transform floatingObjects;

	// Token: 0x04003228 RID: 12840
	[SerializeField]
	private Transform floating;

	// Token: 0x04003229 RID: 12841
	[SerializeField]
	private Transform sunk;
}
