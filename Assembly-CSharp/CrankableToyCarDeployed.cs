using System;
using UnityEngine;

// Token: 0x020002B5 RID: 693
public class CrankableToyCarDeployed : MonoBehaviour
{
	// Token: 0x060011E1 RID: 4577 RVA: 0x0005FCC8 File Offset: 0x0005DEC8
	public void Deploy(CrankableToyCarHoldable holdable, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		this.holdable = holdable;
		holdable.OnCarDeployed();
		base.transform.position = launchPos;
		base.transform.rotation = launchRot;
		base.transform.localScale = holdable.transform.lossyScale;
		this.rb.linearVelocity = releaseVel;
		this.startedAtTimestamp = Time.time;
		this.expiresAtTimestamp = Time.time + lifetime;
		this.isRemote = isRemote;
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0005FD40 File Offset: 0x0005DF40
	private void Update()
	{
		if (!this.isRemote && Time.time > this.expiresAtTimestamp)
		{
			if (this.holdable != null)
			{
				this.holdable.OnCarReturned();
			}
			return;
		}
		if (!this.wheelDriver.hasCollision)
		{
			this.expiresAtTimestamp -= Time.deltaTime;
			if (!this.offGroundDrivingAudio.isPlaying)
			{
				this.offGroundDrivingAudio.GTPlay();
				this.drivingAudio.Stop();
			}
		}
		else if (!this.drivingAudio.isPlaying)
		{
			this.drivingAudio.GTPlay();
			this.offGroundDrivingAudio.Stop();
		}
		float time = Mathf.InverseLerp(this.startedAtTimestamp, this.expiresAtTimestamp, Time.time);
		float d = this.thrustCurve.Evaluate(time);
		this.wheelDriver.SetThrust(this.maxThrust * d);
	}

	// Token: 0x04001591 RID: 5521
	[SerializeField]
	private Rigidbody rb;

	// Token: 0x04001592 RID: 5522
	[SerializeField]
	private FakeWheelDriver wheelDriver;

	// Token: 0x04001593 RID: 5523
	[SerializeField]
	private Vector3 maxThrust;

	// Token: 0x04001594 RID: 5524
	[SerializeField]
	private AnimationCurve thrustCurve;

	// Token: 0x04001595 RID: 5525
	private float startedAtTimestamp;

	// Token: 0x04001596 RID: 5526
	private float expiresAtTimestamp;

	// Token: 0x04001597 RID: 5527
	private CrankableToyCarHoldable holdable;

	// Token: 0x04001598 RID: 5528
	[SerializeField]
	private AudioSource drivingAudio;

	// Token: 0x04001599 RID: 5529
	[SerializeField]
	private AudioSource offGroundDrivingAudio;

	// Token: 0x0400159A RID: 5530
	private bool isRemote;
}
