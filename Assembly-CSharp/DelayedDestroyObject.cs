using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class DelayedDestroyObject : MonoBehaviour
{
	// Token: 0x06000333 RID: 819 RVA: 0x00013569 File Offset: 0x00011769
	private void Start()
	{
		this._timeToDie = Time.time + this.lifetime;
	}

	// Token: 0x06000334 RID: 820 RVA: 0x0001357D File Offset: 0x0001177D
	private void LateUpdate()
	{
		if (Time.time >= this._timeToDie)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x040003D1 RID: 977
	public float lifetime = 10f;

	// Token: 0x040003D2 RID: 978
	private float _timeToDie;
}
