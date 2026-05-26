using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class ScaleSpring : MonoBehaviour
{
	// Token: 0x06000073 RID: 115 RVA: 0x00003C30 File Offset: 0x00001E30
	public void Tick()
	{
		this.m_targetScale = ((this.m_targetScale == ScaleSpring.kSmallScale) ? ScaleSpring.kLargeScale : ScaleSpring.kSmallScale);
		this.m_lastTickTime = Time.time;
		base.GetComponent<BoingEffector>().MoveDistance = ScaleSpring.kMoveDistance * ((this.m_targetScale == ScaleSpring.kSmallScale) ? -1f : 1f);
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00003C91 File Offset: 0x00001E91
	public void Start()
	{
		this.Tick();
		this.m_spring.Reset(this.m_targetScale * Vector3.one);
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00003CB4 File Offset: 0x00001EB4
	public void FixedUpdate()
	{
		if (Time.time - this.m_lastTickTime > ScaleSpring.kInterval)
		{
			this.Tick();
		}
		this.m_spring.TrackHalfLife(this.m_targetScale * Vector3.one, 6f, 0.05f, Time.fixedDeltaTime);
		base.transform.localScale = this.m_spring.Value;
		base.GetComponent<BoingEffector>().MoveDistance *= Mathf.Min(0.99f, 35f * Time.fixedDeltaTime);
	}

	// Token: 0x04000067 RID: 103
	private static readonly float kInterval = 2f;

	// Token: 0x04000068 RID: 104
	private static readonly float kSmallScale = 0.6f;

	// Token: 0x04000069 RID: 105
	private static readonly float kLargeScale = 2f;

	// Token: 0x0400006A RID: 106
	private static readonly float kMoveDistance = 30f;

	// Token: 0x0400006B RID: 107
	private Vector3Spring m_spring;

	// Token: 0x0400006C RID: 108
	private float m_targetScale;

	// Token: 0x0400006D RID: 109
	private float m_lastTickTime;
}
