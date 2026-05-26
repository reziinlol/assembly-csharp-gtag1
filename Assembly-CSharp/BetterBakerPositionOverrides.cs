using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D2C RID: 3372
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x0400646D RID: 25709
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000D2D RID: 3373
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x0400646E RID: 25710
		public GameObject go;

		// Token: 0x0400646F RID: 25711
		public Transform bakingTransform;

		// Token: 0x04006470 RID: 25712
		public Transform gameTransform;
	}
}
