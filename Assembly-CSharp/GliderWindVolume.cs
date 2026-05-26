using System;
using UnityEngine;

// Token: 0x02000C9D RID: 3229
public class GliderWindVolume : MonoBehaviour
{
	// Token: 0x0600501A RID: 20506 RVA: 0x001A9B1F File Offset: 0x001A7D1F
	public void SetProperties(float speed, float accel, AnimationCurve svaCurve, Vector3 windDirection)
	{
		this.maxSpeed = speed;
		this.maxAccel = accel;
		this.speedVsAccelCurve.CopyFrom(svaCurve);
		this.localWindDirection = windDirection;
	}

	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x0600501B RID: 20507 RVA: 0x001A9B43 File Offset: 0x001A7D43
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	// Token: 0x0600501C RID: 20508 RVA: 0x001A9B58 File Offset: 0x001A7D58
	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float time = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float d = this.speedVsAccelCurve.Evaluate(time) * this.maxAccel;
		return windDirection * d;
	}

	// Token: 0x04006237 RID: 25143
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04006238 RID: 25144
	[SerializeField]
	private float maxAccel = 15f;

	// Token: 0x04006239 RID: 25145
	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x0400623A RID: 25146
	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
