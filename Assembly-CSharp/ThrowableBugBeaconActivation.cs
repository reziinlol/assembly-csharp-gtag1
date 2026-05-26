using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000DDD RID: 3549
public class ThrowableBugBeaconActivation : MonoBehaviour
{
	// Token: 0x060056E6 RID: 22246 RVA: 0x001C2A5C File Offset: 0x001C0C5C
	private void Awake()
	{
		this.tbb = base.GetComponent<ThrowableBugBeacon>();
	}

	// Token: 0x060056E7 RID: 22247 RVA: 0x001C2A6A File Offset: 0x001C0C6A
	private void OnEnable()
	{
		base.StartCoroutine(this.SendSignals());
	}

	// Token: 0x060056E8 RID: 22248 RVA: 0x00005511 File Offset: 0x00003711
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x060056E9 RID: 22249 RVA: 0x001C2A79 File Offset: 0x001C0C79
	private IEnumerator SendSignals()
	{
		uint count = 0U;
		while (this.signalCount == 0U || count < this.signalCount)
		{
			yield return new WaitForSeconds(Random.Range(this.minCallTime, this.maxCallTime));
			switch (this.mode)
			{
			case ThrowableBugBeaconActivation.ActivationMode.CALL:
				this.tbb.Call();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.DISMISS:
				this.tbb.Dismiss();
				break;
			case ThrowableBugBeaconActivation.ActivationMode.LOCK:
				this.tbb.Lock();
				break;
			}
			uint num = count;
			count = num + 1U;
		}
		yield break;
	}

	// Token: 0x040066EA RID: 26346
	[SerializeField]
	private float minCallTime = 1f;

	// Token: 0x040066EB RID: 26347
	[SerializeField]
	private float maxCallTime = 5f;

	// Token: 0x040066EC RID: 26348
	[SerializeField]
	private uint signalCount;

	// Token: 0x040066ED RID: 26349
	[SerializeField]
	private ThrowableBugBeaconActivation.ActivationMode mode;

	// Token: 0x040066EE RID: 26350
	private ThrowableBugBeacon tbb;

	// Token: 0x02000DDE RID: 3550
	private enum ActivationMode
	{
		// Token: 0x040066F0 RID: 26352
		CALL,
		// Token: 0x040066F1 RID: 26353
		DISMISS,
		// Token: 0x040066F2 RID: 26354
		LOCK
	}
}
