using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class CurveBall : MonoBehaviour
{
	// Token: 0x0600007D RID: 125 RVA: 0x00004218 File Offset: 0x00002418
	public void Reset()
	{
		float f = Random.Range(0f, MathUtil.TwoPi);
		float num = Mathf.Cos(f);
		float num2 = Mathf.Sin(f);
		this.m_speedX = 40f * num;
		this.m_speedZ = 40f * num2;
		this.m_timer = 0f;
		Vector3 position = base.transform.position;
		position.x = -10f * num;
		position.z = -10f * num2;
		base.transform.position = position;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x0000429A File Offset: 0x0000249A
	public void Start()
	{
		this.Reset();
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000042A4 File Offset: 0x000024A4
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_timer > this.Interval)
		{
			this.Reset();
		}
		Vector3 position = base.transform.position;
		position.x += this.m_speedX * deltaTime;
		position.z += this.m_speedZ * deltaTime;
		base.transform.position = position;
		this.m_timer += deltaTime;
	}

	// Token: 0x0400007B RID: 123
	public float Interval = 2f;

	// Token: 0x0400007C RID: 124
	private float m_speedX;

	// Token: 0x0400007D RID: 125
	private float m_speedZ;

	// Token: 0x0400007E RID: 126
	private float m_timer;
}
