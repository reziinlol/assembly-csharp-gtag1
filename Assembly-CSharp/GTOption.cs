using System;
using UnityEngine;

// Token: 0x02000346 RID: 838
[Serializable]
public struct GTOption<T>
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x060014A4 RID: 5284 RVA: 0x0006E0EA File Offset: 0x0006C2EA
	public T ResolvedValue
	{
		get
		{
			if (!this.enabled)
			{
				return this.defaultValue;
			}
			return this.value;
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0006E101 File Offset: 0x0006C301
	public GTOption(T defaultValue)
	{
		this.enabled = false;
		this.value = defaultValue;
		this.defaultValue = defaultValue;
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x0006E118 File Offset: 0x0006C318
	public void ResetValue()
	{
		this.value = this.defaultValue;
	}

	// Token: 0x04001958 RID: 6488
	[Tooltip("When checked, the filter is applied; when unchecked (default), it is ignored.")]
	[SerializeField]
	public bool enabled;

	// Token: 0x04001959 RID: 6489
	[SerializeField]
	public T value;

	// Token: 0x0400195A RID: 6490
	[NonSerialized]
	public readonly T defaultValue;
}
