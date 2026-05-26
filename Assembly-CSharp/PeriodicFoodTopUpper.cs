using System;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class PeriodicFoodTopUpper : MonoBehaviour
{
	// Token: 0x06000386 RID: 902 RVA: 0x000148B8 File Offset: 0x00012AB8
	private void Awake()
	{
		this.food = base.GetComponentInParent<CrittersFood>();
	}

	// Token: 0x06000387 RID: 903 RVA: 0x000148C8 File Offset: 0x00012AC8
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.waitingToRefill && this.food.currentFood == 0f)
		{
			this.waitingToRefill = true;
			this.timeFoodEmpty = Time.time;
		}
		if (this.waitingToRefill && Time.time > this.timeFoodEmpty + this.waitToRefill)
		{
			this.waitingToRefill = false;
			this.food.Initialize();
		}
	}

	// Token: 0x04000405 RID: 1029
	private CrittersFood food;

	// Token: 0x04000406 RID: 1030
	private float timeFoodEmpty;

	// Token: 0x04000407 RID: 1031
	private bool waitingToRefill;

	// Token: 0x04000408 RID: 1032
	public float waitToRefill = 10f;

	// Token: 0x04000409 RID: 1033
	public GameObject foodObject;
}
