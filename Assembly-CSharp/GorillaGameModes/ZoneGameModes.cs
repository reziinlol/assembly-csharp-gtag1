using System;

namespace GorillaGameModes
{
	// Token: 0x02000EAF RID: 3759
	[Serializable]
	public struct ZoneGameModes
	{
		// Token: 0x04006ACE RID: 27342
		public GTZone[] zone;

		// Token: 0x04006ACF RID: 27343
		public GameModeType[] modes;

		// Token: 0x04006AD0 RID: 27344
		public GameModeType[] privateModes;
	}
}
