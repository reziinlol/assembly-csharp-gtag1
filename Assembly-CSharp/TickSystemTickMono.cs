using System;
using UnityEngine;

// Token: 0x02000D07 RID: 3335
internal abstract class TickSystemTickMono : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170007CD RID: 1997
	// (get) Token: 0x060052A3 RID: 21155 RVA: 0x001B1F07 File Offset: 0x001B0107
	// (set) Token: 0x060052A4 RID: 21156 RVA: 0x001B1F0F File Offset: 0x001B010F
	public bool TickRunning { get; set; }

	// Token: 0x060052A5 RID: 21157 RVA: 0x00019E3F File Offset: 0x0001803F
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060052A6 RID: 21158 RVA: 0x00019E47 File Offset: 0x00018047
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060052A7 RID: 21159 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void Tick()
	{
	}
}
