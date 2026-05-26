using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004FC RID: 1276
public class BetaChecker : MonoBehaviour
{
	// Token: 0x06002001 RID: 8193 RVA: 0x000AC233 File Offset: 0x000AA433
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000AC260 File Offset: 0x000AA460
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x04002AC6 RID: 10950
	public GameObject[] objectsToEnable;

	// Token: 0x04002AC7 RID: 10951
	public bool doNotEnable;
}
