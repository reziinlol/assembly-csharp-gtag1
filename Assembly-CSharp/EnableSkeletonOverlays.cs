using System;
using UnityEngine;

// Token: 0x0200032C RID: 812
public class EnableSkeletonOverlays : MonoBehaviour
{
	// Token: 0x06001418 RID: 5144 RVA: 0x0006C71A File Offset: 0x0006A91A
	private void OnEnable()
	{
		Shader.SetGlobalFloat(this._BlackAndWhite, 1f);
		GorillaBodyRenderer.EnableSkeletonOverlays(this.bodyMaterial, this.skeletonMaterial);
	}

	// Token: 0x06001419 RID: 5145 RVA: 0x0006C742 File Offset: 0x0006A942
	private void OnDisable()
	{
		Shader.SetGlobalFloat(this._BlackAndWhite, 0f);
		GorillaBodyRenderer.DisableSkeletonOverlays();
	}

	// Token: 0x040018EC RID: 6380
	[SerializeField]
	private Material bodyMaterial;

	// Token: 0x040018ED RID: 6381
	[SerializeField]
	private Material skeletonMaterial;

	// Token: 0x040018EE RID: 6382
	private ShaderHashId _BlackAndWhite = "_GreyZoneActive";
}
