using System;
using UnityEngine;

// Token: 0x02000585 RID: 1413
public class HotPepperFace : MonoBehaviour
{
	// Token: 0x060023C5 RID: 9157 RVA: 0x000C048F File Offset: 0x000BE68F
	public void PlayFX(float delay)
	{
		if (delay < 0f)
		{
			this.PlayFX();
			return;
		}
		base.Invoke("PlayFX", delay);
	}

	// Token: 0x060023C6 RID: 9158 RVA: 0x000C04AC File Offset: 0x000BE6AC
	public void PlayFX()
	{
		this._faceMesh.SetActive(true);
		this._thermalSourceVolume.SetActive(true);
		this._fireFX.Play();
		this._flameSpeaker.GTPlay();
		this._breathSpeaker.GTPlay();
		base.Invoke("StopFX", this._effectLength);
	}

	// Token: 0x060023C7 RID: 9159 RVA: 0x000C0503 File Offset: 0x000BE703
	public void StopFX()
	{
		this._faceMesh.SetActive(false);
		this._thermalSourceVolume.SetActive(false);
		this._fireFX.Stop();
		this._flameSpeaker.GTStop();
		this._breathSpeaker.GTStop();
	}

	// Token: 0x04002EE8 RID: 12008
	[SerializeField]
	private GameObject _faceMesh;

	// Token: 0x04002EE9 RID: 12009
	[SerializeField]
	private ParticleSystem _fireFX;

	// Token: 0x04002EEA RID: 12010
	[SerializeField]
	private AudioSource _flameSpeaker;

	// Token: 0x04002EEB RID: 12011
	[SerializeField]
	private AudioSource _breathSpeaker;

	// Token: 0x04002EEC RID: 12012
	[SerializeField]
	private float _effectLength = 1.5f;

	// Token: 0x04002EED RID: 12013
	[SerializeField]
	private GameObject _thermalSourceVolume;
}
