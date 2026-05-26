using System;
using System.Collections.Generic;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x02001321 RID: 4897
	public class CrittersSpawningData : MonoBehaviour
	{
		// Token: 0x06007B65 RID: 31589 RVA: 0x002847A4 File Offset: 0x002829A4
		public void InitializeSpawnCollection()
		{
			for (int i = 0; i < this.SpawnParametersList.Count; i++)
			{
				for (int j = 0; j < this.SpawnParametersList[i].ChancesToSpawn; j++)
				{
					this.templateCollection.Add(i);
				}
			}
		}

		// Token: 0x06007B66 RID: 31590 RVA: 0x002847F0 File Offset: 0x002829F0
		public int GetRandomTemplate()
		{
			int index = Random.Range(0, this.templateCollection.Count - 1);
			return this.templateCollection[index];
		}

		// Token: 0x04008CBF RID: 36031
		public List<CrittersSpawningData.CreatureSpawnParameters> SpawnParametersList;

		// Token: 0x04008CC0 RID: 36032
		private List<int> templateCollection = new List<int>();

		// Token: 0x02001322 RID: 4898
		[Serializable]
		public class CreatureSpawnParameters
		{
			// Token: 0x04008CC1 RID: 36033
			public CritterTemplate Template;

			// Token: 0x04008CC2 RID: 36034
			public int ChancesToSpawn;

			// Token: 0x04008CC3 RID: 36035
			[HideInInspector]
			[NonSerialized]
			public int StartingIndex;
		}
	}
}
