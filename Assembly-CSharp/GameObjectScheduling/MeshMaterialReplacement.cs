using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02001330 RID: 4912
	[CreateAssetMenu(fileName = "New Mesh Material Replacement", menuName = "Game Object Scheduling/New Mesh Material Replacement", order = 1)]
	public class MeshMaterialReplacement : ScriptableObject
	{
		// Token: 0x04008CFE RID: 36094
		public Mesh mesh;

		// Token: 0x04008CFF RID: 36095
		public Material[] materials;
	}
}
