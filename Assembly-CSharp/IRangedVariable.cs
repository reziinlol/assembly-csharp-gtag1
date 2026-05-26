using System;
using UnityEngine;

// Token: 0x020008F1 RID: 2289
public interface IRangedVariable<T> : IVariable<T>, IVariable
{
	// Token: 0x17000559 RID: 1369
	// (get) Token: 0x06003BE0 RID: 15328
	// (set) Token: 0x06003BE1 RID: 15329
	T Min { get; set; }

	// Token: 0x1700055A RID: 1370
	// (get) Token: 0x06003BE2 RID: 15330
	// (set) Token: 0x06003BE3 RID: 15331
	T Max { get; set; }

	// Token: 0x1700055B RID: 1371
	// (get) Token: 0x06003BE4 RID: 15332
	T Range { get; }

	// Token: 0x1700055C RID: 1372
	// (get) Token: 0x06003BE5 RID: 15333
	AnimationCurve Curve { get; }
}
