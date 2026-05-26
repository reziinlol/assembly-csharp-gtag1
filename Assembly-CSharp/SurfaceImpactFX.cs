using System;
using UnityEngine;

// Token: 0x02000DC5 RID: 3525
public class SurfaceImpactFX : MonoBehaviour
{
	// Token: 0x06005668 RID: 22120 RVA: 0x001C08C8 File Offset: 0x001BEAC8
	public void Awake()
	{
		if (this.particleFX == null)
		{
			this.particleFX = base.GetComponent<ParticleSystem>();
		}
		if (this.particleFX == null)
		{
			Debug.LogError("SurfaceImpactFX: No ParticleSystem found! Disabling component.", this);
			base.enabled = false;
			return;
		}
		this.fxMainModule = this.particleFX.main;
	}

	// Token: 0x06005669 RID: 22121 RVA: 0x001C0921 File Offset: 0x001BEB21
	public void SetScale(float scale)
	{
		this.fxMainModule.gravityModifierMultiplier = this.startingGravityModifier * scale;
		base.transform.localScale = this.startingScale * scale;
	}

	// Token: 0x0400664A RID: 26186
	public ParticleSystem particleFX;

	// Token: 0x0400664B RID: 26187
	public float startingGravityModifier;

	// Token: 0x0400664C RID: 26188
	public Vector3 startingScale = Vector3.one;

	// Token: 0x0400664D RID: 26189
	private ParticleSystem.MainModule fxMainModule;
}
