using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000358 RID: 856
[DefaultExecutionOrder(2001)]
public class ObjectHierarchyFlattener : MonoBehaviour, IGorillaSimpleBackgroundWorker
{
	// Token: 0x060014E9 RID: 5353 RVA: 0x0006F550 File Offset: 0x0006D750
	private void ResetTransform()
	{
		if (!this.initialized || (this.originalParentGO != null && this.originalParentGO.activeInHierarchy))
		{
			return;
		}
		base.transform.SetParent(this.originalParentTransform);
		this.isAttachedToOverride = false;
		base.transform.localPosition = this.originalLocalPosition;
		base.transform.localRotation = this.originalLocalRotation;
		base.transform.localScale = this.originalScale;
		this.initialized = false;
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x0006F5D3 File Offset: 0x0006D7D3
	public void CrumbDisabled()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		}
		if (this != null)
		{
			base.Invoke("ResetTransform", 0f);
		}
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0006F604 File Offset: 0x0006D804
	public void InvokeLateUpdate()
	{
		if (this.maintainRelativeScale)
		{
			base.transform.localScale = Vector3.Scale(this.originalParentTransform.lossyScale, this.originalScale);
		}
		base.transform.rotation = this.originalParentTransform.rotation * this.originalLocalRotation;
		base.transform.position = this.originalParentTransform.position + base.transform.rotation * this.calcOffset * (this.originalParentTransform.lossyScale.x / this.originalParentScale) * this.originalParentScale;
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x0006F6B3 File Offset: 0x0006D8B3
	private void OnEnable()
	{
		this.abandonWork = false;
		GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x0006F6C2 File Offset: 0x0006D8C2
	private void OnDisable()
	{
		this.abandonWork = true;
		ObjectHierarchyFlattenerManager.UnregisterOHF(this);
		if (base.enabled)
		{
			base.Invoke("ResetTransformIfStillDisabled", 0f);
		}
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x0006F6EC File Offset: 0x0006D8EC
	private void OnDestroy()
	{
		base.CancelInvoke();
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x0006F6F4 File Offset: 0x0006D8F4
	private void ResetTransformIfStillDisabled()
	{
		if (!base.isActiveAndEnabled)
		{
			this.ResetTransform();
		}
	}

	// Token: 0x060014F0 RID: 5360 RVA: 0x0006F704 File Offset: 0x0006D904
	public void SimpleWork()
	{
		if (this.initialized || this.abandonWork)
		{
			return;
		}
		if (this.trackTransformOfParent)
		{
			ObjectHierarchyFlattenerManager.RegisterOHF(this);
		}
		if (!this.isAttachedToOverride)
		{
			this.originalParentTransform = base.transform.parent;
			this.originalParentGO = this.originalParentTransform.gameObject;
			this.originalLocalPosition = base.transform.localPosition;
			this.originalLocalRotation = base.transform.localRotation;
			this.originalParentScale = base.transform.parent.lossyScale.x;
			this.originalScale = base.transform.localScale;
			this.calcOffset = Vector3.Scale(this.originalLocalPosition, this.originalScale);
			FlattenerCrumb flattenerCrumb = this.originalParentGO.GetComponent<FlattenerCrumb>();
			if (flattenerCrumb == null)
			{
				flattenerCrumb = this.originalParentGO.AddComponent<FlattenerCrumb>();
			}
			flattenerCrumb.AddFlattenerReference(this);
		}
		base.transform.SetParent((this.overrideParentTransform != null) ? this.overrideParentTransform : null);
		this.isAttachedToOverride = true;
		this.initialized = true;
	}

	// Token: 0x040019C8 RID: 6600
	public const int k_monoDefaultExecutionOrder = 2001;

	// Token: 0x040019C9 RID: 6601
	[DebugReadout]
	private GameObject originalParentGO;

	// Token: 0x040019CA RID: 6602
	private Transform originalParentTransform;

	// Token: 0x040019CB RID: 6603
	private Vector3 originalLocalPosition;

	// Token: 0x040019CC RID: 6604
	private Vector3 calcOffset;

	// Token: 0x040019CD RID: 6605
	private Quaternion originalLocalRotation;

	// Token: 0x040019CE RID: 6606
	private Vector3 originalScale;

	// Token: 0x040019CF RID: 6607
	private float originalParentScale;

	// Token: 0x040019D0 RID: 6608
	public bool trackTransformOfParent;

	// Token: 0x040019D1 RID: 6609
	public bool maintainRelativeScale;

	// Token: 0x040019D2 RID: 6610
	private FlattenerCrumb crumb;

	// Token: 0x040019D3 RID: 6611
	public Transform overrideParentTransform;

	// Token: 0x040019D4 RID: 6612
	private bool isAttachedToOverride;

	// Token: 0x040019D5 RID: 6613
	private bool initialized;

	// Token: 0x040019D6 RID: 6614
	private bool abandonWork = true;
}
