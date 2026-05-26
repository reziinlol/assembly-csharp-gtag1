using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000613 RID: 1555
public class BuilderZoneRenderers : MonoBehaviour
{
	// Token: 0x060026AD RID: 9901 RVA: 0x000CC974 File Offset: 0x000CAB74
	private void Start()
	{
		this.allRenderers.Clear();
		this.allRenderers.AddRange(this.renderers);
		foreach (GameObject gameObject in this.rootObjects)
		{
			this.allRenderers.AddRange(gameObject.GetComponentsInChildren<Renderer>(true));
		}
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000CCA24 File Offset: 0x000CAC24
	private void OnDestroy()
	{
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000CCA5C File Offset: 0x000CAC5C
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
		if (flag && !this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer in this.allRenderers)
			{
				renderer.enabled = true;
			}
			using (List<Canvas>.Enumerator enumerator2 = this.canvases.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Canvas canvas = enumerator2.Current;
					canvas.enabled = true;
				}
				return;
			}
		}
		if (!flag && this.inBuilderZone)
		{
			this.inBuilderZone = flag;
			foreach (Renderer renderer2 in this.allRenderers)
			{
				renderer2.enabled = false;
			}
			foreach (Canvas canvas2 in this.canvases)
			{
				canvas2.enabled = false;
			}
		}
	}

	// Token: 0x0400322A RID: 12842
	public List<Renderer> renderers;

	// Token: 0x0400322B RID: 12843
	public List<Canvas> canvases;

	// Token: 0x0400322C RID: 12844
	public List<GameObject> rootObjects;

	// Token: 0x0400322D RID: 12845
	private bool inBuilderZone;

	// Token: 0x0400322E RID: 12846
	private List<Renderer> allRenderers = new List<Renderer>(200);
}
