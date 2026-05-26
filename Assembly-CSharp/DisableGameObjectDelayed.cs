using System;
using UnityEngine;

// Token: 0x02000D48 RID: 3400
public class DisableGameObjectDelayed : MonoBehaviour
{
	// Token: 0x060053B5 RID: 21429 RVA: 0x001B6449 File Offset: 0x001B4649
	private void OnEnable()
	{
		this.enabledTime = Time.time;
	}

	// Token: 0x060053B6 RID: 21430 RVA: 0x001B6456 File Offset: 0x001B4656
	private void Update()
	{
		if (Time.time > this.enabledTime + this.delayTime)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x060053B7 RID: 21431 RVA: 0x001B6478 File Offset: 0x001B4678
	public void EnableAndResetTimer()
	{
		base.gameObject.SetActive(true);
		this.OnEnable();
	}

	// Token: 0x040064D3 RID: 25811
	public float delayTime = 1f;

	// Token: 0x040064D4 RID: 25812
	public float enabledTime;
}
