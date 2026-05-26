using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000A29 RID: 2601
[DisallowMultipleComponent]
public class VerletLine : MonoBehaviour
{
	// Token: 0x06004283 RID: 17027 RVA: 0x0016370C File Offset: 0x0016190C
	private void Awake()
	{
		this._nodes = new VerletLine.LineNode[this.segmentNumber];
		this._positions = new Vector3[this.segmentNumber];
		for (int i = 0; i < this.segmentNumber; i++)
		{
			float t = (float)i / (float)(this.segmentNumber - 1);
			Vector3 vector = Vector3.Lerp(this.lineStart.position, this.lineEnd.position, t);
			this._nodes[i] = new VerletLine.LineNode
			{
				position = vector,
				lastPosition = vector,
				acceleration = this.gravity
			};
		}
		this.line.positionCount = this._nodes.Length;
		this.endRigidbody = this.lineEnd.GetComponent<Rigidbody>();
		if (this.endRigidbody)
		{
			this.endRigidbody.maxLinearVelocity = this.endMaxSpeed;
			this.endRigidbodyParent = this.endRigidbody.transform.parent;
			this.rigidBodyStartingLocalPosition = this.endRigidbody.transform.localPosition;
			this.endRigidbody.transform.parent = null;
			this.endRigidbody.gameObject.SetActive(false);
		}
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06004284 RID: 17028 RVA: 0x0016384C File Offset: 0x00161A4C
	private void OnEnable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(true);
			this.endRigidbody.transform.localPosition = this.endRigidbodyParent.TransformPoint(this.rigidBodyStartingLocalPosition);
		}
	}

	// Token: 0x06004285 RID: 17029 RVA: 0x00163898 File Offset: 0x00161A98
	private void OnDisable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004286 RID: 17030 RVA: 0x001638B8 File Offset: 0x00161AB8
	public void SetLength(float total, float delay = 0f)
	{
		this.segmentTargetLength = total / (float)this.segmentNumber;
		if (this.segmentTargetLength < this.segmentMinLength)
		{
			this.segmentTargetLength = this.segmentMinLength;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06004287 RID: 17031 RVA: 0x00163920 File Offset: 0x00161B20
	public void AddSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength + amount;
		if (this.segmentTargetLength <= 0f)
		{
			return;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x0016397C File Offset: 0x00161B7C
	public void RemoveSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength - amount;
		if (this.segmentTargetLength <= this.segmentMinLength)
		{
			this.segmentTargetLength = (this.segmentLength = this.segmentMinLength);
			return;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06004289 RID: 17033 RVA: 0x001639D1 File Offset: 0x00161BD1
	private IEnumerator ResizeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield break;
	}

	// Token: 0x0600428A RID: 17034 RVA: 0x001639E0 File Offset: 0x00161BE0
	private void Update()
	{
		if (this.segmentLength.Approx(this.segmentTargetLength, 0.1f))
		{
			this.segmentLength = this.segmentTargetLength;
			return;
		}
		this.segmentLength = Mathf.Lerp(this.segmentLength, this.segmentTargetLength, this.resizeSpeed * this.resizeScale * Time.deltaTime);
		if (this.scaleLineWidth)
		{
			this.line.widthMultiplier = base.transform.lossyScale.x;
		}
	}

	// Token: 0x0600428B RID: 17035 RVA: 0x00163A60 File Offset: 0x00161C60
	public void ForceTotalLength(float totalLength)
	{
		float num = totalLength / (float)((this.segmentNumber < 1) ? 1 : this.segmentNumber);
		this.segmentLength = (this.segmentTargetLength = num);
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x0600428C RID: 17036 RVA: 0x00163AA8 File Offset: 0x00161CA8
	private void FixedUpdate()
	{
		for (int i = 0; i < this._nodes.Length; i++)
		{
			VerletLine.Simulate(ref this._nodes[i], Time.fixedDeltaTime);
		}
		for (int j = 0; j < this.simIterations; j++)
		{
			for (int k = 0; k < this._nodes.Length - 1; k++)
			{
				VerletLine.LimitDistance(ref this._nodes[k], ref this._nodes[k + 1], this.segmentLength);
			}
		}
		this._nodes[0].position = this.lineStart.position;
		if (this.endRigidbody)
		{
			if (this.onlyPullAtEdges)
			{
				if ((this.endRigidbody.transform.position - this.lineStart.position).IsLongerThan(this.totalLineLength))
				{
					Vector3 a = this.lineStart.position + (this.endRigidbody.transform.position - this.lineStart.position).normalized * this.totalLineLength;
					this.endRigidbody.linearVelocity += (a - this.endRigidbody.transform.position) / Time.fixedDeltaTime;
					if (this.endRigidbody.linearVelocity.IsLongerThan(this.endMaxSpeed))
					{
						this.endRigidbody.linearVelocity = this.endRigidbody.linearVelocity.normalized * this.endMaxSpeed;
					}
				}
			}
			else
			{
				VerletLine.LineNode[] nodes = this._nodes;
				Vector3 force = (nodes[nodes.Length - 1].position - this.lineEnd.position) * (this.tension * this.tensionScale);
				Quaternion rotation = this.endRigidbody.rotation;
				VerletLine.LineNode[] nodes2 = this._nodes;
				Vector3 position = nodes2[nodes2.Length - 1].position;
				VerletLine.LineNode[] nodes3 = this._nodes;
				Quaternion.LookRotation(position - nodes3[nodes3.Length - 2].position);
				if (!this.endRigidbody.isKinematic)
				{
					this.endRigidbody.AddForceAtPosition(force, this.endRigidbody.transform.TransformPoint(this.endLineAnchorLocalPosition));
				}
			}
		}
		VerletLine.LineNode[] nodes4 = this._nodes;
		nodes4[nodes4.Length - 1].position = this.lineEnd.position;
		for (int l = 0; l < this._nodes.Length; l++)
		{
			this._positions[l] = this._nodes[l].position;
		}
		this.line.SetPositions(this._positions);
	}

	// Token: 0x0600428D RID: 17037 RVA: 0x00163D68 File Offset: 0x00161F68
	private static void Simulate(ref VerletLine.LineNode p, float dt)
	{
		Vector3 position = p.position;
		p.position += p.position - p.lastPosition + p.acceleration * (dt * dt);
		p.lastPosition = position;
	}

	// Token: 0x0600428E RID: 17038 RVA: 0x00163DC0 File Offset: 0x00161FC0
	private static void LimitDistance(ref VerletLine.LineNode p1, ref VerletLine.LineNode p2, float restLength)
	{
		Vector3 a = p2.position - p1.position;
		float num = a.magnitude + 1E-05f;
		float num2 = (num - restLength) / num;
		p1.position += a * (num2 * 0.5f);
		p2.position -= a * (num2 * 0.5f);
	}

	// Token: 0x04005475 RID: 21621
	public Transform lineStart;

	// Token: 0x04005476 RID: 21622
	public Transform lineEnd;

	// Token: 0x04005477 RID: 21623
	[Space]
	public LineRenderer line;

	// Token: 0x04005478 RID: 21624
	public Rigidbody endRigidbody;

	// Token: 0x04005479 RID: 21625
	public Transform endRigidbodyParent;

	// Token: 0x0400547A RID: 21626
	public Vector3 endLineAnchorLocalPosition;

	// Token: 0x0400547B RID: 21627
	private Vector3 rigidBodyStartingLocalPosition;

	// Token: 0x0400547C RID: 21628
	[Space]
	public int segmentNumber = 10;

	// Token: 0x0400547D RID: 21629
	public float segmentLength = 0.03f;

	// Token: 0x0400547E RID: 21630
	public float segmentTargetLength = 0.03f;

	// Token: 0x0400547F RID: 21631
	public float segmentMaxLength = 0.03f;

	// Token: 0x04005480 RID: 21632
	public float segmentMinLength = 0.03f;

	// Token: 0x04005481 RID: 21633
	[Space]
	public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	// Token: 0x04005482 RID: 21634
	public int simIterations = 6;

	// Token: 0x04005483 RID: 21635
	public float tension = 10f;

	// Token: 0x04005484 RID: 21636
	public float tensionScale = 1f;

	// Token: 0x04005485 RID: 21637
	public float endMaxSpeed = 48f;

	// Token: 0x04005486 RID: 21638
	[FormerlySerializedAs("lerpSpeed")]
	[Space]
	public float resizeSpeed = 1f;

	// Token: 0x04005487 RID: 21639
	public float resizeScale = 1f;

	// Token: 0x04005488 RID: 21640
	[NonSerialized]
	private VerletLine.LineNode[] _nodes = new VerletLine.LineNode[0];

	// Token: 0x04005489 RID: 21641
	[NonSerialized]
	private Vector3[] _positions = new Vector3[0];

	// Token: 0x0400548A RID: 21642
	private float totalLineLength;

	// Token: 0x0400548B RID: 21643
	[SerializeField]
	private bool onlyPullAtEdges;

	// Token: 0x0400548C RID: 21644
	[SerializeField]
	private bool scaleLineWidth = true;

	// Token: 0x02000A2A RID: 2602
	[Serializable]
	public struct LineNode
	{
		// Token: 0x0400548D RID: 21645
		public Vector3 position;

		// Token: 0x0400548E RID: 21646
		public Vector3 lastPosition;

		// Token: 0x0400548F RID: 21647
		public Vector3 acceleration;
	}
}
