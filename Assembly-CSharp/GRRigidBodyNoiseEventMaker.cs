using System;
using UnityEngine;

// Token: 0x020007CD RID: 1997
public class GRRigidBodyNoiseEventMaker : MonoBehaviour
{
	// Token: 0x060032F4 RID: 13044 RVA: 0x00117338 File Offset: 0x00115538
	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > this.velocityThreshold && base.GetComponent<GameEntity>() != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(collision.GetContact(0).point, 1f, 1f);
		}
	}

	// Token: 0x04004234 RID: 16948
	public float velocityThreshold = 5f;
}
