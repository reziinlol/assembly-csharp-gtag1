using System;
using UnityEngine;

// Token: 0x02000AE3 RID: 2787
public class MonoBehaviourStatic<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x1700069C RID: 1692
	// (get) Token: 0x06004720 RID: 18208 RVA: 0x0017FCB1 File Offset: 0x0017DEB1
	public static T Instance
	{
		get
		{
			return MonoBehaviourStatic<T>.gInstance;
		}
	}

	// Token: 0x06004721 RID: 18209 RVA: 0x0017FCB8 File Offset: 0x0017DEB8
	protected void Awake()
	{
		if (MonoBehaviourStatic<T>.gInstance && MonoBehaviourStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
		}
		MonoBehaviourStatic<T>.gInstance = (this as T);
		this.OnAwake();
	}

	// Token: 0x06004722 RID: 18210 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnAwake()
	{
	}

	// Token: 0x040059A1 RID: 22945
	protected static T gInstance;
}
