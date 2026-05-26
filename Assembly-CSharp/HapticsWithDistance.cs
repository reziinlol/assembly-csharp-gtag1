using System;
using UnityEngine;

// Token: 0x02000582 RID: 1410
[RequireComponent(typeof(SphereCollider))]
public class HapticsWithDistance : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060023B4 RID: 9140 RVA: 0x000C01CF File Offset: 0x000BE3CF
	private bool OnWrongLayer()
	{
		return base.gameObject.layer != 18;
	}

	// Token: 0x060023B5 RID: 9141 RVA: 0x000C01E3 File Offset: 0x000BE3E3
	public void SetVibrationMult(float mult)
	{
		this.vibrationMult = mult;
	}

	// Token: 0x060023B6 RID: 9142 RVA: 0x000C01EC File Offset: 0x000BE3EC
	public void FingerFlexVibrationMult(bool dummy, float mult)
	{
		this.SetVibrationMult(mult);
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000C01F5 File Offset: 0x000BE3F5
	private void Awake()
	{
		this.inverseColliderRadius = 1f / base.GetComponent<SphereCollider>().radius;
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000C0210 File Offset: 0x000BE410
	private void OnTriggerEnter(Collider other)
	{
		GorillaGrabber gorillaGrabber;
		if (other.TryGetComponent<GorillaGrabber>(out gorillaGrabber) && gorillaGrabber.enabled)
		{
			if (gorillaGrabber.IsLeftHand)
			{
				this.leftOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
				return;
			}
			if (gorillaGrabber.IsRightHand)
			{
				this.rightOfflineHand = gorillaGrabber.transform;
				TickSystem<object>.AddTickCallback(this);
			}
		}
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000C0264 File Offset: 0x000BE464
	private void OnTriggerExit(Collider other)
	{
		if (this.leftOfflineHand == other.transform)
		{
			this.leftOfflineHand = null;
			if (!this.rightOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
				return;
			}
		}
		else if (this.rightOfflineHand == other.transform)
		{
			this.rightOfflineHand = null;
			if (!this.leftOfflineHand)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}
	}

	// Token: 0x060023BA RID: 9146 RVA: 0x000C02CC File Offset: 0x000BE4CC
	private void OnDisable()
	{
		this.leftOfflineHand = null;
		this.rightOfflineHand = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060023BB RID: 9147 RVA: 0x000C02E2 File Offset: 0x000BE4E2
	// (set) Token: 0x060023BC RID: 9148 RVA: 0x000C02EA File Offset: 0x000BE4EA
	public bool TickRunning { get; set; }

	// Token: 0x060023BD RID: 9149 RVA: 0x000C02F4 File Offset: 0x000BE4F4
	public void Tick()
	{
		Vector3 position = base.transform.position;
		if (this.leftOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(true, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.leftOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
		if (this.rightOfflineHand)
		{
			GorillaTagger.Instance.StartVibration(false, this.vibrationMult * this.vibrationIntensityByDistance.Evaluate(Vector3.Distance(this.rightOfflineHand.position, position) * this.inverseColliderRadius), Time.deltaTime);
		}
	}

	// Token: 0x04002EDB RID: 11995
	[SerializeField]
	[Tooltip("X is the normalized distance and should start at 0 and end at 1. Y is the vibration amplitude and can be anywhere from 0-1.")]
	private AnimationCurve vibrationIntensityByDistance;

	// Token: 0x04002EDC RID: 11996
	private float inverseColliderRadius;

	// Token: 0x04002EDD RID: 11997
	private float vibrationMult = 1f;

	// Token: 0x04002EDE RID: 11998
	private Transform leftOfflineHand;

	// Token: 0x04002EDF RID: 11999
	private Transform rightOfflineHand;
}
