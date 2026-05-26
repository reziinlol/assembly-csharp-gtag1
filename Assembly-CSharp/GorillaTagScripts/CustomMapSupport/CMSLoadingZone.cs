using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F35 RID: 3893
	public class CMSLoadingZone : MonoBehaviour
	{
		// Token: 0x06006117 RID: 24855 RVA: 0x001F459E File Offset: 0x001F279E
		private void Start()
		{
			base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
		}

		// Token: 0x06006118 RID: 24856 RVA: 0x001F45B4 File Offset: 0x001F27B4
		public void SetupLoadingZone(LoadZoneSettings settings, in string[] assetBundleSceneFilePaths)
		{
			this.scenesToLoad = this.GetSceneIndexes(settings.scenesToLoad, assetBundleSceneFilePaths);
			this.scenesToUnload = this.CleanSceneUnloadArray(settings.scenesToUnload, settings.scenesToLoad, assetBundleSceneFilePaths);
			this.useDynamicLighting = settings.useDynamicLighting;
			this.dynamicLightingAmbientColor = settings.UberShaderAmbientDynamicLight;
			base.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x06006119 RID: 24857 RVA: 0x001F463C File Offset: 0x001F283C
		private int[] GetSceneIndexes(List<string> sceneNames, in string[] assetBundleSceneFilePaths)
		{
			int[] array = new int[sceneNames.Count];
			for (int i = 0; i < sceneNames.Count; i++)
			{
				for (int j = 0; j < assetBundleSceneFilePaths.Length; j++)
				{
					if (string.Equals(sceneNames[i], this.GetSceneNameFromFilePath(assetBundleSceneFilePaths[j])))
					{
						array[i] = j;
						break;
					}
				}
			}
			return array;
		}

		// Token: 0x0600611A RID: 24858 RVA: 0x001F4694 File Offset: 0x001F2894
		private int[] CleanSceneUnloadArray(List<string> unload, List<string> load, in string[] assetBundleSceneFilePaths)
		{
			for (int i = 0; i < load.Count; i++)
			{
				if (unload.Contains(load[i]))
				{
					unload.Remove(load[i]);
				}
			}
			return this.GetSceneIndexes(unload, assetBundleSceneFilePaths);
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x001F46D8 File Offset: 0x001F28D8
		public void OnTriggerEnter(Collider other)
		{
			if (other == GTPlayer.Instance.bodyCollider)
			{
				if (this.useDynamicLighting)
				{
					CustomMapLoader.SetZoneDynamicLighting(true);
					GameLightingManager.instance.SetAmbientLightDynamic(this.dynamicLightingAmbientColor);
				}
				else
				{
					CustomMapLoader.SetZoneDynamicLighting(false);
					GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				}
				CustomMapManager.LoadZoneTriggered(this.scenesToLoad, this.scenesToUnload);
			}
		}

		// Token: 0x0600611C RID: 24860 RVA: 0x001F4741 File Offset: 0x001F2941
		private string GetSceneNameFromFilePath(string filePath)
		{
			string[] array = filePath.Split("/", StringSplitOptions.None);
			return array[array.Length - 1].Split(".", StringSplitOptions.None)[0];
		}

		// Token: 0x04006FD2 RID: 28626
		private int[] scenesToLoad;

		// Token: 0x04006FD3 RID: 28627
		private int[] scenesToUnload;

		// Token: 0x04006FD4 RID: 28628
		private bool useDynamicLighting;

		// Token: 0x04006FD5 RID: 28629
		private Color dynamicLightingAmbientColor;
	}
}
