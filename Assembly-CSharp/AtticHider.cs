using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005E2 RID: 1506
public class AtticHider : MonoBehaviour
{
	// Token: 0x06002574 RID: 9588 RVA: 0x000C67C1 File Offset: 0x000C49C1
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x000C67EF File Offset: 0x000C49EF
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000C6818 File Offset: 0x000C4A18
	private void OnZoneChanged()
	{
		if (this.AtticRenderer == null)
		{
			return;
		}
		if (ZoneManagement.instance.IsZoneActive(GTZone.attic))
		{
			if (this._coroutine != null)
			{
				base.StopCoroutine(this._coroutine);
				this._coroutine = null;
			}
			this._coroutine = base.StartCoroutine(this.WaitForAtticLoad());
			return;
		}
		if (this._coroutine != null)
		{
			base.StopCoroutine(this._coroutine);
			this._coroutine = null;
		}
		this.AtticRenderer.enabled = true;
	}

	// Token: 0x06002577 RID: 9591 RVA: 0x000C6897 File Offset: 0x000C4A97
	private IEnumerator WaitForAtticLoad()
	{
		while (!ZoneManagement.instance.IsSceneLoaded(GTZone.attic))
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
		this.AtticRenderer.enabled = false;
		this._coroutine = null;
		yield break;
	}

	// Token: 0x040030EF RID: 12527
	[SerializeField]
	private MeshRenderer AtticRenderer;

	// Token: 0x040030F0 RID: 12528
	private Coroutine _coroutine;
}
