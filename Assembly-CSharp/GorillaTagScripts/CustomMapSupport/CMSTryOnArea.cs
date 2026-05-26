using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000F3E RID: 3902
	public class CMSTryOnArea : MonoBehaviour
	{
		// Token: 0x06006151 RID: 24913 RVA: 0x001F5C4E File Offset: 0x001F3E4E
		public void InitializeForCustomMap(CompositeTriggerEvents customMapTryOnArea, Scene customMapScene)
		{
			this.originalScene = customMapScene;
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.AddCollider(this.tryOnAreaCollider);
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x001F5C71 File Offset: 0x001F3E71
		public void RemoveFromCustomMap(CompositeTriggerEvents customMapTryOnArea)
		{
			if (this.tryOnAreaCollider.IsNull())
			{
				return;
			}
			customMapTryOnArea.RemoveCollider(this.tryOnAreaCollider);
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x001F5C8D File Offset: 0x001F3E8D
		public bool IsFromScene(Scene unloadingScene)
		{
			return unloadingScene == this.originalScene;
		}

		// Token: 0x04006FFB RID: 28667
		private Scene originalScene;

		// Token: 0x04006FFC RID: 28668
		public BoxCollider tryOnAreaCollider;
	}
}
