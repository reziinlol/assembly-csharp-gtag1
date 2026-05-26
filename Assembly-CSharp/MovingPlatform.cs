using System;
using GTMathUtil;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000906 RID: 2310
public class MovingPlatform : BasePlatform
{
	// Token: 0x06003C55 RID: 15445 RVA: 0x001492D0 File Offset: 0x001474D0
	public float InitTimeOffset()
	{
		return this.startPercentage * this.cycleLength;
	}

	// Token: 0x06003C56 RID: 15446 RVA: 0x001492DF File Offset: 0x001474DF
	private long InitTimeOffsetMs()
	{
		return (long)(this.InitTimeOffset() * 1000f);
	}

	// Token: 0x06003C57 RID: 15447 RVA: 0x001492EE File Offset: 0x001474EE
	private long NetworkTimeMs()
	{
		if (PhotonNetwork.InRoom)
		{
			return (long)((ulong)(PhotonNetwork.ServerTimestamp + int.MinValue) + (ulong)this.InitTimeOffsetMs());
		}
		return (long)(Time.time * 1000f);
	}

	// Token: 0x06003C58 RID: 15448 RVA: 0x00149317 File Offset: 0x00147517
	private long CycleLengthMs()
	{
		return (long)(this.cycleLength * 1000f);
	}

	// Token: 0x06003C59 RID: 15449 RVA: 0x00149328 File Offset: 0x00147528
	public double PlatformTime()
	{
		long num = this.NetworkTimeMs();
		long num2 = this.CycleLengthMs();
		return (double)(num - num / num2 * num2) / 1000.0;
	}

	// Token: 0x06003C5A RID: 15450 RVA: 0x00149353 File Offset: 0x00147553
	public int CycleCount()
	{
		return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
	}

	// Token: 0x06003C5B RID: 15451 RVA: 0x00149364 File Offset: 0x00147564
	public float CycleCompletionPercent()
	{
		float num = (float)(this.PlatformTime() / (double)this.cycleLength);
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.startDelay > 0f)
		{
			float num2 = this.startDelay / this.cycleLength;
			if (num <= num2)
			{
				num = 0f;
			}
			else
			{
				num = (num - num2) / (1f - num2);
			}
		}
		return num;
	}

	// Token: 0x06003C5C RID: 15452 RVA: 0x001493C6 File Offset: 0x001475C6
	public bool CycleForward()
	{
		return (this.CycleCount() + (this.startNextCycle ? 1 : 0)) % 2 == 0;
	}

	// Token: 0x06003C5D RID: 15453 RVA: 0x001493E0 File Offset: 0x001475E0
	private void Awake()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.rb = base.GetComponent<Rigidbody>();
		this.initLocalRotation = base.transform.localRotation;
		if (this.pivot != null)
		{
			this.initOffset = this.pivot.transform.position - this.startXf.transform.position;
		}
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06003C5E RID: 15454 RVA: 0x001494B0 File Offset: 0x001476B0
	private void OnEnable()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		base.transform.localRotation = this.initLocalRotation;
		this.startPos = this.startXf.position;
		this.endPos = this.endXf.position;
		this.startRot = this.startXf.rotation;
		this.endRot = this.endXf.rotation;
		this.platformInitLocalPos = base.transform.localPosition;
		this.currT = this.startPercentage;
	}

	// Token: 0x06003C5F RID: 15455 RVA: 0x00149539 File Offset: 0x00147739
	private Vector3 UpdatePointToPoint()
	{
		return Vector3.Lerp(this.startPos, this.endPos, this.smoothedPercent);
	}

	// Token: 0x06003C60 RID: 15456 RVA: 0x00149554 File Offset: 0x00147754
	private Vector3 UpdateArc()
	{
		float angle = Mathf.Lerp(this.rotateStartAmt, this.rotateStartAmt + this.rotateAmt, this.smoothedPercent);
		Quaternion quaternion = this.initLocalRotation;
		Vector3 b = Quaternion.AngleAxis(angle, Vector3.forward) * this.initOffset;
		return this.pivot.transform.position + b;
	}

	// Token: 0x06003C61 RID: 15457 RVA: 0x001495B2 File Offset: 0x001477B2
	private Quaternion UpdateRotation()
	{
		return Quaternion.Slerp(this.startRot, this.endRot, this.smoothedPercent);
	}

	// Token: 0x06003C62 RID: 15458 RVA: 0x001495CB File Offset: 0x001477CB
	private Quaternion UpdateContinuousRotation()
	{
		return Quaternion.AngleAxis(this.smoothedPercent * 360f, Vector3.up) * base.transform.parent.rotation;
	}

	// Token: 0x06003C63 RID: 15459 RVA: 0x001495F8 File Offset: 0x001477F8
	private void SetupContext()
	{
		double time = PhotonNetwork.Time;
		if (this.lastServerTime == time)
		{
			this.dtSinceServerUpdate += Time.fixedDeltaTime;
		}
		else
		{
			this.dtSinceServerUpdate = 0f;
			this.lastServerTime = time;
		}
		float num = this.currT;
		this.currT = this.CycleCompletionPercent();
		this.currForward = this.CycleForward();
		this.percent = this.currT;
		if (this.reverseDirOnCycle)
		{
			this.percent = (this.currForward ? this.currT : (1f - this.currT));
		}
		if (this.reverseDir)
		{
			this.percent = 1f - this.percent;
		}
		this.smoothedPercent = this.percent;
		this.lastNT = time;
		this.lastT = Time.time;
	}

	// Token: 0x06003C64 RID: 15460 RVA: 0x001496C8 File Offset: 0x001478C8
	private void Update()
	{
		if (this.platformType == MovingPlatform.PlatformType.Child)
		{
			return;
		}
		this.SetupContext();
		Vector3 vector = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		bool flag = false;
		switch (this.platformType)
		{
		case MovingPlatform.PlatformType.PointToPoint:
			vector = this.UpdatePointToPoint();
			break;
		case MovingPlatform.PlatformType.Arc:
			vector = this.UpdateArc();
			flag = true;
			break;
		case MovingPlatform.PlatformType.Rotation:
			quaternion = this.UpdateRotation();
			flag = true;
			break;
		case MovingPlatform.PlatformType.ContinuousRotation:
			quaternion = this.UpdateContinuousRotation();
			flag = true;
			break;
		}
		if (!this.debugMovement)
		{
			this.lastPos = this.rb.position;
			this.lastRot = this.rb.rotation;
			if (this.platformType != MovingPlatform.PlatformType.Rotation)
			{
				this.rb.MovePosition(vector);
			}
			if (flag)
			{
				this.rb.MoveRotation(quaternion);
			}
		}
		else
		{
			this.lastPos = base.transform.position;
			this.lastRot = base.transform.rotation;
			base.transform.position = vector;
			if (flag)
			{
				base.transform.rotation = quaternion;
			}
		}
		this.deltaPosition = vector - this.lastPos;
	}

	// Token: 0x06003C65 RID: 15461 RVA: 0x001497E9 File Offset: 0x001479E9
	public Vector3 ThisFrameMovement()
	{
		return this.deltaPosition;
	}

	// Token: 0x04004CE6 RID: 19686
	public MovingPlatform.PlatformType platformType;

	// Token: 0x04004CE7 RID: 19687
	public float cycleLength;

	// Token: 0x04004CE8 RID: 19688
	public float smoothingHalflife = 0.1f;

	// Token: 0x04004CE9 RID: 19689
	public float rotateStartAmt;

	// Token: 0x04004CEA RID: 19690
	public float rotateAmt;

	// Token: 0x04004CEB RID: 19691
	public bool reverseDirOnCycle = true;

	// Token: 0x04004CEC RID: 19692
	public bool reverseDir;

	// Token: 0x04004CED RID: 19693
	private CriticalSpringDamper springCD = new CriticalSpringDamper();

	// Token: 0x04004CEE RID: 19694
	private Rigidbody rb;

	// Token: 0x04004CEF RID: 19695
	public Transform startXf;

	// Token: 0x04004CF0 RID: 19696
	public Transform endXf;

	// Token: 0x04004CF1 RID: 19697
	public Vector3 platformInitLocalPos;

	// Token: 0x04004CF2 RID: 19698
	private Vector3 startPos;

	// Token: 0x04004CF3 RID: 19699
	private Vector3 endPos;

	// Token: 0x04004CF4 RID: 19700
	private Quaternion startRot;

	// Token: 0x04004CF5 RID: 19701
	private Quaternion endRot;

	// Token: 0x04004CF6 RID: 19702
	public float startPercentage;

	// Token: 0x04004CF7 RID: 19703
	public float startDelay;

	// Token: 0x04004CF8 RID: 19704
	public bool startNextCycle;

	// Token: 0x04004CF9 RID: 19705
	public Transform pivot;

	// Token: 0x04004CFA RID: 19706
	private Quaternion initLocalRotation;

	// Token: 0x04004CFB RID: 19707
	private Vector3 initOffset;

	// Token: 0x04004CFC RID: 19708
	private float currT;

	// Token: 0x04004CFD RID: 19709
	private float percent;

	// Token: 0x04004CFE RID: 19710
	private float smoothedPercent = -1f;

	// Token: 0x04004CFF RID: 19711
	private bool currForward;

	// Token: 0x04004D00 RID: 19712
	private float dtSinceServerUpdate;

	// Token: 0x04004D01 RID: 19713
	private double lastServerTime;

	// Token: 0x04004D02 RID: 19714
	public Vector3 currentVelocity;

	// Token: 0x04004D03 RID: 19715
	public Vector3 rotationalAxis;

	// Token: 0x04004D04 RID: 19716
	public float angularVelocity;

	// Token: 0x04004D05 RID: 19717
	public Vector3 rotationPivot;

	// Token: 0x04004D06 RID: 19718
	public Vector3 lastPos;

	// Token: 0x04004D07 RID: 19719
	public Quaternion lastRot;

	// Token: 0x04004D08 RID: 19720
	public Vector3 deltaPosition;

	// Token: 0x04004D09 RID: 19721
	public bool debugMovement;

	// Token: 0x04004D0A RID: 19722
	private double lastNT;

	// Token: 0x04004D0B RID: 19723
	private float lastT;

	// Token: 0x02000907 RID: 2311
	public enum PlatformType
	{
		// Token: 0x04004D0D RID: 19725
		PointToPoint,
		// Token: 0x04004D0E RID: 19726
		Arc,
		// Token: 0x04004D0F RID: 19727
		Rotation,
		// Token: 0x04004D10 RID: 19728
		Child,
		// Token: 0x04004D11 RID: 19729
		ContinuousRotation
	}
}
