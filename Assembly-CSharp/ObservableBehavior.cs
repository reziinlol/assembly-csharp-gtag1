using System;
using UnityEngine;

// Token: 0x02000D66 RID: 3430
public abstract class ObservableBehavior : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
	// Token: 0x170007F1 RID: 2033
	// (get) Token: 0x06005457 RID: 21591 RVA: 0x001B898C File Offset: 0x001B6B8C
	// (set) Token: 0x06005458 RID: 21592 RVA: 0x001B8994 File Offset: 0x001B6B94
	public ObservableBehaviorRule ObservableBehaviorRule
	{
		get
		{
			return this.observableBehaviorRule;
		}
		set
		{
			this.observableBehaviorRule = value;
			this.firstFrame = true;
		}
	}

	// Token: 0x170007F2 RID: 2034
	// (get) Token: 0x06005459 RID: 21593 RVA: 0x001B89A4 File Offset: 0x001B6BA4
	public float Distance
	{
		get
		{
			return this.dist;
		}
	}

	// Token: 0x0600545A RID: 21594 RVA: 0x001B89AC File Offset: 0x001B6BAC
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.UnityOnEnable();
	}

	// Token: 0x0600545B RID: 21595 RVA: 0x001B89BB File Offset: 0x001B6BBB
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
		this.UnityOnDisable();
	}

	// Token: 0x0600545C RID: 21596 RVA: 0x001B89E0 File Offset: 0x001B6BE0
	private void OnDestroy()
	{
		if (this.observable)
		{
			this.observable = false;
			this.OnLostObservable();
		}
	}

	// Token: 0x0600545D RID: 21597 RVA: 0x001B89F8 File Offset: 0x001B6BF8
	void IGorillaSliceableSimple.SliceUpdate()
	{
		bool flag = this.observableVolume != null && this.observableVolume.LocalRigPresent;
		if (this.observableVolume == null && this.observableBehaviorRule != null)
		{
			Transform transform = Camera.main.transform;
			this.dist = Vector3.Distance(transform.position, base.transform.position);
			float num;
			if (this.observableBehaviorRule.InverseObservable)
			{
				num = Vector3.Dot((base.transform.position - transform.position).normalized, base.transform.forward);
			}
			else
			{
				num = Vector3.Dot((transform.position - base.transform.position).normalized, transform.transform.forward);
			}
			flag = (this.observableBehaviorRule.ObservableDistanceRange.x <= this.dist && this.dist <= this.observableBehaviorRule.ObservableDistanceRange.y && this.observableBehaviorRule.ObservableDotRange.x <= num && num <= this.observableBehaviorRule.ObservableDotRange.y);
		}
		if (this.firstFrame || this.observable != flag)
		{
			if (flag)
			{
				this.OnBecameObservable();
			}
			else
			{
				this.OnLostObservable();
			}
		}
		this.observable = flag;
		this.firstFrame = false;
		if (flag)
		{
			this.ObservableSliceUpdate();
		}
	}

	// Token: 0x0600545E RID: 21598 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void UnityOnEnable()
	{
	}

	// Token: 0x0600545F RID: 21599 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void UnityOnDisable()
	{
	}

	// Token: 0x06005460 RID: 21600
	protected abstract void OnLostObservable();

	// Token: 0x06005461 RID: 21601
	protected abstract void OnBecameObservable();

	// Token: 0x06005462 RID: 21602
	protected abstract void ObservableSliceUpdate();

	// Token: 0x06005463 RID: 21603 RVA: 0x001B8B70 File Offset: 0x001B6D70
	public bool BuildValidationCheck()
	{
		if (this.observableVolume == null && this.observableBehaviorRule == null)
		{
			Debug.LogError("observableVolume & observableBehaviorRule can't both be null!");
			return false;
		}
		if (this.observableVolume != null && this.observableBehaviorRule != null)
		{
			Debug.LogWarning("observableVolume will override the observableBehaviorRule");
		}
		return true;
	}

	// Token: 0x06005464 RID: 21604 RVA: 0x001B8BCC File Offset: 0x001B6DCC
	public void OnDrawGizmosSelected()
	{
		if (this.observableBehaviorRule != null)
		{
			if (this.observableBehaviorRule.ObservableDistanceRange.x > 0f)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.transform.position, this.observableBehaviorRule.ObservableDistanceRange.x);
			}
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.observableBehaviorRule.ObservableDistanceRange.y);
		}
	}

	// Token: 0x0400651C RID: 25884
	private bool firstFrame = true;

	// Token: 0x0400651D RID: 25885
	protected bool observable = true;

	// Token: 0x0400651E RID: 25886
	[SerializeField]
	private ObservableBehaviorRule observableBehaviorRule;

	// Token: 0x0400651F RID: 25887
	[SerializeField]
	private RigEventVolume observableVolume;

	// Token: 0x04006520 RID: 25888
	private float dist;
}
