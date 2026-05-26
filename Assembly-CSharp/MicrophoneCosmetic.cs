using System;
using UnityEngine;

// Token: 0x02000227 RID: 551
public class MicrophoneCosmetic : MonoBehaviour
{
	// Token: 0x06000E93 RID: 3731 RVA: 0x0004F398 File Offset: 0x0004D598
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0)
		{
			this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 16000);
		}
		else
		{
			int sampleRate = AudioSettings.GetConfiguration().sampleRate;
			this.audioSource.clip = Microphone.Start(null, true, 10, sampleRate);
		}
		this.audioSource.loop = true;
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0004F418 File Offset: 0x0004D618
	private void OnEnable()
	{
		int num = (Application.platform == RuntimePlatform.Android && Microphone.devices.Length != 0) ? Microphone.GetPosition(Microphone.devices[0]) : Microphone.GetPosition(null);
		num -= 10;
		if ((float)num < 0f)
		{
			num = this.audioSource.clip.samples + num - 1;
		}
		this.audioSource.GTPlay();
		this.audioSource.timeSamples = num;
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0004F485 File Offset: 0x0004D685
	private void OnDisable()
	{
		this.audioSource.GTStop();
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0004F494 File Offset: 0x0004D694
	private void Update()
	{
		Vector3 vector = this.mouthTransform.position - base.transform.position;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude < this.mouthProximityRampRange.x * this.mouthProximityRampRange.x)
		{
			float magnitude = vector.magnitude;
			num = Mathf.InverseLerp(this.mouthProximityRampRange.x, this.mouthProximityRampRange.y, magnitude);
		}
		if (num != this.audioSource.volume)
		{
			this.audioSource.volume = num;
		}
		int num2 = this.audioSource.timeSamples -= 10;
		if ((float)num2 < 0f)
		{
			num2 = this.audioSource.clip.samples + num2 - 1;
		}
		this.audioSource.clip.SetData(this.zero, num2);
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnAudioFilterRead(float[] data, int channels)
	{
	}

	// Token: 0x04001177 RID: 4471
	[SerializeField]
	private Transform mouthTransform;

	// Token: 0x04001178 RID: 4472
	[SerializeField]
	private Vector2 mouthProximityRampRange = new Vector2(0.6f, 0.3f);

	// Token: 0x04001179 RID: 4473
	private AudioSource audioSource;

	// Token: 0x0400117A RID: 4474
	private float[] zero = new float[1];
}
