using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003C3 RID: 963
public class ZoneConditionalVisibility : MonoBehaviour
{
	// Token: 0x0600171F RID: 5919 RVA: 0x00085C2B File Offset: 0x00083E2B
	private void Awake()
	{
		if (this.renderersOnly)
		{
			this.renderers = new List<Renderer>(32);
			base.GetComponentsInChildren<Renderer>(false, this.renderers);
		}
	}

	// Token: 0x06001720 RID: 5920 RVA: 0x00085C4F File Offset: 0x00083E4F
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001721 RID: 5921 RVA: 0x00085C7D File Offset: 0x00083E7D
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001722 RID: 5922 RVA: 0x00085CA8 File Offset: 0x00083EA8
	private void OnZoneChanged()
	{
		bool flag = (this.zones == null || this.zones.Length == 0) ? ZoneManagement.IsInZone(this.zone) : this.InAnyZone();
		if (this.invisibleWhileLoaded)
		{
			if (this.renderersOnly)
			{
				for (int i = 0; i < this.renderers.Count; i++)
				{
					if (this.renderers[i] != null)
					{
						this.renderers[i].enabled = !flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(!flag);
			return;
		}
		else
		{
			if (this.renderersOnly)
			{
				for (int j = 0; j < this.renderers.Count; j++)
				{
					if (this.renderers[j] != null)
					{
						this.renderers[j].enabled = flag;
					}
				}
				return;
			}
			base.gameObject.SetActive(flag);
			return;
		}
	}

	// Token: 0x06001723 RID: 5923 RVA: 0x00085D8C File Offset: 0x00083F8C
	private bool InAnyZone()
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (ZoneManagement.IsInZone(this.zones[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002250 RID: 8784
	[SerializeField]
	private GTZone zone;

	// Token: 0x04002251 RID: 8785
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x04002252 RID: 8786
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04002253 RID: 8787
	[SerializeField]
	private bool renderersOnly;

	// Token: 0x04002254 RID: 8788
	private List<Renderer> renderers;
}
