using System;
using System.Collections.Generic;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002FF RID: 767
[ExecuteInEditMode]
public class CosmeticsDemoRig : MonoBehaviour
{
	// Token: 0x040017F7 RID: 6135
	[SerializeField]
	private VRRig _vrRig;

	// Token: 0x040017F8 RID: 6136
	private Transform[] _vrRigBoneXforms;

	// Token: 0x040017F9 RID: 6137
	private Transform[] _vrRigSlotXforms;

	// Token: 0x040017FA RID: 6138
	[SerializeField]
	private Transform chestOffset;

	// Token: 0x040017FB RID: 6139
	[SerializeField]
	private Transform leftArmOffset;

	// Token: 0x040017FC RID: 6140
	[SerializeField]
	private Transform rightArmOffset;

	// Token: 0x040017FD RID: 6141
	private Vector3 badgeDefaultPos;

	// Token: 0x040017FE RID: 6142
	private Quaternion badgeDefaultRot;

	// Token: 0x040017FF RID: 6143
	private bool isInitialized;

	// Token: 0x04001800 RID: 6144
	private CosmeticsDemoRig.EdSpawnedCosmetic emptyCosmetic;

	// Token: 0x04001801 RID: 6145
	private Material defaultFaceMaterial;

	// Token: 0x04001802 RID: 6146
	[SerializeField]
	[HideInInspector]
	private Material myDefaultSkinMaterialInstance;

	// Token: 0x04001803 RID: 6147
	[SerializeField]
	[HideInInspector]
	private Material materialToChangeTo0;

	// Token: 0x04001804 RID: 6148
	[SerializeField]
	[HideInInspector]
	private Color monkeColor = new Color(0f, 0f, 0f);

	// Token: 0x04001805 RID: 6149
	[SerializeField]
	[HideInInspector]
	private GorillaSkin currentSkin;

	// Token: 0x04001806 RID: 6150
	[SerializeField]
	[HideInInspector]
	private GorillaSkin defaultSkin;

	// Token: 0x04001807 RID: 6151
	[SerializeField]
	[HideInInspector]
	private Material[] faceMaterialSwaps = new Material[10];

	// Token: 0x04001808 RID: 6152
	[HideInInspector]
	public int materialIndex;

	// Token: 0x04001809 RID: 6153
	private int selectedMouth;

	// Token: 0x0400180A RID: 6154
	[HideInInspector]
	public UnityEvent<Color> OnColorChange;

	// Token: 0x0400180B RID: 6155
	[SerializeField]
	private CosmeticsDemoRig.EdSpawnedCosmetic[] spawnedCosmetics = new CosmeticsDemoRig.EdSpawnedCosmetic[16];

	// Token: 0x02000300 RID: 768
	[Serializable]
	private struct EdSpawnedCosmetic
	{
		// Token: 0x0400180C RID: 6156
		public string itemName;

		// Token: 0x0400180D RID: 6157
		public CosmeticSO so;

		// Token: 0x0400180E RID: 6158
		public List<GameObject> objects;

		// Token: 0x0400180F RID: 6159
		public List<GameObject> holdableObjects;

		// Token: 0x04001810 RID: 6160
		public bool isEmpty;
	}
}
