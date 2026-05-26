using System;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class SuperInfectionHandDisplay : MonoBehaviour
{
	// Token: 0x06000A46 RID: 2630 RVA: 0x00037124 File Offset: 0x00035324
	public void EnableHands(bool on)
	{
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.gameObjects[i].SetActive(on);
		}
	}

	// Token: 0x04000C6F RID: 3183
	[SerializeField]
	private GameObject[] gameObjects;
}
