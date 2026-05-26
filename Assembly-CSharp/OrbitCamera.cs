using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x0600006F RID: 111 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Start()
	{
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00003B84 File Offset: 0x00001D84
	public void Update()
	{
		this.m_phase += OrbitCamera.kOrbitSpeed * MathUtil.TwoPi * Time.deltaTime;
		base.transform.position = new Vector3(-4f * Mathf.Cos(this.m_phase), 6f, 4f * Mathf.Sin(this.m_phase));
		base.transform.rotation = Quaternion.LookRotation((new Vector3(0f, 3f, 0f) - base.transform.position).normalized);
	}

	// Token: 0x04000065 RID: 101
	private static readonly float kOrbitSpeed = 0.01f;

	// Token: 0x04000066 RID: 102
	private float m_phase;
}
