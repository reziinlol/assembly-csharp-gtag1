using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200003A RID: 58
public class ColliderOffsetOverride : MonoBehaviour
{
	// Token: 0x060000EF RID: 239 RVA: 0x00005E60 File Offset: 0x00004060
	private void Awake()
	{
		if (this.autoSearch)
		{
			this.FindColliders();
		}
		foreach (Collider collider in this.colliders)
		{
			if (collider != null)
			{
				collider.contactOffset = 0.01f * this.targetScale;
			}
		}
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00005ED8 File Offset: 0x000040D8
	public void FindColliders()
	{
		foreach (Collider item in base.gameObject.GetComponents<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00005F48 File Offset: 0x00004148
	public void FindCollidersRecursively()
	{
		foreach (Collider item in base.gameObject.GetComponentsInChildren<Collider>().ToList<Collider>())
		{
			if (!this.colliders.Contains(item))
			{
				this.colliders.Add(item);
			}
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x00005FB8 File Offset: 0x000041B8
	private void AutoDisabled()
	{
		this.autoSearch = true;
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00005FC1 File Offset: 0x000041C1
	private void AutoEnabled()
	{
		this.autoSearch = false;
	}

	// Token: 0x040000FF RID: 255
	public List<Collider> colliders;

	// Token: 0x04000100 RID: 256
	[HideInInspector]
	public bool autoSearch;

	// Token: 0x04000101 RID: 257
	public float targetScale = 1f;
}
