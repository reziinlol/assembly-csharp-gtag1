using System;
using UnityEngine;

// Token: 0x02000012 RID: 18
public class ParentedObjectStressTestMain : MonoBehaviour
{
	// Token: 0x06000050 RID: 80 RVA: 0x00002D04 File Offset: 0x00000F04
	public void Start()
	{
		for (int i = 0; i < (int)this.NumObjects.x; i++)
		{
			for (int j = 0; j < (int)this.NumObjects.y; j++)
			{
				for (int k = 0; k < (int)this.NumObjects.z; k++)
				{
					UnityEngine.Object.Instantiate<GameObject>(this.Object).transform.position = new Vector3(2f * ((float)i / (this.NumObjects.x - 1f) - 0.5f) * this.NumObjects.x * this.Spacing.x, 2f * ((float)j / (this.NumObjects.y - 1f) - 0.5f) * this.NumObjects.y * this.Spacing.y, 2f * ((float)k / (this.NumObjects.z - 1f) - 0.5f) * this.NumObjects.z * this.Spacing.z);
				}
			}
		}
	}

	// Token: 0x04000034 RID: 52
	public GameObject Object;

	// Token: 0x04000035 RID: 53
	public Vector3 NumObjects;

	// Token: 0x04000036 RID: 54
	public Vector3 Spacing;
}
