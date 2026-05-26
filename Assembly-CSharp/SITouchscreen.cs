using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class SITouchscreen : MonoBehaviour
{
	// Token: 0x060009C3 RID: 2499 RVA: 0x00034B4C File Offset: 0x00032D4C
	private void OnTriggerEnter(Collider other)
	{
		this.OnTriggerStay(other);
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00034B58 File Offset: 0x00032D58
	private void OnTriggerStay(Collider other)
	{
		Transform indicator = this.GetIndicator(other);
		if (indicator != null)
		{
			this.controllingTransform = indicator;
			this.lastTouched = Time.time;
		}
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00034B88 File Offset: 0x00032D88
	private void OnTriggerExit(Collider other)
	{
		if (this.controllingTransform == null || this.GetIndicator(other) != this.controllingTransform)
		{
			return;
		}
		this.controllingTransform = null;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00034BB4 File Offset: 0x00032DB4
	private Transform GetIndicator(Collider other)
	{
		if (this.notFingerTouchDict.Contains(other))
		{
			return null;
		}
		GorillaTriggerColliderHandIndicator componentInParent;
		if (!this.fingerTouchDict.TryGetValue(other, out componentInParent))
		{
			componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				this.notFingerTouchDict.Add(other);
				return null;
			}
			this.fingerTouchDict.Add(other, componentInParent);
		}
		return componentInParent.transform;
	}

	// Token: 0x04000BE6 RID: 3046
	public Transform controllingTransform;

	// Token: 0x04000BE7 RID: 3047
	public float lastTouched;

	// Token: 0x04000BE8 RID: 3048
	public Vector3 lastPosition;

	// Token: 0x04000BE9 RID: 3049
	private Dictionary<Collider, GorillaTriggerColliderHandIndicator> fingerTouchDict = new Dictionary<Collider, GorillaTriggerColliderHandIndicator>();

	// Token: 0x04000BEA RID: 3050
	private HashSet<Collider> notFingerTouchDict = new HashSet<Collider>();
}
