using System;
using UnityEngine;

// Token: 0x02000AF4 RID: 2804
public class VelocityHelperTest : MonoBehaviour
{
	// Token: 0x060047DA RID: 18394 RVA: 0x00181675 File Offset: 0x0017F875
	private void Setup()
	{
		this.lastPosition = base.transform.position;
		this.lastVelocity = Vector3.zero;
		this.velocity = Vector3.zero;
		this.speed = 0f;
	}

	// Token: 0x060047DB RID: 18395 RVA: 0x001816A9 File Offset: 0x0017F8A9
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060047DC RID: 18396 RVA: 0x001816B4 File Offset: 0x0017F8B4
	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 b = (position - this.lastPosition) / deltaTime;
		this.velocity = Vector3.Lerp(this.lastVelocity, b, deltaTime);
		this.speed = this.velocity.magnitude;
		this.lastPosition = position;
		this.lastVelocity = b;
	}

	// Token: 0x060047DD RID: 18397 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x040059E3 RID: 23011
	public Vector3 velocity;

	// Token: 0x040059E4 RID: 23012
	public float speed;

	// Token: 0x040059E5 RID: 23013
	[Space]
	public Vector3 lastVelocity;

	// Token: 0x040059E6 RID: 23014
	public Vector3 lastPosition;

	// Token: 0x040059E7 RID: 23015
	[Space]
	[SerializeField]
	private float[] _deltaTimes = new float[5];
}
