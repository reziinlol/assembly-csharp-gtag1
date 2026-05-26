using System;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public class PuppetFollow : MonoBehaviour
{
	// Token: 0x06001E81 RID: 7809 RVA: 0x000A2D74 File Offset: 0x000A0F74
	private void FixedUpdate()
	{
		base.transform.position = this.sourceTarget.position - this.sourceBase.position + this.puppetBase.position;
		base.transform.localRotation = this.sourceTarget.localRotation;
	}

	// Token: 0x040028C2 RID: 10434
	public Transform sourceTarget;

	// Token: 0x040028C3 RID: 10435
	public Transform sourceBase;

	// Token: 0x040028C4 RID: 10436
	public Transform puppetBase;
}
