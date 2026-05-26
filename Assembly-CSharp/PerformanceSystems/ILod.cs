using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000EA4 RID: 3748
	public interface ILod
	{
		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06005C0A RID: 23562
		int CurrentLod { get; }

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x06005C0B RID: 23563
		Vector3 Position { get; }

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06005C0C RID: 23564
		float[] LodRanges { get; }

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06005C0D RID: 23565
		UnityEvent[] OnLodRangeEvents { get; }

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06005C0E RID: 23566
		UnityEvent OnCulledEvent { get; }

		// Token: 0x06005C0F RID: 23567
		void UpdateLod(Vector3 refPos);
	}
}
