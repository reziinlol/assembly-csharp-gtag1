using System;
using UnityEngine;

// Token: 0x020005A8 RID: 1448
public class DestroyIfNotBeta : MonoBehaviour
{
	// Token: 0x060024A4 RID: 9380 RVA: 0x000C47F9 File Offset: 0x000C29F9
	private void Awake()
	{
		bool shouldKeepIfBeta = this.m_shouldKeepIfBeta;
		bool shouldKeepIfCreatorBuild = this.m_shouldKeepIfCreatorBuild;
		Object.Destroy(base.gameObject);
	}

	// Token: 0x04003001 RID: 12289
	public bool m_shouldKeepIfBeta = true;

	// Token: 0x04003002 RID: 12290
	public bool m_shouldKeepIfCreatorBuild;
}
