using System;
using UnityEngine;

// Token: 0x02000329 RID: 809
public class DisableOtherObjectsWhileActive : MonoBehaviour
{
	// Token: 0x06001414 RID: 5140 RVA: 0x0006C691 File Offset: 0x0006A891
	private void OnEnable()
	{
		this.SetAllActive(false);
	}

	// Token: 0x06001415 RID: 5141 RVA: 0x0006C69A File Offset: 0x0006A89A
	private void OnDisable()
	{
		this.SetAllActive(true);
	}

	// Token: 0x06001416 RID: 5142 RVA: 0x0006C6A4 File Offset: 0x0006A8A4
	private void SetAllActive(bool active)
	{
		for (int i = 0; i < this.otherObjects.Length; i++)
		{
			GameObject gameObject = this.otherObjects[i];
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
		}
		for (int j = 0; j < this.otherXSceneObjects.Length; j++)
		{
			XSceneRef xsceneRef = this.otherXSceneObjects[j];
			GameObject gameObject2;
			if (xsceneRef.TryResolve(out gameObject2) && gameObject2 != null)
			{
				gameObject2.SetActive(active);
			}
		}
	}

	// Token: 0x040018E0 RID: 6368
	public const string preErr = "[GT/DisableOtherObjectsWhileActive]  ERROR!!!  ";

	// Token: 0x040018E1 RID: 6369
	public GameObject[] otherObjects;

	// Token: 0x040018E2 RID: 6370
	public XSceneRef[] otherXSceneObjects;
}
