using System;
using UnityEngine;

// Token: 0x02000D06 RID: 3334
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x170007CC RID: 1996
	// (get) Token: 0x0600529D RID: 21149 RVA: 0x001B1EE6 File Offset: 0x001B00E6
	// (set) Token: 0x0600529E RID: 21150 RVA: 0x001B1EEE File Offset: 0x001B00EE
	public bool PreTickRunning { get; set; }

	// Token: 0x0600529F RID: 21151 RVA: 0x001B1EF7 File Offset: 0x001B00F7
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x060052A0 RID: 21152 RVA: 0x001B1EFF File Offset: 0x001B00FF
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x060052A1 RID: 21153 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PreTick()
	{
	}
}
