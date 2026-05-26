using System;
using System.Collections;
using UnityEngine;

// Token: 0x020009C7 RID: 2503
public class SodaBubble : MonoBehaviour
{
	// Token: 0x06004014 RID: 16404 RVA: 0x00156EC3 File Offset: 0x001550C3
	public void Pop()
	{
		base.StartCoroutine(this.PopCoroutine());
	}

	// Token: 0x06004015 RID: 16405 RVA: 0x00156ED2 File Offset: 0x001550D2
	private IEnumerator PopCoroutine()
	{
		this.audioSource.GTPlay();
		this.bubbleMesh.gameObject.SetActive(false);
		this.bubbleCollider.gameObject.SetActive(false);
		yield return new WaitForSeconds(1f);
		this.bubbleMesh.gameObject.SetActive(true);
		this.bubbleCollider.gameObject.SetActive(true);
		ObjectPools.instance.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0400509A RID: 20634
	public MeshRenderer bubbleMesh;

	// Token: 0x0400509B RID: 20635
	public Rigidbody body;

	// Token: 0x0400509C RID: 20636
	public MeshCollider bubbleCollider;

	// Token: 0x0400509D RID: 20637
	public AudioSource audioSource;
}
