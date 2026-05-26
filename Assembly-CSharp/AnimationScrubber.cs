using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class AnimationScrubber : MonoBehaviour
{
	// Token: 0x0600115A RID: 4442 RVA: 0x0005D3C0 File Offset: 0x0005B5C0
	private void LateUpdate()
	{
		if (!this.scrubberActive)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.targetAnimator.GetCurrentAnimatorStateInfo(0);
		AnimatorClipInfo[] currentAnimatorClipInfo = this.targetAnimator.GetCurrentAnimatorClipInfo(0);
		this.targetAnimator.Play(currentAnimatorClipInfo[0].clip.name, 0, this.animationPlaybackTime / currentAnimatorStateInfo.length);
	}

	// Token: 0x040014B8 RID: 5304
	public bool scrubberActive;

	// Token: 0x040014B9 RID: 5305
	public float animationPlaybackTime;

	// Token: 0x040014BA RID: 5306
	public Animator targetAnimator;
}
