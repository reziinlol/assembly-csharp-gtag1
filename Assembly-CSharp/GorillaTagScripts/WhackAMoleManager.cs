using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EC9 RID: 3785
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06005D2A RID: 23850 RVA: 0x001D8D5E File Offset: 0x001D6F5E
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x06005D2B RID: 23851 RVA: 0x00018E08 File Offset: 0x00017008
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005D2C RID: 23852 RVA: 0x00018E11 File Offset: 0x00017011
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001D8D74 File Offset: 0x001D6F74
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001D8DC4 File Offset: 0x001D6FC4
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001D8DCC File Offset: 0x001D6FCC
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001D8DDB File Offset: 0x001D6FDB
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x04006BC5 RID: 27589
		public static WhackAMoleManager instance;

		// Token: 0x04006BC6 RID: 27590
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
