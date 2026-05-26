using System;
using UnityEngine;

// Token: 0x0200060C RID: 1548
public class BuilderPortraitEyes : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002698 RID: 9880 RVA: 0x000CC512 File Offset: 0x000CA712
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.scale = base.transform.lossyScale.x;
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000CC531 File Offset: 0x000CA731
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.eyes.transform.position = this.eyeCenter.transform.position;
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000CC55C File Offset: 0x000CA75C
	public void SliceUpdate()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		Vector3 b = Vector3.ClampMagnitude(Vector3.ProjectOnPlane(GorillaTagger.Instance.headCollider.transform.position - this.eyeCenter.position, this.eyeCenter.forward), this.moveRadius * this.scale);
		this.eyes.transform.position = this.eyeCenter.position + b;
	}

	// Token: 0x04003207 RID: 12807
	[SerializeField]
	private Transform eyeCenter;

	// Token: 0x04003208 RID: 12808
	[SerializeField]
	private GameObject eyes;

	// Token: 0x04003209 RID: 12809
	[SerializeField]
	private float moveRadius = 0.5f;

	// Token: 0x0400320A RID: 12810
	private float scale = 1f;
}
