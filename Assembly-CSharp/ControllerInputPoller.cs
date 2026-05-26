using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000664 RID: 1636
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x060028BF RID: 10431 RVA: 0x000DD2B6 File Offset: 0x000DB4B6
	public bool LeftHandValid
	{
		get
		{
			return this.leftControllerIsValid || this.handTrackingActive;
		}
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x060028C0 RID: 10432 RVA: 0x000DD2C8 File Offset: 0x000DB4C8
	public bool RightHandValid
	{
		get
		{
			return this.rightControllerIsValid || this.handTrackingActive;
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x060028C1 RID: 10433 RVA: 0x000DD2DA File Offset: 0x000DB4DA
	[DebugReadout]
	public bool leftIndexPressed
	{
		get
		{
			return this._leftIndexPressed;
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x060028C2 RID: 10434 RVA: 0x000DD2E2 File Offset: 0x000DB4E2
	[DebugReadout]
	public bool leftIndexReleased
	{
		get
		{
			return this._leftIndexReleased;
		}
	}

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x060028C3 RID: 10435 RVA: 0x000DD2EA File Offset: 0x000DB4EA
	[DebugReadout]
	public bool rightIndexPressed
	{
		get
		{
			return this._rightIndexPressed;
		}
	}

	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x060028C4 RID: 10436 RVA: 0x000DD2F2 File Offset: 0x000DB4F2
	[DebugReadout]
	public bool rightIndexReleased
	{
		get
		{
			return this._rightIndexReleased;
		}
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060028C5 RID: 10437 RVA: 0x000DD2FA File Offset: 0x000DB4FA
	[DebugReadout]
	public bool leftIndexPressedThisFrame
	{
		get
		{
			return this._leftIndexPressedThisFrame;
		}
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060028C6 RID: 10438 RVA: 0x000DD302 File Offset: 0x000DB502
	[DebugReadout]
	public bool leftIndexReleasedThisFrame
	{
		get
		{
			return this._leftIndexReleasedThisFrame;
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060028C7 RID: 10439 RVA: 0x000DD30A File Offset: 0x000DB50A
	[DebugReadout]
	public bool rightIndexPressedThisFrame
	{
		get
		{
			return this._rightIndexPressedThisFrame;
		}
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x060028C8 RID: 10440 RVA: 0x000DD312 File Offset: 0x000DB512
	[DebugReadout]
	public bool rightIndexReleasedThisFrame
	{
		get
		{
			return this._rightIndexReleasedThisFrame;
		}
	}

	// Token: 0x17000412 RID: 1042
	// (get) Token: 0x060028C9 RID: 10441 RVA: 0x000DD31A File Offset: 0x000DB51A
	[DebugReadout]
	public Vector3 leftVelocity
	{
		get
		{
			return this._leftVelocity;
		}
	}

	// Token: 0x17000413 RID: 1043
	// (get) Token: 0x060028CA RID: 10442 RVA: 0x000DD322 File Offset: 0x000DB522
	[DebugReadout]
	public Vector3 rightVelocity
	{
		get
		{
			return this._rightVelocity;
		}
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x060028CB RID: 10443 RVA: 0x000DD32A File Offset: 0x000DB52A
	[DebugReadout]
	public Vector3 leftAngularVelocity
	{
		get
		{
			return this._leftAngularVelocity;
		}
	}

	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x060028CC RID: 10444 RVA: 0x000DD332 File Offset: 0x000DB532
	[DebugReadout]
	public Vector3 rightAngularVelocity
	{
		get
		{
			return this._rightAngularVelocity;
		}
	}

	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x060028CD RID: 10445 RVA: 0x000DD33A File Offset: 0x000DB53A
	// (set) Token: 0x060028CE RID: 10446 RVA: 0x000DD342 File Offset: 0x000DB542
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x060028CF RID: 10447 RVA: 0x000DD34B File Offset: 0x000DB54B
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000DD380 File Offset: 0x000DB580
	public static void AddUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Add(callback);
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000DD3E8 File Offset: 0x000DB5E8
	public static void RemoveUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Remove(callback);
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000DD454 File Offset: 0x000DB654
	public void LateUpdate()
	{
		this.leftControllerIsValid = this.leftControllerDevice.isValid;
		if (!this.leftControllerIsValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			this.leftControllerIsValid = this.leftControllerDevice.isValid;
			if (this.leftControllerIsValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		this.rightControllerIsValid = this.rightControllerDevice.isValid;
		if (!this.rightControllerIsValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		InputDevice inputDevice = this.leftControllerDevice;
		InputDevice inputDevice2 = this.rightControllerDevice;
		InputDevice inputDevice3 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.leftControllerRotation);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.leftControllerPrimary2DAxis);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.leftControllerTriggerButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.rightControllerPrimary2DAxis);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.rightControllerTriggerButton);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerTriggerButton = SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerTriggerButton = SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.headRotation);
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
		}
		else if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
		this.handTrackingActive = false;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out this._leftVelocity);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out this._leftAngularVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out this._rightVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out this._rightAngularVelocity);
		this._UpdatePressFlags();
		if (this.didModifyOnUpdate)
		{
			List<Action> list = this.onUpdateNext;
			List<Action> list2 = this.onUpdate;
			this.onUpdate = list;
			this.onUpdateNext = list2;
			this.didModifyOnUpdate = false;
		}
		foreach (Action action in this.onUpdate)
		{
			action();
		}
	}

	// Token: 0x060028D3 RID: 10451 RVA: 0x000DDA4C File Offset: 0x000DBC4C
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, out bool grabMomentary, out bool grabReleaseMomentary, float grabThreshold, float grabReleaseThreshold)
	{
		bool flag = grabValue >= grabThreshold;
		bool flag2 = grabValue <= grabReleaseThreshold;
		grabMomentary = (flag && !grab);
		grabReleaseMomentary = (flag2 && !grabRelease);
		grab = flag;
		grabRelease = flag2;
	}

	// Token: 0x060028D4 RID: 10452 RVA: 0x000DDA90 File Offset: 0x000DBC90
	public void RecalculateGrabState()
	{
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
			return;
		}
		if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000DDBC3 File Offset: 0x000DBDC3
	public static bool GetIndexPressed(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.rightIndexPressed;
		}
		return ControllerInputPoller.instance.leftIndexPressed;
	}

	// Token: 0x060028D6 RID: 10454 RVA: 0x000DDBE8 File Offset: 0x000DBDE8
	public static bool GetIndexReleased(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.rightIndexReleased;
		}
		return ControllerInputPoller.instance.leftIndexReleased;
	}

	// Token: 0x060028D7 RID: 10455 RVA: 0x000DDC0D File Offset: 0x000DBE0D
	public static bool GetIndexPressedThisFrame(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.leftIndexPressedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexPressedThisFrame;
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000DDC32 File Offset: 0x000DBE32
	public static bool GetIndexReleasedThisFrame(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.leftIndexReleasedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexReleasedThisFrame;
	}

	// Token: 0x060028D9 RID: 10457 RVA: 0x000DDC57 File Offset: 0x000DBE57
	public static bool GetGrab(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x060028DA RID: 10458 RVA: 0x000DDC7C File Offset: 0x000DBE7C
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000DDCA1 File Offset: 0x000DBEA1
	public static bool GetGrabMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabMomentary;
	}

	// Token: 0x060028DC RID: 10460 RVA: 0x000DDCC6 File Offset: 0x000DBEC6
	public static bool GetGrabReleaseMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabReleaseMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabReleaseMomentary;
	}

	// Token: 0x060028DD RID: 10461 RVA: 0x000DDCEB File Offset: 0x000DBEEB
	public static Vector2 Primary2DAxis(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimary2DAxis;
		}
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000DDD0A File Offset: 0x000DBF0A
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000DDD2F File Offset: 0x000DBF2F
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x060028E0 RID: 10464 RVA: 0x000DDD54 File Offset: 0x000DBF54
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x060028E1 RID: 10465 RVA: 0x000DDD79 File Offset: 0x000DBF79
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x060028E2 RID: 10466 RVA: 0x000DDD9E File Offset: 0x000DBF9E
	public static float GripFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000DDDC7 File Offset: 0x000DBFC7
	public static float TriggerFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000DDDF0 File Offset: 0x000DBFF0
	public static float TriggerTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000DDE19 File Offset: 0x000DC019
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000DDE53 File Offset: 0x000DC053
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000DDE8D File Offset: 0x000DC08D
	public static Vector3 DeviceVelocity(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftVelocity;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000DDEB6 File Offset: 0x000DC0B6
	public static Vector3 DeviceAngularVelocity(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftAngularVelocity;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightAngularVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000DDEE0 File Offset: 0x000DC0E0
	public static bool PositionValid(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerDevice.isValid;
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000DDF30 File Offset: 0x000DC130
	public static bool HasPressFlags(XRNode node, EControllerInputPressFlags inputStateFlags)
	{
		EControllerInputPressFlags inputStateFlags2 = ControllerInputPoller.GetInputStateFlags(node);
		return inputStateFlags != EControllerInputPressFlags.None && (inputStateFlags2 & inputStateFlags) == inputStateFlags;
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x060028EB RID: 10475 RVA: 0x000DDF4F File Offset: 0x000DC14F
	// (set) Token: 0x060028EC RID: 10476 RVA: 0x000DDF57 File Offset: 0x000DC157
	public EControllerInputPressFlags leftPressFlags { get; private set; }

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x060028ED RID: 10477 RVA: 0x000DDF60 File Offset: 0x000DC160
	// (set) Token: 0x060028EE RID: 10478 RVA: 0x000DDF68 File Offset: 0x000DC168
	public EControllerInputPressFlags rightPressFlags { get; private set; }

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x060028EF RID: 10479 RVA: 0x000DDF71 File Offset: 0x000DC171
	// (set) Token: 0x060028F0 RID: 10480 RVA: 0x000DDF79 File Offset: 0x000DC179
	public EControllerInputPressFlags leftPressFlagsLastFrame { get; private set; }

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x060028F1 RID: 10481 RVA: 0x000DDF82 File Offset: 0x000DC182
	// (set) Token: 0x060028F2 RID: 10482 RVA: 0x000DDF8A File Offset: 0x000DC18A
	public EControllerInputPressFlags rightPressFlagsLastFrame { get; private set; }

	// Token: 0x060028F3 RID: 10483 RVA: 0x000DDF93 File Offset: 0x000DC193
	public static EControllerInputPressFlags GetInputStateFlags(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftPressFlags;
		}
		if (node != XRNode.RightHand)
		{
			return EControllerInputPressFlags.None;
		}
		return ControllerInputPoller.instance.rightPressFlags;
	}

	// Token: 0x060028F4 RID: 10484 RVA: 0x000DDFB8 File Offset: 0x000DC1B8
	public static void AddCallbackOnPressStart(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, flags, callback);
	}

	// Token: 0x060028F5 RID: 10485 RVA: 0x000DDFC6 File Offset: 0x000DC1C6
	public static void AddCallbackOnPressEnd(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, flags, callback);
	}

	// Token: 0x060028F6 RID: 10486 RVA: 0x000DDFD4 File Offset: 0x000DC1D4
	public static void AddCallbackOnPressUpdate(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, flags, callback);
	}

	// Token: 0x060028F7 RID: 10487 RVA: 0x000DDFE4 File Offset: 0x000DC1E4
	private static void _AddInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		if (callback == null || flags == EControllerInputPressFlags.None)
		{
			return;
		}
		if (ref_callbacksInfo.list.Capacity <= ref_callbacksInfo.list.Count)
		{
			ref_callbacksInfo.list.Capacity = ref_callbacksInfo.list.Count * 2;
		}
		ref_callbacksInfo.list.Add(new ControllerInputPoller._InputCallback(flags, callback));
	}

	// Token: 0x060028F8 RID: 10488 RVA: 0x000DE03A File Offset: 0x000DC23A
	public static void RemoveCallbackOnPressStart(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, callback);
	}

	// Token: 0x060028F9 RID: 10489 RVA: 0x000DE047 File Offset: 0x000DC247
	public static void RemoveCallbackOnPressEnd(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, callback);
	}

	// Token: 0x060028FA RID: 10490 RVA: 0x000DE054 File Offset: 0x000DC254
	public static void RemoveCallbackOnPressUpdate(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, callback);
	}

	// Token: 0x060028FB RID: 10491 RVA: 0x000DE064 File Offset: 0x000DC264
	private static void _RemoveInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, Action<EHandednessFlags> callback)
	{
		if (callback == null)
		{
			return;
		}
		ref_callbacksInfo.list.RemoveAll((ControllerInputPoller._InputCallback sub) => sub.callback == callback);
	}

	// Token: 0x060028FC RID: 10492 RVA: 0x000DE0A0 File Offset: 0x000DC2A0
	private void _UpdatePressFlags()
	{
		this.leftPressFlagsLastFrame = this.leftPressFlags;
		this.leftPressFlags = ((this.leftIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.leftGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.leftControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.leftControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None));
		this.rightPressFlagsLastFrame = this.rightPressFlags;
		this.rightPressFlags = ((this.rightIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.rightGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.rightControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.rightControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None));
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressStart, ControllerInputPoller._EPressCadence.Start, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressEnd, ControllerInputPoller._EPressCadence.End, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressUpdate, ControllerInputPoller._EPressCadence.Held, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
	}

	// Token: 0x060028FD RID: 10493 RVA: 0x000DE1A0 File Offset: 0x000DC3A0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _UpdatePressFlags_Callbacks(ref ControllerInputPoller._InputCallbacksCadenceInfo callbacksInfo, ControllerInputPoller._EPressCadence cadence, EControllerInputPressFlags lFlags_now, EControllerInputPressFlags lFlags_old, EControllerInputPressFlags rFlags_now, EControllerInputPressFlags rFlags_old)
	{
		for (int i = 0; i < callbacksInfo.list.Count; i++)
		{
			EControllerInputPressFlags flags = callbacksInfo.list[i].flags;
			Action<EHandednessFlags> callback = callbacksInfo.list[i].callback;
			EHandednessFlags ehandednessFlags = ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Left, cadence, flags, lFlags_now, lFlags_old) | ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Right, cadence, flags, rFlags_now, rFlags_old);
			if (ehandednessFlags != EHandednessFlags.None && callback != null)
			{
				try
				{
					callbacksInfo.list[i].callback(ehandednessFlags);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
	}

	// Token: 0x060028FE RID: 10494 RVA: 0x000DE238 File Offset: 0x000DC438
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static EHandednessFlags _IsHandContributingToPressCadence(EHandednessFlags hand, ControllerInputPoller._EPressCadence pressCadence, EControllerInputPressFlags cbFlags, EControllerInputPressFlags flags_now, EControllerInputPressFlags flags_old)
	{
		if ((pressCadence != ControllerInputPoller._EPressCadence.Held || (cbFlags & flags_now) != cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.Start || (cbFlags & flags_now) != cbFlags || (cbFlags & flags_old) == cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.End || (cbFlags & flags_now) == cbFlags || (cbFlags & flags_old) != cbFlags))
		{
			return EHandednessFlags.None;
		}
		return hand;
	}

	// Token: 0x0400353A RID: 13626
	public const int k_defaultExecutionOrder = -400;

	// Token: 0x0400353B RID: 13627
	[OnEnterPlay_SetNull]
	public static volatile ControllerInputPoller instance;

	// Token: 0x0400353C RID: 13628
	public float leftControllerIndexFloat;

	// Token: 0x0400353D RID: 13629
	public float leftControllerGripFloat;

	// Token: 0x0400353E RID: 13630
	public float rightControllerIndexFloat;

	// Token: 0x0400353F RID: 13631
	public float rightControllerGripFloat;

	// Token: 0x04003540 RID: 13632
	public float leftControllerIndexTouch;

	// Token: 0x04003541 RID: 13633
	public float rightControllerIndexTouch;

	// Token: 0x04003542 RID: 13634
	public float rightStickLRFloat;

	// Token: 0x04003543 RID: 13635
	public Vector3 leftControllerPosition;

	// Token: 0x04003544 RID: 13636
	public Vector3 rightControllerPosition;

	// Token: 0x04003545 RID: 13637
	public Vector3 headPosition;

	// Token: 0x04003546 RID: 13638
	public Quaternion leftControllerRotation;

	// Token: 0x04003547 RID: 13639
	public Quaternion rightControllerRotation;

	// Token: 0x04003548 RID: 13640
	public Quaternion headRotation;

	// Token: 0x04003549 RID: 13641
	public InputDevice leftControllerDevice;

	// Token: 0x0400354A RID: 13642
	public InputDevice rightControllerDevice;

	// Token: 0x0400354B RID: 13643
	public InputDevice headDevice;

	// Token: 0x0400354C RID: 13644
	public bool leftControllerIsValid;

	// Token: 0x0400354D RID: 13645
	public bool rightControllerIsValid;

	// Token: 0x0400354E RID: 13646
	public bool handTrackingActive;

	// Token: 0x0400354F RID: 13647
	public bool leftControllerPrimaryButton;

	// Token: 0x04003550 RID: 13648
	public bool leftControllerSecondaryButton;

	// Token: 0x04003551 RID: 13649
	public bool rightControllerPrimaryButton;

	// Token: 0x04003552 RID: 13650
	public bool rightControllerSecondaryButton;

	// Token: 0x04003553 RID: 13651
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x04003554 RID: 13652
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x04003555 RID: 13653
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x04003556 RID: 13654
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x04003557 RID: 13655
	public bool leftControllerTriggerButton;

	// Token: 0x04003558 RID: 13656
	public bool rightControllerTriggerButton;

	// Token: 0x04003559 RID: 13657
	public bool leftGrab;

	// Token: 0x0400355A RID: 13658
	public bool leftGrabRelease;

	// Token: 0x0400355B RID: 13659
	public bool rightGrab;

	// Token: 0x0400355C RID: 13660
	public bool rightGrabRelease;

	// Token: 0x0400355D RID: 13661
	public bool leftGrabMomentary;

	// Token: 0x0400355E RID: 13662
	public bool leftGrabReleaseMomentary;

	// Token: 0x0400355F RID: 13663
	public bool rightGrabMomentary;

	// Token: 0x04003560 RID: 13664
	public bool rightGrabReleaseMomentary;

	// Token: 0x04003561 RID: 13665
	private bool _leftIndexPressed;

	// Token: 0x04003562 RID: 13666
	private bool _leftIndexReleased;

	// Token: 0x04003563 RID: 13667
	private bool _rightIndexPressed;

	// Token: 0x04003564 RID: 13668
	private bool _rightIndexReleased;

	// Token: 0x04003565 RID: 13669
	private bool _leftIndexPressedThisFrame;

	// Token: 0x04003566 RID: 13670
	private bool _leftIndexReleasedThisFrame;

	// Token: 0x04003567 RID: 13671
	private bool _rightIndexPressedThisFrame;

	// Token: 0x04003568 RID: 13672
	private bool _rightIndexReleasedThisFrame;

	// Token: 0x04003569 RID: 13673
	private Vector3 _leftVelocity;

	// Token: 0x0400356A RID: 13674
	private Vector3 _rightVelocity;

	// Token: 0x0400356B RID: 13675
	private Vector3 _leftAngularVelocity;

	// Token: 0x0400356C RID: 13676
	private Vector3 _rightAngularVelocity;

	// Token: 0x0400356E RID: 13678
	public Vector2 leftControllerPrimary2DAxis;

	// Token: 0x0400356F RID: 13679
	public Vector2 rightControllerPrimary2DAxis;

	// Token: 0x04003570 RID: 13680
	public AnimationCurve handTriggerCurve;

	// Token: 0x04003571 RID: 13681
	public AnimationCurve handGripCurve;

	// Token: 0x04003572 RID: 13682
	private List<Action> onUpdate = new List<Action>();

	// Token: 0x04003573 RID: 13683
	private List<Action> onUpdateNext = new List<Action>();

	// Token: 0x04003574 RID: 13684
	private bool didModifyOnUpdate;

	// Token: 0x04003575 RID: 13685
	public Vector3 leftHandOffset = new Vector3(0.01f, -0.16f, 0f);

	// Token: 0x04003576 RID: 13686
	public Quaternion leftHandRotation = Quaternion.Euler(89f, 6f, 11f);

	// Token: 0x04003577 RID: 13687
	public Vector3 rightHandOffset = new Vector3(-0.01f, -0.16f, 0f);

	// Token: 0x04003578 RID: 13688
	public Quaternion rightHandRotation = Quaternion.Euler(89f, 6f, 11f);

	// Token: 0x0400357D RID: 13693
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressStart = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x0400357E RID: 13694
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressEnd = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x0400357F RID: 13695
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressUpdate = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x02000665 RID: 1637
	private enum _EPressCadence
	{
		// Token: 0x04003581 RID: 13697
		Start,
		// Token: 0x04003582 RID: 13698
		End,
		// Token: 0x04003583 RID: 13699
		Held
	}

	// Token: 0x02000666 RID: 1638
	private struct _InputCallback
	{
		// Token: 0x06002901 RID: 10497 RVA: 0x000DE31F File Offset: 0x000DC51F
		public _InputCallback(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
		{
			this.flags = flags;
			this.callback = callback;
		}

		// Token: 0x04003584 RID: 13700
		public readonly EControllerInputPressFlags flags;

		// Token: 0x04003585 RID: 13701
		public readonly Action<EHandednessFlags> callback;
	}

	// Token: 0x02000667 RID: 1639
	private struct _InputCallbacksCadenceInfo
	{
		// Token: 0x06002902 RID: 10498 RVA: 0x000DE32F File Offset: 0x000DC52F
		public _InputCallbacksCadenceInfo(int initialCapacity)
		{
			this.list = new List<ControllerInputPoller._InputCallback>(initialCapacity);
		}

		// Token: 0x04003586 RID: 13702
		public readonly List<ControllerInputPoller._InputCallback> list;
	}
}
