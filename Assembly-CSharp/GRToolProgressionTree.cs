using System;
using System.Collections.Generic;
using GorillaNetworking;

// Token: 0x02000802 RID: 2050
public class GRToolProgressionTree
{
	// Token: 0x06003485 RID: 13445 RVA: 0x00120FE0 File Offset: 0x0011F1E0
	public GRToolProgressionTree()
	{
		this.InitializeToolMapping();
		this.InitializeClubPartMapping();
		this.InitializeFlashPartMapping();
		this.InitializeRevivePartMapping();
		this.InitializeCollectorPartMapping();
		this.InitializeLanternPartMapping();
		this.InitializeShieldGunPartMapping();
		this.InitializeDirectionalShieldPartMapping();
		this.InitializeEnergyEfficiencyPartMapping();
		this.InitializeDockWristPartMapping();
		this.InitializeDropPodPartMapping();
	}

	// Token: 0x06003486 RID: 13446 RVA: 0x001210BC File Offset: 0x0011F2BC
	public void Init(GhostReactor ghostReactor, GRToolProgressionManager toolManager)
	{
		this.reactor = ghostReactor;
		this.manager = toolManager;
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated += this.OnProgressionTreeUpdate;
			ProgressionManager.Instance.OnInventoryUpdated += this.OnInventoryUpdated;
		}
		this.RefreshProgressionTree();
		this.RefreshUserInventory();
	}

	// Token: 0x06003487 RID: 13447 RVA: 0x0012111C File Offset: 0x0011F31C
	public string GetTreeId()
	{
		return this.treeId;
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x00121124 File Offset: 0x0011F324
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		List<GRTool.GRToolType> list = new List<GRTool.GRToolType>();
		foreach (GRTool.GRToolType item in this.toolTree.Keys)
		{
			list.Add(item);
		}
		return list;
	}

	// Token: 0x06003489 RID: 13449 RVA: 0x00121184 File Offset: 0x0011F384
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		List<GRToolProgressionTree.GRToolProgressionNode> result = new List<GRToolProgressionTree.GRToolProgressionNode>();
		this.AddToolProgressionChildren(this.toolTree[tool], ref result);
		return result;
	}

	// Token: 0x0600348A RID: 13450 RVA: 0x001211AC File Offset: 0x0011F3AC
	public GRToolProgressionTree.GRToolProgressionNode GetToolNode(GRTool.GRToolType tool)
	{
		if (this.toolTree.ContainsKey(tool))
		{
			return this.toolTree[tool];
		}
		return null;
	}

	// Token: 0x0600348B RID: 13451 RVA: 0x001211CA File Offset: 0x0011F3CA
	public GRToolProgressionTree.GRToolProgressionNode GetPartNode(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			return this.partTree[part];
		}
		return null;
	}

	// Token: 0x0600348C RID: 13452 RVA: 0x001211E8 File Offset: 0x0011F3E8
	public void RefreshProgressionTree()
	{
		ProgressionManager.Instance.RefreshProgressionTree();
	}

	// Token: 0x0600348D RID: 13453 RVA: 0x001211F4 File Offset: 0x0011F3F4
	public void RefreshUserInventory()
	{
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x0600348E RID: 13454 RVA: 0x00121200 File Offset: 0x0011F400
	private void OnProgressionTreeUpdate()
	{
		UserHydratedProgressionTreeResponse tree = ProgressionManager.Instance.GetTree(this.treeName);
		if (tree != null)
		{
			this.ProcessToolProgressionTree(tree);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x00121238 File Offset: 0x0011F438
	private void OnInventoryUpdated()
	{
		ProgressionManager.MothershipItemSummary mothershipItemSummary;
		if (ProgressionManager.Instance.GetInventoryItem(this.researchPointsEntitlement, out mothershipItemSummary))
		{
			this.currentResearchPoints = mothershipItemSummary.Quantity;
		}
		ProgressionManager.MothershipItemSummary mothershipItemSummary2;
		ProgressionManager.MothershipItemSummary mothershipItemSummary3;
		ProgressionManager.MothershipItemSummary mothershipItemSummary4;
		if (ProgressionManager.Instance.GetInventoryItem(this.fullTimeEntitlement, out mothershipItemSummary2))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.partTimeEntitlement, out mothershipItemSummary3))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.internEntitlement, out mothershipItemSummary4))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.Intern;
		}
		else
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.None;
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x001212D3 File Offset: 0x0011F4D3
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmploymentLevel()
	{
		return this.currentEmploymentLevel;
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x001212DC File Offset: 0x0011F4DC
	private void AddToolProgressionChildren(GRToolProgressionTree.GRToolProgressionNode currentNode, ref List<GRToolProgressionTree.GRToolProgressionNode> list)
	{
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in currentNode.children)
		{
			list.Add(grtoolProgressionNode);
			this.AddToolProgressionChildren(grtoolProgressionNode, ref list);
		}
	}

	// Token: 0x06003492 RID: 13458 RVA: 0x00121338 File Offset: 0x0011F538
	public int GetNumberOfResearchPoints()
	{
		return this.currentResearchPoints;
	}

	// Token: 0x06003493 RID: 13459 RVA: 0x00121340 File Offset: 0x0011F540
	private void InitializeToolMapping()
	{
		this.toolMapping["ChargeBaton"] = GRTool.GRToolType.Club;
		this.toolMapping["FlashTool"] = GRTool.GRToolType.Flash;
		this.toolMapping["Revive"] = GRTool.GRToolType.Revive;
		this.toolMapping["Collector"] = GRTool.GRToolType.Collector;
		this.toolMapping["Lantern"] = GRTool.GRToolType.Lantern;
		this.toolMapping["ShieldGun"] = GRTool.GRToolType.ShieldGun;
		this.toolMapping["DirectionalShield"] = GRTool.GRToolType.DirectionalShield;
		this.toolMapping["DockWrist"] = GRTool.GRToolType.DockWrist;
		this.toolMapping["EnergyEfficiency"] = GRTool.GRToolType.EnergyEfficiency;
		this.toolMapping["DropPodBasic"] = GRTool.GRToolType.DropPod;
	}

	// Token: 0x06003494 RID: 13460 RVA: 0x001213FC File Offset: 0x0011F5FC
	private void InitializeClubPartMapping()
	{
		this.partMapping["ChargeBaton"] = GRToolProgressionManager.ToolParts.Baton;
		this.partMapping["BatonDamage1"] = GRToolProgressionManager.ToolParts.BatonDamage1;
		this.partMapping["BatonDamage2"] = GRToolProgressionManager.ToolParts.BatonDamage2;
		this.partMapping["BatonDamage3"] = GRToolProgressionManager.ToolParts.BatonDamage3;
	}

	// Token: 0x06003495 RID: 13461 RVA: 0x00121450 File Offset: 0x0011F650
	private void InitializeFlashPartMapping()
	{
		this.partMapping["FlashTool"] = GRToolProgressionManager.ToolParts.Flash;
		this.partMapping["FlashDamage1"] = GRToolProgressionManager.ToolParts.FlashDamage1;
		this.partMapping["FlashDamage2"] = GRToolProgressionManager.ToolParts.FlashDamage2;
		this.partMapping["FlashDamage3"] = GRToolProgressionManager.ToolParts.FlashDamage3;
	}

	// Token: 0x06003496 RID: 13462 RVA: 0x001214A4 File Offset: 0x0011F6A4
	private void InitializeCollectorPartMapping()
	{
		this.partMapping["Collector"] = GRToolProgressionManager.ToolParts.Collector;
		this.partMapping["CollectorBonus1"] = GRToolProgressionManager.ToolParts.CollectorBonus1;
		this.partMapping["CollectorBonus2"] = GRToolProgressionManager.ToolParts.CollectorBonus2;
		this.partMapping["CollectorBonus3"] = GRToolProgressionManager.ToolParts.CollectorBonus3;
	}

	// Token: 0x06003497 RID: 13463 RVA: 0x001214F9 File Offset: 0x0011F6F9
	private void InitializeRevivePartMapping()
	{
		this.partMapping["Revive"] = GRToolProgressionManager.ToolParts.Revive;
	}

	// Token: 0x06003498 RID: 13464 RVA: 0x00121510 File Offset: 0x0011F710
	private void InitializeLanternPartMapping()
	{
		this.partMapping["Lantern"] = GRToolProgressionManager.ToolParts.Lantern;
		this.partMapping["LanternIntensity1"] = GRToolProgressionManager.ToolParts.LanternIntensity1;
		this.partMapping["LanternIntensity2"] = GRToolProgressionManager.ToolParts.LanternIntensity2;
		this.partMapping["LanternIntensity3"] = GRToolProgressionManager.ToolParts.LanternIntensity3;
	}

	// Token: 0x06003499 RID: 13465 RVA: 0x00121568 File Offset: 0x0011F768
	private void InitializeShieldGunPartMapping()
	{
		this.partMapping["ShieldGun"] = GRToolProgressionManager.ToolParts.ShieldGun;
		this.partMapping["ShieldGunStrength1"] = GRToolProgressionManager.ToolParts.ShieldGunStrength1;
		this.partMapping["ShieldGunStrength2"] = GRToolProgressionManager.ToolParts.ShieldGunStrength2;
		this.partMapping["ShieldGunStrength3"] = GRToolProgressionManager.ToolParts.ShieldGunStrength3;
	}

	// Token: 0x0600349A RID: 13466 RVA: 0x001215C0 File Offset: 0x0011F7C0
	private void InitializeDirectionalShieldPartMapping()
	{
		this.partMapping["DirectionalShield"] = GRToolProgressionManager.ToolParts.DirectionalShield;
		this.partMapping["DirectionalShieldSize1"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize1;
		this.partMapping["DirectionalShieldSize2"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize2;
		this.partMapping["DirectionalShieldSize3"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize3;
	}

	// Token: 0x0600349B RID: 13467 RVA: 0x00121618 File Offset: 0x0011F818
	private void InitializeEnergyEfficiencyPartMapping()
	{
		this.partMapping["EnergyEfficiency"] = GRToolProgressionManager.ToolParts.EnergyEff;
		this.partMapping["EnergyEff1"] = GRToolProgressionManager.ToolParts.EnergyEff1;
		this.partMapping["EnergyEff2"] = GRToolProgressionManager.ToolParts.EnergyEff2;
		this.partMapping["EnergyEff3"] = GRToolProgressionManager.ToolParts.EnergyEff3;
	}

	// Token: 0x0600349C RID: 13468 RVA: 0x0012166D File Offset: 0x0011F86D
	private void InitializeDockWristPartMapping()
	{
		this.partMapping["DockWrist"] = GRToolProgressionManager.ToolParts.DockWrist;
		this.partMapping["StatusWatch"] = GRToolProgressionManager.ToolParts.StatusWatch;
		this.partMapping["RattyBackpack"] = GRToolProgressionManager.ToolParts.RattyBackpack;
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x001216A8 File Offset: 0x0011F8A8
	private void InitializeDropPodPartMapping()
	{
		this.partMapping["DropPodBasic"] = GRToolProgressionManager.ToolParts.DropPodBasic;
		this.partMapping["DropPodChassis01"] = GRToolProgressionManager.ToolParts.DropPodChassis1;
		this.partMapping["DropPodChassis02"] = GRToolProgressionManager.ToolParts.DropPodChassis2;
		this.partMapping["DropPodChassis03"] = GRToolProgressionManager.ToolParts.DropPodChassis3;
	}

	// Token: 0x0600349E RID: 13470 RVA: 0x00121700 File Offset: 0x0011F900
	private void AddFakeNodes()
	{
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Club))
		{
			this.toolTree[GRTool.GRToolType.Club] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Baton",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Baton,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Baton),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Baton))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Baton] = this.toolTree[GRTool.GRToolType.Club];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.EnergyEfficiency))
		{
			this.toolTree[GRTool.GRToolType.EnergyEfficiency] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "EnergyEfficiency",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.EnergyEff,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.EnergyEff),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.EnergyEff))
		{
			this.partTree[GRToolProgressionManager.ToolParts.EnergyEff] = this.toolTree[GRTool.GRToolType.EnergyEfficiency];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Collector))
		{
			this.toolTree[GRTool.GRToolType.Collector] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Collector",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Collector,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Collector),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Collector))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Collector] = this.toolTree[GRTool.GRToolType.Collector];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Lantern))
		{
			this.toolTree[GRTool.GRToolType.Lantern] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Lantern",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Lantern,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Lantern),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Lantern))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Lantern] = this.toolTree[GRTool.GRToolType.Lantern];
		}
	}

	// Token: 0x0600349F RID: 13471 RVA: 0x00121930 File Offset: 0x0011FB30
	private void ProcessNodes()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			GRToolProgressionTree.GRToolProgressionRawNode value = keyValuePair.Value;
			foreach (string key in value.requiredByIds)
			{
				if (this.nodeTree.ContainsKey(key))
				{
					this.nodeTree[key].progressionNode.children.Add(value.progressionNode);
					value.progressionNode.parents.Add(this.nodeTree[key].progressionNode);
				}
			}
			value.progressionNode.requiredEmployeeLevel = this.GetEmployeeLevel(value.requiredEntitlements);
			string key2 = value.progressionNode.name.Trim();
			if (this.toolMapping.ContainsKey(key2))
			{
				GRTool.GRToolType key3 = this.toolMapping[key2];
				value.progressionNode.rootNode = true;
				if (!value.progressionNode.unlocked && this.autoUnlockNodeId == string.Empty && value.progressionNode.researchCost == 0 && value.progressionNode.requiredEmployeeLevel == GRToolProgressionTree.EmployeeLevelRequirement.None)
				{
					this.autoUnlockNodeId = value.progressionNode.id;
				}
				this.toolTree[key3] = value.progressionNode;
			}
			this.partTree[value.progressionNode.type] = value.progressionNode;
		}
	}

	// Token: 0x060034A0 RID: 13472 RVA: 0x00121AFC File Offset: 0x0011FCFC
	private void PopulateMetadata()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			keyValuePair.Value.progressionNode.partMetadata = this.manager.GetPartMetadata(keyValuePair.Value.progressionNode.type);
		}
	}

	// Token: 0x060034A1 RID: 13473 RVA: 0x00121B78 File Offset: 0x0011FD78
	private GRToolProgressionTree.EmployeeLevelRequirement GetEmployeeLevel(List<string> rawRequiredEntitlements)
	{
		foreach (string text in rawRequiredEntitlements)
		{
			string a = text.Trim();
			if (a == "Intern")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.Intern;
			}
			if (a == "PartTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
			}
			if (a == "FullTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
			}
		}
		return GRToolProgressionTree.EmployeeLevelRequirement.None;
	}

	// Token: 0x060034A2 RID: 13474 RVA: 0x00121BFC File Offset: 0x0011FDFC
	private void ProcessTreeNode(UserHydratedNodeDefinition treeNode)
	{
		GRToolProgressionTree.GRToolProgressionRawNode grtoolProgressionRawNode = new GRToolProgressionTree.GRToolProgressionRawNode();
		grtoolProgressionRawNode.progressionNode.id = treeNode.id;
		grtoolProgressionRawNode.progressionNode.name = treeNode.name;
		grtoolProgressionRawNode.progressionNode.unlocked = treeNode.unlocked;
		if (this.partMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
		{
			if (this.toolMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
			{
				grtoolProgressionRawNode.progressionNode.rootNode = true;
			}
			grtoolProgressionRawNode.progressionNode.type = this.partMapping[grtoolProgressionRawNode.progressionNode.name];
		}
		if (treeNode.cost != null && treeNode.cost.items != null)
		{
			foreach (KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair in treeNode.cost.items)
			{
				if (keyValuePair.Key.Trim() == this.researchPointsEntitlement)
				{
					grtoolProgressionRawNode.progressionNode.researchCost = keyValuePair.Value.Delta;
				}
			}
		}
		foreach (MothershipEntitlementCatalogItem mothershipEntitlementCatalogItem in treeNode.prerequisite_entitlements)
		{
			grtoolProgressionRawNode.requiredEntitlements.Add(mothershipEntitlementCatalogItem.name);
		}
		foreach (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t variant in treeNode.prerequisite_nodes.nodes)
		{
			ComplexPrerequisiteNodes value = new ComplexPrerequisiteNodes();
			NodeReference nodeReference = new NodeReference();
			if (!MothershipApi.TryGetComplexPrerequisiteNodeFromVariant(variant, value) && MothershipApi.TryGetNodeReferenceFromVariant(variant, nodeReference))
			{
				grtoolProgressionRawNode.requiredByIds.Add(nodeReference.node_id);
			}
		}
		if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.None && this.pendingPartUnlock == grtoolProgressionRawNode.progressionNode.type)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodBasic || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis1 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis2 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis3)
			{
				if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.DropPodBasic)
				{
					grplayer.SendPodUpgradeTelemetry(grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, 0, grtoolProgressionRawNode.progressionNode.researchCost);
				}
			}
			else
			{
				grplayer.SendToolUpgradeTelemetry("Research", grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, grtoolProgressionRawNode.progressionNode.researchCost, 0, 0);
			}
			this.pendingPartUnlock = GRToolProgressionManager.ToolParts.None;
		}
		this.nodeTree[grtoolProgressionRawNode.progressionNode.id] = grtoolProgressionRawNode;
	}

	// Token: 0x060034A3 RID: 13475 RVA: 0x00121EB8 File Offset: 0x001200B8
	private void ProcessToolProgressionTree(UserHydratedProgressionTreeResponse tree)
	{
		if (tree.Tree.name != this.treeName)
		{
			return;
		}
		this.toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();
		this.nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();
		this.treeId = tree.Tree.id;
		foreach (UserHydratedNodeDefinition treeNode in tree.Nodes)
		{
			this.ProcessTreeNode(treeNode);
		}
		this.PopulateMetadata();
		this.ProcessNodes();
		this.AddFakeNodes();
		if (this.autoUnlockNodeId != string.Empty)
		{
			string nodeId = this.autoUnlockNodeId;
			this.autoUnlockNodeId = string.Empty;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, nodeId, this.reactor);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x060034A4 RID: 13476 RVA: 0x00121FA4 File Offset: 0x001201A4
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			this.pendingPartUnlock = part;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, this.partTree[part].id, this.reactor);
		}
	}

	// Token: 0x04004489 RID: 17545
	private string treeName = "GRTools";

	// Token: 0x0400448A RID: 17546
	private string treeId = string.Empty;

	// Token: 0x0400448B RID: 17547
	private string researchPointsEntitlement = "GR_ResearchPoints";

	// Token: 0x0400448C RID: 17548
	private Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode> toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x0400448D RID: 17549
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode> partTree = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x0400448E RID: 17550
	private Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode> nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();

	// Token: 0x0400448F RID: 17551
	private Dictionary<string, GRTool.GRToolType> toolMapping = new Dictionary<string, GRTool.GRToolType>();

	// Token: 0x04004490 RID: 17552
	private Dictionary<string, GRToolProgressionManager.ToolParts> partMapping = new Dictionary<string, GRToolProgressionManager.ToolParts>();

	// Token: 0x04004491 RID: 17553
	private string autoUnlockNodeId = string.Empty;

	// Token: 0x04004492 RID: 17554
	private int currentResearchPoints;

	// Token: 0x04004493 RID: 17555
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x04004494 RID: 17556
	[NonSerialized]
	private GRToolProgressionManager manager;

	// Token: 0x04004495 RID: 17557
	[NonSerialized]
	private GRToolProgressionTree.EmployeeLevelRequirement currentEmploymentLevel;

	// Token: 0x04004496 RID: 17558
	private string internEntitlement = "Intern";

	// Token: 0x04004497 RID: 17559
	private string partTimeEntitlement = "PartTime";

	// Token: 0x04004498 RID: 17560
	private string fullTimeEntitlement = "FullTime";

	// Token: 0x04004499 RID: 17561
	private GRToolProgressionManager.ToolParts pendingPartUnlock;

	// Token: 0x02000803 RID: 2051
	public enum EmployeeLevelRequirement
	{
		// Token: 0x0400449B RID: 17563
		None,
		// Token: 0x0400449C RID: 17564
		Intern,
		// Token: 0x0400449D RID: 17565
		PartTime,
		// Token: 0x0400449E RID: 17566
		FullTime
	}

	// Token: 0x02000804 RID: 2052
	public class GRToolProgressionNode
	{
		// Token: 0x0400449F RID: 17567
		public string id;

		// Token: 0x040044A0 RID: 17568
		public string name;

		// Token: 0x040044A1 RID: 17569
		public bool unlocked;

		// Token: 0x040044A2 RID: 17570
		public int researchCost;

		// Token: 0x040044A3 RID: 17571
		public bool rootNode;

		// Token: 0x040044A4 RID: 17572
		public GRToolProgressionManager.ToolParts type;

		// Token: 0x040044A5 RID: 17573
		public GRToolProgressionManager.ToolProgressionMetaData partMetadata;

		// Token: 0x040044A6 RID: 17574
		public List<GRToolProgressionTree.GRToolProgressionNode> children = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x040044A7 RID: 17575
		public List<GRToolProgressionTree.GRToolProgressionNode> parents = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x040044A8 RID: 17576
		public GRToolProgressionTree.EmployeeLevelRequirement requiredEmployeeLevel;
	}

	// Token: 0x02000805 RID: 2053
	private class GRToolProgressionRawNode
	{
		// Token: 0x040044A9 RID: 17577
		public GRToolProgressionTree.GRToolProgressionNode progressionNode = new GRToolProgressionTree.GRToolProgressionNode();

		// Token: 0x040044AA RID: 17578
		public List<string> requiredByIds = new List<string>();

		// Token: 0x040044AB RID: 17579
		public List<string> requiredEntitlements = new List<string>();
	}
}
