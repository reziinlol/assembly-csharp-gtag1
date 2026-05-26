using System;
using UnityEngine;

// Token: 0x02000D05 RID: 3333
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x170007C9 RID: 1993
	// (get) Token: 0x06005291 RID: 21137 RVA: 0x001B1EA3 File Offset: 0x001B00A3
	// (set) Token: 0x06005292 RID: 21138 RVA: 0x001B1EAB File Offset: 0x001B00AB
	public bool PreTickRunning { get; set; }

	// Token: 0x170007CA RID: 1994
	// (get) Token: 0x06005293 RID: 21139 RVA: 0x001B1EB4 File Offset: 0x001B00B4
	// (set) Token: 0x06005294 RID: 21140 RVA: 0x001B1EBC File Offset: 0x001B00BC
	public bool TickRunning { get; set; }

	// Token: 0x170007CB RID: 1995
	// (get) Token: 0x06005295 RID: 21141 RVA: 0x001B1EC5 File Offset: 0x001B00C5
	// (set) Token: 0x06005296 RID: 21142 RVA: 0x001B1ECD File Offset: 0x001B00CD
	public bool PostTickRunning { get; set; }

	// Token: 0x06005297 RID: 21143 RVA: 0x001B1ED6 File Offset: 0x001B00D6
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x06005298 RID: 21144 RVA: 0x001B1EDE File Offset: 0x001B00DE
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x06005299 RID: 21145 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PreTick()
	{
	}

	// Token: 0x0600529A RID: 21146 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void Tick()
	{
	}

	// Token: 0x0600529B RID: 21147 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void PostTick()
	{
	}
}
