using System;
using UnityEngine.Events;

// Token: 0x020000AD RID: 173
public class GenericObservable : ObservableBehavior
{
	// Token: 0x0600042C RID: 1068 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected override void ObservableSliceUpdate()
	{
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00018896 File Offset: 0x00016A96
	protected override void OnBecameObservable()
	{
		UnityEvent onObservable = this.OnObservable;
		if (onObservable == null)
		{
			return;
		}
		onObservable.Invoke();
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x000188A8 File Offset: 0x00016AA8
	protected override void OnLostObservable()
	{
		UnityEvent onUnobservable = this.OnUnobservable;
		if (onUnobservable == null)
		{
			return;
		}
		onUnobservable.Invoke();
	}

	// Token: 0x0400048F RID: 1167
	public UnityEvent OnObservable;

	// Token: 0x04000490 RID: 1168
	public UnityEvent OnUnobservable;
}
