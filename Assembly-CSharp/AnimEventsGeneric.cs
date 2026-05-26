using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E2 RID: 1250
public class AnimEventsGeneric : MonoBehaviour
{
	// Token: 0x06001E63 RID: 7779 RVA: 0x000A283F File Offset: 0x000A0A3F
	public void Event1()
	{
		this.event1.Invoke();
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000A284C File Offset: 0x000A0A4C
	public void Event2()
	{
		this.event2.Invoke();
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x000A2859 File Offset: 0x000A0A59
	public void Event3()
	{
		this.event3.Invoke();
	}

	// Token: 0x06001E66 RID: 7782 RVA: 0x000A2866 File Offset: 0x000A0A66
	public void Event4()
	{
		this.event4.Invoke();
	}

	// Token: 0x06001E67 RID: 7783 RVA: 0x000A2873 File Offset: 0x000A0A73
	public void Event5()
	{
		this.event5.Invoke();
	}

	// Token: 0x06001E68 RID: 7784 RVA: 0x000A2880 File Offset: 0x000A0A80
	public void Event6()
	{
		this.event6.Invoke();
	}

	// Token: 0x06001E69 RID: 7785 RVA: 0x000A288D File Offset: 0x000A0A8D
	public void Event7()
	{
		this.event7.Invoke();
	}

	// Token: 0x06001E6A RID: 7786 RVA: 0x000A289A File Offset: 0x000A0A9A
	public void Event8()
	{
		this.event8.Invoke();
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x000A28A7 File Offset: 0x000A0AA7
	public void Event9()
	{
		this.event9.Invoke();
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x000A28B4 File Offset: 0x000A0AB4
	public void Event10()
	{
		this.event10.Invoke();
	}

	// Token: 0x0400288F RID: 10383
	[SerializeField]
	private UnityEvent event1;

	// Token: 0x04002890 RID: 10384
	[SerializeField]
	private UnityEvent event2;

	// Token: 0x04002891 RID: 10385
	[SerializeField]
	private UnityEvent event3;

	// Token: 0x04002892 RID: 10386
	[SerializeField]
	private UnityEvent event4;

	// Token: 0x04002893 RID: 10387
	[SerializeField]
	private UnityEvent event5;

	// Token: 0x04002894 RID: 10388
	[SerializeField]
	private UnityEvent event6;

	// Token: 0x04002895 RID: 10389
	[SerializeField]
	private UnityEvent event7;

	// Token: 0x04002896 RID: 10390
	[SerializeField]
	private UnityEvent event8;

	// Token: 0x04002897 RID: 10391
	[SerializeField]
	private UnityEvent event9;

	// Token: 0x04002898 RID: 10392
	[SerializeField]
	private UnityEvent event10;
}
