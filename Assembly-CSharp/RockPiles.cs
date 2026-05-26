using System;
using UnityEngine;

// Token: 0x02000245 RID: 581
public class RockPiles : MonoBehaviour
{
	// Token: 0x06000F8F RID: 3983 RVA: 0x00054824 File Offset: 0x00052A24
	public void Show(int visiblePercentage)
	{
		if (visiblePercentage <= 0)
		{
			this.ShowRock(-1);
			return;
		}
		int rockToShow = -1;
		int num = -1;
		for (int i = 0; i < this._rocks.Length; i++)
		{
			RockPiles.RockPile rockPile = this._rocks[i];
			if (visiblePercentage >= rockPile.threshold && num < rockPile.threshold)
			{
				rockToShow = i;
				num = rockPile.threshold;
			}
		}
		this.ShowRock(rockToShow);
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x00054884 File Offset: 0x00052A84
	private void ShowRock(int rockToShow)
	{
		for (int i = 0; i < this._rocks.Length; i++)
		{
			this._rocks[i].visual.SetActive(i == rockToShow);
		}
	}

	// Token: 0x040012C3 RID: 4803
	[SerializeField]
	private RockPiles.RockPile[] _rocks;

	// Token: 0x02000246 RID: 582
	[Serializable]
	public struct RockPile
	{
		// Token: 0x040012C4 RID: 4804
		public GameObject visual;

		// Token: 0x040012C5 RID: 4805
		public int threshold;
	}
}
