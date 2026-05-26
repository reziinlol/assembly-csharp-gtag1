using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005C3 RID: 1475
public class GenericCounter : MonoBehaviour
{
	// Token: 0x06002512 RID: 9490 RVA: 0x000C59A0 File Offset: 0x000C3BA0
	public void CountUp()
	{
		this.currentCount++;
		this.DoCallbacks();
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000C59B6 File Offset: 0x000C3BB6
	public void CountDown()
	{
		this.currentCount--;
		this.DoCallbacks();
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000C59CC File Offset: 0x000C3BCC
	private void DoCallbacks()
	{
		if (this.currentCount < this.Threshold)
		{
			this.whenLessThan.Invoke();
			return;
		}
		if (this.currentCount == this.Threshold)
		{
			this.whenEqual.Invoke();
			return;
		}
		this.whenGreaterThan.Invoke();
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000C5A18 File Offset: 0x000C3C18
	public void ResetCounter()
	{
		this.currentCount = 0;
	}

	// Token: 0x04003063 RID: 12387
	[SerializeField]
	private int Threshold;

	// Token: 0x04003064 RID: 12388
	[SerializeField]
	private UnityEvent whenLessThan;

	// Token: 0x04003065 RID: 12389
	[SerializeField]
	private UnityEvent whenEqual;

	// Token: 0x04003066 RID: 12390
	[SerializeField]
	private UnityEvent whenGreaterThan;

	// Token: 0x04003067 RID: 12391
	private int currentCount;
}
