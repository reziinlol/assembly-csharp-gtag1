using System;
using UnityEngine;

// Token: 0x02000DB0 RID: 3504
public class SpawnManager : MonoBehaviour
{
	// Token: 0x060055E4 RID: 21988 RVA: 0x001BF6EA File Offset: 0x001BD8EA
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
