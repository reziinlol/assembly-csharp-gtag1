using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001BD RID: 445
public class MetroSpotlight : MonoBehaviour
{
	// Token: 0x06000BE5 RID: 3045 RVA: 0x00040E2C File Offset: 0x0003F02C
	public void Tick()
	{
		if (!this._light)
		{
			return;
		}
		if (!this._target)
		{
			return;
		}
		this._time += this.speed * Time.deltaTime * Time.deltaTime;
		Vector3 position = this._target.position;
		Vector3 normalized = (position - this._light.position).normalized;
		Vector3 vector = Vector3.Cross(normalized, this._blimp.forward);
		Vector3 yDir = Vector3.Cross(normalized, vector);
		Vector3 worldPosition = MetroSpotlight.Figure8(position, vector, yDir, this._radius, this._time, this._offset, this._theta);
		this._light.LookAt(worldPosition);
	}

	// Token: 0x06000BE6 RID: 3046 RVA: 0x00040EE0 File Offset: 0x0003F0E0
	private static Vector3 Figure8(Vector3 origin, Vector3 xDir, Vector3 yDir, float scale, float t, float offset, float theta)
	{
		float num = 2f / (3f - Mathf.Cos(2f * (t + offset)));
		float d = scale * num * Mathf.Cos(t + offset);
		float d2 = scale * num * Mathf.Sin(2f * (t + offset)) / 2f;
		Vector3 axis = Vector3.Cross(xDir, yDir);
		Quaternion rotation = Quaternion.AngleAxis(theta, axis);
		xDir = rotation * xDir;
		yDir = rotation * yDir;
		Vector3 b = xDir * d + yDir * d2;
		return origin + b;
	}

	// Token: 0x04000E7C RID: 3708
	[SerializeField]
	private Transform _blimp;

	// Token: 0x04000E7D RID: 3709
	[SerializeField]
	private Transform _light;

	// Token: 0x04000E7E RID: 3710
	[SerializeField]
	private Transform _target;

	// Token: 0x04000E7F RID: 3711
	[FormerlySerializedAs("_scale")]
	[SerializeField]
	private float _radius = 1f;

	// Token: 0x04000E80 RID: 3712
	[SerializeField]
	private float _offset;

	// Token: 0x04000E81 RID: 3713
	[SerializeField]
	private float _theta;

	// Token: 0x04000E82 RID: 3714
	public float speed = 16f;

	// Token: 0x04000E83 RID: 3715
	[Space]
	private float _time;
}
