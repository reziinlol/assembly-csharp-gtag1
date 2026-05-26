using System;
using UnityEngine;

// Token: 0x02000DCD RID: 3533
public class LightningGenerator : MonoBehaviour
{
	// Token: 0x0600568B RID: 22155 RVA: 0x001C0F44 File Offset: 0x001BF144
	private void Awake()
	{
		this.strikes = new LightningStrike[this.maxConcurrentStrikes];
		for (int i = 0; i < this.strikes.Length; i++)
		{
			if (i == 0)
			{
				this.strikes[i] = this.prototype;
			}
			else
			{
				this.strikes[i] = Object.Instantiate<LightningStrike>(this.prototype, base.transform);
			}
			this.strikes[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x0600568C RID: 22156 RVA: 0x001C0FB4 File Offset: 0x001BF1B4
	private void OnEnable()
	{
		LightningDispatcher.RequestLightningStrike += this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x0600568D RID: 22157 RVA: 0x001C0FC7 File Offset: 0x001BF1C7
	private void OnDisable()
	{
		LightningDispatcher.RequestLightningStrike -= this.LightningDispatcher_RequestLightningStrike;
	}

	// Token: 0x0600568E RID: 22158 RVA: 0x001C0FDA File Offset: 0x001BF1DA
	private LightningStrike LightningDispatcher_RequestLightningStrike(Vector3 t1, Vector3 t2)
	{
		this.index = (this.index + 1) % this.strikes.Length;
		return this.strikes[this.index];
	}

	// Token: 0x0400666A RID: 26218
	[SerializeField]
	private uint maxConcurrentStrikes = 10U;

	// Token: 0x0400666B RID: 26219
	[SerializeField]
	private LightningStrike prototype;

	// Token: 0x0400666C RID: 26220
	private LightningStrike[] strikes;

	// Token: 0x0400666D RID: 26221
	private int index;
}
