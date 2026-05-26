using System;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class SIBlasterExplosion : MonoBehaviour
{
	// Token: 0x06000510 RID: 1296 RVA: 0x0001C50E File Offset: 0x0001A70E
	private void OnDisable()
	{
		SIGadgetBlasterProjectile.DespawnExplosion(base.gameObject);
	}
}
