using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class SITechTreeSO : ScriptableObject
{
	// Token: 0x170000BA RID: 186
	// (get) Token: 0x0600094D RID: 2381 RVA: 0x000320A2 File Offset: 0x000302A2
	// (set) Token: 0x0600094E RID: 2382 RVA: 0x000320AA File Offset: 0x000302AA
	public List<SITechTreePage> TreePages { get; private set; }

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x0600094F RID: 2383 RVA: 0x000320B3 File Offset: 0x000302B3
	// (set) Token: 0x06000950 RID: 2384 RVA: 0x000320BB File Offset: 0x000302BB
	public int TreePageCount { get; private set; }

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000951 RID: 2385 RVA: 0x000320C4 File Offset: 0x000302C4
	// (set) Token: 0x06000952 RID: 2386 RVA: 0x000320CC File Offset: 0x000302CC
	public int[] TreeNodeCounts { get; private set; }

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000953 RID: 2387 RVA: 0x000320D5 File Offset: 0x000302D5
	// (set) Token: 0x06000954 RID: 2388 RVA: 0x000320DD File Offset: 0x000302DD
	public List<GraphNode<SITechTreeNode>> AllNodes { get; private set; }

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000955 RID: 2389 RVA: 0x000320E6 File Offset: 0x000302E6
	// (set) Token: 0x06000956 RID: 2390 RVA: 0x000320EE File Offset: 0x000302EE
	public bool Initialized { get; private set; }

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000957 RID: 2391 RVA: 0x000320F7 File Offset: 0x000302F7
	public List<GameEntity> SpawnableEntities
	{
		get
		{
			this.EnsureInitialized();
			return this._spawnableEntities;
		}
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00032105 File Offset: 0x00030305
	public bool TryGetNode(SIUpgradeType upgradeType, out GraphNode<SITechTreeNode> node)
	{
		return this._nodeLookup.TryGetValue(upgradeType, out node);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00032114 File Offset: 0x00030314
	public bool TryGetUpgradeTypeByEntityTypeId(int entityTypeId, out SIUpgradeType upgradeType)
	{
		return this._upgradeTypeByEntityTypeId.TryGetValue(entityTypeId, out upgradeType);
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00032123 File Offset: 0x00030323
	public bool IsSpawnableEntityTypeId(int entityTypeId)
	{
		this.EnsureInitialized();
		return this._spawnableEntityTypeIds.Contains(entityTypeId);
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x00032138 File Offset: 0x00030338
	public bool IsValidPage(SITechTreePageId id)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x0003219C File Offset: 0x0003039C
	public SITechTreePage GetTreePage(SITechTreePageId id)
	{
		SITechTreePage result;
		if (!this.TryGetTreePage(id, out result))
		{
			return null;
		}
		return result;
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x000321B8 File Offset: 0x000303B8
	public bool TryGetTreePage(SITechTreePageId id, out SITechTreePage treePage)
	{
		foreach (SITechTreePage sitechTreePage in this.TreePages)
		{
			if (sitechTreePage.pageId == id && sitechTreePage.IsValid)
			{
				treePage = sitechTreePage;
				return true;
			}
		}
		treePage = null;
		return false;
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x00032224 File Offset: 0x00030424
	public bool IsValidNode(int pageId, int nodeId)
	{
		return this.IsValidNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x00032233 File Offset: 0x00030433
	public bool IsValidNode(SIUpgradeType upgradeType)
	{
		return this._nodeLookup.ContainsKey(upgradeType);
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x00032241 File Offset: 0x00030441
	public SITechTreeNode GetTreeNode(int pageId, int nodeId)
	{
		return this.GetTreeNode(SIUpgradeTypeSystem.GetUpgradeType(pageId, nodeId));
	}

	// Token: 0x06000961 RID: 2401 RVA: 0x00032250 File Offset: 0x00030450
	public SITechTreeNode GetTreeNode(SIUpgradeType upgradeType)
	{
		GraphNode<SITechTreeNode> graphNode;
		if (this._nodeLookup.TryGetValue(upgradeType, out graphNode))
		{
			return graphNode.Value;
		}
		return null;
	}

	// Token: 0x06000962 RID: 2402 RVA: 0x00032275 File Offset: 0x00030475
	public void EnsureInitialized()
	{
		if (!this.Initialized)
		{
			this.InitTechTree();
		}
	}

	// Token: 0x06000963 RID: 2403 RVA: 0x00032288 File Offset: 0x00030488
	private void InitTechTree()
	{
		Debug.Log("[SI] SITechTreeSO.InitTechTree");
		this.ClearTechTree();
		this.TreePages = new List<SITechTreePage>();
		this._spawnableEntities = new List<GameEntity>();
		int num = 0;
		foreach (SITechTreePage sitechTreePage in this.treePages)
		{
			if (sitechTreePage.IsValid)
			{
				sitechTreePage.BuildGraph();
				foreach (GraphNode<SITechTreeNode> graphNode in sitechTreePage.Roots)
				{
					foreach (GraphNode<SITechTreeNode> graphNode2 in graphNode.TraversePreOrder())
					{
						if (!this._nodeLookup.ContainsKey(graphNode2.Value.upgradeType))
						{
							this._nodeLookup.Add(graphNode2.Value.upgradeType, graphNode2);
						}
					}
				}
				foreach (SITechTreeNode sitechTreeNode in sitechTreePage.DispensableGadgets)
				{
					num++;
					this.AddSpawnableGadget(sitechTreeNode.unlockedGadgetPrefab);
				}
				if (sitechTreePage.Roots.Count > 0)
				{
					this.TreePages.Add(sitechTreePage);
				}
			}
		}
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Clear();
		}
		else
		{
			this._upgradeTypeByEntityTypeId = new NativeHashMap<int, SIUpgradeType>(num, Allocator.Persistent);
		}
		foreach (SITechTreePage sitechTreePage2 in this.treePages)
		{
			if (sitechTreePage2.IsValid)
			{
				foreach (SITechTreeNode sitechTreeNode2 in sitechTreePage2.DispensableGadgets)
				{
					int staticHash = sitechTreeNode2.unlockedGadgetPrefab.gameObject.name.GetStaticHash();
					this._upgradeTypeByEntityTypeId.TryAdd(staticHash, sitechTreeNode2.upgradeType);
				}
			}
		}
		this.AllNodes = new List<GraphNode<SITechTreeNode>>(this._nodeLookup.Values);
		this.TreePageCount = (from v in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType))
		select v.GetPageId()).Max() + 1;
		this.TreeNodeCounts = new int[this.TreePageCount];
		foreach (SIUpgradeType self in (SIUpgradeType[])Enum.GetValues(typeof(SIUpgradeType)))
		{
			int pageId = self.GetPageId();
			int nodeId = self.GetNodeId();
			this.TreeNodeCounts[pageId] = Mathf.Max(this.TreeNodeCounts[pageId], nodeId + 1);
		}
		this.Initialized = true;
	}

	// Token: 0x06000964 RID: 2404 RVA: 0x00032580 File Offset: 0x00030780
	private void AddSpawnableGadget(GameEntity entity)
	{
		this._spawnableEntities.Add(entity);
		this._spawnableEntityTypeIds.Add(entity.gameObject.name.GetStaticHash());
		IPrefabRequirements component = entity.GetComponent<IPrefabRequirements>();
		if (component != null)
		{
			foreach (GameEntity gameEntity in component.RequiredPrefabs)
			{
				this._spawnableEntities.Add(gameEntity);
				this._spawnableEntityTypeIds.Add(gameEntity.gameObject.name.GetStaticHash());
			}
		}
	}

	// Token: 0x06000965 RID: 2405 RVA: 0x00032620 File Offset: 0x00030820
	private void ClearTechTree()
	{
		SITechTreePage[] array = this.treePages;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ClearGraph();
		}
		this._nodeLookup.Clear();
		this._spawnableEntityTypeIds.Clear();
		if (this._upgradeTypeByEntityTypeId.IsCreated)
		{
			this._upgradeTypeByEntityTypeId.Dispose();
		}
		this.Initialized = false;
	}

	// Token: 0x04000B5E RID: 2910
	private const string preLog = "[SITechTreeSO]  ";

	// Token: 0x04000B5F RID: 2911
	private const string preErr = "[SITechTreeSO]  ERROR!!!  ";

	// Token: 0x04000B60 RID: 2912
	private const int RESOURCE_CAP = 20;

	// Token: 0x04000B61 RID: 2913
	[SerializeField]
	private SITechTreePage[] treePages;

	// Token: 0x04000B62 RID: 2914
	private readonly Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>> _nodeLookup = new Dictionary<SIUpgradeType, GraphNode<SITechTreeNode>>();

	// Token: 0x04000B63 RID: 2915
	private NativeHashMap<int, SIUpgradeType> _upgradeTypeByEntityTypeId;

	// Token: 0x04000B64 RID: 2916
	private readonly HashSet<int> _spawnableEntityTypeIds = new HashSet<int>();

	// Token: 0x04000B6A RID: 2922
	private List<GameEntity> _spawnableEntities;
}
