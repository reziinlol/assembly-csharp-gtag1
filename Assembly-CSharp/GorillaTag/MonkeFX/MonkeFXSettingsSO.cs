using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x020011A9 RID: 4521
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x06007251 RID: 29265 RVA: 0x002539FE File Offset: 0x00251BFE
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x04008231 RID: 33329
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x04008232 RID: 33330
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
