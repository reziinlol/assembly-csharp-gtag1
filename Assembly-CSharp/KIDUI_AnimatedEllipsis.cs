using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000B98 RID: 2968
public class KIDUI_AnimatedEllipsis : MonoBehaviour
{
	// Token: 0x06004A9F RID: 19103 RVA: 0x0018EC6A File Offset: 0x0018CE6A
	private void Awake()
	{
		if (this._ellipsisObjects != null)
		{
			return;
		}
		this.SetupEllipsis();
	}

	// Token: 0x06004AA0 RID: 19104 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x06004AA1 RID: 19105 RVA: 0x0018EC7B File Offset: 0x0018CE7B
	private void OnDisable()
	{
		this.StopAnimation();
	}

	// Token: 0x06004AA2 RID: 19106 RVA: 0x0018EC84 File Offset: 0x0018CE84
	private void SetupEllipsis()
	{
		if (this._ellipsisRoot == null)
		{
			this._ellipsisRoot = base.gameObject;
		}
		this._ellipsisObjects = new ValueTuple<GameObject, float, float, float>[this._ellipsisStartingValues.Count];
		for (int i = 0; i < this._ellipsisStartingValues.Count; i++)
		{
			float num = this._ellipsisStartingValues[i];
			this._ellipsisObjects[i].Item1 = Object.Instantiate<GameObject>(this._ellipsisPrefab, this._ellipsisRoot.transform);
			this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			this._ellipsisObjects[i].Item2 = (this._ellipsisObjects[i].Item3 = num);
		}
	}

	// Token: 0x06004AA3 RID: 19107 RVA: 0x0018ED5A File Offset: 0x0018CF5A
	private IEnumerator EllipsisAnimation()
	{
		int currIndex = 0;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				int num = i - currIndex;
				if (num < 0)
				{
					num = this._ellipsisStartingValues.Count + num;
				}
				float d = this._ellipsisStartingValues[num];
				this._ellipsisObjects[i].Item1.transform.localScale = Vector3.one * d;
			}
			int num2 = currIndex;
			currIndex = num2 + 1;
			if (currIndex >= this._ellipsisObjects.Length)
			{
				currIndex = 0;
			}
			yield return new WaitForSeconds(this._pauseBetweenScale);
		}
		yield break;
	}

	// Token: 0x06004AA4 RID: 19108 RVA: 0x0018ED69 File Offset: 0x0018CF69
	private IEnumerator EllipsisAnimation2()
	{
		float time = 0f;
		while (this._runAnimation)
		{
			for (int i = 0; i < this._ellipsisObjects.Length; i++)
			{
				float offsetTime = this._scaleDuration / (float)(this._ellipsisObjects.Length + 1) * (float)i;
				float num = this.LerpLoop(this._startingScale, this._endScale, time, offsetTime, this._scaleDuration);
				this._ellipsisObjects[i].Item1.transform.localScale = new Vector3(num, num, num);
			}
			time += Time.deltaTime * this._animationSpeedMultiplier;
			yield return null;
		}
		yield break;
	}

	// Token: 0x06004AA5 RID: 19109 RVA: 0x0018ED78 File Offset: 0x0018CF78
	public Task StartAnimation()
	{
		KIDUI_AnimatedEllipsis.<StartAnimation>d__24 <StartAnimation>d__;
		<StartAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAnimation>d__.<>4__this = this;
		<StartAnimation>d__.<>1__state = -1;
		<StartAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StartAnimation>d__24>(ref <StartAnimation>d__);
		return <StartAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06004AA6 RID: 19110 RVA: 0x0018EDBC File Offset: 0x0018CFBC
	public Task StopAnimation()
	{
		KIDUI_AnimatedEllipsis.<StopAnimation>d__25 <StopAnimation>d__;
		<StopAnimation>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StopAnimation>d__.<>4__this = this;
		<StopAnimation>d__.<>1__state = -1;
		<StopAnimation>d__.<>t__builder.Start<KIDUI_AnimatedEllipsis.<StopAnimation>d__25>(ref <StopAnimation>d__);
		return <StopAnimation>d__.<>t__builder.Task;
	}

	// Token: 0x06004AA7 RID: 19111 RVA: 0x0018EE00 File Offset: 0x0018D000
	public float LerpLoop(float start, float end, float time, float offsetTime, float duration)
	{
		float time2 = (offsetTime - time) % duration / duration;
		float t = this._ellipsisAnimationCurve.Evaluate(time2);
		return Mathf.Lerp(start, end, t);
	}

	// Token: 0x04005D6E RID: 23918
	[Header("Ellipsis Spawning")]
	[SerializeField]
	private bool _animateOnStart = true;

	// Token: 0x04005D6F RID: 23919
	[SerializeField]
	private int _ellipsisCount = 3;

	// Token: 0x04005D70 RID: 23920
	[SerializeField]
	private GameObject _ellipsisPrefab;

	// Token: 0x04005D71 RID: 23921
	[SerializeField]
	private GameObject _ellipsisRoot;

	// Token: 0x04005D72 RID: 23922
	[SerializeField]
	private List<float> _ellipsisStartingValues = new List<float>();

	// Token: 0x04005D73 RID: 23923
	[Header("Animation Settings")]
	[SerializeField]
	private bool _shouldLerp;

	// Token: 0x04005D74 RID: 23924
	[SerializeField]
	private AnimationCurve _ellipsisAnimationCurve;

	// Token: 0x04005D75 RID: 23925
	[SerializeField]
	private float _animationSpeedMultiplier = 0.25f;

	// Token: 0x04005D76 RID: 23926
	[SerializeField]
	private float _startingScale = 0.33f;

	// Token: 0x04005D77 RID: 23927
	[SerializeField]
	private float _intermediaryScale = 0.66f;

	// Token: 0x04005D78 RID: 23928
	[SerializeField]
	private float _endScale = 1f;

	// Token: 0x04005D79 RID: 23929
	[SerializeField]
	private float _scaleDuration = 0.25f;

	// Token: 0x04005D7A RID: 23930
	[SerializeField]
	private float _pauseBetweenScale = 0.25f;

	// Token: 0x04005D7B RID: 23931
	[SerializeField]
	private float _pauseBetweenCycles = 0.5f;

	// Token: 0x04005D7C RID: 23932
	private bool _runAnimation;

	// Token: 0x04005D7D RID: 23933
	private float _nextChange;

	// Token: 0x04005D7E RID: 23934
	[TupleElementNames(new string[]
	{
		"ellipsis",
		"startingScale",
		"currentScale",
		"lerpT"
	})]
	private ValueTuple<GameObject, float, float, float>[] _ellipsisObjects;

	// Token: 0x04005D7F RID: 23935
	private Coroutine _animationCoroutine;
}
