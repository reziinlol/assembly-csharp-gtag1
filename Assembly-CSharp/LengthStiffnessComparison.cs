using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class LengthStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000055 RID: 85 RVA: 0x00002ECF File Offset: 0x000010CF
	private void Start()
	{
		this.m_timer = 0f;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00002EDC File Offset: 0x000010DC
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] array = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			foreach (Transform transform in array)
			{
				Vector3 position = transform.position;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 6f);
		foreach (Transform transform2 in array)
		{
			Vector3 position2 = transform2.position;
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
			transform2.rotation = Quaternion.AngleAxis(this.Tilt * (1f - num3), Vector3.right);
		}
	}

	// Token: 0x04000039 RID: 57
	public float Run = 11f;

	// Token: 0x0400003A RID: 58
	public float Tilt = 15f;

	// Token: 0x0400003B RID: 59
	public float Period = 3f;

	// Token: 0x0400003C RID: 60
	public float Rest = 3f;

	// Token: 0x0400003D RID: 61
	public Transform BonesA;

	// Token: 0x0400003E RID: 62
	public Transform BonesB;

	// Token: 0x0400003F RID: 63
	private float m_timer;
}
