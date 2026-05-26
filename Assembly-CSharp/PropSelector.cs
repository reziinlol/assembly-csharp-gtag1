using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000280 RID: 640
public class PropSelector : MonoBehaviour
{
	// Token: 0x06001146 RID: 4422 RVA: 0x0005CDA8 File Offset: 0x0005AFA8
	private void Start()
	{
		foreach (GameObject gameObject in new List<GameObject>((from x in this._props
		orderby PropSelector._gRandom.Next()
		select x).Take(this._desiredActivePropsNum)))
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x0400149C RID: 5276
	[SerializeField]
	private List<GameObject> _props = new List<GameObject>();

	// Token: 0x0400149D RID: 5277
	[SerializeField]
	private int _desiredActivePropsNum = 1;

	// Token: 0x0400149E RID: 5278
	private static readonly Random _gRandom = new Random();
}
