using System;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200109E RID: 4254
	public class StoreBundleData : ScriptableObject
	{
		// Token: 0x06006AB5 RID: 27317 RVA: 0x00227908 File Offset: 0x00225B08
		public void OnValidate()
		{
			if (this.playfabBundleID.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + base.name);
			}
			if (this.bundleSKU.Contains(' '))
			{
				Debug.LogError("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + base.name);
			}
		}

		// Token: 0x04007ADD RID: 31453
		public string playfabBundleID = "NULL";

		// Token: 0x04007ADE RID: 31454
		public string bundleSKU = "NULL SKU";

		// Token: 0x04007ADF RID: 31455
		public NexusCreatorCode creatorCode;

		// Token: 0x04007AE0 RID: 31456
		public Sprite bundleImage;

		// Token: 0x04007AE1 RID: 31457
		public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";
	}
}
