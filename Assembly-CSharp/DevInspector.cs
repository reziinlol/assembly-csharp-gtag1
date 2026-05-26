using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000323 RID: 803
public class DevInspector : MonoBehaviour
{
	// Token: 0x060013EB RID: 5099 RVA: 0x0006BA83 File Offset: 0x00069C83
	private void OnEnable()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040018C6 RID: 6342
	public GameObject pivot;

	// Token: 0x040018C7 RID: 6343
	public Text outputInfo;

	// Token: 0x040018C8 RID: 6344
	public Component[] componentToInspect;

	// Token: 0x040018C9 RID: 6345
	public bool isEnabled;

	// Token: 0x040018CA RID: 6346
	public bool autoFind = true;

	// Token: 0x040018CB RID: 6347
	public GameObject canvas;

	// Token: 0x040018CC RID: 6348
	public int sidewaysOffset;
}
