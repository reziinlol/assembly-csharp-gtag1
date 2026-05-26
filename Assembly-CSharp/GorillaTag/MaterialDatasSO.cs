using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001149 RID: 4425
	[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
	public class MaterialDatasSO : ScriptableObject
	{
		// Token: 0x0400800D RID: 32781
		public List<GTPlayer.MaterialData> datas;

		// Token: 0x0400800E RID: 32782
		public List<HashWrapper> surfaceEffects;
	}
}
