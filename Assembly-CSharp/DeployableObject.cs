using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002B8 RID: 696
public class DeployableObject : TransferrableObject
{
	// Token: 0x060011F8 RID: 4600 RVA: 0x000606D7 File Offset: 0x0005E8D7
	protected override void Awake()
	{
		this._deploySignal.OnSignal += this.DeployRPC;
		base.Awake();
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000606F8 File Offset: 0x0005E8F8
	internal override void OnEnable()
	{
		this._deploySignal.Enable();
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		for (int i = 0; i < this._rigAwareObjects.Length; i++)
		{
			IRigAware rigAware = this._rigAwareObjects[i] as IRigAware;
			if (rigAware != null)
			{
				rigAware.SetRig(componentInParent);
			}
		}
		this.m_VRRig = componentInParent;
		ListProcessor<Action<RigContainer>> disableEvent = this.m_VRRig.rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Add(action);
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x00060795 File Offset: 0x0005E995
	internal override void OnDisable()
	{
		this.m_VRRig = null;
		this._deploySignal.Disable();
		if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
		base.OnDisable();
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x000607C4 File Offset: 0x0005E9C4
	private void OnRigPreDisable(RigContainer rc)
	{
		this.m_spamChecker.Reset();
		ListProcessor<Action<RigContainer>> disableEvent = rc.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Remove(action);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x000607FC File Offset: 0x0005E9FC
	protected override void OnDestroy()
	{
		this._deploySignal.Dispose();
		base.OnDestroy();
	}

	// Token: 0x060011FD RID: 4605 RVA: 0x00060810 File Offset: 0x0005EA10
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this._objectToDeploy.activeSelf)
			{
				this.DeployChild();
				return;
			}
		}
		else if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
	}

	// Token: 0x060011FE RID: 4606 RVA: 0x00060864 File Offset: 0x0005EA64
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRig.LocalRig != this.ownerRig)
		{
			return false;
		}
		bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(Vector3.zero);
		Quaternion rotation = transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		this.DeployLocal(vector, rotation, averageVelocity, false);
		this._deploySignal.Raise(ReceiverGroup.Others, BitPackUtils.PackWorldPosForNetwork(vector), BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f));
		return true;
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x00060907 File Offset: 0x0005EB07
	protected virtual void DeployLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this.DisableWhileDeployed(true);
		this._child.Deploy(this, launchPos, launchRot, releaseVel, isRemote);
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x00060924 File Offset: 0x0005EB24
	private void DeployRPC(long packedPos, int packedRot, long packedVel, PhotonSignalInfo info)
	{
		if (info.sender != base.OwningPlayer())
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "DeployRPC");
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork(packedRot);
		Vector3 inVel = BitPackUtils.UnpackWorldPosFromNetwork(packedVel) / 100f;
		float num = 10000f;
		if (!vector.IsValid(num) || !launchRot.IsValid() || !this.m_VRRig.IsPositionInRange(vector, this._maxDeployDistance))
		{
			return;
		}
		this.DeployLocal(vector, launchRot, this.m_VRRig.ClampVelocityRelativeToPlayerSafe(inVel, this._maxThrowVelocity, 100f), true);
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x000609D4 File Offset: 0x0005EBD4
	private void DisableWhileDeployed(bool active)
	{
		if (this._disabledWhileDeployed.IsNullOrEmpty<GameObject>())
		{
			return;
		}
		for (int i = 0; i < this._disabledWhileDeployed.Length; i++)
		{
			this._disabledWhileDeployed[i].SetActive(!active);
		}
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x00060A13 File Offset: 0x0005EC13
	public void DeployChild()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this._objectToDeploy.SetActive(true);
		this.DisableWhileDeployed(true);
		UnityEvent onDeploy = this._onDeploy;
		if (onDeploy == null)
		{
			return;
		}
		onDeploy.Invoke();
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00060A46 File Offset: 0x0005EC46
	public void ReturnChild()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this._objectToDeploy.SetActive(false);
		this.DisableWhileDeployed(false);
		UnityEvent onReturn = this._onReturn;
		if (onReturn == null)
		{
			return;
		}
		onReturn.Invoke();
	}

	// Token: 0x040015BF RID: 5567
	[SerializeField]
	private GameObject _objectToDeploy;

	// Token: 0x040015C0 RID: 5568
	[SerializeField]
	private DeployedChild _child;

	// Token: 0x040015C1 RID: 5569
	[SerializeField]
	private GameObject[] _disabledWhileDeployed = new GameObject[0];

	// Token: 0x040015C2 RID: 5570
	[SerializeField]
	private SoundBankPlayer deploySound;

	// Token: 0x040015C3 RID: 5571
	[SerializeField]
	private PhotonSignal<long, int, long> _deploySignal = "_deploySignal";

	// Token: 0x040015C4 RID: 5572
	[SerializeField]
	private float _maxDeployDistance = 4f;

	// Token: 0x040015C5 RID: 5573
	[SerializeField]
	private float _maxThrowVelocity = 50f;

	// Token: 0x040015C6 RID: 5574
	[SerializeField]
	private UnityEvent _onDeploy;

	// Token: 0x040015C7 RID: 5575
	[SerializeField]
	private UnityEvent _onReturn;

	// Token: 0x040015C8 RID: 5576
	[SerializeField]
	private Component[] _rigAwareObjects = new Component[0];

	// Token: 0x040015C9 RID: 5577
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(2, 1f, 0.5f);

	// Token: 0x040015CA RID: 5578
	private VRRig m_VRRig;
}
