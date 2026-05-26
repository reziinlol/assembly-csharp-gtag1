using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

// Token: 0x020007ED RID: 2029
public class GRTool : MonoBehaviour, IGameEntitySerialize, IGameEntityComponent, IGameEntityDebugComponent
{
	// Token: 0x14000059 RID: 89
	// (add) Token: 0x060033C7 RID: 13255 RVA: 0x0011D0A4 File Offset: 0x0011B2A4
	// (remove) Token: 0x060033C8 RID: 13256 RVA: 0x0011D0DC File Offset: 0x0011B2DC
	public event GRTool.EnergyChangeEvent OnEnergyChange;

	// Token: 0x1400005A RID: 90
	// (add) Token: 0x060033C9 RID: 13257 RVA: 0x0011D114 File Offset: 0x0011B314
	// (remove) Token: 0x060033CA RID: 13258 RVA: 0x0011D14C File Offset: 0x0011B34C
	public event GRTool.ToolUpgradedEvent onToolUpgraded;

	// Token: 0x060033CB RID: 13259 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x0011D181 File Offset: 0x0011B381
	private void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.RefreshMeters();
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x0011D1A4 File Offset: 0x0011B3A4
	public void OnEntityInit()
	{
		this.energy = this.GetEnergyStart();
		GhostReactor.ToolEntityCreateData toolEntityCreateData = GhostReactor.ToolEntityCreateData.Unpack(this.gameEntity.createData);
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null)
		{
			GRToolUpgradePurchaseStationFull toolUpgradeStationFullForIndex = ghostReactorManager.GetToolUpgradeStationFullForIndex(toolEntityCreateData.stationIndex);
			if (toolUpgradeStationFullForIndex != null)
			{
				toolUpgradeStationFullForIndex.InitLinkedEntity(this.gameEntity);
			}
		}
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x0011D205 File Offset: 0x0011B405
	public int GetEnergyMax()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyMax);
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x0011D213 File Offset: 0x0011B413
	public int GetEnergyUseCost()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyUseCost);
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x0011D221 File Offset: 0x0011B421
	public int GetEnergyStart()
	{
		if (!this.attributes.HasValueForAttribute(GRAttributeType.EnergyStart))
		{
			return 0;
		}
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyStart);
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x0011D23F File Offset: 0x0011B43F
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x0011D268 File Offset: 0x0011B468
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x060033D5 RID: 13269 RVA: 0x0011D291 File Offset: 0x0011B491
	public void RefillEnergy(int count, GameEntityId chargingEntityId)
	{
		this.SetEnergyInternal(this.energy + count, chargingEntityId);
	}

	// Token: 0x060033D6 RID: 13270 RVA: 0x0011D2A2 File Offset: 0x0011B4A2
	public void RefillEnergy()
	{
		this.SetEnergyInternal(this.GetEnergyMax(), GameEntityId.Invalid);
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x0011D2B5 File Offset: 0x0011B4B5
	public void UseEnergy()
	{
		this.SetEnergyInternal(this.energy - this.GetEnergyUseCost(), GameEntityId.Invalid);
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x0011D2CF File Offset: 0x0011B4CF
	public bool HasEnoughEnergy()
	{
		return this.energy >= this.GetEnergyUseCost();
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x0011D2E2 File Offset: 0x0011B4E2
	public void SetEnergy(int newEnergy)
	{
		this.SetEnergyInternal(newEnergy, GameEntityId.Invalid);
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x0011D2F0 File Offset: 0x0011B4F0
	public bool IsEnergyFull()
	{
		return this.energy >= this.GetEnergyMax();
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x0011D304 File Offset: 0x0011B504
	private void SetEnergyInternal(int value, GameEntityId chargingEntityId)
	{
		int num = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.GetEnergyMax());
		int energyChange = this.energy - num;
		GRTool.EnergyChangeEvent onEnergyChange = this.OnEnergyChange;
		if (onEnergyChange != null)
		{
			onEnergyChange(this, energyChange, chargingEntityId);
		}
		this.RefreshMeters();
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x0011D350 File Offset: 0x0011B550
	public void RefreshMeters()
	{
		for (int i = 0; i < this.energyMeters.Count; i++)
		{
			this.energyMeters[i].Refresh();
		}
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x0011D384 File Offset: 0x0011B584
	public bool HasUpgradeInstalled(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i].installedItem != null && this.upgradeSlots[i].installedItem.UpgradeType == upgradeID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x0011D3D8 File Offset: 0x0011B5D8
	public GRTool.Upgrade FindMatchingUpgrade(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				return this.upgrades[i];
			}
		}
		return null;
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x0011D420 File Offset: 0x0011B620
	public float GetPointDistanceToUpgrade(Vector3 point, GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return -1f;
		}
		if (this.upgradeListsAreValidFor != upgrade)
		{
			this.reservedMeshFilterSearchList.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<MeshFilter>(this.reservedMeshFilterSearchList);
			this.reservedMeshFilterSearchListSkinned.Clear();
			upgrade.VisibleItem[0].GetComponentsInChildren<SkinnedMeshRenderer>(false, this.reservedMeshFilterSearchListSkinned);
			this.upgradeListsAreValidFor = upgrade;
		}
		float num = float.MaxValue;
		foreach (MeshFilter meshFilter in this.reservedMeshFilterSearchList)
		{
			Vector3 vector = meshFilter.transform.InverseTransformPoint(point);
			Bounds bounds = meshFilter.sharedMesh.bounds;
			Vector3 b = new Vector3(Mathf.Clamp(vector.x, bounds.min.x, bounds.max.x), Mathf.Clamp(vector.y, bounds.min.y, bounds.max.y), Mathf.Clamp(vector.z, bounds.min.z, bounds.max.z));
			Vector3 vector2 = vector - b;
			float sqrMagnitude = meshFilter.transform.TransformVector(vector2).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
			}
		}
		if (this.reservedMeshFilterSearchListSkinned != null)
		{
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.reservedMeshFilterSearchListSkinned)
			{
				Vector3 vector3 = skinnedMeshRenderer.transform.InverseTransformPoint(point);
				Bounds localBounds = skinnedMeshRenderer.localBounds;
				Vector3 b2 = new Vector3(Mathf.Clamp(vector3.x, localBounds.min.x, localBounds.max.x), Mathf.Clamp(vector3.y, localBounds.min.y, localBounds.max.y), Mathf.Clamp(vector3.z, localBounds.min.z, localBounds.max.z));
				Vector3 vector4 = vector3 - b2;
				float sqrMagnitude2 = skinnedMeshRenderer.transform.TransformVector(vector4).sqrMagnitude;
				if (sqrMagnitude2 < num)
				{
					num = sqrMagnitude2;
				}
			}
		}
		if (num == 3.4028235E+38f)
		{
			return Vector3.Distance(point, upgrade.VisibleItem[0].transform.position);
		}
		return Mathf.Sqrt(num);
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x0011D6BC File Offset: 0x0011B8BC
	public Transform GetUpgradeAttachTransform(GRTool.Upgrade upgrade)
	{
		if (upgrade.VisibleItem.Count < 1)
		{
			return null;
		}
		return upgrade.VisibleItem[0].transform;
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x0011D6E0 File Offset: 0x0011B8E0
	public void UpgradeTool(GRToolProgressionManager.ToolParts upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].UpgradeType == upgradeID)
			{
				this.ClearUpgradeSlot(this.upgrades[i].Slot);
				for (int j = 0; j < this.upgrades[i].VisibleItem.Count; j++)
				{
					this.upgrades[i].VisibleItem[j].SetActive(true);
				}
				for (int k = 0; k < this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems.Count; k++)
				{
					this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems[k].SetActive(false);
				}
				foreach (GRBonusEntry entry in this.upgrades[i].bonusEffects)
				{
					this.attributes.AddBonus(entry);
				}
				this.upgradeSlots[this.upgrades[i].Slot].installedItem = this.upgrades[i];
				if (this.UpgradeFXNode != null && this.upgrades[i].VisibleItem.Count > 0)
				{
					this.UpgradeFXNode.transform.position = this.upgrades[i].VisibleItem[0].transform.position;
					this.UpgradeFXNode.transform.rotation = this.upgrades[i].VisibleItem[0].transform.rotation;
					ParticleSystem componentInChildren = this.UpgradeFXNode.GetComponentInChildren<ParticleSystem>();
					AudioSource componentInChildren2 = this.UpgradeFXNode.GetComponentInChildren<AudioSource>();
					if (componentInChildren != null)
					{
						componentInChildren.Play();
					}
					if (componentInChildren2 != null)
					{
						componentInChildren2.Play();
					}
				}
			}
		}
		GRTool.ToolUpgradedEvent toolUpgradedEvent = this.onToolUpgraded;
		if (toolUpgradedEvent == null)
		{
			return;
		}
		toolUpgradedEvent(this);
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x0011D934 File Offset: 0x0011BB34
	public void ClearUpgradeSlot(int slot)
	{
		if (this.upgradeSlots[slot].installedItem != null)
		{
			for (int i = 0; i < this.upgradeSlots[slot].installedItem.VisibleItem.Count; i++)
			{
				this.upgradeSlots[slot].installedItem.VisibleItem[i].SetActive(false);
			}
			foreach (GRBonusEntry entry in this.upgradeSlots[slot].installedItem.bonusEffects)
			{
				this.attributes.RemoveBonus(entry);
			}
			for (int j = 0; j < this.upgradeSlots[slot].DefaultVisibleItems.Count; j++)
			{
				this.upgradeSlots[slot].DefaultVisibleItems[j].SetActive(true);
			}
		}
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x0011DA38 File Offset: 0x0011BC38
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		writer.Write(this.upgradeSlots.Count);
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i] != null)
			{
				if (this.upgradeSlots[i].installedItem != null)
				{
					writer.Write(this.upgradeSlots[i].installedItem.UpgradeType.ToString());
				}
				else
				{
					writer.Write("");
				}
			}
			else
			{
				writer.Write("");
			}
		}
		writer.Write(this.energy);
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x0011DADC File Offset: 0x0011BCDC
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GRToolProgressionManager.ToolParts upgradeID = GRToolProgressionManager.ToolParts.None;
			if (Enum.TryParse<GRToolProgressionManager.ToolParts>(reader.ReadString(), out upgradeID))
			{
				this.UpgradeTool(upgradeID);
			}
		}
		int num2 = reader.ReadInt32();
		this.SetEnergy(num2);
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x0011DB24 File Offset: 0x0011BD24
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x0011DB7F File Offset: 0x0011BD7F
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Tool Energy: <color=\"yellow\">{0}<color=\"white\"> ", this.energy));
	}

	// Token: 0x0400437E RID: 17278
	public GRAttributes attributes;

	// Token: 0x0400437F RID: 17279
	public List<GRTool.Upgrade> upgrades;

	// Token: 0x04004380 RID: 17280
	public List<GRTool.UpgradeSlot> upgradeSlots = new List<GRTool.UpgradeSlot>();

	// Token: 0x04004381 RID: 17281
	public List<GRMeterEnergy> energyMeters;

	// Token: 0x04004382 RID: 17282
	public GameEntity gameEntity;

	// Token: 0x04004383 RID: 17283
	public GRTool.GRToolType toolType;

	// Token: 0x04004384 RID: 17284
	[ReadOnly]
	public int energy;

	// Token: 0x04004386 RID: 17286
	public GameObject UpgradeFXNode;

	// Token: 0x04004388 RID: 17288
	private List<MeshFilter> reservedMeshFilterSearchList = new List<MeshFilter>(32);

	// Token: 0x04004389 RID: 17289
	private List<SkinnedMeshRenderer> reservedMeshFilterSearchListSkinned = new List<SkinnedMeshRenderer>(32);

	// Token: 0x0400438A RID: 17290
	private GRTool.Upgrade upgradeListsAreValidFor;

	// Token: 0x020007EE RID: 2030
	public enum GRToolType
	{
		// Token: 0x0400438C RID: 17292
		None,
		// Token: 0x0400438D RID: 17293
		Club,
		// Token: 0x0400438E RID: 17294
		Collector,
		// Token: 0x0400438F RID: 17295
		Flash,
		// Token: 0x04004390 RID: 17296
		Lantern,
		// Token: 0x04004391 RID: 17297
		Revive,
		// Token: 0x04004392 RID: 17298
		ShieldGun,
		// Token: 0x04004393 RID: 17299
		DirectionalShield,
		// Token: 0x04004394 RID: 17300
		DockWrist,
		// Token: 0x04004395 RID: 17301
		EnergyEfficiency,
		// Token: 0x04004396 RID: 17302
		DropPod,
		// Token: 0x04004397 RID: 17303
		HockeyStick,
		// Token: 0x04004398 RID: 17304
		StatusWatch,
		// Token: 0x04004399 RID: 17305
		RattyBackpack
	}

	// Token: 0x020007EF RID: 2031
	[Serializable]
	public class Upgrade
	{
		// Token: 0x0400439A RID: 17306
		public GRToolProgressionManager.ToolParts UpgradeType;

		// Token: 0x0400439B RID: 17307
		public int Slot;

		// Token: 0x0400439C RID: 17308
		public List<GameObject> VisibleItem;

		// Token: 0x0400439D RID: 17309
		public List<GRBonusEntry> bonusEffects;
	}

	// Token: 0x020007F0 RID: 2032
	[Serializable]
	public class UpgradeSlot
	{
		// Token: 0x0400439E RID: 17310
		public List<GameObject> DefaultVisibleItems;

		// Token: 0x0400439F RID: 17311
		[NonSerialized]
		public GRTool.Upgrade installedItem;
	}

	// Token: 0x020007F1 RID: 2033
	// (Invoke) Token: 0x060033EB RID: 13291
	public delegate void EnergyChangeEvent(GRTool tool, int energyChange, GameEntityId chargingEntityId);

	// Token: 0x020007F2 RID: 2034
	// (Invoke) Token: 0x060033EF RID: 13295
	public delegate void ToolUpgradedEvent(GRTool tool);
}
