using System;
using GorillaLocomotion.Gameplay;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

// Token: 0x020003EE RID: 1006
public class LckDirectGrabbable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000036 RID: 54
	// (add) Token: 0x060017EA RID: 6122 RVA: 0x00088C0C File Offset: 0x00086E0C
	// (remove) Token: 0x060017EB RID: 6123 RVA: 0x00088C44 File Offset: 0x00086E44
	public event Action onGrabbed;

	// Token: 0x14000037 RID: 55
	// (add) Token: 0x060017EC RID: 6124 RVA: 0x00088C7C File Offset: 0x00086E7C
	// (remove) Token: 0x060017ED RID: 6125 RVA: 0x00088CB4 File Offset: 0x00086EB4
	public event Action onReleased;

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x060017EE RID: 6126 RVA: 0x00088CE9 File Offset: 0x00086EE9
	public GorillaGrabber grabber
	{
		get
		{
			return this._grabber;
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060017EF RID: 6127 RVA: 0x00088CF1 File Offset: 0x00086EF1
	public bool isGrabbed
	{
		get
		{
			return this._grabber != null;
		}
	}

	// Token: 0x060017F0 RID: 6128 RVA: 0x00088CFF File Offset: 0x00086EFF
	public Vector3 GetLocalGrabbedPosition(GorillaGrabber grabber)
	{
		if (grabber == null)
		{
			return Vector3.zero;
		}
		return base.transform.InverseTransformPoint(grabber.transform.position);
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x00088D26 File Offset: 0x00086F26
	public bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return this._grabber == null || grabber == this._grabber;
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x00088D44 File Offset: 0x00086F44
	public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		if (!base.isActiveAndEnabled)
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		if (this._grabber != null && this._grabber != grabber)
		{
			this.ForceRelease();
		}
		bool flag;
		bool flag2;
		if (this._precise && this.IsSlingshotHeldInHand(out flag, out flag2) && ((grabber.XrNode == XRNode.LeftHand && flag) || (grabber.XrNode == XRNode.RightHand && flag2)))
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		this._grabber = grabber;
		GtColliderTriggerProcessor.CurrentGrabbedHand = grabber.XrNode;
		GtColliderTriggerProcessor.IsGrabbingTablet = true;
		grabbedTransform = base.transform;
		localGrabbedPosition = this.GetLocalGrabbedPosition(this._grabber);
		this.target.SetParent(grabber.transform, true);
		Action action = this.onGrabbed;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletGrabbed = this.OnTabletGrabbed;
		if (onTabletGrabbed == null)
		{
			return;
		}
		onTabletGrabbed.Invoke();
	}

	// Token: 0x060017F3 RID: 6131 RVA: 0x00088E44 File Offset: 0x00087044
	public void OnGrabReleased(GorillaGrabber grabber)
	{
		this.target.transform.SetParent(this._originalTargetParent, true);
		this._grabber = null;
		GtColliderTriggerProcessor.IsGrabbingTablet = false;
		Action action = this.onReleased;
		if (action != null)
		{
			action();
		}
		UnityEvent onTabletReleased = this.OnTabletReleased;
		if (onTabletReleased == null)
		{
			return;
		}
		onTabletReleased.Invoke();
	}

	// Token: 0x060017F4 RID: 6132 RVA: 0x00088E96 File Offset: 0x00087096
	public void ForceGrab(GorillaGrabber grabber)
	{
		grabber.Inject(base.transform, this.GetLocalGrabbedPosition(grabber));
	}

	// Token: 0x060017F5 RID: 6133 RVA: 0x00088EAB File Offset: 0x000870AB
	public void ForceRelease()
	{
		if (this._grabber == null)
		{
			return;
		}
		this._grabber.Inject(null, Vector3.zero);
	}

	// Token: 0x060017F6 RID: 6134 RVA: 0x00088ED0 File Offset: 0x000870D0
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		if (rig == null || rig.projectileWeapon == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = rig.projectileWeapon.InLeftHand();
		rightHand = rig.projectileWeapon.InRightHand();
		return rig.projectileWeapon.InHand();
	}

	// Token: 0x060017F7 RID: 6135 RVA: 0x00088F31 File Offset: 0x00087131
	public void SetOriginalTargetParent(Transform parent)
	{
		this._originalTargetParent = parent;
	}

	// Token: 0x060017F8 RID: 6136 RVA: 0x00023994 File Offset: 0x00021B94
	public bool MomentaryGrabOnly()
	{
		return true;
	}

	// Token: 0x060017FA RID: 6138 RVA: 0x00014807 File Offset: 0x00012A07
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x0400231E RID: 8990
	public UnityEvent OnTabletGrabbed = new UnityEvent();

	// Token: 0x0400231F RID: 8991
	public UnityEvent OnTabletReleased = new UnityEvent();

	// Token: 0x04002320 RID: 8992
	[SerializeField]
	private Transform _originalTargetParent;

	// Token: 0x04002321 RID: 8993
	public Transform target;

	// Token: 0x04002322 RID: 8994
	[SerializeField]
	private bool _precise;

	// Token: 0x04002323 RID: 8995
	private GorillaGrabber _grabber;
}
