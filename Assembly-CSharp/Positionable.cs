using System;
using UnityEngine;

// Token: 0x0200049A RID: 1178
public class Positionable : MonoBehaviour
{
	// Token: 0x06001C7B RID: 7291 RVA: 0x0009A350 File Offset: 0x00098550
	public void CopyPostion(Transform t)
	{
		base.transform.position = t.position;
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x0009A363 File Offset: 0x00098563
	public void StickRightUnder(Transform t)
	{
		base.transform.position = t.position;
		base.transform.localPosition = Vector3.zero;
	}
}
