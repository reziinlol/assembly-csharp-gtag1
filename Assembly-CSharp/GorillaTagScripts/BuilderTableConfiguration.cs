using System;

namespace GorillaTagScripts
{
	// Token: 0x02000EDD RID: 3805
	[Serializable]
	public class BuilderTableConfiguration
	{
		// Token: 0x06005DD3 RID: 24019 RVA: 0x001DC4E4 File Offset: 0x001DA6E4
		public BuilderTableConfiguration()
		{
			this.version = 0;
			this.TableResourceLimits = new int[3];
			this.PlotResourceLimits = new int[3];
			this.updateCountdownDate = string.Empty;
		}

		// Token: 0x04006C6D RID: 27757
		public const int CONFIGURATION_VERSION = 0;

		// Token: 0x04006C6E RID: 27758
		public int version;

		// Token: 0x04006C6F RID: 27759
		public int[] TableResourceLimits;

		// Token: 0x04006C70 RID: 27760
		public int[] PlotResourceLimits;

		// Token: 0x04006C71 RID: 27761
		public int DroppedPieceLimit;

		// Token: 0x04006C72 RID: 27762
		public string updateCountdownDate;
	}
}
