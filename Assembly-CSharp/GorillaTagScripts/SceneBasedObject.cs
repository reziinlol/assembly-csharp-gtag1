using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F28 RID: 3880
	public class SceneBasedObject : MonoBehaviour
	{
		// Token: 0x060060EA RID: 24810 RVA: 0x001F368F File Offset: 0x001F188F
		public bool IsLocalPlayerInScene()
		{
			return (ZoneManagement.instance.GetAllLoadedScenes().Count <= 1 || this.zone != GTZone.forest) && ZoneManagement.instance.IsSceneLoaded(this.zone);
		}

		// Token: 0x04006F80 RID: 28544
		public GTZone zone;
	}
}
