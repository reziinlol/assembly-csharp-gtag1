using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000DF2 RID: 3570
public class TransformOscillation : MonoBehaviour
{
	// Token: 0x06005757 RID: 22359 RVA: 0x001C3EF2 File Offset: 0x001C20F2
	private void Awake()
	{
		if (this.useRigidbodyMotion && !this.targetRigidbody)
		{
			this.targetRigidbody = base.GetComponent<Rigidbody>();
		}
		this.lastRotOffs = Quaternion.identity;
		this.startTime = Time.time;
		this.isRunning = false;
	}

	// Token: 0x06005758 RID: 22360 RVA: 0x001C3F32 File Offset: 0x001C2132
	private void OnEnable()
	{
		this.lastPosOffs = Vector3.zero;
		this.lastRotOffs = Quaternion.identity;
		if (this.startOnEnable)
		{
			this.StartOscillation();
			return;
		}
		this.isRunning = false;
	}

	// Token: 0x06005759 RID: 22361 RVA: 0x001C3F60 File Offset: 0x001C2160
	public void StartOscillation()
	{
		this.startTime = Time.time;
		this.isRunning = true;
	}

	// Token: 0x0600575A RID: 22362 RVA: 0x001C3F74 File Offset: 0x001C2174
	private float GetTimeSeconds()
	{
		if (!this.useServerTime)
		{
			return Time.timeSinceLevelLoad;
		}
		if (GorillaComputer.instance == null)
		{
			return Time.timeSinceLevelLoad;
		}
		this.dt = GorillaComputer.instance.GetServerTime();
		return (float)this.dt.Minute * 60f + (float)this.dt.Second + (float)this.dt.Millisecond / 1000f;
	}

	// Token: 0x0600575B RID: 22363 RVA: 0x001C3FE8 File Offset: 0x001C21E8
	private void ComputeOffsets(float t)
	{
		this.offsPos.x = this.PosAmp.x * Mathf.Sin(t * this.PosFreq.x);
		this.offsPos.y = this.PosAmp.y * Mathf.Sin(t * this.PosFreq.y);
		this.offsPos.z = this.PosAmp.z * Mathf.Sin(t * this.PosFreq.z);
		this.offsRot.x = this.RotAmp.x * Mathf.Sin(t * this.RotFreq.x);
		this.offsRot.y = this.RotAmp.y * Mathf.Sin(t * this.RotFreq.y);
		this.offsRot.z = this.RotAmp.z * Mathf.Sin(t * this.RotFreq.z);
	}

	// Token: 0x0600575C RID: 22364 RVA: 0x001C40EC File Offset: 0x001C22EC
	private void LateUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (this.useRigidbodyMotion && this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion rhs = Quaternion.Euler(this.offsRot);
		Vector3 a = transform.localPosition - this.lastPosOffs;
		Quaternion lhs = transform.localRotation * Quaternion.Inverse(this.lastRotOffs);
		transform.localPosition = a + this.offsPos;
		transform.localRotation = lhs * rhs;
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = rhs;
	}

	// Token: 0x0600575D RID: 22365 RVA: 0x001C41B0 File Offset: 0x001C23B0
	private void FixedUpdate()
	{
		if (!this.isRunning)
		{
			return;
		}
		if (this.useTimeLimit && Time.time - this.startTime >= this.timer)
		{
			return;
		}
		if (!this.useRigidbodyMotion || !this.targetRigidbody)
		{
			return;
		}
		float timeSeconds = this.GetTimeSeconds();
		this.ComputeOffsets(timeSeconds);
		Transform transform = base.transform;
		Quaternion quaternion = Quaternion.Euler(this.offsRot);
		Transform parent = transform.parent;
		Vector3 b = parent ? parent.TransformVector(this.lastPosOffs) : this.lastPosOffs;
		Quaternion rotation = parent ? (parent.rotation * this.lastRotOffs * Quaternion.Inverse(parent.rotation)) : this.lastRotOffs;
		Vector3 a = transform.position - b;
		Quaternion lhs = transform.rotation * Quaternion.Inverse(rotation);
		Vector3 b2 = parent ? parent.TransformVector(this.offsPos) : this.offsPos;
		Quaternion rhs = parent ? (parent.rotation * quaternion * Quaternion.Inverse(parent.rotation)) : quaternion;
		this.targetRigidbody.MovePosition(a + b2);
		this.targetRigidbody.MoveRotation(lhs * rhs);
		this.lastPosOffs = this.offsPos;
		this.lastRotOffs = quaternion;
	}

	// Token: 0x04006748 RID: 26440
	[SerializeField]
	private Vector3 PosAmp;

	// Token: 0x04006749 RID: 26441
	[SerializeField]
	private Vector3 PosFreq;

	// Token: 0x0400674A RID: 26442
	[SerializeField]
	private Vector3 RotAmp;

	// Token: 0x0400674B RID: 26443
	[SerializeField]
	private Vector3 RotFreq;

	// Token: 0x0400674C RID: 26444
	[SerializeField]
	private bool useServerTime;

	// Token: 0x0400674D RID: 26445
	[Header("Rigidbody Motion (optional)")]
	[Tooltip("If true and a Rigidbody is present, applies motion using Rigidbody.MovePosition/MoveRotation in FixedUpdate.")]
	[SerializeField]
	private bool useRigidbodyMotion;

	// Token: 0x0400674E RID: 26446
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x0400674F RID: 26447
	[Header("Activation Timer (optional)")]
	[Tooltip("If true, oscillation only runs for 'activeDurationSeconds' after OnEnable; otherwise it runs indefinitely.")]
	[SerializeField]
	private bool useTimeLimit;

	// Token: 0x04006750 RID: 26448
	[SerializeField]
	private float timer = 2f;

	// Token: 0x04006751 RID: 26449
	[Header("Start Behavior (optional)")]
	[Tooltip("If true, oscillation starts automatically on OnEnable(). If false, call StartOscillation() manually.")]
	[SerializeField]
	private bool startOnEnable = true;

	// Token: 0x04006752 RID: 26450
	private Vector3 lastPosOffs = Vector3.zero;

	// Token: 0x04006753 RID: 26451
	private Quaternion lastRotOffs = Quaternion.identity;

	// Token: 0x04006754 RID: 26452
	private Vector3 offsPos;

	// Token: 0x04006755 RID: 26453
	private Vector3 offsRot;

	// Token: 0x04006756 RID: 26454
	private DateTime dt;

	// Token: 0x04006757 RID: 26455
	private float startTime;

	// Token: 0x04006758 RID: 26456
	private bool isRunning;
}
