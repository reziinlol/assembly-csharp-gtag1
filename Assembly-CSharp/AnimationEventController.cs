using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class AnimationEventController : MonoBehaviour
{
	// Token: 0x06000B91 RID: 2961 RVA: 0x0003E35C File Offset: 0x0003C55C
	public void TriggerAttackVFX()
	{
		this.fxAttack.SetActive(false);
		this.fxAttack.SetActive(true);
	}

	// Token: 0x04000DF7 RID: 3575
	public GameObject fxAttack;
}
