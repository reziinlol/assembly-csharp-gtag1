using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class BeeAvoidPoint : MonoBehaviour
{
	// Token: 0x06000E3A RID: 3642 RVA: 0x0004E843 File Offset: 0x0004CA43
	private void Start()
	{
		BeeSwarmManager.RegisterAvoidPoint(base.gameObject);
		FlockingManager.RegisterAvoidPoint(base.gameObject);
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0004E85B File Offset: 0x0004CA5B
	private void OnDestroy()
	{
		BeeSwarmManager.UnregisterAvoidPoint(base.gameObject);
		FlockingManager.UnregisterAvoidPoint(base.gameObject);
	}
}
