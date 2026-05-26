using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000D0 RID: 208
[RequireComponent(typeof(Animation))]
public class GorillaEventAnimationController : MonoBehaviour
{
	// Token: 0x06000503 RID: 1283 RVA: 0x0001BDAC File Offset: 0x00019FAC
	private void Awake()
	{
		this.bakedAnimationData = new Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>>();
		for (int i = 0; i < this.bakedAnimKeyframeData.Count; i++)
		{
			AnimationClip clip = this.bakedAnimKeyframeData[i].clip;
			this.bakedAnimationData.Add(clip, new Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>());
			for (int j = 0; j < this.bakedAnimKeyframeData[i].gEAKeyframeData.Count; j++)
			{
				this.bakedAnimationData[clip].Add(this.bakedAnimKeyframeData[i].gEAKeyframeData[j].gEA, this.bakedAnimKeyframeData[i].gEAKeyframeData[j].keyframeData);
			}
		}
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x0001BE70 File Offset: 0x0001A070
	private void Update()
	{
		AnimationState animationState = null;
		foreach (object obj in this.controllingAnimation)
		{
			AnimationState animationState2 = (AnimationState)obj;
			if (animationState2.weight == 1f)
			{
				animationState = animationState2;
				this.currentClip = animationState2.clip;
				break;
			}
		}
		if (!this.playAnimation)
		{
			if (this.controllingAnimation.isPlaying)
			{
				this.controllingAnimation.Stop();
			}
			return;
		}
		if (!this.controllingAnimation.enabled)
		{
			this.controllingAnimation.enabled = true;
		}
		if (this.currentClip != this.clips[this.animationClipIndex] || animationState == null || !this.controllingAnimation.isPlaying)
		{
			this.currentClip = this.clips[this.animationClipIndex];
			this.currentClip.legacy = true;
			while (this.lateStart > 0f && this.currentClip.length < this.lateStart && this.animationClipIndex < this.clips.Count - 1)
			{
				this.lateStart -= this.currentClip.length;
				List<AnimationClip> list = this.clips;
				int index = this.animationClipIndex + 1;
				this.animationClipIndex = index;
				this.currentClip = list[index];
				this.currentClip.legacy = true;
			}
			this.controllingAnimation.Play(this.currentClip.name);
			animationState = this.controllingAnimation[this.currentClip.name];
			animationState.time = Math.Min(this.lateStart, this.currentClip.length);
			this.lateStart = 0f;
		}
		float time = animationState.time;
		if (!this.bakedAnimationData.ContainsKey(this.currentClip))
		{
			return;
		}
		foreach (KeyValuePair<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>> keyValuePair in this.bakedAnimationData[this.currentClip])
		{
			GorillaEventAnimation key = keyValuePair.Key;
			List<GorillaEventAnimationController.ControlledAnimationKeyframeData> value = keyValuePair.Value;
			int i = 0;
			while (i < value.Count)
			{
				if (value.Count < 2 || i == value.Count - 1 || time < value[i].startTime || time < value[i + 1].startTime)
				{
					bool flag = false;
					int num = value[i].animationClipIndex;
					float startTime = time - value[i].startTime + value[i].startOffset;
					if (key.enabled != value[i].animEnabled)
					{
						key.enabled = value[i].animEnabled;
						flag = value[i].animEnabled;
					}
					if (value[i].animEnabled && (key._clipIndex != value[i].animationClipIndex || !key._animation.IsPlaying(key.clips[num].name)))
					{
						flag = true;
					}
					if (flag)
					{
						key.PlayClipByIndex(value[i].animationClipIndex, startTime);
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001C210 File Offset: 0x0001A410
	public void SetPlayState(bool isPlaying)
	{
		this.playAnimation = isPlaying;
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x0001C219 File Offset: 0x0001A419
	public void SetAnimationClip(int clipIndex)
	{
		this.animationClipIndex = clipIndex;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x0001C222 File Offset: 0x0001A422
	public void StartPlaying(float secondsPast)
	{
		this.lateStart = secondsPast;
		this.animationClipIndex = 0;
		this.playAnimation = true;
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x0001C239 File Offset: 0x0001A439
	public void StartPlaying()
	{
		this.StartPlaying(0f);
	}

	// Token: 0x040005BE RID: 1470
	public Animation controllingAnimation;

	// Token: 0x040005BF RID: 1471
	public bool playAnimation;

	// Token: 0x040005C0 RID: 1472
	private float lateStart;

	// Token: 0x040005C1 RID: 1473
	public int animationClipIndex;

	// Token: 0x040005C2 RID: 1474
	public List<AnimationClip> clips;

	// Token: 0x040005C3 RID: 1475
	private AnimationClip currentClip;

	// Token: 0x040005C4 RID: 1476
	private Dictionary<AnimationClip, Dictionary<GorillaEventAnimation, List<GorillaEventAnimationController.ControlledAnimationKeyframeData>>> bakedAnimationData;

	// Token: 0x040005C5 RID: 1477
	[SerializeField]
	[HideInInspector]
	private List<GorillaEventAnimationController.AnimToGEAKeyframeData> bakedAnimKeyframeData;

	// Token: 0x020000D1 RID: 209
	[Serializable]
	public struct AnimToGEAKeyframeData
	{
		// Token: 0x040005C6 RID: 1478
		public AnimationClip clip;

		// Token: 0x040005C7 RID: 1479
		public List<GorillaEventAnimationController.GEAKeyframeData> gEAKeyframeData;
	}

	// Token: 0x020000D2 RID: 210
	[Serializable]
	public struct GEAKeyframeData
	{
		// Token: 0x040005C8 RID: 1480
		public GorillaEventAnimation gEA;

		// Token: 0x040005C9 RID: 1481
		public List<GorillaEventAnimationController.ControlledAnimationKeyframeData> keyframeData;
	}

	// Token: 0x020000D3 RID: 211
	[Serializable]
	public struct ControlledAnimationKeyframeData
	{
		// Token: 0x0600050A RID: 1290 RVA: 0x0001C246 File Offset: 0x0001A446
		public ControlledAnimationKeyframeData(int _index, float _time, float _startOffset, bool _animEnabled)
		{
			this.animationClipIndex = _index;
			this.startTime = _time;
			this.startOffset = _startOffset;
			this.animEnabled = _animEnabled;
		}

		// Token: 0x040005CA RID: 1482
		public int animationClipIndex;

		// Token: 0x040005CB RID: 1483
		public float startTime;

		// Token: 0x040005CC RID: 1484
		public float startOffset;

		// Token: 0x040005CD RID: 1485
		public bool animEnabled;
	}
}
