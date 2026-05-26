using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class SIGameEntityStealthVisibility : MonoBehaviour
{
	// Token: 0x0600070F RID: 1807 RVA: 0x0002866A File Offset: 0x0002686A
	private void OnEnable()
	{
		this.revealRange = Mathf.Min(this.revealRange, this.hideRange);
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x00028683 File Offset: 0x00026883
	private void OnDisable()
	{
		this.SetVisibility(true);
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0002868C File Offset: 0x0002688C
	private void LateUpdate()
	{
		Vector3 position = GTPlayer.Instance.transform.position;
		float num = Vector3.SqrMagnitude(base.transform.position - position);
		if (this.isStealthed && num < this.revealRange * this.revealRange)
		{
			this.SetVisibility(true);
			return;
		}
		if (!this.isStealthed && num > this.hideRange * this.hideRange)
		{
			this.SetVisibility(false);
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x00028700 File Offset: 0x00026900
	private void SetVisibility(bool visible)
	{
		this.isStealthed = !visible;
		for (int i = 0; i < this.stealthedComponents.Length; i++)
		{
			this.stealthedComponents[i].enabled = visible;
		}
	}

	// Token: 0x040008EF RID: 2287
	[SerializeField]
	private Renderer[] stealthedComponents;

	// Token: 0x040008F0 RID: 2288
	[SerializeField]
	private float revealRange = 5f;

	// Token: 0x040008F1 RID: 2289
	[SerializeField]
	private float hideRange = 8f;

	// Token: 0x040008F2 RID: 2290
	private bool isStealthed;
}
