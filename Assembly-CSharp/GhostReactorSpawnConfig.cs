using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000712 RID: 1810
[CreateAssetMenu(fileName = "GhostReactorSpawnConfig", menuName = "ScriptableObjects/GhostReactorSpawnConfig")]
public class GhostReactorSpawnConfig : ScriptableObject
{
	// Token: 0x04003ACC RID: 15052
	public List<GhostReactorSpawnConfig.EntitySpawnGroup> entitySpawnGroups;

	// Token: 0x02000713 RID: 1811
	public enum SpawnPointType
	{
		// Token: 0x04003ACE RID: 15054
		Enemy,
		// Token: 0x04003ACF RID: 15055
		Collectible,
		// Token: 0x04003AD0 RID: 15056
		Barrier,
		// Token: 0x04003AD1 RID: 15057
		HazardLiquid,
		// Token: 0x04003AD2 RID: 15058
		Phantom,
		// Token: 0x04003AD3 RID: 15059
		Pest,
		// Token: 0x04003AD4 RID: 15060
		Crate,
		// Token: 0x04003AD5 RID: 15061
		Tool,
		// Token: 0x04003AD6 RID: 15062
		ChaosSeed,
		// Token: 0x04003AD7 RID: 15063
		HazardTower,
		// Token: 0x04003AD8 RID: 15064
		MiniBoss,
		// Token: 0x04003AD9 RID: 15065
		SpawnPointTypeCount
	}

	// Token: 0x02000714 RID: 1812
	[Serializable]
	public struct EntitySpawnGroup
	{
		// Token: 0x04003ADA RID: 15066
		public GhostReactorSpawnConfig.SpawnPointType spawnPointType;

		// Token: 0x04003ADB RID: 15067
		public GameEntity entity;

		// Token: 0x04003ADC RID: 15068
		public GRBreakableItemSpawnConfig randomEntity;

		// Token: 0x04003ADD RID: 15069
		public int spawnCount;
	}
}
