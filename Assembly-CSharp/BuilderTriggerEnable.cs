using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200065B RID: 1627
public class BuilderTriggerEnable : MonoBehaviour
{
	// Token: 0x06002888 RID: 10376 RVA: 0x000DC2E0 File Offset: 0x000DA4E0
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null || component.OwningNetPlayer == null)
		{
			return;
		}
		if (!component.OwningNetPlayer.IsLocal)
		{
			return;
		}
		if (this.activateOnEnter != null)
		{
			for (int i = 0; i < this.activateOnEnter.Count; i++)
			{
				if (this.activateOnEnter[i] != null)
				{
					this.activateOnEnter[i].SetActive(true);
				}
			}
		}
		if (this.deactivateOnEnter != null)
		{
			for (int j = 0; j < this.deactivateOnEnter.Count; j++)
			{
				if (this.deactivateOnEnter[j] != null)
				{
					this.deactivateOnEnter[j].SetActive(false);
				}
			}
		}
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000DC3B8 File Offset: 0x000DA5B8
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody == null)
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null || component.OwningNetPlayer == null)
		{
			return;
		}
		if (!component.OwningNetPlayer.IsLocal)
		{
			return;
		}
		if (this.activateOnExit != null)
		{
			for (int i = 0; i < this.activateOnExit.Count; i++)
			{
				if (this.activateOnExit[i] != null)
				{
					this.activateOnExit[i].SetActive(true);
				}
			}
		}
		if (this.deactivateOnExit != null)
		{
			for (int j = 0; j < this.deactivateOnExit.Count; j++)
			{
				if (this.deactivateOnExit[j] != null)
				{
					this.deactivateOnExit[j].SetActive(false);
				}
			}
		}
	}

	// Token: 0x040034DB RID: 13531
	public List<GameObject> activateOnEnter;

	// Token: 0x040034DC RID: 13532
	public List<GameObject> deactivateOnEnter;

	// Token: 0x040034DD RID: 13533
	public List<GameObject> activateOnExit;

	// Token: 0x040034DE RID: 13534
	public List<GameObject> deactivateOnExit;
}
