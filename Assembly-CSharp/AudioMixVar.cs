using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020005E5 RID: 1509
[Serializable]
public class AudioMixVar
{
	// Token: 0x170003EC RID: 1004
	// (get) Token: 0x06002582 RID: 9602 RVA: 0x000C69EC File Offset: 0x000C4BEC
	// (set) Token: 0x06002583 RID: 9603 RVA: 0x000C6A3B File Offset: 0x000C4C3B
	public float value
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			if (!this.mixer)
			{
				return 0f;
			}
			float result;
			if (!this.mixer.GetFloat(this.name, out result))
			{
				return 0f;
			}
			return result;
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.name, value);
			}
		}
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000C6A5D File Offset: 0x000C4C5D
	public void ReturnToPool()
	{
		if (this._pool != null)
		{
			this._pool.Return(this);
		}
	}

	// Token: 0x040030F5 RID: 12533
	public AudioMixerGroup group;

	// Token: 0x040030F6 RID: 12534
	public AudioMixer mixer;

	// Token: 0x040030F7 RID: 12535
	public string name;

	// Token: 0x040030F8 RID: 12536
	[NonSerialized]
	public bool taken;

	// Token: 0x040030F9 RID: 12537
	[SerializeField]
	private AudioMixVarPool _pool;
}
