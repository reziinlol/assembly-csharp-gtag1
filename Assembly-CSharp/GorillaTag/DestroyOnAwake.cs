using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200113A RID: 4410
	public class DestroyOnAwake : MonoBehaviour
	{
		// Token: 0x06007002 RID: 28674 RVA: 0x00248F40 File Offset: 0x00247140
		protected void Awake()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06007003 RID: 28675 RVA: 0x00248F70 File Offset: 0x00247170
		protected void OnEnable()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}

		// Token: 0x06007004 RID: 28676 RVA: 0x00248FA0 File Offset: 0x002471A0
		protected void Update()
		{
			try
			{
				Object.Destroy(base.gameObject);
			}
			catch
			{
			}
		}
	}
}
