using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020005CB RID: 1483
public class GorillaIKHandTarget : MonoBehaviour
{
	// Token: 0x0600252B RID: 9515 RVA: 0x000C5CD1 File Offset: 0x000C3ED1
	private void Start()
	{
		this.thisRigidbody = base.gameObject.GetComponent<Rigidbody>();
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000C5CE4 File Offset: 0x000C3EE4
	private void FixedUpdate()
	{
		this.thisRigidbody.MovePosition(this.handToStickTo.transform.position);
		base.transform.rotation = this.handToStickTo.transform.rotation;
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x0400307F RID: 12415
	public GameObject handToStickTo;

	// Token: 0x04003080 RID: 12416
	public bool isLeftHand;

	// Token: 0x04003081 RID: 12417
	public float hapticStrength;

	// Token: 0x04003082 RID: 12418
	private Rigidbody thisRigidbody;

	// Token: 0x04003083 RID: 12419
	private XRController controllerReference;
}
