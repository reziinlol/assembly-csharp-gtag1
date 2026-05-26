using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200060F RID: 1551
[CreateAssetMenu(fileName = "BuilderMaterialResourceColors", menuName = "Gorilla Tag/Builder/ResourceColors", order = 0)]
public class BuilderResourceColors : ScriptableObject
{
	// Token: 0x0600269F RID: 9887 RVA: 0x000CC618 File Offset: 0x000CA818
	public Color GetColorForType(BuilderResourceType type)
	{
		foreach (BuilderResourceColor builderResourceColor in this.colors)
		{
			if (builderResourceColor.type == type)
			{
				return builderResourceColor.color;
			}
		}
		return Color.black;
	}

	// Token: 0x0400320E RID: 12814
	public List<BuilderResourceColor> colors;
}
