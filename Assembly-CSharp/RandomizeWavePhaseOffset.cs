using System;
using UnityEngine;

// Token: 0x02000224 RID: 548
public class RandomizeWavePhaseOffset : MonoBehaviour
{
	// Token: 0x06000E88 RID: 3720 RVA: 0x0004F228 File Offset: 0x0004D428
	private void Start()
	{
		Material material = base.GetComponent<MeshRenderer>().material;
		UberShader.VertexWavePhaseOffset.SetValue<float>(material, Random.Range(this.minPhaseOffset, this.maxPhaseOffset));
	}

	// Token: 0x0400116D RID: 4461
	[SerializeField]
	private float minPhaseOffset;

	// Token: 0x0400116E RID: 4462
	[SerializeField]
	private float maxPhaseOffset;
}
