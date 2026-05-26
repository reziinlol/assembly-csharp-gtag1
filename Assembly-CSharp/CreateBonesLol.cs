using System;
using UnityEngine;

// Token: 0x020005A2 RID: 1442
public class CreateBonesLol : MonoBehaviour
{
	// Token: 0x0600248E RID: 9358 RVA: 0x000C4248 File Offset: 0x000C2448
	private void Update()
	{
		if (this.skeleton.Bones.Count <= 0)
		{
			return;
		}
		foreach (OVRBone ovrbone in this.skeleton.Bones)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.cube);
			gameObject.transform.parent = ovrbone.Transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		base.enabled = false;
	}

	// Token: 0x04002FF1 RID: 12273
	public GameObject cube;

	// Token: 0x04002FF2 RID: 12274
	public OVRSkeleton skeleton;
}
