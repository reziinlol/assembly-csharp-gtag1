using System;
using System.Collections.Generic;

namespace GorillaTagScripts
{
	// Token: 0x02000EF6 RID: 3830
	[Serializable]
	public class BuilderTableData
	{
		// Token: 0x06005F33 RID: 24371 RVA: 0x001EA634 File Offset: 0x001E8834
		public BuilderTableData()
		{
			this.version = 4;
			this.numEdits = 0;
			this.numPieces = 0;
			this.pieceType = new List<int>(1024);
			this.pieceId = new List<int>(1024);
			this.parentId = new List<int>(1024);
			this.attachIndex = new List<int>(1024);
			this.parentAttachIndex = new List<int>(1024);
			this.placement = new List<int>(1024);
			this.materialType = new List<int>(1024);
			this.overlapingPieces = new List<int>(1024);
			this.overlappedPieces = new List<int>(1024);
			this.overlapInfo = new List<long>(1024);
			this.timeOffset = new List<int>(1024);
		}

		// Token: 0x06005F34 RID: 24372 RVA: 0x001EA70C File Offset: 0x001E890C
		public void Clear()
		{
			this.numPieces = 0;
			this.pieceType.Clear();
			this.pieceId.Clear();
			this.parentId.Clear();
			this.attachIndex.Clear();
			this.parentAttachIndex.Clear();
			this.placement.Clear();
			this.materialType.Clear();
			this.overlapingPieces.Clear();
			this.overlappedPieces.Clear();
			this.overlapInfo.Clear();
			this.timeOffset.Clear();
		}

		// Token: 0x04006DE9 RID: 28137
		public const int BUILDER_TABLE_DATA_VERSION = 4;

		// Token: 0x04006DEA RID: 28138
		public int version;

		// Token: 0x04006DEB RID: 28139
		public int numEdits;

		// Token: 0x04006DEC RID: 28140
		public int numPieces;

		// Token: 0x04006DED RID: 28141
		public List<int> pieceType;

		// Token: 0x04006DEE RID: 28142
		public List<int> pieceId;

		// Token: 0x04006DEF RID: 28143
		public List<int> parentId;

		// Token: 0x04006DF0 RID: 28144
		public List<int> attachIndex;

		// Token: 0x04006DF1 RID: 28145
		public List<int> parentAttachIndex;

		// Token: 0x04006DF2 RID: 28146
		public List<int> placement;

		// Token: 0x04006DF3 RID: 28147
		public List<int> materialType;

		// Token: 0x04006DF4 RID: 28148
		public List<int> overlapingPieces;

		// Token: 0x04006DF5 RID: 28149
		public List<int> overlappedPieces;

		// Token: 0x04006DF6 RID: 28150
		public List<long> overlapInfo;

		// Token: 0x04006DF7 RID: 28151
		public List<int> timeOffset;
	}
}
