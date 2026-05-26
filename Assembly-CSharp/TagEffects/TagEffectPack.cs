using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x020010D1 RID: 4305
	[CreateAssetMenu(fileName = "New Tag Effect Pack", menuName = "Tag Effect Pack")]
	public class TagEffectPack : ScriptableObject
	{
		// Token: 0x04007C01 RID: 31745
		public GameObject thirdPerson;

		// Token: 0x04007C02 RID: 31746
		public bool thirdPersonParentEffect = true;

		// Token: 0x04007C03 RID: 31747
		public GameObject firstPerson;

		// Token: 0x04007C04 RID: 31748
		public bool firstPersonParentEffect = true;

		// Token: 0x04007C05 RID: 31749
		public GameObject highFive;

		// Token: 0x04007C06 RID: 31750
		public bool highFiveParentEffect;

		// Token: 0x04007C07 RID: 31751
		public GameObject fistBump;

		// Token: 0x04007C08 RID: 31752
		public bool fistBumpParentEffect;

		// Token: 0x04007C09 RID: 31753
		public bool shouldFaceTagger;
	}
}
