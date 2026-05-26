using System;
using UnityEngine;

// Token: 0x02000607 RID: 1543
public class BuilderPaintBucket : MonoBehaviour
{
	// Token: 0x06002688 RID: 9864 RVA: 0x000CC2D8 File Offset: 0x000CA4D8
	private void Awake()
	{
		if (string.IsNullOrEmpty(this.materialId))
		{
			return;
		}
		this.materialType = this.materialId.GetHashCode();
		if (this.bucketMaterialOptions != null && this.paintBucketRenderer != null)
		{
			Material material;
			int num;
			this.bucketMaterialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material != null)
			{
				this.paintBucketRenderer.material = material;
			}
		}
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000CC34C File Offset: 0x000CA54C
	private void OnTriggerEnter(Collider other)
	{
		if (this.materialType == -1)
		{
			return;
		}
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			BuilderPaintBrush component = attachedRigidbody.GetComponent<BuilderPaintBrush>();
			if (component != null)
			{
				component.SetBrushMaterial(this.materialType);
			}
		}
	}

	// Token: 0x040031F8 RID: 12792
	[SerializeField]
	private BuilderMaterialOptions bucketMaterialOptions;

	// Token: 0x040031F9 RID: 12793
	[SerializeField]
	private MeshRenderer paintBucketRenderer;

	// Token: 0x040031FA RID: 12794
	[SerializeField]
	private string materialId;

	// Token: 0x040031FB RID: 12795
	private int materialType = -1;
}
