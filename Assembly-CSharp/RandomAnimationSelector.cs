using System;
using UnityEngine;

// Token: 0x0200058E RID: 1422
[RequireComponent(typeof(Animator))]
public class RandomAnimationSelector : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600240B RID: 9227 RVA: 0x000C19EC File Offset: 0x000BFBEC
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.animationTrigger = Animator.StringToHash(this.animationTriggerName);
		this.animationSelect = Animator.StringToHash(this.animationSelectName);
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000C1A1C File Offset: 0x000BFC1C
	public void OnEnable()
	{
		if (this.animator != null)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lastSliceUpdateTime = Time.time;
		}
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000C1A40 File Offset: 0x000BFC40
	public void SliceUpdate()
	{
		float num = Time.time - this.lastSliceUpdateTime;
		this.lastSliceUpdateTime = Time.time;
		float num2 = 1f - Mathf.Exp(-this.animationChancePerSecond * num);
		if (Random.value < num2)
		{
			float value = Time.time - (float)((int)Time.time);
			this.animator.SetFloat(this.animationSelect, value);
			this.animator.SetTrigger(this.animationTrigger);
		}
	}

	// Token: 0x04002F49 RID: 12105
	[SerializeField]
	private string animationTriggerName;

	// Token: 0x04002F4A RID: 12106
	private int animationTrigger;

	// Token: 0x04002F4B RID: 12107
	[SerializeField]
	private string animationSelectName;

	// Token: 0x04002F4C RID: 12108
	private int animationSelect;

	// Token: 0x04002F4D RID: 12109
	[Range(0f, 1f)]
	[SerializeField]
	private float animationChancePerSecond = 0.33f;

	// Token: 0x04002F4E RID: 12110
	private Animator animator;

	// Token: 0x04002F4F RID: 12111
	private float lastSliceUpdateTime;
}
