using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200039C RID: 924
public class SpinParametricAnimation : MonoBehaviour
{
	// Token: 0x0600165C RID: 5724 RVA: 0x00081A31 File Offset: 0x0007FC31
	protected void OnEnable()
	{
		this.axis = this.axis.normalized;
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x00081A44 File Offset: 0x0007FC44
	protected void LateUpdate()
	{
		Transform transform = base.transform;
		this._animationProgress = (this._animationProgress + Time.deltaTime * this.revolutionsPerSecond) % 1f;
		float num = this.timeCurve.Evaluate(this._animationProgress) * 360f;
		float angle = num - this._oldAngle;
		this._oldAngle = num;
		if (this.WorldSpaceRotation)
		{
			transform.rotation = Quaternion.AngleAxis(angle, this.axis) * transform.rotation;
			return;
		}
		transform.localRotation = Quaternion.AngleAxis(angle, this.axis) * transform.localRotation;
	}

	// Token: 0x04002077 RID: 8311
	[Tooltip("Axis to rotate around.")]
	public Vector3 axis = Vector3.up;

	// Token: 0x04002078 RID: 8312
	[Tooltip("Whether rotation is in World Space or Local Space")]
	public bool WorldSpaceRotation = true;

	// Token: 0x04002079 RID: 8313
	[FormerlySerializedAs("speed")]
	[Tooltip("Speed of rotation.")]
	public float revolutionsPerSecond = 0.25f;

	// Token: 0x0400207A RID: 8314
	[Tooltip("Affects the progress of the animation over time.")]
	public AnimationCurve timeCurve;

	// Token: 0x0400207B RID: 8315
	private float _animationProgress;

	// Token: 0x0400207C RID: 8316
	private float _oldAngle;
}
