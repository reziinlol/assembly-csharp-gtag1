using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Token: 0x02000619 RID: 1561
public class BuilderConveyor : MonoBehaviour
{
	// Token: 0x060026C6 RID: 9926 RVA: 0x000CD080 File Offset: 0x000CB280
	private void Start()
	{
		this.InitIfNeeded();
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000CD080 File Offset: 0x000CB280
	public void Setup()
	{
		this.InitIfNeeded();
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000CD088 File Offset: 0x000CB288
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.nextPieceToSpawn = 0;
		this.grabbedPieceTypes = new Queue<int>(10);
		this.grabbedPieceMaterials = new Queue<int>(10);
		this.setSelector.Setup(this._includeCategories);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		this.setSelector.OnSelectedGroup.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.initialized = true;
		this.splineLength = this.spline.Splines[0].GetLength();
		this.maxItemsOnSpline = Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
		this.nativeSpline = new NativeSpline(this.spline.Splines[0], this.spline.transform.localToWorldMatrix, Allocator.Persistent);
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000CD1FC File Offset: 0x000CB3FC
	public int GetMaxItemsOnConveyor()
	{
		return Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000CD219 File Offset: 0x000CB419
	public float GetFrameMovement()
	{
		return this.conveyorMoveSpeed / this.splineLength;
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000CD228 File Offset: 0x000CB428
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedGroup.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
		this.nativeSpline.Dispose();
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000CD25F File Offset: 0x000CB45F
	public void OnSelectedSetChange(int displayGroupID)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, displayGroupID, true);
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000CD284 File Offset: 0x000CB484
	public void SetSelection(int displayGroupID)
	{
		this.setSelector.SetSelection(displayGroupID);
		this.currentDisplayGroup = this.setSelector.GetSelectedGroup();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentDisplayGroup.pieceSubsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		this.nextPieceToSpawn = 0;
		this.loopCount = 0;
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000CD330 File Offset: 0x000CB530
	public int GetSelectedDisplayGroupID()
	{
		return this.setSelector.GetSelectedGroup().GetDisplayGroupIdentifier();
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000CD344 File Offset: 0x000CB544
	public void UpdateConveyor()
	{
		if (!this.initialized)
		{
			this.Setup();
		}
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (builderPiece.state != BuilderPiece.State.OnConveyor)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient && builderPiece.state != BuilderPiece.State.None)
				{
					this.grabbedPieceTypes.Enqueue(builderPiece.pieceType);
					this.grabbedPieceMaterials.Enqueue(builderPiece.materialType);
				}
				builderPiece.shelfOwner = -1;
				this.piecesOnConveyor.RemoveAt(i);
				this.table.conveyorManager.RemovePieceFromJob(builderPiece);
			}
		}
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000CD3E8 File Offset: 0x000CB5E8
	public void RemovePieceFromConveyor(Transform pieceTransform)
	{
		foreach (BuilderPiece builderPiece in this.piecesOnConveyor)
		{
			if (builderPiece.transform == pieceTransform)
			{
				this.piecesOnConveyor.Remove(builderPiece);
				builderPiece.shelfOwner = -1;
				this.table.RequestRecyclePiece(builderPiece, false, -1);
				break;
			}
		}
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000CD468 File Offset: 0x000CB668
	private Vector3 EvaluateSpline(float t)
	{
		float t2;
		this._evaluateCurve = this.nativeSpline.GetCurve(this.nativeSpline.SplineToCurveT(t, out t2));
		return CurveUtility.EvaluatePosition(this._evaluateCurve, t2);
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000CD4A8 File Offset: 0x000CB6A8
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
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
		if (this.waitForResourceChange)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble >= this.nextSpawnTime)
		{
			this.SpawnNextPiece();
			this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		}
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x000CD538 File Offset: 0x000CB738
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x060026D4 RID: 9940 RVA: 0x000CD541 File Offset: 0x000CB741
	public void OnAvailableResourcesChange()
	{
		this.waitForResourceChange = false;
	}

	// Token: 0x060026D5 RID: 9941 RVA: 0x000CD54A File Offset: 0x000CB74A
	public Transform GetSpawnTransform()
	{
		return this.spawnTransform;
	}

	// Token: 0x060026D6 RID: 9942 RVA: 0x000CD554 File Offset: 0x000CB754
	public void OnShelfPieceCreated(BuilderPiece piece, float timeOffset)
	{
		float num = timeOffset * this.conveyorMoveSpeed / this.splineLength;
		if (num > 1f)
		{
			Debug.LogWarningFormat("Piece {0} add to shelf time {1}", new object[]
			{
				piece.pieceId,
				num
			});
		}
		int count = this.piecesOnConveyor.Count;
		this.piecesOnConveyor.Add(piece);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		Vector3 a = this.EvaluateSpline(num2);
		Quaternion rotation = this.spawnTransform.rotation * Quaternion.Euler(piece.desiredShelfRotationOffset);
		Vector3 position = a + this.spawnTransform.rotation * piece.desiredShelfOffset;
		piece.transform.SetPositionAndRotation(position, rotation);
		if (num <= 1f)
		{
			this.table.conveyorManager.AddPieceToJob(piece, num2, this.shelfID);
		}
	}

	// Token: 0x060026D7 RID: 9943 RVA: 0x000CD635 File Offset: 0x000CB835
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		this.piecesOnConveyor.Remove(piece);
		if (piece != null)
		{
			this.table.conveyorManager.RemovePieceFromJob(piece);
		}
	}

	// Token: 0x060026D8 RID: 9944 RVA: 0x000CD65E File Offset: 0x000CB85E
	public void OnClearTable()
	{
		this.piecesOnConveyor.Clear();
		this.grabbedPieceTypes.Clear();
		this.grabbedPieceMaterials.Clear();
	}

	// Token: 0x060026D9 RID: 9945 RVA: 0x000CD684 File Offset: 0x000CB884
	public void ResetConveyorState()
	{
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (!(builderPiece == null))
			{
				BuilderTable.BuilderCommand cmd = new BuilderTable.BuilderCommand
				{
					type = BuilderTable.BuilderCommandType.Recycle,
					pieceId = builderPiece.pieceId,
					localPosition = builderPiece.transform.position,
					localRotation = builderPiece.transform.rotation,
					player = NetworkSystem.Instance.LocalPlayer,
					isLeft = false,
					parentPieceId = -1
				};
				this.table.ExecutePieceRecycled(cmd);
			}
		}
		this.OnClearTable();
	}

	// Token: 0x060026DA RID: 9946 RVA: 0x000CD73C File Offset: 0x000CB93C
	private void SpawnNextPiece()
	{
		int num;
		int materialType;
		this.FindNextAffordablePieceType(out num, out materialType);
		if (num == -1)
		{
			return;
		}
		this.table.RequestCreateConveyorPiece(num, materialType, this.shelfID);
	}

	// Token: 0x060026DB RID: 9947 RVA: 0x000CD76C File Offset: 0x000CB96C
	private void FindNextAffordablePieceType(out int pieceType, out int materialType)
	{
		if (this.grabbedPieceTypes.Count > 0)
		{
			pieceType = this.grabbedPieceTypes.Dequeue();
			materialType = this.grabbedPieceMaterials.Dequeue();
			return;
		}
		pieceType = -1;
		materialType = -1;
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		for (int i = this.nextPieceToSpawn; i < this.piecesInSet.Count; i++)
		{
			BuilderPiece piecePrefab = this.piecesInSet[i].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab))
			{
				if (i + 1 >= this.piecesInSet.Count)
				{
					this.loopCount++;
					this.loopCount = Mathf.Max(0, this.loopCount);
				}
				this.nextPieceToSpawn = (i + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[i]);
				return;
			}
		}
		this.loopCount++;
		this.loopCount = Mathf.Max(0, this.loopCount);
		for (int j = 0; j < this.nextPieceToSpawn; j++)
		{
			BuilderPiece piecePrefab2 = this.piecesInSet[j].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab2))
			{
				this.nextPieceToSpawn = (j + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab2.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[j]);
				return;
			}
		}
		this.waitForResourceChange = true;
	}

	// Token: 0x060026DC RID: 9948 RVA: 0x000CD8F0 File Offset: 0x000CBAF0
	private int GetMaterialType(BuilderPieceSet.PieceInfo info)
	{
		if (info.piecePrefab.materialOptions != null && info.overrideSetMaterial && info.pieceMaterialTypes.Length != 0)
		{
			int num = this.loopCount % info.pieceMaterialTypes.Length;
			string text = info.pieceMaterialTypes[num];
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogErrorFormat("Empty Material Override for piece {0} in set {1}", new object[]
				{
					info.piecePrefab.name,
					this.currentDisplayGroup.displayName
				});
				return -1;
			}
			return text.GetHashCode();
		}
		else
		{
			if (string.IsNullOrEmpty(this.currentDisplayGroup.defaultMaterial))
			{
				return -1;
			}
			return this.currentDisplayGroup.defaultMaterial.GetHashCode();
		}
	}

	// Token: 0x0400324F RID: 12879
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04003250 RID: 12880
	public List<BuilderPieceSet.BuilderPieceCategory> _includeCategories;

	// Token: 0x04003251 RID: 12881
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x04003252 RID: 12882
	public int shelfID = -1;

	// Token: 0x04003253 RID: 12883
	[Header("Conveyor Properties")]
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x04003254 RID: 12884
	[SerializeField]
	private SplineContainer spline;

	// Token: 0x04003255 RID: 12885
	private float conveyorMoveSpeed = 0.2f;

	// Token: 0x04003256 RID: 12886
	private float spawnDelay = 1.5f;

	// Token: 0x04003257 RID: 12887
	private double nextSpawnTime;

	// Token: 0x04003258 RID: 12888
	private int nextPieceToSpawn;

	// Token: 0x04003259 RID: 12889
	private BuilderPieceSet.BuilderDisplayGroup currentDisplayGroup;

	// Token: 0x0400325A RID: 12890
	private int loopCount;

	// Token: 0x0400325B RID: 12891
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x0400325C RID: 12892
	private Queue<int> grabbedPieceTypes;

	// Token: 0x0400325D RID: 12893
	private Queue<int> grabbedPieceMaterials;

	// Token: 0x0400325E RID: 12894
	private List<BuilderPiece> piecesOnConveyor = new List<BuilderPiece>(10);

	// Token: 0x0400325F RID: 12895
	private Vector3 moveDirection;

	// Token: 0x04003260 RID: 12896
	private bool waitForResourceChange;

	// Token: 0x04003261 RID: 12897
	private bool initialized;

	// Token: 0x04003262 RID: 12898
	private float splineLength = 1f;

	// Token: 0x04003263 RID: 12899
	private int maxItemsOnSpline;

	// Token: 0x04003264 RID: 12900
	private UnityEngine.Splines.BezierCurve _evaluateCurve;

	// Token: 0x04003265 RID: 12901
	public NativeSpline nativeSpline;

	// Token: 0x04003266 RID: 12902
	private bool shouldVerifySetSelection;
}
