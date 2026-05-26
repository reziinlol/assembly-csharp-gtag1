using System;
using UnityEngine;

// Token: 0x02000E22 RID: 3618
public class ZoneGraphBSP : MonoBehaviour
{
	// Token: 0x17000853 RID: 2131
	// (get) Token: 0x06005820 RID: 22560 RVA: 0x001CA037 File Offset: 0x001C8237
	// (set) Token: 0x06005821 RID: 22561 RVA: 0x001CA03E File Offset: 0x001C823E
	public static ZoneGraphBSP Instance { get; private set; }

	// Token: 0x06005822 RID: 22562 RVA: 0x001CA046 File Offset: 0x001C8246
	private void Awake()
	{
		if (ZoneGraphBSP.Instance == null)
		{
			ZoneGraphBSP.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06005823 RID: 22563 RVA: 0x001CA064 File Offset: 0x001C8264
	public void Preprocess()
	{
		BoxCollider[] componentsInChildren = base.GetComponentsInChildren<BoxCollider>(true);
		if (componentsInChildren != null)
		{
			foreach (BoxCollider boxCollider in componentsInChildren)
			{
				if (boxCollider.transform.GetComponent<ZoneDef>() != null)
				{
					Object.Destroy(boxCollider);
				}
				else
				{
					Object.Destroy(boxCollider.gameObject);
				}
			}
		}
	}

	// Token: 0x06005824 RID: 22564 RVA: 0x001CA0B8 File Offset: 0x001C82B8
	public void CompileBSP()
	{
		ZoneDef[] componentsInChildren = base.gameObject.GetComponentsInChildren<ZoneDef>();
		this.bspTree = BSPTreeBuilder.BuildTree(componentsInChildren);
		if (this.bspTree != null && this.bspTree.nodes != null)
		{
			Debug.Log(string.Format("BSP Tree compiled with {0} zones, {1} nodes", componentsInChildren.Length, this.bspTree.nodes.Length));
			return;
		}
		Debug.Log("BSP Tree compilation failed - no zones found");
	}

	// Token: 0x06005825 RID: 22565 RVA: 0x001CA126 File Offset: 0x001C8326
	public ZoneDef FindZoneAtPoint(Vector3 worldPoint)
	{
		SerializableBSPTree serializableBSPTree = this.bspTree;
		if (serializableBSPTree == null)
		{
			return null;
		}
		return serializableBSPTree.FindZone(worldPoint);
	}

	// Token: 0x06005826 RID: 22566 RVA: 0x001CA13A File Offset: 0x001C833A
	public bool IsPointInAnyZone(Vector3 worldPoint)
	{
		return this.FindZoneAtPoint(worldPoint) != null;
	}

	// Token: 0x06005827 RID: 22567 RVA: 0x001CA149 File Offset: 0x001C8349
	public bool HasCompiledTree()
	{
		return this.bspTree != null && this.bspTree.nodes != null && this.bspTree.nodes.Length != 0;
	}

	// Token: 0x06005828 RID: 22568 RVA: 0x001CA171 File Offset: 0x001C8371
	public SerializableBSPTree GetBSPTree()
	{
		return this.bspTree;
	}

	// Token: 0x040068C4 RID: 26820
	[SerializeField]
	private SerializableBSPTree bspTree;
}
