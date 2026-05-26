using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007FE RID: 2046
public class GRToolProgressionManager : MonoBehaviourTick
{
	// Token: 0x1400005B RID: 91
	// (add) Token: 0x0600345A RID: 13402 RVA: 0x0011FFC8 File Offset: 0x0011E1C8
	// (remove) Token: 0x0600345B RID: 13403 RVA: 0x00120000 File Offset: 0x0011E200
	public event Action OnProgressionUpdated;

	// Token: 0x0600345C RID: 13404 RVA: 0x00120035 File Offset: 0x0011E235
	public void SetPendingTreeToProcess()
	{
		this.pendingTreeToProcess = true;
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x0012003E File Offset: 0x0011E23E
	public void UpdateInventory()
	{
		this.pendingUpdateInventory = true;
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x00120048 File Offset: 0x0011E248
	public void Init(GhostReactor ghostReactor)
	{
		this.reactor = ghostReactor;
		this.PopulateToolPartMetadata();
		this.PopulateEmployeeLevelMetadata();
		if (this.researchStations != null)
		{
			foreach (GRResearchStation grresearchStation in this.researchStations)
			{
				grresearchStation.Init(this, ghostReactor);
			}
		}
		if (this.toolUpgradeStations != null)
		{
			foreach (GRToolUpgradeStation grtoolUpgradeStation in this.toolUpgradeStations)
			{
				grtoolUpgradeStation.Init(this, ghostReactor);
			}
		}
		this.toolProgressionTree.Init(this.reactor, this);
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x00120110 File Offset: 0x0011E310
	public override void Tick()
	{
		if (this.sendUpdate)
		{
			Action onProgressionUpdated = this.OnProgressionUpdated;
			if (onProgressionUpdated != null)
			{
				onProgressionUpdated();
			}
			this.sendUpdate = false;
		}
		if (this.pendingTreeToProcess)
		{
			this.toolProgressionTree.RefreshProgressionTree();
			this.pendingTreeToProcess = false;
		}
		if (this.pendingUpdateInventory)
		{
			this.toolProgressionTree.RefreshUserInventory();
			this.pendingUpdateInventory = false;
		}
	}

	// Token: 0x06003460 RID: 13408 RVA: 0x00120171 File Offset: 0x0011E371
	public void SendMothershipUpdated()
	{
		this.sendUpdate = true;
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x0012017C File Offset: 0x0011E37C
	public GRToolProgressionManager.ToolProgressionMetaData GetPartMetadata(GRToolProgressionManager.ToolParts part)
	{
		GRToolProgressionManager.ToolProgressionMetaData result;
		this.partMetadata.TryGetValue(part, out result);
		return result;
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x0012019C File Offset: 0x0011E39C
	private void PopulateToolPartMetadata()
	{
		this.PopulateClubPartMetadata();
		this.PopulateFlashPartMetadata();
		this.PopulateCollectorPartMetadata();
		this.PopulateLanternPartMetadata();
		this.PopulateShieldGunPartMetadata();
		this.PopulateDirectionalShieldPartMetadata();
		this.PopulateEnergyEfficiencyPartMetadata();
		this.PopulateRevivePartMetadata();
		this.PopulateDockWristPartMetadata();
		this.PopulateDropPodPartMetadata();
		this.PopulateHocketStickMetadata();
	}

	// Token: 0x06003463 RID: 13411 RVA: 0x001201EC File Offset: 0x0011E3EC
	private void PopulateEmployeeLevelMetadata()
	{
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.None] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "None",
			level = 0
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.Intern] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Intern",
			level = 2
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.PartTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Part Time",
			level = 3
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.FullTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Full Time",
			level = 4
		};
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x001202A0 File Offset: 0x0011E4A0
	private void PopulateClubPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Baton] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Charge Baton",
			description = "50,000 volts of ghost-zapping power",
			annotation = "Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Lead Core",
			description = "Conductive lead sheath",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Osmium Core",
			description = "More mass for more win",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Electrified Spikes",
			description = "Impales, shocks, and crushes simultaneously",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶❶"
		};
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x001203A0 File Offset: 0x0011E5A0
	private void PopulateFlashPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Flash] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Spectral Flash",
			description = "Makes strong ghosts vulnerable",
			annotation = "Damages ghost armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Spectral Lens",
			description = "Safety through momentary paralysis",
			annotation = "Attaches to Spectral Flash. Stuns enemies."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Parabolic Focuser",
			description = "When you want ghosts to feel it",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Disintegrates armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Beta Wave Amplifier",
			description = "Exposure with explosive results",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Shatters armor."
		};
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x001204A0 File Offset: 0x0011E6A0
	private void PopulateCollectorPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Collector] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Collector",
			description = "Every team needs a sucker",
			annotation = "Collects essence and recharges tools"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Vortex Intake",
			description = "Harvests ambient essence",
			annotation = "Attaches to Collector.  Recharges over time."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Cyclone Intake",
			description = "Creates a wormhole to a twin universe",
			annotation = "Attaches to Collector. 2x collection bonus."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Hurricane Intake",
			description = "A Category 5 commitment to teamwork",
			annotation = "Attaches to Collector. 2x collection bonus.  Area recharge."
		};
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x001205A4 File Offset: 0x0011E7A4
	private void PopulateLanternPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Lantern] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Lantern",
			description = "Creates the gentle glow of safety",
			annotation = "Illuminates dark areas."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Kinetic Power",
			description = "Saves batteries to optimize shareholder value",
			annotation = "Attaches to Lantern. Doesn't need recharge."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Flare Discharge",
			description = "Blaze the trail for your team",
			annotation = "Attaches to Lantern. Drops long-lasting flares."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Gamma Burster",
			description = "See through walls. Do not aim at important body parts",
			annotation = "Attaches to Lantern. X-ray ghost vision."
		};
	}

	// Token: 0x06003468 RID: 13416 RVA: 0x001206A8 File Offset: 0x0011E8A8
	private void PopulateShieldGunPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGun] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Forcefield Gun",
			description = "Corporate armor for fragile assets",
			annotation = "Gives forcefields."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Truebright Nozzle",
			description = "Nuclear protection",
			annotation = "Attaches to Forcefield Gun. Increases light."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Stealth Nozzle",
			description = "Protection they'll never see coming",
			annotation = "Attaches to Forcefield Gun. Gives temporary stealth."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Medic Nozzle",
			description = "Restores productivity through impact therapy",
			annotation = "Attaches to Forcefield Gun. Heals to full."
		};
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x001207AC File Offset: 0x0011E9AC
	private void PopulateDirectionalShieldPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShield] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Umbrella Shield",
			description = "Protects company property",
			annotation = "Blocks attacks."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Sling Shield",
			description = "Deflects danger and liability",
			annotation = "Attaches to Umbrella Shield. Reflects projectiles."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Harmshadow",
			description = "The best defense is a good offense",
			annotation = "Attaches to Umbrella Shield. Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Total Defense Array",
			description = "The only safety device with a kill count",
			annotation = "Attaches to Shield. Reflects projectiles. Impact power: ❶❶"
		};
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x001208B0 File Offset: 0x0011EAB0
	private void PopulateEnergyEfficiencyPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Flash",
			description = "Lead Core Does things!"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Regulator",
			description = "Do more with less",
			annotation = "Attaches to most tools. Efficiency: +❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Optimizer",
			description = "Half the juice, double the morale",
			annotation = "Attaches to most tools. Efficiency: +❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Peak Power",
			description = "Efficiency that borders on spiritual enlightenment",
			annotation = "Attaches to most tools. Efficiency: +❶❶❶"
		};
	}

	// Token: 0x0600346B RID: 13419 RVA: 0x001209A7 File Offset: 0x0011EBA7
	private void PopulateRevivePartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Revive] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Revive",
			description = "Turns fatal injuries into teachable moments",
			annotation = "Brings defeated employees back to life."
		};
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x001209E4 File Offset: 0x0011EBE4
	private void PopulateDockWristPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DockWrist] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 500,
			name = "Wrist Dock",
			description = "Wearable storage that maximizes output per limb",
			annotation = "Extra storage slot"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.StatusWatch] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Ecto Watch",
			description = "Keep track of your location and statistics",
			annotation = "Compass and stat tracker"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.RattyBackpack] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 300,
			name = "Ratty Backpack",
			description = "Torn up backpack we found laying around. Can store one item.",
			annotation = "Worn on the back. It's a backpack."
		};
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x00120AAC File Offset: 0x0011ECAC
	private void PopulateDropPodPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodBasic] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Starter Pod",
			description = "Descend with confidence in a personal drop pod!\nSupports drops to 5000m\nUpgradable for deeper drops",
			annotation = "DropPodBasic"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Reinforced Pod Chassis",
			description = "Upgrade your drop pod to support drops to 10000m",
			annotation = "DropPodChassis1"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Iron Pod Chassis",
			description = "Upgrade your drop pod to support drops to 15000m",
			annotation = "DropPodChassis2"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Steel Pod Chassis",
			description = "Upgrade your drop pod to support drops to 20000m",
			annotation = "DropPodChassis3"
		};
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x00120BAE File Offset: 0x0011EDAE
	private void PopulateHocketStickMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.HockeyStick] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 10,
			name = "Hockey Stick",
			description = "A Used Hockey Stick",
			annotation = "Hit things with it?"
		};
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x00120BEB File Offset: 0x0011EDEB
	public int GetRequiredEmployeeLevel(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].level;
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x00120BFE File Offset: 0x0011EDFE
	public string GetEmployeeLevelDisplayName(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].name;
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x00120C11 File Offset: 0x0011EE11
	public int GetNumberOfResearchPoints()
	{
		return this.toolProgressionTree.GetNumberOfResearchPoints();
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x00120C1E File Offset: 0x0011EE1E
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		return this.toolProgressionTree.GetSupportedTools();
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x00120C2B File Offset: 0x0011EE2B
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		return this.toolProgressionTree.GetToolUpgrades(tool);
	}

	// Token: 0x06003474 RID: 13428 RVA: 0x00120C3C File Offset: 0x0011EE3C
	public int GetRecycleShiftCredit(GRTool.GRToolType tool)
	{
		if (tool == GRTool.GRToolType.HockeyStick)
		{
			return (int)(10f / (float)this.reactor.vrRigs.Count);
		}
		GRToolProgressionTree.GRToolProgressionNode toolNode = this.toolProgressionTree.GetToolNode(tool);
		if (toolNode != null)
		{
			return (int)((float)(toolNode.partMetadata.shiftCreditCost / 2) / (float)this.reactor.vrRigs.Count);
		}
		return 0;
	}

	// Token: 0x06003475 RID: 13429 RVA: 0x00120C9A File Offset: 0x0011EE9A
	public bool GetShiftCreditCost(GRToolProgressionManager.ToolParts part, out int shiftCreditCost)
	{
		shiftCreditCost = 0;
		if (this.partMetadata.ContainsKey(part))
		{
			shiftCreditCost += this.partMetadata[part].shiftCreditCost;
			return true;
		}
		return false;
	}

	// Token: 0x06003476 RID: 13430 RVA: 0x00120CC8 File Offset: 0x0011EEC8
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		bool flag;
		if (!this.IsPartUnlocked(part, out flag))
		{
			return;
		}
		if (!flag)
		{
			int numberOfResearchPoints = this.GetNumberOfResearchPoints();
			int num;
			if (!this.GetPartUnlockJuiceCost(part, out num))
			{
				return;
			}
			if (numberOfResearchPoints < num)
			{
				return;
			}
			GRToolProgressionTree.EmployeeLevelRequirement employeeLevel;
			if (!this.GetPartUnlockEmployeeRequiredLevel(part, out employeeLevel))
			{
				return;
			}
			int requiredEmployeeLevel = this.GetRequiredEmployeeLevel(this.GetCurrentEmployeeLevel());
			int requiredEmployeeLevel2 = this.GetRequiredEmployeeLevel(employeeLevel);
			if (requiredEmployeeLevel < requiredEmployeeLevel2)
			{
				return;
			}
			this.toolProgressionTree.AttemptToUnlockPart(part);
		}
	}

	// Token: 0x06003477 RID: 13431 RVA: 0x00120D30 File Offset: 0x0011EF30
	public bool IsPartUnlocked(GRToolProgressionManager.ToolParts part, out bool unlocked)
	{
		unlocked = false;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		unlocked = partNode.unlocked;
		return true;
	}

	// Token: 0x06003478 RID: 13432 RVA: 0x00120D5C File Offset: 0x0011EF5C
	public bool GetPartUnlockEmployeeRequiredLevel(GRToolProgressionManager.ToolParts part, out GRToolProgressionTree.EmployeeLevelRequirement level)
	{
		level = GRToolProgressionTree.EmployeeLevelRequirement.None;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		level = partNode.requiredEmployeeLevel;
		return true;
	}

	// Token: 0x06003479 RID: 13433 RVA: 0x00120D88 File Offset: 0x0011EF88
	public bool GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts part, out int juiceCost)
	{
		juiceCost = 0;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		juiceCost = partNode.researchCost;
		return true;
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x00120DB4 File Offset: 0x0011EFB4
	public bool GetPartUnlockRequiredParentParts(GRToolProgressionManager.ToolParts part, out List<GRToolProgressionManager.ToolParts> requiredParts)
	{
		requiredParts = new List<GRToolProgressionManager.ToolParts>();
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in partNode.parents)
		{
			requiredParts.Add(grtoolProgressionNode.type);
		}
		return true;
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x00120E28 File Offset: 0x0011F028
	public bool GetPlayerShiftCredit(out int playerShiftCredit)
	{
		playerShiftCredit = 0;
		if (VRRig.LocalRig != null)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (grplayer != null)
			{
				playerShiftCredit = grplayer.ShiftCredits;
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x00120E64 File Offset: 0x0011F064
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmployeeLevel()
	{
		return this.toolProgressionTree.GetCurrentEmploymentLevel();
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x00120E71 File Offset: 0x0011F071
	public string GetTreeId()
	{
		return this.toolProgressionTree.GetTreeId();
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x00120E80 File Offset: 0x0011F080
	public int GetDropPodLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x00120EA0 File Offset: 0x0011F0A0
	public int GetDropPodChasisLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return 3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return 2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x00120EE0 File Offset: 0x0011F0E0
	public ProgressionManager.DrillUpgradeLevel GetDrillLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade1;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Base;
		}
		return ProgressionManager.DrillUpgradeLevel.None;
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x00120F30 File Offset: 0x0011F130
	public int GetJuiceCostForDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgradeLevel)
	{
		int result = 0;
		switch (upgradeLevel)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodBasic, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis1, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis2, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis3, out result);
			break;
		}
		return result;
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x00120F8C File Offset: 0x0011F18C
	public int GetSRCostForDrillUpgradeLevel(ProgressionManager.DrillUpgradeLevel level)
	{
		switch (level)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			return 3600;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			return 0;
		default:
			return 0;
		}
	}

	// Token: 0x04004452 RID: 17490
	[NonSerialized]
	private Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata> employeeLevelMetadata = new Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata>();

	// Token: 0x04004453 RID: 17491
	[NonSerialized]
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData> partMetadata = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData>();

	// Token: 0x04004454 RID: 17492
	[NonSerialized]
	private GRToolProgressionTree toolProgressionTree = new GRToolProgressionTree();

	// Token: 0x04004455 RID: 17493
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x04004456 RID: 17494
	[SerializeField]
	private List<GRResearchStation> researchStations;

	// Token: 0x04004457 RID: 17495
	[SerializeField]
	private List<GRToolUpgradeStation> toolUpgradeStations;

	// Token: 0x04004458 RID: 17496
	[NonSerialized]
	private bool pendingTreeToProcess;

	// Token: 0x04004459 RID: 17497
	[NonSerialized]
	private bool pendingUpdateInventory;

	// Token: 0x0400445B RID: 17499
	private bool sendUpdate;

	// Token: 0x020007FF RID: 2047
	public class ToolProgressionMetaData
	{
		// Token: 0x0400445C RID: 17500
		public string name;

		// Token: 0x0400445D RID: 17501
		public string description;

		// Token: 0x0400445E RID: 17502
		public string annotation;

		// Token: 0x0400445F RID: 17503
		public int shiftCreditCost;
	}

	// Token: 0x02000800 RID: 2048
	public struct EmployeeMetadata
	{
		// Token: 0x04004460 RID: 17504
		public string name;

		// Token: 0x04004461 RID: 17505
		public int level;
	}

	// Token: 0x02000801 RID: 2049
	public enum ToolParts
	{
		// Token: 0x04004463 RID: 17507
		None,
		// Token: 0x04004464 RID: 17508
		Baton,
		// Token: 0x04004465 RID: 17509
		BatonDamage1,
		// Token: 0x04004466 RID: 17510
		BatonDamage2,
		// Token: 0x04004467 RID: 17511
		BatonDamage3,
		// Token: 0x04004468 RID: 17512
		Flash,
		// Token: 0x04004469 RID: 17513
		FlashDamage1,
		// Token: 0x0400446A RID: 17514
		FlashDamage2,
		// Token: 0x0400446B RID: 17515
		FlashDamage3,
		// Token: 0x0400446C RID: 17516
		Collector,
		// Token: 0x0400446D RID: 17517
		CollectorBonus1,
		// Token: 0x0400446E RID: 17518
		CollectorBonus2,
		// Token: 0x0400446F RID: 17519
		CollectorBonus3,
		// Token: 0x04004470 RID: 17520
		Lantern,
		// Token: 0x04004471 RID: 17521
		LanternIntensity1,
		// Token: 0x04004472 RID: 17522
		LanternIntensity2,
		// Token: 0x04004473 RID: 17523
		LanternIntensity3,
		// Token: 0x04004474 RID: 17524
		ShieldGun,
		// Token: 0x04004475 RID: 17525
		ShieldGunStrength1,
		// Token: 0x04004476 RID: 17526
		ShieldGunStrength2,
		// Token: 0x04004477 RID: 17527
		ShieldGunStrength3,
		// Token: 0x04004478 RID: 17528
		DirectionalShield,
		// Token: 0x04004479 RID: 17529
		DirectionalShieldSize1,
		// Token: 0x0400447A RID: 17530
		DirectionalShieldSize2,
		// Token: 0x0400447B RID: 17531
		DirectionalShieldSize3,
		// Token: 0x0400447C RID: 17532
		EnergyEff,
		// Token: 0x0400447D RID: 17533
		EnergyEff1,
		// Token: 0x0400447E RID: 17534
		EnergyEff2,
		// Token: 0x0400447F RID: 17535
		EnergyEff3,
		// Token: 0x04004480 RID: 17536
		DockWrist,
		// Token: 0x04004481 RID: 17537
		Revive,
		// Token: 0x04004482 RID: 17538
		DropPodBasic,
		// Token: 0x04004483 RID: 17539
		DropPodChassis1,
		// Token: 0x04004484 RID: 17540
		DropPodChassis2,
		// Token: 0x04004485 RID: 17541
		DropPodChassis3,
		// Token: 0x04004486 RID: 17542
		StatusWatch,
		// Token: 0x04004487 RID: 17543
		RattyBackpack,
		// Token: 0x04004488 RID: 17544
		HockeyStick
	}
}
