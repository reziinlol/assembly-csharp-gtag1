using System;
using GorillaTagScripts.Builder;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED2 RID: 3794
	public class BuilderAttachGridPlane : MonoBehaviour
	{
		// Token: 0x06005D69 RID: 23913 RVA: 0x001D98D1 File Offset: 0x001D7AD1
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06005D6A RID: 23914 RVA: 0x001D98F0 File Offset: 0x001D7AF0
		public void Setup(BuilderPiece piece, int attachIndex, float gridSize)
		{
			this.piece = piece;
			this.attachIndex = attachIndex;
			this.pieceToGridPosition = piece.transform.InverseTransformPoint(base.transform.position);
			this.pieceToGridRotation = Quaternion.Inverse(piece.transform.rotation) * base.transform.rotation;
			float num = (float)(this.width + 2) * gridSize;
			float num2 = (float)(this.length + 2) * gridSize;
			this.boundingRadius = Mathf.Sqrt(num * num + num2 * num2);
			this.connected = new bool[this.width * this.length];
			this.widthOffset = ((this.width % 2 == 0) ? (gridSize / 2f) : 0f);
			this.lengthOffset = ((this.length % 2 == 0) ? (gridSize / 2f) : 0f);
			this.gridPlaneDataIndex = -1;
			this.childPieceCount = 0;
		}

		// Token: 0x06005D6B RID: 23915 RVA: 0x001D99DC File Offset: 0x001D7BDC
		public void OnReturnToPool(BuilderPool pool)
		{
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				SnapOverlap snapOverlap = nextOverlap;
				nextOverlap = nextOverlap.nextOverlap;
				if (snapOverlap.otherPlane != null)
				{
					snapOverlap.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
				}
				this.SetConnected(snapOverlap.bounds, false);
				pool.DestroySnapOverlap(snapOverlap);
			}
			this.firstOverlap = null;
			int num = this.width * this.length;
			for (int i = 0; i < num; i++)
			{
				this.connected[i] = false;
			}
			this.childPieceCount = 0;
		}

		// Token: 0x06005D6C RID: 23916 RVA: 0x001D9A64 File Offset: 0x001D7C64
		public Vector3 GetGridPosition(int x, int z, float gridSize)
		{
			float num = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num2 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			return this.center.position + this.center.rotation * new Vector3((float)x * gridSize - num, (this.male ? 0.002f : -0.002f) * gridSize, (float)z * gridSize - num2);
		}

		// Token: 0x06005D6D RID: 23917 RVA: 0x001D9AEA File Offset: 0x001D7CEA
		public int GetChildCount()
		{
			return this.childPieceCount;
		}

		// Token: 0x06005D6E RID: 23918 RVA: 0x001D9AF4 File Offset: 0x001D7CF4
		public void ChangeChildPieceCount(int delta)
		{
			this.childPieceCount += delta;
			if (this.piece.parentPiece == null)
			{
				return;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return;
			}
			this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].ChangeChildPieceCount(delta);
		}

		// Token: 0x06005D6F RID: 23919 RVA: 0x001D9B7A File Offset: 0x001D7D7A
		public void AddSnapOverlap(SnapOverlap newOverlap)
		{
			if (this.firstOverlap == null)
			{
				this.firstOverlap = newOverlap;
			}
			else
			{
				newOverlap.nextOverlap = this.firstOverlap;
				this.firstOverlap = newOverlap;
			}
			this.SetConnected(newOverlap.bounds, true);
		}

		// Token: 0x06005D70 RID: 23920 RVA: 0x001D9BB0 File Offset: 0x001D7DB0
		public void RemoveSnapsWithDifferentRoot(BuilderPiece root, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (root == null || nextOverlap.otherPlane.piece.GetRootPiece() != root)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					snapOverlap3.otherPlane.RemoveSnapsWithPiece(this.piece, pool);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x06005D71 RID: 23921 RVA: 0x001D9CC8 File Offset: 0x001D7EC8
		public void RemoveSnapsWithPiece(BuilderPiece piece, BuilderPool pool)
		{
			if (this.firstOverlap == null)
			{
				return;
			}
			if (piece == null || pool == null)
			{
				return;
			}
			SnapOverlap snapOverlap = null;
			SnapOverlap nextOverlap = this.firstOverlap;
			while (nextOverlap != null)
			{
				if (nextOverlap.otherPlane == null || nextOverlap.otherPlane.piece == null)
				{
					SnapOverlap snapOverlap2 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap2.bounds, false);
					pool.DestroySnapOverlap(snapOverlap2);
				}
				else if (nextOverlap.otherPlane.piece == piece)
				{
					SnapOverlap snapOverlap3 = nextOverlap;
					if (snapOverlap == null)
					{
						this.firstOverlap = nextOverlap.nextOverlap;
						nextOverlap = this.firstOverlap;
					}
					else
					{
						snapOverlap.nextOverlap = nextOverlap.nextOverlap;
						nextOverlap = snapOverlap.nextOverlap;
					}
					this.SetConnected(snapOverlap3.bounds, false);
					pool.DestroySnapOverlap(snapOverlap3);
				}
				else
				{
					snapOverlap = nextOverlap;
					nextOverlap = nextOverlap.nextOverlap;
				}
			}
		}

		// Token: 0x06005D72 RID: 23922 RVA: 0x001D9DC8 File Offset: 0x001D7FC8
		private void SetConnected(SnapBounds bounds, bool connect)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 >= num3 || num4 < 0)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return;
					}
					this.connected[num4] = connect;
				}
			}
		}

		// Token: 0x06005D73 RID: 23923 RVA: 0x001D9E98 File Offset: 0x001D8098
		public bool IsConnected(SnapBounds bounds)
		{
			int num = this.width / 2 - ((this.width % 2 == 0) ? 1 : 0);
			int num2 = this.length / 2 - ((this.length % 2 == 0) ? 1 : 0);
			int num3 = this.connected.Length;
			for (int i = bounds.min.x; i <= bounds.max.x; i++)
			{
				for (int j = bounds.min.y; j <= bounds.max.y; j++)
				{
					int num4 = (num + i) * this.length + (j + num2);
					if (num4 < 0 || num4 >= num3)
					{
						if (this.piece != null)
						{
							int pieceId = this.piece.pieceId;
						}
						return false;
					}
					if (this.connected[num4])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06005D74 RID: 23924 RVA: 0x001D9F6C File Offset: 0x001D816C
		public void CalcGridOverlap(BuilderAttachGridPlane otherGridPlane, Vector3 otherPieceLocalPos, Quaternion otherPieceLocalRot, float gridSize, out Vector2Int min, out Vector2Int max)
		{
			int num = otherGridPlane.width;
			int num2 = otherGridPlane.length;
			Quaternion rotation = otherPieceLocalRot * otherGridPlane.pieceToGridRotation;
			Vector3 lossyScale = base.transform.lossyScale;
			otherPieceLocalPos.Scale(base.transform.lossyScale);
			Vector3 vector = otherPieceLocalPos + otherPieceLocalRot * otherGridPlane.pieceToGridPosition;
			if (Mathf.Abs(Vector3.Dot(rotation * Vector3.forward, Vector3.forward)) < 0.707f)
			{
				num = otherGridPlane.length;
				num2 = otherGridPlane.width;
			}
			float num3 = (num % 2 == 0) ? (gridSize / 2f) : 0f;
			float num4 = (num2 % 2 == 0) ? (gridSize / 2f) : 0f;
			float num5 = (this.width % 2 == 0) ? (gridSize / 2f) : 0f;
			float num6 = (this.length % 2 == 0) ? (gridSize / 2f) : 0f;
			float num7 = num3 - num5;
			float num8 = num4 - num6;
			int num9 = Mathf.RoundToInt((vector.x - num7) / gridSize);
			int num10 = Mathf.RoundToInt((vector.z - num8) / gridSize);
			int num11 = num9 + Mathf.FloorToInt((float)num / 2f);
			int num12 = num10 + Mathf.FloorToInt((float)num2 / 2f);
			int a = num11 - (num - 1);
			int a2 = num12 - (num2 - 1);
			int num13 = Mathf.FloorToInt((float)this.width / 2f);
			int num14 = Mathf.FloorToInt((float)this.length / 2f);
			int b = num13 - (this.width - 1);
			int b2 = num14 - (this.length - 1);
			min = new Vector2Int(Mathf.Max(a, b), Mathf.Max(a2, b2));
			max = new Vector2Int(Mathf.Min(num11, num13), Mathf.Min(num12, num14));
		}

		// Token: 0x06005D75 RID: 23925 RVA: 0x001DA130 File Offset: 0x001D8330
		public bool IsAttachedToMovingGrid()
		{
			return this.piece.state == BuilderPiece.State.AttachedAndPlaced && !this.piece.isBuiltIntoTable && (this.isMoving || (!(this.piece.parentPiece == null) && this.piece.parentAttachIndex >= 0 && this.piece.parentAttachIndex < this.piece.parentPiece.gridPlanes.Count && this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].IsAttachedToMovingGrid()));
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x001DA1D4 File Offset: 0x001D83D4
		public BuilderAttachGridPlane GetMovingParentGrid()
		{
			if (this.piece.isBuiltIntoTable)
			{
				return null;
			}
			if (this.movesOnPlace && this.movingPart != null && !this.movingPart.IsAnchoredToTable())
			{
				return this;
			}
			if (this.piece.parentPiece == null)
			{
				return null;
			}
			if (this.piece.parentAttachIndex < 0 || this.piece.parentAttachIndex >= this.piece.parentPiece.gridPlanes.Count)
			{
				return null;
			}
			return this.piece.parentPiece.gridPlanes[this.piece.parentAttachIndex].GetMovingParentGrid();
		}

		// Token: 0x04006BF8 RID: 27640
		[Tooltip("Are the snap points in this grid \"outies\"")]
		public bool male;

		// Token: 0x04006BF9 RID: 27641
		[Tooltip("(Optional) midpoint of the grid")]
		public Transform center;

		// Token: 0x04006BFA RID: 27642
		[Tooltip("number of snap points wide (local X-axis)")]
		public int width;

		// Token: 0x04006BFB RID: 27643
		[Tooltip("number of snap points long (local z-axis)")]
		public int length;

		// Token: 0x04006BFC RID: 27644
		[NonSerialized]
		public int gridPlaneDataIndex;

		// Token: 0x04006BFD RID: 27645
		[NonSerialized]
		public BuilderItem item;

		// Token: 0x04006BFE RID: 27646
		[NonSerialized]
		public BuilderPiece piece;

		// Token: 0x04006BFF RID: 27647
		[NonSerialized]
		public int attachIndex;

		// Token: 0x04006C00 RID: 27648
		[NonSerialized]
		public float boundingRadius;

		// Token: 0x04006C01 RID: 27649
		[NonSerialized]
		public Vector3 pieceToGridPosition;

		// Token: 0x04006C02 RID: 27650
		[NonSerialized]
		public Quaternion pieceToGridRotation;

		// Token: 0x04006C03 RID: 27651
		[NonSerialized]
		public bool[] connected;

		// Token: 0x04006C04 RID: 27652
		[NonSerialized]
		public SnapOverlap firstOverlap;

		// Token: 0x04006C05 RID: 27653
		[NonSerialized]
		public float widthOffset;

		// Token: 0x04006C06 RID: 27654
		[NonSerialized]
		public float lengthOffset;

		// Token: 0x04006C07 RID: 27655
		private int childPieceCount;

		// Token: 0x04006C08 RID: 27656
		[HideInInspector]
		public bool isMoving;

		// Token: 0x04006C09 RID: 27657
		[HideInInspector]
		public bool movesOnPlace;

		// Token: 0x04006C0A RID: 27658
		[HideInInspector]
		public BuilderMovingPart movingPart;
	}
}
