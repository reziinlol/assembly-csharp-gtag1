using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020008FF RID: 2303
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x06003C2B RID: 15403 RVA: 0x00148B10 File Offset: 0x00146D10
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x06003C2C RID: 15404 RVA: 0x00148B3C File Offset: 0x00146D3C
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x06003C2D RID: 15405 RVA: 0x00148B72 File Offset: 0x00146D72
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04004CC6 RID: 19654
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04004CC7 RID: 19655
	public Transform rootParent;

	// Token: 0x04004CC8 RID: 19656
	private WorldShareableItem item;

	// Token: 0x04004CC9 RID: 19657
	private Transform grabPtInitParent;
}
