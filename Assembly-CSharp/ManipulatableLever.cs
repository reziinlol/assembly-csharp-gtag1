using System;
using UnityEngine;

// Token: 0x02000529 RID: 1321
public class ManipulatableLever : ManipulatableObject
{
	// Token: 0x06002145 RID: 8517 RVA: 0x000B194B File Offset: 0x000AFB4B
	private void Awake()
	{
		this.localSpace = base.transform.worldToLocalMatrix;
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x000B1960 File Offset: 0x000AFB60
	protected override bool ShouldHandDetach(GameObject hand)
	{
		Vector3 position = this.leverGrip.position;
		Vector3 position2 = hand.transform.position;
		return Vector3.SqrMagnitude(position - position2) > this.breakDistance * this.breakDistance;
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x000B19A0 File Offset: 0x000AFBA0
	protected override void OnHeldUpdate(GameObject hand)
	{
		Vector3 position = hand.transform.position;
		Vector3 upwards = Vector3.Normalize(this.localSpace.MultiplyPoint3x4(position) - base.transform.localPosition);
		Vector3 eulerAngles = Quaternion.LookRotation(Vector3.forward, upwards).eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z -= 360f;
		}
		else if (eulerAngles.z < -180f)
		{
			eulerAngles.z += 360f;
		}
		eulerAngles.z = Mathf.Clamp(eulerAngles.z, this.minAngle, this.maxAngle);
		base.transform.localEulerAngles = eulerAngles;
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x000B1A58 File Offset: 0x000AFC58
	public void SetValue(float value)
	{
		float z = Mathf.Lerp(this.minAngle, this.maxAngle, value);
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		localEulerAngles.z = z;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x000B1A98 File Offset: 0x000AFC98
	public void SetNotch(int notchValue)
	{
		if (this.notches == null)
		{
			return;
		}
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (leverNotch.value == notchValue)
			{
				this.SetValue(Mathf.Lerp(leverNotch.minAngleValue, leverNotch.maxAngleValue, 0.5f));
				return;
			}
		}
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x000B1AF0 File Offset: 0x000AFCF0
	public float GetValue()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (localEulerAngles.z > 180f)
		{
			localEulerAngles.z -= 360f;
		}
		else if (localEulerAngles.z < -180f)
		{
			localEulerAngles.z += 360f;
		}
		return Mathf.InverseLerp(this.minAngle, this.maxAngle, localEulerAngles.z);
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x000B1B5C File Offset: 0x000AFD5C
	public int GetNotch()
	{
		if (this.notches == null)
		{
			return 0;
		}
		float value = this.GetValue();
		foreach (ManipulatableLever.LeverNotch leverNotch in this.notches)
		{
			if (value >= leverNotch.minAngleValue && value <= leverNotch.maxAngleValue)
			{
				return leverNotch.value;
			}
		}
		return 0;
	}

	// Token: 0x04002C01 RID: 11265
	[SerializeField]
	private float breakDistance = 0.2f;

	// Token: 0x04002C02 RID: 11266
	[SerializeField]
	private Transform leverGrip;

	// Token: 0x04002C03 RID: 11267
	[SerializeField]
	private float maxAngle = 22.5f;

	// Token: 0x04002C04 RID: 11268
	[SerializeField]
	private float minAngle = -22.5f;

	// Token: 0x04002C05 RID: 11269
	[SerializeField]
	private ManipulatableLever.LeverNotch[] notches;

	// Token: 0x04002C06 RID: 11270
	private Matrix4x4 localSpace;

	// Token: 0x0200052A RID: 1322
	[Serializable]
	public class LeverNotch
	{
		// Token: 0x04002C07 RID: 11271
		public float minAngleValue;

		// Token: 0x04002C08 RID: 11272
		public float maxAngleValue;

		// Token: 0x04002C09 RID: 11273
		public int value;
	}
}
