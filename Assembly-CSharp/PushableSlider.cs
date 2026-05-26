using System;
using UnityEngine;

// Token: 0x02000575 RID: 1397
public class PushableSlider : MonoBehaviour
{
	// Token: 0x06002382 RID: 9090 RVA: 0x000BF8D1 File Offset: 0x000BDAD1
	public void Awake()
	{
		this.Initialize();
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000BF8D9 File Offset: 0x000BDAD9
	private void Initialize()
	{
		if (this._initialized)
		{
			return;
		}
		this._initialized = true;
		this._localSpace = base.transform.worldToLocalMatrix;
		this._startingPos = base.transform.localPosition;
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000BF910 File Offset: 0x000BDB10
	private void OnTriggerStay(Collider other)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 b = this._localSpace.MultiplyPoint3x4(other.transform.position);
		Vector3 vector = base.transform.localPosition - this._startingPos - b;
		float num = Mathf.Abs(vector.x);
		if (num < this.farPushDist)
		{
			Vector3 currentVelocity = componentInParent.currentVelocity;
			if (Mathf.Sign(vector.x) != Mathf.Sign((this._localSpace.rotation * currentVelocity).x))
			{
				return;
			}
			vector.x = Mathf.Sign(vector.x) * (this.farPushDist - num);
			vector.y = 0f;
			vector.z = 0f;
			Vector3 vector2 = base.transform.localPosition - this._startingPos + vector;
			vector2.x = Mathf.Clamp(vector2.x, this.minXOffset, this.maxXOffset);
			base.transform.localPosition = this.GetXOffsetVector(vector2.x + this._startingPos.x);
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000BFA73 File Offset: 0x000BDC73
	private Vector3 GetXOffsetVector(float x)
	{
		return new Vector3(x, this._startingPos.y, this._startingPos.z);
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000BFA94 File Offset: 0x000BDC94
	public void SetProgress(float value)
	{
		this.Initialize();
		value = Mathf.Clamp(value, 0f, 1f);
		float num = Mathf.Lerp(this.minXOffset, this.maxXOffset, value);
		base.transform.localPosition = this.GetXOffsetVector(this._startingPos.x + num);
		this._previousLocalPosition = new Vector3(num, 0f, 0f);
		this._cachedProgress = value;
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000BFB08 File Offset: 0x000BDD08
	public float GetProgress()
	{
		this.Initialize();
		Vector3 vector = base.transform.localPosition - this._startingPos;
		if (vector == this._previousLocalPosition)
		{
			return this._cachedProgress;
		}
		this._previousLocalPosition = vector;
		this._cachedProgress = (vector.x - this.minXOffset) / (this.maxXOffset - this.minXOffset);
		return this._cachedProgress;
	}

	// Token: 0x04002EAB RID: 11947
	[SerializeField]
	private float farPushDist = 0.015f;

	// Token: 0x04002EAC RID: 11948
	[SerializeField]
	private float maxXOffset;

	// Token: 0x04002EAD RID: 11949
	[SerializeField]
	private float minXOffset;

	// Token: 0x04002EAE RID: 11950
	private Matrix4x4 _localSpace;

	// Token: 0x04002EAF RID: 11951
	private Vector3 _startingPos;

	// Token: 0x04002EB0 RID: 11952
	private Vector3 _previousLocalPosition;

	// Token: 0x04002EB1 RID: 11953
	private float _cachedProgress;

	// Token: 0x04002EB2 RID: 11954
	private bool _initialized;
}
