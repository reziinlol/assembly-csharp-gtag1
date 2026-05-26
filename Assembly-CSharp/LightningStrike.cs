using System;
using UnityEngine;

// Token: 0x02000DCE RID: 3534
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class LightningStrike : MonoBehaviour
{
	// Token: 0x06005690 RID: 22160 RVA: 0x001C1010 File Offset: 0x001BF210
	private void Initialize()
	{
		this.ps = base.GetComponent<ParticleSystem>();
		this.psMain = this.ps.main;
		this.psMain.playOnAwake = true;
		this.psMain.stopAction = ParticleSystemStopAction.Disable;
		this.psShape = this.ps.shape;
		this.psTrails = this.ps.trails;
		this.audioSource = base.GetComponent<AudioSource>();
		this.audioSource.playOnAwake = true;
	}

	// Token: 0x06005691 RID: 22161 RVA: 0x001C108C File Offset: 0x001BF28C
	public void Play(Vector3 p1, Vector3 p2, float beamWidthMultiplier, float audioVolume, float duration, Gradient colorOverLifetime)
	{
		if (this.ps == null)
		{
			this.Initialize();
		}
		base.transform.position = p1;
		base.transform.rotation = Quaternion.LookRotation(p1 - p2);
		this.psShape.radius = Vector3.Distance(p1, p2) * 0.5f;
		this.psShape.position = new Vector3(0f, 0f, -this.psShape.radius);
		this.psShape.randomPositionAmount = Mathf.Clamp(this.psShape.radius / 50f, 0f, 1f);
		this.psTrails.widthOverTrail = new ParticleSystem.MinMaxCurve(beamWidthMultiplier * 0.1f, beamWidthMultiplier);
		this.psTrails.colorOverLifetime = colorOverLifetime;
		this.psMain.duration = duration;
		this.audioSource.volume = Mathf.Clamp(this.psShape.radius / 5f, 0f, 1f) * audioVolume;
		base.gameObject.SetActive(true);
	}

	// Token: 0x0400666E RID: 26222
	public static SRand rand = new SRand("LightningStrike");

	// Token: 0x0400666F RID: 26223
	private ParticleSystem ps;

	// Token: 0x04006670 RID: 26224
	private ParticleSystem.MainModule psMain;

	// Token: 0x04006671 RID: 26225
	private ParticleSystem.ShapeModule psShape;

	// Token: 0x04006672 RID: 26226
	private ParticleSystem.TrailModule psTrails;

	// Token: 0x04006673 RID: 26227
	private AudioSource audioSource;
}
