using System;
using System.Collections;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001107 RID: 4359
	public class TestRopePerf : MonoBehaviour
	{
		// Token: 0x06006DD5 RID: 28117 RVA: 0x0023E491 File Offset: 0x0023C691
		private IEnumerator Start()
		{
			yield break;
		}

		// Token: 0x04007EDB RID: 32475
		[SerializeField]
		private GameObject ropesOld;

		// Token: 0x04007EDC RID: 32476
		[SerializeField]
		private GameObject ropesCustom;

		// Token: 0x04007EDD RID: 32477
		[SerializeField]
		private GameObject ropesCustomVectorized;
	}
}
