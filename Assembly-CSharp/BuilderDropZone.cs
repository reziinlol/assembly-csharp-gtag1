using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200061D RID: 1565
public class BuilderDropZone : MonoBehaviour
{
	// Token: 0x06002703 RID: 9987 RVA: 0x000CEA2A File Offset: 0x000CCC2A
	private void Awake()
	{
		this.repelDirectionWorld = base.transform.TransformDirection(this.repelDirectionLocal.normalized);
	}

	// Token: 0x06002704 RID: 9988 RVA: 0x000CEA48 File Offset: 0x000CCC48
	private void OnTriggerEnter(Collider other)
	{
		if (!this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x06002705 RID: 9989 RVA: 0x000CEB32 File Offset: 0x000CCD32
	public Vector3 GetRepelDirectionWorld()
	{
		return this.repelDirectionWorld;
	}

	// Token: 0x06002706 RID: 9990 RVA: 0x000CEB3C File Offset: 0x000CCD3C
	public void PlayEffect()
	{
		if (this.vfxRoot != null && !this.playingEffect)
		{
			this.vfxRoot.SetActive(true);
			this.playingEffect = true;
			if (this.sfxPrefab != null)
			{
				ObjectPools.instance.Instantiate(this.sfxPrefab, base.transform.position, base.transform.rotation, true);
			}
			base.StartCoroutine(this.DelayedStopEffect());
		}
	}

	// Token: 0x06002707 RID: 9991 RVA: 0x000CEBB5 File Offset: 0x000CCDB5
	private IEnumerator DelayedStopEffect()
	{
		yield return new WaitForSeconds(this.effectDuration);
		this.vfxRoot.SetActive(false);
		this.playingEffect = false;
		yield break;
	}

	// Token: 0x06002708 RID: 9992 RVA: 0x000CEBC4 File Offset: 0x000CCDC4
	private void OnTriggerExit(Collider other)
	{
		if (this.onEnter)
		{
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		BuilderPieceCollider component = other.GetComponent<BuilderPieceCollider>();
		if (component != null)
		{
			BuilderPiece piece = component.piece;
			if (this.table != null && this.table.builderNetworking != null)
			{
				if (piece == null)
				{
					return;
				}
				if (this.dropType == BuilderDropZone.DropType.Recycle)
				{
					bool flag = piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.OnShelf && piece.state > BuilderPiece.State.AttachedAndPlaced;
					if (!piece.isBuiltIntoTable && flag)
					{
						this.table.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, true, -1);
						return;
					}
				}
				else
				{
					this.table.builderNetworking.PieceEnteredDropZone(piece, this.dropType, this.dropZoneID);
				}
			}
		}
	}

	// Token: 0x04003292 RID: 12946
	[SerializeField]
	private BuilderDropZone.DropType dropType;

	// Token: 0x04003293 RID: 12947
	[SerializeField]
	private bool onEnter = true;

	// Token: 0x04003294 RID: 12948
	[SerializeField]
	private GameObject vfxRoot;

	// Token: 0x04003295 RID: 12949
	[SerializeField]
	private GameObject sfxPrefab;

	// Token: 0x04003296 RID: 12950
	public float effectDuration = 1f;

	// Token: 0x04003297 RID: 12951
	private bool playingEffect;

	// Token: 0x04003298 RID: 12952
	public bool overrideDirection;

	// Token: 0x04003299 RID: 12953
	[SerializeField]
	private Vector3 repelDirectionLocal = Vector3.up;

	// Token: 0x0400329A RID: 12954
	private Vector3 repelDirectionWorld = Vector3.up;

	// Token: 0x0400329B RID: 12955
	[HideInInspector]
	public int dropZoneID = -1;

	// Token: 0x0400329C RID: 12956
	internal BuilderTable table;

	// Token: 0x0200061E RID: 1566
	public enum DropType
	{
		// Token: 0x0400329E RID: 12958
		Invalid = -1,
		// Token: 0x0400329F RID: 12959
		Repel,
		// Token: 0x040032A0 RID: 12960
		ReturnToShelf,
		// Token: 0x040032A1 RID: 12961
		BreakApart,
		// Token: 0x040032A2 RID: 12962
		Recycle
	}
}
