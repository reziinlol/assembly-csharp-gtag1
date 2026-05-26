using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class SIGadgetStilt : SIGadget
{
	// Token: 0x17000069 RID: 105
	// (get) Token: 0x06000688 RID: 1672 RVA: 0x000245AE File Offset: 0x000227AE
	// (set) Token: 0x06000689 RID: 1673 RVA: 0x000245B6 File Offset: 0x000227B6
	public bool TriggerToExtend { get; private set; }

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x0600068A RID: 1674 RVA: 0x000245BF File Offset: 0x000227BF
	// (set) Token: 0x0600068B RID: 1675 RVA: 0x000245C7 File Offset: 0x000227C7
	public bool hasMotor { get; private set; }

	// Token: 0x1700006B RID: 107
	// (get) Token: 0x0600068C RID: 1676 RVA: 0x000245D0 File Offset: 0x000227D0
	// (set) Token: 0x0600068D RID: 1677 RVA: 0x000245D8 File Offset: 0x000227D8
	public bool StickToAdjustLength { get; private set; }

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x0600068E RID: 1678 RVA: 0x000245E1 File Offset: 0x000227E1
	// (set) Token: 0x0600068F RID: 1679 RVA: 0x000245E9 File Offset: 0x000227E9
	public bool CanTag { get; private set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x06000690 RID: 1680 RVA: 0x000245F2 File Offset: 0x000227F2
	// (set) Token: 0x06000691 RID: 1681 RVA: 0x000245FA File Offset: 0x000227FA
	public bool CanStun { get; private set; }

	// Token: 0x06000692 RID: 1682 RVA: 0x00024604 File Offset: 0x00022804
	private void Awake()
	{
		this.tipDefaultOffset = this.tip.transform.localPosition;
		this.hasMotor = (this.motorTransform != null);
		this.hasEndB = (this.stiltEndB != null);
		this.hasEndC = (this.stiltEndC != null);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00024710 File Offset: 0x00022910
	private void DisableCurrentStilt()
	{
		if (this.currentStiltID != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltID);
			this.currentStiltID = StiltID.None;
		}
		if (this.currentStiltIDB != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltIDB);
			this.currentStiltIDB = StiltID.None;
		}
		if (this.currentStiltIDC != StiltID.None)
		{
			GTPlayer.Instance.DisableStilt(this.currentStiltIDC);
			this.currentStiltIDC = StiltID.None;
		}
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00024780 File Offset: 0x00022980
	private void OnGrabbed()
	{
		this.DisableCurrentStilt();
		this.HandleStartInteraction();
		if (this.IsEquippedLocal())
		{
			this.activatedLocally = true;
			if (this.gameEntity.heldByHandIndex == 0)
			{
				this.currentStiltID = StiltID.Held_Left;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, true, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Held_Left2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, true, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Held_Left3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, true, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
			}
			else
			{
				this.currentStiltID = StiltID.Held_Right;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, false, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Held_Right2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, false, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Held_Right3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, false, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
			}
		}
		else
		{
			this.activatedLocally = false;
		}
		this.wasSnappedByLocalJoint = SnapJointType.None;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x00024958 File Offset: 0x00022B58
	private void OnReleased()
	{
		this.DisableCurrentStilt();
		this.HandleStopInteraction();
		if (this.gameEntity.WasLastHeldByLocalPlayer() && this.TriggerToExtend && !Mathf.Approximately(this.targetLength, this.retractedLength))
		{
			this.targetLength = this.retractedLength;
			this.gameEntity.RequestState(this.gameEntity.id, this.PackStateForNetwork());
		}
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x000249C4 File Offset: 0x00022BC4
	private void OnSnapped()
	{
		this.DisableCurrentStilt();
		this.HandleStartInteraction();
		if (this.IsEquippedLocal())
		{
			this.wasSnappedByLocalJoint = this.gameEntity.snappedJoint;
			if (this.wasSnappedByLocalJoint == SnapJointType.HandL)
			{
				this.currentStiltID = StiltID.Snapped_Left;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, true, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Snapped_Left2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, true, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Snapped_Left3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, true, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
					return;
				}
			}
			else if (this.wasSnappedByLocalJoint == SnapJointType.HandR)
			{
				this.currentStiltID = StiltID.Snapped_Right;
				GTPlayer.Instance.EnableStilt(this.currentStiltID, false, this.stiltEnd.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				if (this.hasEndB)
				{
					this.currentStiltIDB = StiltID.Snapped_Right2;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDB, false, this.stiltEndB.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
				}
				if (this.hasEndC)
				{
					this.currentStiltIDC = StiltID.Snapped_Right3;
					GTPlayer.Instance.EnableStilt(this.currentStiltIDC, false, this.stiltEndC.position, this.maxArmLength, this.CanTag, this.CanStun, 0f, null);
					return;
				}
			}
		}
		else
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x00024BA1 File Offset: 0x00022DA1
	private void OnUnsnapped()
	{
		this.DisableCurrentStilt();
		this.HandleStopInteraction();
		if (this.wasSnappedByLocalJoint == SnapJointType.HandL)
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
			return;
		}
		if (this.wasSnappedByLocalJoint == SnapJointType.HandR)
		{
			this.wasSnappedByLocalJoint = SnapJointType.None;
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00024BD0 File Offset: 0x00022DD0
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.DisableCurrentStilt();
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x00024C20 File Offset: 0x00022E20
	protected override void OnUpdateAuthority(float dt)
	{
		if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this.DisableCurrentStilt();
			return;
		}
		bool isSpinning = this.IsSpinning;
		bool flag = false;
		if (this.currentStiltID != StiltID.None)
		{
			bool flag2 = !this.TriggerToExtend || this.CheckInput();
			this.IsSpinning = (this.hasMotor && this.CheckInput());
			bool flag3 = false;
			float oldLength = this.targetLength;
			if (this.IsSpinning)
			{
				this.SpinMotor(dt);
				flag = true;
			}
			if (flag2)
			{
				if (this.StickToAdjustLength)
				{
					Vector2 joystickInput = base.GetJoystickInput();
					if (Mathf.Abs(joystickInput.y) > 0.75f && Mathf.Abs(joystickInput.x) < 0.5f)
					{
						this.currentExtendedLength = Mathf.Clamp(this.currentExtendedLength + joystickInput.y * this.lengthChangeSpeed * Time.deltaTime, this.retractedLength, this.maxLength);
					}
				}
				if (!Mathf.Approximately(this.targetLength, this.currentExtendedLength))
				{
					this.targetLength = this.currentExtendedLength;
				}
				if (!Mathf.Approximately(this.targetLength, this.lastSentLength) && Time.time > this.nextAdjustmentSendTime)
				{
					this.nextAdjustmentSendTime = Time.time + this.adjustmentSendRate;
					this.lastSentLength = this.targetLength;
					flag3 = true;
				}
			}
			else if (!Mathf.Approximately(this.targetLength, this.retractedLength))
			{
				this.targetLength = this.retractedLength;
				this.lastSentLength = this.targetLength;
				flag3 = true;
			}
			if (flag3 || this.IsSpinning != isSpinning)
			{
				this.CheckPlaySounds(oldLength, this.targetLength);
				this.gameEntity.RequestState(this.gameEntity.id, this.PackStateForNetwork());
			}
		}
		if (this.hasMotor && !flag && this.motorAudio.isPlaying)
		{
			this.motorAudio.Stop();
		}
		isSpinning = this.IsSpinning;
		this.UpdateEndPoints(this.IsSpinning);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00024DFC File Offset: 0x00022FFC
	private long PackStateForNetwork()
	{
		long num = 0L;
		if (this.IsSpinning)
		{
			num |= 1L;
		}
		else if (this.hasMotor)
		{
			long num2 = (long)Mathf.RoundToInt(this.currentMotorAngle);
			num |= num2 << 1;
		}
		long num3 = (long)Mathf.Clamp(Mathf.RoundToInt(this.targetLength * 1000f), 0, 3000);
		return num | num3 << 10;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00024E5C File Offset: 0x0002305C
	private void UnpackStateFromNetwork(long state)
	{
		this.IsSpinning = ((state & 1L) != 0L);
		if (this.hasMotor && !this.IsSpinning)
		{
			this.currentMotorAngle = (float)(state >> 1 & 511L);
			this.motorTransform.localRotation = Quaternion.AngleAxis(this.currentMotorAngle, Vector3.right);
		}
		int num = (int)(state >> 10 & 4095L);
		this.targetLength = Mathf.Clamp((float)num * 0.001f, this.retractedLength, this.maxLength);
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00024EE0 File Offset: 0x000230E0
	private void SpinMotor(float dt)
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		float num = (activeSuperInfectionManager != null && activeSuperInfectionManager.IsSupercharged) ? 1.5f : 1f;
		this.currentMotorAngle = (this.currentMotorAngle + this.rotateSpeedFactor * num * dt) % 360f;
		this.motorTransform.localRotation = Quaternion.AngleAxis(this.currentMotorAngle, Vector3.right);
		if (!this.motorAudio.isPlaying)
		{
			this.motorAudio.Play();
		}
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00024F60 File Offset: 0x00023160
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		if (this.hasMotor)
		{
			if (this.IsSpinning && (this.gameEntity.heldByActorNumber >= 0 || this.gameEntity.snappedByActorNumber >= 0))
			{
				this.SpinMotor(dt);
			}
			else if (this.motorAudio.isPlaying)
			{
				this.motorAudio.Stop();
			}
		}
		this.UpdateEndPoints(false);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00024FC8 File Offset: 0x000231C8
	private bool CheckInput()
	{
		return this.buttonActivatable.CheckInput(0.25f);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00024FDC File Offset: 0x000231DC
	public override SIUpgradeSet FilterUpgradeNodes(SIUpgradeSet upgrades)
	{
		if (this.restrictedUpgrades.Length == 0)
		{
			return upgrades;
		}
		SIUpgradeSet result = default(SIUpgradeSet);
		foreach (SIUpgradeType upgrade in this.restrictedUpgrades)
		{
			if (upgrades.Contains(upgrade))
			{
				result.Add(upgrade);
			}
		}
		return result;
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00025028 File Offset: 0x00023228
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.CanTag = withUpgrades.Contains(SIUpgradeType.Stilt_Tag_Tip);
		this.CanStun = withUpgrades.Contains(SIUpgradeType.Stilt_Stun_Tip);
		this.TriggerToExtend = (this.buttonActivatable != null && withUpgrades.Contains(SIUpgradeType.Stilt_Retractable));
		this.StickToAdjustLength = (this.TriggerToExtend && withUpgrades.Contains(SIUpgradeType.Stilt_Adjustable_Length));
		this.extendSpeed = (withUpgrades.Contains(SIUpgradeType.Stilt_Retract_Speed) ? this.extendSpeedUpgraded : this.extendSpeedNormal);
		this.retractSpeed = (withUpgrades.Contains(SIUpgradeType.Stilt_Retract_Speed) ? this.retractSpeedUpgraded : this.retractSpeedNormal);
		this.maxLength = ((this.TriggerToExtend && withUpgrades.Contains(SIUpgradeType.Stilt_Max_Length)) ? this.maxLengthUpgraded : this.maxLengthNormal);
		this.currentExtendedLength = this.maxLength;
		this.targetLength = (this.TriggerToExtend ? this.retractedLength : this.currentExtendedLength);
		this.currentLength = this.targetLength;
		this.ApplyCurrentLength();
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002512C File Offset: 0x0002332C
	private void UpdateEndPoints(bool force)
	{
		if (!force && Mathf.Approximately(this.currentLength, this.targetLength))
		{
			return;
		}
		float num = (this.targetLength > this.currentLength) ? this.extendSpeed : this.retractSpeed;
		this.currentLength = Mathf.MoveTowards(this.currentLength, this.targetLength, num * Time.deltaTime);
		this.ApplyCurrentLength();
		if (this.currentStiltID != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltID, this.stiltEnd.position);
		}
		if (this.currentStiltIDB != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltIDB, this.stiltEndB.position);
		}
		if (this.currentStiltIDC != StiltID.None)
		{
			GTPlayer.Instance.UpdateStiltOffset(this.currentStiltIDC, this.stiltEndC.position);
		}
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00025200 File Offset: 0x00023400
	private void ApplyCurrentLength()
	{
		this.tip.transform.localPosition = this.offsetDir * this.currentLength + this.tipDefaultOffset;
		Vector3 localScale = this.midpoint.transform.localScale;
		localScale.z = this.currentLength;
		this.midpoint.transform.localScale = localScale;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00025268 File Offset: 0x00023468
	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal())
		{
			return;
		}
		float oldLength = this.targetLength;
		this.UnpackStateFromNetwork(newState);
		this.CheckPlaySounds(oldLength, this.targetLength);
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00025299 File Offset: 0x00023499
	private void CheckPlaySounds(float oldLength, float newLength)
	{
		if (Mathf.Approximately(oldLength, newLength))
		{
			return;
		}
		if (Mathf.Approximately(newLength, this.retractedLength))
		{
			this.retractSoundBank.Play();
			return;
		}
		if (Mathf.Approximately(oldLength, this.retractedLength))
		{
			this.extendSoundBank.Play();
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000252D8 File Offset: 0x000234D8
	private void HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		this.attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this.attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
		this.attachedVRRig = gamePlayer.rig;
		VRRig vrrig2 = this.attachedVRRig;
		vrrig2.OnMaterialIndexChanged = (Action<int, int>)Delegate.Combine(vrrig2.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		int num = this.isTagged ? 2 : 0;
		if (num != this.attachedVRRig.setMatIndex)
		{
			this.HandleVRRigMaterialIndexChanged(num, this.attachedVRRig.setMatIndex);
		}
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000253BC File Offset: 0x000235BC
	private void HandleStopInteraction()
	{
		this.attachedPlayerActorNr = -1;
		this.attachedNetPlayer = null;
		if (this.attachedVRRig != null)
		{
			VRRig vrrig = this.attachedVRRig;
			vrrig.OnMaterialIndexChanged = (Action<int, int>)Delegate.Remove(vrrig.OnMaterialIndexChanged, new Action<int, int>(this.HandleVRRigMaterialIndexChanged));
		}
		this.attachedVRRig = null;
		if (this.isTagged)
		{
			this.HandleVRRigMaterialIndexChanged(2, 0);
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x00025424 File Offset: 0x00023624
	private void HandleVRRigMaterialIndexChanged(int oldMatIndex, int newMatIndex)
	{
		if (this.attachedPlayerActorNr != -1 && (newMatIndex == 2 || newMatIndex == 1) && this.CanTag)
		{
			SuperInfectionGame superInfectionGame = GorillaGameManager.instance as SuperInfectionGame;
			if (superInfectionGame != null)
			{
				this.isTagged = (this.attachedNetPlayer != null && superInfectionGame.IsInfected(this.attachedNetPlayer));
				if (this.matDest)
				{
					this.matDest.sharedMaterial = this.tagActivatedMat;
				}
				if (this.skinnedMatDest)
				{
					this.skinnedMatDest.sharedMaterial = this.tagActivatedMat;
					goto IL_C5;
				}
				goto IL_C5;
			}
		}
		this.isTagged = false;
		if (this.matDest)
		{
			this.matDest.sharedMaterial = this.defaultMat;
		}
		if (this.skinnedMatDest)
		{
			this.skinnedMatDest.sharedMaterial = this.defaultMat;
		}
		IL_C5:
		GameObject[] array = this.tagActivatedObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(this.isTagged);
		}
	}

	// Token: 0x040007F9 RID: 2041
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x040007FA RID: 2042
	public GameObject tip;

	// Token: 0x040007FB RID: 2043
	[SerializeField]
	private Vector3 offsetDir = Vector3.forward;

	// Token: 0x040007FC RID: 2044
	private Vector3 tipDefaultOffset;

	// Token: 0x040007FD RID: 2045
	public GameObject midpoint;

	// Token: 0x040007FE RID: 2046
	public Transform stiltEnd;

	// Token: 0x040007FF RID: 2047
	private bool hasEndB;

	// Token: 0x04000800 RID: 2048
	public Transform stiltEndB;

	// Token: 0x04000801 RID: 2049
	private bool hasEndC;

	// Token: 0x04000802 RID: 2050
	public Transform stiltEndC;

	// Token: 0x04000803 RID: 2051
	public Transform motorTransform;

	// Token: 0x04000804 RID: 2052
	[SerializeField]
	private AudioSource motorAudio;

	// Token: 0x04000805 RID: 2053
	[SerializeField]
	private SIUpgradeType[] restrictedUpgrades;

	// Token: 0x04000806 RID: 2054
	[SerializeField]
	private float maxLengthNormal;

	// Token: 0x04000807 RID: 2055
	[SerializeField]
	private float maxLengthUpgraded;

	// Token: 0x04000808 RID: 2056
	[SerializeField]
	private float retractedLength;

	// Token: 0x04000809 RID: 2057
	[SerializeField]
	private float lengthChangeSpeed;

	// Token: 0x0400080A RID: 2058
	[SerializeField]
	private float maxArmLength;

	// Token: 0x0400080B RID: 2059
	[SerializeField]
	private float extendSpeedNormal;

	// Token: 0x0400080C RID: 2060
	[SerializeField]
	private float extendSpeedUpgraded;

	// Token: 0x0400080D RID: 2061
	[SerializeField]
	private float retractSpeedNormal;

	// Token: 0x0400080E RID: 2062
	[SerializeField]
	private float retractSpeedUpgraded;

	// Token: 0x0400080F RID: 2063
	[SerializeField]
	private float rotateSpeedFactor;

	// Token: 0x04000810 RID: 2064
	[SerializeField]
	private SoundBankPlayer retractSoundBank;

	// Token: 0x04000811 RID: 2065
	[SerializeField]
	private SoundBankPlayer extendSoundBank;

	// Token: 0x04000812 RID: 2066
	[SerializeField]
	private Material defaultMat;

	// Token: 0x04000813 RID: 2067
	[SerializeField]
	private Material tagActivatedMat;

	// Token: 0x04000814 RID: 2068
	[SerializeField]
	private GameObject[] tagActivatedObjects;

	// Token: 0x04000815 RID: 2069
	[SerializeField]
	private MeshRenderer matDest;

	// Token: 0x04000816 RID: 2070
	[SerializeField]
	private SkinnedMeshRenderer skinnedMatDest;

	// Token: 0x04000817 RID: 2071
	private float currentExtendedLength;

	// Token: 0x0400081D RID: 2077
	private float targetLength;

	// Token: 0x0400081E RID: 2078
	private float currentLength;

	// Token: 0x0400081F RID: 2079
	private float maxLength;

	// Token: 0x04000820 RID: 2080
	private float extendSpeed;

	// Token: 0x04000821 RID: 2081
	private float retractSpeed;

	// Token: 0x04000822 RID: 2082
	private float currentMotorAngle;

	// Token: 0x04000823 RID: 2083
	private float adjustmentSendRate = 0.25f;

	// Token: 0x04000824 RID: 2084
	private float lastSentLength;

	// Token: 0x04000825 RID: 2085
	private float nextAdjustmentSendTime = -1f;

	// Token: 0x04000826 RID: 2086
	private bool IsSpinning;

	// Token: 0x04000827 RID: 2087
	private StiltID currentStiltID = StiltID.None;

	// Token: 0x04000828 RID: 2088
	private StiltID currentStiltIDB = StiltID.None;

	// Token: 0x04000829 RID: 2089
	private StiltID currentStiltIDC = StiltID.None;

	// Token: 0x0400082A RID: 2090
	private SnapJointType wasSnappedByLocalJoint;

	// Token: 0x0400082B RID: 2091
	private const long IsSpinningBit = 1L;

	// Token: 0x0400082C RID: 2092
	private int attachedPlayerActorNr = int.MinValue;

	// Token: 0x0400082D RID: 2093
	private NetPlayer attachedNetPlayer;

	// Token: 0x0400082E RID: 2094
	private VRRig attachedVRRig;

	// Token: 0x0400082F RID: 2095
	private bool isTagged;
}
