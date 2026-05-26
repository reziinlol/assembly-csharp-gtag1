using System;
using UnityEngine;

// Token: 0x02000D67 RID: 3431
[CreateAssetMenu(fileName = "ObservableBehaviorRule", menuName = "Utilities/ObservableBehaviorRule")]
public class ObservableBehaviorRule : ScriptableObject
{
	// Token: 0x170007F3 RID: 2035
	// (get) Token: 0x06005466 RID: 21606 RVA: 0x001B8C68 File Offset: 0x001B6E68
	public Vector2 ObservableDistanceRange
	{
		get
		{
			return this.observableDistanceRange;
		}
	}

	// Token: 0x170007F4 RID: 2036
	// (get) Token: 0x06005467 RID: 21607 RVA: 0x001B8C70 File Offset: 0x001B6E70
	public Vector2 ObservableDotRange
	{
		get
		{
			return this.observableDotRange;
		}
	}

	// Token: 0x170007F5 RID: 2037
	// (get) Token: 0x06005468 RID: 21608 RVA: 0x001B8C78 File Offset: 0x001B6E78
	public bool InverseObservable
	{
		get
		{
			return this.inverseObservable;
		}
	}

	// Token: 0x04006521 RID: 25889
	[SerializeField]
	private Vector2 observableDistanceRange = new Vector2(0f, 15f);

	// Token: 0x04006522 RID: 25890
	[SerializeField]
	private Vector2 observableDotRange = new Vector2(-1f, 0f);

	// Token: 0x04006523 RID: 25891
	[SerializeField]
	private bool inverseObservable;
}
