using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class D20_ShaderManager : MonoBehaviour
{
	// Token: 0x06000E8A RID: 3722 RVA: 0x0004F260 File Offset: 0x0004D460
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.lastPosition = base.transform.position;
		Renderer component = base.GetComponent<Renderer>();
		this.material = component.material;
		this.material.SetVector("_Velocity", this.velocity);
		base.StartCoroutine(this.UpdateVelocityCoroutine());
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0004F2C5 File Offset: 0x0004D4C5
	private IEnumerator UpdateVelocityCoroutine()
	{
		for (;;)
		{
			Vector3 position = base.transform.position;
			this.velocity = (position - this.lastPosition) / this.updateInterval;
			this.lastPosition = position;
			this.material.SetVector("_Velocity", this.velocity);
			yield return new WaitForSeconds(this.updateInterval);
		}
		yield break;
	}

	// Token: 0x0400116F RID: 4463
	private Rigidbody rb;

	// Token: 0x04001170 RID: 4464
	private Vector3 lastPosition;

	// Token: 0x04001171 RID: 4465
	public float updateInterval = 0.1f;

	// Token: 0x04001172 RID: 4466
	public Vector3 velocity;

	// Token: 0x04001173 RID: 4467
	private Material material;
}
