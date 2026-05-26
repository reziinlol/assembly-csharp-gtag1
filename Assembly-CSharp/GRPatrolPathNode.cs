using System;
using UnityEngine;

// Token: 0x020007B6 RID: 1974
public class GRPatrolPathNode : MonoBehaviour
{
	// Token: 0x0600323D RID: 12861 RVA: 0x00113DAC File Offset: 0x00111FAC
	public void OnDrawGizmosSelected()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		GRPatrolPath component = base.transform.parent.GetComponent<GRPatrolPath>();
		if (component == null)
		{
			return;
		}
		component.OnDrawGizmosSelected();
	}
}
