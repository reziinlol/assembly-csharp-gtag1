using System;
using UnityEngine;

// Token: 0x02000865 RID: 2149
public class GorillaHasUITransformFollow : MonoBehaviour
{
	// Token: 0x060037F9 RID: 14329 RVA: 0x00131864 File Offset: 0x0012FA64
	private void Awake()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(base.gameObject.activeSelf);
		}
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x001318A0 File Offset: 0x0012FAA0
	private void OnDestroy()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy(array[i].gameObject);
		}
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x001318D0 File Offset: 0x0012FAD0
	private void OnEnable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x00131900 File Offset: 0x0012FB00
	private void OnDisable()
	{
		GorillaUITransformFollow[] array = this.transformFollowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x040047DF RID: 18399
	public GorillaUITransformFollow[] transformFollowers;
}
