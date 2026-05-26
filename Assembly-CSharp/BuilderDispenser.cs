using System;
using System.Collections;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200061A RID: 1562
public class BuilderDispenser : MonoBehaviour
{
	// Token: 0x060026DE RID: 9950 RVA: 0x000CD9F4 File Offset: 0x000CBBF4
	private void Awake()
	{
		this.nullPiece = new BuilderPieceSet.PieceInfo
		{
			piecePrefab = null,
			overrideSetMaterial = false
		};
	}

	// Token: 0x060026DF RID: 9951 RVA: 0x000CDA20 File Offset: 0x000CBC20
	public void UpdateDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		if (!this.hasPiece && Time.timeAsDouble > this.nextSpawnTime && this.pieceToSpawn.piecePrefab != null)
		{
			this.TrySpawnPiece();
			this.nextSpawnTime = Time.timeAsDouble + (double)this.spawnRetryDelay;
			return;
		}
		if (this.hasPiece && (this.spawnedPieceInstance == null || (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)))
		{
			base.StopAllCoroutines();
			if (this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
		}
	}

	// Token: 0x060026E0 RID: 9952 RVA: 0x000CDAEC File Offset: 0x000CBCEC
	public bool DoesPieceMatchSpawnInfo(BuilderPiece piece)
	{
		if (piece == null || this.pieceToSpawn.piecePrefab == null)
		{
			return false;
		}
		if (piece.pieceType != this.pieceToSpawn.piecePrefab.name.GetStaticHash())
		{
			return false;
		}
		if (!(piece.materialOptions != null))
		{
			return true;
		}
		int num = piece.materialType;
		int num2;
		Material material;
		int num3;
		piece.materialOptions.GetDefaultMaterial(out num2, out material, out num3);
		if (this.pieceToSpawn.overrideSetMaterial)
		{
			for (int i = 0; i < this.pieceToSpawn.pieceMaterialTypes.Length; i++)
			{
				string text = this.pieceToSpawn.pieceMaterialTypes[i];
				if (!string.IsNullOrEmpty(text))
				{
					int hashCode = text.GetHashCode();
					if (hashCode == num)
					{
						return true;
					}
					if (hashCode == num2 && num == -1)
					{
						return true;
					}
				}
				else if (num == -1 || num == num2)
				{
					return true;
				}
			}
		}
		else if (num == this.materialType || (this.materialType == num2 && num == -1) || (num == num2 && this.materialType == -1))
		{
			return true;
		}
		return false;
	}

	// Token: 0x060026E1 RID: 9953 RVA: 0x000CDBF0 File Offset: 0x000CBDF0
	public void ShelfPieceCreated(BuilderPiece piece, bool playAnimation)
	{
		if (this.DoesPieceMatchSpawnInfo(piece))
		{
			if (this.hasPiece && this.spawnedPieceInstance != null)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
			}
			this.spawnedPieceInstance = piece;
			this.hasPiece = true;
			this.spawnCount++;
			this.spawnCount = Mathf.Max(0, this.spawnCount);
			if (this.table.GetTableState() == BuilderTable.TableState.Ready && playAnimation)
			{
				base.StartCoroutine(this.PlayAnimation());
				if (this.playFX)
				{
					ObjectPools.instance.Instantiate(this.dispenserFX, this.spawnTransform.position, this.spawnTransform.rotation, true);
					return;
				}
				this.playFX = true;
				return;
			}
			else
			{
				Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
				Vector3 position = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
				Quaternion rotation = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
				this.spawnedPieceInstance.transform.SetPositionAndRotation(position, rotation);
				this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
				this.playFX = true;
			}
		}
	}

	// Token: 0x060026E2 RID: 9954 RVA: 0x000CDD30 File Offset: 0x000CBF30
	private IEnumerator PlayAnimation()
	{
		this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
		this.animateParent.Rewind();
		this.spawnedPieceInstance.transform.SetParent(this.animateParent.transform);
		this.spawnedPieceInstance.transform.SetLocalPositionAndRotation(this.pieceToSpawn.piecePrefab.desiredShelfOffset, Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset));
		this.animateParent.Play();
		yield return new WaitForSeconds(this.animateParent.clip.length);
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.Displayed)
		{
			this.spawnedPieceInstance.transform.SetParent(null);
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 position = this.displayTransform.position + this.displayTransform.rotation * desiredShelfOffset;
			Quaternion rotation = this.displayTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			this.spawnedPieceInstance.transform.SetPositionAndRotation(position, rotation);
			this.spawnedPieceInstance.SetState(BuilderPiece.State.OnShelf, false);
		}
		yield break;
	}

	// Token: 0x060026E3 RID: 9955 RVA: 0x000CDD40 File Offset: 0x000CBF40
	public void ShelfPieceRecycled(BuilderPiece piece)
	{
		if (piece != null && this.spawnedPieceInstance != null && piece.Equals(this.spawnedPieceInstance))
		{
			piece.shelfOwner = -1;
			this.spawnedPieceInstance = null;
			this.hasPiece = false;
			this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		}
	}

	// Token: 0x060026E4 RID: 9956 RVA: 0x000CDD9C File Offset: 0x000CBF9C
	public void AssignPieceType(BuilderPieceSet.PieceInfo piece, int inMaterialType)
	{
		this.playFX = false;
		this.pieceToSpawn = piece;
		this.materialType = inMaterialType;
		this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
		this.currentAnimation = this.dispenseDefaultAnimation;
		this.animateParent.clip = this.currentAnimation;
		this.spawnCount = 0;
	}

	// Token: 0x060026E5 RID: 9957 RVA: 0x000CDDF8 File Offset: 0x000CBFF8
	private void TrySpawnPiece()
	{
		if (this.spawnedPieceInstance != null && this.spawnedPieceInstance.state == BuilderPiece.State.OnShelf)
		{
			return;
		}
		if (this.pieceToSpawn.piecePrefab == null)
		{
			return;
		}
		if (this.table.HasEnoughResources(this.pieceToSpawn.piecePrefab))
		{
			Vector3 desiredShelfOffset = this.pieceToSpawn.piecePrefab.desiredShelfOffset;
			Vector3 position = this.spawnTransform.position + this.spawnTransform.rotation * desiredShelfOffset;
			Quaternion rotation = this.spawnTransform.rotation * Quaternion.Euler(this.pieceToSpawn.piecePrefab.desiredShelfRotationOffset);
			int num = this.materialType;
			if (this.pieceToSpawn.overrideSetMaterial && this.pieceToSpawn.pieceMaterialTypes.Length != 0)
			{
				int num2 = this.spawnCount % this.pieceToSpawn.pieceMaterialTypes.Length;
				string text = this.pieceToSpawn.pieceMaterialTypes[num2];
				if (string.IsNullOrEmpty(text))
				{
					num = -1;
				}
				else
				{
					num = text.GetHashCode();
				}
			}
			this.table.RequestCreateDispenserShelfPiece(this.pieceToSpawn.piecePrefab.name.GetStaticHash(), position, rotation, num, this.shelfID);
		}
	}

	// Token: 0x060026E6 RID: 9958 RVA: 0x000CDF30 File Offset: 0x000CC130
	public void ParentPieceToShelf(Transform shelfTransform)
	{
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				base.StopAllCoroutines();
				if (this.spawnedPieceInstance != null)
				{
					this.spawnedPieceInstance.shelfOwner = -1;
				}
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				this.hasPiece = false;
				return;
			}
			this.spawnedPieceInstance.SetState(BuilderPiece.State.Displayed, false);
			this.spawnedPieceInstance.transform.SetParent(shelfTransform);
		}
	}

	// Token: 0x060026E7 RID: 9959 RVA: 0x000CDFC8 File Offset: 0x000CC1C8
	public void ClearDispenser()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.pieceToSpawn = this.nullPiece;
		this.hasPiece = false;
		if (this.spawnedPieceInstance != null)
		{
			if (this.spawnedPieceInstance.state != BuilderPiece.State.OnShelf && this.spawnedPieceInstance.state != BuilderPiece.State.Displayed)
			{
				this.spawnedPieceInstance.shelfOwner = -1;
				this.nextSpawnTime = Time.timeAsDouble + (double)this.OnGrabSpawnDelay;
				this.spawnedPieceInstance = null;
				return;
			}
			this.table.RequestRecyclePiece(this.spawnedPieceInstance, false, -1);
		}
	}

	// Token: 0x060026E8 RID: 9960 RVA: 0x000CE054 File Offset: 0x000CC254
	public void OnClearTable()
	{
		this.playFX = false;
		this.nextSpawnTime = 0.0;
		this.hasPiece = false;
		this.spawnedPieceInstance = null;
	}

	// Token: 0x04003267 RID: 12903
	public Transform displayTransform;

	// Token: 0x04003268 RID: 12904
	public Transform spawnTransform;

	// Token: 0x04003269 RID: 12905
	public Animation animateParent;

	// Token: 0x0400326A RID: 12906
	public AnimationClip dispenseDefaultAnimation;

	// Token: 0x0400326B RID: 12907
	public GameObject dispenserFX;

	// Token: 0x0400326C RID: 12908
	private AnimationClip currentAnimation;

	// Token: 0x0400326D RID: 12909
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x0400326E RID: 12910
	[HideInInspector]
	public int shelfID;

	// Token: 0x0400326F RID: 12911
	private BuilderPieceSet.PieceInfo pieceToSpawn;

	// Token: 0x04003270 RID: 12912
	private BuilderPiece spawnedPieceInstance;

	// Token: 0x04003271 RID: 12913
	private int materialType = -1;

	// Token: 0x04003272 RID: 12914
	private BuilderPieceSet.PieceInfo nullPiece;

	// Token: 0x04003273 RID: 12915
	private int spawnCount;

	// Token: 0x04003274 RID: 12916
	private double nextSpawnTime;

	// Token: 0x04003275 RID: 12917
	private bool hasPiece;

	// Token: 0x04003276 RID: 12918
	private float OnGrabSpawnDelay = 0.5f;

	// Token: 0x04003277 RID: 12919
	private float spawnRetryDelay = 2f;

	// Token: 0x04003278 RID: 12920
	private bool playFX;
}
