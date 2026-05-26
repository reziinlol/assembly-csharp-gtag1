using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
[Serializable]
public struct XSceneRef
{
	// Token: 0x06001703 RID: 5891 RVA: 0x00085594 File Offset: 0x00083794
	public bool TryResolve(out XSceneRefTarget result)
	{
		if (this.TargetID == 0)
		{
			result = null;
			return true;
		}
		if (this.didCache && this.cached != null)
		{
			result = this.cached;
			return true;
		}
		XSceneRefTarget xsceneRefTarget;
		if (!XSceneRefGlobalHub.TryResolve(this.TargetScene, this.TargetID, out xsceneRefTarget))
		{
			result = null;
			return false;
		}
		this.cached = xsceneRefTarget;
		this.didCache = true;
		result = xsceneRefTarget;
		return true;
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x000855FC File Offset: 0x000837FC
	public bool TryResolve(out GameObject result)
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? null : xsceneRefTarget.gameObject);
			return true;
		}
		result = null;
		return false;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00085630 File Offset: 0x00083830
	public bool TryResolve<T>(out T result) where T : Component
	{
		XSceneRefTarget xsceneRefTarget;
		if (this.TryResolve(out xsceneRefTarget))
		{
			result = ((xsceneRefTarget == null) ? default(T) : xsceneRefTarget.GetComponent<T>());
			return true;
		}
		result = default(T);
		return false;
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x00085671 File Offset: 0x00083871
	public void AddCallbackOnLoad(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneLoad(callback);
	}

	// Token: 0x06001707 RID: 5895 RVA: 0x0008567F File Offset: 0x0008387F
	public void RemoveCallbackOnLoad(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneLoad(callback);
	}

	// Token: 0x06001708 RID: 5896 RVA: 0x0008568D File Offset: 0x0008388D
	public void AddCallbackOnUnload(Action callback)
	{
		this.TargetScene.AddCallbackOnSceneUnload(callback);
	}

	// Token: 0x06001709 RID: 5897 RVA: 0x0008569B File Offset: 0x0008389B
	public void RemoveCallbackOnUnload(Action callback)
	{
		this.TargetScene.RemoveCallbackOnSceneUnload(callback);
	}

	// Token: 0x0400223F RID: 8767
	public SceneIndex TargetScene;

	// Token: 0x04002240 RID: 8768
	public int TargetID;

	// Token: 0x04002241 RID: 8769
	private XSceneRefTarget cached;

	// Token: 0x04002242 RID: 8770
	private bool didCache;
}
