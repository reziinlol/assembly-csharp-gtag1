using System;

namespace GorillaTagScripts
{
	// Token: 0x02000EED RID: 3821
	public struct BuilderPrivatePlotData
	{
		// Token: 0x06005ED5 RID: 24277 RVA: 0x001E6EFB File Offset: 0x001E50FB
		public BuilderPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			this.plotState = plot.plotState;
			this.ownerActorNumber = plot.GetOwnerActorNumber();
			this.isUnderCapacityLeft = false;
			this.isUnderCapacityRight = false;
		}

		// Token: 0x04006D8A RID: 28042
		public BuilderPiecePrivatePlot.PlotState plotState;

		// Token: 0x04006D8B RID: 28043
		public int ownerActorNumber;

		// Token: 0x04006D8C RID: 28044
		public bool isUnderCapacityLeft;

		// Token: 0x04006D8D RID: 28045
		public bool isUnderCapacityRight;
	}
}
