using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CD1 RID: 3281
public interface IFXEffectContextObject
{
	// Token: 0x17000796 RID: 1942
	// (get) Token: 0x06005174 RID: 20852
	List<int> PrefabPoolIds { get; }

	// Token: 0x17000797 RID: 1943
	// (get) Token: 0x06005175 RID: 20853
	Vector3 Position { get; }

	// Token: 0x17000798 RID: 1944
	// (get) Token: 0x06005176 RID: 20854
	Quaternion Rotation { get; }

	// Token: 0x17000799 RID: 1945
	// (get) Token: 0x06005177 RID: 20855
	AudioSource SoundSource { get; }

	// Token: 0x1700079A RID: 1946
	// (get) Token: 0x06005178 RID: 20856
	AudioClip Sound { get; }

	// Token: 0x1700079B RID: 1947
	// (get) Token: 0x06005179 RID: 20857
	float Volume { get; }

	// Token: 0x1700079C RID: 1948
	// (get) Token: 0x0600517A RID: 20858
	float Pitch { get; }

	// Token: 0x0600517B RID: 20859
	void OnTriggerActions();

	// Token: 0x0600517C RID: 20860
	void OnPlayVisualFX(int effectID, GameObject effect);

	// Token: 0x0600517D RID: 20861
	void OnPlaySoundFX(AudioSource audioSource);
}
