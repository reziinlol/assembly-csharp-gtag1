using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using UnityEngine;

// Token: 0x02000166 RID: 358
[Serializable]
public class SITechTreePage
{
	// Token: 0x170000C4 RID: 196
	// (get) Token: 0x06000970 RID: 2416 RVA: 0x0003272F File Offset: 0x0003092F
	// (set) Token: 0x06000971 RID: 2417 RVA: 0x00032737 File Offset: 0x00030937
	public EAssetReleaseTier EdReleaseTier
	{
		get
		{
			return this.m_edReleaseTier;
		}
		set
		{
			this.m_edReleaseTier = value;
		}
	}

	// Token: 0x170000C5 RID: 197
	// (get) Token: 0x06000972 RID: 2418 RVA: 0x00032740 File Offset: 0x00030940
	public bool IsValid
	{
		get
		{
			EAssetReleaseTier edReleaseTier = this.m_edReleaseTier;
			if (edReleaseTier != EAssetReleaseTier.Disabled && edReleaseTier <= EAssetReleaseTier.PublicRC)
			{
				SITechTreeNode[] array = this.treeNodes;
				return array != null && array.Length != 0;
			}
			return false;
		}
	}

	// Token: 0x170000C6 RID: 198
	// (get) Token: 0x06000973 RID: 2419 RVA: 0x0003276D File Offset: 0x0003096D
	public bool IsAllowed
	{
		get
		{
			return (this.excludedGameModes & (ESuperGameModes)GameMode.CurrentGameModeFlag) == (ESuperGameModes)0;
		}
	}

	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x06000974 RID: 2420 RVA: 0x0003277E File Offset: 0x0003097E
	// (set) Token: 0x06000975 RID: 2421 RVA: 0x00032786 File Offset: 0x00030986
	public List<GraphNode<SITechTreeNode>> Roots { get; private set; }

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06000976 RID: 2422 RVA: 0x0003278F File Offset: 0x0003098F
	// (set) Token: 0x06000977 RID: 2423 RVA: 0x00032797 File Offset: 0x00030997
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x000327A0 File Offset: 0x000309A0
	// (set) Token: 0x06000979 RID: 2425 RVA: 0x000327A8 File Offset: 0x000309A8
	public List<SITechTreeNode> DispensableGadgets { get; private set; }

	// Token: 0x0600097A RID: 2426 RVA: 0x000327B1 File Offset: 0x000309B1
	public void ClearGraph()
	{
		this.Roots = null;
		this.AllNodes = null;
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x000327C4 File Offset: 0x000309C4
	public void BuildGraph()
	{
		SITechTreePage.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.Roots = new List<GraphNode<SITechTreeNode>>();
		this.AllNodes = new List<GraphNode<SITechTreeNode>>();
		this.DispensableGadgets = new List<SITechTreeNode>();
		if (!this.IsValid)
		{
			return;
		}
		CS$<>8__locals1.nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && (sitechTreeNode.parentUpgrades == null || sitechTreeNode.parentUpgrades.Length == 0))
			{
				this.Roots.Add(this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, this.excludedGameModes, ref CS$<>8__locals1));
			}
		}
		foreach (GraphNode<SITechTreeNode> graphNode in this.AllNodes)
		{
			if (graphNode.Value.IsDispensableGadget)
			{
				this.DispensableGadgets.Add(graphNode.Value);
			}
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x000328BC File Offset: 0x00030ABC
	public void PrintGraph()
	{
		foreach (GraphNode<SITechTreeNode> graphNode in this.Roots)
		{
			foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrderDistinct(null))
			{
				Debug.Log(string.Concat(new string[]
				{
					"[SI] Graph node: ",
					graphNode2.Value.nickName,
					" [",
					SITechTreePage.<PrintGraph>g__NodeListText|28_0(graphNode2.Parents),
					"]"
				}));
			}
		}
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x0003299C File Offset: 0x00030B9C
	[CompilerGenerated]
	private GraphNode<SITechTreeNode> <BuildGraph>g__PopulateGraph|27_0(SITechTreeNode node, ESuperGameModes parentExcludedGameModes, ref SITechTreePage.<>c__DisplayClass27_0 A_3)
	{
		node.excludedGameModes |= parentExcludedGameModes;
		GraphNode<SITechTreeNode> graphNode;
		if (!A_3.nodeLookup.TryGetValue(node.upgradeType, out graphNode))
		{
			graphNode = new GraphNode<SITechTreeNode>(node);
			A_3.nodeLookup.Add(node.upgradeType, graphNode);
			this.AllNodes.Add(graphNode);
		}
		SIUpgradeType upgradeType = node.upgradeType;
		foreach (SITechTreeNode sitechTreeNode in this.treeNodes)
		{
			if (sitechTreeNode.IsValid && sitechTreeNode.parentUpgrades != null)
			{
				SIUpgradeType[] parentUpgrades = sitechTreeNode.parentUpgrades;
				for (int j = 0; j < parentUpgrades.Length; j++)
				{
					if (parentUpgrades[j] == upgradeType)
					{
						GraphNode<SITechTreeNode> graphNode2 = this.<BuildGraph>g__PopulateGraph|27_0(sitechTreeNode, node.excludedGameModes, ref A_3);
						if (!graphNode.Children.Contains(graphNode2))
						{
							graphNode.AddChild(graphNode2);
						}
					}
				}
			}
		}
		return graphNode;
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00032A72 File Offset: 0x00030C72
	[CompilerGenerated]
	internal static string <PrintGraph>g__NodeListText|28_0(List<GraphNode<SITechTreeNode>> nodes)
	{
		return string.Join("|", from n in nodes
		select n.Value.nickName);
	}

	// Token: 0x04000B76 RID: 2934
	[SerializeField]
	private EAssetReleaseTier m_edReleaseTier = (EAssetReleaseTier)(-1);

	// Token: 0x04000B77 RID: 2935
	public string nickName;

	// Token: 0x04000B78 RID: 2936
	public SITechTreePageId pageId;

	// Token: 0x04000B79 RID: 2937
	public Sprite icon;

	// Token: 0x04000B7A RID: 2938
	public ESuperGameModes excludedGameModes;

	// Token: 0x04000B7B RID: 2939
	[SerializeField]
	private SITechTreeNode[] treeNodes;

	// Token: 0x04000B7C RID: 2940
	public float costMultiplier = 1f;
}
