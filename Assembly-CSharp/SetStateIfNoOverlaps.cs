using System;
using UnityEngine;

// Token: 0x020005DF RID: 1503
public class SetStateIfNoOverlaps : SetStateConditional
{
	// Token: 0x06002565 RID: 9573 RVA: 0x000C65FF File Offset: 0x000C47FF
	protected override void Setup(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this._volume = animator.GetComponent<VolumeCast>();
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000C660D File Offset: 0x000C480D
	protected override bool CanSetState(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		bool flag = this._volume.CheckOverlaps();
		if (flag)
		{
			this._sinceEnter = 0f;
		}
		return !flag;
	}

	// Token: 0x040030E7 RID: 12519
	public VolumeCast _volume;
}
