using System;
using UnityEngine;

// Token: 0x02000CFD RID: 3325
public abstract class MonoBehaviourTick : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170007C6 RID: 1990
	// (get) Token: 0x06005268 RID: 21096 RVA: 0x001B1AA5 File Offset: 0x001AFCA5
	// (set) Token: 0x06005269 RID: 21097 RVA: 0x001B1AAD File Offset: 0x001AFCAD
	public bool TickRunning { get; set; }

	// Token: 0x0600526A RID: 21098 RVA: 0x00019E3F File Offset: 0x0001803F
	public void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600526B RID: 21099 RVA: 0x00019E47 File Offset: 0x00018047
	public void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0600526C RID: 21100
	public abstract void Tick();
}
