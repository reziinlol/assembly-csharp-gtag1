using System;

namespace GorillaTagScripts
{
	// Token: 0x02000EEB RID: 3819
	public struct BuilderPieceData
	{
		// Token: 0x06005ED4 RID: 24276 RVA: 0x001E6E50 File Offset: 0x001E5050
		public BuilderPieceData(BuilderPiece piece)
		{
			this.pieceId = piece.pieceId;
			this.pieceIndex = piece.pieceDataIndex;
			BuilderPiece parentPiece = piece.parentPiece;
			this.parentPieceIndex = ((parentPiece == null) ? -1 : parentPiece.pieceDataIndex);
			BuilderPiece requestedParentPiece = piece.requestedParentPiece;
			this.requestedParentPieceIndex = ((requestedParentPiece == null) ? -1 : requestedParentPiece.pieceDataIndex);
			this.preventSnapUntilMoved = piece.preventSnapUntilMoved;
			this.isBuiltIntoTable = piece.isBuiltIntoTable;
			this.state = piece.state;
			this.privatePlotIndex = piece.privatePlotIndex;
			this.isArmPiece = piece.isArmShelf;
			this.heldByActorNumber = piece.heldByPlayerActorNumber;
		}

		// Token: 0x04006D7E RID: 28030
		public int pieceId;

		// Token: 0x04006D7F RID: 28031
		public int pieceIndex;

		// Token: 0x04006D80 RID: 28032
		public int parentPieceIndex;

		// Token: 0x04006D81 RID: 28033
		public int requestedParentPieceIndex;

		// Token: 0x04006D82 RID: 28034
		public int heldByActorNumber;

		// Token: 0x04006D83 RID: 28035
		public int preventSnapUntilMoved;

		// Token: 0x04006D84 RID: 28036
		public bool isBuiltIntoTable;

		// Token: 0x04006D85 RID: 28037
		public BuilderPiece.State state;

		// Token: 0x04006D86 RID: 28038
		public int privatePlotIndex;

		// Token: 0x04006D87 RID: 28039
		public bool isArmPiece;
	}
}
