using System;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012C7 RID: 4807
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	public class ChunkComponent : MonoBehaviour
	{
		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x0600783B RID: 30779 RVA: 0x0027784E File Offset: 0x00275A4E
		// (set) Token: 0x0600783C RID: 30780 RVA: 0x00277856 File Offset: 0x00275A56
		public VoxelWorld World { get; set; }

		// Token: 0x0600783D RID: 30781 RVA: 0x0027785F File Offset: 0x00275A5F
		private void Reset()
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.meshRenderer = base.GetComponent<MeshRenderer>();
			this.meshCollider = base.GetComponent<MeshCollider>();
		}

		// Token: 0x04008B40 RID: 35648
		public MeshFilter meshFilter;

		// Token: 0x04008B41 RID: 35649
		public MeshRenderer meshRenderer;

		// Token: 0x04008B42 RID: 35650
		public MeshCollider meshCollider;
	}
}
