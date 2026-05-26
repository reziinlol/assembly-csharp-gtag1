using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000A19 RID: 2585
public class GorillaThrowableController : MonoBehaviour
{
	// Token: 0x06004229 RID: 16937 RVA: 0x001618D9 File Offset: 0x0015FAD9
	protected void Awake()
	{
		this.gorillaThrowableLayerMask = LayerMask.GetMask(new string[]
		{
			"GorillaThrowable"
		});
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x001618F4 File Offset: 0x0015FAF4
	private void LateUpdate()
	{
		if (this.testCanGrab)
		{
			this.testCanGrab = false;
			this.CanGrabAnObject(this.rightHandController, out this.returnCollider);
			Debug.Log(this.returnCollider.gameObject, this.returnCollider.gameObject);
		}
		if (this.leftHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.LeftHand))
			{
				if (this.leftHandGrabbedObject != null)
				{
					this.leftHandGrabbedObject.ThrowThisThingo();
					this.leftHandGrabbedObject = null;
				}
				this.leftHandIsGrabbing = false;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.LeftHand))
		{
			this.leftHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.leftHandController, out this.returnCollider))
			{
				this.leftHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.leftHandGrabbedObject.Grabbed(this.leftHandController);
			}
		}
		if (this.rightHandIsGrabbing)
		{
			if (this.CheckIfHandHasReleased(XRNode.RightHand))
			{
				if (this.rightHandGrabbedObject != null)
				{
					this.rightHandGrabbedObject.ThrowThisThingo();
					this.rightHandGrabbedObject = null;
				}
				this.rightHandIsGrabbing = false;
				return;
			}
		}
		else if (this.CheckIfHandHasGrabbed(XRNode.RightHand))
		{
			this.rightHandIsGrabbing = true;
			if (this.CanGrabAnObject(this.rightHandController, out this.returnCollider))
			{
				this.rightHandGrabbedObject = this.returnCollider.GetComponent<GorillaThrowable>();
				this.rightHandGrabbedObject.Grabbed(this.rightHandController);
			}
		}
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x00161A40 File Offset: 0x0015FC40
	private bool CheckIfHandHasReleased(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue < 0.75f)
		{
			this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
			if (this.triggerValue < 0.75f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600422C RID: 16940 RVA: 0x00161ABC File Offset: 0x0015FCBC
	private bool CheckIfHandHasGrabbed(XRNode node)
	{
		this.inputDevice = InputDevices.GetDeviceAtXRNode(node);
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		if (this.triggerValue > 0.75f)
		{
			return true;
		}
		this.triggerValue = ((node == XRNode.LeftHand) ? SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand) : SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand));
		return this.triggerValue > 0.75f;
	}

	// Token: 0x0600422D RID: 16941 RVA: 0x00161B38 File Offset: 0x0015FD38
	private bool CanGrabAnObject(Transform handTransform, out Collider returnCollider)
	{
		this.magnitude = 100f;
		returnCollider = null;
		Debug.Log("trying:");
		if (Physics.OverlapSphereNonAlloc(handTransform.position, this.handRadius, this.colliders, this.gorillaThrowableLayerMask) > 0)
		{
			Debug.Log("found something!");
			this.minCollider = this.colliders[0];
			foreach (Collider collider in this.colliders)
			{
				if (collider != null)
				{
					Debug.Log("found this", collider);
					if ((collider.transform.position - handTransform.position).magnitude < this.magnitude)
					{
						this.minCollider = collider;
						this.magnitude = (collider.transform.position - handTransform.position).magnitude;
					}
				}
			}
			returnCollider = this.minCollider;
			return true;
		}
		return false;
	}

	// Token: 0x0600422E RID: 16942 RVA: 0x00161C21 File Offset: 0x0015FE21
	public void GrabbableObjectHover(bool isLeft)
	{
		GorillaTagger.Instance.StartVibration(isLeft, this.hoverVibrationStrength, this.hoverVibrationDuration);
	}

	// Token: 0x040053F7 RID: 21495
	public Transform leftHandController;

	// Token: 0x040053F8 RID: 21496
	public Transform rightHandController;

	// Token: 0x040053F9 RID: 21497
	public bool leftHandIsGrabbing;

	// Token: 0x040053FA RID: 21498
	public bool rightHandIsGrabbing;

	// Token: 0x040053FB RID: 21499
	public GorillaThrowable leftHandGrabbedObject;

	// Token: 0x040053FC RID: 21500
	public GorillaThrowable rightHandGrabbedObject;

	// Token: 0x040053FD RID: 21501
	public float hoverVibrationStrength = 0.25f;

	// Token: 0x040053FE RID: 21502
	public float hoverVibrationDuration = 0.05f;

	// Token: 0x040053FF RID: 21503
	public float handRadius = 0.05f;

	// Token: 0x04005400 RID: 21504
	private InputDevice rightDevice;

	// Token: 0x04005401 RID: 21505
	private InputDevice leftDevice;

	// Token: 0x04005402 RID: 21506
	private InputDevice inputDevice;

	// Token: 0x04005403 RID: 21507
	private float triggerValue;

	// Token: 0x04005404 RID: 21508
	private bool boolVar;

	// Token: 0x04005405 RID: 21509
	private Collider[] colliders = new Collider[10];

	// Token: 0x04005406 RID: 21510
	private Collider minCollider;

	// Token: 0x04005407 RID: 21511
	private Collider returnCollider;

	// Token: 0x04005408 RID: 21512
	private float magnitude;

	// Token: 0x04005409 RID: 21513
	public bool testCanGrab;

	// Token: 0x0400540A RID: 21514
	private int gorillaThrowableLayerMask;
}
