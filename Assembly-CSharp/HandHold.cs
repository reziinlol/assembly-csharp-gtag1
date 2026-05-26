using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020008DB RID: 2267
[RequireComponent(typeof(Collider))]
public class HandHold : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003B51 RID: 15185 RVA: 0x001454F4 File Offset: 0x001436F4
	// (remove) Token: 0x06003B52 RID: 15186 RVA: 0x00145528 File Offset: 0x00143728
	public static event HandHold.HandHoldPositionEvent HandPositionRequestOverride;

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x06003B53 RID: 15187 RVA: 0x0014555C File Offset: 0x0014375C
	// (remove) Token: 0x06003B54 RID: 15188 RVA: 0x00145590 File Offset: 0x00143790
	public static event HandHold.HandHoldEvent HandPositionReleaseOverride;

	// Token: 0x06003B55 RID: 15189 RVA: 0x001455C4 File Offset: 0x001437C4
	public void OnDisable()
	{
		for (int i = 0; i < this.currentGrabbers.Count; i++)
		{
			if (this.currentGrabbers[i].IsNotNull())
			{
				this.currentGrabbers[i].Ungrab(this);
			}
		}
	}

	// Token: 0x06003B56 RID: 15190 RVA: 0x0014560C File Offset: 0x0014380C
	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.myTappable = base.GetComponent<Tappable>();
		this.myCollider = base.GetComponent<Collider>();
		this.initialized = true;
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x06003B58 RID: 15192 RVA: 0x00145638 File Offset: 0x00143838
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		this.Initialize();
		grabbedTransform = base.transform;
		Vector3 position = g.transform.position;
		localGrabbedPosition = base.transform.InverseTransformPoint(position);
		Vector3 arg;
		g.Player.AddHandHold(base.transform, localGrabbedPosition, g, g.IsLeftHand, this.rotatePlayerWhenHeld, out arg);
		this.currentGrabbers.AddIfNew(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionRequestOverride != null)
		{
			HandHold.HandPositionRequestOverride(this, g.IsLeftHand, this.CalculateOffset(position));
		}
		UnityEvent<Vector3> onGrab = this.OnGrab;
		if (onGrab != null)
		{
			onGrab.Invoke(arg);
		}
		UnityEvent<HandHold> onGrabHandHold = this.OnGrabHandHold;
		if (onGrabHandHold != null)
		{
			onGrabHandHold.Invoke(this);
		}
		UnityEvent<bool> onGrabHanded = this.OnGrabHanded;
		if (onGrabHanded != null)
		{
			onGrabHanded.Invoke(g.IsLeftHand);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnGrab();
		}
	}

	// Token: 0x06003B59 RID: 15193 RVA: 0x00145720 File Offset: 0x00143920
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		this.Initialize();
		g.Player.RemoveHandHold(g, g.IsLeftHand);
		this.currentGrabbers.Remove(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionReleaseOverride != null)
		{
			HandHold.HandPositionReleaseOverride(this, g.IsLeftHand);
		}
		UnityEvent onRelease = this.OnRelease;
		if (onRelease != null)
		{
			onRelease.Invoke();
		}
		UnityEvent<HandHold> onReleaseHandHold = this.OnReleaseHandHold;
		if (onReleaseHandHold != null)
		{
			onReleaseHandHold.Invoke(this);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnRelease();
		}
	}

	// Token: 0x06003B5A RID: 15194 RVA: 0x001457B0 File Offset: 0x001439B0
	private Vector3 CalculateOffset(Vector3 position)
	{
		switch (this.handSnapMethod)
		{
		case HandHold.HandSnapMethod.SnapToNearestEdge:
			if (this.myCollider == null)
			{
				this.myCollider = base.GetComponent<Collider>();
				if (this.myCollider is MeshCollider && !(this.myCollider as MeshCollider).convex)
				{
					this.handSnapMethod = HandHold.HandSnapMethod.None;
					return Vector3.zero;
				}
			}
			return base.transform.position - this.myCollider.ClosestPoint(position);
		case HandHold.HandSnapMethod.SnapToXAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.right * base.transform.InverseTransformPoint(position).x);
		case HandHold.HandSnapMethod.SnapToYAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.up * base.transform.InverseTransformPoint(position).y);
		case HandHold.HandSnapMethod.SnapToZAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.forward * base.transform.InverseTransformPoint(position).z);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x06003B5B RID: 15195 RVA: 0x001458EE File Offset: 0x00143AEE
	public bool MomentaryGrabOnly()
	{
		return this.forceMomentary;
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x001458F6 File Offset: 0x00143AF6
	public void CopyProperties(HandHoldSettings handHoldSettings)
	{
		this.handSnapMethod = (HandHold.HandSnapMethod)handHoldSettings.handSnapMethod;
		this.rotatePlayerWhenHeld = handHoldSettings.rotatePlayerWhenHeld;
		this.forceMomentary = !handHoldSettings.allowPreGrab;
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x00014807 File Offset: 0x00012A07
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004BD7 RID: 19415
	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	// Token: 0x04004BD8 RID: 19416
	[SerializeField]
	private HandHold.HandSnapMethod handSnapMethod;

	// Token: 0x04004BD9 RID: 19417
	[SerializeField]
	private bool rotatePlayerWhenHeld;

	// Token: 0x04004BDA RID: 19418
	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	// Token: 0x04004BDB RID: 19419
	[SerializeField]
	private UnityEvent<HandHold> OnGrabHandHold;

	// Token: 0x04004BDC RID: 19420
	[SerializeField]
	private UnityEvent<bool> OnGrabHanded;

	// Token: 0x04004BDD RID: 19421
	[SerializeField]
	private UnityEvent OnRelease;

	// Token: 0x04004BDE RID: 19422
	[SerializeField]
	private UnityEvent<HandHold> OnReleaseHandHold;

	// Token: 0x04004BDF RID: 19423
	private bool initialized;

	// Token: 0x04004BE0 RID: 19424
	private Collider myCollider;

	// Token: 0x04004BE1 RID: 19425
	private Tappable myTappable;

	// Token: 0x04004BE2 RID: 19426
	[Tooltip("Turning this on disables \"pregrabbing\". Use pregrabbing to allow players to catch a handhold even if they have squeezed the trigger too soon. Useful if you're anticipating jumping players needed to grab while airborne")]
	[SerializeField]
	private bool forceMomentary = true;

	// Token: 0x04004BE3 RID: 19427
	private List<GorillaGrabber> currentGrabbers = new List<GorillaGrabber>();

	// Token: 0x020008DC RID: 2268
	private enum HandSnapMethod
	{
		// Token: 0x04004BE5 RID: 19429
		None,
		// Token: 0x04004BE6 RID: 19430
		SnapToCenterPoint,
		// Token: 0x04004BE7 RID: 19431
		SnapToNearestEdge,
		// Token: 0x04004BE8 RID: 19432
		SnapToXAxisPoint,
		// Token: 0x04004BE9 RID: 19433
		SnapToYAxisPoint,
		// Token: 0x04004BEA RID: 19434
		SnapToZAxisPoint
	}

	// Token: 0x020008DD RID: 2269
	// (Invoke) Token: 0x06003B60 RID: 15200
	public delegate void HandHoldPositionEvent(HandHold hh, bool lh, Vector3 pos);

	// Token: 0x020008DE RID: 2270
	// (Invoke) Token: 0x06003B64 RID: 15204
	public delegate void HandHoldEvent(HandHold hh, bool lh);
}
