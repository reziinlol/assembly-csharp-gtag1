using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020002F6 RID: 758
public class SpinWithGorillaSpeed : MonoBehaviour
{
	// Token: 0x06001352 RID: 4946 RVA: 0x00065F1F File Offset: 0x0006411F
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.initialRotation = base.transform.localRotation;
		this.spinAxis = this.initialRotation * this.axisOfRotation * Vector3.forward;
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x00065F60 File Offset: 0x00064160
	private void Update()
	{
		Vector3 vector = (this.optionalVelocityEstimator != null) ? this.optionalVelocityEstimator.linearVelocity : this.rig.LatestVelocity();
		vector.y *= this.verticalSpeedInfluence;
		float time = vector.magnitude / this.maxSpeed;
		float num = Time.deltaTime * this.degreesPerSecondAtSpeed.Evaluate(time) * (this.clockwise ? -1f : 1f);
		this.currentAngle = Mathf.Repeat(this.currentAngle + num, 360f);
		Quaternion quaternion = this.initialRotation * Quaternion.AngleAxis(this.currentAngle, this.spinAxis);
		base.transform.SetLocalPositionAndRotation(quaternion * this.centerOfRotation, quaternion);
		if (this.tickSound != null && this.tickClips.Length != 0)
		{
			this.tickAngle += num;
			if (this.tickAngle >= this.tickSoundDegrees)
			{
				this.tickSound.pitch = this.tickPitchAtSpeed.Evaluate(time);
				this.tickSound.volume = this.tickVolumeAtSpeed.Evaluate(time);
				this.tickSound.clip = this.tickClips.GetRandomItem<AudioClip>();
				this.tickSound.GTPlay();
				this.tickAngle = Mathf.Repeat(this.tickAngle, this.tickSoundDegrees);
			}
		}
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x000660C8 File Offset: 0x000642C8
	private void OnDisable()
	{
		this.currentAngle = 0f;
		this.tickAngle = 0f;
	}

	// Token: 0x040017A0 RID: 6048
	[Tooltip("Get the velocity from this component when determining the spin speed. If this is unset, it will use the unsmoothed velocity of the parent VRRig component.")]
	[SerializeField]
	private GorillaVelocityEstimator optionalVelocityEstimator;

	// Token: 0x040017A1 RID: 6049
	[SerializeField]
	private Quaternion axisOfRotation = Quaternion.identity;

	// Token: 0x040017A2 RID: 6050
	[SerializeField]
	private Vector3 centerOfRotation = Vector3.zero;

	// Token: 0x040017A3 RID: 6051
	[Tooltip("The reported speed will be divided by this value before being used to sample AnimationCurves, to allow them to be in the range 0-1.")]
	[SerializeField]
	private float maxSpeed;

	// Token: 0x040017A4 RID: 6052
	[SerializeField]
	private AnimationCurve degreesPerSecondAtSpeed;

	// Token: 0x040017A5 RID: 6053
	[SerializeField]
	private bool clockwise;

	// Token: 0x040017A6 RID: 6054
	[Tooltip("The Y component of the reported speed will be multiplied by this value. At 0, falling will have no effect on the rotation speed.")]
	[SerializeField]
	private float verticalSpeedInfluence = 1f;

	// Token: 0x040017A7 RID: 6055
	[Header("Ticking sound")]
	[Tooltip("After this many degrees of rotation, a \"tick\" sound will play.")]
	[SerializeField]
	private float tickSoundDegrees = 360f;

	// Token: 0x040017A8 RID: 6056
	[SerializeField]
	private AnimationCurve tickVolumeAtSpeed;

	// Token: 0x040017A9 RID: 6057
	[SerializeField]
	private AnimationCurve tickPitchAtSpeed;

	// Token: 0x040017AA RID: 6058
	[SerializeField]
	private AudioSource tickSound;

	// Token: 0x040017AB RID: 6059
	[SerializeField]
	private AudioClip[] tickClips;

	// Token: 0x040017AC RID: 6060
	private VRRig rig;

	// Token: 0x040017AD RID: 6061
	private Quaternion initialRotation;

	// Token: 0x040017AE RID: 6062
	private Vector3 spinAxis;

	// Token: 0x040017AF RID: 6063
	private float currentAngle;

	// Token: 0x040017B0 RID: 6064
	private float tickAngle;
}
