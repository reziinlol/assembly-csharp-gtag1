using System;
using UnityEngine;

// Token: 0x02000CFE RID: 3326
public abstract class MonoBehaviourPostTick : MonoBehaviour, ITickSystemPost
{
	// Token: 0x170007C7 RID: 1991
	// (get) Token: 0x0600526E RID: 21102 RVA: 0x001B1AB6 File Offset: 0x001AFCB6
	// (set) Token: 0x0600526F RID: 21103 RVA: 0x001B1ABE File Offset: 0x001AFCBE
	public bool PostTickRunning { get; set; }

	// Token: 0x06005270 RID: 21104 RVA: 0x001A578D File Offset: 0x001A398D
	public void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06005271 RID: 21105 RVA: 0x00156E8B File Offset: 0x0015508B
	public void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06005272 RID: 21106
	public abstract void PostTick();
}
