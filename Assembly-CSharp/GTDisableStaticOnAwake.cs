using System;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class GTDisableStaticOnAwake : MonoBehaviour
{
	// Token: 0x0600148E RID: 5262 RVA: 0x0006D6E4 File Offset: 0x0006B8E4
	private void Awake()
	{
		base.gameObject.isStatic = false;
		Object.Destroy(this);
	}
}
