using System;
using GorillaTag.Gravity;
using UnityEngine;

// Token: 0x0200030C RID: 780
public class CounterRotator : MonoBehaviour
{
	// Token: 0x060013BF RID: 5055 RVA: 0x0006B392 File Offset: 0x00069592
	private void Start()
	{
		this.startingPosition = this.stabilizedObject.transform.position;
		this.startingRotation = this.stabilizedObject.transform.rotation;
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x0006B3C0 File Offset: 0x000695C0
	private void LateUpdate()
	{
		Quaternion lhs = this.startingRotation * Quaternion.Inverse(this.stabilizedObject.transform.rotation);
		base.transform.rotation = lhs * base.transform.rotation;
		Vector3 b = this.startingPosition - this.stabilizedObject.transform.position;
		base.transform.position += b;
		if (this.gravityCompensator != null)
		{
			this.gravityCompensator.SetGravityDirection(-base.transform.up);
		}
	}

	// Token: 0x04001857 RID: 6231
	[SerializeField]
	private GameObject stabilizedObject;

	// Token: 0x04001858 RID: 6232
	[SerializeField]
	private ChangingBasicGravityZone gravityCompensator;

	// Token: 0x04001859 RID: 6233
	private Vector3 startingPosition;

	// Token: 0x0400185A RID: 6234
	private Quaternion startingRotation;
}
