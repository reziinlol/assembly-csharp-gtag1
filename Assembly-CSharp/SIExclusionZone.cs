using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class SIExclusionZone : MonoBehaviour
{
	// Token: 0x060007DC RID: 2012 RVA: 0x0002AE50 File Offset: 0x00029050
	private void OnDisable()
	{
		foreach (SIGadget sigadget in this.gadgetsInZone)
		{
			if (sigadget != null)
			{
				sigadget.LeaveExclusionZone(this);
			}
		}
		this.gadgetsInZone.Clear();
		if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
		{
			foreach (SIPlayer siplayer in this.playersInZone)
			{
				if (siplayer != null)
				{
					siplayer.exclusionZoneCount--;
				}
			}
		}
		this.playersInZone.Clear();
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002AF20 File Offset: 0x00029120
	private void OnTriggerEnter(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null)
		{
			if (!this.gadgetsInZone.Contains(componentInParent))
			{
				this.gadgetsInZone.Add(componentInParent);
			}
			componentInParent.ApplyExclusionZone(this);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && !this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Add(componentInParent2);
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount++;
			}
		}
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002AFA0 File Offset: 0x000291A0
	private void OnTriggerExit(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null && this.gadgetsInZone.Contains(componentInParent))
		{
			componentInParent.LeaveExclusionZone(this);
			this.gadgetsInZone.Remove(componentInParent);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Remove(componentInParent2);
			if ((this.exclusionType & SIExclusionType.AffectsOthers) != (SIExclusionType)0)
			{
				componentInParent2.exclusionZoneCount--;
			}
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002B022 File Offset: 0x00029222
	public void ClearGadget(SIGadget gadget)
	{
		this.gadgetsInZone.Remove(gadget);
	}

	// Token: 0x040009F5 RID: 2549
	public SIExclusionType exclusionType = SIExclusionType.AffectsOthers;

	// Token: 0x040009F6 RID: 2550
	private List<SIGadget> gadgetsInZone = new List<SIGadget>();

	// Token: 0x040009F7 RID: 2551
	private List<SIPlayer> playersInZone = new List<SIPlayer>();
}
