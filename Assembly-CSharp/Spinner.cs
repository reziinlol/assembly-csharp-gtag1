using System;
using UnityEngine;

// Token: 0x02000029 RID: 41
public class Spinner : MonoBehaviour
{
	// Token: 0x06000096 RID: 150 RVA: 0x00004FF3 File Offset: 0x000031F3
	public void OnEnable()
	{
		this.m_angle = Random.Range(0f, 360f);
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000500C File Offset: 0x0000320C
	public void Update()
	{
		this.m_angle += this.Speed * 360f * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(0f, -this.m_angle, 0f);
	}

	// Token: 0x040000B8 RID: 184
	public float Speed;

	// Token: 0x040000B9 RID: 185
	private float m_angle;
}
