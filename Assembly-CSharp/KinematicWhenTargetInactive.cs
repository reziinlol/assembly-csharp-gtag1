using System;
using UnityEngine;

// Token: 0x02000597 RID: 1431
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x0600243E RID: 9278 RVA: 0x000C2CB8 File Offset: 0x000C0EB8
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x04002FA1 RID: 12193
	public Rigidbody[] rigidBodies;

	// Token: 0x04002FA2 RID: 12194
	public GameObject target;
}
