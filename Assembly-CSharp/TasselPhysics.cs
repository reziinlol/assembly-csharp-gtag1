using System;
using UnityEngine;

// Token: 0x020002FA RID: 762
public class TasselPhysics : MonoBehaviour
{
	// Token: 0x0600136F RID: 4975 RVA: 0x00066730 File Offset: 0x00064930
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
		if (this.LockXAxis)
		{
			this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, this.localCenterOfMass));
			return;
		}
		this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(this.localCenterOfMass));
	}

	// Token: 0x06001370 RID: 4976 RVA: 0x00066788 File Offset: 0x00064988
	private void Update()
	{
		float y = base.transform.lossyScale.y;
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * y * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 a = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 a2 = position + (a - position).normalized * this.centerOfMassLength * y;
		this.velocity = (a2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = a2;
		if (this.LockXAxis)
		{
			foreach (GameObject gameObject in this.tasselInstances)
			{
				gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.right, a2 - position) * this.rotCorrection;
			}
			return;
		}
		foreach (GameObject gameObject2 in this.tasselInstances)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(a2 - position, gameObject2.transform.position - position) * this.rotCorrection;
		}
	}

	// Token: 0x040017D0 RID: 6096
	[SerializeField]
	private GameObject[] tasselInstances;

	// Token: 0x040017D1 RID: 6097
	[SerializeField]
	private Vector3 localCenterOfMass;

	// Token: 0x040017D2 RID: 6098
	[SerializeField]
	private float gravityStrength;

	// Token: 0x040017D3 RID: 6099
	[SerializeField]
	private float drag;

	// Token: 0x040017D4 RID: 6100
	[SerializeField]
	private bool LockXAxis;

	// Token: 0x040017D5 RID: 6101
	private Vector3 lastCenterPos;

	// Token: 0x040017D6 RID: 6102
	private Vector3 velocity;

	// Token: 0x040017D7 RID: 6103
	private float centerOfMassLength;

	// Token: 0x040017D8 RID: 6104
	private Quaternion rotCorrection;
}
