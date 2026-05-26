using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057B RID: 1403
public class BubbleGumEvents : MonoBehaviour
{
	// Token: 0x06002399 RID: 9113 RVA: 0x000BFCC5 File Offset: 0x000BDEC5
	private void OnEnable()
	{
		this._edible.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x0600239A RID: 9114 RVA: 0x000BFCFF File Offset: 0x000BDEFF
	private void OnDisable()
	{
		this._edible.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._edible.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x0600239B RID: 9115 RVA: 0x000BFD39 File Offset: 0x000BDF39
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x0600239C RID: 9116 RVA: 0x000BFD44 File Offset: 0x000BDF44
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x0600239D RID: 9117 RVA: 0x000BFD50 File Offset: 0x000BDF50
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		GorillaTagger instance = GorillaTagger.Instance;
		GameObject gameObject = null;
		if (isViewRig && instance != null)
		{
			gameObject = instance.gameObject;
		}
		else if (!isViewRig)
		{
			gameObject = rig.gameObject;
		}
		if (!BubbleGumEvents.gTargetCache.TryGetValue(gameObject, out this._bubble))
		{
			this._bubble = gameObject.GetComponentsInChildren<GumBubble>(true).FirstOrDefault((GumBubble g) => g.transform.parent.name == "$gum");
			if (isViewRig)
			{
				this._bubble.audioSource = instance.offlineVRRig.tagSound;
				this._bubble.targetScale = Vector3.one * 1.36f;
			}
			else
			{
				this._bubble.audioSource = rig.tagSound;
				this._bubble.targetScale = Vector3.one * 2f;
			}
			BubbleGumEvents.gTargetCache.Add(gameObject, this._bubble);
		}
		GumBubble bubble = this._bubble;
		if (bubble != null)
		{
			bubble.transform.parent.gameObject.SetActive(true);
		}
		GumBubble bubble2 = this._bubble;
		if (bubble2 == null)
		{
			return;
		}
		bubble2.InflateDelayed();
	}

	// Token: 0x04002EBE RID: 11966
	[SerializeField]
	private EdibleHoldable _edible;

	// Token: 0x04002EBF RID: 11967
	[SerializeField]
	private GumBubble _bubble;

	// Token: 0x04002EC0 RID: 11968
	private static Dictionary<GameObject, GumBubble> gTargetCache = new Dictionary<GameObject, GumBubble>(16);

	// Token: 0x0200057C RID: 1404
	public enum EdibleState
	{
		// Token: 0x04002EC2 RID: 11970
		A = 1,
		// Token: 0x04002EC3 RID: 11971
		B,
		// Token: 0x04002EC4 RID: 11972
		C = 4,
		// Token: 0x04002EC5 RID: 11973
		D = 8
	}
}
