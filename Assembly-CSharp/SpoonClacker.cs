using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020009D5 RID: 2517
public class SpoonClacker : MonoBehaviour
{
	// Token: 0x06004072 RID: 16498 RVA: 0x0015847D File Offset: 0x0015667D
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06004073 RID: 16499 RVA: 0x00158488 File Offset: 0x00156688
	private void Setup()
	{
		JointLimits limits = this.hingeJoint.limits;
		this.hingeMin = limits.min;
		this.hingeMax = limits.max;
	}

	// Token: 0x06004074 RID: 16500 RVA: 0x001584BC File Offset: 0x001566BC
	private void Update()
	{
		if (!this.transferObject)
		{
			return;
		}
		TransferrableObject.PositionState currentState = this.transferObject.currentState;
		if (currentState != TransferrableObject.PositionState.InLeftHand && currentState != TransferrableObject.PositionState.InRightHand)
		{
			return;
		}
		float num = MathUtils.Linear(this.hingeJoint.angle, this.hingeMin, this.hingeMax, 0f, 1f);
		float value = (this.invertOut ? (1f - num) : num) * 100f;
		this.skinnedMesh.SetBlendShapeWeight(this.targetBlendShape, value);
		if (!this._lockMin && num <= this.minThreshold)
		{
			this.OnHitMin.Invoke();
			this._lockMin = true;
		}
		else if (!this._lockMax && num >= 1f - this.maxThreshold)
		{
			this.OnHitMax.Invoke();
			this._lockMax = true;
			if (this._sincelastHit.HasElapsed(this.multiHitCutoff, true))
			{
				this.soundsSingle.Play();
			}
			else
			{
				this.soundsMulti.Play();
			}
		}
		if (this._lockMin && num > this.minThreshold * this.hysterisisFactor)
		{
			this._lockMin = false;
		}
		if (this._lockMax && num < 1f - this.maxThreshold * this.hysterisisFactor)
		{
			this._lockMax = false;
		}
	}

	// Token: 0x040050FB RID: 20731
	public TransferrableObject transferObject;

	// Token: 0x040050FC RID: 20732
	public SkinnedMeshRenderer skinnedMesh;

	// Token: 0x040050FD RID: 20733
	public HingeJoint hingeJoint;

	// Token: 0x040050FE RID: 20734
	public int targetBlendShape;

	// Token: 0x040050FF RID: 20735
	public float hingeMin;

	// Token: 0x04005100 RID: 20736
	public float hingeMax;

	// Token: 0x04005101 RID: 20737
	public bool invertOut;

	// Token: 0x04005102 RID: 20738
	public float minThreshold = 0.01f;

	// Token: 0x04005103 RID: 20739
	public float maxThreshold = 0.01f;

	// Token: 0x04005104 RID: 20740
	public float hysterisisFactor = 4f;

	// Token: 0x04005105 RID: 20741
	public UnityEvent OnHitMin;

	// Token: 0x04005106 RID: 20742
	public UnityEvent OnHitMax;

	// Token: 0x04005107 RID: 20743
	private bool _lockMin;

	// Token: 0x04005108 RID: 20744
	private bool _lockMax;

	// Token: 0x04005109 RID: 20745
	public SoundBankPlayer soundsSingle;

	// Token: 0x0400510A RID: 20746
	public SoundBankPlayer soundsMulti;

	// Token: 0x0400510B RID: 20747
	private TimeSince _sincelastHit;

	// Token: 0x0400510C RID: 20748
	[FormerlySerializedAs("multiHitInterval")]
	public float multiHitCutoff = 0.1f;
}
