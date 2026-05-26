using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200115C RID: 4444
	[CreateAssetMenu(fileName = "New String List", menuName = "String List")]
	public class StringList : ScriptableObject
	{
		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x06007081 RID: 28801 RVA: 0x0024ADF2 File Offset: 0x00248FF2
		public string[] Strings
		{
			get
			{
				return this.strings;
			}
		}

		// Token: 0x04008066 RID: 32870
		[SerializeField]
		private string[] strings;
	}
}
