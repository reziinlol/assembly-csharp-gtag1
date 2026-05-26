using System;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Voxels
{
	// Token: 0x020012D6 RID: 4822
	public struct MeshVertexData
	{
		// Token: 0x06007880 RID: 30848 RVA: 0x002793FB File Offset: 0x002775FB
		public MeshVertexData(float3 position, float3 normal, float4 tangent, float4 materials, float4 blend)
		{
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
			this.materials = materials;
			this.blend = blend;
		}

		// Token: 0x06007881 RID: 30849 RVA: 0x00279422 File Offset: 0x00277622
		public override string ToString()
		{
			return string.Format("({0:F2} x {1:F2})", this.position, this.normal);
		}

		// Token: 0x04008B79 RID: 35705
		public float3 position;

		// Token: 0x04008B7A RID: 35706
		public float3 normal;

		// Token: 0x04008B7B RID: 35707
		public float4 tangent;

		// Token: 0x04008B7C RID: 35708
		public float4 materials;

		// Token: 0x04008B7D RID: 35709
		public float4 blend;

		// Token: 0x04008B7E RID: 35710
		public static readonly VertexAttributeDescriptor[] VertexBufferMemoryLayout = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float32, 4, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float32, 4, 0)
		};
	}
}
