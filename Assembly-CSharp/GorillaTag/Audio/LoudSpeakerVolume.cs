using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011FA RID: 4602
	public class LoudSpeakerVolume : MonoBehaviour
	{
		// Token: 0x0600737B RID: 29563 RVA: 0x002586D0 File Offset: 0x002568D0
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("GorillaPlayer"))
			{
				VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
				if (component != null && component.creator != null)
				{
					if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
					{
						this._trigger.OnPlayerEnter(component);
						return;
					}
				}
				else
				{
					Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
				}
			}
		}

		// Token: 0x0600737C RID: 29564 RVA: 0x00258740 File Offset: 0x00256940
		public void OnTriggerExit(Collider other)
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
				{
					this._trigger.OnPlayerExit(component);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
			}
		}

		// Token: 0x040083D8 RID: 33752
		[SerializeField]
		private LoudSpeakerTrigger _trigger;
	}
}
