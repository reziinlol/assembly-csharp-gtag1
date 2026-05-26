using System;
using UnityEngine;

// Token: 0x02000283 RID: 643
public class RadialGravity : MonoBehaviour
{
	// Token: 0x0600114F RID: 4431 RVA: 0x0005CF24 File Offset: 0x0005B124
	private void Start()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component == null)
		{
			return;
		}
		component.centerOfMass = new Vector3(0f, -3f, 0f);
		base.transform.localScale = Vector3.one * Random.Range(1f, 3f);
		component.useGravity = false;
		ConstantForce constantForce = base.gameObject.AddComponent<ConstantForce>();
		constantForce.force = Random.onUnitSphere * Physics.gravity.magnitude;
		if (Vector3.Dot(constantForce.force, Vector3.up) > 0f && Random.value > 0.33f)
		{
			constantForce.force = Random.onUnitSphere * Physics.gravity.magnitude;
		}
	}
}
