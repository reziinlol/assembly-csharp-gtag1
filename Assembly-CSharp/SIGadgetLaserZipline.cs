using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class SIGadgetLaserZipline : SIGadget, ICallBack
{
	// Token: 0x060005BB RID: 1467 RVA: 0x00020F20 File Offset: 0x0001F120
	private static void AccumulateVelocity(Vector3 desiredVelocity)
	{
		if (SIGadgetLaserZipline.s_localPlayerVelocityFrame != Time.frameCount)
		{
			SIGadgetLaserZipline.s_localPlayerVelocityFrame = Time.frameCount;
			SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity = Vector3.zero;
			SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities = 0;
		}
		SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity += desiredVelocity;
		SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities++;
		GTPlayer.Instance.SetVelocity(SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity / (float)SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00020F8C File Offset: 0x0001F18C
	private static void ResetLocalAppliedPositionOffset()
	{
		if (SIGadgetLaserZipline.s_LocalPlayerPositionFrame != Time.frameCount)
		{
			SIGadgetLaserZipline.s_LocalPlayerPositionFrame = Time.frameCount;
			SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset = Vector3.zero;
			SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset = Vector3.zero;
			return;
		}
		GTPlayer.Instance.transform.position -= SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020FE3 File Offset: 0x0001F1E3
	private static void ReapplyPositionOffset()
	{
		GTPlayer.Instance.transform.position += SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00021004 File Offset: 0x0001F204
	private static void AccumulateAndApplyLocalPositionOffset(Vector3 offset)
	{
		SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset += offset;
		SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities++;
		SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset = SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset / (float)SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities;
		GTPlayer.Instance.transform.position += SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00021064 File Offset: 0x0001F264
	private void Awake()
	{
		this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		this.laserBeam.SetActive(false);
		this.laserBeam.transform.SetParent(null);
		this.groundedCooldown = new ResettableUseCounter(1, this.maxSuperchargeUses, new Action<bool>(this.ShowReady));
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

	// Token: 0x060005C0 RID: 1472 RVA: 0x0002116B File Offset: 0x0001F36B
	private void ClearCallback()
	{
		if (this.hasActiveCallback)
		{
			this.activeCallbackOnRig.RemoveLateUpdateCallback(this);
			this.activeCallbackOnRig = null;
			this.hasActiveCallback = false;
			SIPlayer.LocalPlayer.OnKnockback -= this.OnKnockback;
		}
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x000211A5 File Offset: 0x0001F3A5
	private void ShowReady(bool isReady)
	{
		if (isReady)
		{
			this.audioRecharged.Play();
		}
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x000211B5 File Offset: 0x0001F3B5
	private void OnDestroy()
	{
		this.ClearCallback();
		this.laserBeam.gameObject.Destroy();
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnGrabbed()
	{
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnSnapped()
	{
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x000211CD File Offset: 0x0001F3CD
	private void OnReleased()
	{
		this.wasTriggerPressed = false;
		this.laserBeam.SetActive(false);
		this.ClearCallback();
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x000211CD File Offset: 0x0001F3CD
	private void OnUnsnapped()
	{
		this.wasTriggerPressed = false;
		this.laserBeam.SetActive(false);
		this.ClearCallback();
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000211E8 File Offset: 0x0001F3E8
	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.m_buttonActivatable.CheckInput(0.25f);
		bool flag2 = GTPlayer.Instance.IsGroundedButt || GTPlayer.Instance.IsGroundedHand || GTPlayer.Instance.IsTentacleActive;
		if (flag2)
		{
			this.groundedCooldown.Reset();
		}
		if (flag)
		{
			if (this.isLineBroken)
			{
				return;
			}
			if (flag2)
			{
				if (this.wasSlidingUngrounded)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
					return;
				}
			}
			else
			{
				this.wasSlidingUngrounded = true;
			}
			if (!this.wasTriggerPressed)
			{
				if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				if (Time.time < this.coolingDownUntilTimestamp)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				if (this.cooldownOnUseUntilTouchGround && !this.groundedCooldown.TryUse())
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
				if (activeSuperInfectionManager == null || !activeSuperInfectionManager.IsSupercharged)
				{
					this.onUseAudio.PlayOneShot(this.audioSingleUse);
				}
				else
				{
					this.onUseAudio.PlayOneShot(this.groundedCooldown.IsReady ? this.audioReusable : this.audioUsedUp);
				}
				this.laserBeam.SetActive(true);
				this.laserBeam.transform.localPosition = Vector3.zero;
				VRRig.LocalRig.AddLateUpdateCallback(this);
				SIPlayer.LocalPlayer.OnKnockback += this.OnKnockback;
				this.activeCallbackOnRig = VRRig.LocalRig;
				this.hasActiveCallback = true;
				this.activatedAtPoint = this.zipline.transform.TransformPoint(this.ziplineAnchorOffset);
				this.ziplineDirection = this.zipline.transform.forward;
				Vector3 up = VRRig.LocalRig.transform.up;
				if (Vector3.Dot(this.ziplineDirection, up) > 0f)
				{
					this.ziplineDirection = -this.ziplineDirection;
				}
				if (Vector3.Dot(this.ziplineDirection, up) > -0.5f)
				{
					this.ziplineDirection = Vector3.ProjectOnPlane(this.ziplineDirection, up);
					this.ziplineDirection.Normalize();
					this.ziplineDirection += up * -0.5f;
					this.ziplineDirection.Normalize();
				}
				this.activatedAtRotation = Quaternion.LookRotation(this.ziplineDirection);
				this.wasTriggerPressed = true;
				this.wasSlidingUngrounded = (!GTPlayer.Instance.IsGroundedButt && !GTPlayer.Instance.IsGroundedHand);
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
			}
			if (base.IsBlocked(SIExclusionType.AffectsLocalMovement))
			{
				this.isLineBroken = true;
				this.laserBeam.SetActive(false);
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
				return;
			}
			Vector3 rigidbodyVelocity = GTPlayer.Instance.RigidbodyVelocity;
			GTPlayer.Instance.LaserZiplineActiveAtFrame = Time.frameCount + 1;
			float magnitude = rigidbodyVelocity.magnitude;
			float num = Vector3.Dot(GTPlayer.Instance.RigidbodyVelocity, this.ziplineDirection);
			if (this._speedBoost > 0f && num < this.speedBoostVelocityCap)
			{
				num += Time.deltaTime * this._speedBoost;
			}
			SIGadgetLaserZipline.AccumulateVelocity(this.ziplineDirection * num);
			this.UpdateAudioPitch(magnitude);
			this.wasTriggerPressed = true;
			return;
		}
		else
		{
			if (this.wasTriggerPressed)
			{
				this.laserBeam.SetActive(false);
				this.zipline.transform.localRotation = Quaternion.identity;
				this.isLineBroken = false;
				this.wasTriggerPressed = false;
				this.wasSlidingUngrounded = false;
				this.coolingDownUntilTimestamp = Time.time + this.cooldownDuration;
				float d = Vector3.Dot(GTPlayer.Instance.RigidbodyVelocity, this.ziplineDirection);
				SIGadgetLaserZipline.AccumulateVelocity(this.ziplineDirection * d);
				bool isLeftHand;
				if (base.FindAttachedHand(out isLeftHand))
				{
					GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
					float scale = GTPlayer.Instance.scale;
					Vector3 vector = GTPlayer.Instance.turnParent.transform.rotation * -interactPointVelocityTracker.GetAverageVelocity(false, 0.1f, true) * scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * scale);
					GTPlayer.Instance.AddForce(vector, ForceMode.VelocityChange);
				}
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
				return;
			}
			this.isLineBroken = false;
			Vector3 vector2 = this.zipline.parent.forward;
			Vector3 up2 = VRRig.LocalRig.transform.up;
			if (Mathf.Abs(Vector3.Dot(vector2, up2)) < 0.5f)
			{
				vector2 = Vector3.ProjectOnPlane(vector2, up2);
				vector2.Normalize();
				vector2 += up2 * -0.5f;
			}
			Quaternion b = this.zipline.parent.InverseTransformRotation(Quaternion.LookRotation(vector2));
			this.zipline.transform.localRotation = Quaternion.Lerp(this.zipline.transform.localRotation, b, Time.deltaTime * 25f);
			return;
		}
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0002172F File Offset: 0x0001F92F
	private long GetStateLong()
	{
		if (this.wasTriggerPressed && !this.isLineBroken)
		{
			return BitPackUtils.PackAnchoredPosRotForNetwork(this.activatedAtPoint, this.activatedAtRotation);
		}
		return 0L;
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00021758 File Offset: 0x0001F958
	protected override void OnUpdateRemote(float dt)
	{
		if (this.laserBeam.activeSelf && this.activeCallbackOnRig != null)
		{
			this.UpdateAudioPitch(this.activeCallbackOnRig.LatestVelocity().magnitude);
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00021799 File Offset: 0x0001F999
	private void OnKnockback(Vector3 knockbackVector)
	{
		if (this.wasTriggerPressed)
		{
			this.isLineBroken = true;
			this.laserBeam.SetActive(false);
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x000217B8 File Offset: 0x0001F9B8
	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			return;
		}
		if (newState != 0L)
		{
			int attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
			GamePlayer gamePlayer;
			if (attachedPlayerActorNr >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNr, out gamePlayer))
			{
				Vector3 vector;
				Quaternion rotation;
				BitPackUtils.UnpackAnchoredPosRotForNetwork(newState, gamePlayer.rig.transform.position, out vector, out rotation);
				this.activatedAtPoint = vector;
				this.activatedAtRotation = rotation;
				this.ziplineDirection = rotation * Vector3.forward;
				this.laserBeam.SetActive(true);
				gamePlayer.rig.AddLateUpdateCallback(this);
				this.activeCallbackOnRig = gamePlayer.rig;
				this.hasActiveCallback = true;
				this.wasTriggerPressed = true;
				this.isLineBroken = false;
				return;
			}
		}
		else
		{
			this.wasTriggerPressed = false;
			this.isLineBroken = false;
			this.laserBeam.SetActive(false);
			this.ClearCallback();
		}
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00021890 File Offset: 0x0001FA90
	public void CallBack()
	{
		if (!this.wasTriggerPressed || this.isLineBroken)
		{
			this.ClearCallback();
			return;
		}
		if (this.IsEquippedLocal())
		{
			SIGadgetLaserZipline.ResetLocalAppliedPositionOffset();
			Vector3 vector = this.activatedAtPoint - this.zipline.transform.TransformPoint(this.ziplineAnchorOffset);
			vector = vector.ProjectOnPlane(Vector3.zero, this.ziplineDirection);
			if (vector.sqrMagnitude > 1f)
			{
				this.isLineBroken = true;
				this.laserBeam.SetActive(false);
				SIGadgetLaserZipline.ReapplyPositionOffset();
				return;
			}
			SIGadgetLaserZipline.AccumulateAndApplyLocalPositionOffset(vector);
		}
		this.zipline.transform.rotation = this.activatedAtRotation;
		Vector3 position = this.activatedAtPoint + Vector3.Project(this.zipline.transform.TransformPoint(this.ziplineAnchorOffset) - this.activatedAtPoint, this.ziplineDirection);
		this.laserBeam.transform.position = position;
		this.laserBeam.transform.rotation = this.activatedAtRotation;
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00021997 File Offset: 0x0001FB97
	private void UpdateAudioPitch(float playerSpeed)
	{
		this.laserBeamAudio.pitch = 1f + playerSpeed / 30f;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x000219B1 File Offset: 0x0001FBB1
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._speedBoost = (withUpgrades.Contains(SIUpgradeType.AirControl_Zipline_Speed) ? this.upgradedSpeedBoost : 0f);
	}

	// Token: 0x04000709 RID: 1801
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400070A RID: 1802
	[SerializeField]
	private Transform zipline;

	// Token: 0x0400070B RID: 1803
	[SerializeField]
	private Vector3 ziplineAnchorOffset;

	// Token: 0x0400070C RID: 1804
	[SerializeField]
	private GameObject laserBeam;

	// Token: 0x0400070D RID: 1805
	[SerializeField]
	private AudioSource laserBeamAudio;

	// Token: 0x0400070E RID: 1806
	[SerializeField]
	private AudioSource onUseAudio;

	// Token: 0x0400070F RID: 1807
	[SerializeField]
	private float cooldownDuration;

	// Token: 0x04000710 RID: 1808
	[SerializeField]
	private bool cooldownOnUseUntilTouchGround;

	// Token: 0x04000711 RID: 1809
	[SerializeField]
	private int maxSuperchargeUses = 2;

	// Token: 0x04000712 RID: 1810
	[SerializeField]
	private AudioClip audioSingleUse;

	// Token: 0x04000713 RID: 1811
	[SerializeField]
	private AudioClip audioReusable;

	// Token: 0x04000714 RID: 1812
	[SerializeField]
	private AudioClip audioUsedUp;

	// Token: 0x04000715 RID: 1813
	[SerializeField]
	private SoundBankPlayer audioRecharged;

	// Token: 0x04000716 RID: 1814
	[Header("Upgrades")]
	[SerializeField]
	private float upgradedSpeedBoost = 5f;

	// Token: 0x04000717 RID: 1815
	[SerializeField]
	private float speedBoostVelocityCap = 10f;

	// Token: 0x04000718 RID: 1816
	private bool hasActiveCallback;

	// Token: 0x04000719 RID: 1817
	private VRRig activeCallbackOnRig;

	// Token: 0x0400071A RID: 1818
	private bool wasTriggerPressed;

	// Token: 0x0400071B RID: 1819
	private bool isLineBroken;

	// Token: 0x0400071C RID: 1820
	private bool wasSlidingUngrounded;

	// Token: 0x0400071D RID: 1821
	private Quaternion activatedAtRotation;

	// Token: 0x0400071E RID: 1822
	private Vector3 activatedAtPoint;

	// Token: 0x0400071F RID: 1823
	private Vector3 ziplineDirection;

	// Token: 0x04000720 RID: 1824
	private float coolingDownUntilTimestamp;

	// Token: 0x04000721 RID: 1825
	private ResettableUseCounter groundedCooldown;

	// Token: 0x04000722 RID: 1826
	private float _speedBoost;

	// Token: 0x04000723 RID: 1827
	private static int s_localPlayerVelocityFrame = -1;

	// Token: 0x04000724 RID: 1828
	private static Vector3 s_LocalPlayerAccumulatedVelocity;

	// Token: 0x04000725 RID: 1829
	private static int s_LocalPlayerNumAccumulatedVelocities;

	// Token: 0x04000726 RID: 1830
	private static int s_LocalPlayerPositionFrame = -1;

	// Token: 0x04000727 RID: 1831
	private static Vector3 s_LocalPlayerAccumulatedPositionOffset;

	// Token: 0x04000728 RID: 1832
	private static Vector3 s_LocalPlayerAppliedPositionOffset;
}
