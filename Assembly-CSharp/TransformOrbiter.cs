using System;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000DF0 RID: 3568
public class TransformOrbiter : MonoBehaviour
{
	// Token: 0x06005750 RID: 22352 RVA: 0x001C3C30 File Offset: 0x001C1E30
	private void Start()
	{
		TransformOrbiter.<Start>d__5 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<TransformOrbiter.<Start>d__5>(ref <Start>d__);
	}

	// Token: 0x06005751 RID: 22353 RVA: 0x001C3C68 File Offset: 0x001C1E68
	private void LateUpdate()
	{
		double totalSeconds = (GorillaComputer.instance.GetServerTime() - this.anchor).TotalSeconds;
		double num = (double)this.orbit.x * Math.Sin(totalSeconds * this.speed);
		double num2 = (double)this.orbit.y * Math.Cos(totalSeconds * this.speed);
		double num3 = (double)this.orbit.z * Math.Cos(totalSeconds * this.speed);
		base.transform.position = this.barycenter.position + this.translation + new Vector3((float)num, (float)num2, (float)num3);
	}

	// Token: 0x06005752 RID: 22354 RVA: 0x001C3D18 File Offset: 0x001C1F18
	private bool validateBarycenter()
	{
		return this.validateBarycenter(base.transform);
	}

	// Token: 0x06005753 RID: 22355 RVA: 0x001C3D28 File Offset: 0x001C1F28
	private bool validateBarycenter(Transform t)
	{
		if (this.barycenter == null)
		{
			Debug.LogError("The Barycenter cannot be null!");
			return false;
		}
		if (this.barycenter == t)
		{
			Debug.LogError("You cannot use the TransformOrbiter's own transform, or one nested below it, as its Barycenter!");
			return false;
		}
		for (int i = 0; i < t.childCount; i++)
		{
			if (!this.validateBarycenter(t.GetChild(i)))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400673F RID: 26431
	[SerializeField]
	private Transform barycenter;

	// Token: 0x04006740 RID: 26432
	[SerializeField]
	private Vector3 orbit;

	// Token: 0x04006741 RID: 26433
	[SerializeField]
	private Vector3 translation;

	// Token: 0x04006742 RID: 26434
	[SerializeField]
	[Range(0.01f, 5f)]
	private double speed = 1.0;

	// Token: 0x04006743 RID: 26435
	private DateTime anchor = new DateTime(2026, 4, 1);
}
