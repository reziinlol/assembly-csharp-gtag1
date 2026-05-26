using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020009C5 RID: 2501
public class ScienceExperimentSceneElements : MonoBehaviour
{
	// Token: 0x06004011 RID: 16401 RVA: 0x00156E93 File Offset: 0x00155093
	private void Awake()
	{
		ScienceExperimentManager.instance.InitElements(this);
	}

	// Token: 0x06004012 RID: 16402 RVA: 0x00156EA2 File Offset: 0x001550A2
	private void OnDestroy()
	{
		ScienceExperimentManager.instance.DeInitElements();
	}

	// Token: 0x04005095 RID: 20629
	public List<ScienceExperimentSceneElements.DisableByLiquidData> disableByLiquidList = new List<ScienceExperimentSceneElements.DisableByLiquidData>();

	// Token: 0x04005096 RID: 20630
	public ParticleSystem sodaFizzParticles;

	// Token: 0x04005097 RID: 20631
	public ParticleSystem sodaEruptionParticles;

	// Token: 0x020009C6 RID: 2502
	[Serializable]
	public struct DisableByLiquidData
	{
		// Token: 0x04005098 RID: 20632
		public Transform target;

		// Token: 0x04005099 RID: 20633
		public float heightOffset;
	}
}
