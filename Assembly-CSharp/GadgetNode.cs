using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

// Token: 0x02000125 RID: 293
public class GadgetNode : TechTreeNodeBase
{
	// Token: 0x17000079 RID: 121
	// (get) Token: 0x0600073F RID: 1855 RVA: 0x00029027 File Offset: 0x00027227
	private static bool InEditor
	{
		get
		{
			return NodeInspectorBridge.InNodeEditor;
		}
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x06000740 RID: 1856 RVA: 0x00029030 File Offset: 0x00027230
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier eassetReleaseTier = this.releaseTier;
			return eassetReleaseTier != EAssetReleaseTier.Disabled && eassetReleaseTier <= EAssetReleaseTier.PublicRC;
		}
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000741 RID: 1857 RVA: 0x00029050 File Offset: 0x00027250
	public bool IsDispensableGadget
	{
		get
		{
			return this.unlockedGadgetPrefab;
		}
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x06000742 RID: 1858 RVA: 0x0002905D File Offset: 0x0002725D
	private bool ShowGadgetPrefab
	{
		get
		{
			return !GadgetNode.InEditor || this.IsDispensableGadget;
		}
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000743 RID: 1859 RVA: 0x0002906E File Offset: 0x0002726E
	private bool ShowExcludedGameModes
	{
		get
		{
			return !GadgetNode.InEditor || this.excludedGameModes > (ESuperGameModes)0;
		}
	}

	// Token: 0x1700007E RID: 126
	// (get) Token: 0x06000744 RID: 1860 RVA: 0x00029082 File Offset: 0x00027282
	private bool ShowReleaseTier
	{
		get
		{
			return !GadgetNode.InEditor || this.releaseTier != EAssetReleaseTier.PublicRC;
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0002909C File Offset: 0x0002729C
	public void ConfigureFrom(SITechTreeNode sourceNode)
	{
		this.releaseTier = sourceNode.EdReleaseTier;
		this.upgradeType = sourceNode.upgradeType;
		this.nickName = sourceNode.nickName;
		this.description = sourceNode.description;
		this.unlockedGadgetPrefab = sourceNode.unlockedGadgetPrefab;
		this.excludedGameModes = sourceNode.excludedGameModes;
		this.nodeCost = sourceNode.nodeCost.ToArray<SIResource.ResourceCost>();
		this.costOverride = sourceNode.costOverride;
		base.name = this.nickName;
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x0002911C File Offset: 0x0002731C
	public void AssignParentUpgrades(SIUpgradeType[] prerequisites)
	{
		NodePort port = base.GetPort("input");
		port.ClearConnections();
		for (int i = 0; i < prerequisites.Length; i++)
		{
			SIUpgradeType id = prerequisites[i];
			GadgetNode gadgetNode = this.graph.nodes.FirstOrDefault(delegate(Node n)
			{
				GadgetNode gadgetNode2 = n as GadgetNode;
				return gadgetNode2 != null && gadgetNode2.upgradeType == id;
			}) as GadgetNode;
			if (gadgetNode != null)
			{
				NodePort port2 = gadgetNode.GetPort("output");
				port.Connect(port2);
			}
		}
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x000291A0 File Offset: 0x000273A0
	public List<SIUpgradeType> GetParentUpgradeTypes()
	{
		List<SIUpgradeType> list = new List<SIUpgradeType>();
		foreach (Node node in from n in base.GetPort("input").GetConnections()
		select n.node)
		{
			GadgetNode gadgetNode = node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode.upgradeType);
			}
		}
		return list;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00029230 File Offset: 0x00027430
	public SITechTreeNode GenerateTechTreeNode()
	{
		return new SITechTreeNode
		{
			upgradeType = this.upgradeType,
			nickName = this.nickName,
			description = this.description,
			unlockedGadgetPrefab = this.unlockedGadgetPrefab,
			nodeCost = this.nodeCost.ToArray<SIResource.ResourceCost>(),
			excludedGameModes = this.excludedGameModes,
			EdReleaseTier = this.releaseTier,
			parentUpgrades = this.GetParentUpgradeTypes().ToArray()
		};
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x000292AC File Offset: 0x000274AC
	public int GetDepth()
	{
		int num = 0;
		NodePort inputPort = base.GetInputPort("input");
		IEnumerable<NodePort> enumerable = (inputPort != null) ? inputPort.GetConnections() : null;
		foreach (NodePort nodePort in (enumerable ?? Enumerable.Empty<NodePort>()))
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetDepth() + 1);
			}
		}
		return num;
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00029330 File Offset: 0x00027530
	public int GetTreeDepth()
	{
		int num = this.GetDepth();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				num = Mathf.Max(num, gadgetNode.GetTreeDepth());
			}
		}
		return num;
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x000293A8 File Offset: 0x000275A8
	public List<GadgetNode> GetTreeNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		this.GetTreeNodes(list);
		return list;
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x000293C4 File Offset: 0x000275C4
	public void GetTreeNodes(List<GadgetNode> nodes)
	{
		nodes.Add(this);
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				gadgetNode.GetTreeNodes(nodes);
			}
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00029438 File Offset: 0x00027638
	public List<GadgetNode> GetParentNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetPort("input").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x000294AC File Offset: 0x000276AC
	public List<GadgetNode> GetChildNodes()
	{
		List<GadgetNode> list = new List<GadgetNode>();
		foreach (NodePort nodePort in base.GetOutputPort("output").GetConnections())
		{
			GadgetNode gadgetNode = nodePort.node as GadgetNode;
			if (gadgetNode != null)
			{
				list.Add(gadgetNode);
			}
		}
		return list;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x00029520 File Offset: 0x00027720
	public int GetTreeWidth()
	{
		List<GadgetNode> childNodes = this.GetChildNodes();
		if (childNodes.Count == 0)
		{
			return 1;
		}
		int num = 0;
		foreach (GadgetNode gadgetNode in childNodes)
		{
			num += gadgetNode.GetTreeWidth();
		}
		return num;
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00029584 File Offset: 0x00027784
	public bool CostEquals(SIResource.ResourceCost[] cost)
	{
		if (cost.Length != this.nodeCost.Length)
		{
			return false;
		}
		for (int i = 0; i < cost.Length; i++)
		{
			if (!cost[i].Equals(this.nodeCost[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400097E RID: 2430
	[Node.InputAttribute(Node.ShowBackingValue.Unconnected, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty input;

	// Token: 0x0400097F RID: 2431
	[Node.OutputAttribute(Node.ShowBackingValue.Never, Node.ConnectionType.Multiple, Node.TypeConstraint.None, false)]
	public TechTreeNodeBase.Empty output;

	// Token: 0x04000980 RID: 2432
	public SIUpgradeType upgradeType;

	// Token: 0x04000981 RID: 2433
	public string nickName;

	// Token: 0x04000982 RID: 2434
	[TextArea]
	public string description;

	// Token: 0x04000983 RID: 2435
	public SIResource.ResourceCost[] nodeCost;

	// Token: 0x04000984 RID: 2436
	public bool costOverride;

	// Token: 0x04000985 RID: 2437
	[Header("Prefab")]
	public GameEntity unlockedGadgetPrefab;

	// Token: 0x04000986 RID: 2438
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000987 RID: 2439
	public EAssetReleaseTier releaseTier = (EAssetReleaseTier)(-1);
}
