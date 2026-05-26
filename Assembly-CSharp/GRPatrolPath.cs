using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007B5 RID: 1973
public class GRPatrolPath : MonoBehaviour
{
	// Token: 0x0600323A RID: 12858 RVA: 0x00113C50 File Offset: 0x00111E50
	private void Awake()
	{
		this.patrolNodes = new List<Transform>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.patrolNodes.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x00113CA0 File Offset: 0x00111EA0
	public void OnDrawGizmosSelected()
	{
		if (this.patrolNodes == null || base.transform.childCount != this.patrolNodes.Count)
		{
			this.patrolNodes = new List<Transform>(base.transform.childCount);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.patrolNodes.Add(base.transform.GetChild(i));
			}
		}
		if (this.patrolNodes != null)
		{
			for (int j = 0; j < this.patrolNodes.Count; j++)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this.patrolNodes[j].transform.position, Vector3.one * 0.5f);
				if (j < this.patrolNodes.Count - 1)
				{
					Gizmos.DrawLine(this.patrolNodes[j].transform.position, this.patrolNodes[j + 1].transform.position);
				}
			}
		}
	}

	// Token: 0x04004134 RID: 16692
	[NonSerialized]
	public List<Transform> patrolNodes;

	// Token: 0x04004135 RID: 16693
	public int index;
}
