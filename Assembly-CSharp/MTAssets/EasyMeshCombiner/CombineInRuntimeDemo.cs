using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x020010B9 RID: 4281
	public class CombineInRuntimeDemo : MonoBehaviour
	{
		// Token: 0x06006B73 RID: 27507 RVA: 0x0022BD80 File Offset: 0x00229F80
		private void Update()
		{
			if (!this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(true);
				this.undoButton.SetActive(false);
			}
			if (this.runtimeCombiner.isTargetMeshesMerged())
			{
				this.combineButton.SetActive(false);
				this.undoButton.SetActive(true);
			}
		}

		// Token: 0x06006B74 RID: 27508 RVA: 0x0022BDD7 File Offset: 0x00229FD7
		public void CombineMeshes()
		{
			this.runtimeCombiner.CombineMeshes();
		}

		// Token: 0x06006B75 RID: 27509 RVA: 0x0022BDE5 File Offset: 0x00229FE5
		public void UndoMerge()
		{
			this.runtimeCombiner.UndoMerge();
		}

		// Token: 0x04007B76 RID: 31606
		public GameObject combineButton;

		// Token: 0x04007B77 RID: 31607
		public GameObject undoButton;

		// Token: 0x04007B78 RID: 31608
		public RuntimeMeshCombiner runtimeCombiner;
	}
}
