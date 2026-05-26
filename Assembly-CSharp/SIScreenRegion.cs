using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class SIScreenRegion : MonoBehaviour
{
	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000947 RID: 2375 RVA: 0x0003200B File Offset: 0x0003020B
	public bool HasPressedButton
	{
		get
		{
			return this._hasPressedButton;
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00032014 File Offset: 0x00030214
	private void OnTriggerEnter(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Add(componentInParent);
		}
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00032038 File Offset: 0x00030238
	private void OnTriggerExit(Collider other)
	{
		GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent != null)
		{
			this.handIndicators.Remove(componentInParent);
			if (this.handIndicators.Count == 0)
			{
				this.ClearPressedIndicator();
			}
		}
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0003206F File Offset: 0x0003026F
	public void RegisterButtonPress()
	{
		if (this.handIndicators.Count > 0)
		{
			this._hasPressedButton = true;
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00032086 File Offset: 0x00030286
	private void ClearPressedIndicator()
	{
		this._hasPressedButton = false;
	}

	// Token: 0x04000B5C RID: 2908
	private HashSet<GorillaTriggerColliderHandIndicator> handIndicators = new HashSet<GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000B5D RID: 2909
	private bool _hasPressedButton;
}
