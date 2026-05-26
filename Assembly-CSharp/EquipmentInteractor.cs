using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004A8 RID: 1192
public class EquipmentInteractor : MonoBehaviour
{
	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001CEF RID: 7407 RVA: 0x0009CE2B File Offset: 0x0009B02B
	public GorillaHandClimber BodyClimber
	{
		get
		{
			return this.bodyClimber;
		}
	}

	// Token: 0x17000310 RID: 784
	// (get) Token: 0x06001CF0 RID: 7408 RVA: 0x0009CE33 File Offset: 0x0009B033
	public GorillaHandClimber LeftClimber
	{
		get
		{
			return this.leftClimber;
		}
	}

	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001CF1 RID: 7409 RVA: 0x0009CE3B File Offset: 0x0009B03B
	public GorillaHandClimber RightClimber
	{
		get
		{
			return this.rightClimber;
		}
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x0009CE44 File Offset: 0x0009B044
	private void Awake()
	{
		if (EquipmentInteractor.instance == null)
		{
			EquipmentInteractor.instance = this;
			EquipmentInteractor.hasInstance = true;
		}
		else if (EquipmentInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.autoGrabLeft = true;
		this.autoGrabRight = true;
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x0009CE98 File Offset: 0x0009B098
	private void OnDestroy()
	{
		if (EquipmentInteractor.instance == this)
		{
			EquipmentInteractor.hasInstance = false;
			EquipmentInteractor.instance = null;
		}
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x0009CEB7 File Offset: 0x0009B0B7
	public void ReleaseRightHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.rightHand);
		}
		this.autoGrabRight = true;
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x0009CEF6 File Offset: 0x0009B0F6
	public void ReleaseLeftHand()
	{
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		if (this.leftHandHeldEquipment != null)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
		}
		this.autoGrabLeft = true;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0009CF35 File Offset: 0x0009B135
	public void ForceStopClimbing()
	{
		this.bodyClimber.ForceStopClimbing(false, false);
		this.leftClimber.ForceStopClimbing(false, false);
		this.rightClimber.ForceStopClimbing(false, false);
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x0009CF5E File Offset: 0x0009B15E
	public bool GetIsHolding(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.leftHandHeldEquipment != null;
		}
		return this.rightHandHeldEquipment != null;
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x0009CF77 File Offset: 0x0009B177
	public bool IsGrabDisabled(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return this.disableLeftGrab;
		}
		return this.disableRightGrab;
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x0009CF8C File Offset: 0x0009B18C
	public void InteractionPointDisabled(InteractionPoint interactionPoint)
	{
		if (this.iteratingInteractionPoints)
		{
			this.interactionPointsToRemove.Add(interactionPoint);
			return;
		}
		if (this.overlapInteractionPointsLeft != null)
		{
			this.overlapInteractionPointsLeft.Remove(interactionPoint);
		}
		if (this.overlapInteractionPointsRight != null)
		{
			this.overlapInteractionPointsRight.Remove(interactionPoint);
		}
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x0009CFD8 File Offset: 0x0009B1D8
	public bool CanGrabLeft()
	{
		return !this.disableLeftGrab && this.leftHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[0] == null;
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x0009D003 File Offset: 0x0009B203
	public bool CanGrabRight()
	{
		return !this.disableRightGrab && this.rightHandHeldEquipment == null && this.builderPieceInteractor.heldPiece[1] == null;
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x0009D030 File Offset: 0x0009B230
	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.leftClimber.CheckHandClimber();
		this.rightClimber.CheckHandClimber();
		this.CheckInputValue(true);
		this.isLeftGrabbing = ((this.wasLeftGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasLeftGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.leftClimber && this.leftClimber.isClimbingOrGrabbing)
		{
			this.isLeftGrabbing = false;
		}
		this.CheckInputValue(false);
		this.isRightGrabbing = ((this.wasRightGrabPressed && this.grabValue > this.grabThreshold - this.grabHysteresis) || (!this.wasRightGrabPressed && this.grabValue > this.grabThreshold + this.grabHysteresis));
		if (this.rightClimber && this.rightClimber.isClimbingOrGrabbing)
		{
			this.isRightGrabbing = false;
		}
		BuilderPiece pieceInHand = this.builderPieceInteractor.heldPiece[0];
		BuilderPiece pieceInHand2 = this.builderPieceInteractor.heldPiece[1];
		this.FireHandInteractions(this.leftHand, true, pieceInHand);
		this.FireHandInteractions(this.rightHand, false, pieceInHand2);
		if (!this.isRightGrabbing && this.wasRightGrabPressed)
		{
			this.ReleaseRightHand();
		}
		if (!this.isLeftGrabbing && this.wasLeftGrabPressed)
		{
			this.ReleaseLeftHand();
		}
		this.builderPieceInteractor.OnLateUpdate();
		if (GameBallPlayerLocal.instance != null)
		{
			GameBallPlayerLocal.instance.OnUpdateInteract();
		}
		if (GamePlayerLocal.instance != null)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
		}
		this.wasLeftGrabPressed = this.isLeftGrabbing;
		this.wasRightGrabPressed = this.isRightGrabbing;
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x0009D200 File Offset: 0x0009B400
	private void FireHandInteractions(GameObject interactingHand, bool isLeftHand, BuilderPiece pieceInHand)
	{
		if (isLeftHand)
		{
			this.justGrabbed = ((this.isLeftGrabbing && !this.wasLeftGrabPressed) || (this.isLeftGrabbing && this.autoGrabLeft));
			this.justReleased = (this.leftHandHeldEquipment != null && !this.isLeftGrabbing && this.wasLeftGrabPressed);
		}
		else
		{
			this.justGrabbed = ((this.isRightGrabbing && !this.wasRightGrabPressed) || (this.isRightGrabbing && this.autoGrabRight));
			this.justReleased = (this.rightHandHeldEquipment != null && !this.isRightGrabbing && this.wasRightGrabPressed);
		}
		List<InteractionPoint> list = isLeftHand ? this.overlapInteractionPointsLeft : this.overlapInteractionPointsRight;
		bool flag = isLeftHand ? (this.leftHandHeldEquipment != null) : (this.rightHandHeldEquipment != null);
		bool flag2 = pieceInHand != null;
		bool flag3 = isLeftHand ? this.disableLeftGrab : this.disableRightGrab;
		bool flag4 = !flag && !flag2 && !flag3;
		this.iteratingInteractionPoints = true;
		foreach (InteractionPoint interactionPoint in list)
		{
			if (flag4 && interactionPoint != null)
			{
				if (this.justGrabbed)
				{
					interactionPoint.Holdable.OnGrab(interactionPoint, interactingHand);
				}
				else
				{
					interactionPoint.Holdable.OnHover(interactionPoint, interactingHand);
				}
			}
			if (this.justReleased)
			{
				this.tempZone = interactionPoint.GetComponent<DropZone>();
				if (this.tempZone != null)
				{
					if (interactingHand == this.leftHand)
					{
						if (this.leftHandHeldEquipment != null)
						{
							this.leftHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
						}
					}
					else if (this.rightHandHeldEquipment != null)
					{
						this.rightHandHeldEquipment.OnRelease(this.tempZone, interactingHand);
					}
				}
			}
		}
		this.iteratingInteractionPoints = false;
		foreach (InteractionPoint item in this.interactionPointsToRemove)
		{
			if (this.overlapInteractionPointsLeft != null)
			{
				this.overlapInteractionPointsLeft.Remove(item);
			}
			if (this.overlapInteractionPointsRight != null)
			{
				this.overlapInteractionPointsRight.Remove(item);
			}
		}
		this.interactionPointsToRemove.Clear();
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x0009D458 File Offset: 0x0009B658
	public void UpdateHandEquipment(IHoldableObject newEquipment, bool forLeftHand)
	{
		if (forLeftHand)
		{
			if (newEquipment != null && newEquipment == this.rightHandHeldEquipment && !newEquipment.TwoHanded)
			{
				this.rightHandHeldEquipment = null;
			}
			if (this.leftHandHeldEquipment != null)
			{
				this.leftHandHeldEquipment.DropItemCleanup();
			}
			this.leftHandHeldEquipment = newEquipment;
			this.autoGrabLeft = false;
			return;
		}
		if (newEquipment != null && newEquipment == this.leftHandHeldEquipment && !newEquipment.TwoHanded)
		{
			this.leftHandHeldEquipment = null;
		}
		if (this.rightHandHeldEquipment != null)
		{
			this.rightHandHeldEquipment.DropItemCleanup();
		}
		this.rightHandHeldEquipment = newEquipment;
		this.autoGrabRight = false;
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x0009D4E4 File Offset: 0x0009B6E4
	public void CheckInputValue(bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		}
		else
		{
			this.grabValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
			this.tempValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		}
		this.grabValue = Mathf.Max(this.grabValue, this.tempValue);
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x0009D53D File Offset: 0x0009B73D
	public void ForceDropEquipment(IHoldableObject equipment)
	{
		if (this.rightHandHeldEquipment == equipment)
		{
			this.rightHandHeldEquipment = null;
		}
		if (this.leftHandHeldEquipment == equipment)
		{
			this.leftHandHeldEquipment = null;
		}
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x0009D55F File Offset: 0x0009B75F
	public void ForceDropAnyEquipment()
	{
		this.rightHandHeldEquipment = null;
		this.leftHandHeldEquipment = null;
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x0009D570 File Offset: 0x0009B770
	public void ForceDropManipulatableObject(HoldableObject manipulatableObject)
	{
		if ((HoldableObject)this.rightHandHeldEquipment == manipulatableObject)
		{
			this.rightHandHeldEquipment.OnRelease(null, this.rightHand);
			this.rightHandHeldEquipment = null;
			this.autoGrabRight = false;
		}
		if ((HoldableObject)this.leftHandHeldEquipment == manipulatableObject)
		{
			this.leftHandHeldEquipment.OnRelease(null, this.leftHand);
			this.leftHandHeldEquipment = null;
			this.autoGrabLeft = false;
		}
	}

	// Token: 0x0400271B RID: 10011
	[OnEnterPlay_SetNull]
	public static volatile EquipmentInteractor instance;

	// Token: 0x0400271C RID: 10012
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x0400271D RID: 10013
	public IHoldableObject leftHandHeldEquipment;

	// Token: 0x0400271E RID: 10014
	public IHoldableObject rightHandHeldEquipment;

	// Token: 0x0400271F RID: 10015
	public BuilderPieceInteractor builderPieceInteractor;

	// Token: 0x04002720 RID: 10016
	public GameObject rightHand;

	// Token: 0x04002721 RID: 10017
	public GameObject leftHand;

	// Token: 0x04002722 RID: 10018
	public InputDevice leftHandDevice;

	// Token: 0x04002723 RID: 10019
	public InputDevice rightHandDevice;

	// Token: 0x04002724 RID: 10020
	public List<InteractionPoint> overlapInteractionPointsLeft = new List<InteractionPoint>();

	// Token: 0x04002725 RID: 10021
	public List<InteractionPoint> overlapInteractionPointsRight = new List<InteractionPoint>();

	// Token: 0x04002726 RID: 10022
	public float grabRadius;

	// Token: 0x04002727 RID: 10023
	public float grabThreshold = 0.7f;

	// Token: 0x04002728 RID: 10024
	public float grabHysteresis = 0.05f;

	// Token: 0x04002729 RID: 10025
	public bool wasLeftGrabPressed;

	// Token: 0x0400272A RID: 10026
	public bool wasRightGrabPressed;

	// Token: 0x0400272B RID: 10027
	public bool isLeftGrabbing;

	// Token: 0x0400272C RID: 10028
	public bool isRightGrabbing;

	// Token: 0x0400272D RID: 10029
	public bool justReleased;

	// Token: 0x0400272E RID: 10030
	public bool justGrabbed;

	// Token: 0x0400272F RID: 10031
	public bool disableLeftGrab;

	// Token: 0x04002730 RID: 10032
	public bool disableRightGrab;

	// Token: 0x04002731 RID: 10033
	public bool autoGrabLeft;

	// Token: 0x04002732 RID: 10034
	public bool autoGrabRight;

	// Token: 0x04002733 RID: 10035
	private float grabValue;

	// Token: 0x04002734 RID: 10036
	private float tempValue;

	// Token: 0x04002735 RID: 10037
	private DropZone tempZone;

	// Token: 0x04002736 RID: 10038
	private bool iteratingInteractionPoints;

	// Token: 0x04002737 RID: 10039
	private List<InteractionPoint> interactionPointsToRemove = new List<InteractionPoint>();

	// Token: 0x04002738 RID: 10040
	[SerializeField]
	private GorillaHandClimber bodyClimber;

	// Token: 0x04002739 RID: 10041
	[SerializeField]
	private GorillaHandClimber leftClimber;

	// Token: 0x0400273A RID: 10042
	[SerializeField]
	private GorillaHandClimber rightClimber;
}
