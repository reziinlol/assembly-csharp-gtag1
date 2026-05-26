using System;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class SkeletonPathingNode : MonoBehaviour
{
	// Token: 0x06000C72 RID: 3186 RVA: 0x000440BC File Offset: 0x000422BC
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000F0F RID: 3855
	public bool ejectionPoint;

	// Token: 0x04000F10 RID: 3856
	public SkeletonPathingNode[] connectedNodes;

	// Token: 0x04000F11 RID: 3857
	public float distanceToExitNode;
}
