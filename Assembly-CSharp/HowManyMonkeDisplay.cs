using System;
using TMPro;
using UnityEngine;

// Token: 0x020003DE RID: 990
public class HowManyMonkeDisplay : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001787 RID: 6023 RVA: 0x0008722C File Offset: 0x0008542C
	public void OnEnable()
	{
		this.currValue = (this.nextValue = HowManyMonke.ThisMany);
		this.text.text = this.currValue.ToString("N0");
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x0008726F File Offset: 0x0008546F
	public void OnDisable()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x00087299 File Offset: 0x00085499
	private void OnDestroy()
	{
		HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x000872BB File Offset: 0x000854BB
	private void HowManyMonke_OnCheck(int thisMany)
	{
		this.currValue = this.nextValue;
		this.nextValue = thisMany;
		this.checkTime = Time.time;
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x000872DC File Offset: 0x000854DC
	public void SliceUpdate()
	{
		float time = Mathf.Lerp((float)this.currValue, (float)this.nextValue, (Time.time - this.checkTime) / HowManyMonke.RecheckDelay);
		this.text.text = time.ToString("N0");
		this.particleSystem.emission.rateOverTime = this.particleSystemRateToCount.Evaluate(time);
		float sqrMagnitude = (VRRig.LocalRig.transform.position - base.transform.position).sqrMagnitude;
		if (this.observable && sqrMagnitude > this.observableDistance)
		{
			this.observable = false;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Remove(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
				return;
			}
		}
		else if (!this.observable && sqrMagnitude < this.observableDistance)
		{
			this.observable = true;
			HowManyMonke.OnCheck = (Action<int>)Delegate.Combine(HowManyMonke.OnCheck, new Action<int>(this.HowManyMonke_OnCheck));
			if (this.observableActive)
			{
				this.observableActive.SetActive(this.observable);
			}
		}
	}

	// Token: 0x040022C5 RID: 8901
	[SerializeField]
	private TMP_Text text;

	// Token: 0x040022C6 RID: 8902
	[SerializeField]
	private float observableDistance = 100f;

	// Token: 0x040022C7 RID: 8903
	[SerializeField]
	private GameObject observableActive;

	// Token: 0x040022C8 RID: 8904
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x040022C9 RID: 8905
	[SerializeField]
	private AnimationCurve particleSystemRateToCount;

	// Token: 0x040022CA RID: 8906
	private bool observable;

	// Token: 0x040022CB RID: 8907
	private int currValue;

	// Token: 0x040022CC RID: 8908
	private int nextValue;

	// Token: 0x040022CD RID: 8909
	private float checkTime;
}
