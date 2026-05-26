using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED9 RID: 3801
	public class BuilderPool : MonoBehaviour, IGorillaSimpleBackgroundWorker
	{
		// Token: 0x06005DAE RID: 23982 RVA: 0x001DB41E File Offset: 0x001D961E
		private void Awake()
		{
			if (BuilderPool.instance == null)
			{
				BuilderPool.instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005DAF RID: 23983 RVA: 0x001DB43C File Offset: 0x001D963C
		public void Setup()
		{
			if (this.isSetup)
			{
				return;
			}
			this.piecePools = new List<List<BuilderPiece>>(512);
			this.piecePoolLookup = new Dictionary<int, int>(512);
			this.bumpGlowPool = new List<BuilderBumpGlow>(256);
			this.AddToGlowBumpPool(256);
			this.snapOverlapPool = new List<SnapOverlap>(4096);
			this.AddToSnapOverlapPool(4096);
			this.isSetup = true;
		}

		// Token: 0x06005DB0 RID: 23984 RVA: 0x001DB4B0 File Offset: 0x001D96B0
		public void BuildFromShelves(List<BuilderShelf> shelves)
		{
			for (int i = 0; i < shelves.Count; i++)
			{
				BuilderShelf builderShelf = shelves[i];
				for (int j = 0; j < builderShelf.buildPieceSpawns.Count; j++)
				{
					BuilderShelf.BuildPieceSpawn buildPieceSpawn = builderShelf.buildPieceSpawns[j];
					this.AddToPool(buildPieceSpawn.buildPiecePrefab.name.GetStaticHash(), buildPieceSpawn.count);
				}
			}
		}

		// Token: 0x06005DB1 RID: 23985 RVA: 0x001DB515 File Offset: 0x001D9715
		public IEnumerator BuildFromPieceSets()
		{
			if (this.hasBuiltPieceSets)
			{
				yield break;
			}
			this.hasBuiltPieceSets = true;
			List<BuilderPieceSet> allPieceSets = BuilderSetManager.instance.GetAllPieceSets();
			foreach (BuilderPieceSet builderPieceSet in allPieceSets)
			{
				bool isStarterSet = BuilderSetManager.instance.GetStarterSetsConcat().Contains(builderPieceSet.playfabID);
				bool isFallbackSet = builderPieceSet.SetName.Equals("HIDDEN");
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
						int count;
						if (!this.piecePoolLookup.TryGetValue(staticHash, out count))
						{
							count = this.piecePools.Count;
							this.piecePools.Add(new List<BuilderPiece>(128));
							this.piecePoolLookup.Add(staticHash, count);
							if (!isFallbackSet)
							{
								int num = isStarterSet ? 32 : 8;
								int i = 0;
								while (i < num)
								{
									if (this.piecesToAdd.Count == 0)
									{
										GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
									}
									i += 2;
									this.piecesToAdd.Enqueue(staticHash);
								}
							}
						}
						yield return null;
					}
					List<BuilderPieceSet.PieceInfo>.Enumerator enumerator3 = default(List<BuilderPieceSet.PieceInfo>.Enumerator);
				}
				List<BuilderPieceSet.BuilderPieceSubset>.Enumerator enumerator2 = default(List<BuilderPieceSet.BuilderPieceSubset>.Enumerator);
			}
			List<BuilderPieceSet>.Enumerator enumerator = default(List<BuilderPieceSet>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06005DB2 RID: 23986 RVA: 0x001DB524 File Offset: 0x001D9724
		public void SimpleWork()
		{
			int count = 2;
			if (this.piecesToAdd.Count > 0)
			{
				this.AddToPool(this.piecesToAdd.Dequeue(), count);
			}
			if (this.piecesToAdd.Count > 0)
			{
				GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
			}
		}

		// Token: 0x06005DB3 RID: 23987 RVA: 0x001DB568 File Offset: 0x001D9768
		private void AddToPool(int pieceType, int count)
		{
			int count2;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count2))
			{
				count2 = this.piecePools.Count;
				this.piecePools.Add(new List<BuilderPiece>(count * 8));
				this.piecePoolLookup.Add(pieceType, count2);
				Debug.LogWarningFormat("Creating Pool for piece {0} of size {1}. Is this piece not in a piece set?", new object[]
				{
					pieceType,
					count * 8
				});
			}
			BuilderPiece piecePrefab = BuilderSetManager.instance.GetPiecePrefab(pieceType);
			if (piecePrefab == null)
			{
				return;
			}
			List<BuilderPiece> list = this.piecePools[count2];
			for (int i = 0; i < count; i++)
			{
				BuilderPiece builderPiece = Object.Instantiate<BuilderPiece>(piecePrefab);
				builderPiece.OnCreatedByPool();
				builderPiece.gameObject.SetActive(false);
				list.Add(builderPiece);
			}
		}

		// Token: 0x06005DB4 RID: 23988 RVA: 0x001DB62C File Offset: 0x001D982C
		public BuilderPiece CreatePiece(int pieceType, bool assertNotEmpty)
		{
			int count;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count))
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("No Pool Found for {0} Adding 4", new object[]
					{
						pieceType
					});
				}
				count = this.piecePools.Count;
				this.AddToPool(pieceType, 4);
			}
			List<BuilderPiece> list = this.piecePools[count];
			if (list.Count == 0)
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("Pool for {0} is Empty Adding 4", new object[]
					{
						pieceType
					});
				}
				this.AddToPool(pieceType, 4);
			}
			BuilderPiece result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}

		// Token: 0x06005DB5 RID: 23989 RVA: 0x001DB6D0 File Offset: 0x001D98D0
		public void DestroyPiece(BuilderPiece piece)
		{
			if (piece == null)
			{
				Debug.LogError("Why is a null piece being destroyed");
				return;
			}
			int index;
			if (!this.piecePoolLookup.TryGetValue(piece.pieceType, out index))
			{
				Debug.LogErrorFormat("No Pool Found for {0} Cannot return to pool", new object[]
				{
					piece.pieceType
				});
				return;
			}
			List<BuilderPiece> list = this.piecePools[index];
			if (list.Count == 128)
			{
				piece.OnReturnToPool();
				Object.Destroy(piece.gameObject);
				return;
			}
			piece.gameObject.SetActive(false);
			piece.transform.SetParent(null);
			piece.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			piece.OnReturnToPool();
			list.Add(piece);
		}

		// Token: 0x06005DB6 RID: 23990 RVA: 0x001DB798 File Offset: 0x001D9998
		private void AddToGlowBumpPool(int count)
		{
			if (this.bumpGlowPrefab == null)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				BuilderBumpGlow builderBumpGlow = Object.Instantiate<BuilderBumpGlow>(this.bumpGlowPrefab);
				builderBumpGlow.gameObject.SetActive(false);
				this.bumpGlowPool.Add(builderBumpGlow);
			}
		}

		// Token: 0x06005DB7 RID: 23991 RVA: 0x001DB7E4 File Offset: 0x001D99E4
		public BuilderBumpGlow CreateGlowBump()
		{
			if (this.bumpGlowPool.Count == 0)
			{
				this.AddToGlowBumpPool(4);
			}
			BuilderBumpGlow result = this.bumpGlowPool[this.bumpGlowPool.Count - 1];
			this.bumpGlowPool.RemoveAt(this.bumpGlowPool.Count - 1);
			return result;
		}

		// Token: 0x06005DB8 RID: 23992 RVA: 0x001DB838 File Offset: 0x001D9A38
		public void DestroyBumpGlow(BuilderBumpGlow bump)
		{
			if (bump == null)
			{
				return;
			}
			bump.gameObject.SetActive(false);
			bump.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			this.bumpGlowPool.Add(bump);
		}

		// Token: 0x06005DB9 RID: 23993 RVA: 0x001DB888 File Offset: 0x001D9A88
		private void AddToSnapOverlapPool(int count)
		{
			this.snapOverlapPool.Capacity = this.snapOverlapPool.Capacity + count;
			for (int i = 0; i < count; i++)
			{
				this.snapOverlapPool.Add(new SnapOverlap
				{
					inPool = true
				});
			}
		}

		// Token: 0x06005DBA RID: 23994 RVA: 0x001DB8D0 File Offset: 0x001D9AD0
		public SnapOverlap CreateSnapOverlap(BuilderAttachGridPlane otherPlane, SnapBounds bounds)
		{
			if (this.snapOverlapPool.Count == 0)
			{
				this.AddToSnapOverlapPool(1024);
			}
			SnapOverlap snapOverlap = this.snapOverlapPool[this.snapOverlapPool.Count - 1];
			this.snapOverlapPool.RemoveAt(this.snapOverlapPool.Count - 1);
			snapOverlap.otherPlane = otherPlane;
			snapOverlap.bounds = bounds;
			snapOverlap.nextOverlap = null;
			snapOverlap.inPool = false;
			return snapOverlap;
		}

		// Token: 0x06005DBB RID: 23995 RVA: 0x001DB941 File Offset: 0x001D9B41
		public void DestroySnapOverlap(SnapOverlap snapOverlap)
		{
			if (snapOverlap.inPool)
			{
				return;
			}
			snapOverlap.otherPlane = null;
			snapOverlap.nextOverlap = null;
			snapOverlap.inPool = true;
			this.snapOverlapPool.Add(snapOverlap);
		}

		// Token: 0x06005DBC RID: 23996 RVA: 0x001DB970 File Offset: 0x001D9B70
		private void OnDestroy()
		{
			for (int i = 0; i < this.piecePools.Count; i++)
			{
				if (this.piecePools[i] != null)
				{
					foreach (BuilderPiece builderPiece in this.piecePools[i])
					{
						if (builderPiece != null)
						{
							Object.Destroy(builderPiece);
						}
					}
					this.piecePools[i].Clear();
				}
			}
			this.piecePoolLookup.Clear();
			foreach (BuilderBumpGlow obj in this.bumpGlowPool)
			{
				Object.Destroy(obj);
			}
			this.bumpGlowPool.Clear();
		}

		// Token: 0x04006C42 RID: 27714
		public List<List<BuilderPiece>> piecePools;

		// Token: 0x04006C43 RID: 27715
		public Dictionary<int, int> piecePoolLookup;

		// Token: 0x04006C44 RID: 27716
		[HideInInspector]
		public List<BuilderBumpGlow> bumpGlowPool;

		// Token: 0x04006C45 RID: 27717
		public BuilderBumpGlow bumpGlowPrefab;

		// Token: 0x04006C46 RID: 27718
		[HideInInspector]
		public List<SnapOverlap> snapOverlapPool;

		// Token: 0x04006C47 RID: 27719
		public static BuilderPool instance;

		// Token: 0x04006C48 RID: 27720
		private const int POOl_CAPACITY = 128;

		// Token: 0x04006C49 RID: 27721
		private const int INITIAL_INSTANCE_COUNT_STARTER = 32;

		// Token: 0x04006C4A RID: 27722
		private const int INITIAL_INSTANCE_COUNT_PREMIUM = 8;

		// Token: 0x04006C4B RID: 27723
		private bool isSetup;

		// Token: 0x04006C4C RID: 27724
		private bool hasBuiltPieceSets;

		// Token: 0x04006C4D RID: 27725
		private Queue<int> piecesToAdd = new Queue<int>();
	}
}
