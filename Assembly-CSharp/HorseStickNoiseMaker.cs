using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class HorseStickNoiseMaker : MonoBehaviour
{
	// Token: 0x06000DF3 RID: 3571 RVA: 0x0004C4F4 File Offset: 0x0004A6F4
	protected void OnEnable()
	{
		if (!this.gorillaPlayerXform && !base.transform.TryFindByPath(this.gorillaPlayerXform_path, out this.gorillaPlayerXform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"HorseStickNoiseMaker: DEACTIVATING! Could not find gorillaPlayerXform using path: \"",
				this.gorillaPlayerXform_path,
				"\"\nThis component's transform path: \"",
				base.transform.GetPath(),
				"\""
			}));
			base.gameObject.SetActive(false);
			return;
		}
		this.oldPos = this.gorillaPlayerXform.position;
		this.distElapsed = 0f;
		this.timeSincePlay = 0f;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x0004C59C File Offset: 0x0004A79C
	protected void LateUpdate()
	{
		Vector3 position = this.gorillaPlayerXform.position;
		Vector3 vector = position - this.oldPos;
		this.distElapsed += vector.magnitude;
		this.timeSincePlay += Time.deltaTime;
		this.oldPos = position;
		if (this.distElapsed >= this.metersPerClip && this.timeSincePlay >= this.minSecBetweenClips)
		{
			this.soundBankPlayer.Play();
			this.distElapsed = 0f;
			this.timeSincePlay = 0f;
			if (this.particleFX != null)
			{
				this.particleFX.Play();
			}
		}
	}

	// Token: 0x040010B8 RID: 4280
	[Tooltip("Meters the object should traverse between playing a provided audio clip.")]
	public float metersPerClip = 4f;

	// Token: 0x040010B9 RID: 4281
	[Tooltip("Number of seconds that must elapse before playing another audio clip.")]
	public float minSecBetweenClips = 1.5f;

	// Token: 0x040010BA RID: 4282
	public SoundBankPlayer soundBankPlayer;

	// Token: 0x040010BB RID: 4283
	[Tooltip("Transform assigned in Gorilla Player Networked Prefab to the Gorilla Player Networked parent to keep track of distance traveled.")]
	public Transform gorillaPlayerXform;

	// Token: 0x040010BC RID: 4284
	[Delayed]
	public string gorillaPlayerXform_path;

	// Token: 0x040010BD RID: 4285
	[Tooltip("Optional particle FX to spawn when sound plays")]
	public ParticleSystem particleFX;

	// Token: 0x040010BE RID: 4286
	private Vector3 oldPos;

	// Token: 0x040010BF RID: 4287
	private float timeSincePlay;

	// Token: 0x040010C0 RID: 4288
	private float distElapsed;
}
