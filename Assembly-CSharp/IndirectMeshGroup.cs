using System;
using UnityEngine;

// Token: 0x02000374 RID: 884
public sealed class IndirectMeshGroup : MonoBehaviour
{
	// Token: 0x060015A8 RID: 5544 RVA: 0x0007251C File Offset: 0x0007071C
	private void OnEnable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), true);
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x0007252A File Offset: 0x0007072A
	private void OnDisable()
	{
		IndirectMeshRenderer.SetGroupVisible(base.GetInstanceID(), false);
	}
}
