using System;
using UnityEngine;

// Token: 0x020005B2 RID: 1458
public class DisableScreamer : MonoBehaviour
{
	// Token: 0x060024C1 RID: 9409 RVA: 0x000C4E6D File Offset: 0x000C306D
	private void OnDisable()
	{
		Debug.LogError("oh my god i've been disabled! aaag!!! AAAAAAAAA!!!!", base.gameObject);
	}
}
