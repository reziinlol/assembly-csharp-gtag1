using System;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class PlayerGameEventLocationTrigger : MonoBehaviour
{
	// Token: 0x06000FC2 RID: 4034 RVA: 0x000555B2 File Offset: 0x000537B2
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			PlayerGameEvents.TriggerEnterLocation(this.locationName);
		}
	}

	// Token: 0x040012FA RID: 4858
	[SerializeField]
	private string locationName;
}
