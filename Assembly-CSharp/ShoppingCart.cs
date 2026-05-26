using System;
using UnityEngine;

// Token: 0x0200055F RID: 1375
public class ShoppingCart : MonoBehaviour
{
	// Token: 0x060022EC RID: 8940 RVA: 0x000BB6BD File Offset: 0x000B98BD
	public void Awake()
	{
		if (ShoppingCart.instance == null)
		{
			ShoppingCart.instance = this;
			return;
		}
		if (ShoppingCart.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Start()
	{
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Update()
	{
	}

	// Token: 0x04002DF8 RID: 11768
	[OnEnterPlay_SetNull]
	public static volatile ShoppingCart instance;
}
