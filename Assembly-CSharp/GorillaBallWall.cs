using System;
using UnityEngine;

// Token: 0x02000848 RID: 2120
public class GorillaBallWall : MonoBehaviour
{
	// Token: 0x060036F2 RID: 14066 RVA: 0x0012E0BC File Offset: 0x0012C2BC
	private void Awake()
	{
		if (GorillaBallWall.instance == null)
		{
			GorillaBallWall.instance = this;
			return;
		}
		if (GorillaBallWall.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x04004704 RID: 18180
	[OnEnterPlay_SetNull]
	public static volatile GorillaBallWall instance;
}
