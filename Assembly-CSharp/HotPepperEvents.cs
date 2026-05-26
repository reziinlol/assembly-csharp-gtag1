using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000583 RID: 1411
public class HotPepperEvents : MonoBehaviour
{
	// Token: 0x060023BF RID: 9151 RVA: 0x000C03AE File Offset: 0x000BE5AE
	private void OnEnable()
	{
		this._pepper.onBiteWorld.AddListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.AddListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060023C0 RID: 9152 RVA: 0x000C03E8 File Offset: 0x000BE5E8
	private void OnDisable()
	{
		this._pepper.onBiteWorld.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteWorld));
		this._pepper.onBiteView.RemoveListener(new UnityAction<VRRig, int>(this.OnBiteView));
	}

	// Token: 0x060023C1 RID: 9153 RVA: 0x000C0422 File Offset: 0x000BE622
	public void OnBiteView(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, true);
	}

	// Token: 0x060023C2 RID: 9154 RVA: 0x000C042D File Offset: 0x000BE62D
	public void OnBiteWorld(VRRig rig, int nextState)
	{
		this.OnBite(rig, nextState, false);
	}

	// Token: 0x060023C3 RID: 9155 RVA: 0x000C0438 File Offset: 0x000BE638
	public void OnBite(VRRig rig, int nextState, bool isViewRig)
	{
		if (nextState != 8)
		{
			return;
		}
		GameObject gameObject = rig.cosmeticReferences.Get(this.m_targetEffectID);
		if (gameObject.IsNull())
		{
			return;
		}
		HotPepperFace component = gameObject.GetComponent<HotPepperFace>();
		if (component.IsNull())
		{
			return;
		}
		component.PlayFX(1f);
	}

	// Token: 0x04002EE1 RID: 12001
	[SerializeField]
	private EdibleHoldable _pepper;

	// Token: 0x04002EE2 RID: 12002
	[SerializeField]
	private CosmeticRefID m_targetEffectID = CosmeticRefID.HotPepperFaceEffect;

	// Token: 0x02000584 RID: 1412
	public enum EdibleState
	{
		// Token: 0x04002EE4 RID: 12004
		A = 1,
		// Token: 0x04002EE5 RID: 12005
		B,
		// Token: 0x04002EE6 RID: 12006
		C = 4,
		// Token: 0x04002EE7 RID: 12007
		D = 8
	}
}
