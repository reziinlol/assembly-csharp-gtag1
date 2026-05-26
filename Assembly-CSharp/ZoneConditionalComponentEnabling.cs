using System;
using UnityEngine;

// Token: 0x020003C1 RID: 961
public class ZoneConditionalComponentEnabling : MonoBehaviour
{
	// Token: 0x06001717 RID: 5911 RVA: 0x00085A23 File Offset: 0x00083C23
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001718 RID: 5912 RVA: 0x00085A51 File Offset: 0x00083C51
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001719 RID: 5913 RVA: 0x00085A7C File Offset: 0x00083C7C
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.IsInZone(this.zone);
		bool enabled = this.invisibleWhileLoaded ? (!flag) : flag;
		if (this.components != null)
		{
			for (int i = 0; i < this.components.Length; i++)
			{
				if (this.components[i] != null)
				{
					this.components[i].enabled = enabled;
				}
			}
		}
		if (this.m_renderers != null)
		{
			for (int j = 0; j < this.m_renderers.Length; j++)
			{
				if (this.m_renderers[j] != null)
				{
					this.m_renderers[j].enabled = enabled;
				}
			}
		}
		if (this.m_colliders != null)
		{
			for (int k = 0; k < this.m_colliders.Length; k++)
			{
				if (this.m_colliders[k] != null)
				{
					this.m_colliders[k].enabled = enabled;
				}
			}
		}
	}

	// Token: 0x04002248 RID: 8776
	[SerializeField]
	private GTZone zone;

	// Token: 0x04002249 RID: 8777
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x0400224A RID: 8778
	[SerializeField]
	private Behaviour[] components;

	// Token: 0x0400224B RID: 8779
	[SerializeField]
	private Renderer[] m_renderers;

	// Token: 0x0400224C RID: 8780
	[SerializeField]
	private Collider[] m_colliders;
}
