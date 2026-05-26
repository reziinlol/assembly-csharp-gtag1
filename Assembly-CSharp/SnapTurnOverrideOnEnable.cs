using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000DAF RID: 3503
public class SnapTurnOverrideOnEnable : MonoBehaviour, ISnapTurnOverride
{
	// Token: 0x060055E0 RID: 21984 RVA: 0x001BF66C File Offset: 0x001BD86C
	private void OnEnable()
	{
		if (this.snapTurn == null && GorillaTagger.Instance != null)
		{
			this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		}
		if (this.snapTurn != null)
		{
			this.snapTurnOverride = true;
			this.snapTurn.SetTurningOverride(this);
		}
	}

	// Token: 0x060055E1 RID: 21985 RVA: 0x001BF6C5 File Offset: 0x001BD8C5
	private void OnDisable()
	{
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
	}

	// Token: 0x060055E2 RID: 21986 RVA: 0x001BF6E2 File Offset: 0x001BD8E2
	bool ISnapTurnOverride.TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x040065F7 RID: 26103
	private GorillaSnapTurn snapTurn;

	// Token: 0x040065F8 RID: 26104
	private bool snapTurnOverride;
}
