using System;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x02001118 RID: 4376
	public static class GameObjectExtensions
	{
		// Token: 0x06006E2F RID: 28207 RVA: 0x00240C0F File Offset: 0x0023EE0F
		public static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : MonoBehaviour
		{
			while (!obj.TryGetComponent<T>(out component))
			{
				obj = ((obj.transform.parent != null) ? obj.transform.parent.gameObject : null);
				if (!(obj != null))
				{
					return false;
				}
			}
			return true;
		}
	}
}
