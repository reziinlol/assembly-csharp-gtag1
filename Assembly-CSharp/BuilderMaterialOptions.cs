using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000626 RID: 1574
[CreateAssetMenu(fileName = "BuilderMaterialOptions01a", menuName = "Gorilla Tag/Builder/Options", order = 0)]
public class BuilderMaterialOptions : ScriptableObject
{
	// Token: 0x06002738 RID: 10040 RVA: 0x000D026C File Offset: 0x000CE46C
	public void GetMaterialFromType(int materialType, out Material material, out int soundIndex)
	{
		if (this.options == null)
		{
			material = null;
			soundIndex = -1;
			return;
		}
		foreach (BuilderMaterialOptions.Options options in this.options)
		{
			if (options.materialId.GetHashCode() == materialType)
			{
				material = options.material;
				soundIndex = options.soundIndex;
				return;
			}
		}
		material = null;
		soundIndex = -1;
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000D02F0 File Offset: 0x000CE4F0
	public void GetDefaultMaterial(out int materialType, out Material material, out int soundIndex)
	{
		if (this.options.Count > 0)
		{
			materialType = this.options[0].materialId.GetHashCode();
			material = this.options[0].material;
			soundIndex = this.options[0].soundIndex;
			return;
		}
		materialType = -1;
		material = null;
		soundIndex = -1;
	}

	// Token: 0x040032DB RID: 13019
	public List<BuilderMaterialOptions.Options> options;

	// Token: 0x02000627 RID: 1575
	[Serializable]
	public class Options
	{
		// Token: 0x040032DC RID: 13020
		public string materialId;

		// Token: 0x040032DD RID: 13021
		public Material material;

		// Token: 0x040032DE RID: 13022
		[GorillaSoundLookup]
		public int soundIndex;

		// Token: 0x040032DF RID: 13023
		[NonSerialized]
		public int materialType;
	}
}
