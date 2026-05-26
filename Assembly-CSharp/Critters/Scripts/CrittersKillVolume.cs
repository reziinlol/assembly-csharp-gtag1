using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02001320 RID: 4896
	public class CrittersKillVolume : MonoBehaviour
	{
		// Token: 0x06007B63 RID: 31587 RVA: 0x00284768 File Offset: 0x00282968
		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody)
			{
				CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
				if (component.IsNotNull())
				{
					component.gameObject.SetActive(false);
				}
			}
		}
	}
}
