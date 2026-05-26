using System;
using UnityEngine;

// Token: 0x02000857 RID: 2135
public class GorillaEyeExpressions : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003741 RID: 14145 RVA: 0x0012F39D File Offset: 0x0012D59D
	private void Awake()
	{
		this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x0012F3AB File Offset: 0x0012D5AB
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x0012F3CA File Offset: 0x0012D5CA
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x0012F3F8 File Offset: 0x0012D5F8
	private void CheckEyeEffects()
	{
		if (this.loudness == null)
		{
			this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
		}
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.overrideDuration > 0f)
		{
			this.overrideDuration -= this.deltaTime;
			if (this.overrideDuration <= 0f)
			{
				this.overrideUV = this.BaseUV;
			}
		}
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x0012F494 File Offset: 0x0012D694
	private void UpdateEyeExpression()
	{
		this.targetFace.GetComponent<Renderer>().material.SetVector(this._BaseMap_ST, new Vector4(0.5f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	// Token: 0x04004765 RID: 18277
	public GameObject targetFace;

	// Token: 0x04004766 RID: 18278
	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	// Token: 0x04004767 RID: 18279
	[SerializeField]
	private float screamDuration = 0.5f;

	// Token: 0x04004768 RID: 18280
	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	// Token: 0x04004769 RID: 18281
	private Vector2 BaseUV = Vector3.zero;

	// Token: 0x0400476A RID: 18282
	private GorillaSpeakerLoudness loudness;

	// Token: 0x0400476B RID: 18283
	private float overrideDuration;

	// Token: 0x0400476C RID: 18284
	private Vector2 overrideUV;

	// Token: 0x0400476D RID: 18285
	private float timeLastUpdated;

	// Token: 0x0400476E RID: 18286
	private float deltaTime;

	// Token: 0x0400476F RID: 18287
	private ShaderHashId _BaseMap_ST = "_BaseMap_ST";
}
