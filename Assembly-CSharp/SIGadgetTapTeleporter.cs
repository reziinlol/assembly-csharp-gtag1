using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class SIGadgetTapTeleporter : SIGadget
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x060006A9 RID: 1705 RVA: 0x00025570 File Offset: 0x00023770
	// (set) Token: 0x060006AA RID: 1706 RVA: 0x00025578 File Offset: 0x00023778
	public Color identifierColor { get; private set; }

	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060006AB RID: 1707 RVA: 0x00025581 File Offset: 0x00023781
	// (set) Token: 0x060006AC RID: 1708 RVA: 0x00025589 File Offset: 0x00023789
	public bool useStealthTeleporters { get; private set; }

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060006AD RID: 1709 RVA: 0x00025592 File Offset: 0x00023792
	// (set) Token: 0x060006AE RID: 1710 RVA: 0x0002559A File Offset: 0x0002379A
	public bool isVelocityPreserved { get; private set; }

	// Token: 0x17000071 RID: 113
	// (get) Token: 0x060006AF RID: 1711 RVA: 0x000255A3 File Offset: 0x000237A3
	// (set) Token: 0x060006B0 RID: 1712 RVA: 0x000255AB File Offset: 0x000237AB
	public bool hasInfiniteDuration { get; private set; }

	// Token: 0x060006B1 RID: 1713 RVA: 0x000255B4 File Offset: 0x000237B4
	public override void OnEntityInit()
	{
		this.gameEntity.OnStateChanged += this.HandleStateChanged;
		this.gameEntity.onEntityDestroyed += this.HandleOnDestroyed;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.HandleHandAttached));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.HandleHandAttached));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.HandleHandDetach));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.HandleHandDetach));
		this.identifierColor = this.GenerateColor(this.gameEntity.GetNetId());
		this.ApplyIdentifierColor();
		this.UpdateNextSelectionDisplay();
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x000256B0 File Offset: 0x000238B0
	private void HandleOnDestroyed(GameEntity entity)
	{
		if (this.gameEntity.IsAuthority())
		{
			if (this._selection1Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
			}
			if (this._selection2Teleport)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			}
		}
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x00025724 File Offset: 0x00023924
	private new void OnDisable()
	{
		this.HandleHandDetach();
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0002572C File Offset: 0x0002392C
	private void HandleHandAttached()
	{
		if (this.IsEquippedLocal())
		{
			this.isHandTapSetup = true;
			GorillaTagger.Instance.OnHandTap += this.HandleOnHandTap;
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00025753 File Offset: 0x00023953
	private void HandleHandDetach()
	{
		if (this.isHandTapSetup)
		{
			this.isHandTapSetup = false;
			GorillaTagger.Instance.OnHandTap -= this.HandleOnHandTap;
		}
		this.isActivated = false;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00025784 File Offset: 0x00023984
	private void HandleOnHandTap(bool isLeft, Vector3 position, Vector3 normal)
	{
		bool flag;
		if (base.FindAttachedHand(out flag) && isLeft == flag && this.isActivated)
		{
			this.PlaceTapTeleporter(position, normal);
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x000257B0 File Offset: 0x000239B0
	private Color GenerateColor(int seed)
	{
		Random.InitState(seed);
		float num = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		float num2 = Mathf.Lerp(this.maxBrightness, this.minBrightness, Random.value);
		Color black = Color.black;
		switch (Random.Range(0, 3))
		{
		case 0:
			black.r = num;
			black.g = num2;
			break;
		case 1:
			black.g = num;
			black.b = num2;
			break;
		case 2:
			black.b = num;
			black.r = num2;
			break;
		}
		return black;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00025848 File Offset: 0x00023A48
	protected override void OnUpdateAuthority(float dt)
	{
		this.isActivated = this.buttonActivatable.CheckInput(0.25f);
		if (this.nextPlacementDelay > 0f)
		{
			this.nextPlacementDelay -= dt;
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0002587C File Offset: 0x00023A7C
	private void PlaceTapTeleporter(Vector3 position, Vector3 normal)
	{
		if (this.nextPlacementDelay > 0f)
		{
			return;
		}
		if (!this.CheckValidTeleporterPlacement(position, normal))
		{
			return;
		}
		if (base.IsBlocked())
		{
			this.blockedSFX.Play();
			return;
		}
		base.SendClientToAuthorityRPC(0, new object[]
		{
			position,
			Quaternion.LookRotation(normal, base.transform.forward),
			this.nextSelectionId,
			this.hasInfiniteDuration ? -1f : this.portalDefaultDuration
		});
		this.CycleSelection();
		this.nextPlacementDelay = this.placementDelay;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x00025924 File Offset: 0x00023B24
	private bool CheckValidTeleporterPlacement(Vector3 position, Vector3 direction)
	{
		Vector3 point = position + direction * this.nearOffset;
		Vector3 point2 = position + direction * this.farOffset;
		return Physics.OverlapCapsuleNonAlloc(point, point2, this.overlapCheckRadius, this.overlapCheckResults, this.overlapCheckLayers) == 0;
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x00025976 File Offset: 0x00023B76
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.instanceUpgrades = withUpgrades;
		this.useStealthTeleporters = withUpgrades.Contains(SIUpgradeType.Tapteleport_Stealth);
		this.isVelocityPreserved = withUpgrades.Contains(SIUpgradeType.Tapteleport_Keep_Velocity);
		this.hasInfiniteDuration = withUpgrades.Contains(SIUpgradeType.Tapteleport_Infinite_Use);
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x000259B8 File Offset: 0x00023BB8
	public override void ProcessClientToAuthorityRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 4)
			{
				return;
			}
			Vector3 vector;
			if (!GameEntityManager.ValidateDataType<Vector3>(data[0], out vector))
			{
				return;
			}
			Quaternion rotation;
			if (!GameEntityManager.ValidateDataType<Quaternion>(data[1], out rotation))
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[2], out num))
			{
				return;
			}
			if (num < 0 || num > 100)
			{
				return;
			}
			float duration;
			if (!GameEntityManager.ValidateDataType<float>(data[3], out duration))
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			if (Vector3.Distance(vector, base.transform.position) > this.placementCheckDistance)
			{
				return;
			}
			if (!this.CheckValidTeleporterPlacement(vector, rotation * Vector3.forward))
			{
				return;
			}
			this.RemoveTeleporter(num);
			this.PlaceNewTapTeleporter(vector, rotation, num, duration);
		}
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x00025A6C File Offset: 0x00023C6C
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID == 0)
		{
			if (data == null || data.Length != 1)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			if (num < 0 || num > 1)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.nextSelectionId = num;
			this.UpdateNextSelectionDisplay();
		}
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x00025AC4 File Offset: 0x00023CC4
	private void RemoveTeleporter(int selectId)
	{
		if (selectId == 0)
		{
			if (this._selection1Teleport != null && this._selection1Teleport.gameObject.activeSelf)
			{
				this.gameEntity.manager.RequestDestroyItem(this._selection1Teleport.gameEntity.id);
				this._selection1Teleport = null;
				return;
			}
		}
		else if (selectId == 1 && this._selection2Teleport != null && this._selection2Teleport.gameObject.activeSelf)
		{
			this.gameEntity.manager.RequestDestroyItem(this._selection2Teleport.gameEntity.id);
			this._selection2Teleport = null;
		}
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x00025B6C File Offset: 0x00023D6C
	private void PlaceNewTapTeleporter(Vector3 position, Quaternion rotation, int selectionId, float duration)
	{
		GameEntityId gameEntityId = this.gameEntity.manager.RequestCreateItem(this.teleportPointPrefab.gameObject.name.GetStaticHash(), position, rotation, BitPackUtils.PackIntsIntoLong(selectionId, (int)duration));
		if (gameEntityId != GameEntityId.Invalid)
		{
			SIGadgetTapTeleporterDeployable component = this.gameEntity.manager.GetGameEntity(gameEntityId).GetComponent<SIGadgetTapTeleporterDeployable>();
			if (selectionId == 0)
			{
				if (this._selection2Teleport != null)
				{
					this._selection2Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection2Teleport);
				this._selection1Teleport = component;
			}
			else if (selectionId == 1)
			{
				if (this._selection1Teleport != null)
				{
					this._selection1Teleport.SetLink(this, component);
				}
				component.SetLink(this, this._selection1Teleport);
				this._selection2Teleport = component;
			}
			this.UpdateNewTeleporters();
		}
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x00025C40 File Offset: 0x00023E40
	private void UpdateNewTeleporters()
	{
		int value;
		if (this._selection1Teleport)
		{
			value = this._selection1Teleport.gameEntity.GetNetId();
		}
		else
		{
			value = 0;
		}
		int value2;
		if (this._selection2Teleport)
		{
			value2 = this._selection2Teleport.gameEntity.GetNetId();
		}
		else
		{
			value2 = 0;
		}
		long newState = BitPackUtils.PackIntsIntoLong(value, value2);
		this.gameEntity.RequestState(this.gameEntity.id, newState);
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00025CB0 File Offset: 0x00023EB0
	private void HandleStateChanged(long oldState, long newState)
	{
		if (this.gameEntity.IsAuthority())
		{
			return;
		}
		int netId;
		int netId2;
		BitPackUtils.UnpackIntsFromLong(newState, out netId, out netId2);
		GameEntity gameEntityFromNetId = this.gameEntity.manager.GetGameEntityFromNetId(netId);
		if (gameEntityFromNetId != null)
		{
			this._selection1Teleport = gameEntityFromNetId.GetComponent<SIGadgetTapTeleporterDeployable>();
		}
		else
		{
			this._selection1Teleport = null;
		}
		GameEntity gameEntityFromNetId2 = this.gameEntity.manager.GetGameEntityFromNetId(netId2);
		if (gameEntityFromNetId2 != null)
		{
			this._selection2Teleport = gameEntityFromNetId2.GetComponent<SIGadgetTapTeleporterDeployable>();
			return;
		}
		this._selection2Teleport = null;
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00025D34 File Offset: 0x00023F34
	private void ApplyIdentifierColor()
	{
		this.identifierColorDisplay.material.color = this.identifierColor;
	}

	// Token: 0x060006C3 RID: 1731 RVA: 0x00025D4C File Offset: 0x00023F4C
	private void UpdateNextSelectionDisplay()
	{
		if (this.nextSelectionId == 0)
		{
			this.selectionColorDisplay.material = this.selectionColor1;
			return;
		}
		if (this.nextSelectionId == 1)
		{
			this.selectionColorDisplay.material = this.selectionColor2;
		}
	}

	// Token: 0x060006C4 RID: 1732 RVA: 0x00025D82 File Offset: 0x00023F82
	public void CycleSelection()
	{
		this.nextSelectionId = (this.nextSelectionId + 1) % 2;
		this.UpdateNextSelectionDisplay();
		base.SendClientToClientRPC(0, new object[]
		{
			this.nextSelectionId
		});
	}

	// Token: 0x04000830 RID: 2096
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x04000831 RID: 2097
	[SerializeField]
	private GameObject teleportPointPrefab;

	// Token: 0x04000832 RID: 2098
	[SerializeField]
	private SoundBankPlayer blockedSFX;

	// Token: 0x04000833 RID: 2099
	[SerializeField]
	private float placementDelay = 0.5f;

	// Token: 0x04000834 RID: 2100
	[SerializeField]
	private Renderer identifierColorDisplay;

	// Token: 0x04000835 RID: 2101
	[SerializeField]
	private Renderer selectionColorDisplay;

	// Token: 0x04000836 RID: 2102
	[SerializeField]
	private Material selectionColor1;

	// Token: 0x04000837 RID: 2103
	[SerializeField]
	private Material selectionColor2;

	// Token: 0x04000838 RID: 2104
	[SerializeField]
	private float portalDefaultDuration = 30f;

	// Token: 0x04000839 RID: 2105
	private float placementCheckDistance = 0.3f;

	// Token: 0x0400083E RID: 2110
	private SIGadgetTapTeleporterDeployable _selection1Teleport;

	// Token: 0x0400083F RID: 2111
	private SIGadgetTapTeleporterDeployable _selection2Teleport;

	// Token: 0x04000840 RID: 2112
	private bool isHandTapSetup;

	// Token: 0x04000841 RID: 2113
	private bool isActivated;

	// Token: 0x04000842 RID: 2114
	private float nextPlacementDelay;

	// Token: 0x04000843 RID: 2115
	private int nextSelectionId;

	// Token: 0x04000844 RID: 2116
	private SIUpgradeSet instanceUpgrades;

	// Token: 0x04000845 RID: 2117
	private float minBrightness = 0.3f;

	// Token: 0x04000846 RID: 2118
	private float maxBrightness = 1f;

	// Token: 0x04000847 RID: 2119
	[SerializeField]
	private LayerMask overlapCheckLayers;

	// Token: 0x04000848 RID: 2120
	[SerializeField]
	private float nearOffset = 0.11f;

	// Token: 0x04000849 RID: 2121
	[SerializeField]
	private float farOffset = 0.664f;

	// Token: 0x0400084A RID: 2122
	[SerializeField]
	private float overlapCheckRadius = 0.1f;

	// Token: 0x0400084B RID: 2123
	private Collider[] overlapCheckResults = new Collider[1];
}
