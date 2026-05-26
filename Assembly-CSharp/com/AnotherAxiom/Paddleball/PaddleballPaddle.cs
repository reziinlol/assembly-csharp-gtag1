using System;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x020010CB RID: 4299
	public class PaddleballPaddle : MonoBehaviour
	{
		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06006BA9 RID: 27561 RVA: 0x0022E1AE File Offset: 0x0022C3AE
		public bool Right
		{
			get
			{
				return this.right;
			}
		}

		// Token: 0x04007BEC RID: 31724
		[SerializeField]
		private bool right;
	}
}
