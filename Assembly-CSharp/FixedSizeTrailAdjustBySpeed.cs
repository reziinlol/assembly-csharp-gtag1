using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000688 RID: 1672
public class FixedSizeTrailAdjustBySpeed : MonoBehaviour
{
	// Token: 0x060029AB RID: 10667 RVA: 0x000E0ECA File Offset: 0x000DF0CA
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000E0ED2 File Offset: 0x000DF0D2
	private void OnEnable()
	{
		this.ResetTrailState();
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000E0ED2 File Offset: 0x000DF0D2
	private void OnDisable()
	{
		this.ResetTrailState();
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000E0EDC File Offset: 0x000DF0DC
	private void ResetTrailState()
	{
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		this._lastSpeed = 0f;
		this._lastPosition = base.transform.position;
		if (!this.trail)
		{
			return;
		}
		this.trail.length = this.minLength;
		this.trail.Setup();
		this.LerpTrailColors(0f);
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000E0F5C File Offset: 0x000DF15C
	private void Setup()
	{
		this._lastPosition = base.transform.position;
		this._rawVelocity = Vector3.zero;
		this._rawSpeed = 0f;
		this._speed = 0f;
		if (this.trail)
		{
			this._initGravity = this.trail.gravity;
			this.trail.applyPhysics = this.adjustPhysics;
		}
		this.LerpTrailColors(0.5f);
	}

	// Token: 0x060029B0 RID: 10672 RVA: 0x000E0FD8 File Offset: 0x000DF1D8
	private void LerpTrailColors(float t = 0.5f)
	{
		GradientColorKey[] colorKeys = this._mixGradient.colorKeys;
		int num = colorKeys.Length;
		for (int i = 0; i < num; i++)
		{
			float time = (float)i / (float)(num - 1);
			Color a = this.minColors.Evaluate(time);
			Color b = this.maxColors.Evaluate(time);
			Color color = Color.Lerp(a, b, t);
			colorKeys[i].color = color;
			colorKeys[i].time = time;
		}
		this._mixGradient.colorKeys = colorKeys;
		if (this.trail)
		{
			this.trail.renderer.colorGradient = this._mixGradient;
		}
	}

	// Token: 0x060029B1 RID: 10673 RVA: 0x000E1078 File Offset: 0x000DF278
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		this._rawVelocity = (position - this._lastPosition) / deltaTime;
		this._rawSpeed = this._rawVelocity.magnitude;
		if (this._rawSpeed > this.retractMin)
		{
			this._speed += this.expandSpeed * deltaTime;
		}
		if (this._rawSpeed <= this.retractMin)
		{
			this._speed -= this.retractSpeed * deltaTime;
		}
		if (this._speed > this.maxSpeed)
		{
			this._speed = this.maxSpeed;
		}
		this._speed = Mathf.Lerp(this._lastSpeed, this._speed, 0.5f);
		if (this._speed < 0.01f)
		{
			this._speed = 0f;
		}
		this.AdjustTrail();
		this._lastSpeed = this._speed;
		this._lastPosition = position;
	}

	// Token: 0x060029B2 RID: 10674 RVA: 0x000E1170 File Offset: 0x000DF370
	private void AdjustTrail()
	{
		if (!this.trail)
		{
			return;
		}
		float num = MathUtils.Linear(this._speed, this.minSpeed, this.maxSpeed, 0f, 1f);
		float length = MathUtils.Linear(num, 0f, 1f, this.minLength, this.maxLength);
		this.trail.length = length;
		this.LerpTrailColors(num);
		if (this.adjustPhysics)
		{
			Transform transform = base.transform;
			Vector3 b = transform.forward * this.gravityOffset.z + transform.right * this.gravityOffset.x + transform.up * this.gravityOffset.y;
			Vector3 b2 = (this._initGravity + b) * (1f - num);
			this.trail.gravity = Vector3.Lerp(Vector3.zero, b2, 0.5f);
		}
	}

	// Token: 0x04003651 RID: 13905
	public FixedSizeTrail trail;

	// Token: 0x04003652 RID: 13906
	public bool adjustPhysics = true;

	// Token: 0x04003653 RID: 13907
	private Vector3 _rawVelocity;

	// Token: 0x04003654 RID: 13908
	private float _rawSpeed;

	// Token: 0x04003655 RID: 13909
	private float _speed;

	// Token: 0x04003656 RID: 13910
	private float _lastSpeed;

	// Token: 0x04003657 RID: 13911
	private Vector3 _lastPosition;

	// Token: 0x04003658 RID: 13912
	private Vector3 _initGravity;

	// Token: 0x04003659 RID: 13913
	public Vector3 gravityOffset = Vector3.zero;

	// Token: 0x0400365A RID: 13914
	[Space]
	public float retractMin = 0.5f;

	// Token: 0x0400365B RID: 13915
	[Space]
	[FormerlySerializedAs("sizeIncreaseSpeed")]
	public float expandSpeed = 16f;

	// Token: 0x0400365C RID: 13916
	[FormerlySerializedAs("sizeDecreaseSpeed")]
	public float retractSpeed = 4f;

	// Token: 0x0400365D RID: 13917
	[Space]
	public float minSpeed;

	// Token: 0x0400365E RID: 13918
	public float minLength = 1f;

	// Token: 0x0400365F RID: 13919
	public Gradient minColors = GradientHelper.FromColor(new Color(0f, 1f, 1f, 1f));

	// Token: 0x04003660 RID: 13920
	[Space]
	public float maxSpeed = 10f;

	// Token: 0x04003661 RID: 13921
	public float maxLength = 8f;

	// Token: 0x04003662 RID: 13922
	public Gradient maxColors = GradientHelper.FromColor(new Color(1f, 1f, 0f, 1f));

	// Token: 0x04003663 RID: 13923
	[Space]
	[SerializeField]
	private Gradient _mixGradient = new Gradient
	{
		colorKeys = new GradientColorKey[8],
		alphaKeys = Array.Empty<GradientAlphaKey>()
	};

	// Token: 0x02000689 RID: 1673
	[Serializable]
	public struct GradientKey
	{
		// Token: 0x060029B4 RID: 10676 RVA: 0x000E1345 File Offset: 0x000DF545
		public GradientKey(Color color, float time)
		{
			this.color = color;
			this.time = time;
		}

		// Token: 0x04003664 RID: 13924
		public Color color;

		// Token: 0x04003665 RID: 13925
		public float time;
	}
}
