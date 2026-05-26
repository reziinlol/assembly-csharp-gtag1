using System;
using UnityEngine;

// Token: 0x02000398 RID: 920
public class SoundOnCollisionTagSpecific : MonoBehaviour
{
	// Token: 0x06001653 RID: 5715 RVA: 0x000817E4 File Offset: 0x0007F9E4
	private void OnTriggerEnter(Collider collider)
	{
		if (Time.time > this.nextSound && collider.gameObject.CompareTag(this.tagName))
		{
			this.nextSound = Time.time + this.noiseCooldown;
			this.audioSource.GTPlayOneShot(this.collisionSounds[Random.Range(0, this.collisionSounds.Length)], 0.5f);
		}
	}

	// Token: 0x0400206C RID: 8300
	public string tagName;

	// Token: 0x0400206D RID: 8301
	public float noiseCooldown = 1f;

	// Token: 0x0400206E RID: 8302
	private float nextSound;

	// Token: 0x0400206F RID: 8303
	public AudioSource audioSource;

	// Token: 0x04002070 RID: 8304
	public AudioClip[] collisionSounds;
}
