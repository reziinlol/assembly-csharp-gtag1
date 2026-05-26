using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200075C RID: 1884
[CreateAssetMenu(fileName = "GhostReactorDropTableOverrides", menuName = "ScriptableObjects/GhostReactorDropTableOverride")]
public class GRDropTableOverrides : ScriptableObject
{
	// Token: 0x06002FC3 RID: 12227 RVA: 0x00103760 File Offset: 0x00101960
	public GRBreakableItemSpawnConfig GetOverride(GRBreakableItemSpawnConfig table)
	{
		for (int i = 0; i < this.overrides.Count; i++)
		{
			if (this.overrides[i].table == table)
			{
				return this.overrides[i].overrideTable;
			}
		}
		return null;
	}

	// Token: 0x04003D32 RID: 15666
	public List<GRDropTableOverrides.DropTableOverride> overrides;

	// Token: 0x0200075D RID: 1885
	[Serializable]
	public class DropTableOverride
	{
		// Token: 0x04003D33 RID: 15667
		public GRBreakableItemSpawnConfig table;

		// Token: 0x04003D34 RID: 15668
		public GRBreakableItemSpawnConfig overrideTable;
	}
}
