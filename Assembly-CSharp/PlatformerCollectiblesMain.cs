using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class PlatformerCollectiblesMain : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x0000391C File Offset: 0x00001B1C
	public void Start()
	{
		int num = 0;
		while ((float)num < this.CoinGridCount)
		{
			float x = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num / (this.CoinGridCount - 1f);
			int num2 = 0;
			while ((float)num2 < this.CoinGridCount)
			{
				float z = -0.5f * this.CoinGridSize + this.CoinGridSize * (float)num2 / (this.CoinGridCount - 1f);
				Object.Instantiate<GameObject>(this.Coin).transform.position = new Vector3(x, 0.2f, z);
				num2++;
			}
			num++;
		}
	}

	// Token: 0x0400005C RID: 92
	public GameObject Coin;

	// Token: 0x0400005D RID: 93
	public float CoinGridCount = 5f;

	// Token: 0x0400005E RID: 94
	public float CoinGridSize = 7f;
}
