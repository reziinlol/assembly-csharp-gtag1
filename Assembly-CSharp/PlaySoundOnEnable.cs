using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002AB RID: 683
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x060011BD RID: 4541 RVA: 0x0005F011 File Offset: 0x0005D211
	private void Reset()
	{
		this._source = base.GetComponent<AudioSource>();
		if (this._source)
		{
			this._source.playOnAwake = false;
		}
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0005F038 File Offset: 0x0005D238
	private void OnEnable()
	{
		this.Play();
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0005F040 File Offset: 0x0005D240
	private void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x0005F048 File Offset: 0x0005D248
	public void Play()
	{
		if (this._loop && this._clips.Length == 1 && this._loopDelay == Vector2.zero)
		{
			this._source.clip = this._clips[0];
			this._source.loop = true;
			this._source.GTPlay();
			return;
		}
		this._source.loop = false;
		if (this._loop)
		{
			base.StartCoroutine(this.DoLoop());
			return;
		}
		this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
		this._source.GTPlay();
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x0005F0F2 File Offset: 0x0005D2F2
	private IEnumerator DoLoop()
	{
		while (base.enabled)
		{
			this._source.clip = this._clips[Random.Range(0, this._clips.Length)];
			this._source.GTPlay();
			while (this._source.isPlaying)
			{
				yield return null;
			}
			float num = Random.Range(this._loopDelay.x, this._loopDelay.y);
			if (num > 0f)
			{
				float waitEndTime = Time.time + num;
				while (Time.time < waitEndTime)
				{
					yield return null;
				}
			}
		}
		yield break;
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0005F101 File Offset: 0x0005D301
	public void Stop()
	{
		this._source.GTStop();
		this._source.loop = false;
	}

	// Token: 0x0400154D RID: 5453
	[SerializeField]
	private AudioSource _source;

	// Token: 0x0400154E RID: 5454
	[SerializeField]
	private AudioClip[] _clips;

	// Token: 0x0400154F RID: 5455
	[SerializeField]
	private bool _loop;

	// Token: 0x04001550 RID: 5456
	[SerializeField]
	private Vector2 _loopDelay;
}
