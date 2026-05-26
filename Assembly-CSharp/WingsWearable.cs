using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class WingsWearable : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06000DC6 RID: 3526 RVA: 0x0004B7EC File Offset: 0x000499EC
	private void Awake()
	{
		if (this.animator == null)
		{
			GTDev.LogError<string>("WingsWearable on " + base.gameObject.name + " missing animator", null);
			return;
		}
		this.xform = this.animator.transform;
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0004B839 File Offset: 0x00049A39
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.oldPos = this.xform.localPosition;
		this.lastSliceTime = Time.unscaledTime;
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x0004B860 File Offset: 0x00049A60
	public void SliceUpdate()
	{
		Vector3 position = this.xform.position;
		float unscaledTime = Time.unscaledTime;
		float num = Mathf.Max(unscaledTime - this.lastSliceTime, Mathf.Epsilon);
		float f = (position - this.oldPos).magnitude / num;
		float value = this.flapSpeedCurve.Evaluate(Mathf.Abs(f));
		this.animator.SetFloat(this.flapSpeedParamID, value);
		this.oldPos = position;
		this.lastSliceTime = unscaledTime;
	}

	// Token: 0x0400106D RID: 4205
	[Tooltip("This animator must have a parameter called 'FlapSpeed'")]
	public Animator animator;

	// Token: 0x0400106E RID: 4206
	[Tooltip("X axis is move speed, Y axis is flap speed")]
	public AnimationCurve flapSpeedCurve;

	// Token: 0x0400106F RID: 4207
	private Transform xform;

	// Token: 0x04001070 RID: 4208
	private Vector3 oldPos;

	// Token: 0x04001071 RID: 4209
	private float lastSliceTime;

	// Token: 0x04001072 RID: 4210
	private readonly int flapSpeedParamID = Animator.StringToHash("FlapSpeed");
}
