using System;
using UnityEngine;

// Token: 0x020005F5 RID: 1525
public class MonkeBallBallEjectZone : MonoBehaviour
{
	// Token: 0x060025FF RID: 9727 RVA: 0x000C9488 File Offset: 0x000C7688
	private void OnCollisionEnter(Collision collision)
	{
		GameBall component = collision.gameObject.GetComponent<GameBall>();
		if (component != null && collision.contacts.Length != 0)
		{
			component.SetVelocity(collision.contacts[0].impulse.normalized * this.ejectVelocity);
		}
	}

	// Token: 0x04003166 RID: 12646
	public Transform target;

	// Token: 0x04003167 RID: 12647
	public float ejectVelocity = 15f;
}
