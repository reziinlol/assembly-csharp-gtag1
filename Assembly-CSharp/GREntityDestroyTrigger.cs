using System;
using UnityEngine;

// Token: 0x0200079C RID: 1948
public class GREntityDestroyTrigger : MonoBehaviour
{
	// Token: 0x060031DB RID: 12763 RVA: 0x00111DA8 File Offset: 0x0010FFA8
	private void OnTriggerEnter(Collider other)
	{
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.IsAuthority())
		{
			component.manager.RequestDestroyItem(component.id);
		}
	}
}
