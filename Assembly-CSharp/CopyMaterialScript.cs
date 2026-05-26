using System;
using UnityEngine;

// Token: 0x02000CC9 RID: 3273
public class CopyMaterialScript : MonoBehaviour
{
	// Token: 0x06005155 RID: 20821 RVA: 0x001AD0EE File Offset: 0x001AB2EE
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06005156 RID: 20822 RVA: 0x001AD105 File Offset: 0x001AB305
	private void Update()
	{
		if (this.sourceToCopyMaterialFrom.material != this.mySkinnedMeshRenderer.material)
		{
			this.mySkinnedMeshRenderer.material = this.sourceToCopyMaterialFrom.material;
		}
	}

	// Token: 0x040062B1 RID: 25265
	public SkinnedMeshRenderer sourceToCopyMaterialFrom;

	// Token: 0x040062B2 RID: 25266
	public SkinnedMeshRenderer mySkinnedMeshRenderer;
}
