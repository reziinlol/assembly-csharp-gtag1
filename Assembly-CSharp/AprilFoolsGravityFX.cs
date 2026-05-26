using System;
using System.Collections;
using GorillaExtensions;
using GorillaTag.Gravity;
using UnityEngine;

// Token: 0x02000C9E RID: 3230
public class AprilFoolsGravityFX : MonoBehaviour
{
	// Token: 0x0600501E RID: 20510 RVA: 0x001A9BFC File Offset: 0x001A7DFC
	private void Start()
	{
		BasicGravityZone basicGravityZone = base.gameObject.AddComponent<PersonalGravityZone>();
		MonkeGravityController component = base.GetComponent<MonkeGravityController>();
		basicGravityZone.AddTarget(component);
		component.SetPersonalGravityDirection(Random.insideUnitCircle.x0y().WithY(-0.5f).normalized);
		base.StartCoroutine(this.BackToNormal());
	}

	// Token: 0x0600501F RID: 20511 RVA: 0x001A9C50 File Offset: 0x001A7E50
	private IEnumerator BackToNormal()
	{
		yield return new WaitForSeconds(180f);
		PersonalGravityZone component = base.GetComponent<PersonalGravityZone>();
		MonkeGravityController component2 = base.GetComponent<MonkeGravityController>();
		component.RemoveTarget(component2);
		yield break;
	}
}
