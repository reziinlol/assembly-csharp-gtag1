using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000814 RID: 2068
public class GRToolUpgradePurchaseStationShelf : MonoBehaviour
{
	// Token: 0x06003520 RID: 13600 RVA: 0x00126038 File Offset: 0x00124238
	public void Awake()
	{
		for (int i = 0; i < this.gRPurchaseSlots.Count; i++)
		{
			Renderer[] componentsInChildren = this.gRPurchaseSlots[i].SlotPivot.gameObject.GetComponentsInChildren<Renderer>();
			this.slotRenderers.Add(componentsInChildren);
			Material[][] array = new Material[componentsInChildren.Length][];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				array[j] = componentsInChildren[j].sharedMaterials;
			}
			this.slotOriginalMaterials.Add(array);
		}
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x001260B4 File Offset: 0x001242B4
	public void SetMaterialOverride(int slotID, Material overrideMaterial)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].overrideMaterial == overrideMaterial)
		{
			return;
		}
		if (slotID >= this.slotRenderers.Count)
		{
			return;
		}
		this.gRPurchaseSlots[slotID].overrideMaterial = overrideMaterial;
		for (int i = 0; i < this.slotRenderers[slotID].Length; i++)
		{
			Renderer renderer = this.slotRenderers[slotID][i];
			if (overrideMaterial == null)
			{
				renderer.materials = this.slotOriginalMaterials[slotID][i];
			}
			else
			{
				Material[] array = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = overrideMaterial;
				}
				renderer.materials = array;
			}
		}
	}

	// Token: 0x06003522 RID: 13602 RVA: 0x00126180 File Offset: 0x00124380
	public void SetBacklightStateAndMaterial(int slotID, bool isEnabled, Material materialOverride)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].BacklightRenderer != null)
		{
			if (!isEnabled)
			{
				this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = false;
				return;
			}
			this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = true;
			this.gRPurchaseSlots[slotID].BacklightRenderer.sharedMaterial = materialOverride;
		}
	}

	// Token: 0x04004591 RID: 17809
	public string ShelfName;

	// Token: 0x04004592 RID: 17810
	private List<Material[][]> slotOriginalMaterials = new List<Material[][]>();

	// Token: 0x04004593 RID: 17811
	private List<Renderer[]> slotRenderers = new List<Renderer[]>();

	// Token: 0x04004594 RID: 17812
	public List<GRToolUpgradePurchaseStationShelf.GRPurchaseSlot> gRPurchaseSlots;

	// Token: 0x02000815 RID: 2069
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x04004595 RID: 17813
		public TMP_Text Name;

		// Token: 0x04004596 RID: 17814
		public TMP_Text Price;

		// Token: 0x04004597 RID: 17815
		public Transform SlotPivot;

		// Token: 0x04004598 RID: 17816
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x04004599 RID: 17817
		public GameEntity ToolEntityPrefab;

		// Token: 0x0400459A RID: 17818
		public float RopeYaw;

		// Token: 0x0400459B RID: 17819
		public float RopePitch;

		// Token: 0x0400459C RID: 17820
		public MeshRenderer BacklightRenderer;

		// Token: 0x0400459D RID: 17821
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x0400459E RID: 17822
		[NonSerialized]
		public bool canAfford;

		// Token: 0x0400459F RID: 17823
		[NonSerialized]
		public string purchaseText = "";
	}
}
