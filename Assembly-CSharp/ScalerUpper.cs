using System;
using UnityEngine;

// Token: 0x020004E1 RID: 1249
public class ScalerUpper : MonoBehaviour
{
	// Token: 0x06001E5F RID: 7775 RVA: 0x000A279C File Offset: 0x000A099C
	private void Update()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one * this.scaleCurve.Evaluate(this.t);
		}
		this.t += Time.deltaTime;
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x000A27FB File Offset: 0x000A09FB
	private void OnEnable()
	{
		this.t = 0f;
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x000A2808 File Offset: 0x000A0A08
	private void OnDisable()
	{
		for (int i = 0; i < this.target.Length; i++)
		{
			this.target[i].transform.localScale = Vector3.one;
		}
	}

	// Token: 0x0400288C RID: 10380
	[SerializeField]
	private Transform[] target;

	// Token: 0x0400288D RID: 10381
	[SerializeField]
	private AnimationCurve scaleCurve;

	// Token: 0x0400288E RID: 10382
	private float t;
}
