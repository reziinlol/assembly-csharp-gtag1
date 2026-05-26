using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200010F RID: 271
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetPlatformDeployer : SIGadget, I_SIDisruptable, IEnergyGadget
{
	// Token: 0x06000664 RID: 1636 RVA: 0x000238C8 File Offset: 0x00021AC8
	private void Start()
	{
		this.previewPlatform.SetActive(false);
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Combine(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x00023930 File Offset: 0x00021B30
	private void OnDestroy()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnReleased = (Action)Delegate.Remove(gameEntity.OnReleased, new Action(this.HandleStopInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Remove(gameEntity2.OnUnsnapped, new Action(this.HandleStopInteraction));
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0002398B File Offset: 0x00021B8B
	private void HandleStopInteraction()
	{
		this.SetState(SIGadgetPlatformDeployer.State.Idle);
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000667 RID: 1639 RVA: 0x00023994 File Offset: 0x00021B94
	public bool UsesEnergy
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000668 RID: 1640 RVA: 0x00023997 File Offset: 0x00021B97
	public bool IsFull
	{
		get
		{
			return this.remainingRechargeTime <= 0f;
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000239AC File Offset: 0x00021BAC
	public void UpdateRecharge(float dt)
	{
		if (this.remainingRechargeTime > 0f)
		{
			int num = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.remainingRechargeTime = Mathf.Max(this.remainingRechargeTime - dt, 0f);
			int num2 = Mathf.CeilToInt(this.remainingRechargeTime / this.chargeRecoveryTime);
			this.chargeDisplay.UpdateDisplay(this.maxCharges - num2);
			if (num2 != num && this.gameEntity.IsHeldOrSnappedByLocalPlayer)
			{
				this.rechargeSFX.Play();
				bool forLeftController;
				if (base.FindAttachedHand(out forLeftController))
				{
					GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				}
			}
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x00023A6C File Offset: 0x00021C6C
	protected override void OnUpdateAuthority(float dt)
	{
		SIGadgetPlatformDeployer.State state = this.state;
		if (state != SIGadgetPlatformDeployer.State.Idle)
		{
			if (state != SIGadgetPlatformDeployer.State.Deploying)
			{
				return;
			}
			if (this.CheckReleaseInputs())
			{
				if (this.IsChargeAvailable())
				{
					this.TryDeployPlatform();
				}
				this.SetStateAuthority(SIGadgetPlatformDeployer.State.Idle);
				return;
			}
			this.UpdatePreview();
			return;
		}
		else
		{
			if (this.CheckInitInputs())
			{
				if (this.IsChargeAvailable())
				{
					if (this.isInstancePlace)
					{
						if (!this.wasInputPressed)
						{
							this.TryDeployInstantPlatform();
						}
					}
					else
					{
						this.SetStateAuthority(SIGadgetPlatformDeployer.State.Deploying);
					}
				}
				this.wasInputPressed = true;
				return;
			}
			this.wasInputPressed = false;
			return;
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x00023AEC File Offset: 0x00021CEC
	protected override void OnUpdateRemote(float dt)
	{
		SIGadgetPlatformDeployer.State state = (SIGadgetPlatformDeployer.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
		SIGadgetPlatformDeployer.State state2 = this.state;
		if (state2 != SIGadgetPlatformDeployer.State.Idle && state2 == SIGadgetPlatformDeployer.State.Deploying)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00023B2C File Offset: 0x00021D2C
	private bool CheckInitInputs()
	{
		if (!this.buttonActivatable.CheckInput(this.inputSensitivity))
		{
			return false;
		}
		if (this.isInstancePlace)
		{
			return true;
		}
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		return Vector3.Distance(position, position2) <= this.activationHandDistance;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00023B8D File Offset: 0x00021D8D
	private bool CheckReleaseInputs()
	{
		return !this.buttonActivatable.CheckInput(this.inputSensitivity);
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00023BA3 File Offset: 0x00021DA3
	private bool IsChargeAvailable()
	{
		return (float)this.maxCharges * this.chargeRecoveryTime - this.remainingRechargeTime > this.chargeRecoveryTime;
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00023BC5 File Offset: 0x00021DC5
	private void SpendCharge()
	{
		this.remainingRechargeTime += this.chargeRecoveryTime;
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00023BDA File Offset: 0x00021DDA
	private static bool IsLeftHandOrSnapSlot(int handIndex)
	{
		return handIndex == 0 || handIndex == 2;
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00023BE8 File Offset: 0x00021DE8
	private void TryDeployInstantPlatform()
	{
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		GamePlayer gamePlayer;
		if (!this.TryGetGamePlayer(out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindSnapIndex(this.gameEntity.id);
		if (num == -1)
		{
			num = gamePlayer.FindHandIndex(this.gameEntity.id);
		}
		if (num == -1)
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			vector = base.transform.position - base.transform.up * this.handDepthOffset;
			quaternion = base.transform.rotation;
			Debug.DrawRay(base.transform.position, -base.transform.up * 0.3f, Color.blue, 10f);
			Debug.DrawRay(base.transform.position, base.transform.forward * 0.3f, Color.blue, 10f);
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		else
		{
			Transform transform = SIGadgetPlatformDeployer.IsLeftHandOrSnapSlot(num) ? gamePlayer.leftHand : gamePlayer.rightHand;
			vector = transform.position;
			Vector3 up = transform.up;
			Vector3 right = transform.right;
			Debug.DrawRay(vector, right * 0.3f, Color.red, 10f);
			Debug.DrawRay(vector, up * 0.3f, Color.red, 10f);
			quaternion = Quaternion.LookRotation(up, right);
			vector += right * this.handDepthOffset;
			Debug.DrawRay(vector, quaternion * Vector3.forward * 0.3f, Color.green, 10f);
		}
		this.DeployPlatform(vector, quaternion);
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00023DC0 File Offset: 0x00021FC0
	private void TryDeployPlatform()
	{
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		Vector3 position = gamePlayer.leftHand.position;
		Vector3 position2 = gamePlayer.rightHand.position;
		if (Vector3.Distance(position, position2) > this.deployMinRequiredHandDistance)
		{
			if (base.IsBlocked())
			{
				this.blockedSFX.Play();
				return;
			}
			Vector3 pos;
			Quaternion rot;
			Vector3 vector;
			if (this.TryGetPlatformPosRotScale(out pos, out rot, out vector))
			{
				this.DeployPlatform(pos, rot);
				return;
			}
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00023E2C File Offset: 0x0002202C
	private void DeployPlatform(Vector3 pos, Quaternion rot)
	{
		this.SpendCharge();
		this.CreateLocalPlatformInstance(pos, rot);
		int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
		if (this.gameEntity.IsAuthority())
		{
			base.SendAuthorityToClientRPC(0, new object[]
			{
				actorNumber,
				pos,
				rot
			});
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[]
		{
			actorNumber,
			pos,
			rot
		});
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x00023EB8 File Offset: 0x000220B8
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
			{
				return;
			}
			Quaternion rot;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rot))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(base.transform.position, vector) > 2f)
			{
				return;
			}
			this.CreateLocalPlatformInstance(vector, rot);
			base.SendAuthorityToClientRPC(0, data);
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x00023F3C File Offset: 0x0002213C
	public override void ProcessAuthorityToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 3)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			Vector3 pos;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out pos))
			{
				return;
			}
			Quaternion rot;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rot))
			{
				return;
			}
			if (num != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.CreateLocalPlatformInstance(pos, rot);
			}
		}
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x00023F98 File Offset: 0x00022198
	private void CreateLocalPlatformInstance(Vector3 pos, Quaternion rot)
	{
		if (this.deployedPlatformCount >= this.maxCharges)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this.platformPrefab, true);
		if (gameObject != null)
		{
			SIGadgetPlatformDeployerPlatform component = gameObject.GetComponent<SIGadgetPlatformDeployerPlatform>();
			if (component != null)
			{
				this.deployedPlatformCount++;
				SIGadgetPlatformDeployerPlatform sigadgetPlatformDeployerPlatform = component;
				sigadgetPlatformDeployerPlatform.OnDisabled = (Action)Delegate.Combine(sigadgetPlatformDeployerPlatform.OnDisabled, new Action(delegate()
				{
					this.deployedPlatformCount--;
				}));
			}
			gameObject.transform.SetPositionAndRotation(pos, rot);
			ISIGameDeployable isigameDeployable;
			if (gameObject.TryGetComponent<ISIGameDeployable>(out isigameDeployable))
			{
				isigameDeployable.ApplyUpgrades(this.instanceUpgrades);
			}
		}
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x00024032 File Offset: 0x00022232
	private void SetStateAuthority(SIGadgetPlatformDeployer.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00024054 File Offset: 0x00022254
	private void SetState(SIGadgetPlatformDeployer.State newState)
	{
		if (newState == this.state || !this.CanChangeState((long)newState))
		{
			return;
		}
		this.state = newState;
		SIGadgetPlatformDeployer.State state = this.state;
		if (state == SIGadgetPlatformDeployer.State.Idle)
		{
			this.SetPreviewVisibility(false);
			return;
		}
		if (state != SIGadgetPlatformDeployer.State.Deploying)
		{
			return;
		}
		this.SetPreviewVisibility(true);
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0002409A File Offset: 0x0002229A
	public bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 2L;
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x000240A9 File Offset: 0x000222A9
	private void SetPreviewVisibility(bool enabled)
	{
		this.previewPlatform.SetActive(enabled);
		if (enabled)
		{
			this.UpdatePreview();
		}
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x000240C0 File Offset: 0x000222C0
	private void UpdatePreview()
	{
		Vector3 position;
		Quaternion rotation;
		Vector3 localScale;
		if (this.TryGetPlatformPosRotScale(out position, out rotation, out localScale))
		{
			this.previewPlatform.transform.SetPositionAndRotation(position, rotation);
			this.previewPlatform.transform.localScale = localScale;
			GamePlayer gamePlayer;
			if (this.TryGetGamePlayer(out gamePlayer))
			{
				Vector3 position2 = gamePlayer.leftHand.position;
				Vector3 position3 = gamePlayer.rightHand.position;
				if (Vector3.Distance(position2, position3) > this.deployMinRequiredHandDistance)
				{
					this.previewMesh.material = this.validPreviewMaterial;
					return;
				}
				this.previewMesh.material = this.invalidPreviewMaterial;
			}
		}
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00024154 File Offset: 0x00022354
	private bool TryGetPlatformPosRotScale(out Vector3 pos, out Quaternion rot, out Vector3 scale)
	{
		pos = Vector3.zero;
		rot = Quaternion.identity;
		scale = Vector3.one;
		GamePlayer gamePlayer;
		if (this.TryGetGamePlayer(out gamePlayer))
		{
			Vector3 position = gamePlayer.leftHand.position;
			Vector3 position2 = gamePlayer.rightHand.position;
			Vector3 position3 = gamePlayer.rig.head.rigTarget.position;
			Vector3 vector = (position + position2) / 2f;
			Vector3 normalized = (position3 - vector).normalized;
			Vector3 forward = Vector3.ProjectOnPlane((position - position2).normalized, normalized);
			pos = vector + -normalized * this.handDepthOffset;
			rot = Quaternion.LookRotation(forward, normalized);
			return true;
		}
		return false;
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00024228 File Offset: 0x00022428
	private bool TryGetGamePlayer(out GamePlayer player)
	{
		player = null;
		return GamePlayer.TryGetGamePlayer(this.gameEntity.snappedByActorNumber, out player) || GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out player);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00024258 File Offset: 0x00022458
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		bool flag = withUpgrades.Contains(SIUpgradeType.Platform_Capacity);
		this.maxCharges = (flag ? this.maxChargesHighCapacity : this.maxChargesDefault);
		this.chargeDisplay = (flag ? this.chargeDisplayHighCapacity : this.chargeDisplayDefault);
		this.chargeRecoveryTime = (withUpgrades.Contains(SIUpgradeType.Platform_Cooldown) ? this.chargeRecoveryTimeFast : this.chargeRecoveryTimeDefault);
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x000242C9 File Offset: 0x000224C9
	public void Disrupt(float disruptTime)
	{
		this.remainingRechargeTime = (float)this.maxCharges * this.chargeRecoveryTime + disruptTime;
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x000242E1 File Offset: 0x000224E1
	protected override void HandleBlockedActionChanged(bool isBlocked)
	{
		this.blockedDisplayMesh.material = (isBlocked ? this.blockedMat : this.unblockedMat);
	}

	// Token: 0x040007CB RID: 1995
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x040007CC RID: 1996
	[SerializeField]
	private SoundBankPlayer rechargeSFX;

	// Token: 0x040007CD RID: 1997
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x040007CE RID: 1998
	[SerializeField]
	private MeshRenderer blockedDisplayMesh;

	// Token: 0x040007CF RID: 1999
	[SerializeField]
	private Material unblockedMat;

	// Token: 0x040007D0 RID: 2000
	[SerializeField]
	private Material blockedMat;

	// Token: 0x040007D1 RID: 2001
	[SerializeField]
	private GameObject platformPrefab;

	// Token: 0x040007D2 RID: 2002
	[Header("Activation")]
	[SerializeField]
	private bool isInstancePlace;

	// Token: 0x040007D3 RID: 2003
	[SerializeField]
	private float activationHandDistance = 0.2f;

	// Token: 0x040007D4 RID: 2004
	[SerializeField]
	private float inputSensitivity = 0.25f;

	// Token: 0x040007D5 RID: 2005
	[Header("Deploy")]
	[SerializeField]
	private float deployMinRequiredHandDistance = 0.2f;

	// Token: 0x040007D6 RID: 2006
	[SerializeField]
	private GameObject previewPlatform;

	// Token: 0x040007D7 RID: 2007
	[SerializeField]
	private float handInset = 0.1f;

	// Token: 0x040007D8 RID: 2008
	[SerializeField]
	private float handDepthOffset = 0.3f;

	// Token: 0x040007D9 RID: 2009
	[SerializeField]
	private MeshRenderer previewMesh;

	// Token: 0x040007DA RID: 2010
	[SerializeField]
	private Material validPreviewMaterial;

	// Token: 0x040007DB RID: 2011
	[SerializeField]
	private Material invalidPreviewMaterial;

	// Token: 0x040007DC RID: 2012
	[Header("Charges")]
	private int maxCharges = 3;

	// Token: 0x040007DD RID: 2013
	private float chargeRecoveryTime = 10f;

	// Token: 0x040007DE RID: 2014
	private SIChargeDisplay chargeDisplay;

	// Token: 0x040007DF RID: 2015
	[SerializeField]
	private int maxChargesDefault = 3;

	// Token: 0x040007E0 RID: 2016
	[SerializeField]
	private int maxChargesHighCapacity = 5;

	// Token: 0x040007E1 RID: 2017
	[SerializeField]
	private SIChargeDisplay chargeDisplayDefault;

	// Token: 0x040007E2 RID: 2018
	[SerializeField]
	private SIChargeDisplay chargeDisplayHighCapacity;

	// Token: 0x040007E3 RID: 2019
	[SerializeField]
	private float chargeRecoveryTimeDefault = 10f;

	// Token: 0x040007E4 RID: 2020
	[SerializeField]
	private float chargeRecoveryTimeFast = 5f;

	// Token: 0x040007E5 RID: 2021
	private SIGadgetPlatformDeployer.State state;

	// Token: 0x040007E6 RID: 2022
	private bool wasInputPressed;

	// Token: 0x040007E7 RID: 2023
	private float remainingRechargeTime;

	// Token: 0x040007E8 RID: 2024
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x040007E9 RID: 2025
	private const float MAX_DEPLOY_DIST = 2f;

	// Token: 0x040007EA RID: 2026
	private int deployedPlatformCount;

	// Token: 0x02000110 RID: 272
	private enum State
	{
		// Token: 0x040007EC RID: 2028
		Idle,
		// Token: 0x040007ED RID: 2029
		Deploying,
		// Token: 0x040007EE RID: 2030
		Count
	}
}
