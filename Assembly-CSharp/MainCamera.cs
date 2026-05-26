using System;
using UnityEngine;

// Token: 0x02000AE1 RID: 2785
public class MainCamera : MonoBehaviourStatic<MainCamera>
{
	// Token: 0x06004707 RID: 18183 RVA: 0x0017F83D File Offset: 0x0017DA3D
	public static implicit operator Camera(MainCamera mc)
	{
		return mc.camera;
	}

	// Token: 0x04005997 RID: 22935
	public Camera camera;
}
