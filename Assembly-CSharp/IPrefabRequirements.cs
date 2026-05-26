using System;
using System.Collections.Generic;

// Token: 0x020000FC RID: 252
public interface IPrefabRequirements
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x060005E8 RID: 1512
	IEnumerable<GameEntity> RequiredPrefabs { get; }
}
