using System;
using System.Collections.Generic;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x020004F4 RID: 1268
[Serializable]
internal class HandEffectContext : IFXEffectContextObject
{
	// Token: 0x17000360 RID: 864
	// (get) Token: 0x06001FB3 RID: 8115 RVA: 0x000AB103 File Offset: 0x000A9303
	public List<int> PrefabPoolIds
	{
		get
		{
			return this.prefabHashes;
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x06001FB4 RID: 8116 RVA: 0x000AB10B File Offset: 0x000A930B
	public Vector3 Position
	{
		get
		{
			return this.position;
		}
	}

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x06001FB5 RID: 8117 RVA: 0x000AB113 File Offset: 0x000A9313
	public Quaternion Rotation
	{
		get
		{
			return this.rotation;
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000AB11B File Offset: 0x000A931B
	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	// Token: 0x17000364 RID: 868
	// (get) Token: 0x06001FB7 RID: 8119 RVA: 0x000AB123 File Offset: 0x000A9323
	public Color Color
	{
		get
		{
			return this.color;
		}
	}

	// Token: 0x17000365 RID: 869
	// (get) Token: 0x06001FB8 RID: 8120 RVA: 0x000AB12B File Offset: 0x000A932B
	public AudioSource SoundSource
	{
		get
		{
			return this.handSoundSource;
		}
	}

	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06001FB9 RID: 8121 RVA: 0x000AB133 File Offset: 0x000A9333
	public AudioClip Sound
	{
		get
		{
			return this.soundFX;
		}
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06001FBA RID: 8122 RVA: 0x000AB13B File Offset: 0x000A933B
	public float Volume
	{
		get
		{
			return this.soundVolume;
		}
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x06001FBB RID: 8123 RVA: 0x000AB143 File Offset: 0x000A9343
	public float Pitch
	{
		get
		{
			return this.soundPitch;
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000AB14B File Offset: 0x000A934B
	public void AddFXPrefab(int hash)
	{
		this.prefabHashes.Add(hash);
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000AB15C File Offset: 0x000A935C
	public void RemoveFXPrefab(int hash)
	{
		int num = this.prefabHashes.IndexOf(hash, 2);
		if (num >= 2)
		{
			this.prefabHashes.RemoveAt(num);
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06001FBE RID: 8126 RVA: 0x000AB187 File Offset: 0x000A9387
	// (set) Token: 0x06001FBF RID: 8127 RVA: 0x000AB192 File Offset: 0x000A9392
	public bool SeparateUpTapCooldown
	{
		get
		{
			return this.separateUpTapCooldownCount > 0;
		}
		set
		{
			this.separateUpTapCooldownCount = Mathf.Max(this.separateUpTapCooldownCount + (value ? 1 : -1), 0);
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06001FC0 RID: 8128 RVA: 0x000AB1AE File Offset: 0x000A93AE
	// (set) Token: 0x06001FC1 RID: 8129 RVA: 0x000AB1C0 File Offset: 0x000A93C0
	public HandTapOverrides DownTapOverrides
	{
		get
		{
			return this.downTapOverrides ?? this.defaultDownTapOverrides;
		}
		set
		{
			this.downTapOverrides = value;
		}
	}

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06001FC2 RID: 8130 RVA: 0x000AB1C9 File Offset: 0x000A93C9
	// (set) Token: 0x06001FC3 RID: 8131 RVA: 0x000AB1DB File Offset: 0x000A93DB
	public HandTapOverrides UpTapOverrides
	{
		get
		{
			return this.upTapOverrides ?? this.defaultUpTapOverrides;
		}
		set
		{
			this.upTapOverrides = value;
		}
	}

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x06001FC4 RID: 8132 RVA: 0x000AB1E4 File Offset: 0x000A93E4
	// (remove) Token: 0x06001FC5 RID: 8133 RVA: 0x000AB21C File Offset: 0x000A941C
	public event Action<HandEffectContext> handTapDown;

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x06001FC6 RID: 8134 RVA: 0x000AB254 File Offset: 0x000A9454
	// (remove) Token: 0x06001FC7 RID: 8135 RVA: 0x000AB28C File Offset: 0x000A948C
	public event Action<HandEffectContext> handTapUp;

	// Token: 0x06001FC8 RID: 8136 RVA: 0x000AB2C1 File Offset: 0x000A94C1
	public void OnTriggerActions()
	{
		if (this.isDownTap)
		{
			Action<HandEffectContext> action = this.handTapDown;
			if (action == null)
			{
				return;
			}
			action(this);
			return;
		}
		else
		{
			Action<HandEffectContext> action2 = this.handTapUp;
			if (action2 == null)
			{
				return;
			}
			action2(this);
			return;
		}
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000AB2F0 File Offset: 0x000A94F0
	public void OnPlayVisualFX(int fxID, GameObject fx)
	{
		FXModifier fxmodifier;
		if (fx.TryGetComponent<FXModifier>(out fxmodifier))
		{
			fxmodifier.UpdateScale(this.soundVolume * ((fxID == GorillaAmbushManager.HandEffectHash) ? GorillaAmbushManager.HandFXScaleModifier : 1f), this.color);
		}
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnPlaySoundFX(AudioSource audioSource)
	{
	}

	// Token: 0x04002A78 RID: 10872
	internal List<int> prefabHashes = new List<int>
	{
		-1,
		-1
	};

	// Token: 0x04002A79 RID: 10873
	internal Vector3 position;

	// Token: 0x04002A7A RID: 10874
	internal Quaternion rotation;

	// Token: 0x04002A7B RID: 10875
	internal float speed;

	// Token: 0x04002A7C RID: 10876
	internal Color color = Color.white;

	// Token: 0x04002A7D RID: 10877
	[SerializeField]
	internal AudioSource handSoundSource;

	// Token: 0x04002A7E RID: 10878
	internal AudioClip soundFX;

	// Token: 0x04002A7F RID: 10879
	internal float soundVolume;

	// Token: 0x04002A80 RID: 10880
	internal float soundPitch;

	// Token: 0x04002A81 RID: 10881
	internal int separateUpTapCooldownCount;

	// Token: 0x04002A82 RID: 10882
	[SerializeField]
	internal HandTapOverrides defaultDownTapOverrides;

	// Token: 0x04002A83 RID: 10883
	internal HandTapOverrides downTapOverrides;

	// Token: 0x04002A84 RID: 10884
	[SerializeField]
	internal HandTapOverrides defaultUpTapOverrides;

	// Token: 0x04002A85 RID: 10885
	internal HandTapOverrides upTapOverrides;

	// Token: 0x04002A88 RID: 10888
	internal bool isDownTap;

	// Token: 0x04002A89 RID: 10889
	internal bool isLeftHand;
}
