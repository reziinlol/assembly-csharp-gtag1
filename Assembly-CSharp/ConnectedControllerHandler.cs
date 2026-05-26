using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000662 RID: 1634
internal class ConnectedControllerHandler : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x060028AB RID: 10411 RVA: 0x000DCD12 File Offset: 0x000DAF12
	// (set) Token: 0x060028AC RID: 10412 RVA: 0x000DCD19 File Offset: 0x000DAF19
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x060028AD RID: 10413 RVA: 0x000DCD21 File Offset: 0x000DAF21
	[SerializeField]
	private bool rightValid
	{
		get
		{
			return this.overrideRightEnable || (ControllerInputPoller.instance.RightHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.RightController));
		}
	}

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x060028AE RID: 10414 RVA: 0x000DCD56 File Offset: 0x000DAF56
	[SerializeField]
	private bool leftValid
	{
		get
		{
			return this.overrideLeftEnable || (ControllerInputPoller.instance.LeftHandValid && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController));
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x060028AF RID: 10415 RVA: 0x000DCD8B File Offset: 0x000DAF8B
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x060028B0 RID: 10416 RVA: 0x000DCD93 File Offset: 0x000DAF93
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000DCD9C File Offset: 0x000DAF9C
	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		this.UpdateControllerStates();
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000DCE50 File Offset: 0x000DB050
	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = GTPlayer.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000DCED7 File Offset: 0x000DB0D7
	public void SetRightHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.rightHandFollower.positionOffset = positionOffset;
		this.rightHandFollower.rotationOffset = rotationOffset;
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000DCEF1 File Offset: 0x000DB0F1
	public void SetLeftHandOffsets(Vector3 positionOffset, Quaternion rotationOffset)
	{
		this.leftHandFollower.positionOffset = positionOffset;
		this.leftHandFollower.rotationOffset = rotationOffset;
	}

	// Token: 0x060028B5 RID: 10421 RVA: 0x000DCF0B File Offset: 0x000DB10B
	public void SetOculusOffsets(bool rightHand = true, bool leftHand = true)
	{
		if (rightHand)
		{
			this.SetRightHandOffsets(this.oculusRightPosOffset, this.oculusRightRotOffset);
		}
		if (leftHand)
		{
			this.SetLeftHandOffsets(this.oculusLeftPosOffset, this.oculusLeftRotOffset);
		}
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000DCF37 File Offset: 0x000DB137
	private void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000DCF3F File Offset: 0x000DB13F
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x000DCF48 File Offset: 0x000DB148
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
	}

	// Token: 0x060028B9 RID: 10425 RVA: 0x000DCF6A File Offset: 0x000DB16A
	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	// Token: 0x060028BA RID: 10426 RVA: 0x000DCF94 File Offset: 0x000DB194
	public void SliceUpdate()
	{
		if (this.playerHandler.inOverlay)
		{
			return;
		}
		this.updateControllers = false;
		if (ControllerInputPoller.instance.RightHandValid)
		{
			this.tempRightPos = ControllerInputPoller.DevicePosition(XRNode.RightHand);
			if (this.tempRightPos == this.lastRightPos)
			{
				if (Time.time > this.timeStoppedMovingRight + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers |= OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingRight = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.RightController))
				{
					this.overriddenControllers &= ~OverrideControllers.RightController;
					this.updateControllers = true;
				}
			}
			this.lastRightPos = this.tempRightPos;
		}
		if (ControllerInputPoller.instance.LeftHandValid)
		{
			this.tempLeftPos = ControllerInputPoller.DevicePosition(XRNode.LeftHand);
			if (this.tempLeftPos == this.lastLeftPos)
			{
				if (Time.time > this.timeStoppedMovingLeft + this.stoppedDurationMinimum && !this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers |= OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			else
			{
				this.timeStoppedMovingLeft = Time.time;
				if (this.overriddenControllers.HasFlag(OverrideControllers.LeftController))
				{
					this.overriddenControllers &= ~OverrideControllers.LeftController;
					this.updateControllers = true;
				}
			}
			this.lastLeftPos = this.tempLeftPos;
		}
		if ((!this.leftXRController.enabled && this.leftValid) || (!this.rightXRController.enabled && this.rightValid))
		{
			this.updateControllers = true;
		}
		if (this.updateControllers)
		{
			this.overrideEnabled = (this.overriddenControllers > OverrideControllers.None);
			this.UpdateControllerStates();
		}
	}

	// Token: 0x060028BB RID: 10427 RVA: 0x000DD174 File Offset: 0x000DB374
	private void UpdateControllerStates()
	{
		this.leftXRController.enabled = this.leftValid;
		this.rightXRController.enabled = this.rightValid;
		this.AssignSnapturnController();
	}

	// Token: 0x060028BC RID: 10428 RVA: 0x000DD1A0 File Offset: 0x000DB3A0
	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	// Token: 0x060028BD RID: 10429 RVA: 0x000DD204 File Offset: 0x000DB404
	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool result;
		if (controllerNode != XRNode.LeftHand)
		{
			result = (controllerNode != XRNode.RightHand || this.rightValid);
		}
		else
		{
			result = this.leftValid;
		}
		return result;
	}

	// Token: 0x0400351F RID: 13599
	[SerializeField]
	private HandTransformFollowOffset rightHandFollower;

	// Token: 0x04003520 RID: 13600
	[SerializeField]
	private HandTransformFollowOffset leftHandFollower;

	// Token: 0x04003521 RID: 13601
	[SerializeField]
	private XRController rightXRController;

	// Token: 0x04003522 RID: 13602
	[SerializeField]
	private XRController leftXRController;

	// Token: 0x04003523 RID: 13603
	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	// Token: 0x04003524 RID: 13604
	private List<XRController> rightControllerList;

	// Token: 0x04003525 RID: 13605
	private List<XRController> leftcontrollerList;

	// Token: 0x04003526 RID: 13606
	[SerializeField]
	private bool overrideEnabled;

	// Token: 0x04003527 RID: 13607
	private bool overrideLeftEnable;

	// Token: 0x04003528 RID: 13608
	private bool overrideRightEnable;

	// Token: 0x04003529 RID: 13609
	[SerializeField]
	private Vector3 lastRightPos;

	// Token: 0x0400352A RID: 13610
	[SerializeField]
	private Vector3 lastLeftPos;

	// Token: 0x0400352B RID: 13611
	private Vector3 tempRightPos;

	// Token: 0x0400352C RID: 13612
	private Vector3 tempLeftPos;

	// Token: 0x0400352D RID: 13613
	private bool updateControllers;

	// Token: 0x0400352E RID: 13614
	private GTPlayer playerHandler;

	// Token: 0x0400352F RID: 13615
	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float stoppedDurationMinimum = 5f;

	// Token: 0x04003530 RID: 13616
	[SerializeField]
	private OverrideControllers overriddenControllers;

	// Token: 0x04003531 RID: 13617
	private float timeStoppedMovingLeft;

	// Token: 0x04003532 RID: 13618
	private float timeStoppedMovingRight;

	// Token: 0x04003533 RID: 13619
	public Vector3 oculusRightPosOffset = new Vector3(0f, -0.27f, 0.09f);

	// Token: 0x04003534 RID: 13620
	public Quaternion oculusRightRotOffset = Quaternion.Euler(275f, 270f, -5f);

	// Token: 0x04003535 RID: 13621
	public Vector3 oculusLeftPosOffset = new Vector3(--0f, -0.27f, 0.09f);

	// Token: 0x04003536 RID: 13622
	public Quaternion oculusLeftRotOffset = Quaternion.Euler(275f, 90f, 5f);
}
