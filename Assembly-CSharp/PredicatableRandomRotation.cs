using System;
using UnityEngine;

// Token: 0x02000D78 RID: 3448
public class PredicatableRandomRotation : MonoBehaviour
{
	// Token: 0x0600548B RID: 21643 RVA: 0x001B8FE4 File Offset: 0x001B71E4
	private void Start()
	{
		if (this.source == null)
		{
			this.source = base.transform;
		}
	}

	// Token: 0x0600548C RID: 21644 RVA: 0x001B9000 File Offset: 0x001B7200
	private void Update()
	{
		float d = (this.source.position.x * this.source.position.x + this.source.position.y * this.source.position.y + this.source.position.z * this.source.position.z) % 1f;
		base.transform.Rotate(this.rot * d);
	}

	// Token: 0x0400652B RID: 25899
	[SerializeField]
	private Vector3 rot = Vector3.zero;

	// Token: 0x0400652C RID: 25900
	[SerializeField]
	private Transform source;
}
