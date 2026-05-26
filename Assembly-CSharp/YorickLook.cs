using System;
using UnityEngine;

// Token: 0x02000553 RID: 1363
public class YorickLook : MonoBehaviour
{
	// Token: 0x060022C0 RID: 8896 RVA: 0x000BACE3 File Offset: 0x000B8EE3
	private void Awake()
	{
		this.overlapRigs = new VRRig[20];
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x000BACF4 File Offset: 0x000B8EF4
	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			Vector3 rhs = this.rigs[i].tagSound.transform.position - base.transform.position;
			if (rhs.magnitude <= this.lookRadius)
			{
				float num4 = Vector3.Dot(-base.transform.up, rhs.normalized);
				if (num4 > num2)
				{
					this.overlapRigs[num3++] = this.rigs[i];
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 rhs = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, rhs);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 target = -base.transform.up;
		Vector3 target2 = -base.transform.up;
		if (this.lookTarget != null)
		{
			target = (this.lookTarget.position - this.leftEye.position).normalized;
			target2 = (this.lookTarget.position - this.rightEye.position).normalized;
		}
		Vector3 forward = Vector3.RotateTowards(this.leftEye.rotation * Vector3.forward, target, this.rotSpeed * 3.1415927f, 0f);
		Vector3 forward2 = Vector3.RotateTowards(this.rightEye.rotation * Vector3.forward, target2, this.rotSpeed * 3.1415927f, 0f);
		this.leftEye.rotation = Quaternion.LookRotation(forward);
		this.rightEye.rotation = Quaternion.LookRotation(forward2);
	}

	// Token: 0x04002DCB RID: 11723
	public Transform leftEye;

	// Token: 0x04002DCC RID: 11724
	public Transform rightEye;

	// Token: 0x04002DCD RID: 11725
	public Transform lookTarget;

	// Token: 0x04002DCE RID: 11726
	public float lookRadius = 0.5f;

	// Token: 0x04002DCF RID: 11727
	public VRRig[] rigs = new VRRig[20];

	// Token: 0x04002DD0 RID: 11728
	public VRRig[] overlapRigs;

	// Token: 0x04002DD1 RID: 11729
	public float rotSpeed = 1f;

	// Token: 0x04002DD2 RID: 11730
	public float lookAtAngleDegrees = 60f;
}
