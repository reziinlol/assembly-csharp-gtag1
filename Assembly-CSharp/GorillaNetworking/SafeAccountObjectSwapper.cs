using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x0200103F RID: 4159
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		// Token: 0x06006807 RID: 26631 RVA: 0x002192A2 File Offset: 0x002174A2
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		// Token: 0x06006808 RID: 26632 RVA: 0x002192E0 File Offset: 0x002174E0
		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		// Token: 0x06006809 RID: 26633 RVA: 0x002192EC File Offset: 0x002174EC
		public void SwitchToSafeMode()
		{
			foreach (GameObject gameObject in this.UnSafeGameObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.UnSafeTexts)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			foreach (GameObject gameObject3 in this.SafeTexts)
			{
				if (gameObject3 != null)
				{
					gameObject3.SetActive(true);
				}
			}
			foreach (GameObject gameObject4 in this.SafeModeObjects)
			{
				if (gameObject4 != null)
				{
					gameObject4.SetActive(true);
				}
			}
		}

		// Token: 0x04007761 RID: 30561
		public GameObject[] UnSafeGameObjects;

		// Token: 0x04007762 RID: 30562
		public GameObject[] UnSafeTexts;

		// Token: 0x04007763 RID: 30563
		public GameObject[] SafeTexts;

		// Token: 0x04007764 RID: 30564
		public GameObject[] SafeModeObjects;
	}
}
