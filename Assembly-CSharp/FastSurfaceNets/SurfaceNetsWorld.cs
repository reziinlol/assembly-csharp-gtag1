using System;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x020012C4 RID: 4804
	public class SurfaceNetsWorld : MonoBehaviour
	{
		// Token: 0x06007819 RID: 30745 RVA: 0x00276F9C File Offset: 0x0027519C
		private void Awake()
		{
			this.Generate();
		}

		// Token: 0x0600781A RID: 30746 RVA: 0x00276FA4 File Offset: 0x002751A4
		private void Generate()
		{
			this.DestroyChildren();
			for (int i = -this.radius.x; i <= this.radius.x; i++)
			{
				for (int j = -this.radius.y; j <= this.radius.y; j++)
				{
					for (int k = -this.radius.z; k <= this.radius.z; k++)
					{
						int3 @int = new int3(i, j, k);
						SurfaceNetsChunk surfaceNetsChunk = Object.Instantiate<SurfaceNetsChunk>(this.chunkPrefab, base.transform);
						surfaceNetsChunk.Id = @int;
						surfaceNetsChunk.parameters = this.parameters;
						surfaceNetsChunk.name = string.Format("SurfaceNetsChunk_{0}_{1}_{2}", @int.x, @int.y, @int.z);
						surfaceNetsChunk.transform.localPosition = @int.ToFloat3() * 32f;
						surfaceNetsChunk.BuildChunk();
					}
				}
			}
		}

		// Token: 0x0600781B RID: 30747 RVA: 0x002770B0 File Offset: 0x002752B0
		private void DestroyChildren()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				if (child != null)
				{
					JamUtil.Destroy(child.gameObject);
				}
			}
		}

		// Token: 0x04008B1B RID: 35611
		public SurfaceNetsChunk chunkPrefab;

		// Token: 0x04008B1C RID: 35612
		public int3 radius;

		// Token: 0x04008B1D RID: 35613
		public GenerationParameters parameters;
	}
}
