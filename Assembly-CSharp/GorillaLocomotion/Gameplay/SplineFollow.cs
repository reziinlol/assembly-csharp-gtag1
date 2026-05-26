using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001105 RID: 4357
	public sealed class SplineFollow : MonoBehaviour
	{
		// Token: 0x06006DCB RID: 28107 RVA: 0x0023E148 File Offset: 0x0023C348
		public void Start()
		{
			base.transform.rotation *= this._rotationFix;
			this._smoothRotationTrackingRateExp = Mathf.Exp(this._smoothRotationTrackingRate);
			this._progress = this._splineProgressOffset;
			this._progressPerFixedUpdate = Time.fixedDeltaTime / this._duration;
			this._secondsToCycles = (double)(1f / this._duration);
			this._nativeSpline = new NativeSpline(this._unitySpline.Spline, this._unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
			if (this._approximate)
			{
				this.CalculateApproximationNodes();
			}
		}

		// Token: 0x06006DCC RID: 28108 RVA: 0x0023E1F0 File Offset: 0x0023C3F0
		private void CalculateApproximationNodes()
		{
			for (int i = 0; i < this._approximationResolution; i++)
			{
				float3 v;
				float3 v2;
				float3 v3;
				this._nativeSpline.Evaluate((float)i / (float)this._approximationResolution, out v, out v2, out v3);
				SplineFollow.SplineNode item = new SplineFollow.SplineNode(v, v2, v3);
				this._approximationNodes.Add(item);
			}
			if (this._nativeSpline.Closed)
			{
				this._approximationNodes.Add(this._approximationNodes[0]);
			}
		}

		// Token: 0x06006DCD RID: 28109 RVA: 0x0023E274 File Offset: 0x0023C474
		private void FixedUpdate()
		{
			if (!this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x06006DCE RID: 28110 RVA: 0x0023E284 File Offset: 0x0023C484
		private void Update()
		{
			if (this._approximate)
			{
				this.FollowSpline();
			}
		}

		// Token: 0x06006DCF RID: 28111 RVA: 0x0023E294 File Offset: 0x0023C494
		private void FollowSpline()
		{
			if (PhotonNetwork.InRoom)
			{
				double num = PhotonNetwork.Time * this._secondsToCycles + (double)this._splineProgressOffset;
				this._progress = (float)(num % 1.0);
			}
			else
			{
				this._progress = (this._progress + this._progressPerFixedUpdate) % 1f;
			}
			SplineFollow.SplineNode splineNode = this.EvaluateSpline(this._progress);
			base.transform.position = splineNode.Position;
			Quaternion a = Quaternion.LookRotation(splineNode.Tangent) * this._rotationFix;
			base.transform.rotation = Quaternion.Slerp(a, base.transform.rotation, Mathf.Exp(-this._smoothRotationTrackingRateExp * Time.deltaTime));
		}

		// Token: 0x06006DD0 RID: 28112 RVA: 0x0023E350 File Offset: 0x0023C550
		private SplineFollow.SplineNode EvaluateSpline(float t)
		{
			t %= 1f;
			if (this._approximate)
			{
				float num = t * (float)this._approximationNodes.Count;
				int num2 = (int)num;
				float t2 = num - (float)num2;
				num2 %= this._approximationNodes.Count;
				SplineFollow.SplineNode a = this._approximationNodes[num2];
				SplineFollow.SplineNode b = this._approximationNodes[(num2 + 1) % this._approximationNodes.Count];
				return SplineFollow.SplineNode.Lerp(a, b, t2);
			}
			float3 v;
			float3 v2;
			float3 v3;
			this._nativeSpline.Evaluate(t, out v, out v2, out v3);
			return new SplineFollow.SplineNode(v, v2, v3);
		}

		// Token: 0x06006DD1 RID: 28113 RVA: 0x0023E3EC File Offset: 0x0023C5EC
		private void OnDestroy()
		{
			this._nativeSpline.Dispose();
		}

		// Token: 0x04007ECB RID: 32459
		[SerializeField]
		[Tooltip("If true, approximates the spline position. Only use when exact position does not matter.")]
		private bool _approximate;

		// Token: 0x04007ECC RID: 32460
		[SerializeField]
		private SplineContainer _unitySpline;

		// Token: 0x04007ECD RID: 32461
		[SerializeField]
		private float _duration;

		// Token: 0x04007ECE RID: 32462
		private double _secondsToCycles;

		// Token: 0x04007ECF RID: 32463
		[SerializeField]
		private float _smoothRotationTrackingRate = 0.5f;

		// Token: 0x04007ED0 RID: 32464
		private float _smoothRotationTrackingRateExp;

		// Token: 0x04007ED1 RID: 32465
		private float _progressPerFixedUpdate;

		// Token: 0x04007ED2 RID: 32466
		[SerializeField]
		private float _splineProgressOffset;

		// Token: 0x04007ED3 RID: 32467
		[SerializeField]
		private Quaternion _rotationFix = Quaternion.identity;

		// Token: 0x04007ED4 RID: 32468
		private NativeSpline _nativeSpline;

		// Token: 0x04007ED5 RID: 32469
		private float _progress;

		// Token: 0x04007ED6 RID: 32470
		[Header("Approximate Spline Parameters")]
		[SerializeField]
		[Range(4f, 200f)]
		private int _approximationResolution = 100;

		// Token: 0x04007ED7 RID: 32471
		private readonly List<SplineFollow.SplineNode> _approximationNodes = new List<SplineFollow.SplineNode>();

		// Token: 0x02001106 RID: 4358
		private struct SplineNode
		{
			// Token: 0x06006DD3 RID: 28115 RVA: 0x0023E42C File Offset: 0x0023C62C
			public SplineNode(Vector3 position, Vector3 tangent, Vector3 up)
			{
				this.Position = position;
				this.Tangent = tangent;
				this.Up = up;
			}

			// Token: 0x06006DD4 RID: 28116 RVA: 0x0023E454 File Offset: 0x0023C654
			public static SplineFollow.SplineNode Lerp(SplineFollow.SplineNode a, SplineFollow.SplineNode b, float t)
			{
				return new SplineFollow.SplineNode(Vector3.Lerp(a.Position, b.Position, t), Vector3.Lerp(a.Tangent, b.Tangent, t), Vector3.Lerp(a.Up, b.Up, t));
			}

			// Token: 0x04007ED8 RID: 32472
			public readonly Vector3 Position;

			// Token: 0x04007ED9 RID: 32473
			public readonly Vector3 Tangent;

			// Token: 0x04007EDA RID: 32474
			public readonly Vector3 Up;
		}
	}
}
