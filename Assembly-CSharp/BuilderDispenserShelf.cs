using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200061C RID: 1564
public class BuilderDispenserShelf : MonoBehaviour
{
	// Token: 0x060026F0 RID: 9968 RVA: 0x000CE233 File Offset: 0x000CC433
	private void BuildDispenserPool()
	{
		this.dispenserPool = new List<BuilderDispenser>(12);
		this.activeDispensers = new List<BuilderDispenser>(6);
		this.AddToDispenserPool(6);
	}

	// Token: 0x060026F1 RID: 9969 RVA: 0x000CE258 File Offset: 0x000CC458
	private void AddToDispenserPool(int count)
	{
		if (this.dispenserPrefab == null)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			BuilderDispenser builderDispenser = Object.Instantiate<BuilderDispenser>(this.dispenserPrefab, this.shelfCenter);
			builderDispenser.gameObject.SetActive(false);
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
			this.dispenserPool.Add(builderDispenser);
		}
	}

	// Token: 0x060026F2 RID: 9970 RVA: 0x000CE2C4 File Offset: 0x000CC4C4
	private void ActivateDispensers()
	{
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentGroup.pieceSubsets)
		{
			if (this._includedCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		int count = this.piecesInSet.Count;
		if (this.dispenserPool.Count < count)
		{
			this.AddToDispenserPool(count - this.dispenserPool.Count);
		}
		this.activeDispensers.Clear();
		for (int i = 0; i < this.dispenserPool.Count; i++)
		{
			if (i < count)
			{
				BuilderDispenser builderDispenser = this.dispenserPool[i];
				builderDispenser.gameObject.SetActive(true);
				float x = this.shelfWidth / -2f + this.shelfWidth / (float)(count * 2) + this.shelfWidth / (float)count * (float)i;
				builderDispenser.transform.localPosition = new Vector3(x, 0f, 0f);
				builderDispenser.AssignPieceType(this.piecesInSet[i], this.currentGroup.defaultMaterial.GetHashCode());
				this.activeDispensers.Add(builderDispenser);
			}
			else
			{
				this.dispenserPool[i].ClearDispenser();
				this.dispenserPool[i].gameObject.SetActive(false);
			}
		}
		this.dispenserToUpdate = 0;
	}

	// Token: 0x060026F3 RID: 9971 RVA: 0x000CE470 File Offset: 0x000CC670
	public void Setup()
	{
		this.InitIfNeeded();
		foreach (BuilderDispenser builderDispenser in this.dispenserPool)
		{
			builderDispenser.table = this.table;
			builderDispenser.shelfID = this.shelfID;
		}
	}

	// Token: 0x060026F4 RID: 9972 RVA: 0x000CE4D8 File Offset: 0x000CC6D8
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.setSelector.Setup(this._includedCategories);
		this.currentGroup = this.setSelector.GetSelectedGroup();
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.BuildDispenserPool();
		this.ActivateDispensers();
		this.initialized = true;
	}

	// Token: 0x060026F5 RID: 9973 RVA: 0x000CE53F File Offset: 0x000CC73F
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
	}

	// Token: 0x060026F6 RID: 9974 RVA: 0x000CE56B File Offset: 0x000CC76B
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, false);
	}

	// Token: 0x060026F7 RID: 9975 RVA: 0x000CE590 File Offset: 0x000CC790
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
		if ((this.initialized && this.currentGroup == null) || selectedGroup.displayName != this.currentGroup.displayName)
		{
			this.currentGroup = selectedGroup;
			if (this.table.GetTableState() == BuilderTable.TableState.Ready)
			{
				if (!this.animatingShelf)
				{
					this.StartShelfSwap();
					return;
				}
			}
			else
			{
				this.animatingShelf = false;
				this.ImmediateShelfSwap();
			}
		}
	}

	// Token: 0x060026F8 RID: 9976 RVA: 0x000CE60E File Offset: 0x000CC80E
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x060026F9 RID: 9977 RVA: 0x000CE620 File Offset: 0x000CC820
	private void ImmediateShelfSwap()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
		this.ActivateDispensers();
	}

	// Token: 0x060026FA RID: 9978 RVA: 0x000CE678 File Offset: 0x000CC878
	private void StartShelfSwap()
	{
		this.dispenserToClear = 0;
		this.timeToClearShelf = (double)(Time.time + 0.15f);
		this.resetAnimation.Rewind();
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ParentPieceToShelf(this.resetAnimation.transform);
		}
		this.resetAnimation.Play();
		this.animatingShelf = true;
	}

	// Token: 0x060026FB RID: 9979 RVA: 0x000CE70C File Offset: 0x000CC90C
	public void UpdateShelf()
	{
		if (this.animatingShelf && (double)Time.time > this.timeToClearShelf)
		{
			if (this.dispenserToClear < this.activeDispensers.Count)
			{
				if (this.dispenserToClear == 0)
				{
					this.resetSoundBank.Play();
				}
				this.activeDispensers[this.dispenserToClear].ClearDispenser();
				this.dispenserToClear++;
				return;
			}
			if (!this.resetAnimation.isPlaying)
			{
				this.playSpawnSetSound = true;
				this.ActivateDispensers();
				this.animatingShelf = false;
			}
		}
	}

	// Token: 0x060026FC RID: 9980 RVA: 0x000CE79C File Offset: 0x000CC99C
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (!this.initialized)
		{
			return;
		}
		if (this.animatingShelf)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet.BuilderDisplayGroup selectedGroup = this.setSelector.GetSelectedGroup();
			if (selectedGroup == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedGroup.setID))
			{
				int defaultGroupID = this.setSelector.GetDefaultGroupID();
				if (defaultGroupID != -1)
				{
					this.OnSelectedSetChange(defaultGroupID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.activeDispensers.Count > 0)
		{
			this.activeDispensers[this.dispenserToUpdate].UpdateDispenser();
			this.dispenserToUpdate = (this.dispenserToUpdate + 1) % this.activeDispensers.Count;
		}
	}

	// Token: 0x060026FD RID: 9981 RVA: 0x000CE84F File Offset: 0x000CCA4F
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x060026FE RID: 9982 RVA: 0x000CE858 File Offset: 0x000CCA58
	public void OnShelfPieceCreated(BuilderPiece piece, bool playfx)
	{
		if (this.playSpawnSetSound && playfx)
		{
			this.audioSource.GTPlayOneShot(this.spawnNewSetSound, 1f);
			this.playSpawnSetSound = false;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceCreated(piece, playfx);
		}
	}

	// Token: 0x060026FF RID: 9983 RVA: 0x000CE8D4 File Offset: 0x000CCAD4
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ShelfPieceRecycled(piece);
		}
	}

	// Token: 0x06002700 RID: 9984 RVA: 0x000CE928 File Offset: 0x000CCB28
	public void OnClearTable()
	{
		if (!this.initialized)
		{
			return;
		}
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.OnClearTable();
		}
		base.StopAllCoroutines();
		if (this.animatingShelf)
		{
			this.resetAnimation.Rewind();
			this.animatingShelf = false;
		}
	}

	// Token: 0x06002701 RID: 9985 RVA: 0x000CE9A4 File Offset: 0x000CCBA4
	public void ClearShelf()
	{
		foreach (BuilderDispenser builderDispenser in this.activeDispensers)
		{
			builderDispenser.ClearDispenser();
		}
	}

	// Token: 0x0400327C RID: 12924
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x0400327D RID: 12925
	public List<BuilderPieceSet.BuilderPieceCategory> _includedCategories;

	// Token: 0x0400327E RID: 12926
	[Header("Dispenser Shelf Properties")]
	public Transform shelfCenter;

	// Token: 0x0400327F RID: 12927
	public float shelfWidth = 1.4f;

	// Token: 0x04003280 RID: 12928
	public Animation resetAnimation;

	// Token: 0x04003281 RID: 12929
	[SerializeField]
	private SoundBankPlayer resetSoundBank;

	// Token: 0x04003282 RID: 12930
	[SerializeField]
	private AudioClip spawnNewSetSound;

	// Token: 0x04003283 RID: 12931
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003284 RID: 12932
	private bool playSpawnSetSound;

	// Token: 0x04003285 RID: 12933
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04003286 RID: 12934
	public int shelfID = -1;

	// Token: 0x04003287 RID: 12935
	private BuilderPieceSet.BuilderDisplayGroup currentGroup;

	// Token: 0x04003288 RID: 12936
	private bool initialized;

	// Token: 0x04003289 RID: 12937
	public BuilderDispenser dispenserPrefab;

	// Token: 0x0400328A RID: 12938
	private List<BuilderDispenser> dispenserPool;

	// Token: 0x0400328B RID: 12939
	private List<BuilderDispenser> activeDispensers;

	// Token: 0x0400328C RID: 12940
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x0400328D RID: 12941
	private bool animatingShelf;

	// Token: 0x0400328E RID: 12942
	private double timeToClearShelf = double.MaxValue;

	// Token: 0x0400328F RID: 12943
	private int dispenserToClear;

	// Token: 0x04003290 RID: 12944
	private int dispenserToUpdate;

	// Token: 0x04003291 RID: 12945
	private bool shouldVerifySetSelection;
}
