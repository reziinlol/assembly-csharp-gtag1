using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02001323 RID: 4899
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x06007B69 RID: 31593 RVA: 0x00284830 File Offset: 0x00282A30
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
