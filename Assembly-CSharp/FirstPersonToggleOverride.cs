using System;
using UnityEngine;

// Token: 0x0200009F RID: 159
[RequireComponent(typeof(Renderer))]
public class FirstPersonToggleOverride : MonoBehaviour
{
	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060003F5 RID: 1013 RVA: 0x00017991 File Offset: 0x00015B91
	public bool Toggle
	{
		get
		{
			return this.toggle;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00017999 File Offset: 0x00015B99
	public Renderer Renderer
	{
		get
		{
			return this._renderer;
		}
	}

	// Token: 0x04000463 RID: 1123
	[SerializeField]
	private Renderer _renderer;

	// Token: 0x04000464 RID: 1124
	[SerializeField]
	private bool toggle;

	// Token: 0x04000465 RID: 1125
	[SerializeField]
	private bool doNotToggle = true;
}
