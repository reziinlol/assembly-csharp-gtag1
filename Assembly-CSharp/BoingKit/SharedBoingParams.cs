using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001387 RID: 4999
	[CreateAssetMenu(fileName = "BoingParams", menuName = "Boing Kit/Shared Boing Params", order = 550)]
	public class SharedBoingParams : ScriptableObject
	{
		// Token: 0x06007DD5 RID: 32213 RVA: 0x002965A4 File Offset: 0x002947A4
		public SharedBoingParams()
		{
			this.Params.Init();
		}

		// Token: 0x04008F4E RID: 36686
		public BoingWork.Params Params;
	}
}
