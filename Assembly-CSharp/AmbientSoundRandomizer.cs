using System;
using UnityEngine;

// Token: 0x02000D1E RID: 3358
public class AmbientSoundRandomizer : MonoBehaviour
{
	// Token: 0x060052E9 RID: 21225 RVA: 0x001B2C3D File Offset: 0x001B0E3D
	private void Button_Cache()
	{
		this.audioSources = base.GetComponentsInChildren<AudioSource>();
	}

	// Token: 0x060052EA RID: 21226 RVA: 0x001B2C4B File Offset: 0x001B0E4B
	private void Awake()
	{
		this.SetTarget();
	}

	// Token: 0x060052EB RID: 21227 RVA: 0x001B2C54 File Offset: 0x001B0E54
	private void Update()
	{
		if (this.timer >= this.timerTarget)
		{
			int num = Random.Range(0, this.audioSources.Length);
			int num2 = Random.Range(0, this.audioClips.Length);
			this.audioSources[num].clip = this.audioClips[num2];
			this.audioSources[num].GTPlay();
			this.SetTarget();
			return;
		}
		this.timer += Time.deltaTime;
	}

	// Token: 0x060052EC RID: 21228 RVA: 0x001B2CC8 File Offset: 0x001B0EC8
	private void SetTarget()
	{
		this.timerTarget = this.baseTime + Random.Range(0f, this.randomModifier);
		this.timer = 0f;
	}

	// Token: 0x0400644E RID: 25678
	[SerializeField]
	private AudioSource[] audioSources;

	// Token: 0x0400644F RID: 25679
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x04006450 RID: 25680
	[SerializeField]
	private float baseTime = 15f;

	// Token: 0x04006451 RID: 25681
	[SerializeField]
	private float randomModifier = 5f;

	// Token: 0x04006452 RID: 25682
	private float timer;

	// Token: 0x04006453 RID: 25683
	private float timerTarget;
}
