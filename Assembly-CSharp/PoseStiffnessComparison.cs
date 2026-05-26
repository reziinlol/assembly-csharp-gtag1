using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class PoseStiffnessComparison : MonoBehaviour
{
	// Token: 0x06000058 RID: 88 RVA: 0x000030BC File Offset: 0x000012BC
	private void Start()
	{
		this.m_timer = 0f;
		this.m_yA = this.BonesA.position.y;
		this.m_yB = this.BonesB.position.y;
	}

	// Token: 0x06000059 RID: 89 RVA: 0x000030F8 File Offset: 0x000012F8
	private void FixedUpdate()
	{
		BoingBones[] components = this.BonesA.GetComponents<BoingBones>();
		BoingBones[] components2 = this.BonesB.GetComponents<BoingBones>();
		Transform[] source = new Transform[]
		{
			this.BonesA.transform,
			this.BonesB.transform
		};
		float[] source2 = new float[]
		{
			this.m_yA,
			this.m_yB
		};
		IEnumerable<BoingBones> enumerable = components.Concat(components2);
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num = 0.5f * this.Run;
		this.m_timer += fixedDeltaTime;
		if (this.m_timer > this.Period + this.Rest)
		{
			this.m_timer = Mathf.Repeat(this.m_timer, this.Period + this.Rest);
			for (int i = 0; i < 2; i++)
			{
				Transform transform = source.ElementAt(i);
				float y = source2.ElementAt(i);
				Vector3 position = transform.position;
				position.y = y;
				position.z = -num;
				transform.position = position;
			}
			foreach (BoingBones boingBones in enumerable)
			{
				boingBones.Reboot();
			}
		}
		float num2 = Mathf.Min(1f, this.m_timer * MathUtil.InvSafe(this.Period));
		float num3 = 1f - Mathf.Pow(1f - num2, 1.5f);
		for (int j = 0; j < 2; j++)
		{
			Transform transform2 = source.ElementAt(j);
			float num4 = source2.ElementAt(j);
			Vector3 position2 = transform2.position;
			position2.y = num4 + 2f * Mathf.Sin(12.566371f * num3);
			position2.z = Mathf.Lerp(-num, num, num3);
			transform2.position = position2;
		}
	}

	// Token: 0x04000040 RID: 64
	public float Run = 11f;

	// Token: 0x04000041 RID: 65
	public float Tilt = 15f;

	// Token: 0x04000042 RID: 66
	public float Period = 3f;

	// Token: 0x04000043 RID: 67
	public float Rest = 3f;

	// Token: 0x04000044 RID: 68
	public Transform BonesA;

	// Token: 0x04000045 RID: 69
	public Transform BonesB;

	// Token: 0x04000046 RID: 70
	private float m_yA;

	// Token: 0x04000047 RID: 71
	private float m_yB;

	// Token: 0x04000048 RID: 72
	private float m_timer;
}
