using System;
using System.Collections.Generic;
using GorillaGameModes;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010D8 RID: 4312
	[Serializable]
	public class ModeTagEffect
	{
		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06006BF6 RID: 27638 RVA: 0x0022F34A File Offset: 0x0022D54A
		public HashSet<GameModeType> Modes
		{
			get
			{
				if (this.modesHash == null)
				{
					this.modesHash = new HashSet<GameModeType>(this.modes);
				}
				return this.modesHash;
			}
		}

		// Token: 0x04007C26 RID: 31782
		[SerializeField]
		private GameModeType[] modes;

		// Token: 0x04007C27 RID: 31783
		private HashSet<GameModeType> modesHash;

		// Token: 0x04007C28 RID: 31784
		public TagEffectPack tagEffect;

		// Token: 0x04007C29 RID: 31785
		public bool blockTagOverride;

		// Token: 0x04007C2A RID: 31786
		public bool blockFistBumpOverride;

		// Token: 0x04007C2B RID: 31787
		public bool blockHiveFiveOverride;
	}
}
