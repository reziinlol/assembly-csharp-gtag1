using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009CC RID: 2508
public class SimpleUnloadUnusedAssets : MonoBehaviour
{
	// Token: 0x06004041 RID: 16449 RVA: 0x00157D6C File Offset: 0x00155F6C
	private void OnEnable()
	{
		base.StartCoroutine(this.UnloadUnusedAssets());
	}

	// Token: 0x06004042 RID: 16450 RVA: 0x00157D7B File Offset: 0x00155F7B
	private IEnumerator UnloadUnusedAssets()
	{
		yield return new WaitForSeconds(this.WaitForUnload);
		Debug.Log(string.Format("SimpleUnloadUnusedAssets: Forcing unload unused assets after waiting {0} seconds!", this.WaitForUnload));
		Resources.UnloadUnusedAssets();
		yield break;
	}

	// Token: 0x040050DA RID: 20698
	public float WaitForUnload = 5f;
}
