using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class GorillaEventAnimation : MonoBehaviour
{
	// Token: 0x060004FF RID: 1279 RVA: 0x0001BCAC File Offset: 0x00019EAC
	private void Awake()
	{
		if (this._animation == null)
		{
			this._animation = base.GetComponentInChildren<Animation>();
		}
		this._animation.playAutomatically = false;
		for (int i = 0; i < this.clips.Length; i++)
		{
			this.clips[i].legacy = true;
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x0001BD00 File Offset: 0x00019F00
	private void OnDisable()
	{
		this._animation.enabled = false;
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x0001BD10 File Offset: 0x00019F10
	public void PlayClipByIndex(int index, float startTime)
	{
		if (index < 0 || index >= this.clips.Length)
		{
			return;
		}
		if (!this._animation.enabled)
		{
			this._animation.enabled = true;
		}
		AnimationClip animationClip = this.clips[index];
		if (this._animation.GetClip(animationClip.name) == null)
		{
			this._animation.AddClip(animationClip, animationClip.name);
		}
		this._animation.Play(animationClip.name);
		this._animation[animationClip.name].time = startTime;
		this._clipIndex = index;
	}

	// Token: 0x040005B9 RID: 1465
	public Animation _animation;

	// Token: 0x040005BA RID: 1466
	public float offsetTime;

	// Token: 0x040005BB RID: 1467
	public int animationClipIndex;

	// Token: 0x040005BC RID: 1468
	public AnimationClip[] clips;

	// Token: 0x040005BD RID: 1469
	[NonSerialized]
	public int _clipIndex;
}
