using System;
using UnityEngine;

// Token: 0x020009EE RID: 2542
[DefaultExecutionOrder(100)]
public class TransformFollow : MonoBehaviour
{
	// Token: 0x0600410E RID: 16654 RVA: 0x0015BB80 File Offset: 0x00159D80
	private void Awake()
	{
		this.prevPos = base.transform.position;
		if (this.rotationOnly && base.transform.parent != null && base.transform.parent.GetComponent<TransformFollow>() != null)
		{
			this.forRigRecording = true;
		}
		if (this.forRigRecording)
		{
			this.parentFollow = base.transform.parent.GetComponent<TransformFollow>();
		}
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x0015BBF8 File Offset: 0x00159DF8
	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		if (!this.rotationOnly)
		{
			Vector3 a;
			Quaternion rotation;
			this.transformToFollow.GetPositionAndRotation(out a, out rotation);
			base.transform.SetPositionAndRotation(a + rotation * this.offset, rotation);
			return;
		}
		if (this.forRigRecording)
		{
			base.transform.localRotation = Quaternion.Inverse(this.parentFollow.transformToFollow.rotation) * this.transformToFollow.rotation;
			return;
		}
		base.transform.rotation = this.transformToFollow.rotation;
	}

	// Token: 0x040051B9 RID: 20921
	public Transform transformToFollow;

	// Token: 0x040051BA RID: 20922
	public Vector3 offset;

	// Token: 0x040051BB RID: 20923
	public Vector3 prevPos;

	// Token: 0x040051BC RID: 20924
	public bool rotationOnly;

	// Token: 0x040051BD RID: 20925
	private bool forRigRecording;

	// Token: 0x040051BE RID: 20926
	private TransformFollow parentFollow;
}
