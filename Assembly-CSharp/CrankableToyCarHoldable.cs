using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public class CrankableToyCarHoldable : TransferrableObject
{
	// Token: 0x060011E4 RID: 4580 RVA: 0x0005FE1E File Offset: 0x0005E01E
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x0005FE40 File Offset: 0x0005E040
	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += this.OnDeployRPC;
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x0005FF2B File Offset: 0x0005E12B
	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0005FF4C File Offset: 0x0005E14C
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0005FFA0 File Offset: 0x0005E1A0
	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float value = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? pitchOverride = new float?(value);
				soundBankPlayer.Play(null, pitchOverride);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x000600B4 File Offset: 0x0005E2B4
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x000601E3 File Offset: 0x0005E3E3
	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x00060208 File Offset: 0x0005E408
	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 launchPos = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 releaseVel = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float lifetime = (float)args[3];
		float num = 10000f;
		if (launchPos.IsValid(num) && launchRot.IsValid())
		{
			float num2 = 10000f;
			if (releaseVel.IsValid(num2))
			{
				this.DeployCarLocal(launchPos, launchRot, releaseVel, lifetime, true);
				return;
			}
		}
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000602BD File Offset: 0x0005E4BD
	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000602E5 File Offset: 0x0005E4E5
	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	// Token: 0x0400159B RID: 5531
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	// Token: 0x0400159C RID: 5532
	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	// Token: 0x0400159D RID: 5533
	[SerializeField]
	private GameObject deployablePart;

	// Token: 0x0400159E RID: 5534
	[SerializeField]
	private GameObject disabledWhileDeployed;

	// Token: 0x0400159F RID: 5535
	[SerializeField]
	private float crankAnglePerClick;

	// Token: 0x040015A0 RID: 5536
	[SerializeField]
	private float maxCrankStrength;

	// Token: 0x040015A1 RID: 5537
	[SerializeField]
	private float minClickPitch;

	// Token: 0x040015A2 RID: 5538
	[SerializeField]
	private float maxClickPitch;

	// Token: 0x040015A3 RID: 5539
	[SerializeField]
	private float minLifetime;

	// Token: 0x040015A4 RID: 5540
	[SerializeField]
	private float maxLifetime;

	// Token: 0x040015A5 RID: 5541
	[SerializeField]
	private SoundBankPlayer clickSound;

	// Token: 0x040015A6 RID: 5542
	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	// Token: 0x040015A7 RID: 5543
	[SerializeField]
	private float crankHapticStrength = 0.1f;

	// Token: 0x040015A8 RID: 5544
	[SerializeField]
	private float crankHapticDuration = 0.05f;

	// Token: 0x040015A9 RID: 5545
	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	// Token: 0x040015AA RID: 5546
	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	// Token: 0x040015AB RID: 5547
	private float currentCrankStrength;

	// Token: 0x040015AC RID: 5548
	private float currentCrankClickAmount;

	// Token: 0x040015AD RID: 5549
	private RubberDuckEvents _events;
}
