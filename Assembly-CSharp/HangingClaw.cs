using System;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
[RequireComponent(typeof(LineRenderer))]
public class HangingClaw : MonoBehaviourPostTick
{
	// Token: 0x060031F9 RID: 12793 RVA: 0x00112A14 File Offset: 0x00110C14
	protected void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		Vector3 position = base.transform.position;
		this.segmentCount = 4;
		float magnitude = (this.endTransform.position - position).magnitude;
		this.segmentCount = Mathf.Max(2, this.segmentCount);
		this.baseSegLen = magnitude / (float)this.segmentCount;
		this.ropeSegs = new HangingClaw.RopeSegment[this.segmentCount];
		this.invMass = new float[this.segmentCount];
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 p = Vector3.Lerp(position, this.endTransform.position, (float)i / (float)(this.segmentCount - 1));
			this.ropeSegs[i] = new HangingClaw.RopeSegment(p);
		}
		this.invMass[0] = 0f;
		for (int j = 1; j < this.segmentCount - 1; j++)
		{
			this.invMass[j] = 1f / Mathf.Max(0.0001f, this.segmentMassKg);
		}
		this.invMass[this.segmentCount - 1] = 1f / Mathf.Max(0.0001f, this.endMassKg);
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x00112B48 File Offset: 0x00110D48
	public override void PostTick()
	{
		this.Simulate();
		this.DrawRope();
		int num = this.segmentCount - 1;
		int num2 = this.segmentCount;
		this.endTransform.position = this.ropeSegs[num].pos;
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x00112B90 File Offset: 0x00110D90
	private void Simulate()
	{
		float num = this.baseSegLen;
		this.targetSegLenScaled = num * (1f + this.slackFraction);
		float d = 0.01111f;
		float f = Time.time * 0.5f;
		Vector3 b = this.gravity * d * d;
		Vector3 topPos = base.transform.position + new Vector3(0f, 0.012f * Mathf.Sin(f), 0.02f * Mathf.Cos(f));
		for (int i = 1; i < this.segmentCount; i++)
		{
			Vector3 a = this.ropeSegs[i].pos - this.ropeSegs[i].posOld;
			this.ropeSegs[i].posOld = this.ropeSegs[i].pos;
			HangingClaw.RopeSegment[] array = this.ropeSegs;
			int num2 = i;
			array[num2].pos = array[num2].pos + (a * this.velocityDamping + b);
		}
		int num3 = 3;
		for (int j = 0; j < num3; j++)
		{
			this.ApplyConstraints(topPos);
		}
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x00112CCC File Offset: 0x00110ECC
	private void ApplyConstraints(Vector3 topPos)
	{
		this.ropeSegs[0].pos = topPos;
		this.ropeSegs[0].posOld = topPos;
		float stiffness = Mathf.Clamp01(this.ropeStiffness);
		for (int i = 0; i < this.segmentCount - 1; i++)
		{
			this.ApplyConstraintSegment(ref this.ropeSegs[i], ref this.ropeSegs[i + 1], this.invMass[i], this.invMass[i + 1], stiffness);
		}
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x00112D50 File Offset: 0x00110F50
	private void ApplyConstraintSegment(ref HangingClaw.RopeSegment a, ref HangingClaw.RopeSegment b, float wA, float wB, float stiffness)
	{
		Vector3 a2 = b.pos - a.pos;
		float magnitude = a2.magnitude;
		if (magnitude < 1E-06f)
		{
			return;
		}
		float num = magnitude - this.targetSegLenScaled;
		if (Mathf.Abs(num) < 1E-06f)
		{
			return;
		}
		Vector3 a3 = a2 / magnitude;
		float num2 = wA + wB;
		if (num2 <= 0f)
		{
			return;
		}
		Vector3 a4 = a3 * (num * stiffness);
		a.pos += a4 * (wA / num2);
		b.pos += -a4 * (wB / num2);
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x00112E04 File Offset: 0x00111004
	private void DrawRope()
	{
		if (this.lineRenderer == null)
		{
			return;
		}
		this.lineRenderer.positionCount = this.segmentCount;
		for (int i = 0; i < this.segmentCount; i++)
		{
			Vector3 pos = this.ropeSegs[i].pos;
			if (this.heightCap && pos.y > this.heightCap.position.y)
			{
				pos.y = this.heightCap.position.y;
			}
			this.lineRenderer.SetPosition(i, this.ropeSegs[i].pos);
		}
	}

	// Token: 0x040040E4 RID: 16612
	public Transform endTransform;

	// Token: 0x040040E5 RID: 16613
	public Transform heightCap;

	// Token: 0x040040E6 RID: 16614
	private int segmentCount = 6;

	// Token: 0x040040E7 RID: 16615
	public float segmentMassKg = 1f;

	// Token: 0x040040E8 RID: 16616
	public float endMassKg = 5f;

	// Token: 0x040040E9 RID: 16617
	public float ropeStiffness = 0.9f;

	// Token: 0x040040EA RID: 16618
	public float slackFraction = 0.02f;

	// Token: 0x040040EB RID: 16619
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x040040EC RID: 16620
	public float velocityDamping = 0.98f;

	// Token: 0x040040ED RID: 16621
	private float maxY;

	// Token: 0x040040EE RID: 16622
	private LineRenderer lineRenderer;

	// Token: 0x040040EF RID: 16623
	private HangingClaw.RopeSegment[] ropeSegs;

	// Token: 0x040040F0 RID: 16624
	private float baseSegLen;

	// Token: 0x040040F1 RID: 16625
	private float targetSegLenScaled;

	// Token: 0x040040F2 RID: 16626
	private float[] invMass;

	// Token: 0x020007A5 RID: 1957
	public struct RopeSegment
	{
		// Token: 0x06003200 RID: 12800 RVA: 0x00112F1B File Offset: 0x0011111B
		public RopeSegment(Vector3 p)
		{
			this.pos = p;
			this.posOld = p;
		}

		// Token: 0x040040F3 RID: 16627
		public Vector3 pos;

		// Token: 0x040040F4 RID: 16628
		public Vector3 posOld;
	}
}
